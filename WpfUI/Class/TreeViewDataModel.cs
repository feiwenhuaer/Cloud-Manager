using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace WPFUI.Class
{
    public class TreeViewDataModel : System.ComponentModel.INotifyPropertyChanged
    {
        public TreeViewDataModel(TreeViewDataModel Parent)
        {
            if (Parent != null) this.Parent = Parent;
        }
        public TreeviewDataItem DisplayData { get; set; }
        public TreeViewDataModel Parent { get; set; }
        private ObservableCollection<TreeViewDataModel> _children;
        public ObservableCollection<TreeViewDataModel> Children
        {
            get { return _children ?? (_children = new ObservableCollection<TreeViewDataModel>()); }
            set
            {
                _children = value;
                NotifyPropertyChange("Children");
            }
        }
        private void NotifyPropertyChange(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }

        #region INotifyPropertyChanged Members
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
