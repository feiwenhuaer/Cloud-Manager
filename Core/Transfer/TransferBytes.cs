using Cloud.Dropbox;
using Cloud.GoogleDrive;
using Cloud.MegaNz;
using Core.Cloud;
using Newtonsoft.Json;
using SupDataDll;
using SupDataDll.Class.Mega;
using SupDataDll.Crypt;
using System;
using System.IO;
using System.Linq;

namespace Core.Transfer
{
    class TransferBytes
    {
        ItemsTransferManager GroupManager;
        public TransferItem item { get; private set; }

        //mega
        int[] chunksSizesToUploadMega;
        int indexPosMega = 0;
        MegaUpload mega_up;

        object clientTo;
        public TransferBytes(TransferItem item, ItemsTransferManager GroupManager, object clientTo = null)
        {
            if (item.From.node.Info.Size == 0) { item.ErrorMsg = "File size zero."; Dispose(); }
            this.item = item;
            this.GroupManager = GroupManager;
            if (item.buffer == null) item.buffer = new byte[128 * 1024];
            if (clientTo != null) this.clientTo = clientTo;
            if (item.To.node.GetRoot().RootInfo.Type == CloudType.Mega) InitUploadMega();//InitUploadMega
            //Make Stream To
            if (item.ChunkUploadSize > 0) MakeNextChunkUploadInStreamTo(true);
            else this.item.To.stream = AppSetting.ManageCloud.GetFileStream(item.To.node, item.SizeWasTransfer);//download to disk
            //begin transfer
            item.status = StatusTransfer.Running;
            item.From.stream.BeginRead(item.buffer, 0, item.buffer.Length, new AsyncCallback(GetFrom), 0);
        }

        void GetFrom(IAsyncResult result)
        {
            try
            {
                #region Get byte[]
                item.byteread = item.From.stream.EndRead(result);
                if (item.ChunkUploadSize < 0 && item.To.stream.CanSeek) item.To.stream.Seek(item.SizeWasTransfer, SeekOrigin.Begin);//if download to disk then seek.
                item.To.stream.Write(item.buffer, 0, item.byteread);
                item.SizeWasTransfer += item.byteread;
                #endregion

                #region transfer done/force stop.
                if (item.SizeWasTransfer == item.From.node.Info.Size || item.status != StatusTransfer.Running || GroupManager.GroupData.status != StatusTransfer.Running)//transfer done/force stop.
                {
                    if (item.ChunkUploadSize < 0 || item.SizeWasTransfer == item.From.node.Info.Size)//save last pos if download or done
                    {
                        if (this.item.From.node.GetRoot().RootInfo.Type == CloudType.Mega) SaveEncryptDataDownloadMega();
                        item.SaveSizeTransferSuccess = item.SizeWasTransfer;
                    }
                    try { item.From.stream.Close(); } catch { }//close stream if can
                    try { item.To.stream.Close(); } catch { }//close stream if can

                    switch (GroupManager.GroupData.status)
                    {
                        case StatusTransfer.Waiting: item.status = StatusTransfer.Waiting;break;
                        case StatusTransfer.Running: break;
                        default: item.status = StatusTransfer.Stop; break;
                    }
                    if(item.status == StatusTransfer.Remove) GroupManager.GroupData.items.Remove(item);
                    else if (item.status == StatusTransfer.Running && GroupManager.GroupData.status == StatusTransfer.Running)
                    {
                        item.status = StatusTransfer.Done;
                        if (item.To.node.GetRoot().RootInfo.Type == CloudType.Dropbox) if (!SaveUploadDropbox()) item.status = StatusTransfer.Error;
                    }
                    else item.status = StatusTransfer.Stop;
                    Dispose();
                    return;
                }
                #endregion

                #region initial next byte[]
                if (item.ChunkUploadSize > 0)//if upload
                {
                    int totalchunkupload = (int)result.AsyncState;
                    totalchunkupload += item.byteread;
                    if (totalchunkupload == item.ChunkUploadSize) { indexPosMega++; MakeNextChunkUploadInStreamTo(); totalchunkupload = 0; }//make new stream of next chunk and set totalchunkupload=0
                    int nexbyteread = Math.Min(item.ChunkUploadSize - totalchunkupload, item.buffer.Length); //item.ChunkUploadSize - totalchunkupload >= item.buffer.Length ? item.buffer.Length : item.ChunkUploadSize - totalchunkupload;//1<= nexbyteread = (chunksize - totalupload) <= buffer Length
                    item.From.stream.BeginRead(item.buffer, 0, nexbyteread, new AsyncCallback(GetFrom), totalchunkupload);
                }
                else//if download
                {
                    if (this.item.From.node.GetRoot().RootInfo.Type == CloudType.Mega) SaveEncryptDataDownloadMega();
                    item.SaveSizeTransferSuccess = item.SizeWasTransfer;
                    item.From.stream.BeginRead(item.buffer, 0, item.buffer.Length, new AsyncCallback(GetFrom), 0);
                }
                #endregion
            }
            catch (Exception ex)
            {
                if (AppSetting.TransferManager.status == StatusUpDownApp.StopForClosingApp || AppSetting.TransferManager.status == StatusUpDownApp.SavingData) return;
                item.ErrorMsg = ex.Message;
                item.status = StatusTransfer.Error;
                Dispose();
            }
        }

