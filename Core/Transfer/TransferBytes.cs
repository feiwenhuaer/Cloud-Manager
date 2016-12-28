using DropboxHttpRequest;
using GoogleDriveHttprequest;
using SupDataDll;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Core.Transfer
{
    public class TransferBytes
    {
        TransferItem item;
        object clientTo;
        public TransferBytes(TransferItem item, object clientTo)
        {
            this.item = item;
            this.clientTo = clientTo;
            //Make Stream To
            if (item.ChunkUploadSize > 0) MakeNextChunkUploadInStreamTo(true);
            else this.item.To.stream = AppSetting.ManageCloud.GetFileStream(item.To.path, null, false, item.Transfer);
            //begin transfer
            item.status = StatusUpDown.Running;
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
                if (item.Transfer == item.From.Size | item.status != StatusUpDown.Running)//transfer done/force stop.
                {
                    item.TransferRequest = item.Transfer;
                    try { item.From.stream.Close(); } catch { }
                    try { item.To.stream.Close(); } catch { }
                    if (item.status == StatusUpDown.Running) item.status = StatusUpDown.Done;
                    else item.status = StatusUpDown.Stop;
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
            catch (Exception ex) { item.ErrorMsg = ex.Message; item.status = StatusUpDown.Error; }
        }

        int MakeNextChunkUploadInStreamTo(bool CreateNew = false)
        {
            if (item.ChunkUploadSize <= 0) throw new Exception("Not upload type");
            long pos_end = item.Transfer + item.ChunkUploadSize - 1;
            if (pos_end >= item.From.Size) pos_end = item.From.Size - 1;

            switch (item.To.TypeCloud)
            {
                case CloudName.Dropbox:
                    if (!CreateNew) ((DropboxRequestAPIv2)clientTo).GetResponse_upload_session_append();//get data return from server
                    item.To.stream = ((DropboxRequestAPIv2)clientTo).upload_session_append(item.From.Fileid, pos_end - item.Transfer + 1, item.Transfer);
                    break;

                case CloudName.GoogleDrive:
                    if (!CreateNew) ((DriveAPIHttprequestv2)clientTo).GetResponse_Files_insert_resumable();//get data return from server
                    item.To.stream = ((DriveAPIHttprequestv2)clientTo).Files_insert_resumable(item.From.Fileid, item.Transfer, pos_end, item.From.Size);
                    break;


                default: throw new Exception("Not support.");
            }
            item.TransferRequest = item.Transfer;//
            return 0;
        }
    }
}
