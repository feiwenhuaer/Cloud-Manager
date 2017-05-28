using CloudManagerGeneralLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace WpfUI.Class
{
    public class ContextMenuDataModel: INotifyPropertyChanged
    {
        public ContextMenuDataModel(string Text)
        {
            this.Text = Text;
        }
        public ContextMenuDataModel(LanguageKey Key)
        {
            Text = Setting_UI.reflection_eventtocore.SettingAndLanguage.GetTextLanguage(Key);
            this.Key = Key;
            IsEnabled = true;
        }
        public ContextMenuDataModel(string Name,CloudType Img_type)
        {
            this.Text = Name;
            this.Img = Setting_UI.GetImage(ListBitmapImageResource.list_bm_cloud[(int)Img_type]);
            this.Type = Img_type;
        }
        public ContextMenuDataModel(CloudType Name_n_Img_type)
        {
            this.Text = Name_n_Img_type.ToString();
            this.Img = Setting_UI.GetImage(ListBitmapImageResource.list_bm_cloud[(int)Name_n_Img_type]);
            this.Type = Name_n_Img_type;
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

        private bool _IsEnabled = true;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                _IsEnabled = value;
                NotifyPropertyChange("IsEnabled");
            }
        }


        public string Text { get; set; }
        public LanguageKey Key { get; set; }
        public Image Img { get; set; }
        public CloudType Type { get; set; }
        
        private void NotifyPropertyChange(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
