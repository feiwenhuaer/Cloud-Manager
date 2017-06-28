using CloudManagerGeneralLib.Class;

namespace Core.Transfer
{
    public class JsonDataSaveGroup
    {
        public ItemNode fromfolder;
        public ItemNode savefolder;
        public bool AreCut = false;
        public TransferGroup Group = new TransferGroup();
    }
}
