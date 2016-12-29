using SupDataDll;
using System.Windows.Controls;

namespace WpfUI.UI.Main.Lv_ud
{
    /// <summary>
    /// Interaction logic for UC_TLV.xaml
    /// </summary>
    public partial class UC_TLV :UserControl
    {
        public TransferDataTLVWPF data;

        public UC_TLV()
        {
            InitializeComponent();
            this.DataContext = this;
            data = new TransferDataTLVWPF();
            treeList.Model = data;
        }

        private void treeList_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            treeList.SelectedItems.Clear();
        }
    }
}
