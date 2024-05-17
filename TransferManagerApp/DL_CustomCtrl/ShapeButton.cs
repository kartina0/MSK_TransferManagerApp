using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel.DataAnnotations;

namespace DL_CustomCtrl
{
   
    public partial class ShapeButton : Button
    {
       
        /// <summary>
        /// 形状種別
        /// </summary>
        public enum ShapeType
        {
            Squaer = 0,
            RoundedSquaer,
            Arc,
            TriagleTop,
            TriagleBottom,
        }

        // 表示領域にマウスポインタ進入フラグ
        private bool _mouseEnterFlag = false;
        // クリックフラグ
        private bool _MouseDownFlag = false;

        // 角丸初期値
        private int _EllipseCorner = 50;

        // 形状
        private ShapeType _shape = ShapeType.Squaer;

        // ON/OFF状態
        private bool _onState = false;

        public ShapeButton()
        {
            InitializeComponent();

        }
        
        [Category("形状")]
        [Description("形状の変更")]
        public ShapeType Shape
        {
            get
            {
                return _shape;
            }
            set
            {
                bool diffShape = _shape != value;
                _shape = value;
                // 形状が変化したら再描画
                if (diffShape)
                    this.Refresh();
            }
        }
        [Category("形状")]
        [Description("角丸数値")]
        [DefaultValue(50)]
        public int RoundedCorner
        {
            get { return _EllipseCorner; }
            set
            {
                _EllipseCorner = value;
                this.Refresh();
            }
        }

        [Category("グラデーション")]
        [Description("CheckOFF時の色設定")]
        public GradationColor Gradation_OffState
        {
            get; set;
        } = new GradationColor();


        [Category("グラデーション")]
        [Description("CheckON時の色設定")]
        public GradationColor Gradation_OnState
        {
            get; set;
        } = new GradationColor();


        [Category("グラデーション")]
        [Description("MouseDown+CheckOFF時の色設定")]
        public GradationColor Gradation_MouseDown_OffState
        {
            get; set;
        } = new GradationColor();


        [Category("グラデーション")]
        [Description("MouseDown+CheckON時の色設定")]
        public GradationColor Gradation_MouseDown_OnState
        {
            get; set;
        } = new GradationColor();


        [Category("グラデーション")]
        [Description("MouseOver+CheckOFF時の色設定")]
        public GradationColor Gradation_MouseOver_OffState
        {
            get; set;
        } = new GradationColor();


        [Category("グラデーション")]
        [Description("MouseOver+CheckON時の色設定")]
        public GradationColor Gradation_MouseOver_OnState
        {
            get; set;
        } = new GradationColor();




        
        [Category("表示状態")]
        [Description("ON/OFF状態の変更")]
        public bool OnState
        {
            get { return _onState; }
            set
            {
                bool diffState = _onState != value;
                _onState = value;

                // 状態が変化したら再描画
                if (diffState)
                    this.Refresh();
            }
        }



