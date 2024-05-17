using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_CommonLibrary
{
    [Serializable]
    public class Rect2D
    {
        public double X = 0;
        public double Y = 0;
        public double Width = 0;
        public double Height = 0;

        public Rect2D()
        {
            this.X          = 0;
            this.Y          = 0;
            this.Width      = 0;
            this.Height     = 0;
        }

        public Rect2D(double w, double h)
        {
            this.X = 0;
            this.Y = 0;
            this.Width = w;
            this.Height = h;
        }
        public Rect2D(double x, double y, double w, double h)
        {
            this.X      = x;
            this.Y      = y;
            this.Width  = w;
            this.Height = h;
        }


        public Rect2D(string pos)
        {
            pos = pos.Replace("(", "");
            pos = pos.Replace(")", "");

            string[] col = pos.Split(',');
            if (col.Length >= 1) X      = double.Parse(col[0]);
            if (col.Length >= 2) Y      = double.Parse(col[1]);
            if (col.Length >= 3) Width  = double.Parse(col[2]);
            if (col.Length >= 4) Height = double.Parse(col[3]);
        }


        public static Rect2D operator +(Rect2D a, Rect2D b)
        {
            return new Rect2D(a.X + b.X, a.Y + b.Y, a.Width + b.Width, a.Height + b.Height);
        }

        public static Rect2D operator -(Rect2D a, Rect2D b)
        {
            return new Rect2D(a.X - b.X, a.Y - b.Y, a.Width - b.Width, a.Height - b.Height);
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + "," + Width + "," + Height + ")";
        }


        public double Left
        {
            get { return X; }
            set { X = value; }

        }
        public double Right
        {
            get { return X + Width; }
            set
            {
                Width = value - X;
            }
        }
        public double Top
        {
            get { return Y; }
            set { Y = value; }

        }
        public double Bottom
        {
            get { return Y + Height; }
            set
            {
                Height = value - Y;
            }
        }

        /// <summary>
        /// 中心
        /// </summary>
        public PointDbl Center
        {
            get { return new PointDbl(X + Width / 2, Y + Height / 2); }
            set
            {
                X = value.X - Width / 2;
                Y = value.Y - Height / 2;
            }

        }

        /// <summary>
        /// コピーを返す
        /// </summary>
        /// <returns></returns>
        public Rect2D GetCopy()
        {
            return new Rect2D(X, Y, Width, Height);
        }


        /// <summary>
        /// 始点と終点をPointDblに変換
        /// </summary>
        /// <returns></returns>
        public PointDbl[] ToPointDbl()
        {
            PointDbl[] points = new PointDbl[2];
            points[0] = new PointDbl(X, Y);
            points[1] = new PointDbl(X + Width, Y + Height);
            return points;
        }
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public void Set(Rect2D rect)
        {
            if (rect.Width < 0)
            {
                this.X = rect.Right;
                this.Width = rect.Width * -1;
            }
            else
            {
                this.X = rect.X;
                this.Width = rect.Width;
            }

            if (rect.Height < 0)
            {
                this.Y = rect.Bottom;
                this.Height = rect.Height * -1;
            }
            else
            {
                this.Y = rect.Y;
                this.Height = rect.Height;
            }
        }

    }
}
