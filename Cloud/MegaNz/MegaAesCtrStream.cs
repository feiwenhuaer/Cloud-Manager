namespace Cloud.MegaNz
{
    using CloudManagerGeneralLib.Class.Mega;
    using CloudManagerGeneralLib.Crypt;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class MegaAesCtrStreamCrypter : MegaAesCtrStream
    {
        public MegaAesCtrStreamCrypter(Stream stream)
          : base(stream, stream.Length, Mode.Crypt, Crypto.CreateAesKey(), Crypto.CreateAesKey().CopySubArray(8))
        {
        }

        public MegaAesCtrStreamCrypter(Stream stream, long FileLength, DataCryptoMega InfoEncrypt)
          : base(stream, FileLength, Mode.Crypt, InfoEncrypt == null ? Crypto.CreateAesKey() : InfoEncrypt.fileKey, InfoEncrypt == null ? Crypto.CreateAesKey().CopySubArray(8) : InfoEncrypt.iv)
        {
            if (InfoEncrypt == null) return;
            this.position = InfoEncrypt.position;
            this.metaMac = InfoEncrypt.metaMac;
            this.counter = InfoEncrypt.counter;
            this.currentCounter = InfoEncrypt.currentCounter;
            this.currentChunkMac = InfoEncrypt.currentChunkMac;
            this.fileMac = InfoEncrypt.fileMac;
        }

        public byte[] FileKey
        {
            get { return this.fileKey; }
        }

        public byte[] Iv
        {
            get { return this.iv; }
        }

        public byte[] MetaMac
        {
            get
            {
                if (this.position != this.streamLength)
                {
                    throw new NotSupportedException("Stream must be fully read to obtain computed FileMac");
                }

                return this.metaMac;
            }
        }
    }

    internal class MegaAesCtrStreamDecrypter : MegaAesCtrStream
    {
        private readonly byte[] expectedMetaMac;

        public MegaAesCtrStreamDecrypter(Stream stream, long streamLength, byte[] fileKey, byte[] iv, byte[] expectedMetaMac)
          : base(stream, streamLength, Mode.Decrypt, fileKey, iv)
        {
            if (expectedMetaMac == null || expectedMetaMac.Length != 8)
            {
                throw new ArgumentException("Invalid expectedMetaMac");
            }

            this.expectedMetaMac = expectedMetaMac;
        }

        public MegaAesCtrStreamDecrypter(Stream stream,long fileLength, byte[] fileKey, byte[] iv, byte[] expectedMetaMac,
            DataCryptoMega dataCrypto) :base(stream,fileLength,Mode.Decrypt,fileKey,iv)
        {
            if (expectedMetaMac == null || expectedMetaMac.Length != 8) throw new ArgumentException("Invalid expectedMetaMac");
            this.expectedMetaMac = expectedMetaMac;

            if (dataCrypto == null) return;
            this.position = dataCrypto.position;
            this.metaMac = dataCrypto.metaMac;
            this.counter = dataCrypto.counter;
            this.currentCounter = dataCrypto.currentCounter;
            this.currentChunkMac = dataCrypto.currentChunkMac;
            this.fileMac = dataCrypto.fileMac;
        }

        protected override void OnStreamRead()
        {
            if (!this.expectedMetaMac.SequenceEqual(this.metaMac))
            {
                throw new DownloadException();
            }
        }
    }

    public abstract class MegaAesCtrStream : Stream, StreamMegaInterface
    {
        protected readonly byte[] fileKey;
        protected readonly byte[] iv;
        protected readonly long streamLength;
        protected long position = 0;//need save for resume
        protected byte[] metaMac = new byte[8];//need save for resume

        private readonly Stream stream;
        private readonly Mode mode;
        private readonly long[] chunksPositions;//need save for resume\\
        protected byte[] counter = new byte[8];//need save for resume
        protected long currentCounter = 0;//need save for resume
        protected byte[] currentChunkMac = new byte[16];//need save for resume
        protected byte[] fileMac = new byte[16];//need save for resume

        protected MegaAesCtrStream(Stream stream, long FileLength, Mode mode)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            this.stream = stream;
            this.streamLength = FileLength;
            this.mode = mode;

            this.chunksPositions = this.GetChunksPositions(this.streamLength);
        }

        protected MegaAesCtrStream(Stream stream, long streamLength, Mode mode, byte[] fileKey, byte[] iv)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (fileKey == null || fileKey.Length != 16)
            {
                throw new ArgumentException("Invalid fileKey");
            }

            if (iv == null || iv.Length != 8)
            {
                throw new ArgumentException("Invalid Iv");
            }

            this.stream = stream;
            this.streamLength = streamLength;
            this.mode = mode;
            this.fileKey = fileKey;
            this.iv = iv;

            this.chunksPositions = this.GetChunksPositions(this.streamLength);
        }

        protected enum Mode
        {
            Crypt,
            Decrypt
        }

        public long[] ChunksPositions
        {
            get { return this.chunksPositions; }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return this.streamLength; }
        }

        public override long Position
        {
            get
            {
                return this.position;
            }

            set
            {
                if (this.position != value)
                {
                    throw new NotSupportedException("Seek is not supported");
                }
            }
        }
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.position == this.streamLength) return 0;

            for (long pos = this.position; pos < Math.Min(this.position + count, this.streamLength); pos += 16)//16 byte(128 bit)
            {
                // We are on a chunk bondary
                if (this.chunksPositions.Any(chunk => chunk == pos))
                {
                    if (pos != 0)
                    {
                        // Compute the current chunk mac data on each chunk bondary
                        this.ComputeChunk();
                    }

                    // Init chunk mac with Iv values
                    for (int i = 0; i < 8; i++)
                    {
                        this.currentChunkMac[i] = this.iv[i];
                        this.currentChunkMac[i + 8] = this.iv[i];
                    }
                }

                this.IncrementCounter();

                // Iterate each AES 16 bytes block
                byte[] input = new byte[16];
                byte[] output = new byte[input.Length];
                int inputLength = this.stream.Read(input, 0, input.Length);
                if (inputLength != input.Length)//read full 16 byte
                {
                    // Sometimes, the stream is not finished but the read is not complete
                    inputLength += this.stream.Read(input, inputLength, input.Length - inputLength);
                }

                // Merge Iv and counter
                byte[] ivCounter = new byte[16];
                Array.Copy(this.iv, ivCounter, 8);
                Array.Copy(this.counter, 0, ivCounter, 8, 8);

                byte[] encryptedIvCounter = Crypto.EncryptAes(ivCounter, this.fileKey);

                for (int inputPos = 0; inputPos < inputLength; inputPos++)
                {
                    output[inputPos] = (byte)(encryptedIvCounter[inputPos] ^ input[inputPos]);
                    this.currentChunkMac[inputPos] ^= (this.mode == Mode.Crypt) ? input[inputPos] : output[inputPos];
                }

                // Copy to buffer
                Array.Copy(output, 0, buffer, offset + pos - this.position, Math.Min(output.Length, this.streamLength - pos));

                // Crypt to current chunk mac
                this.currentChunkMac = Crypto.EncryptAes(this.currentChunkMac, this.fileKey);
            }

            long len = Math.Min(count, this.streamLength - this.position);
            this.position += len;

            // When stream is fully processed, we compute the last chunk
            if (this.position == this.streamLength)
            {
                this.ComputeChunk();

                // Compute Meta MAC
                for (int i = 0; i < 4; i++)
                {
                    this.metaMac[i] = (byte)(this.fileMac[i] ^ this.fileMac[i + 4]);
                    this.metaMac[i + 4] = (byte)(this.fileMac[i + 8] ^ this.fileMac[i + 12]);
                }

                this.OnStreamRead();
            }

            return (int)len;
        }

        public DataCryptoMega GetSave()
        {
            DataCryptoMega dataCrypto = new DataCryptoMega();
            dataCrypto.position = this.position;
            dataCrypto.metaMac = this.metaMac;
            dataCrypto.counter = this.counter;
            dataCrypto.currentCounter = this.currentCounter;
            dataCrypto.currentChunkMac = this.currentChunkMac;
            dataCrypto.fileMac = this.fileMac;
            return dataCrypto;
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        protected virtual void OnStreamRead()
        {
        }

        private void IncrementCounter()
        {
            byte[] counter = BitConverter.GetBytes(this.currentCounter++);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(counter);
            }

            Array.Copy(counter, this.counter, 8);
        }

        private void ComputeChunk()
        {
            for (int i = 0; i < 16; i++)
            {
                this.fileMac[i] ^= this.currentChunkMac[i];
            }

            this.fileMac = Crypto.EncryptAes(this.fileMac, this.fileKey);
        }

        private long[] GetChunksPositions(long size)
        {
            List<long> chunks = new List<long>();
            chunks.Add(0);

            long chunkStartPosition = 0;
            for (int idx = 1; (idx <= 8) && (chunkStartPosition < (size - (idx * 131072))); idx++)
            {
                chunkStartPosition += idx * 131072;
                chunks.Add(chunkStartPosition);
            }

            while ((chunkStartPosition + 1048576) < size)//1048576 = 1024^2 = 1Mb
            {
                chunkStartPosition += 1048576;
                chunks.Add(chunkStartPosition);
            }
            return chunks.ToArray();
        }
    }
}
