using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using PharmaceuticalInformation.BaseTypes;

namespace PharmaceuticalInformation.Service
{
    public class ManagementOfWindows : BaseType
    {

        #region ' Fields '

        //
        private DataTable ListOfWindows;
        private ToolStripItemCollection MenuItems;

        #endregion

        #region ' Designer '

        public ManagementOfWindows(string PathToLogFile, ToolStripItemCollection MenuItems)
            : base(PathToLogFile)
        {
            //
            this.ListOfWindows = CreatingListOfWindows();
            this.MenuItems = MenuItems;
        }

        public ManagementOfWindows(ToolStripItemCollection MenuItems)
            : this("", MenuItems)
        {
            //
        }

        // Creating List Of Windows
        private DataTable CreatingListOfWindows()
        {
            //
            DataTable ListOfWindows = new DataTable("ListOfWindows");
            //
            ListOfWindows.Columns.Add("Key", typeof(string));
            ListOfWindows.Columns.Add("Showing", typeof(bool));
            ListOfWindows.Columns.Add("OneWindow", typeof(bool));
            ListOfWindows.Columns.Add("CaptionOfWindow", typeof(string));
            ListOfWindows.Columns.Add("Form", typeof(Form));
            //
            ListOfWindows.PrimaryKey = new DataColumn[1] { ListOfWindows.Columns["Key"] };
            // Return
            return ListOfWindows;
        }

        #endregion

        #region ' Management Of Forms '

        // Addition Window
        public void Addition(string NameOfWindow, Form AdditionWindow)
        {
            //
            if ((NameOfWindow != null) && (NameOfWindow != ""))
                    if (AdditionWindow != null)
                        if (ListOfWindows.Rows.Find(NameOfWindow) == null)
                        {
                            DataRow NewWindow = ListOfWindows.NewRow();
                            NewWindow["Key"] = NameOfWindow;
                            NewWindow["Showing"] = true;
                            NewWindow["OneWindow"] = true;
                            NewWindow["Form"] = AdditionWindow;
                            ListOfWindows.Rows.Add(NewWindow);
                            ListOfWindows.AcceptChanges();
                            //
                            RefreshingMenuItems();
                        }
        }

        // Addition Window
        public void Addition(Form AdditionWindow)
        {
            //
            if (AdditionWindow != null)
            {
                //
                string NameOfWindow = AdditionWindow.Name;
                int Number = 0;
                //
                do
                {
                    NameOfWindow = String.Format("{0}{1}", NameOfWindow, ++Number);
                } 
                while (ListOfWindows.Rows.Find(NameOfWindow) != null);
                //
                if (ListOfWindows.Rows.Find(NameOfWindow) == null)
                {
                    //
                    DataRow NewWindow = ListOfWindows.NewRow();
                    //
                    NewWindow["Key"] = NameOfWindow;
                    NewWindow["Showing"] = true;
                    NewWindow["OneWindow"] = false;
                    NewWindow["CaptionOfWindow"] = AdditionWindow.Text;
                    NewWindow["Form"] = AdditionWindow;
                    //
                    ListOfWindows.Rows.Add(NewWindow);
                    ListOfWindows.AcceptChanges();
                    //
                    RefreshingMenuItems();
                }
            }
        }

        // Removing Window
        public void Removing(string NameOfWindow)
        {
            //
            if ((NameOfWindow != null) && (NameOfWindow != ""))
            {
                DataRow FindRow = ListOfWindows.Rows.Find(NameOfWindow);
                if (FindRow != null)
                {
                    FindRow.Delete();
                    ListOfWindows.AcceptChanges();
                    //
                    RefreshingMenuItems();
                }
            }
        }

        // Contains Window
        public bool Contains(string NameOfWindow)
        {
            //
            if ((NameOfWindow != null) && (NameOfWindow != ""))
            {
                if (ListOfWindows.Rows.Find(NameOfWindow) != null)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        // Showing Window
        public void ShowingWindow(string NameOfWindow)
        {
            //
            if ((NameOfWindow != null) && (NameOfWindow != ""))
            {
                DataRow FindRow = ListOfWindows.Rows.Find(NameOfWindow);
                if (FindRow != null)
                {
                    FindRow["Showing"] = true;
                    ((Form)FindRow["Form"]).Show();
                    ListOfWindows.AcceptChanges();
                    //
                    RefreshingMenuItems();
                }
            }
        }

        // Hiding Window
        public void HidingWindow(string NameOfWindow)
        {
            //
            if ((NameOfWindow != null) && (NameOfWindow != ""))
            {
                DataRow FindRow = ListOfWindows.Rows.Find(NameOfWindow);
                if (FindRow != null)
                {
                    FindRow["Showing"] = false;
                    ListOfWindows.AcceptChanges();
                    //
                    RefreshingMenuItems();
                }
            }
        }

        // Refreshing Caption
        public void RefreshingCaption(string NameOfWindow, string NewCaption)
        {
            //
            if ((NameOfWindow != null) && (NameOfWindow != ""))
            {
                DataRow FindRow = ListOfWindows.Rows.Find(NameOfWindow);
                if (FindRow != null)
                {
                    //
                    ((Form) FindRow["Form"]).Text = NewCaption;
                    //
                    ListOfWindows.AcceptChanges();
                    //
                    RefreshingMenuItems();
                }
            }
        }

        // Is Shown
        public bool IsShown(string NameOfWindow)
        {
            //
            if ((NameOfWindow != null) && (NameOfWindow != ""))
            {
                DataRow FindRow = ListOfWindows.Rows.Find(NameOfWindow);
                if (FindRow != null)
                {
                    if ((bool)FindRow["Showing"])
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }

        // Getting Window
        public Form GettingWindow(string NameOfWindow)
        {
            //
            if ((NameOfWindow != null) && (NameOfWindow != ""))
            {
                DataRow FindRow = ListOfWindows.Rows.Find(NameOfWindow);
                if (FindRow != null)
                    return ((Form)FindRow["Form"]);
                else
                    return null;
            }
            else
                return null;
        }

        #endregion

        // Focus Go To Form
        public void FocusGoToForm(string NameOfWindow)
        {
            //
            if ((NameOfWindow != null) && (NameOfWindow != ""))
            {
                DataRow FindRow = ListOfWindows.Rows.Find(NameOfWindow);
                if (FindRow != null)
                    ((Form)FindRow["Form"]).Focus();
            }
        }

        #region ' MenuItems '

        //
        private void RefreshingMenuItems()
        {
            //
            if (MenuItems != null)
            {
                //
                MenuItems.Clear();
                //
                foreach (DataRow CurrentWindow in ListOfWindows.Rows)
                    if ((bool)CurrentWindow["Showing"])
                    {
                        //
                        ToolStripMenuItem NewItem = new ToolStripMenuItem();
                        NewItem.Name = CurrentWindow["Key"].ToString();
                        NewItem.Text = ((Form) CurrentWindow["Form"]).Text;
                        NewItem.Tag = CurrentWindow["Form"];
                        NewItem.Click += new EventHandler(NewItem_Click);
                        //
                        MenuItems.Add(NewItem);
                    }
            }
        }

        private void NewItem_Click(object sender, EventArgs e)
        {
            //
            ((Form)((ToolStripMenuItem)sender).Tag).Focus();
        }

        #endregion

    }
}