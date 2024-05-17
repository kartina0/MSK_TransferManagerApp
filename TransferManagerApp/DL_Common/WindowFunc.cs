// ----------------------------------------------
// Copyright © 2017 DATALINK
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace DL_CommonLibrary
{
    public class WindowFunc
    {

        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("User32.dll")]
        private extern static bool PrintWindow(System.IntPtr hWnd, System.IntPtr dc, uint reservedFlag);
        [DllImport("User32.dll")]
        private extern static bool GetWindowRect(IntPtr hWnd, out RECT rect);

        /// <summary>
        /// 指定したプロセス名が含まれるウィンドウのScreenDumpを保存する
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="fname"></param>
        public static void PrintScreen(string procName, string fname)
        {
            RECT rect = new RECT();

            foreach (Process p in Process.GetProcesses())
            {
                if (p.MainWindowHandle != IntPtr.Zero)
                {
                    //Console.WriteLine(p.ProcessName + ":" + p.MainWindowTitle);
                    if (p.ProcessName.IndexOf(procName) >= 0)
                    {
                        GetWindowRect(p.MainWindowHandle, out rect);
                        Bitmap img = new Bitmap((rect.right - rect.left), (rect.bottom - rect.top));
                        Graphics memg = Graphics.FromImage(img);
                        IntPtr dc = memg.GetHdc();
                        PrintWindow(p.MainWindowHandle, dc, 0);
                        memg.ReleaseHdc(dc);
                        memg.Dispose();
                        img.Save(fname);
                        img.Dispose();
                    }
                }
            }

        }

        /// <summary>
        /// ソフトウェアキーボード表示
        /// </summary>
        /// <param name="show"></param>
        public static void ShowSoftKeybpord(bool show)
        {
            try
            {
                string procName = "osk";
                System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(procName);
                if (processes.Length == 0)
                {
                    System.Diagnostics.Process.Start(procName + ".exe");
                }
                
            }
            catch { }
        }
        /// <summary>
        /// 入力値検査
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void CheckKeyPressValue<T>(object sender, KeyPressEventArgs e, bool signed)
        {
            TextBox ctrl = (TextBox)sender;
            try
            {
                if (typeof(T) != typeof(string))
                {
                    if (e.KeyChar == '\b')
                        e.Handled = false;
                    else if (e.KeyChar >= '0' && e.KeyChar <= '9')
                        e.Handled = false;
                    else
                    {
                        if (e.KeyChar == '-' && ctrl.SelectionStart == 0 && signed)
                        {
                            e.Handled = false;
                        }

                        else if (e.KeyChar == '.' && typeof(T) == typeof(double))
                        {
                            if (ctrl.Text.IndexOf('.') >= 0)
                                e.Handled = true;
                        }
                        else
                        {
                            e.Handled = true;
                        }
                    }
                    if (ctrl.Text != "" && ctrl.Text != "-")
                    {
                        double d = double.Parse(ctrl.Text);
                    }
                }
            }
            catch { e.Handled = true; }
        }

        /// <summary>
        /// 入力値検査
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void CheckKeyPressValueHex(object sender, KeyPressEventArgs e)
        {
            TextBox ctrl = (TextBox)sender;
            try
            {

                if (e.KeyChar == '\b')
                    e.Handled = false;
                else if (e.KeyChar >= '0' && e.KeyChar <= '9')
                    e.Handled = false;
                else if (e.KeyChar >= 'A' && e.KeyChar <= 'F')
                    e.Handled = false;
                else if (e.KeyChar >= 'a' && e.KeyChar <= 'f')
                    e.Handled = false;
                else
                    e.Handled = true;

            }
            catch { e.Handled = true; }
        }

        /// ---------------------------------------------------------------------------------------
        /// <summary>
        /// Search control by control name.<BR/>
        ///     指定したコントロール内に含まれるコントロールを指定した名前で検索します。</summary>
        /// <param name="hParent">
        /// target parent control<BR/>
        ///     検索対象となる親コントロール。</param>
        /// <param name="stName">
        /// search control name.<BR/>
        ///     検索するコントロールの名前。</param>
        /// <returns>
        /// find control instance.<BR/>
        ///     取得したコントロールのインスタンス。取得できなかった場合は null。</returns>
        /// ---------------------------------------------------------------------------------------
        public static System.Windows.Forms.Control FindControl(System.Windows.Forms.Control hParent, string stName)
        {
            // hParent 内のすべてのコントロールを列挙する
            foreach (System.Windows.Forms.Control hControl in hParent.Controls)
            {
                // 列挙したコントロールにコントロールが含まれている場合は再帰呼び出しする
                if (hControl.HasChildren)
                {
                    System.Windows.Forms.Control hFindControl = FindControl(hControl, stName);

                    // 再帰呼び出し先でコントロールが見つかった場合はそのまま返す
                    if (hFindControl != null)
                    {
                        return hFindControl;
                    }
                }

                // コントロール名が合致した場合はそのコントロールのインスタンスを返す
                if (hControl.Name == stName)
                {
                    return hControl;
                }
            }

            return null;
        }

        /// <summary>
        /// フォルダを開くダイアログ
        /// </summary>
        /// <param name="defaultDir"></param>
        /// <returns></returns>
        public static string ShowFolderDialog(string defaultDir, IWin32Window owner)
        {
            string dir = "";
            try
            {
                FolderBrowserDialog fb = new FolderBrowserDialog();
                fb.SelectedPath = defaultDir;
                DialogResult res = fb.ShowDialog(owner);
                if (res != DialogResult.Cancel)
                {
                    dir = fb.SelectedPath;
                }
            }
            catch { dir = ""; }
            return dir;
        }

        public static string ShowFileOpenDialog(string defaultDir, string filter, int filterIndex, IWin32Window owner)
        {
            string fname = "";
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();

                dlg.InitialDirectory = defaultDir;
                dlg.Filter = filter;
                dlg.FilterIndex = filterIndex;
                DialogResult res = dlg.ShowDialog(owner);
                if (res != DialogResult.Cancel)
                {
                    fname = dlg.FileName;
                }

            }
            catch { fname = ""; }
            return fname;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultDir"></param>
        /// <param name="filter"></param>
        /// <param name="filterIndex"></param>
        /// <returns></returns>
        public static string ShowFileSelectDialog(string defaultDir, string filter, int filterIndex, IWin32Window owner)
        {
            string fname = "";
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();

                dlg.InitialDirectory = defaultDir;
                dlg.Filter = filter;
                dlg.FilterIndex = filterIndex;

                DialogResult res = dlg.ShowDialog(owner);
                if (res != DialogResult.Cancel)
                {
                    fname = dlg.FileName;
                }
            }
            catch { fname = ""; }
            return fname;
        }
        public static string ShowFileSaveDialog(string defaultDir, string filter, int filterIndex, IWin32Window owner)
        {
            string fname = "";
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.InitialDirectory = defaultDir;
                dlg.Filter = filter;
                dlg.FilterIndex = filterIndex;
                DialogResult res = dlg.ShowDialog(owner);
                if (res != DialogResult.Cancel)
                {
                    fname = dlg.FileName;
                }
                else
                {
                    fname = "";
                }
            }
            catch { fname = ""; }
            return fname;
        }

        /// <summary>
        /// 数値入力されているか確認
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        public static bool IsNumber(TextBox ctrl)
        {
            bool rs = true;
            try
            {
                double d = double.Parse(ctrl.Text);
            }
            catch { rs = false; }
            return rs;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="busy"></param>
        public static void SetValueLabel(Label ctrl, string v , bool busy)
        {
            try
            {
                ctrl.Text = v;
                if (busy)
                    ctrl.BackColor = Color.Yellow;
                else
                    ctrl.BackColor = Color.Transparent;
            }
            catch { }
        }
        /// <summary>
        /// ｴﾗｰ状態を表示するﾗﾍﾞﾙの表示色変更
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="on"></param>
        public static void SetErrorSignalLabel(Control ctrl, bool on)
        {
            try
            {
                if (on)
                    ctrl.BackColor = Color.Red;
                else
                    ctrl.BackColor = Color.Lime;
            }
            catch { }
        }


        public static void SetSignalLabel(Control ctrl, bool on)
        {
            try
            {
                if (on)
                    ctrl.BackColor = Color.Aqua;
                else
                    ctrl.BackColor = Color.DarkGray;
            }
            catch { }
        }

        public static void SetButtonStatus(Control ctrl, Color onColor, bool on , bool error)
        {
            try
            {
                if (error)
                {
                    ctrl.BackColor = Color.Red;
                }
                else
                {
                    if (on)
                        ctrl.BackColor = onColor;
                    else
                        ctrl.BackColor = Color.Gray;
                }
            }
            catch { }
        }

        public static void SetButtonEnable(Control ctrl, Color onColor, bool enable, bool error)
        {
            try
            {
                ctrl.Enabled = enable;
                if (error)
                {
                    ctrl.BackColor = Color.Red;

                }
                else
                {
                    
                    if (enable)
                        ctrl.BackColor = onColor;
                    else
                        ctrl.BackColor = Color.Gray;
                }
                
            }
            catch { }
        }


        public static void SetOperationStep(Control ctrl, bool on, bool error)
        {
            try
            {
                if (error)
                {
                    ctrl.BackColor = Color.Red;
                }
                else
                {
                    if (on)
                        ctrl.BackColor = Color.PapayaWhip;
                    else
                        ctrl.BackColor = Color.Gray;
                }
            }
            catch { }
        }

        public static void SetOperationStep(Control ctrl, Color onColor,  bool on, bool error)
        {
            try
            {
                if (error)
                {
                    ctrl.BackColor = Color.Red;
                }
                else
                {
                    if (on)
                        ctrl.BackColor = onColor;
                    else
                        ctrl.BackColor = Color.Gray;
                }
            }
            catch { }
        }

        public static void SetPositionText(Control ctrl, double pos)
        {
            try
            {
                ctrl.Text = string.Format("{0:0.00}", pos);
            }
            catch { }
        }


        public static void SetPositionTextWithDiffColor(Control ctrl, double pos)
        {
            try
            {
                string s = string.Format("{0:000.00}", pos);

                if (ctrl.Text != s)
                {
                    ctrl.BackColor = Color.Aqua;
                }
                else
                {
                    ctrl.BackColor = Color.FromName("Control");
                }
                ctrl.Text = string.Format("{0:000.00}", pos);

            }
            catch { }
        }

        public static void SetValueText(Control ctrl, Int16 val, bool error)
        {
            try
            {
                if (error)
                {
                    ctrl.ForeColor = Color.Red;
                    ctrl.Text = "----";
                }
                else
                {
                    ctrl.ForeColor = Color.Black;
                    ctrl.Text = string.Format("{0:0}", val);
                }
            }
            catch { }
        }

        public static void SetValueText(Control ctrl, Int32 val, bool error)
        {
            try
            {
                if (error)
                {
                    ctrl.ForeColor = Color.Red;
                    ctrl.Text = "----";
                }
                else
                {
                    ctrl.ForeColor = Color.Black;
                    ctrl.Text = string.Format("{0:0}", val);
                }
            }
            catch { }
        }
        public static void SetStringText(Control ctrl, string text, bool error)
        {
            try
            {
                if (error)
                {
                    ctrl.ForeColor = Color.Red;
                    ctrl.Text = "----";
                }
                else
                {
                    ctrl.ForeColor = Color.Black;
                    ctrl.Text = text;
                }

            }
            catch { }
        }

        public static void SetStringTextWithDiffColor(Control ctrl, string text, bool error)
        {
            try
            {
                if (error)
                {
                    ctrl.ForeColor = Color.Red;
                    ctrl.Text = "----";
                }
                else
                {
                    if (ctrl.Text != text)
                    {
                        ctrl.ForeColor = Color.Blue;
                        ctrl.Text = text;
                    }
                    else
                    {
                        ctrl.ForeColor = Color.Black;
                        ctrl.Text = text;
                    }
                }

            }
            catch { }
        }

        /// <summary>
        /// 文字が異なる場合は青字表示
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="text"></param>
        /// <param name="error"></param>
        public static void SetTextColor(Control ctrl, string text, bool error)
        {
            try
            {
                if (error)
                    ctrl.ForeColor = Color.Red;
                else
                    if (ctrl.Text != text)
                        ctrl.ForeColor = Color.Blue;
                    else
                        ctrl.ForeColor = Color.Black;
            
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="text"></param>
        /// <param name="error"></param>
        public static void SetTextColor(Control ctrl, int val, bool error)
        {
            try
            {
                int v = int.Parse(ctrl.Text);
                if (error)
                    ctrl.ForeColor = Color.Red;
                else
                    if (v != val)
                        ctrl.ForeColor = Color.Blue;
                    else
                        ctrl.ForeColor = Color.Black;

            }
            catch { }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="text"></param>
        /// <param name="error"></param>
        public static void SetTextColor(Control ctrl, double val, bool error)
        {
            try
            {
                double v = double.Parse(ctrl.Text);
                if (error)
                    ctrl.ForeColor = Color.Red;
                else
                    if (v != val)
                        ctrl.ForeColor = Color.Blue;
                    else
                        ctrl.ForeColor = Color.Black;

            }
            catch { }
        }
        /// <summary>
        /// 別スレッドから委譲先を指定してメッセージボックス表示
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="msg"></param>
        public static void DisplayMessageBox(Form owner, string msg)
        {
            try
            {
                if (owner.InvokeRequired)
                    owner.Invoke(new delegate_display_messageBox(_DisplayMessageBox), msg);
                else
                    _DisplayMessageBox(msg);
            }
            catch (Exception ex) { ErrorManager.ErrorHandler(ex); }
        }

        public static void _DisplayMessageBox(string msg)
        {
            try
            {
                MessageBox.Show(msg);
            }
            catch (Exception ex) { ErrorManager.ErrorHandler(ex); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="label"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public static void InitCartridgeInfoControl(DataGridView ctrl, int startIndex, int count)
        {

            try
            {
                DataSet dtSet = new DataSet(ctrl.Name);
                DataTable dtTable = null;
                DataColumn dtCol = null;


                int currentTopIndex = 0;
                try
                {
                    currentTopIndex = ctrl.FirstDisplayedCell.RowIndex;
                }
                catch { }


                try
                {

                    dtSet = new DataSet(ctrl.Name);
                    dtTable = dtSet.Tables.Add(string.Format("table_{0}", ctrl.Name));

                    dtCol = dtTable.Columns.Add("状態");

                    ctrl.DataSource = dtSet;
                    ctrl.DataMember = string.Format("table_{0}", ctrl.Name);

                    for (int i = 0; i < count -1 ; i++)
                    {
                        dtTable.Rows.Add();
                    }

                }
                catch { ctrl.Enabled = false; }

                ctrl.DataSource = dtTable;

               // ctrl.AlternatingRowsDefaultCellStyle.BackColor = Color.Bisque;

                ctrl.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (ctrl.Rows.Count > currentTopIndex)
                    ctrl.FirstDisplayedScrollingRowIndex = currentTopIndex;

                for (int i = 0; i < ctrl.Columns.Count; i++)
                {

                    ctrl.Columns[i].ReadOnly = true;

                    ctrl.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                   // ctrl.Columns[i].DefaultCellStyle.SelectionBackColor = Color.Transparent;
                   // ctrl.Columns[i].DefaultCellStyle.SelectionForeColor = Color.Black;
                }

                for (int i = 0; i < ctrl.Rows.Count; i++)
                {
                    ctrl.Rows[i].HeaderCell.Value = string.Format("{0:00}", startIndex + i + 1);
                }
                ctrl.RowHeadersWidth = 70;
            }
            catch { }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="label"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public static void InitFileInfoControl(DataGridView ctrl, string[] label, Type[] type, string value, bool readOnly, int initialRowCount)
        {

            try
            {
                DataSet dtSet = new DataSet(ctrl.Name);
                DataTable dtTable = null;
                DataColumn dtCol = null;


                int currentTopIndex = 0;
                try
                {
                    currentTopIndex = ctrl.FirstDisplayedCell.RowIndex;
                }
                catch { }


                try
                {

                    dtSet = new DataSet(ctrl.Name);
                    dtTable = dtSet.Tables.Add(string.Format("table_{0}", ctrl.Name));

                    for (int i = 0; i < label.Length; i++)
                    {
                        dtCol = dtTable.Columns.Add(label[i], type[i]);
                    }

                    ctrl.DataSource = dtSet;
                    ctrl.DataMember = string.Format("table_{0}", ctrl.Name);


                    for (int i = 0; i < initialRowCount; i++)
                    {
                        dtTable.Rows.Add(value);
                    }

                }
                catch { ctrl.Enabled = false; }

                ctrl.DataSource = dtTable;


                ctrl.AlternatingRowsDefaultCellStyle.BackColor = Color.Bisque;

                ctrl.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                if (ctrl.Rows.Count > currentTopIndex)
                    ctrl.FirstDisplayedScrollingRowIndex = currentTopIndex;

                for (int i = 0; i < ctrl.Columns.Count; i++)
                {
   
                    ctrl.Columns[i].ReadOnly = readOnly;
        
                    ctrl.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;

                    if (i > 0 && i < 3)
                        ctrl.Columns[i].Width = 130;
                    else if (i == 3)
                        ctrl.Columns[i].Width = 110;
                    else
                        ctrl.Columns[i].Width = 90;
                }
            }
            catch { }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="columnCount"></param>
        /// <param name="initialRowCount"></param>
        public static void InitDataGridView(DataGridView ctrl, int columnCount, int initialRowCount, bool readOnly)
        {
            try
            {
                DataSet dtSet = new DataSet(ctrl.Name);
                DataTable dtTable = null;
                DataColumn dtCol = null;


                int currentTopIndex = 0;
                try
                {
                    currentTopIndex = ctrl.FirstDisplayedCell.RowIndex;
                }
                catch { }


                try
                {

                    dtSet = new DataSet(ctrl.Name);
                    dtTable = dtSet.Tables.Add(string.Format("table_{0}", ctrl.Name));

                    for (int i = 0; i < columnCount; i++)
                    {
                        dtCol = dtTable.Columns.Add("",Type.GetType("System.String"));
                    }

                    ctrl.DataSource = dtSet;
                    ctrl.DataMember = string.Format("table_{0}", ctrl.Name);


                    for (int i = 0; i < initialRowCount -1; i++)
                    {
                        dtTable.Rows.Add("");
                    }
                }
                catch { ctrl.Enabled = false; }

                ctrl.DataSource = dtTable;


                ctrl.AlternatingRowsDefaultCellStyle.BackColor = Color.Bisque;

                ctrl.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                ctrl.ColumnHeadersVisible = false;
                if (ctrl.Rows.Count > currentTopIndex)
                    ctrl.FirstDisplayedScrollingRowIndex = currentTopIndex;

                for (int i = 0; i < ctrl.Columns.Count; i++)
                {
                    ctrl.Columns[i].ReadOnly = true;
                    ctrl.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;

                    if (i > 0 && i < 3)
                        ctrl.Columns[i].Width = 80;
                    else if (i == 3)
                        ctrl.Columns[i].Width = 80;
                    else
                        ctrl.Columns[i].Width = 80;
                }
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="columnName"></param>
        /// <param name="rowIndex"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetCellData(DataGridView ctrl, string columnName, int rowIndex, string value)
        {
            try
            {
                ctrl[columnName, rowIndex].Value = value;
            }
            catch { }
            return true;

        }
        /// <summary>
        /// 指定したセルのデータを変更する
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetCellData(DataGridView ctrl, int columnIndex, int rowIndex, string value)
        {
            try
            {
                ctrl[columnIndex, rowIndex].Value = value;
            }
            catch { }
            return true;
        }
        public static bool SetCellData(DataGridView ctrl, int columnIndex, int rowIndex, string value, Color backColor)
        {
            try
            {
                ctrl[columnIndex, rowIndex].Value = value;
                ctrl[columnIndex, rowIndex].Style.BackColor = backColor;
            }
            catch { }
            return true;
        }

        /// <summary>
        /// 指定したセルのデータを変更する
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="columnName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool SetCellData(DataGridView ctrl, string columnName, string[] values)
        {
            try
            {

                bool readOnly = ctrl.Columns[columnName].ReadOnly;
                ctrl.Columns[columnName].ReadOnly = false;

                for (int i = 0; i < values.Length; i++)
                {
                    if (ctrl.RowCount - 1 < i)
                        break;
                    ctrl[columnName, i].Value = values[i];
                }
                ctrl.Columns[columnName].ReadOnly = readOnly;

            }
            catch { }
            return true;
        }

        /// <summary>
        /// 指定したセルのデータを変更する
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="columnIndex"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool SetCellData(DataGridView ctrl, int columnIndex, string[] values)
        {
            try
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (ctrl.RowCount - 1 < i)
                    {
                        DataTable dtTable = (DataTable)ctrl.DataSource;
                        dtTable.Rows.Add(values[i]);

                    }
                    else
                    {
                        ctrl[columnIndex, i].Value = values[i];
                    }
                }


            }
            catch { }
            return true;
        }
        /// <summary>
        /// 指定したセルのデータを変更する
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="columnIndex"></param>
        /// <param name="Index"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool SetCellData(DataGridView ctrl, int columnIndex, int[] Index, string[] values)
        {
            try
            {
                for (int i = 0; i < Index.Length; i++)
                {
                    if (ctrl.RowCount - 1 < Index[i])
                        break;

                    ctrl[columnIndex, Index[i]].Value = values[i];
                }


            }
            catch { }
            return true;
        }

        /// <summary>
        /// 昇順で取得
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        public static int[] GetSelectedRowIndex(DataGridView ctrl)
        {
            System.Collections.SortedList list = new System.Collections.SortedList();
            int[] selectIndex = new int[ctrl.SelectedRows.Count];

            foreach (DataGridViewRow row in ctrl.SelectedRows)
            {   // selected line 
                list.Add(row.Index, row.Index);
            }

            for (int i = 0; i < list.Count; i++)
            {
                selectIndex[i] = (int)list.GetByIndex(i);
            }
            return selectIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public static string[] GetCellData(DataGridView ctrl, int columnIndex)
        {
            string[] s = new string[0];
            try
            {
                s = new string[ctrl.RowCount - 1];
                for (int i = 0; i < s.Length; i++)
                {
                    s[i] = "0";
                }

                for (int i = 0; i < s.Length; i++)
                {
                    s[i] = ctrl[columnIndex, i].Value.ToString();

                    if (columnIndex == 1)
                    {
                        double temp = double.Parse(s[i]);
                        s[i] = temp.ToString("0.000000");
                    }
                }

            }
            catch { }
            return s;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string[] GetCellData(DataGridView ctrl, string columnName)
        {
            string[] s = new string[0];
            try
            {
                s = new string[ctrl.RowCount];
                for (int i = 0; i < s.Length; i++)
                {
                    s[i] = "0";
                }

                for (int i = 0; i < s.Length; i++)
                {
                    s[i] = ctrl[columnName, i].Value.ToString();
                    if (columnName.ToUpper().Contains("RADIUS"))
                    {
                        double temp = double.Parse(s[i]);
                        s[i] = temp.ToString("0.000000");
                    }
                }

            }
            catch { }
            return s;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="columnIndex"></param>
        /// <param name="startRowIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string[] GetCellData(DataGridView ctrl, int columnIndex, int startRowIndex, int count)
        {
            string[] s = new string[0];
            try
            {
                s = new string[count];


                for (int i = 0; i < count; i++)
                {
                    s[i] = ctrl[columnIndex, i + startRowIndex].Value.ToString();
                }

            }
            catch { }
            return s;
        }

        public static string GetCellData(DataGridView ctrl, int columnIndex, int startRowIndex)
        {
            string s = "";
            try
            {
                s = ctrl[columnIndex, startRowIndex].Value.ToString();
            }
            catch { }
            return s;
        }

        /// <summary>
        /// @@20180509-1
        /// コントロール内の指定した種別のコントロールの値をファイルに保存する
        /// </summary>
        /// <param name="hParent"></param>
        /// <param name="type"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static void SaveControlSetting(System.Windows.Forms.Control hParent, Type type, string fname, string parentName)
        {
            try
            {
                string section = "";
                string key = "";
                string filePath = System.IO.Path.GetFullPath(fname);

                // hParent 内のすべてのコントロールを列挙する
                foreach (System.Windows.Forms.Control hControl in hParent.Controls)
                {
                    // 列挙したコントロールにコントロールが含まれている場合は再帰呼び出しする
                    if (hControl.HasChildren)
                    {
                        SaveControlSetting(hControl, type, filePath, parentName);
                    }
                    else
                    {

                        if (hControl.GetType() == type)
                        {
                            section = parentName;
                            key = hControl.Name;

                            if (typeof(Label) == type)
                                FileIo.WriteIniValue(filePath, section, key, hControl.Text);
                            if (typeof(TextBox) == type)
                                FileIo.WriteIniValue(filePath, section, key, hControl.Text);
                            if (typeof(CheckBox) == type)
                                FileIo.WriteIniValue(filePath, section, key, ((CheckBox)hControl).Checked ? "1" : "0");
                        }
                    }
                }
            }
            catch { }
        }
        /// <summary>
        /// @@20180509-1
        /// コントロール内の指定した種別のコントロールの値をファイルに保存する
        /// </summary>
        /// <param name="hParent"></param>
        /// <param name="type"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static void LoadControlSetting(System.Windows.Forms.Control hParent, Type type, string fname, string parentName)
        {
            try
            {
                string section = "";
                string key = "";
                string filePath = System.IO.Path.GetFullPath(fname);

                // hParent 内のすべてのコントロールを列挙する
                foreach (System.Windows.Forms.Control hControl in hParent.Controls)
                {
                    // 列挙したコントロールにコントロールが含まれている場合は再帰呼び出しする
                    if (hControl.HasChildren)
                    {
                        LoadControlSetting(hControl, type, filePath, parentName);
                    }
                    else
                    {

                        if (hControl.GetType() == type)
                        {
                            section = parentName;
                            key = hControl.Name;
                            string buf = "";
                            bool exist = FileIo.ReadIniFile<string>(filePath, section, key, ref buf);

                            if (exist)
                            {
                                if (typeof(Label) == type)
                                    hControl.Text = buf;
                                if (typeof(TextBox) == type)
                                    hControl.Text = buf;
                                if (typeof(CheckBox) == type)
                                    ((CheckBox)hControl).Checked = buf == "1" ? true : false;
                                if (typeof(RadioButton) == type)
                                    ((RadioButton)hControl).Checked = buf == "1" ? true : false;
                            }
                        }
                    }
                }
            }
            catch { }
        }
        /// <summary>
        /// @@20180509-1
        /// 指定したコントロールの値をファイルに保存する
        /// </summary>
        /// <param name="hControl">コントロール</param>
        /// <param name="fname">保存ファイル名</param>
        /// <param name="parentName">保存時のセクション名</param> 
        /// <returns></returns>
        public static void SaveControlSetting(System.Windows.Forms.Control hControl, string fname, string parentName)
        {
            try
            {
                string section = "";
                string key = "";
                string filePath = System.IO.Path.GetFullPath(fname);
                Type type = hControl.GetType();

                section = parentName;
                key = hControl.Name;

                if (typeof(Label) == type)
                    FileIo.WriteIniValue(filePath, section, key, hControl.Text);
                if (typeof(TextBox) == type)
                    FileIo.WriteIniValue(filePath, section, key, hControl.Text);
                if (typeof(CheckBox) == type)
                    FileIo.WriteIniValue(filePath, section, key, ((CheckBox)hControl).Checked ? "1" : "0");
                if (typeof(RadioButton) == type)
                    FileIo.WriteIniValue(filePath, section, key, ((RadioButton)hControl).Checked ? "1" : "0");
            }
            catch { }
        }


        /// <summary>
        /// @@20180509-1
        /// 指定したコントロールの値をファイルから取得する
        /// </summary>
        /// <param name="hControl">コントロール</param>
        /// <param name="fname">ファイル名</param>
        /// <param name="parentName">セクション名</param> 
        /// <returns></returns>
        public static void LoadControlSetting(System.Windows.Forms.Control hControl, string fname, string parentName)
        {
            try
            {
                string section = "";
                string key = "";
                string filePath = System.IO.Path.GetFullPath(fname);
                Type type = hControl.GetType();

                section = parentName;
                key = hControl.Name;

                string buf = "";
                bool exist = FileIo.ReadIniFile<string>(filePath, section, key, ref buf);

                if (exist)
                {
                    if (typeof(Label) == type)
                        hControl.Text = buf;
                    if (typeof(TextBox) == type)
                        hControl.Text = buf;
                    if (typeof(CheckBox) == type)
                        ((CheckBox)hControl).Checked = buf == "1" ? true : false;
                    if (typeof(RadioButton) == type)
                        ((RadioButton)hControl).Checked = buf == "1" ? true : false;
                }

            }
            catch { }
        }

        /// <summary>
        /// @@20190227-1
        /// フォームを表示するモニタを設定する
        /// </summary>
        /// <param name="index"></param>
        public static void SetDisplay(int displayIndex, Form ctrl)
        {
            try
            {
                //フォームを表示するディスプレイのScreenを取得する
                Screen[] list = Screen.AllScreens;
                // デフォルトLocation(フォームプロパティのStartPositionがManualになっている場合のみ有効)
                Point orgLocation = ctrl.Location;
                // ディスプレイが無い場合は１つ目のディスプレイに表示
                if (list.Count() <= displayIndex) displayIndex = 0;

                // プロパティ上のWindowStateを取得
                FormWindowState ws = ctrl.WindowState;
                // 最大表示時はディスプレイ切り替えられないのでNormalに変更する
                ctrl.WindowState = FormWindowState.Normal;
                //フォームの開始位置をディスプレイの左上座標に設定する
                ctrl.StartPosition = FormStartPosition.Manual;
                int x = list[displayIndex].Bounds.Location.X + orgLocation.X;
                int y = list[displayIndex].Bounds.Location.Y + orgLocation.Y;

                ctrl.Location = new Point(x, y);

                // プロパティで設定されているWindowStateに戻す
                ctrl.WindowState = ws;
            }
            catch { }
        }
    }


}
