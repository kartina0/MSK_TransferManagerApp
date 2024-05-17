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
    public delegate void delegate_StateChanged(object sender, int stateNum, object Tag);
    public partial class MultiStateButton : Button
    {
        /// <summary>
        /// ステート変更後イベント
        /// </summary>
        public event delegate_StateChanged StateChanged;

        /// <summary>
        /// 
        /// </summary>
        private MultiState[] m_State = new MultiState[0];

        /// <summary>
        /// 現在のステータス番号
        /// </summary>
        private int m_CurrentStateIndex = 0;

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


        /// <summary>
        /// 現在のステータス番号
        /// </summary>
        public int CurrentStateIndex
        {
            get { return m_CurrentStateIndex; }
            set { m_CurrentStateIndex = value; }
        }
        /// <summary>
        /// 現在のステータス番号
        /// </summary>
        public MultiState CurrentState
        {
            get 
            {
                MultiState obj = null;
                try
                {
                    obj = m_State[m_CurrentStateIndex];
                }
                catch { }
                return obj;
            }
            set 
            {
                try
                {
                    m_State[m_CurrentStateIndex] = value;
                }
                catch { }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MultiState[] State
        {
            get { return m_State; }
            set { m_State = value; }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MultiStateButton()
        {
            InitializeComponent();
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (this.Visible)
            {

                if (m_CurrentStateIndex >= m_State.Length)
                    m_CurrentStateIndex = 0;
                if (StateChanged != null)
                    StateChanged(this, m_CurrentStateIndex, m_State[m_CurrentStateIndex]);
                ChangeDisplay();

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pe"></param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        /// <summary>
        /// クリックイベント
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            try
            {
                m_CurrentStateIndex++;

                if (m_CurrentStateIndex >= m_State.Length)
                    m_CurrentStateIndex = 0;
                if (StateChanged != null)
                    StateChanged(this, m_CurrentStateIndex, m_State[m_CurrentStateIndex]);
                ChangeDisplay();
            }
            catch { }
        }

        /// <summary>
        /// 表示切替
        /// </summary>
        private void ChangeDisplay()
        {
            try
            {
                MultiState state = m_State[m_CurrentStateIndex];
                this.Text = state.Text;
                this.BackColor = state.BackColor;
                this.ForeColor = state.ForeColor;
                this.Tag = (object)state.Tag;
            }
            catch { }
        }

    }

    public class MultiState
    {
        private Color m_BackColor = Color.LightGray;
        private Color m_ForeColor = Color.Black;
        private string m_Text = "";
        private string _m_Tag;

        public Color BackColor
        {
            get { return m_BackColor; }
            set { m_BackColor = value; }
        }
        public Color ForeColor
        {
            get { return m_ForeColor; }
            set { m_ForeColor = value; }
        }
        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }
        public string Tag
        {
            get { return _m_Tag; }
            set { _m_Tag = value; }
        }
    }
}
