using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupDataDll.UiInheritance
{
    public abstract class SettingUI
    {
        public static Reflection_EventToCore reflection_eventtocore = new Reflection_EventToCore();
        public static ManagerThread ManagerThreads = new ManagerThread();
    }
}
