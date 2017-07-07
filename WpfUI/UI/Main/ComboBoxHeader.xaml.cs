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
        IItemNode node;
        public IItemNode Node
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

        void UpdateData(IItemNode newnode)
        {
            int start_index = 0;
            if (node != null)
            {
                IItemNode sameparent = newnode.FindSameParent(node);
                if (sameparent == null) start_index = node.GetFullPath().IndexOf(node.GetFullPath().Find(n => n == sameparent)) + 1;
                while (Source.Count - 1 >= start_index) Source.RemoveAt(start_index);
            }
            newnode.GetFullPath().ForEach(n => { Source.Add(new ComboBoxData(n)); });
            if (Source.Count >= 0) comboBox.SelectedIndex = Source.Count - 1;
        }
    }

    public class ComboBoxData
    {
        public string Text { get; private set; }
        IItemNode node;
        public IItemNode Node { get { return node; } set { node = value; UpdateData(); } }

        public ComboBoxData(IItemNode Node)
        {
            this.Node = Node;
        }
        void UpdateData()
        {
            if(node is RootNode && (node as RootNode).RootType.Type != CloudType.LocalDisk) this.Text = (node as RootNode).RootType.Type.ToString() + ":" + (node as RootNode).RootType.Email;
            else this.Text = Node.Info.Name;
        }
    }
}
