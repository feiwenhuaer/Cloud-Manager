using System;
using System.IO;
using System.Text;

namespace Core.StaticClass
{
    public static class ReadWriteData
    {
        public static TextReader Read(string filename)
        {
            FileStream RawStream = new FileStream(AppSetting.RootDirectory + "\\" + filename, FileMode.Open, FileAccess.Read);
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
            Random rd = new Random();
            int val = rd.Next(1, 2 ^ 8 - 1);
            byte[] Buff = AppSetting.Crypt(data, val);
            FileStream FS = new FileStream(AppSetting.RootDirectory + "\\" + filename, FileMode.Create, FileAccess.Write);
            FS.WriteByte((byte)val);
            FS.Write(Buff, 0, Buff.Length);
            FS.Close();
            FS = null;
        }
    }
}
