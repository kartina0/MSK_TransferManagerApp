using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_CommonLibrary
{
    [Serializable]
    public class Rect3D_Dbl
    {
        public double Width = 0;
        public double Height = 0;
        public double Depth = 0;

        public double Left = 0;
        public double Right = 0;

        public Rect3D_Dbl()
        {
            this.Width      = 0;
            this.Height     = 0;
            this.Depth      = 0;
            this.Left       = 0;
            this.Right      = 0;
        }

        public Rect3D_Dbl(double w, double h, double depth)
        {
            this.Width  = w;
            this.Height = h;
            this.Depth  = depth;
            this.Left   = 0;
            this.Right  = 0;
        }
        public Rect3D_Dbl(double w, double h, double depth, double left, double right)
        {
            this.Width  = w;
            this.Height = h;
            this.Depth = depth;
            this.Left   = left;
            this.Right  = right;
        }


        public Rect3D_Dbl(string pos)
        {
            pos = pos.Replace("(", "");
            pos = pos.Replace(")", "");

            string[] col = pos.Split(',');
            if (col.Length >= 1) Width  = double.Parse(col[0]);
            if (col.Length >= 2) Height = double.Parse(col[1]);
            if (col.Length >= 3) Depth  = double.Parse(col[2]);
            if (col.Length >= 4) Left   = double.Parse(col[3]);
            if (col.Length >= 5) Right  = double.Parse(col[4]);

        }


        public static Rect3D_Dbl operator +(Rect3D_Dbl a, Rect3D_Dbl b)
        {
            return new Rect3D_Dbl(a.Width + b.Width, a.Height + b.Height, a.Depth + b.Depth, a.Left + b.Left, a.Right + b.Right);
        }

        public static Rect3D_Dbl operator -(Rect3D_Dbl a, Rect3D_Dbl b)
        {
            return new Rect3D_Dbl(a.Width - b.Width, a.Height - b.Height, a.Depth - b.Depth, a.Left - b.Left, a.Right - b.Right);
        }

        public override string ToString()
        {
            return "(" + Width + "," + Height + "," + Depth + "," + Left + "," + Right + ")";
        }


        /// <summary>
        /// コピーを返す
        /// </summary>
        /// <returns></returns>
        public Rect3D_Dbl GetCopy()
        {
            return new Rect3D_Dbl(Width, Height, Depth, Left, Right);
        }
    }
}
