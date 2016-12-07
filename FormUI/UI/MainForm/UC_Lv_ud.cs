using System;
using System.Collections.Generic;
using System.Windows.Forms;
//using BrightIdeasSoftware;
using SupDataDll;
using System.ComponentModel;

namespace FormUI.UI.MainForm
{
    public partial class UC_Lv_ud : UserControl, SupDataDll.UiInheritance.UIUC_Lv_ud
    {

        #region Reflection
        System.Windows.Forms.Form mainform;
        public object UIMain { set { mainform = (System.Windows.Forms.Form)value; } }

        public int AddNewGroup(UD_group_work group_work)
        {
            group_work.col[2] = group_work.status.ToString();
            Groups.Add(group_work);
            if (group_work.change == ChangeTLV.Processing) TLV_ud.AddObject(group_work);
            else TLV_done.AddObject(group_work);
            return Groups.IndexOf(group_work);
        }

        public void RemoveGroup(UD_group_work Group)
        {
            TLV_ud.RemoveObject(Groups[Groups.IndexOf(Group)]);
            TLV_done.RemoveObject(Groups[Groups.IndexOf(Group)]);
            Groups.Remove(Group);
        }

        public void RefreshAll()
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => DoRefresh()));
                }
                else DoRefresh();
            }
            catch (InvalidAsynchronousStateException ex) { }
        }

        void DoRefresh()
        {
            foreach (UD_group_work item in Groups)
            {
                if (item.change == ChangeTLV.Done)
                {
                    foreach (var child in item.items)
                    {
                        if (child.status != child.CheckChangeStatus)
                        {
                            TLV_done.RefreshObject(child);
                            child.CheckChangeStatus = child.status;
                        }
                    }
                }

                if (item.change == ChangeTLV.Processing)
                {
                    foreach (var child in item.items)
                    {
                        if (child.status == StatusUpDown.Running | child.status != child.CheckChangeStatus)
                        {
                            TLV_ud.RefreshObject(child);
                            child.CheckChangeStatus = child.status;
                        }
                    }
                }

                if (item.change == ChangeTLV.DoneToProcessing)
                {
                    TLV_done.RemoveObject(Groups[Groups.IndexOf(item)]);
                    TLV_ud.AddObject(Groups[Groups.IndexOf(item)]);
                    item.change = ChangeTLV.Processing;
                }

                if (item.change == ChangeTLV.ProcessingToDone)
                {
                    TLV_ud.RemoveObject(Groups[Groups.IndexOf(item)]);
                    TLV_done.AddObject(Groups[Groups.IndexOf(item)]);
                    item.change = ChangeTLV.Done;
                }
            }
        }

        public void LoadLanguage()
        {
            TP_processing.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TP_processing);
            TP_done.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TP_done);

            changeStatusToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_ChangeStatus);
            numberOfParallelDownloadsToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_numberOfParallelDownloads);
            removeToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_remove);
            startToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_start);
            stopToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_stop);
            waitingToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_wating);
            errorToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_error);
            forceStartToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_forcestart);
            forceWaitingToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_forcewaiting);

            Set_TLV_lang(TLV_ud);
            Set_TLV_lang(TLV_done);
        }
        void Set_TLV_lang(TreeListView lv)
        {
            for (int i = 0; i < lv.Columns.Count; i++)//0-7
            {
                lv.Columns[i].Text = Setting_UI.reflection_eventtocore._GetTextLanguage("TLV_UD_Columns_" + i.ToString());
            }
        }
        #endregion


        List<UD_group_work> Groups = new List<UD_group_work>();
        private List<OLVColumn> LoadHeader_TLV(List<HeaderTLV> list)
        {
            List<OLVColumn> ListHeader_TLV = new List<OLVColumn>();
            foreach (HeaderTLV h in list)
            {
                OLVColumn hd = new OLVColumn();
                hd.Width = h.Width;
                hd.Text = h.name;
                ListHeader_TLV.Add(hd);
            }
            return ListHeader_TLV;
        }// 8 header
        private void LoadListView_TLV(ref TreeListView TLV, ControlCollection cc, OLVColumn[] array_header)
        {
            TLV = new TreeListView();
            TLV.Dock = DockStyle.Fill;
            TLV.View = View.Details;
            TLV.MultiSelect = true;
            TLV.GridLines = true;
            TLV.FullRowSelect = true;

            TLV.Columns.AddRange(array_header);

            cc.Add(TLV);// add to ControlCollection
            //
            TLV.CanExpandGetter = delegate (object x) { return (x is UD_group_work); };//group
            TLV.ChildrenGetter = delegate (object x)//child
            {
                UD_group_work list_ud_item = x as UD_group_work;
                return list_ud_item.items;
            };

            TLV.SelectedIndexChanged += TLV_SelectedIndexChanged;
            TLV.ContextMenuStrip = contextMenuStrip1;
            //col 0
            array_header[0].AspectGetter = delegate (object x)
            {
                if (x is UD_group_work)//group
                {
                    return ((UD_group_work)x).Name;
                }
                else//child
                {
                    return "";
                }
            };

            for (int val = 1; val < array_header.Length; val++)
            {
                int i = val - 1;
                array_header[val].AspectGetter = delegate (object x)
                {
                    if (x is UD_item_work)//item in group
                    {
                        return ((UD_item_work)x).col[i];
                    }
                    if (x is UD_group_work)//group
                    {
                        return ((UD_group_work)x).col[i];
                    }
                    return "";
                };
            }
        }

        List<UD_group_work> parents;
        List<UD_item_work> childs;

        private List<HeaderTLV> Create_Headeritem_TLV_ud()
        {
            List<HeaderTLV> list = new List<HeaderTLV>();
            list.Add(new HeaderTLV() { name = "#", Width = 30 });
            list.Add(new HeaderTLV() { name = "From", Width = 250 });
            list.Add(new HeaderTLV() { name = "To", Width = 250 });
            list.Add(new HeaderTLV() { name = "Status", Width = 60 });
            list.Add(new HeaderTLV() { name = "Progress", Width = 150 });
            list.Add(new HeaderTLV() { name = "Speed", Width = 100 });
            list.Add(new HeaderTLV() { name = "Estimated", Width = 100 });
            list.Add(new HeaderTLV() { name = "Error", Width = 250 });
            return list;
        }

        public TreeListView TLV_ud;
        public TreeListView TLV_done;



        TreeListView olv;
        #region UserControl
        public UC_Lv_ud()
        {
            InitializeComponent();
            List<HeaderTLV> list = Create_Headeritem_TLV_ud();
            LoadListView_TLV(ref TLV_ud, TP_processing.Controls, LoadHeader_TLV(list).ToArray());
            LoadListView_TLV(ref TLV_done, TP_done.Controls, LoadHeader_TLV(list).ToArray());
        }
        #endregion

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ////tabControl1.TabPages[tabControl1.SelectedIndex].Font = new Font(tabControl1.TabPages[tabControl1.SelectedIndex].Font, FontStyle.Bold);
            //int temp = tabControl1.SelectedIndex == 0 ? 1 : 0;
            //tabControl1.TabPages[temp].Font = new Font(tabControl1.TabPages[temp].Font, FontStyle.Regular);
        }

        private void TLV_SelectedIndexChanged(object sender, EventArgs e)
        {
            parents = new List<UD_group_work>();
            childs = new List<UD_item_work>();
            olv = sender as TreeListView;
            for (int i = 0; i < olv.SelectedObjects.Count; i++)
            {
                if (olv.SelectedObjects[i].GetType() == typeof(UD_item_work)) childs.Add(olv.SelectedObjects[i] as UD_item_work);
                else parents.Add(olv.SelectedObjects[i] as UD_group_work);
            }
        }
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (parents == null & childs == null) { e.Cancel = true; return; }
            startToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = false;
            waitingToolStripMenuItem.Enabled = false;
            errorToolStripMenuItem.Enabled = false;
            numberOfParallelDownloadsToolStripMenuItem.Enabled = false;

            removeToolStripMenuItem.Enabled = true;

            if (parents != null && parents.Count != 0)//parent menu
            {
                if (parents.Count == 1) numberOfParallelDownloadsToolStripMenuItem.Enabled = true;
                foreach (var parent in parents)
                {
                    if (parent.status == StatusUpDown.Stop | parent.status == StatusUpDown.Waiting) startToolStripMenuItem.Enabled = true;
                    if (parent.status == StatusUpDown.Waiting | parent.status == StatusUpDown.Running) stopToolStripMenuItem.Enabled = true;
                    if (parent.status == StatusUpDown.Stop) waitingToolStripMenuItem.Enabled = true;
                    if (parent.status == StatusUpDown.Removing) removeToolStripMenuItem.Enabled = true;
                    if (errorToolStripMenuItem.Enabled == false) foreach (var child in parent.items)
                        {
                            if (child.status == StatusUpDown.Error)
                            {
                                errorToolStripMenuItem.Enabled = true;
                                break;
                            }
                        }
                }
            }

            if (childs != null && childs.Count != 0)// child menu
            {
                foreach (var child in childs)
                {
                    if (child.status == StatusUpDown.Waiting | child.status == StatusUpDown.Stop | child.status == StatusUpDown.Error) startToolStripMenuItem.Enabled = true;
                    if (child.status == StatusUpDown.Running | child.status == StatusUpDown.Stop | child.status == StatusUpDown.Started) waitingToolStripMenuItem.Enabled = true;
                    if (child.status == StatusUpDown.Started | child.status == StatusUpDown.Running | child.status == StatusUpDown.Waiting) stopToolStripMenuItem.Enabled = true;
                    if (child.status == StatusUpDown.Removing) removeToolStripMenuItem.Enabled = false;
                }
            }
        }

        private void numberOfParallelDownloadsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (parents != null && parents.Count == 1)
            {
                ChangeNumberItemDownload f = new ChangeNumberItemDownload(parents[0].MaxItemsDownload);
                f.ShowDialog(mainform);
                if (f.NumberItems != parents[0].MaxItemsDownload) parents[0].MaxItemsDownload = f.NumberItems;
            }
        }

        #region ChangeStatus
        private void ChangeStatus(StatusUpDown val)
        {
            if (childs != null && childs.Count != 0)
            {
                foreach (var child in childs)
                {
                    //set Started when item is Stop,Waiting
                    if (val == StatusUpDown.Started && (child.status == StatusUpDown.Stop | child.status == StatusUpDown.Waiting |
                                                        child.status == StatusUpDown.Error))
                    {
                        child.status = val;
                        UD_group_work pr = olv.GetParent(child) as UD_group_work;
                        if (pr != null && (pr.status != StatusUpDown.Running | pr.status != StatusUpDown.Loading | pr.status != StatusUpDown.Removing)) pr.status = val;
                    }
                    //set Stop child
                    if (val == StatusUpDown.Stop && (child.status != StatusUpDown.Done | child.status != StatusUpDown.Error | child.status != StatusUpDown.Stop)) child.status = val;
                    //set Waiting child
                    if (val == StatusUpDown.Waiting && child.status != StatusUpDown.Done) child.status = val;

                    if (val == StatusUpDown.Remove) child.status = val;
                }
            }
            if (parents != null && parents.Count != 0)
            {
                foreach (var parent in parents)
                {
                    if (val == StatusUpDown.Started && (parent.status != StatusUpDown.Running)) parent.status = val;
                    if (val == StatusUpDown.Stop && (parent.status != StatusUpDown.Done | parent.status != StatusUpDown.Loading | parent.status != StatusUpDown.Stop)) parent.status = val;
                    if (val == StatusUpDown.Waiting && (parent.status != StatusUpDown.Done | parent.status != StatusUpDown.Removing)) parent.status = val;
                    if (val == StatusUpDown.Remove) parent.status = val;
                }
            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeStatus(StatusUpDown.Started);
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeStatus(StatusUpDown.Stop);
        }

        private void waitingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeStatus(StatusUpDown.Waiting);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show(this, "Are you sure to remove?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rs == DialogResult.Yes) ChangeStatus(StatusUpDown.Remove);
        }

        void ErrorSetForce(StatusUpDown val)
        {
            foreach (var parent in parents)
            {
                foreach (var child in parent.items)
                {
                    if (child.status == StatusUpDown.Error)
                    {
                        child.status = val;
                    }
                }
            }
        }

        private void forceStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ErrorSetForce(StatusUpDown.Started);
        }

        private void forceWaitingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ErrorSetForce(StatusUpDown.Waiting);
        }
        #endregion
    }

    public class HeaderTLV
    {
        public int Width;
        public string name;
    }
}
