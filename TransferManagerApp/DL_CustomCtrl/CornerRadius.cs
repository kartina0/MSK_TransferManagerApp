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
using System.Drawing.Drawing2D;

namespace DL_CustomCtrl
{
    
    public partial class CornerRadius : Label
    {
        public enum STATE
        {
            OFF=0,
            ON,
            BUSY,
            ERROR,
        }
        #region "Private Variable"

        private bool m_DrawBorderLine;

        /// <summary>
        /// 背景色
        /// </summary>
        private Color m_BackColor;

        /// <summary>
        /// 枠色
        /// </summary>
        private Color m_BorderColor;

        /// <summary>
        /// Active ON 時の背景色
        /// </summary>
        private Color m_OnColor = Color.Blue;
        /// <summary>
        /// Active OFF 時の背景色
        /// </summary>
        private Color m_OffColor = Color.DimGray;
        /// <summary>
        /// Busy On 時の背景色
        /// </summary>
        private Color m_BusyColor = Color.Yellow;
        /// <summary>
        /// Error On 時の背景色
        /// </summary>
        private Color m_ErrorColor = Color.Red;

        /// <summary>
        /// Active ON 時の文字色
        /// </summary>
        private Color m_OnForeColor = Color.White;
        /// <summary>
        /// Active OFF 時の文字色
        /// </summary>
        private Color m_OffForeColor = Color.DimGray;
        /// <summary>
        /// Busy On 時の文字色
        /// </summary>
        private Color m_BusyForeColor = Color.Black;
        /// <summary>
        /// Error On 時の文字色
        /// </summary>
        private Color m_ErrorForeColor = Color.Black;

        /// <summary>
        /// ON時のテキスト
        /// </summary>
        private string m_OnText = "";

        /// <summary>
        /// OFF時のテキスト
        /// </summary>
        private string m_OffText = "";

        /// <summary>
        /// Busy時のテキスト
        /// </summary>
        private string m_BusyText = "";

        /// <summary>
        /// Error時のテキスト
        /// </summary>
        private string m_ErrorText = "";

        #endregion
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CornerRadius()
        {
            //InitializeComponent();
            base.BackColor = Color.Transparent;
            base.BorderStyle = System.Windows.Forms.BorderStyle.None;
            base.AutoSize = false;
            base.TextAlign = ContentAlignment.MiddleCenter;
         
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pevent"></param>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            try
            {
                base.OnPaintBackground(pevent);

                Rectangle ar = this.ClientRectangle;
                ar.Width--;
                ar.Height--;
                Rectangle lr = this.ClientRectangle;
                bool canArc = ((ar.Width > 0) && (ar.Height > 0));

                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.StartFigure();
                    if ((this.RadiusTopRight > 0) && (canArc == true))
                    {
                        int w = this.RadiusTopRight > ar.Width ? ar.Width : this.RadiusTopRight;
                        int h = this.RadiusTopRight > ar.Height ? ar.Height : this.RadiusTopRight;
                        gp.AddArc(ar.Right - w, ar.Top, w, h, 270, 90);
                    }
                    else
                    {
                        gp.AddLine(lr.Right, lr.Top, lr.Right, lr.Top);
                    }
                    if ((this.RadiusBottomRight > 0) && (canArc == true))
                    {
                        int w = this.RadiusBottomRight > ar.Width ? ar.Width : this.RadiusBottomRight;
                        int h = this.RadiusBottomRight > ar.Height ? ar.Height : this.RadiusBottomRight;
                        gp.AddArc(ar.Right - w, ar.Bottom - h, w, h, 0, 90);
                    }
                    else
                    {
                        gp.AddLine(lr.Right, lr.Bottom, lr.Right, lr.Bottom);
                    }
                    if ((this.RadiusBottomLeft > 0) && (canArc == true))
                    {
                        int w = this.RadiusBottomLeft > ar.Width ? ar.Width : this.RadiusBottomLeft;
                        int h = this.RadiusBottomLeft > ar.Height ? ar.Height : this.RadiusBottomLeft;
                        gp.AddArc(ar.Left, ar.Bottom - h, w, h, 90, 90);
                    }
                    else
                    {
                        gp.AddLine(lr.Left, lr.Bottom, lr.Left, lr.Bottom);
                    }
                    if ((RadiusTopLeft > 0) && (canArc == true))
                    {
                        int w = this.RadiusTopLeft > ar.Width ? ar.Width : this.RadiusTopLeft;
                        int h = this.RadiusTopLeft > ar.Height ? ar.Height : this.RadiusTopLeft;
                        gp.AddArc(ar.Left, ar.Top, w, h, 180, 90);
                    }
                    else
                    {
                        gp.AddLine(lr.Left, lr.Top, lr.Left, lr.Top);
                    }
                    gp.CloseFigure();

                    if (canArc == true)
                    {
                        pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    }

                    using (SolidBrush sb = new SolidBrush(this.BackColor))
                    {
                        if (this.m_DrawBorderLine)
                            pevent.Graphics.DrawPath(new Pen(Color.Black), gp);

                        pevent.Graphics.FillPath(sb, gp);
                    }
                    if (canArc == true)
                    {
                        pevent.Graphics.SmoothingMode = SmoothingMode.Default;
                    }
                }
            }
            catch { }
        }

        #region BackColor


        /// <summary>
        /// 
        /// </summary>
        public new Color BackColor
        {
            get
            {
                if (this.m_BackColor != Color.Empty)
                {
                    return this.m_BackColor;
                }

                if (this.Parent != null)
                {
                    return this.Parent.BackColor;
                }

                return Control.DefaultBackColor;
            }
            set
            {
                this.m_BackColor = value;
                this.Invalidate();
            }
        }



