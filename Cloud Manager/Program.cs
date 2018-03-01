#define TESTCLASS
using Core.StaticClass;
using CloudManagerGeneralLib;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CloudManagerGeneralLib.UiInheritance;
using Core.CloudSubClass;
using Core.Transfer;
using Core.Class;
using System.Runtime.InteropServices;
using System.Reflection;
using System.ServiceProcess;
using System.Security.Principal;
using System.Diagnostics;

namespace Cloud_Manager
{
  public static class Program
  {
    public static string guid;
    public static Mutex mutex;
    [STAThread]
    static void Main(string[] args)
    {
      Assembly assembly = typeof(Program).Assembly;
      GuidAttribute attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
      guid = attribute.Value;
      mutex = new Mutex(true, "{" + guid + "}");

      if (mutex.WaitOne(TimeSpan.Zero, true))
      {
        AppSetting.MainThread = Thread.CurrentThread;
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
#if DEBUG
        DeleteFile_dev();// dev mode
#endif
        //load instance class
        AppSetting.LoadAPIKey();
        ReadWriteData.CreateFolderSaveData();// create folder %appdata%\\CloudManager
        Get_mimeType.Load();//mimeType (google drive upload)
        AppSetting.ManageCloud = new CloudManager();//explorer
        AppSetting.TransferManager = new GroupsTransferManager();//transfer file
        AppSetting.login = new Core.Class.Login();//load login

        //load core
        AppSetting.settings = new Settings();//load settings
        AppSetting.settings.ReadSettings();
        AppSetting.lang = new Languages(AppSetting.settings.GetSettingsAsString(SettingsKey.lang));//load language

        if (!LoadDllUI.Load()) return;//load dll UI
        AddEventHandleUI.CreateInstanceLogin();//Create Login UI
        AddEventHandleUI.Load_Setting_UI();//Load Setting_UI and event

        AppSetting.login.Load(args);// load login form
        AppSetting.UILogin.ShowDialog_();
        if (!string.IsNullOrEmpty(AppSetting.Pass))
        {
#if DEBUG && TESTCLASS
          Core.TestClass.Test();
#endif
          AppSetting.TransferManager.Start();
          AddEventHandleUI.Load_UIMain();
        showMainForm:
          AppSetting.TransferManager.status = StatusUpDownApp.Start;
          AppSetting.UIMain.ShowDialog_();
          if (AppSetting.UIMain.AreReloadUI)//if reload ui
          {
            AppSetting.CloseOauthUI();
            AppSetting.TransferManager.status = StatusUpDownApp.Pause;
            //clean memory
            AppSetting.UIMain = null;
            //reload dll ui
            if (!LoadDllUI.Load()) return;//reload dll
                                          //Reflection_UI.Load_Setting_UI();//Load Setting_UI and event
            AddEventHandleUI.Load_UIMain();//create instance main and LV_ud
            AppSetting.settings.SaveSettings();
            GC.Collect();
            goto showMainForm;
          }
          // close all thread
          AppSetting.UIclose = (UIClosing)Activator.CreateInstance(LoadDllUI.GetTypeInterface(typeof(UIClosing)));
          AppSetting.TransferManager.Eventupdateclosingform += AppSetting.UIclose.updatedata;
          AppSetting.TransferManager.Eventcloseapp += AppSetting.UIclose.Close_;
          AppSetting.TransferManager.status = StatusUpDownApp.StopForClosingApp;
          AppSetting.UIclose.ShowDialog_();
        }
        mutex.ReleaseMutex();
      }
      else
      {
        MessageBox.Show("false");
      }
    }

    static bool CheckAdminRight()
    {
      using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
      {
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
      }
    }

    static void StartProc(string[] args,bool AsAdmin = false)
    {
      string arg = "";
      foreach (string a in args) arg += a + " ";
      arg.TrimEnd(' ');
      Process proc = new Process();
      ProcessStartInfo info = new ProcessStartInfo(System.Reflection.Assembly.GetEntryAssembly().Location, arg);
      info.UseShellExecute = true;
      if (AsAdmin)
      {        
        info.Verb = "runas";
      }
      proc.StartInfo = info;
      proc.Start();
    }

#if DEBUG
    private static void DeleteFile_dev()
    {
      Console.WriteLine("Debug mode");


      //FileInfo info = new FileInfo(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\lang\\" + "eng.xml");
      //if (info.Exists) info.Delete();
      //info = new FileInfo("Settings.dat");
      //if (info.Exists) info.Delete();
      //info = new FileInfo(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)() + "\\" + "DataUploadDownload.dat");
      //if (info.Exists) info.Delete();

      DirectoryInfo dinfo = new DirectoryInfo(@"E:\temp");
      if (dinfo.Exists) { dinfo.Delete(true); }
      dinfo.Create();
    }
#endif
  }
}