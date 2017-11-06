using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CustomHttpRequest
{
  class UploadStream : Stream
  {
    Stream stream;
    long length;
    long position;
    public UploadStream(Stream stream,long Length)
    {
      this.stream = stream;
      this.length = Length;
      this.position = 0;
    }

    public override bool CanRead { get { return false; } }
    public override bool CanSeek { get { return false; } }
    public override bool CanWrite { get { return true; } }
    public override long Length { get { return this.length; } }
    public override long Position { get { return this.position; } set { throw new NotImplementedException(); } }
    public override void Flush() { throw new NotImplementedException(); }
    public override int Read(byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }
    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotImplementedException();
    }
    public override void SetLength(long value)
    {
      throw new NotImplementedException();
    }
    public override void Write(byte[] buffer, int offset, int count)
    {
      if (count + this.position > this.length) throw new Exception("Error: Data upload > Content-Length.");
      stream.Write(buffer, offset, count);
      this.position += count;
    }
  }

  class DownloadStream : Stream
  {
    Stream stream;
    long length;
    long position;
    public DownloadStream(Stream stream, long Length)
    {
      this.stream = stream;
      this.length = Length;
      this.position = 0;
    }

    public override bool CanRead { get { return true; } }
    public override bool CanSeek { get { return false; } }
    public override bool CanWrite { get { return false; } }
    public override long Length { get { return this.length; } }
    public override long Position { get { return this.position; } set { throw new NotImplementedException(); } }
    public override void Flush() { throw new NotImplementedException(); }
    public override int Read(byte[] buffer, int offset, int count)
    {
      int byte_read = this.stream.Read(buffer, offset, count);
      position += byte_read;
      return byte_read;
    }
    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotImplementedException();
    }
    public override void SetLength(long value)
    {
      throw new NotImplementedException();
    }
    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }
  }
}
