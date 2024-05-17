using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace DL_CommonLibrary
{
    public static class Dialogs
    {
        static public void ShowInformationMessage(string message, string title, System.Drawing.Icon icon = null)
        {
            Title = title;

            // MessageBox.Show(message, Title, MessageBoxButtons.OK,icon);
            // MessageBox.Show(message, Title, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

            frmMessageBox dlg = new frmMessageBox(title, message, icon);
            dlg.ShowMessage();

        }
        static public void ShowInformationMessage(Form owner, string message, string title, System.Drawing.Icon icon = null)
        {
            Title = title;

            // MessageBox.Show(message, Title, MessageBoxButtons.OK,icon);
            owner.Invoke((MethodInvoker)(() =>
            {
                owner.Activate();
                //MessageBox.Show(message, Title, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                //MessageBox.Show(message, Title, MessageBoxButtons.YesNo, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                frmMessageBox dlg = new frmMessageBox(title, message, icon);
                dlg.TopMost = true;
                dlg.ShowMessage();
                owner.Activate();
            }));
        }

        static private string _title;
        static private string Title
        {
            get
            {
                if (string.IsNullOrEmpty(_title))
                {
                    _title = GetTitle();
                }
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        static private string GetTitle()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return Path.GetFileNameWithoutExtension(assembly.CodeBase);
        }
#if true
        /// <summary>
        /// 決定、取消メッセージボックス表示
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        static public bool ShowYesCancelMessage(string msg, string title, System.Drawing.Icon icon = null)
        {
            frmYesNoMessageBox dlg = new frmYesNoMessageBox(title, msg, icon, null, "取消");
            dlg.ShowMessage();
            return dlg.DialogResult == DialogResult.Yes;
        }

        /// <summary>
        /// @@20190703 
        /// メッセージボックス表示 ボタンのテキストを指定
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        static public bool ShowYesCancelMessage(Form owner, string msg, string title, string yesText, string noText, System.Drawing.Icon icon = null)
        {
            Title = title;
            bool ok = false;
            // MessageBox.Show(message, Title, MessageBoxButtons.OK,icon);
            owner.Invoke((MethodInvoker)(() =>
            {
                owner.Activate();
                //MessageBox.Show(message, Title, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                //MessageBox.Show(message, Title, MessageBoxButtons.YesNo, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                frmYesNoMessageBox dlg = new frmYesNoMessageBox(title, msg, icon, yesText, noText);
                dlg.TopMost = true;
                dlg.ShowMessage();
                ok = dlg.DialogResult == DialogResult.Yes;
                owner.Activate();
            }));
            return ok;
        }

        /// <summary>
        /// 決定、取消メッセージボックス表示
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        static public bool ShowYesCancelMessage(Form owner, string msg, string title, System.Drawing.Icon icon = null)
        {
            Title = title;
            bool ok = false;
            // MessageBox.Show(message, Title, MessageBoxButtons.OK,icon);
            owner.Invoke((MethodInvoker)(() =>
            {
                owner.Activate();
                //MessageBox.Show(message, Title, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                //MessageBox.Show(message, Title, MessageBoxButtons.YesNo, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                frmYesNoMessageBox dlg = new frmYesNoMessageBox(title, msg, icon, null, "取消");
                dlg.TopMost = true;
                dlg.ShowMessage();
                ok = dlg.DialogResult == DialogResult.Yes;
                owner.Activate();
            }));
            return ok;
        }
        /// <summary>
        /// Yes,No メッセージボックス表示
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        static public bool ShowYesNoMessage(string msg, string title, System.Drawing.Icon icon = null )
        {
            frmYesNoMessageBox dlg = new frmYesNoMessageBox(title, msg, icon);
            dlg.ShowMessage();
            return dlg.DialogResult == DialogResult.Yes;
        }
        /// <summary>
        /// @@20190703
        /// Yes,No メッセージボックス表示
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        static public bool ShowYesNoMessage(string msg, string title, string yesText, string noText, System.Drawing.Icon icon = null)
        {
            frmYesNoMessageBox dlg = new frmYesNoMessageBox(title, msg, icon, yesText, noText);
            dlg.ShowMessage();
            return dlg.DialogResult == DialogResult.Yes;
        }


        /// <summary>
        /// Yes,No メッセージボックス表示
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        static public bool ShowYesNoMessage(Form owner, string msg, string title, System.Drawing.Icon icon = null)
        {
            Title = title;
            bool ok = false;
            // MessageBox.Show(message, Title, MessageBoxButtons.OK,icon);
            owner.Invoke((MethodInvoker)(() =>
            {
                owner.Activate();
                //MessageBox.Show(message, Title, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                //MessageBox.Show(message, Title, MessageBoxButtons.YesNo, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                frmYesNoMessageBox dlg = new frmYesNoMessageBox(title, msg);
                dlg.TopMost = true;
                dlg.ShowMessage();
                ok = dlg.DialogResult == DialogResult.Yes;
                owner.Activate();
            }));
            return ok;
        }

#else
        /// <summary>
        /// Yes,No メッセージボックス表示
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        static public bool ShowYesNoMessage(string msg, string title,  MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            return ShowYesNoMessage(msg, title, true, icon);
        }

        /// <summary>
        /// Yes,No メッセージボックス表示
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        static public bool ShowYesNoMessage(Form owner, string msg, string title, MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            DialogResult res = DialogResult.No;
            owner.Invoke((MethodInvoker)(() =>
            {
                owner.Activate();
                res = MessageBox.Show(msg, Title, MessageBoxButtons.YesNo, icon, MessageBoxDefaultButton.Button2, MessageBoxOptions.DefaultDesktopOnly);
                owner.Activate();
            }));
            return res == DialogResult.Yes;
        }

        /// <summary>
        /// Yes,No メッセージボックス表示
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        static public bool ShowYesNoMessage(string msg, string title, bool modal = true, MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            Title = title;
            if (modal)
            {
                return DialogResult.Yes == MessageBox.Show(msg, Title, MessageBoxButtons.YesNo, icon, MessageBoxDefaultButton.Button2, MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                return DialogResult.Yes == MessageBox.Show(msg, Title, MessageBoxButtons.YesNo, icon, MessageBoxDefaultButton.Button2);
            }
        }
#endif

        /// <summary>
        /// @@20190916-1
        /// 文字列入力BOX
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        static public string ShowStringInputBox(string defaultText, string msg, string title, string yesText, string noText)
        {
            frmStringInputBox dlg = new frmStringInputBox(title, msg, defaultText, yesText, noText);
            dlg.ShowDialog();
            DialogResult res = dlg.DialogResult;
            if (res == DialogResult.Yes) return dlg.InputString;

            return "";
        }


    }
}
