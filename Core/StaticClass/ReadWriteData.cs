using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Core.StaticClass
{
  public static class ReadWriteData
  {
    /// <summary>
    /// Default path: %appdata%\CloudManager
    /// </summary>
    public static readonly string Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\CloudManager";

    public const string File_Login = "Login.dat";
    public const string File_Settings = "Settings.dat";
    public const string File_DataUploadDownload = "DataUploadDownload.dat";

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
    static object sync_log = new object();
    public static void WriteLog(string message)
    {
      try
      {
        Monitor.Enter(sync_log);
        FileStream f = new FileStream(Path + "\\" + "log.txt", FileMode.OpenOrCreate, FileAccess.Write);
        byte[] data = Encoding.UTF8.GetBytes("[" + DateTime.Now.ToString("ss-mm-hh dd-MM-yyy") + "] : " + message + "\r\n");
        f.Seek(0, SeekOrigin.End);
        f.Write(data, 0, data.Length);
        f.Close();
      }
      finally { Monitor.Exit(sync_log); }
    }

    public static void Delete(string filename)
    {
      FileInfo info = new FileInfo(Path + "\\" + filename);
      if (info.Exists) info.Delete();
    }

  }
}