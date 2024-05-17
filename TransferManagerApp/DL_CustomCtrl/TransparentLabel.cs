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
    public partial class TransparentLabel : Label
    {

        public TransparentLabel()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

      
        private void DrawParentControl(Control c, System.Windows.Forms.PaintEventArgs pevent)
        {
            using (Bitmap bmp = new Bitmap(c.Width, c.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    using (PaintEventArgs p = new PaintEventArgs(g, c.ClientRectangle))
                    {
                        this.InvokePaintBackground(c, p);
                        this.InvokePaint(c, p);
                    }
                }

                int offsetX = this.Left + (int)Math.Floor((double)(this.Bounds.Width - this.ClientRectangle.Width) / 2.0);
                int offsetY = this.Top + (int)Math.Floor((double)(this.Bounds.Height - this.ClientRectangle.Height) / 2.0);
                pevent.Graphics.DrawImage(bmp, this.ClientRectangle, new Rectangle(offsetX, offsetY, this.ClientRectangle.Width, this.ClientRectangle.Height), GraphicsUnit.Pixel);
            }
        }

        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
        {
            this.DrawParentControl(this.Parent, pevent);
            try
            {
                // 親コントロールとの間のコントロールを親側から描画
                for (int i = this.Parent.Controls.Count - 1; i >= 0; i--)
                {
                    Control c = this.Parent.Controls[i];
                    if (c == this)
                    {
                        break;
                    }
                    if (this.Bounds.IntersectsWith(c.Bounds) == false)
                    {
                        continue;
                    }
                    this.DrawBackControl(c, pevent);
                }
            }
            catch { }
        }

        private void DrawBackControl(Control c, System.Windows.Forms.PaintEventArgs pevent)
        {
            try
            {
                using (Bitmap bmp = new Bitmap(c.Width, c.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    c.DrawToBitmap(bmp, new Rectangle(0, 0, c.Width, c.Height));
                    int offsetX = (c.Left - this.Left) - (int)Math.Floor((double)(this.Bounds.Width - this.ClientRectangle.Width) / 2.0);
                    int offsetY = (c.Top - this.Top) - (int)Math.Floor((double)(this.Bounds.Height - this.ClientRectangle.Height) / 2.0);
                    pevent.Graphics.DrawImage(bmp, offsetX, offsetY, c.Width, c.Height);
                }
            }
            catch { }
        }
    }
}
