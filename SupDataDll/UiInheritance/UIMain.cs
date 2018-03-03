using CloudManagerGeneralLib.Class;
using System;
using System.Collections.ObjectModel;

namespace CloudManagerGeneralLib.UiInheritance
{
  public interface UIMain
  {
    bool AreReloadUI { get; }
    void ShowDialog_();
    void AddNewCloudToTV(RootNode newcloud);

    TransferListViewData ItemsTransfer { get; set; }
    //void UpdateGroup(TransferGroup Group, UpdateTransfer_TLVUD type);

    void FileSaveDialog(string InitialDirectory, string FileName, string Filter, IItemNode node);
    void ShowChildUI(object UI, bool ShowDialog, bool Owner);
    T CreateUI<T>(Type type);
  }
}