        [Category("表示")]
        [Description("枠 描画有無")]
        public bool DrawBorderLine
        {
            get { return m_DrawBorderLine; }
            set { m_DrawBorderLine = value; }
        }

        [Category("表示")]
        [Description("枠色")]
        public Color BorderColor
        {
            get
            {
                if (this.m_BorderColor != Color.Empty)
                {
                    return this.m_BorderColor;
                }

                if (this.Parent != null)
                {
                    return Color.Black;
                }

                return Color.Black;
            }
            set
            {
                this.m_BorderColor = value;
                this.Invalidate();
            }
        }




        [Category("表示")]
        [Description("ActiveCtrl = OFF時の背景色")]
        public Color BackColor_OFF
        {
            get { return m_OffColor; }
            set { m_OffColor = value; }
        }
        [Category("表示")]
        [Description("ActiveCtrl = ON時の背景色")]
        public Color BackColor_ON
        {
            get { return m_OnColor; }
            set { m_OnColor = value; }
        }
        [Category("表示")]
        [Description("ActiveCtrl = BUSY時の背景色")]
        public Color BackColor_Busy
        {
            get { return m_BusyColor; }
            set { m_BusyColor = value; }
        }
        [Category("表示")]
        [Description("ActiveCtrl = ERROR時の背景色")]
        public Color BackColor_Error
        {
            get { return m_ErrorColor; }
            set { m_ErrorColor = value; }
        }

        [Category("表示")]
        [Description("ActiveCtrl = OFF時の文字色")]
        public Color ForeColor_OFF
        {
            get { return m_OffForeColor; }
            set { m_OffForeColor = value; }
        }
        [Category("表示")]
        [Description("ActiveCtrl = ON時の文字色")]
        public Color ForeColor_ON
        {
            get { return m_OnForeColor; }
            set { m_OnForeColor = value; }
        }
        [Category("表示")]
        [Description("ActiveCtrl = BUSY時の文字色")]
        public Color ForeColor_Busy
        {
            get { return m_BusyForeColor; }
            set { m_BusyForeColor = value; }
        }

        [Category("表示")]
        [Description("ActiveCtrl = ERROR時の文字色")]
        public Color ForeColor_Error
        {
            get { return m_ErrorForeColor; }
            set { m_ErrorForeColor = value; }
        }

        [Category("表示")]
        [Description("ActiveCtrl = OFF時の文字")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Text_OFF
        {
            get { return m_OffText; }
            set { m_OffText = value; }
        }
        [Category("表示")]
        [Description("ActiveCtrl = ON時の文字")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Text_ON
        {
            get { return m_OnText; }
            set { m_OnText = value; }
        }
        [Category("表示")]
        [Description("ActiveCtrl = BUSY時の文字")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Text_Busy
        {
            get { return m_BusyText; }
            set { m_BusyText = value; }
        }
        [Category("表示")]
        [Description("ActiveCtrl = ERROR時の文字")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Text_Error
        {
            get { return m_ErrorText; }
            set { m_ErrorText = value; }
        }

        public override void ResetBackColor()
        {
            this.BackColor = Color.Empty;
        }

        private Boolean ShouldSerializeBackColor()
        {
            return this.m_BackColor != Color.Empty;
        }

        #endregion



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
                if (m_ActiveCtrl== STATE.ON)
                {
                    this.BackColor = m_OnColor;
                    this.ForeColor = m_OnForeColor;
                    this.Text = m_OnText;
                }
                else if (m_ActiveCtrl == STATE.BUSY)
                {
                    this.BackColor = m_BusyColor;
                    this.ForeColor = m_BusyForeColor;
                    this.Text = m_BusyText;
                }
                else if (m_ActiveCtrl == STATE.ERROR)
                {
                    this.BackColor = m_ErrorColor;
                    this.ForeColor = m_ErrorForeColor;
                    this.Text = m_ErrorText;
                }
                else
                {
                    this.BackColor = m_OffColor;
                    this.ForeColor = m_OffForeColor;
                    this.Text = m_OffText;
                }
            }
        }

        private int _RadiusTopLeft;
        [Category("表示")]
        [DefaultValue(0)]
        [Description("コントロールの左上の角の半径を取得または設定します。")]
        public int RadiusTopLeft
        {
            get { return this._RadiusTopLeft; }
            set
            {
                this._RadiusTopLeft = value;
                this.Invalidate();
            }
        }

        private int _RadiusTopRight;
        [Category("表示")]
        [DefaultValue(0)]
        [Description("コントロールの右上の角の半径を取得または設定します。")]
        public int RadiusTopRight
        {
            get { return this._RadiusTopRight; }
            set
            {
                this._RadiusTopRight = value;
                this.Invalidate();
            }
        }

        private int _RadiusBottomLeft;
        [Category("表示")]
        [DefaultValue(0)]
        [Description("コントロールの左下の角の半径を取得または設定します。")]
        public int RadiusBottomLeft
        {
            get { return this._RadiusBottomLeft; }
            set
            {
                this._RadiusBottomLeft = value;
                this.Invalidate();
            }
        }

        private int _RadiusBottomRight;
        [Category("表示")]
        [DefaultValue(0)]
        [Description("コントロールの右下の角の半径を取得または設定します。")]
        public int RadiusBottomRight
        {
            get { return this._RadiusBottomRight; }
            set
            {
                this._RadiusBottomRight = value;
                this.Invalidate();
            }
        }
    }

}
