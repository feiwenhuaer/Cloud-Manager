using CloudManagerGeneralLib.Class;

namespace CloudManagerGeneralLib.UiInheritance
{
    public interface UIUC_TLV_ud
    {
        object UIMain { set; }
        int AddNewGroup(TransferGroup Group);

        void RemoveGroup(TransferGroup Group);

        void RefreshAll();
        
        void LoadLanguage();
    }
}
