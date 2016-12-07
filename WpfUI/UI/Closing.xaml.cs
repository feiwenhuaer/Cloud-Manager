using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfUI.UI
{
    /// <summary>
    /// Interaction logic for Closing.xaml
    /// </summary>
    public partial class Closing : Window, SupDataDll.UiInheritance.UIClosing
    {
        public Closing()
        {
            InitializeComponent();
        }

        #region interface
        public void Close_()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() => this.Close()));
            }
            else this.Close();
        }

        public void ShowDialog_()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() => this.ShowDialog()));
            }
            else this.ShowDialog();
        }

        public void updatedata(string text)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() => updatelabel(text)));
            }
            else updatelabel(text);
        }
        #endregion

        void updatelabel(string text)
        {
            label.Content = text;
        }
    }
}
