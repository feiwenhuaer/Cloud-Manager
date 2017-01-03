using Cloud.Dropbox;
using Cloud.GoogleDrive;
using Core.Cloud;
using Newtonsoft.Json;
using SupDataDll;
using System;
using System.IO;

namespace Core.Transfer
{
    public class TransferBytes
    {
        public TransferItem item { get; private set; }
        object clientTo;
        public TransferBytes(TransferItem item, object clientTo)
        {
            this.item = item;
            this.clientTo = clientTo;
            //Make Stream To
            if (item.ChunkUploadSize > 0) MakeNextChunkUploadInStreamTo(true);
            else this.item.To.stream = AppSetting.ManageCloud.GetFileStream(item.To.ap.Path_Raw, null, false, item.Transfer);//download to disk
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
                if (item.ChunkUploadSize < 0 && item.To.stream.CanSeek) item.To.stream.Seek(item.Transfer, SeekOrigin.Begin);//if download to disk then seek.
                item.To.stream.Write(item.buffer, 0, item.byteread);
                item.Transfer += item.byteread;
                #endregion

                #region transfer done/force stop.
                if (item.Transfer == item.From.Size | item.status != StatusTransfer.Running)//transfer done/force stop.
                {
                    item.TransferRequest = item.Transfer;
                    try { item.From.stream.Close(); } catch { }
                    try { item.To.stream.Close(); } catch { }
                    if (item.status == StatusTransfer.Running)
                    {
                        if (item.To.ap.TypeCloud == CloudName.Dropbox)
                        {
                            if (!SaveUploadDropbox())
                            {
                                item.status = StatusTransfer.Error;
                                return;
                            }
                        }
                        item.status = StatusTransfer.Done;
                    }
                    else item.status = StatusTransfer.Stop;
                    return;
                }
                #endregion

                #region initial next byte[]
                if (item.ChunkUploadSize > 0)//if upload
                {
                    int totalchunkupload = (int)result.AsyncState;
                    totalchunkupload += item.byteread;
                    if (totalchunkupload == item.ChunkUploadSize) totalchunkupload = MakeNextChunkUploadInStreamTo();//make new stream of next chunk and set totalchunkupload=0
                    int nexbyteread = item.ChunkUploadSize - totalchunkupload >= item.buffer.Length ? item.buffer.Length : item.ChunkUploadSize - totalchunkupload;//1<= nexbyteread = (chunksize - totalupload) <= buffer Length
                    item.From.stream.BeginRead(item.buffer, 0, nexbyteread, new AsyncCallback(GetFrom), totalchunkupload);
                }
                else//if download
                {
                    item.TransferRequest = item.Transfer;
                    item.From.stream.BeginRead(item.buffer, 0, item.buffer.Length, new AsyncCallback(GetFrom), 0);
                }
                #endregion
            }
            catch (Exception ex)
            {
                if (AppSetting.TransferManager.status == StatusUpDownApp.StopForClosingApp | AppSetting.TransferManager.status == StatusUpDownApp.SavingData) return;
                item.ErrorMsg = ex.Message;
                item.status = StatusTransfer.Error;
            }
        }

        int MakeNextChunkUploadInStreamTo(bool CreateNew = false)
        {
            if (item.ChunkUploadSize <= 0) throw new Exception("Not upload type");
            long pos_end = item.Transfer + item.ChunkUploadSize - 1;
            if (pos_end >= item.From.Size) pos_end = item.From.Size - 1;

            switch (item.To.ap.TypeCloud)
            {
                case CloudName.Dropbox:
                    if (!CreateNew) ((DropboxRequestAPIv2)clientTo).GetResponse_upload_session_append();//get data return from server
                    item.To.stream = ((DropboxRequestAPIv2)clientTo).upload_session_append(item.UploadID, pos_end - item.Transfer + 1, item.Transfer);
                    break;

                case CloudName.GoogleDrive:
                    if (!CreateNew) ((DriveAPIHttprequestv2)clientTo).GetResponse_Files_insert_resumable();//get data return from server
                    item.To.stream = ((DriveAPIHttprequestv2)clientTo).Files_insert_resumable(item.UploadID, item.Transfer, pos_end, item.From.Size);
                    break;


                default: throw new Exception("Not support.");
            }
            item.TransferRequest = item.Transfer;//
            return 0;
        }

        bool SaveUploadDropbox()
        {
            //create folder if not found
            if (Dropbox.AutoCreateFolder(item.To.ap.GetPath(), item.To.ap.Email) != item.To.ap.GetPath())
            {
                item.ErrorMsg = "Failed to create folder: " + item.To.ap.GetPath();
                return false;
            }
            dynamic json_ = JsonConvert.DeserializeObject(((DropboxRequestAPIv2)clientTo).upload_session_finish(null, item.UploadID, item.Transfer, item.To.ap.GetPath(), DropboxUploadMode.add));
            long size = json_.size;
            if (size == item.From.Size) return true;
            else
            {
                item.ErrorMsg = "File size was upload not match.";
                return false;
            }
        }
    }
}
