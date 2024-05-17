using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_CommonLibrary
{
    [Serializable]
    public class RobotPos
    {

        public double X = 0;
        public double Y = 0;
        public double Z = 0;
        public double T = 0;

        public RobotPos()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
            this.T = 0;

        }

        public RobotPos(double x, double y, double z, double t)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.T = t;
        }
        public RobotPos(string pos)
        {
            pos = pos.Replace("(", "");
            pos = pos.Replace(")", "");

            string[] col = pos.Split(',');
            if (col.Length >= 1) X = double.Parse(col[0]);
            if (col.Length >= 2) Y = double.Parse(col[1]);
            if (col.Length >= 3) Z = double.Parse(col[2]);
            if (col.Length >= 4) T = double.Parse(col[3]);
        }

        public void Clear()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
            this.T = 0;
        }


        public static RobotPos operator +(RobotPos a, RobotPos b)
        {
            return new RobotPos(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.T+ b.T);
        }

        public static RobotPos operator -(RobotPos a, RobotPos b)
        {
            return new RobotPos(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.T - b.T);
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + "," + Z + "," + T + ")";
        }
        
        /// <summary>
        /// コピーを返す
        /// </summary>
        /// <returns></returns>
        public RobotPos GetCopy()
        {
            return new RobotPos(X, Y, Z, T);
        }

        public void Copy(ref RobotPos dest)
        {
            dest.X = this.X;
            dest.Y = this.Y;
            dest.Z = this.Z;
            dest.T = this.T;

        }

        /// <summary>
        /// 同じ値か確認
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsEqual(RobotPos obj)
        {
            if (this.X != obj.X) return false;
            if (this.Y != obj.Y) return false;
            if (this.Z != obj.Z) return false;
            if (this.T != obj.T) return false;
            return true;
        }
        /// <summary>
        /// 2DのpointDblへ変換  
        /// </summary>
        /// <returns></returns>
        public PointDbl ToPointDbl()
        {
            return new PointDbl(this.X, this.Y);
        }
    }
}
