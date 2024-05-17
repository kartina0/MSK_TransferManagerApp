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
    public partial class LongPushButton : Button
    {

        delegate void delegate_void();
        /// <summary>
        /// 長押し時間[ms]
        /// </summary>
        private int m_Time = 0;
        /// <summary>
        /// クリック時のボタン色
        /// </summary>
        private Color m_PushColor = Color.FromName("Control");

        private Color m_OrgBackColor = Color.FromName("Control");
        /// <summary>
        /// Enable = false時に色を変更するモード
        /// </summary>
        private bool m_EnabledColorChangeMode = false;
        /// <summary>
        /// クリック時のボタン色
        /// </summary>
        private Color m_DisableColor = Color.Gray;

        /// <summary>
        /// フラットマウスダウンカラー
        /// </summary>
        private Color m_FlatDownColor = Color.FromName("Control");



        /// <summary>
        /// false時のテキストの色
        /// </summary>
        private Color m_DisableForeColor = Color.Gray;

        /// <summary>
        /// 
        /// </summary>
        private Color m_OriginalForeColor = Color.DimGray;

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
        [Description("クリックイベント発生させるまでの長押し時間[ms]")]
        public int PushTime
        {
            get { return m_Time; }
            set
            {
                m_Time = value;
            }
        }

        [Category("カスタム")]
        [Description("クリック時のボタン背景色")]
        public Color LongPushBackColor
        {
            get { return m_PushColor; }
            set
            {
                m_PushColor = value;
            }
        }

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


        /// <summary>
        /// 長押し検知タイマ
        /// </summary>
        private System.Threading.Timer m_Timer;

        public LongPushButton()
        {
            InitializeComponent();
            m_FlatDownColor = this.FlatAppearance.MouseDownBackColor;
        }

        /// <summary>
        /// 文字色
        /// </summary>
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                //m_OriginalForeColor = value;
            }
        }

        /// <summary>
        /// 背景色
        /// </summary>
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
               // m_OrgBackColor = value;
            }
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
#if false
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
#endif
            m_OrgBackColor = base.BackColor;
            m_OriginalForeColor = base.ForeColor;
        }

#if false
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
#endif

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            if (!m_EnabledColorChangeMode) return;
            if (this.Enabled)
            {
                this.BackColor = m_OrgBackColor;
                this.ForeColor = m_OriginalForeColor;
            }
            else
            {
                this.BackColor = m_DisableColor;
                this.ForeColor = m_DisableForeColor;

            }
        }

        /// <summary>
        /// マウスダウンでタイマを作成する
        /// </summary>
        /// <param name="mevent"></param>
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            //m_OrgBackColor = this.BackColor;
            m_Timer = new System.Threading.Timer(_TimerCallback, null, m_Time, System.Threading.Timeout.Infinite);
            base.OnMouseDown(mevent);
        }
        /// <summary>
        /// マウスアップ
        /// タイマを破棄してイベントが発生しないようにする
        /// </summary>
        /// <param name="mevent"></param>
        protected override void  OnMouseUp(MouseEventArgs mevent)
        {
            try
            {
                m_Timer.Dispose();
                this.BackColor = m_OrgBackColor;
                this.FlatAppearance.MouseDownBackColor = m_FlatDownColor;
            }
            catch { }
 	    }

        /// <summary>
        /// タイマイベント
        /// 指定時間長押ししていると発生
        /// </summary>
        /// <param name="state"></param>
        public void _TimerCallback(object state)
        {
            try
            {
                this.Invoke(new delegate_void(_RaiseOnClick));
            }
            catch { }
        }

        /// <summary>
        /// マウスポインターがコントロールの表示領域から外れた時
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            //元の色に戻す
            if(!EnabledColorChangeMode)
                this.BackColor = m_OrgBackColor;
            this.FlatAppearance.MouseDownBackColor = m_OrgBackColor;
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// マウスポインターがコントロールの表示領域に入った時
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            this.FlatAppearance.MouseDownBackColor = m_FlatDownColor;
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// フォームのボタンクリックイベントをコール
        /// </summary>
        private void _RaiseOnClick()
        {
            this.BackColor = m_PushColor;
            this.FlatAppearance.MouseDownBackColor = m_PushColor;
            base.OnClick(new EventArgs());

        }

    }
}
