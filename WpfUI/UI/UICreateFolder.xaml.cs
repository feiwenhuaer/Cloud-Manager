﻿using CloudManagerGeneralLib.Class;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace WpfUI.UI
{
    /// <summary>
    /// Interaction logic for UICreateFolder.xaml
    /// </summary>
    public partial class UICreateFolder : Window
    {
        ItemNode parent;
        public UICreateFolder(ItemNode parent)
        {
            InitializeComponent();
            BT_create.Visibility = Visibility.Hidden;
            this.parent = parent;
        }

        private void BT_create_Click(object sender, RoutedEventArgs e)
        {
            DoCreateFolder();
        }
        private void BT_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex rg = new Regex("^[^\\/\\\\:<>\\|\"*?]+$");// ^[^\\/\\\\:<>\\|\"*?]+$
            Match match = rg.Match(textBox.Text);
            if (match.Success) BT_create.Visibility = Visibility.Visible;
            else BT_create.Visibility = Visibility.Hidden;
        }

        void DoCreateFolder()
        {
            Thread thr = new Thread(CreateFolder);
            Setting_UI.ManagerThreads.CleanThr();
            Setting_UI.ManagerThreads.createfolder.Add(thr);
            thr.Start(textBox.Text);
        }
        void CreateFolder(object obj)
        {
            ItemNode n = new ItemNode();
            n.Info.Name = (string)obj;
            parent.AddChild(n);
            Setting_UI.reflection_eventtocore.ExplorerAndManagerFile.CreateFolder(n);
            Dispatcher.Invoke(new Action(() => this.Close()));
        }
    }
}
