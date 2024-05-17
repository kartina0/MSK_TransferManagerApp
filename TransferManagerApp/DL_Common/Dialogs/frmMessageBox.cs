using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DL_CommonLibrary
{
    public partial class frmMessageBox : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetAsyncKeyState(int vKey);


        private string m_Msg = "";
        private string m_Title = "";
        private Size m_Size = new Size();
        private Icon _icon = null;
        public frmMessageBox(string title, string message,Icon icon = null)
        {
            InitializeComponent();
            //lblMessage.Font = System.Drawing.SystemFonts.MessageBoxFont;
            //this.Font = System.Drawing.SystemFonts.MessageBoxFont;
            //btnOK.Font = System.Drawing.SystemFonts.MessageBoxFont;


            _icon = icon;
            m_Msg = message;
            m_Title = title;
        }

        public void ShowMessage()
        {
            lblMessage.Text = m_Msg;
            int w = lblMessage.Width;
            this.Width = w + (25);
            this.Text = m_Title;
            //Application.DoEvents();
            this.ShowDialog();
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            tmrUpdateWindow.Stop(); // @@20160121-2
            this.DialogResult = System.Windows.Forms.DialogResult.OK;

        }

        private void frmMessageBox_Load(object sender, EventArgs e)
        {
            int w = lblMessage.Width;
            this.Width = w + (25);
            this.Text = m_Title;


            if (_icon != null)
            {
                Bitmap iconImage = new Bitmap(32, 32);
                Graphics g = Graphics.FromImage(iconImage);
                g.DrawIcon(_icon, 0, 0);
                picIcon.Image = iconImage;
                g.Dispose();
            }

            
            this.pictureBox1.Width = this.Width;
            this.pictureBox1.Height = lblMessage.Height + lblMessage.Top;
            if (this.pictureBox1.Height < 80)
                this.pictureBox1.Height = 80;

            if (this.Width < 200) this.Width = 200;
            if (this.Height < 160) this.Height = 160;

            this.Height = pictureBox1.Height + 90;

            // @@20160121-1
            // 一度関数をコールしてキー押下をクリアしておく
            GetAsyncKeyState((int)Keys.Y);
            GetAsyncKeyState((int)Keys.N);

            tmrUpdateWindow.Start();    // @@20160121-2
        }

        private void tmrUpdateWindow_Tick(object sender, EventArgs e)
        {
            try
            {
                this.Height = pictureBox1.Height + 90;
            }
            catch { }

            if (this.Handle != IntPtr.Zero)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    FootSwitchKeyCheck();
                }));
            }
        }

        /// <summary>
        /// フットスイッチのキーを確認
        /// </summary>
        private void FootSwitchKeyCheck()
        {
            try
            {
                bool okKey = false;
                okKey = GetAsyncKeyState((int)Keys.Y) != 0;
                bool noKey = false;
                noKey = GetAsyncKeyState((int)Keys.N) != 0;
                if (okKey)
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    tmrUpdateWindow.Stop();
                }
            }
            catch { }
        }

    }
}
