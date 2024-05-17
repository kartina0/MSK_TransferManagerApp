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
    public partial class UpDownText : TextBox
    {

        /// <summary>
        /// 上下キーでのステップ量
        /// </summary>
        private double m_UpDnStep = 0;

        /// <summary>
        /// 最大値
        /// </summary>
        private double m_MaxValue = 0;

        /// <summary>
        /// 最小値
        /// </summary>
        private double m_MinValue = 0;

        /// <summary>
        /// 
        /// </summary>
        private bool m_EnableUpDown = false;
        /// <summary>
        /// 
        /// </summary>
        private bool m_UpDownLoopMode = false;

        /// <summary>
        /// 入力無し時の表示文字
        /// </summary>
        private string m_NullString = "";
        /// <summary>
        /// 
        /// </summary>
        private string m_FormatString = "{0}";

        /// <summary>
        /// 通常背景色
        /// </summary>
        private Color m_NormalForeColor = Color.Black;

        /// <summary>
        /// エラー背景色
        /// </summary>
        private Color m_ErrorForeColor = Color.Red;

        /// <summary>
        /// ハイライト使用有無
        /// </summary>
        private bool m_EnableHilight = false;

        private Color m_OrgBackColor = Color.White;


        /// <summary>
        /// 初回確認
        /// </summary>
        private bool _firstLoad = false;
        /// <summary>
        /// @@20190703
        /// 整数のみ入力
        /// </summary>
        private bool _IntergerOnly = false;

        /// <summary>
        /// タイマ
        /// </summary>
        private System.Threading.Timer _autoLimitCheckTimer = null;

        /// <summary>
        /// 一定周期で値のリミット表示更新を行う
        /// </summary>
        private int _autoLimitCheckTime = 0;

        /// <summary>
        /// タイマ破棄
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!DesignMode && _autoLimitCheckTimer != null)
                    _autoLimitCheckTimer.Dispose();
                _autoLimitCheckTimer = null;
            }
            catch { }
            base.Dispose(disposing);
        }
        /// <summary>
        /// タイマ破棄
        /// </summary>
        protected override void DestroyHandle()
        {
            try
            {
                if (!DesignMode && _autoLimitCheckTimer != null)
                    _autoLimitCheckTimer.Dispose();
                _autoLimitCheckTimer = null;
            }
            catch { }
            base.DestroyHandle();
        }

        /// <summary>
        /// ハンドル作成
        /// </summary>
        protected override void CreateHandle()
        {
            base.CreateHandle();
            if (!DesignMode && _autoLimitCheckTime > 0)
            {
                _autoLimitCheckTimer = new System.Threading.Timer(_TimerCallback, null, 100, _autoLimitCheckTime);
            }
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


        /// <summary>
        /// 
        /// </summary>
        [Category("数値")]
        [Description("異常時の文字色")]
        public Color ErrorForeColor
        {
            get { return m_ErrorForeColor; }
            set
            {
                m_ErrorForeColor = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Category("数値")]
        [Description("ステップ量")]
        public double UpDonwStep
        {
            get { return m_UpDnStep; }
            set
            {
                m_UpDnStep = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Category("数値")]
        [Description("通常時の文字色")]
        public Color NormalForeColor
        {
            get { return m_NormalForeColor; }
            set
            {
                m_NormalForeColor = value;
            }
        }
        [Category("数値")]
        [Description("上限")]
        public double MaxValue
        {
            get { return m_MaxValue; }
            set
            {
                m_MaxValue = value;
            }
        }

        [Category("数値")]
        [Description("下限値")]
        public double MinValue
        {
            get { return m_MinValue; }
            set
            {
                m_MinValue = value;
            }
        }

        [Category("数値")]
        [Description("上下キーによる数値変更")]
        public bool EnableUpDown
        {
            get { return m_EnableUpDown; }
            set
            {
                m_EnableUpDown = value;
            }
        }

        [Category("数値")]
        [Description("最大値の次に最小値を表示")]
        public bool UpDownLoopMode
        {
            get { return m_UpDownLoopMode; }
            set
            {
                m_UpDownLoopMode = value;
            }
        }

        [Category("数値")]
        [Description("入力無し時の表示文字")]
        public string NullString
        {
            get { return m_NullString; }
            set
            {
                m_NullString = value;
            }
        }

        [Category("数値")]
        [Description("書式")]
        public string FormatString
        {
            get { return m_FormatString; }
            set
            {
                m_FormatString = value;
            }
        }

        [Category("表示")]
        [Description("ハイライト")]
        public bool EnableHiLight
        {
            get { return m_EnableHilight; }
            set { m_EnableHilight = value; }
        }

        /// <summary>
        /// @@20190703
        /// </summary>
        [Category("数値")]
        [Description("整数のみ入力")]
        public bool IntegerOnly
        {
            get { return _IntergerOnly; }
            set { _IntergerOnly = value; }
        }

        [Category("数値")]
        [Description("リミット表示更新間隔")]
        public int AutoLimitCheckTime
        {
            get { return _autoLimitCheckTime; }
            set { _autoLimitCheckTime = value; }
        }


        /// <summary>
        /// 入力値がリミットをオーバーしているか確認
        /// </summary>
        /// <returns></returns>
        public bool IsLimitOver()
        {
            bool ret = false;
            try
            {
                double v = double.Parse(base.Text);
                if (v > m_MaxValue || v < m_MinValue)
                    ret = true;
            }
            catch { ret = true; }
            return ret;
        }

        /// <summary>
        /// 数値取得
        /// </summary>
        public double Value
        {
            get
            {
                double v = 0;
                try
                {
                    double.TryParse(this.Text, out v);
                }
                catch { }
                return v;
            }
            set
            {
                this.Text = string.Format(this.FormatString, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            // @@20190703
            if (_IntergerOnly && (e.KeyChar == '.'))
            {
                e.Handled = true;
                return;
            }

            base.OnKeyPress(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // @@20190703
            if (_IntergerOnly && (e.KeyData == Keys.OemPeriod || e.KeyData == Keys.Decimal))
            {
                e.Handled = true;
                return;
            }

            if (m_EnableUpDown)
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {

                    double v = 0;
                    bool ok = double.TryParse(base.Text, out v);
                    if (!ok) return;

                    if (e.KeyCode == Keys.Up) v += m_UpDnStep;
                    if (e.KeyCode == Keys.Down) v -= m_UpDnStep;

                    if (m_UpDownLoopMode)
                    {   // くるくるモード
                        if (v < m_MinValue) v = m_MaxValue;
                        if (v > m_MaxValue) v = m_MinValue;
                    }
                    else
                    {   // 通常(最大最小値までで停止)
                        if (v < m_MinValue) v = m_MinValue;
                        if (v > m_MaxValue) v = m_MaxValue;
                    }

                    base.Text = string.Format(this.FormatString, v);
                }
                else
                {
                    base.OnKeyDown(e);
                }
            }
            else
            {
                base.OnKeyDown(e);
            }



        }
        /// <summary>
        /// テキスト変化
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(EventArgs e)
        {
            try
            {
                // @@20190703
                if (_IntergerOnly)
                {
                    string s = this.Text;
                    if(s.IndexOf('.')>0)
                    {
                        int v = (int)this.Value;
                        this.Text = v.ToString();
                    }
                }


                if (IsLimitOver())
                    this.ForeColor = m_ErrorForeColor;
                else
                    this.ForeColor = m_NormalForeColor;
            }
            catch { }
            base.OnTextChanged(e);
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
        /// <summary>
        /// フォーカスON
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnter(EventArgs e)
        {
            if (m_EnableHilight)
            {
                m_OrgBackColor = this.BackColor;
                this.BackColor = Color.Yellow;
            }

            base.OnEnter(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            //if (base.Text == "") base.Text = m_NullString;
            base.OnVisibleChanged(e);
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
        }

        protected override void OnValidating(CancelEventArgs e)
        {
            base.OnValidating(e);
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
        }
        protected override void OnLeave(EventArgs e)
        {
            if (IsLimitOver())
                this.ForeColor = m_ErrorForeColor;
            else
                this.ForeColor = m_NormalForeColor;

            if (m_EnableHilight) this.BackColor = m_OrgBackColor;

            base.OnLeave(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }
        protected override void OnTextAlignChanged(EventArgs e)
        {
            base.OnTextAlignChanged(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (IsLimitOver())
                this.ForeColor = m_ErrorForeColor;
            else
                this.ForeColor = m_NormalForeColor;

            base.OnPaint(e);
        }
        protected override void OnValidated(EventArgs e)
        {
            if (IsLimitOver())
                this.ForeColor = m_ErrorForeColor;
            else
                this.ForeColor = m_NormalForeColor;

            base.OnValidated(e);
        }

        /// <summary>
        /// タイマイベント
        /// 定期的にリミット値を確認し背景色を変更する
        /// </summary>
        /// <param name="state"></param>
        public void _TimerCallback(object state)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    if (IsLimitOver())
                        this.ForeColor = m_ErrorForeColor;
                    else
                        this.ForeColor = m_NormalForeColor;

                    if (m_EnableHilight) this.BackColor = m_OrgBackColor;
                });
            }
            catch { }
        }

    }
}
