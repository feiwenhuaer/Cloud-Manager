using System;
using System.Collections.Generic;
using System.Windows.Forms;
//using BrightIdeasSoftware;
using SupDataDll;
using System.ComponentModel;
using BrightIdeasSoftware;

namespace FormUI.UI.MainForm
{
    public partial class UC_Lv_ud : UserControl, SupDataDll.UiInheritance.UIUC_TLV_ud
    {

        #region Reflection
        System.Windows.Forms.Form mainform;
        public object UIMain { set { mainform = (System.Windows.Forms.Form)value; } }

        public int AddNewGroup(TransferGroup group_work)
        {
            group_work.col[2] = group_work.status.ToString();
            Groups.Add(group_work);
            if (group_work.change == ChangeTLV.Processing) TLV_ud.AddObject(group_work);
            else TLV_done.AddObject(group_work);
            return Groups.IndexOf(group_work);
        }

        public void RemoveGroup(TransferGroup Group)
        {
            TLV_ud.RemoveObject(Groups[Groups.IndexOf(Group)]);
            TLV_done.RemoveObject(Groups[Groups.IndexOf(Group)]);
            Groups.Remove(Group);
        }

        public void RefreshAll()
        {
                if (InvokeRequired)Invoke(new Action(() => DoRefresh()));
                else DoRefresh();
        }

        void DoRefresh()
        {
            foreach (TransferGroup item in Groups)
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
                        if (child.status == StatusTransfer.Running | child.status != child.CheckChangeStatus)
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
            waitingToolStripMenuItem.Text = Setting_UI.reflection_eventtocore._GetTextLanguage(LanguageKey.TSMI_waiting);
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


        List<TransferGroup> Groups = new List<TransferGroup>();
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
            TLV.CanExpandGetter = delegate (object x) { return (x is TransferGroup); };//group
            TLV.ChildrenGetter = delegate (object x)//child
            {
                TransferGroup list_ud_item = x as TransferGroup;
                return list_ud_item.items;
            };

            TLV.SelectedIndexChanged += TLV_SelectedIndexChanged;
            TLV.ContextMenuStrip = contextMenuStrip1;
            //col 0
            array_header[0].AspectGetter = delegate (object x)
            {
                if (x is TransferGroup)//group
                {
                    return ((TransferGroup)x).Name;
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
                    if (x is TransferItem)//item in group
                    {
                        return ((TransferItem)x).col[i];
                    }
                    if (x is TransferGroup)//group
                    {
                        return ((TransferGroup)x).col[i];
                    }
                    return "";
                };
            }
        }

        List<TransferGroup> parents;
        List<TransferItem> childs;

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
            parents = new List<TransferGroup>();
            childs = new List<TransferItem>();
            olv = sender as TreeListView;
            for (int i = 0; i < olv.SelectedObjects.Count; i++)
            {
                if (olv.SelectedObjects[i].GetType() == typeof(TransferItem)) childs.Add(olv.SelectedObjects[i] as TransferItem);
                else parents.Add(olv.SelectedObjects[i] as TransferGroup);
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
                    if (parent.status == StatusTransfer.Stop | parent.status == StatusTransfer.Waiting) startToolStripMenuItem.Enabled = true;
                    if (parent.status == StatusTransfer.Waiting | parent.status == StatusTransfer.Running) stopToolStripMenuItem.Enabled = true;
                    if (parent.status == StatusTransfer.Stop) waitingToolStripMenuItem.Enabled = true;
                    if (parent.status == StatusTransfer.Removing) removeToolStripMenuItem.Enabled = true;
                    if (errorToolStripMenuItem.Enabled == false) foreach (var child in parent.items)
                        {
                            if (child.status == StatusTransfer.Error)
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
                    if (child.status == StatusTransfer.Waiting | child.status == StatusTransfer.Stop | child.status == StatusTransfer.Error) startToolStripMenuItem.Enabled = true;
                    if (child.status == StatusTransfer.Running | child.status == StatusTransfer.Stop | child.status == StatusTransfer.Started) waitingToolStripMenuItem.Enabled = true;
                    if (child.status == StatusTransfer.Started | child.status == StatusTransfer.Running | child.status == StatusTransfer.Waiting) stopToolStripMenuItem.Enabled = true;
                    if (child.status == StatusTransfer.Removing) removeToolStripMenuItem.Enabled = false;
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
        private void ChangeStatus(StatusTransfer val)
        {
            if (childs != null && childs.Count != 0)
            {
                foreach (var child in childs)
                {
                    //set Started when item is Stop,Waiting
                    if (val == StatusTransfer.Started && (child.status == StatusTransfer.Stop | child.status == StatusTransfer.Waiting |
                                                        child.status == StatusTransfer.Error))
                    {
                        child.status = val;
                        TransferGroup pr = olv.GetParent(child) as TransferGroup;
                        if (pr != null && (pr.status != StatusTransfer.Running | pr.status != StatusTransfer.Loading | pr.status != StatusTransfer.Removing)) pr.status = val;
                    }
                    else
                    //set Stop child
                        if (val == StatusTransfer.Stop && (child.status != StatusTransfer.Done | child.status != StatusTransfer.Error | child.status != StatusTransfer.Stop)) child.status = val;
                    else
                    //set Waiting child
                        if (val == StatusTransfer.Waiting && child.status != StatusTransfer.Done) child.status = val;
                    else
                        if (val == StatusTransfer.Remove) child.status = val;
                }
            }
            if (parents != null && parents.Count != 0)
            {
                foreach (var parent in parents)
                {
                    if (val == StatusTransfer.Started && (parent.status != StatusTransfer.Running)) parent.status = val;
                    else
                    if (val == StatusTransfer.Stop && (parent.status != StatusTransfer.Done | parent.status != StatusTransfer.Loading | parent.status != StatusTransfer.Stop)) parent.status = val;
                    else
                    if (val == StatusTransfer.Waiting && (parent.status != StatusTransfer.Done | parent.status != StatusTransfer.Removing)) parent.status = val;
                    else
                    if (val == StatusTransfer.Remove) parent.status = val;
                }
            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeStatus(StatusTransfer.Started);
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeStatus(StatusTransfer.Stop);
        }

        private void waitingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeStatus(StatusTransfer.Waiting);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show(this, "Are you sure to remove?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rs == DialogResult.Yes) ChangeStatus(StatusTransfer.Remove);
        }

        void ErrorSetForce(StatusTransfer val)
        {
            foreach (var parent in parents)
            {
                foreach (var child in parent.items)
                {
                    if (child.status == StatusTransfer.Error)
                    {
                        child.status = val;
                    }
                }
            }
        }

        private void forceStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ErrorSetForce(StatusTransfer.Started);
        }

        private void forceWaitingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ErrorSetForce(StatusTransfer.Waiting);
        }
        #endregion
    }

    public class HeaderTLV
    {
        public int Width;
        public string name;
    }
}
