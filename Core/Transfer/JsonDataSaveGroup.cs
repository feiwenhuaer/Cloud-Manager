using SupDataDll;
using System.Collections.Generic;

namespace Core.Transfer
{
    public class JsonDataSaveGroup
    {
        public string fromfolder_raw;
        public string savefolder_raw;
        public bool AreCut = false;
        public UD_group_work Group = new UD_group_work();
    }
}
