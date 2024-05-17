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
using System.Threading;

namespace DL_CustomCtrl
{
    public partial class ColorRadioButton : RadioButton
    {
        public enum STATE
        {
            OFF = 0,
            ON,
            BUSY,
        }

        /// <summary>
        /// ON時の背景色
        /// </summary>
        private Color m_BackColor_ON  = Color.FromName("Control");
        /// <summary>
        /// OFF時の背景色
        /// </summary>
        private Color m_BackColor_OFF = Color.FromName("Control");
        /// <summary>
        /// Busy時の背景色
        /// </summary>
        private Color m_BusyColor = Color.Yellow;

        /// <summary>
        /// ON時の文字色
        /// </summary>
        private Color m_ForeColor_ON = Color.Black;
        /// <summary>
        /// OFF時の文字色
        /// </summary>
        private Color m_ForeColor_OFF = Color.Black;
        /// <summary>
        /// Busy時の文字色
        /// </summary>
        private Color m_BusyForeColor = Color.Black;


        /// <summary>
        /// 点滅用タイマ
        /// </summary>
        private System.Threading.Timer m_Blink;

        /// <summary>
        /// ON時のテキスト
        /// </summary>
        private string m_OnText = "";

        /// <summary>
        /// OFF時のテキスト
        /// </summary>
        private string m_OffText = "";

        /// <summary>
        /// @@20190917
        /// Enable = false時に色を変更するモード
        /// </summary>
        private bool m_EnabledColorChangeMode = false;

        /// <summary>
        /// 無効時のテキストの色
        /// </summary>
        private Color m_DisableForeColor = Color.Gray;

        /// <summary>
        /// 元の文字色
        /// </summary>
        private Color m_OriginalForeColor = Color.DimGray;
        /// <summary>
        /// 無効時のボタン色
        /// </summary>
        private Color m_DisableColor = Color.Gray;

        [Category("カスタム")]
        [Description("Enable = False時の背景色")]
        public Color DisableColor
        {
            get { return m_DisableColor; }
            set
            {
                m_DisableColor = value;
            }
        }
        [Category("カスタム")]
        [Description("Enable = False時のテキスト色")]
        public Color DisableForeColor
        {
            get { return m_DisableForeColor; }
            set
            {
                m_DisableForeColor = value;
            }
        }

        [Category("カスタム")]
        [Description("Enable = false時に色を変更するモード")]
        public bool EnabledColorChangeMode
        {
            get { return m_EnabledColorChangeMode; }
            set
            {
                m_EnabledColorChangeMode = value;
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
                if (this.Checked)
                {
                    this.BackColor = m_BackColor_ON;
                    this.ForeColor = m_ForeColor_ON;
                }
                else
                {
                    this.BackColor = m_BackColor_OFF;
                    this.ForeColor = m_ForeColor_OFF;
                }
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
                if (this.Checked)
                {
                    this.BackColor = m_BackColor_ON;
                    this.ForeColor = m_ForeColor_ON;
                }
                else
                {
                    this.BackColor = m_BackColor_OFF;
                    this.ForeColor = m_ForeColor_OFF;
                }
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
                if (this.Checked)
                {
                    this.BackColor = m_BackColor_ON;
                    this.ForeColor = m_ForeColor_ON;
                }
                else
                {
                    this.BackColor = m_BackColor_OFF;
                    this.ForeColor = m_ForeColor_OFF;
                }
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
                if (this.Checked)
                {
                    this.BackColor = m_BackColor_ON;
                    this.ForeColor = m_ForeColor_ON;
                }
                else
                {
                    this.BackColor = m_BackColor_OFF;
                    this.ForeColor = m_ForeColor_OFF;
                }
            }
        }


        [Localizable(true)]
        [Category("表示")]
        [Description("ActiveCtrl = OFF時の文字")]
        public string Text_OFF
        {
            get { return m_OffText; }
            set { m_OffText = value; }
        }
        [Category("表示")]
        [Description("ActiveCtrl = ON時の文字")]
        public string Text_ON
        {
            get { return m_OnText; }
            set { m_OnText = value; }
        }

        private STATE m_ActiveCtrl;
        [Category("表示")]
        [DefaultValue(STATE.OFF)]
        [Description("コントロールの表示を切り替えます")]
        public STATE ActiveCtrl
        {
            get { return m_ActiveCtrl; }
            set
            {
                this.m_ActiveCtrl = value;
                if (m_ActiveCtrl == STATE.ON)
                {
                    this.Text = m_OnText;
                    this.BackColor = m_BackColor_ON;
                    this.ForeColor = m_ForeColor_ON;
                }
                else
                {
                    this.Text = m_OffText;
                    this.BackColor = m_BackColor_OFF;
                    this.ForeColor = m_ForeColor_OFF;
                }
            }
        }


        public ColorRadioButton()
        {
            InitializeComponent();
            this.Appearance = System.Windows.Forms.Appearance.Button;
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.AutoSize = false;
        }

        /// <summary>
        /// 初回確認
        /// </summary>
        private bool _firstLoad = false;

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

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (this.Checked)
            {
                this.BackColor = m_BackColor_ON;
                this.ForeColor = m_ForeColor_ON;
            }
            else
            {
                this.BackColor = m_BackColor_OFF;
                this.ForeColor = m_ForeColor_OFF;
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        /// <summary>
        /// @@20190917
        /// 有効無効切り替え
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            if (this.Checked)
            {
                this.BackColor = m_BackColor_ON;
                this.ForeColor = m_ForeColor_ON;
            }
            else
            {
                if (this.Enabled)
                {
                    this.BackColor = m_BackColor_OFF;
                    this.ForeColor = m_ForeColor_OFF;
                }
                else
                {
                    this.BackColor = m_DisableColor;
                    this.ForeColor = m_DisableForeColor;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);

            if (this.Checked)
            {
                this.BackColor = m_BackColor_ON;
                this.ForeColor = m_ForeColor_ON;
            }
            else
            {
                if (this.Enabled)
                {
                    this.BackColor = m_BackColor_OFF;
                    this.ForeColor = m_ForeColor_OFF;
                }
                else
                {
                    this.BackColor = m_DisableColor;
                    this.ForeColor = m_DisableForeColor;
                }
            }
        }






    }
}
