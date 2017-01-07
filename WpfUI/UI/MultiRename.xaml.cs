using SupDataDll;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
    /// Interaction logic for MultiRename.xaml
    /// </summary>
    public partial class MultiRename : Window
    {
        AnalyzePath ap;
        ObservableCollection<LV_renameData> lv_data;
        public MultiRename(ObservableCollection<LV_renameData> items,string parent)
        {
            lv_data = items;
            ap = new AnalyzePath(parent);
            InitializeComponent();
            listView.ItemsSource = lv_data;
        }

        private void TB_name_TextChanged(object sender, TextChangedEventArgs e)
        {
            ChageTo();
        }
        Thread thr;
        private void BT_Change_Click(object sender, RoutedEventArgs e)
        {
            thr = new Thread(RenameItems);
        }

        private void BT_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        string regex_num = "{\\d+}";
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChageTo();
        }

        void ChageTo()
        {
            int startnumber = 1;
            List<char> list = new List<char>();
            for (int i = 1; i < lv_data.Count.ToString().Length; i++) list.Add('0');
            string formatnumber = new String(list.ToArray());
            Regex rg = new Regex(regex_num);
            Match m = rg.Match(lv_data[0].From);
            if(m.Success) int.TryParse(m.Value.Remove(m.Value.Length - 1).Remove(0, 1),out startnumber);
            foreach(LV_renameData item in lv_data)
            {
                item.To = StringResult(item.From, startnumber, formatnumber);
                AnalyzePath ap = new AnalyzePath(item.To);
                item.Newname = ap.NameLastItem;
                startnumber++;
            }
        }
        string StringResult(string from, int num, string numFormat)
        {
            Regex rg = new Regex(regex_num);
            Match m = rg.Match(from);
            if (m.Success) return from.Remove(m.Index, m.Length).Insert(m.Index, num.ToString(numFormat));
            else return from + num.ToString(numFormat);
        }


        void RenameItems()
        {
            bool isfalse = false;
            foreach(LV_renameData item in lv_data)
            {
                try
                {
                    AnalyzePath ap = new AnalyzePath(item.From);
                    if (ap.TypeCloud == CloudName.GoogleDrive ?
                        Setting_UI.reflection_eventtocore._MoveItem(null, null, item.ID, null, null,item.Newname, ap.Email, CloudName.GoogleDrive):
                        Setting_UI.reflection_eventtocore._MoveItem(item.From, item.To, item.ID, null, null, null, null)) item.Result = "Success";
                    else { item.Result = "Failed"; isfalse = true; }
                }catch (Exception ex) { item.Result = ex.Message; isfalse = true; }
            }
            if (!isfalse) Dispatcher.Invoke(new Action(() => Close()));
        }
    }

    public class LV_renameData: INotifyPropertyChanged
    {
        string from;
        public string From { get { return from; }
            set
            {
                from = value;
                NotifyPropertyChange("From");
            }
        }

        string to;
        public string To { get { return to; }
            set
            {
                to = value;
                NotifyPropertyChange("To");
            }
        }

        string result;
        public string Result{ get { return result; }
            set
            {
                result = value;
                NotifyPropertyChange("Result");
            }
        }

        public string ID { get; set; }

        public string Newname { get; set; }

        private void NotifyPropertyChange(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
