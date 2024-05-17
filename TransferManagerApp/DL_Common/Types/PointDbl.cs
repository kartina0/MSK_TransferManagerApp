using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_CommonLibrary
{
    [Serializable]
    public class PointDbl
    {

        public double X = 0;
        public double Y = 0;

        public PointDbl()
        {
            this.X = 0;
            this.Y = 0;
        }

        public PointDbl(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public PointDbl(string pos)
        {
            pos = pos.Replace("(", "");
            pos = pos.Replace(")", "");

            string[] col = pos.Split(',');
            if (col.Length >= 1) X = double.Parse(col[0]);
            if (col.Length >= 2) Y = double.Parse(col[1]);
        }


        public static PointDbl operator +(PointDbl a, PointDbl b)
        {
            return new PointDbl(a.X + b.X, a.Y + b.Y);
        }

        public static PointDbl operator -(PointDbl a, PointDbl b)
        {
            return new PointDbl(a.X - b.X, a.Y - b.Y);
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + ")";
        }

        /// <summary>
        /// 距離を取得
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public double GetDistance(PointDbl a)
        {
            return Misc.GetDistance(this, a);
        }
        /// <summary>
        /// 角度を取得
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public double GetDegree(PointDbl a)
        {
            return Misc.GetDegree(this, a);
        }

        /// <summary>
        /// コピーを返す
        /// </summary>
        /// <returns></returns>
        public PointDbl GetCopy()
        {
            return new PointDbl(X, Y);
        }

        /// <summary>
        /// 同じ値か確認
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsEqual(PointDbl obj)
        {
            if (this.X != obj.X) return false;
            if (this.Y != obj.Y) return false;
            return true;
        }


        public System.Drawing.Point ToPoint()
        {
            return new System.Drawing.Point((int)X, (int)Y);
        }
        public System.Drawing.PointF ToPointF()
        {
            return new System.Drawing.PointF((float)X, (float)Y);
        }


    }
}
