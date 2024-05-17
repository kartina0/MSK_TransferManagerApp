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
    public partial class HorizontalIndicator : PictureBox
    {
        /// <summary>
        /// 背景色へのグラデーション量
        /// </summary>
        private const int OffPeixel = 10;
        /// <summary>
        /// 
        /// </summary>
        public enum Direction
        {
            TopToButtom = 0,
            ButtomToTop,
        }
        /// <summary>
        /// 方向
        /// </summary>
        private Direction m_Dir = Direction.ButtomToTop;

        /// <summary>
        /// min値の色
        /// </summary>
        private Color m_MinColor = Color.White;

        /// <summary>
        /// max値の色
        /// </summary>
        private Color m_MaxColor = Color.Yellow;
        
        /// <summary>
        /// 現在値
        /// </summary>
        private double m_Val = 0;
        /// <summary>
        /// 最大値
        /// </summary>
        private double m_Max = 0;
        /// <summary>
        /// 最小値
        /// </summary>
        private double m_Min = 0;

        [Category("カスタム")]
        [Description("最大値の色")]
        public Color MaxColor
        {
            get { return m_MaxColor; }
            set
            {
                m_MaxColor = value;
            }
        }

        [Category("カスタム")]
        [Description("最小値の色")]
        public Color MinColor
        {
            get { return m_MinColor; }
            set
            {
                m_MinColor = value;
            }
        }


        [Category("カスタム")]
        [Description("方向")]
        public Direction Dir   
        {
            get { return m_Dir; }
            set
            {
                m_Dir = value;
            }
        }
        [Category("カスタム")]
        [Description("値")]
        public double Value
        {
            get { return m_Val; }
            set
            {
                m_Val = value;
            }
        }
        [Category("カスタム")]
        [Description("最小値")]
        public double Min
        {
            get { return m_Min; }
            set
            {
                m_Min = value;
            }
        }
        [Category("カスタム")]
        [Description("最大値")]
        public double Max
        {
            get { return m_Max; }
            set
            {
                m_Max = value;
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


        //public HorizontalIndicator()
        //{
        //    InitializeComponent();
        //}

        protected override void OnPaint(PaintEventArgs pe)
        {
            try
            {
                double height = this.Height;
                double width = this.Width;
                double r = width / (m_Max - m_Min);
                double vp = m_Val * r;
                Rectangle rect = new Rectangle(0, 0, (int)width, (int)vp);
                Rectangle rect2 = new Rectangle(0, (int)vp, (int)width, (int)30);
                LinearGradientBrush gb = null;
                LinearGradientBrush gb2 = null;
                if (m_Dir == Direction.TopToButtom)
                {
                    rect = new Rectangle(0, 0, (int)vp, (int)height);
                    try
                    {
                        if (rect.Height > 0)
                        {
                            gb = new LinearGradientBrush(
                                            rect,
                                            m_MinColor,
                                            m_MaxColor,
                                            LinearGradientMode.Vertical);
                        }
                    }
                    catch { }
                }
                else if (m_Dir == Direction.ButtomToTop)
                {
                    // Widthは小数部切り捨て分考慮する
                    int w = (int)(width - (int)(width - vp));
                    int xpos = (int)(width - vp);

                    rect = new Rectangle((int)width, 0, (int)w, (int)height);

                    try
                    {
                        if (rect.Height > 0)
                        {
                            gb = new LinearGradientBrush(
                                            rect,
                                            m_MaxColor,
                                            m_MinColor,
                                            LinearGradientMode.Vertical);
                        }
  
                    }
                    catch { }

                }

                pe.Graphics.Clear(Color.Black);

                //四角を描く
                if (gb != null)
                {
                    gb.GammaCorrection = true;
                    gb.WrapMode = WrapMode.Tile;
                
                    pe.Graphics.FillRectangle(gb, rect);
                    pe.Graphics.FillRectangle(gb2, rect2);             


                }
            }
            catch { }
            base.OnPaint(pe);
            
        }



    }
}
