using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CloudManagerGeneralLib;
using System.ComponentModel;
using BrightIdeasSoftware;
using CloudManagerGeneralLib.Class;

namespace FormUI.UI.MainForm
{
    public partial class UC_Lv_ud : UserControl
    {
        
        System.Windows.Forms.Form mainform;
        public object UIMain { set { mainform = (System.Windows.Forms.Form)value; } }

        public int AddNewGroup(TransferGroup group_work)
        {
            if (Setting_UI.ExitAPP_Flag) return -1;
            group_work.col[2] = group_work.status.ToString();
            Groups.Add(group_work);
            if (group_work.change == ChangeTLV.Processing) TLV_ud.AddObject(group_work);
            else TLV_done.AddObject(group_work);
            return Groups.IndexOf(group_work);
        }

        public void RemoveGroup(TransferGroup Group)
        {
            if (Setting_UI.ExitAPP_Flag) return;
            TLV_ud.RemoveObject(Groups[Groups.IndexOf(Group)]);
            TLV_done.RemoveObject(Groups[Groups.IndexOf(Group)]);
            Groups.Remove(Group);
        }

        public void RefreshAll()
        {
            if (Setting_UI.ExitAPP_Flag) return;
            if (InvokeRequired)Invoke(new Action(() => DoRefresh()));
                else DoRefresh();
        }

        void DoRefresh()
        {
            foreach (TransferGroup group in Groups)
            {
                switch(group.change)
                {
                    case ChangeTLV.Done:
                        //foreach (var child in group.items)
                        //{
                        //    if (child.status != child.CheckChangeStatus)
                        //    {
                        //        TLV_done.RefreshObject(child);
                        //        child.CheckChangeStatus = child.status;
                        //    }
                        //}
                        TLV_done.RefreshObject(group);
                        break;

                    case ChangeTLV.Processing:
                        for (int i = 0; i < group.items.Count; i++)
                        {
                            if (group.items[i].status == StatusTransfer.Running | group.items[i].status != group.items[i].CheckChangeStatus)
                            {
                                TLV_ud.RefreshObject(group.items[i]);
                                group.items[i].CheckChangeStatus = group.items[i].status;
                            }
                        }
                        break;

                    case ChangeTLV.DoneToProcessing:
                        TLV_done.RemoveObject(Groups[Groups.IndexOf(group)]);
                        TLV_ud.AddObject(Groups[Groups.IndexOf(group)]);
                        group.change = ChangeTLV.Processing;
                        break;

                    case ChangeTLV.ProcessingToDone:
                        TLV_ud.RemoveObject(Groups[Groups.IndexOf(group)]);
                        TLV_done.AddObject(Groups[Groups.IndexOf(group)]);
                        group.change = ChangeTLV.Done;
                        break;
                }
            }
        }

        public void LoadLanguage()
        {
            TP_processing.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TP_processing);
            TP_done.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TP_done);

            changeStatusToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_ChangeStatus);
            numberOfParallelDownloadsToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_numberOfParallelDownloads);
            removeToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_remove);
            startToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_start);
            stopToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_stop);
            waitingToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_waiting);
            errorToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_error);
            forceStartToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_forcestart);
            forceWaitingToolStripMenuItem.Text = Setting_UI.reflection_eventtocore.GetTextLanguage(LanguageKey.TSMI_forcewaiting);

            Set_TLV_lang(TLV_ud);
            Set_TLV_lang(TLV_done);
        }
        void Set_TLV_lang(TreeListView lv)
        {
            for (int i = 0; i < lv.Columns.Count; i++)//0-7
            {
                lv.Columns[i].Text = Setting_UI.reflection_eventtocore.GetTextLanguage("TLV_UD_Columns_" + i.ToString());
            }
        }


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
                    if (x is CloudManagerGeneralLib.Class.TransferItem)//item in group
                    {
                        return ((CloudManagerGeneralLib.Class.TransferItem)x).col[i];
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
        List<CloudManagerGeneralLib.Class.TransferItem> childs;

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
            childs = new List<CloudManagerGeneralLib.Class.TransferItem>();
            olv = sender as TreeListView;
            for (int i = 0; i < olv.SelectedObjects.Count; i++)
            {
                if (olv.SelectedObjects[i].GetType() == typeof(CloudManagerGeneralLib.Class.TransferItem)) childs.Add(olv.SelectedObjects[i] as CloudManagerGeneralLib.Class.TransferItem);
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

            #region Group menu
            if (parents != null && parents.Count != 0)//parent menu
            {
                if (parents.Count == 1) numberOfParallelDownloadsToolStripMenuItem.Enabled = true;
                foreach (var parent in parents)
                {
                    if(!startToolStripMenuItem.Enabled || !stopToolStripMenuItem.Enabled || !waitingToolStripMenuItem.Enabled) switch(parent.status)
                    {
                        case StatusTransfer.Stop:
                                startToolStripMenuItem.Enabled = true;
                                waitingToolStripMenuItem.Enabled = true;
                                break;
                        case StatusTransfer.Waiting:
                                startToolStripMenuItem.Enabled = true;
                                stopToolStripMenuItem.Enabled = true;
                                break;
                        case StatusTransfer.Running:
                                stopToolStripMenuItem.Enabled = true;
                                break;
                        default:break;

                    }
                    if (!errorToolStripMenuItem.Enabled) foreach (var child in parent.items)
                        {
                            if (child.status == StatusTransfer.Error)
                            {
                                errorToolStripMenuItem.Enabled = true;
                                break;
                            }
                        }
                }
            }
            #endregion

            #region child menu
            if (childs != null && childs.Count != 0)
            {
                foreach (var child in childs)
                {
                    switch(child.status)
                    {
                        case StatusTransfer.Started:
                            stopToolStripMenuItem.Enabled = true;
                            waitingToolStripMenuItem.Enabled = true;
                            break;
                        case StatusTransfer.Waiting:
                            stopToolStripMenuItem.Enabled = true;
                            startToolStripMenuItem.Enabled = true;
                            break;
                        case StatusTransfer.Running:
                            stopToolStripMenuItem.Enabled = true;
                            waitingToolStripMenuItem.Enabled = true;
                            break;
                        case StatusTransfer.Stop:
                            startToolStripMenuItem.Enabled = true;
                            waitingToolStripMenuItem.Enabled = true;
                            break;
                        case StatusTransfer.Error:
                            startToolStripMenuItem.Enabled = true;
                            break;
                        default:break;
                    }
                }
            }
            #endregion
        }

        private void numberOfParallelDownloadsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (parents != null && parents.Count == 1)
            {
                ChangeNumberItemsTransfer f = new ChangeNumberItemsTransfer(parents[0].MaxItemsDownload);
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
                    if (val == StatusTransfer.Started && (child.status == StatusTransfer.Stop || child.status == StatusTransfer.Waiting ||
                                                        child.status == StatusTransfer.Error))
                    {
                        child.status = val;
                        TransferGroup pr = olv.GetParent(child) as TransferGroup;
                        if (pr != null && (pr.status != StatusTransfer.Running || pr.status != StatusTransfer.Loading || pr.status != StatusTransfer.Remove)) pr.status = val;
                    }
                    else
                    //set Stop child
                        if (val == StatusTransfer.Stop && (child.status != StatusTransfer.Done || child.status != StatusTransfer.Error || child.status != StatusTransfer.Stop)) child.status = val;
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
                    if (val == StatusTransfer.Waiting && (parent.status != StatusTransfer.Done | parent.status != StatusTransfer.Remove)) parent.status = val;
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
