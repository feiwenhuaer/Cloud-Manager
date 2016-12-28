using Core;
using Core.StaticClass;
using SupDataDll;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using SupDataDll.UiInheritance;

namespace Cloud_Manager
{
    static class Program
    {
        static Mutex mutex = new Mutex(true, "{6f001441-e901-43d1-994e-9d92e94fdd37}");
        [STAThread]
        static void Main(string[] arg)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                DeleteFile_dev();// dev mode

                AppSetting.Load(); // load setting

                LoadDllUI.Load();//load dll UI
                Reflection_UI.CreateInstanceLogin();//Create Login UI
                Reflection_UI.Load_Setting_UI();//Load Setting_UI and event

                AppSetting.login.Load(arg);// load login form
                AppSetting.UILogin.ShowDialog_();
                if (!string.IsNullOrEmpty(AppSetting.Pass))
                {
                    AppSetting.ud_items.Start();
                    Reflection_UI.Load_UIMain();
                    showMainForm:
                    AppSetting.ud_items.status = StatusUpDownApp.Start;
                    AppSetting.UIMain.ShowDialog_();
                    if (AppSetting.UIMain.AreReloadUI)//if reload ui
                    {
                        AppSetting.ud_items.status = StatusUpDownApp.Pause;
                        //clean memory
                        AppSetting.UIMain = null;
                        AppSetting.uc_lv_ud_instance = null;
                        //reload dll ui
                        LoadDllUI.Load();//reload dll
                        Reflection_UI.Load_Setting_UI();//Load Setting_UI and event
                        Reflection_UI.Load_UIMain();//create instance main and LV_ud
                        AppSetting.settings.SaveSettings();
                        GC.Collect();
                        goto showMainForm;
                    }
                    // close all thread
                    AppSetting.UIclose = (UIClosing)Activator.CreateInstance(LoadDllUI.GetTypeInterface(typeof(UIClosing)));
                    AppSetting.ud_items.Eventupdateclosingform += AppSetting.UIclose.updatedata;
                    AppSetting.ud_items.Eventcloseapp += AppSetting.UIclose.Close_;
                    AppSetting.ud_items.status = StatusUpDownApp.StopForClosingApp;
                    AppSetting.UIclose.ShowDialog_();
                }
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("false");
            }
        }

        private static void DeleteFile_dev()
        {
            Console.WriteLine("Debug mode");
            //FileInfo info = new FileInfo(Directory.GetCurrentDirectory() + "\\lang\\" + "eng.xml");
            //if (info.Exists) info.Delete();
            //info = new FileInfo("Settings.dat");
            //if (info.Exists) info.Delete();
            //info = new FileInfo(Directory.GetCurrentDirectory() + "\\" + "DataUploadDownload.dat");
            //if (info.Exists) info.Delete();

            DirectoryInfo dinfo = new DirectoryInfo(@"E:\temp");
            if (dinfo.Exists) { dinfo.Delete(true); }
            dinfo.Create();
        }
    }
}
