using CloudManagerGeneralLib;
using CloudManagerGeneralLib.Class;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace WpfUI.UI.Main
{
    /// <summary>
    /// Interaction logic for ComboBoxHeader.xaml
    /// </summary>
    public partial class ComboBoxHeader : UserControl
    {
        ItemNode node;
        public ItemNode Node
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

        void UpdateData(ItemNode newnode)
        {
            int start_index = 0;
            if (node != null)
            {
                ItemNode sameparent = newnode.FindSameParent(node);
                if (sameparent == null) start_index = node.GetFullPath().IndexOf(node.GetFullPath().Find(n => n == sameparent)) + 1;
                while (Source.Count - 1 >= start_index) Source.RemoveAt(start_index);
            }
            newnode.GetFullPath().ForEach(n => { Source.Add(new ComboBoxData(n)); });
            if (Source.Count >= 0)
                comboBox.SelectedIndex = Source.Count - 1;
        }
    }

    public class ComboBoxData
    {
        public string Text { get; private set; }
        ItemNode node;
        public ItemNode Node { get { return node; } set { node = value; UpdateData(); } }

        public ComboBoxData(ItemNode Node)
        {
            this.Node = Node;
        }
        void UpdateData()
        {
            switch (node.NodeType.Type)
            {
                case CloudType.Folder:
                case CloudType.LocalDisk: this.Text = Node.Info.Name; break;
                default: this.Text = node.NodeType.Type.ToString() + ":" + Node.NodeType.Email; break;
            }
        }
    }
}
