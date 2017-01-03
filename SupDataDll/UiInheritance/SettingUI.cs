using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupDataDll.UiInheritance
{
    public class SettingUI
    {
        public static Reflection_EventToCore reflection_eventtocore = new Reflection_EventToCore();
        public static ManagerThread ManagerThreads = new ManagerThread();
        public static bool ReloadUI_Flag = false;
        public static bool ExitAPP_Flag = false;
    }
}
