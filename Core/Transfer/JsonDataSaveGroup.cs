using CloudManagerGeneralLib.Class;

namespace Core.Transfer
{
    public class JsonDataSaveGroup
    {
        public IItemNode fromfolder;
        public IItemNode savefolder;
        public bool AreCut = false;
        public TransferGroup Group = new TransferGroup();
    }
}
