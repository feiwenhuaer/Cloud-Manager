using CloudManagerGeneralLib;
using CloudManagerGeneralLib.Class;
using Core.StaticClass;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.CloudSubClass
{

  public class DeleteTask
  {
    List<ItemNode> items;
    CloudManagerGeneralLib.UiInheritance.UIDelete ui;
    bool PernamentDelete = false;
    public DeleteTask(List<ItemNode> items, CloudManagerGeneralLib.UiInheritance.UIDelete ui, bool PernamentDelete = false)
    {
      if (ui == null) throw new Exception("UI is null.");
      if (items == null || items.Count == 0) throw new Exception("Need >= 1 item.");
      this.items = items;
      this.ui = ui;
      this.PernamentDelete = PernamentDelete;
      this.ui.EventCancel += Deleteform_EventCancelDelegate;
      this.ui.EventClosing += Deleteform_EventCloseForm;
    }

    public void Start()
    {
      Thread thr = new Thread(work);
      ui.Show_();
      thr.Start();
    }

    void work()
    {
      Thread.Sleep(500);
      foreach (ItemNode item in items)
      {
        while (cancel) { Thread.Sleep(100); if (closedform) return; }
        if (closedform) return;
        bool Iserror = false;
        ui.UpdateText(AppSetting.lang.GetText(LanguageKey.DeleteForm_updatetext_Deleting.ToString()) + item);
        try
        {
          switch (item.GetRoot.RootType.Type)
          {
            case CloudType.Dropbox:
              if (!Dropbox.Delete(item, PernamentDelete)) Iserror = true;
              break;
            case CloudType.GoogleDrive:
              if (!GoogleDrive.File_trash(item, PernamentDelete)) Iserror = true;
              break;
            case CloudType.LocalDisk:
              if (!LocalDisk.Delete(item, PernamentDelete)) Iserror = true;
              break;
            case CloudType.Mega:
            default: throw new UnknowCloudNameException("Error Unknow Cloud Type: " + item.GetRoot.RootType.Type.ToString());
          }
          if (!Iserror) ui.UpdateText(AppSetting.lang.GetText(LanguageKey.DeleteForm_updatetext_Deleted.ToString()) + "\r\n");
          else
          {
            ui.UpdateText(AppSetting.lang.GetText(LanguageKey.DeleteForm_updatetext_Error.ToString()) + "\r\n");
            if (ui.AutoClose)
            {
              ui.SetAutoClose(false);
            }
          }
        }
        catch (Exception ex)
        {
          ui.UpdateText(AppSetting.lang.GetText(LanguageKey.DeleteForm_updatetext_Error.ToString()) + "\r\nMessage:" + ex.Message + "\r\n");
          if (ui.AutoClose) ui.SetAutoClose(false);
        }
      }
      if (ui.AutoClose) ui.Close_();
      else
      {
        ui.SetTextButtonCancel(AppSetting.lang.GetText(LanguageKey.BT_close.ToString()));
        while (!ui.AutoClose)
        {
          Thread.Sleep(100);
          if (cancel) ui.Close_();
        }
      }
    }

    public bool cancel = false;
    public bool closedform = false;
    public void Deleteform_EventCancelDelegate()
    {
      cancel = !cancel;
    }
    public void Deleteform_EventCloseForm()
    {
      closedform = !closedform;
    }
  }

}
