using CloudManagerGeneralLib.Class;
using System;

namespace CloudManagerGeneralLib.UiInheritance
{
    public interface UIMain
    {
        bool AreReloadUI { get; }
        void ShowDialog_();
        void AddNewCloudToTV(RootNode newcloud);

        void UpdateGroup(TransferGroup Group, UpdateTransfer_TLVUD type);

        void FileSaveDialog(string InitialDirectory,string FileName,string Filter, IItemNode node);
        void ShowChildUI(object UI, bool ShowDialog, bool Owner);
        T CreateUI<T>(Type type);
    }
}
