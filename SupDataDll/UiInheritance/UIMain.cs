using CloudManagerGeneralLib.Class;
using System;

namespace CloudManagerGeneralLib.UiInheritance
{
    public interface UIMain
    {
        bool AreReloadUI { get; }
        void load_uC_Lv_ud(UIUC_TLV_ud control);
        void ShowDialog_();
        void AddNewCloudToTV(ExplorerNode newcloud);

        void FileSaveDialog(string InitialDirectory,string FileName,string Filter, ExplorerNode node);
        void ShowChildUI(object UI, bool ShowDialog, bool Owner);
        T CreateUI<T>(Type type);
    }
}
