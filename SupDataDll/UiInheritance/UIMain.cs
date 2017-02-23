namespace SupDataDll.UiInheritance
{
    public interface UIMain
    {
        bool AreReloadUI { get; }
        void load_uC_Lv_ud(UIUC_TLV_ud control);
        void ShowDialog_();
        void AddNewCloudToTV(string email, CloudType type);

        void FileSaveDialog(string InitialDirectory,string FileName,string Filter, ExplorerNode node);
    }
}
