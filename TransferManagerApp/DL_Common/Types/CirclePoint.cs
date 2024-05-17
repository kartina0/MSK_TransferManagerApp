using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_CommonLibrary
{
    public class CirclePoint
    {
        /// <summary>
        /// 中心X座標
        /// </summary>
        public double X = 0;
        /// <summary>
        /// 中心X座標
        /// </summary>
        public double Y = 0;

        /// <summary>
        /// 半径
        /// </summary>
        public double radius = 0;
        /// <summary>
        /// Left位置取得
        /// </summary>
        public double Left
        {
            get { return X - radius; ; }
        }
        /// <summary>
        /// Top位置取得
        /// </summary>
        public double Top
        {
            get { return Y - radius; ; }
        }

        /// <summary>
        /// 横幅取得
        /// </summary>
        public double Width
        {
            get { return radius * 2; }
        }
        /// <summary>
        /// 縦幅取得
        /// </summary>
        public double Height
        {
            get { return radius * 2; }
        }


        /// <summary>
        /// 中心
        /// </summary>
        public PointDbl Center
        {
            get
            {
                return new PointDbl(X, Y);
            }
        }

        public CirclePoint()
        {
            this.X = 0;
            this.Y = 0;
            this.radius = 0;
        }

        public CirclePoint(double x, double y, double radius)
        {
            this.X = x;
            this.Y = y;
            this.radius = radius;
        }

        public CirclePoint(string pos)
        {
            pos = pos.Replace("(", "");
            pos = pos.Replace(")", "");

            string[] col = pos.Split(',');
            if (col.Length >= 1) X = double.Parse(col[0]);
            if (col.Length >= 2) Y = double.Parse(col[1]);
            if (col.Length >= 3) radius = double.Parse(col[1]);

        }



        public override string ToString()
        {
            return "(" + X + "," + Y + "," + radius + ")";
        }


        /// <summary>
        /// コピーを返す
        /// </summary>
        /// <returns></returns>
        public CirclePoint GetCopy()
        {
            return new CirclePoint(X, Y, radius);
        }
    }
}
