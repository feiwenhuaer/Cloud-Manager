namespace SupDataDll.UiInheritance
{
    public interface UIMain
    {
        bool AreReloadUI { get; }
        void load_uC_Lv_ud(UIUC_Lv_ud control);
        void ShowDialog_();
        void AddNewCloudToTV(string email, CloudName type);

        void FileSaveDialog(string InitialDirectory,string FileName,string Filter, AnalyzePath rp, string filename, long filesize);
    }
}
