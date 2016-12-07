using SupDataDll;
using System.Drawing;

namespace FormUI
{
    public class Setting_UI
    {
        //Reflection
        public static Reflection_EventToCore reflection_eventtocore = new Reflection_EventToCore();


        public static ManagerThread ManagerThreads = new ManagerThread();
        public static Color Background = SystemColors.ControlDark;
    }
}
