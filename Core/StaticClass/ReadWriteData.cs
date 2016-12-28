using System;
using System.IO;
using System.Text;

namespace Core.StaticClass
{
    public static class ReadWriteData
    {
        /// <summary>
        /// Default path: %appdata%\CloudManager
        /// </summary>
        public static string Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\CloudManager";
        public static string File_Login { get { return "Login.dat"; } }
        public static string File_Settings { get { return "Settings.dat"; } }
        public static string File_DataUploadDownload { get { return "DataUploadDownload.dat"; } }


        public static TextReader Read(string filename)
        {
            FileStream RawStream = new FileStream(Path + "\\" + filename, FileMode.Open, FileAccess.Read);
            if (RawStream.Length == 0) return null;
            byte[] crypt = new byte[1];
            byte[] Buffer = new byte[RawStream.Length - 1];
            RawStream.Read(crypt, 0, 1);
            RawStream.Read(Buffer, 0, Buffer.Length);
            RawStream.Close();
            RawStream = null;
            Buffer = AppSetting.Crypt(Buffer, crypt[0]);
            MemoryStream Stream = new MemoryStream(Buffer);
            return new StreamReader(Stream, Encoding.UTF8);
        }

        public static void Write(string filename, byte[] data)
        {
            FileInfo fi = new FileInfo(Path + "\\" + filename);
            if (fi.Exists) fi.Delete();

            Random rd = new Random();
            int val = rd.Next(1, 2 ^ 8 - 1);
            byte[] Buff = AppSetting.Crypt(data, val);
            FileStream FS = new FileStream(Path + "\\" + filename, FileMode.Create, FileAccess.Write);
            FS.WriteByte((byte)val);
            FS.Write(Buff, 0, Buff.Length);
            FS.Close();
            FS = null;
        }

        /// <summary>
        /// Create folder at ReadWriteData.Path
        /// </summary>
        public static void CreateFolderSaveData()
        {
            if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
        }

        public static bool Exists(string filename)
        {
            return File.Exists(Path + "\\" + filename);
        }
    }
}
