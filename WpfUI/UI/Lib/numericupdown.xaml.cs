using System;
using System.Collections.Generic;
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

namespace WpfUI.UI.Lib
{
    /// <summary>
    /// Interaction logic for numericupdown.xaml
    /// </summary>
    public partial class numericupdown : UserControl
    {
        public numericupdown()
        {
            InitializeComponent();
            min = 0;
            max = 10;
        }
        int Num_;
        public int Number{get { return Num_; }
            set
            {
                if (value > max | value < min) throw new Exception("Can't set value <MinValue or >MaxValue");
                Num_ = value; txtNum.Text = value.ToString();
            }
        }

        int max;
        int min;
        public int MaxValue { get { return max; }
            set
            {
                if (value > min)
                {
                    if (Number > value) Number = value;
                    max = value;
                }
                else throw new Exception("Can't set MaxValue <= MinValue");
            }
        }

        public int MinValue { get { return min; }
            set
            {
                if (value < max)
                {
                    if (Number < value) Number = value;
                    min = value;
                }
                else throw new Exception("Can't set MinValue >= MaxValue");
            }
        }

        private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(txtNum.Text, out Num_);
        }

        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {
            Num_++;
            txtNum.Text = Num_.ToString();
        }

        private void cmdDown_Click(object sender, RoutedEventArgs e)
        {
            Num_--;
            txtNum.Text = Num_.ToString();
        }

        private void txtNum_KeyDown(object sender, KeyEventArgs e)
        {
            if((int)e.Key <48 | (int)e.Key >57)
            {
                e.Handled = false;
            }
        }
    }
}
