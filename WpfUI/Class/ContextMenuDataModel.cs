using SupDataDll;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WPFUI.Class
{
    public class ContextMenuDataModel: INotifyPropertyChanged
    {
        public ContextMenuDataModel(LanguageKey Key)
        {
            Text = Setting_UI.reflection_eventtocore._GetTextLanguage(Key);
            this.Key = Key;
            IsEnabled = true;
        }

        private ObservableCollection<ContextMenuDataModel> _Child;
        public ObservableCollection<ContextMenuDataModel> Child {
            get{ return _Child ?? (_Child = new ObservableCollection<ContextMenuDataModel>()); }
            set
            {
                _Child = value;
                NotifyPropertyChange("Child");
            }
        }
        public string Text { get; set; }
        public LanguageKey Key { get; set; }

        private bool _IsEnabled = true;
        public bool IsEnabled { get { return _IsEnabled; }
            set
            {
                _IsEnabled = value;
                NotifyPropertyChange("IsEnabled");
            }
        }

        private void NotifyPropertyChange(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
