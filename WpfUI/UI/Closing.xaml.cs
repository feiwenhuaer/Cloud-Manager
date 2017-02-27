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
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (closeflag) this.Close();
        }

        bool closeflag = false;

        #region interface
        public void Close_()
        {
            closeflag = true;
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
