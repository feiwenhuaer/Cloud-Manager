namespace Cloud.MegaNz
{
    public class DataCryptoMega
    {
        public long position = 0;
        public byte[] metaMac = new byte[8];
        public byte[] counter = new byte[8];
        public long currentCounter = 0;
        public byte[] currentChunkMac = new byte[16];
        public byte[] fileMac = new byte[16];
    }
}