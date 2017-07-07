using Cloud.Dropbox;
using Cloud.GoogleDrive;
using Cloud.MegaNz;
using Core.CloudSubClass;
using Core.StaticClass;
using Newtonsoft.Json;
using CloudManagerGeneralLib;
using CloudManagerGeneralLib.Class;
using System;
using System.IO;
using System.Linq;
using static Core.CloudSubClass.MegaNz;

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
        string completionHandle = "-";

        CloudType Type_root_to;
        CloudType Type_root_from;
        object clientTo;
        public TransferBytes(TransferItem item, ItemsTransferManager GroupManager, object clientTo = null)
        {
            if (item.From.node.Info.Size == 0) { item.ErrorMsg = "File size zero."; Dispose(); }
            this.item = item;
            this.GroupManager = GroupManager;
            Type_root_to = this.item.To.node.GetRoot.RootType.Type;
            Type_root_from = this.item.From.node.GetRoot.RootType.Type;

            if (GroupManager.AreCut && (Type_root_to | CloudType.LocalDisk) == Type_root_from &&
                item.To.node.GetRoot.Info.Name == item.From.node.GetRoot.Info.Name)
            {
                LocalDisk.AutoCreateFolder(this.item.To.node.Parent);
                File.Move(item.From.node.GetFullPathString(), item.To.node.GetFullPathString());
                item.status = StatusTransfer.Moved;
                Dispose();
            }
            else
            {
                if (item.buffer == null) item.buffer = new byte[128 * 1024];
                if (clientTo != null) this.clientTo = clientTo;
                if (item.To.node.GetRoot.RootType.Type == CloudType.Mega) InitUploadMega();//InitUploadMega

                //Make Stream From
                item.From.stream = AppSetting.ManageCloud.GetFileStream(item.From.node, item.SaveSizeTransferSuccess,
                    item.From.node.Info.Size - 1, item.To.node.GetRoot.RootType.Type != CloudType.LocalDisk, item.dataCryptoMega);
                //Make Stream To
                if (item.ChunkUploadSize > 0) MakeNextChunkUploadInStreamTo(true);//upload to cloud
                else this.item.To.stream = AppSetting.ManageCloud.GetFileStream(item.To.node, item.SizeWasTransfer);//download to disk

                item.status = StatusTransfer.Running;
                item.From.stream.BeginRead(item.buffer, 0, item.buffer.Length, new AsyncCallback(GetFrom), 0);
            }
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
                    if (item.ChunkUploadSize < 0 || item.SizeWasTransfer == item.From.node.Info.Size)//save last pos if download or done (up/down)
                    {
                        if (Type_root_from == CloudType.Mega) SaveEncryptDataDownloadMega();//download done from mega
                        if (Type_root_to == CloudType.Mega) CommitUploadMega();//upload done to mega
                        if(Type_root_to == CloudType.Dropbox) ((DropboxRequestAPIv2)clientTo).GetResponse_upload_session_append();//get data return from server
                        item.SaveSizeTransferSuccess = item.SizeWasTransfer;
                    }

                    try { item.From.stream.Close(); } catch { }//close stream if can
                    try { item.To.stream.Close(); } catch { }//close stream if can

                    switch (GroupManager.GroupData.status)
                    {
                        case StatusTransfer.Waiting: item.status = StatusTransfer.Waiting; break;
                        case StatusTransfer.Running: break;
                        default: item.status = StatusTransfer.Stop; break;
                    }
                    if (item.status == StatusTransfer.Remove) GroupManager.GroupData.items.Remove(item);
                    else if (item.status == StatusTransfer.Running && GroupManager.GroupData.status == StatusTransfer.Running)
                    {
                        item.status = StatusTransfer.Done;
                        if (Type_root_to == CloudType.Dropbox) if (!SaveUploadDropbox()) item.status = StatusTransfer.Error;
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
                    if (totalchunkupload == item.ChunkUploadSize)
                    {
                        if (Type_root_to == CloudType.Mega) indexPosMega++;
                        MakeNextChunkUploadInStreamTo();
                        totalchunkupload = 0;
                    }//make new stream of next chunk and set totalchunkupload=0
                    int nexbyteread = Math.Min(item.ChunkUploadSize - totalchunkupload, item.buffer.Length); //item.ChunkUploadSize - totalchunkupload >= item.buffer.Length ? item.buffer.Length : item.ChunkUploadSize - totalchunkupload;//1<= nexbyteread = (chunksize - totalupload) <= buffer Length
                    item.From.stream.BeginRead(item.buffer, 0, nexbyteread, new AsyncCallback(GetFrom), totalchunkupload);
                }
                else//if download
                {
                    if (Type_root_from == CloudType.Mega) SaveEncryptDataDownloadMega();
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

            switch (item.To.node.GetRoot.RootType.Type)
            {
                case CloudType.Dropbox:
                    if (!CreateNew) ((DropboxRequestAPIv2)clientTo).GetResponse_upload_session_append();//get data return from server
                    item.To.stream = ((DropboxRequestAPIv2)clientTo).upload_session_append(
                        new Dropbox_Request_UploadSessionAppend(item.UploadID, item.SizeWasTransfer), 
                        pos_end - item.SizeWasTransfer + 1);
                    break;

                case CloudType.GoogleDrive:
                    if (!CreateNew) ((DriveAPIHttprequestv2)clientTo).Files.Insert_Resumable_Response();//get data return from server
                    item.To.stream = ((DriveAPIHttprequestv2)clientTo).Files.Insert_Resumable(item.UploadID, item.SizeWasTransfer, pos_end, item.From.node.Info.Size);
                    break;
                case CloudType.Mega:
                    if (!CreateNew)
                    {
                        completionHandle = mega_up.ReadDataTextResponse();//get data return from server
                        if (completionHandle.StartsWith("-")) throw new Exception(completionHandle);
                    }
                    item.ChunkUploadSize = chunksSizesToUploadMega[indexPosMega];
                    Uri uri = new Uri(item.UploadID + "/" + item.SizeWasTransfer.ToString());
                    mega_up = new MegaUpload(uri, item.ChunkUploadSize);
                    MegaAesCtrStreamCrypter crypt_stream = item.From.stream as MegaAesCtrStreamCrypter;

                    item.dataCryptoMega = crypt_stream.GetSave();//save
                    item.dataCryptoMega.fileKey = crypt_stream.FileKey;
                    item.dataCryptoMega.iv = crypt_stream.Iv;

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
            item.ChunkUploadSize = chunksSizesToUploadMega[indexPosMega];
        }

        void SaveEncryptDataDownloadMega()
        {
            this.item.dataCryptoMega = (this.item.From.stream as StreamMegaInterface).GetSave();//if null -> error transfer
        }

        bool CommitUploadMega()
        {
            completionHandle = mega_up.ReadDataTextResponse();
            if (completionHandle.StartsWith("-")) throw new Exception(completionHandle);
            MegaAesCtrStreamCrypter encryptedStream = item.From.stream as MegaAesCtrStreamCrypter;
            MegaApiClient client = clientTo as MegaApiClient;
            MegaNzNode parent = new MegaNzNode(item.To.node.Parent.Info.ID);
            INode newitem = client.CommitUpload(item.From.node.Info.Name, parent, encryptedStream, completionHandle);
            if (newitem != null) return true;
            throw new Exception("Commit Upload Failed.");
        }

        bool SaveUploadDropbox()
        {
            Dropbox.AutoCreateFolder(item.To.node.Parent);
            Dropbox_Request_UploadSessionFinish session_finish = new Dropbox_Request_UploadSessionFinish(
                new Dropbox_upload(item.To.node.GetFullPathString(false)),
                new Dropbox_Request_UploadSessionAppend(item.UploadID, item.SizeWasTransfer)
                );
            IDropbox_Response_MetaData json_ = ((DropboxRequestAPIv2)clientTo).upload_session_finish(session_finish);
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
            chunksSizesToUploadMega = null;
            mega_up = null;
            completionHandle = null;
        }

        #region Dropbox Private Class
        class Dropbox_Request_UploadSessionFinish : IDropbox_Request_UploadSessionFinish
        {
            public Dropbox_Request_UploadSessionFinish(IDropbox_upload commit,IDropbox_Request_UploadSessionAppend cursor)
            {
                this.commit = commit;
                this.cursor = cursor;
            }

            public IDropbox_upload commit { get; set; }
            public IDropbox_Request_UploadSessionAppend cursor { get; set; }
        }
        class Dropbox_upload : IDropbox_upload
        {
            public Dropbox_upload(string path, Dropbox_WriteMode mode = Dropbox_WriteMode.add,bool autorename =false,bool mute =false)
            {
                this.path = path;
                this.mode = mode;
                this.autorename = autorename;
                this.mute = mute;
            }

            public bool autorename { get; set; }
            public Dropbox_WriteMode mode { get; set; }
            public bool mute { get; set; }
            public string path { get; set; }
        }
        class Dropbox_Request_UploadSessionAppend : IDropbox_Request_UploadSessionAppend
        {
            public Dropbox_Request_UploadSessionAppend(string session_id,long offset)
            {
                this.offset = offset;
                this.session_id = session_id;
            }

            public long offset { get; set; }
            public string session_id { get; set; }
        }
        #endregion
    }
}
