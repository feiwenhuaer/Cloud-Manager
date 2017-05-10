using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudManagerGeneralLib.UiInheritance
{
    public class SettingUI
    {
        public static RequestToCore reflection_eventtocore = new RequestToCore();
        public static ManagerThread ManagerThreads = new ManagerThread();
        public static bool ReloadUI_Flag = false;
        public static bool ExitAPP_Flag = false;
    }
}
