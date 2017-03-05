using Aga.Controls.Tree;
using SupDataDll;
using SupDataDll.Class;
using System.Collections;
using System.Collections.ObjectModel;

namespace WpfUI.Class
{
    public class TransferDataTLVWPF : ITreeModel
    {
        ObservableCollection<TransferGroup> groups_ = new ObservableCollection<TransferGroup>();
        public void Add(TransferGroup ud_group)
        {
            if (groups_.IndexOf(ud_group) < 0) groups_.Add(ud_group);
        }
        public void Remove(TransferGroup ud_group)
        {
            if (groups_.IndexOf(ud_group) > -1) groups_.Remove(ud_group);
        }


        public IEnumerable GetChildren(object parent)
        {
            var pr = parent as TransferGroup;
            if (parent == null)
            {
                foreach (TransferGroup group in groups_)
                {
                    yield return group;
                }
            }
            else if (pr != null)
            {
                foreach (TransferItem item in pr.items)
                {
                    yield return item;
                }
            }
        }

        public bool HasChildren(object parent)
        {
            return parent is TransferGroup;
        }
    }
}