        //楕円ボタン用
        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            GraphPath.AddArc(Rect.X, Rect.Y, radius, radius, 180, 90);
            GraphPath.AddLine(Rect.X + r2, Rect.Y, Rect.Width - r2, Rect.Y);
            GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y, radius, radius, 270, 90);
            GraphPath.AddLine(Rect.Width, Rect.Y + r2, Rect.Width, Rect.Height - r2);
            GraphPath.AddArc(Rect.X + Rect.Width - radius,
                             Rect.Y + Rect.Height - radius, radius, radius, 0, 90);
            GraphPath.AddLine(Rect.Width - r2, Rect.Height, Rect.X + r2, Rect.Height);
            GraphPath.AddArc(Rect.X, Rect.Y + Rect.Height - radius, radius, radius, 90, 90);
            GraphPath.AddLine(Rect.X, Rect.Height - r2, Rect.X, Rect.Y + r2);
            GraphPath.CloseFigure();
            return GraphPath;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            _mouseEnterFlag = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            _mouseEnterFlag = false;
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (_mouseEnterFlag)
                _MouseDownFlag = true;
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);

            _MouseDownFlag = false;
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {

            base.OnPaint(pe);

            GraphicsPath gp = new GraphicsPath();
            // 使用するグラデーション色を取得
            GradationColor gradationColor = GetGradationColor();

            //四角
            if (Shape == ShapeType.Squaer)
            {
                // 四角
                Point[] points = { new Point(0, 0), new Point(this.Width - 1, 0), new Point(this.Width - 1, this.Height - 1), new Point(0, this.Height - 1) };
                byte[] types = { (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line };

                //PathGradientBrushオブジェクトの作成
                PathGradientBrush b = new PathGradientBrush(points);

                //縁,周囲の色,中央
                Color[] colors = new Color[] { gradationColor.OutSide, gradationColor.Middle, gradationColor.Center };

                //外枠,背景,センターのポジション
                float[] relativePositions = {
                                                0.0f,     // the triangle.
                                                0.2f,     // the boundary
                                                1.0f};    // the center point.
                //グラデーションの定義
                ColorBlend colorBlend = new ColorBlend();
                colorBlend.Colors = colors;
                colorBlend.Positions = relativePositions;
                b.InterpolationColors = colorBlend;

                //描画
                pe.Graphics.FillRectangle(b, pe.Graphics.VisibleClipBounds);
                gp = new GraphicsPath(points, types);
                this.Region = new Region(gp);

                // フチを少し暗めの色で描画する
                Color color = gradationColor.OutSide; 
                float darkRatio = 0.5f;
                Color darkColor = Color.FromArgb(color.A, (int)(color.R * darkRatio), (int)(color.G * darkRatio), (int)(color.B * darkRatio));
                Pen pen = new Pen(darkColor, 4);
                Point[] linepoints = { new Point(0, 0), new Point(this.Width - 1, 0), new Point(this.Width - 1, this.Height - 1), new Point(0, this.Height - 1), new Point(0, 0) };
                pe.Graphics.DrawLines(pen, linepoints);
            }
            //円
            else if (Shape == ShapeType.Arc)
            {

                //パスを作成する
                GraphicsPath p = new GraphicsPath();
                //p.AddRectangle(pe.Graphics.VisibleClipBounds);
                p.AddArc(1, 1, this.Width - 2, this.Height - 2, 0, 360);
                
                //PathGradientBrushオブジェクトの作成
                PathGradientBrush b = gradationColor.ToPathGradientBrush(p);
                b.CenterColor = gradationColor.Center;
                b.SurroundColors = new Color[] { gradationColor.Middle };

                //四角を描く
                pe.Graphics.FillRectangle(b, pe.Graphics.VisibleClipBounds);

                //// 楕円形状
                gp.AddArc(1, 1, this.Width - 2, this.Height - 2, 0, 360);
                this.Region = new Region(gp);

                // フチを少し暗めの色で描画する
                Color color = gradationColor.OutSide;
                float darkRatio = 0.5f;
                Color darkColor = Color.FromArgb(color.A, (int)(color.R * darkRatio), (int)(color.G * darkRatio), (int)(color.B * darkRatio));

                Pen pen = new Pen(darkColor, 4);
                pe.Graphics.DrawArc(pen, 1, 1, this.Width - 2, this.Height - 2, 0, 360);
            }
            // 角丸四角
            else if (Shape == ShapeType.RoundedSquaer)
            {

                // 四角
                Point[] points = { new Point(0, 0), new Point(this.Width - 1, 0), new Point(this.Width - 1, this.Height - 1), new Point(0, this.Height - 1) };
                byte[] types = { (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line };

                //PathGradientBrushオブジェクトの作成
                PathGradientBrush b = new PathGradientBrush(points);
                //縁,周囲の色,中央
                Color[] colors = new Color[] { gradationColor.OutSide, gradationColor.Middle, gradationColor.Center };

                //外枠,背景,センターのポジション
                float[] relativePositions = {
                                                0.0f,     // the triangle.
                                                0.2f,     // the boundary
                                                1.0f};    // the center point.
                //グラデーションの定義
                ColorBlend colorBlend = new ColorBlend();
                colorBlend.Colors = colors;
                colorBlend.Positions = relativePositions;
                b.InterpolationColors = colorBlend;


                //描画
                pe.Graphics.FillRectangle(b, pe.Graphics.VisibleClipBounds);

                //角丸作成
                RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
                using (gp = GetRoundPath(Rect, RoundedCorner))
                {
                    this.Region = new Region(gp);

                    // フチを少し暗めの色で描画する
                    Color color = gradationColor.OutSide;
                    float darkRatio = 0.5f;
                    Color darkColor = Color.FromArgb(color.A, (int)(color.R * darkRatio), (int)(color.G * darkRatio), (int)(color.B * darkRatio));

                    using (Pen pen = new Pen(darkColor, 2))
                    {
                        pen.Alignment = PenAlignment.Inset;
                        pe.Graphics.DrawPath(pen, gp);
                    }
                }

            }
            //下三角
            else if (Shape == ShapeType.TriagleBottom)
            {

                // 下三角形
                Point[] points = { new Point(this.Width / 2 - 1, this.Height - 1), new Point(0, 0), new Point(this.Width - 1, 0) };
                byte[] types = { (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line };

                //PathGradientBrushオブジェクトの作成
                PathGradientBrush b = new PathGradientBrush(points);
                //縁,周囲の色,中央
                Color[] colors = new Color[] { gradationColor.OutSide, gradationColor.Middle, gradationColor.Center };

                //外枠,背景,センターのポジション
                float[] relativePositions = {
                                                0.0f,     // the triangle.
                                                0.2f,     // the boundary
                                                1.0f};    // the center point.
                //グラデーションの定義
                ColorBlend colorBlend = new ColorBlend();
                colorBlend.Colors = colors;
                colorBlend.Positions = relativePositions;
                b.InterpolationColors = colorBlend;

                //描画
                pe.Graphics.FillRectangle(b, pe.Graphics.VisibleClipBounds);

                gp = new GraphicsPath(points, types);
                this.Region = new Region(gp);

                // フチを少し暗めの色で描画する
                Color color = gradationColor.OutSide;

                float darkRatio = 0.5f;
                Color darkColor = Color.FromArgb(color.A, (int)(color.R * darkRatio), (int)(color.G * darkRatio), (int)(color.B * darkRatio));
                Pen pen = new Pen(darkColor, 4);
                Point[] linepoints = { new Point(this.Width / 2 - 1, this.Height - 1), new Point(0, 0), new Point(this.Width - 1, 0), new Point(this.Width / 2 - 1, this.Height - 1) };
                pe.Graphics.DrawLines(pen, linepoints);
            }
            //上三角
            else if (Shape == ShapeType.TriagleTop)
            {
                
                // 上三角形
                Point[] points = { new Point(this.Width / 2 - 1, 0), new Point(this.Width - 1, this.Height - 1), new Point(0, this.Height - 1) };
                byte[] types = { (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line };

                //PathGradientBrushオブジェクトの作成
                PathGradientBrush b = new PathGradientBrush(points);
                //縁,周囲の色,中央
                Color[] colors = new Color[] { gradationColor.OutSide, gradationColor.Middle, gradationColor.Center };


                //外枠,背景,センターのポジション
                float[] relativePositions = {
                                                0.0f,     // the triangle.
                                                0.2f,     // the boundary
                                                1.0f};    // the center point.
                //グラデーションの定義
                ColorBlend colorBlend = new ColorBlend();
                colorBlend.Colors = colors;
                colorBlend.Positions = relativePositions;
                b.InterpolationColors = colorBlend;

                //描画
                pe.Graphics.FillRectangle(b, pe.Graphics.VisibleClipBounds);

                gp = new GraphicsPath(points, types);
                this.Region = new Region(gp);

                // フチを少し暗めの色で描画する
                Color color = gradationColor.OutSide;

                float darkRatio = 0.5f;
                Color darkColor = Color.FromArgb(color.A, (int)(color.R * darkRatio), (int)(color.G * darkRatio), (int)(color.B * darkRatio));
                Pen pen = new Pen(darkColor, 4);
                Point[] linepoints = { new Point(this.Width / 2 - 1, 0), new Point(this.Width - 1, this.Height - 1), new Point(0, this.Height - 1), new Point(this.Width / 2 - 1, 0) };
                pe.Graphics.DrawLines(pen, linepoints);
            }

            //StringFormatを作成
            StringFormat sf = new StringFormat();
            sf.SetStringAlignment(this.TextAlign);

            //文字を書く
            Brush brush = new SolidBrush(gradationColor.ForeColor);
            pe.Graphics.DrawString(this.Text, this.Font, brush, this.ClientRectangle, sf);
        }

        /// <summary>
        /// 使用するグラデ―ション色を取得する
        /// </summary>
        /// <returns></returns>
        private GradationColor GetGradationColor()
        {
            GradationColor gradationColor = new GradationColor();
            if (OnState)
            {
                gradationColor = Gradation_OnState;

                if (_mouseEnterFlag)
                {
                    if (_MouseDownFlag)
                        gradationColor = Gradation_MouseDown_OnState;
                    else
                        gradationColor = Gradation_MouseOver_OnState;
                }

            }
            else
            {
                gradationColor = Gradation_OffState;

                if (_mouseEnterFlag)
                {
                    if (_MouseDownFlag)
                        gradationColor = Gradation_MouseDown_OffState;
                    else
                        gradationColor = Gradation_MouseOver_OffState;
                }
            }
            return gradationColor;
        }
    }

    /// <summary>
    /// 文字配置
    /// </summary>
    public static class ExtClass
    {
        public static ContentAlignment ToContentAlignment(this StringFormat Me)
        {
                return (ContentAlignment)((int)Math.Pow(2, (int)Me.Alignment) << (4 * (int)Me.LineAlignment));
        }

        public static void SetStringAlignment(this StringFormat Me, ContentAlignment ca)
        {
            try
            {
                int Valignment = (int)Math.Floor(Math.Log10((int)ca) / Math.Log10(16));
                int Halignment = (int)(Math.Log10((int)ca >> (4 * Valignment)) / Math.Log10(2));

                Me.LineAlignment = (StringAlignment)Valignment;
                Me.Alignment = (StringAlignment)Halignment;
                
            }
            catch { }
        }
    }



    /// <summary>
    /// グラデーション 色設定
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GradationColor
    {
        private Color _center = Color.White;
        private Color _middle = Color.Gray;
        private Color _outSide = Color.DarkGray;
        private Color _font = Color.Black;

        /// <summary>
        /// グラデーション 内側の色
        /// </summary>
        [Category("表示色")]
        [Description("グラデーション 内側の色")]
        public Color Center
        {
            get
            {
                return _center;
            }
            set
            {
                _center = value;
            }
        }
        /// <summary>
        /// グラデーション 内側の色
        /// </summary>
        [Category("表示色")]
        [Description("グラデーション 中間の色")]
        public Color Middle
        {
            get
            {
                return _middle;
            }
            set
            {
                _middle = value;
            }
        }
        /// <summary>
        /// グラデーション 外側の色
        /// </summary>
        /// 
        [Category("表示色")]
        [Description("グラデーション 外側の色")]
        public Color OutSide
        {
            get
            {
                return _outSide;
            }
            set
            {
                _outSide = value;
            }
        }
        /// <summary>
        /// フォント色
        /// </summary>
        /// 
        [Category("表示色")]
        [Description("フォント色")]
        public Color ForeColor
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public PathGradientBrush ToPathGradientBrush(GraphicsPath path)
        {
            PathGradientBrush brush= new PathGradientBrush(path);
            brush.CenterColor = _center;
            //パス内の点に対応している色を指定する
            brush.SurroundColors = new Color[] { _middle, _outSide };
            return brush;
        }
        public override string ToString()
        {
            return String.Format("{0},{1},{2},{3}", _center.ToString(), _middle.ToString(), _outSide.ToString(), _font.ToString());
        }
    }

}
