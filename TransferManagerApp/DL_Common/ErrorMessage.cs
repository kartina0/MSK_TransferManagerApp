//----------------------------------------------------------
// Copyright © 2017 DATALINK
//----------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using ErrorCodeDefine;

namespace DL_CommonLibrary
{
    public partial class ErrorMessage : Form
    {
        private bool m_Unload = false;


        public ErrorMessage()
        {
            InitializeComponent();
        }

        private void ErrorMessage_Load(object sender, EventArgs e)
        {

            btnResetError.Visible = false;
            btnResetError.Enabled = false;
            
        }

        /// <summary>
        /// メッセージ表示
        /// </summary>
        /// <param name="err"></param>
        public void ShowMessage(string msgFileDir, ErrorCodeList err)
        {
            try
            {
                string path  = System.IO.Path.GetFullPath(msgFileDir);
                string fname = System.IO.Path.Combine(path,string.Format("0x{0:X08}", (UInt32)err));
                string detailMsg = "";
                m_Unload = false;

                if (FileIo.ExistFile(fname))
                {
                    detailMsg = System.IO.File.ReadAllText(fname, System.Text.Encoding.Default);
                }

                lblErrorCode.Text   = string.Format("0x{0:X08}", (UInt32)err);
                lblMessage.Text     = detailMsg;
                this.TopMost = true;
                this.Show();

                while (true)
                {
                    Application.DoEvents();

                    if (m_Unload)
                        break;

                    if (this.IsDisposed)
                        break;
                }

                this.Close();
            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
            }
        }


        public static string GetErrorMessage(string msgFileDir, ErrorCodeList err)
        {
            string detailMsg = "";
            try
            {
                string path = System.IO.Path.GetFullPath(msgFileDir);
                string fname = System.IO.Path.Combine(path, string.Format("0x{0:X08}", (UInt32)err));

                if (FileIo.ExistFile(fname))
                {
                    detailMsg = System.IO.File.ReadAllText(fname, System.Text.Encoding.Default);
                }
            }
            catch { }
            return detailMsg;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                m_Unload = true;
                this.Close();
 
            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
            }
        }

        private void btnResetError_Click(object sender, EventArgs e)
        {

        }

    }
}
