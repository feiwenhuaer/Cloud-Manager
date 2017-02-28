using SupDataDll;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfUI.UI.Main
{
    /// <summary>
    /// Interaction logic for ComboBoxHeader.xaml
    /// </summary>
    public partial class ComboBoxHeader : UserControl
    {
        ExplorerNode node;
        public ExplorerNode Node
        {
            get { return node; }
            set { UpdateData(value); node = value; }
        }
        ObservableCollection<ComboBoxData> Source = new ObservableCollection<ComboBoxData>();
        public ComboBoxHeader()
        {
            InitializeComponent();
            this.comboBox.ItemsSource = Source;
        }

        void UpdateData(ExplorerNode newnode)
        {
            int start_index = 0;
            if (node != null)
            {
                ExplorerNode sameparent = newnode.FindSameParent(node);
                if (sameparent == null) start_index = node.GetFullPath().IndexOf(node.GetFullPath().Find(n => n == sameparent)) + 1;
                while (Source.Count - 1 >= start_index) Source.RemoveAt(start_index);
            }
            newnode.GetFullPath().ForEach(n => { Source.Add(new ComboBoxData(n)); });
            if(Source.Count >= 0) this.comboBox.Text = Source[Source.Count - 1].Text;
        }
    }

    public class ComboBoxData
    {
        public string Text { get; private set; }
        ExplorerNode node;
        public ExplorerNode Node { get { return node; } set { node = value; UpdateData(); } }

        public ComboBoxData(ExplorerNode Node)
        {
            this.Node = Node;
        }
        void UpdateData()
        {
            switch (node.RootInfo.Type)
            {
                case CloudType.Folder:
                case CloudType.LocalDisk: this.Text = Node.Info.Name; break;
                default: this.Text = node.RootInfo.Type.ToString() + ":" + Node.RootInfo.Email; break;
            }
        }
    }
}
