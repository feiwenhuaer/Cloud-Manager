using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WpfUI.Class
{
    public class TreeViewDataModel : INotifyPropertyChanged, IEnumerable
    {
        public TreeViewDataModel(TreeViewDataModel Parent = null)
        {
            if (Parent != null) this.Parent = Parent;
        }
        public TreeviewDataItem DisplayData { get; set; }
        public TreeViewDataModel Parent { get; set; }
        private ObservableCollection<TreeViewDataModel> _childrens;
        public ObservableCollection<TreeViewDataModel> Childrens
        {
            get { return _childrens ?? ( _childrens = new ObservableCollection<TreeViewDataModel>()); }
            set
            {
                _childrens = value;
                NotifyPropertyChange("Childrens");
            }
        }
        private void NotifyPropertyChange(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public IEnumerator GetEnumerator()
        {
            return Childrens.GetEnumerator();
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
