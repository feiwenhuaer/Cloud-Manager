using System.Windows;
using System;

namespace CloudManagerGeneralLib.UiInheritance
{
    public interface UILogin
    {
        WindowState WindowState_ { get; set; }

        bool ShowInTaskbar_ { get; set; }

        void ShowDialog_();

        void Load_User(string User, string Pass, bool AutoLogin);
    }
    public enum WindowState
    {
        Normal = 0,
        Minimized = 1,
        Maximized = 2
    }
}
