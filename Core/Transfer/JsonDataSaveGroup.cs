using CloudManagerGeneralLib.Class;

namespace Core.Transfer
{
    public class JsonDataSaveGroup
    {
        public ExplorerNode fromfolder;
        public ExplorerNode savefolder;
        public bool AreCut = false;
        public TransferGroup Group = new TransferGroup();
    }
}
