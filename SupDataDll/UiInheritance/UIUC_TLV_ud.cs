using System.Windows.Forms;

namespace SupDataDll.UiInheritance
{
    public interface UIUC_TLV_ud
    {
        object UIMain { set; }
        int AddNewGroup(UD_group_work Group);

        void RemoveGroup(UD_group_work Group);

        void RefreshAll();
        
        void LoadLanguage();
    }
}
