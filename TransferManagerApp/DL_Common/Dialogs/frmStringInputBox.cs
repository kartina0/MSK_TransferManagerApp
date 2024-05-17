//----------------------------------------------------------
// Copyright c DATALINK 2017-
//
//----------------------------------------------------------
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
    /// <summary>
    /// @@20190916-1
    /// 文字入力BOX
    /// </summary>
    public partial class frmStringInputBox : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetAsyncKeyState(int vKey);


        private string _Msg = "";
        private string _Title = "";
        private Size _Size = new Size();

        /// <summary>
        /// @@20181005
        /// Yesボタンのテキスト
        /// </summary>
        private string _YesText = "決定";
        /// <summary>
        /// @@20181005
        /// Noボタンのテキスト
        /// </summary>
        private string _NoText = "戻る";

        /// <summary>
        /// 入力文字
        /// </summary>
        public string InputString = "";

        public frmStringInputBox(string title, string message, string defaultText, string yesText = null, string noText = null)
        {
            InitializeComponent();
            //lblMessage.Font = System.Drawing.SystemFonts.MessageBoxFont;
            //this.Font = System.Drawing.SystemFonts.MessageBoxFont;
            //btnOK.Font = System.Drawing.SystemFonts.MessageBoxFont;
            _Msg = message;
            _Title = title;
            InputString = defaultText;
            txtInput.Text = defaultText;
            
            if (yesText != null) _YesText = yesText;
            if (noText != null) _NoText = noText;

        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            tmrUpdateWindow.Stop(); // @@20160121-2
            InputString = txtInput.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;

        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            tmrUpdateWindow.Stop(); // @@20160121-2
            InputString = "";
            this.DialogResult = System.Windows.Forms.DialogResult.No;
        }

        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMessageBox_Load(object sender, EventArgs e)
        {
            lblMessage.Text = _Msg;
            int w = lblMessage.Width;
            this.Width = w + (25);
            this.Text = _Title;
            // @@20181005
            this.btnOK.Text = _YesText;
            this.btnNo.Text = _NoText;
            
            this.ActiveControl = txtInput;
            txtInput.SelectAll();

            using (Graphics gr = Graphics.FromHwnd(btnOK.Handle))
            {
                SizeF size = gr.MeasureString(_YesText, btnOK.Font);


                if (btnOK.Width < size.Width)
                {
                    int btn_width = (int)size.Width + 20;
                    //int offsetSize = btn_width - btnNo.Width;
                    btnOK.Width = btn_width;
                    //btnOK.Left -= offsetSize;
                }
                gr.Dispose();
            }

            using (Graphics gr = Graphics.FromHwnd(btnNo.Handle))
            {
                SizeF size = gr.MeasureString(_NoText, btnNo.Font);

                if (btnNo.Width < size.Width)
                {
                    int btn_width = (int)size.Width + 20;
                    int offsetSize = btn_width  - btnNo.Width;
                    btnNo.Width = btn_width;
                    btnNo.Left -= offsetSize;
                }
                gr.Dispose();
            }



            this.pictureBox1.Width = this.Width;
            this.txtInput.Top = lblMessage.Height + lblMessage.Top;
            this.pictureBox1.Height = this.txtInput.Bottom+10;


            if (this.pictureBox1.Height < 80)
                this.pictureBox1.Height = 80;

            if (this.Width < 300) this.Width = 300;
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
            catch{}

            this.Invoke((MethodInvoker)(() =>
            {
                FootSwitchKeyCheck();
            }));
        }

        /// <summary>
        /// @@@20160121-2
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
                    this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                    tmrUpdateWindow.Stop();
                }
                if (noKey)
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.No;
                    tmrUpdateWindow.Stop();
                }
            }
            catch { }
        }


    }
}
