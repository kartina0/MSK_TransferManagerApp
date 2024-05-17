// ----------------------------------------------
// Copyright © 2017 DATALINK
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace DL_CustomCtrl
{

    public partial class ColorCheckBox : CheckBox
    {

        /// <summary>
        /// ON時の背景色
        /// </summary>
        private Color m_BackColor_ON = Color.FromName("Control");
        /// <summary>
        /// OFF時の背景色
        /// </summary>
        private Color m_BackColor_OFF = Color.FromName("Control");

        /// <summary>
        /// ON時の文字色
        /// </summary>
        private Color m_ForeColor_ON = Color.Black;

        /// <summary>
        /// OFF時の文字色
        /// </summary>
        private Color m_ForeColor_OFF = Color.Black;

        /// <summary>
        /// ON時のテキスト
        /// </summary>
        private string m_Text_ON = "";

        /// <summary>
        /// OFF時のテキスト
        /// </summary>
        private string m_Text_OFF = "";


        /// <summary>
        /// 点滅用タイマ
        /// </summary>
        private System.Threading.Timer m_Blink;

        /// <summary>
        /// 初回確認
        /// </summary>
        private bool _firstLoad=false;

        /// <summary>
        /// ハンドル作成
        /// </summary>
        protected override void CreateHandle()
        {
            base.CreateHandle();
            return;

            // ライセンス確認
            if (this.DesignMode && !LicenceCheck.IsLicence(this.GetType().Name))
            {
                if (!_firstLoad)
                {
                    MessageBox.Show("ライセンスが無い為、使用できません");
                }
                _firstLoad = true;
            }
            else
            {
                base.CreateHandle();
            }
        }


        [Category("カスタム")]
        [Description("ON時の背景色")]
        public Color BACK_COLOR_ON
        {
            get { return m_BackColor_ON; }
            set
            {
                m_BackColor_ON = value;
                // 表示更新
                UpdateDisplay();
            }
        }
        [Category("カスタム")]
        [Description("OFF時の背景色")]
        public Color BACK_COLOR_OFF
        {
            get { return m_BackColor_OFF; }
            set
            {
                m_BackColor_OFF = value;
                // 表示更新
                UpdateDisplay();
            }
        }


        [Category("カスタム")]
        [Description("ON時の文字色")]
        public Color FORE_COLOR_ON
        {
            get { return m_ForeColor_ON; }
            set
            {
                m_ForeColor_ON = value;
                // 表示更新
                UpdateDisplay();
            }
        }
        [Category("カスタム")]
        [Description("OFF時の文字色")]
        public Color FORE_COLOR_OFF
        {
            get { return m_ForeColor_OFF; }
            set
            {
                m_ForeColor_OFF = value;
                // 表示更新
                UpdateDisplay();
            }
        }

        [Category("カスタム")]
        [Description("ON時のテキスト")]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        public string TEXT_ON
        {
            get { return m_Text_ON; }
            set
            {
                m_Text_ON = value;
            }
        }
        [Category("カスタム")]
        [Description("OFF時のテキスト")]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        public string TEXT_OFF
        {
            get { return m_Text_OFF; }
            set
            {
                m_Text_OFF = value;
            }
        }

        public ColorCheckBox()
        {
            InitializeComponent();
            this.Appearance = System.Windows.Forms.Appearance.Button;
            this.BackColor = m_BackColor_OFF;
            this.ForeColor = m_ForeColor_OFF;
            this.Text      = m_Text_OFF;
            this.Appearance = System.Windows.Forms.Appearance.Button;
            this.TextAlign = ContentAlignment.MiddleCenter;

        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            // 表示更新
            UpdateDisplay();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);
            
            // 表示更新
            UpdateDisplay();

        }

        /// <summary>
        /// 表示更新
        /// </summary>
        private void UpdateDisplay()
        {
            if (this.Checked)
            {
                this.BackColor = m_BackColor_ON;
                this.ForeColor = m_ForeColor_ON;
                this.Text = m_Text_ON;
            }
            else
            {
                this.BackColor = m_BackColor_OFF;
                this.ForeColor = m_ForeColor_OFF;
                this.Text = m_Text_OFF;
            }
        }
    }

   
}