        void MakeNextChunkUploadInStreamTo(bool CreateNew = false)
        {
            if (item.ChunkUploadSize <= 0) throw new Exception("Not upload type");
            long pos_end = item.SizeWasTransfer + item.ChunkUploadSize - 1;
            if (pos_end >= item.From.node.Info.Size) pos_end = item.From.node.Info.Size - 1;

            switch (item.To.node.GetRoot().RootInfo.Type)
            {
                case CloudType.Dropbox:
                    if (!CreateNew) ((DropboxRequestAPIv2)clientTo).GetResponse_upload_session_append();//get data return from server
                    item.To.stream = ((DropboxRequestAPIv2)clientTo).upload_session_append(item.UploadID, pos_end - item.SizeWasTransfer + 1, item.SizeWasTransfer);
                    break;

                case CloudType.GoogleDrive:
                    if (!CreateNew) ((DriveAPIHttprequestv2)clientTo).GetResponse_Files_insert_resumable();//get data return from server
                    item.To.stream = ((DriveAPIHttprequestv2)clientTo).Files_insert_resumable(item.UploadID, item.SizeWasTransfer, pos_end, item.From.node.Info.Size);
                    break;
                case CloudType.Mega:
                    if (!CreateNew) mega_up.ReadDataTextResponse();
                    item.ChunkUploadSize = chunksSizesToUploadMega[indexPosMega];
                    Uri uri = new Uri(item.UploadID + "/" + item.SizeWasTransfer.ToString());
                    mega_up = new MegaUpload(uri, item.ChunkUploadSize);
                    item.To.stream = mega_up.MakeStreamUpload();
                    break;

                default: throw new Exception("Not support.");
            }
            item.SaveSizeTransferSuccess = item.SizeWasTransfer;
            return;
        }

        void InitUploadMega()
        {
            // remake chunksSizesToUploadMega
            chunksSizesToUploadMega = ((MegaApiClient)clientTo).ComputeChunksSizesToUpload((item.From.stream as MegaAesCtrStreamCrypter).ChunksPositions, item.From.node.Info.Size).ToArray();
            int chunkscount = chunksSizesToUploadMega.Count();

            if (item.SaveSizeTransferSuccess > 0)
            {
                long check = chunksSizesToUploadMega[0];
                while (check != item.SaveSizeTransferSuccess - 1)//find indexPosMega
                {
                    indexPosMega++;
                    check += chunksSizesToUploadMega[indexPosMega];
                    if (indexPosMega >= chunkscount) throw new Exception("Chunks not match.");
                }
            }
        }

        void SaveEncryptDataDownloadMega()
        {
            StreamMegaInterface stream = this.item.From.stream as StreamMegaInterface;
            this.item.dataCryptoMega = stream.GetSave();//if null -> error transfer
        }

        bool SaveUploadMega()
        {
            MegaAesCtrStreamCrypter encryptedStream = item.From.stream as MegaAesCtrStreamCrypter;
            MegaApiClient client = clientTo as MegaApiClient;
            INode newitem = client.CommitUpload(item.From.node.Info.Name, null, encryptedStream, "");

            return false;
        }

        bool SaveUploadDropbox()
        {
            //create folder if not found
            Dropbox.AutoCreateFolder(item.To.node.Parent);
            dynamic json_ = JsonConvert.DeserializeObject(((DropboxRequestAPIv2)clientTo).upload_session_finish(null, item.UploadID, item.SizeWasTransfer, item.To.node.GetFullPathString(false,true), DropboxUploadMode.add));
            long size = json_.size;
            if (size == item.From.node.Info.Size) return true;
            else
            {
                item.ErrorMsg = "File size was upload not match.";
                return false;
            }
        }

        void Dispose()
        {
            GroupManager.ItemsTransferWork.Remove(this);
            GroupManager = null;
            item = null;
            clientTo = null;
        }
    }
}
