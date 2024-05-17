using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_CommonLibrary
{
    public class RobotSpeed
    {

        public double X = 0;
        public double Y = 0;
        public double Z = 0;
        public double T = 0;

        public RobotSpeed()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
            this.T = 0;

        }

        public RobotSpeed(double x, double y, double z, double t)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.T = t;
        }
        public RobotSpeed(string pos)
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


        public static RobotSpeed operator +(RobotSpeed a, RobotSpeed b)
        {
            return new RobotSpeed(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.T+ b.T);
        }

        public static RobotSpeed operator -(RobotSpeed a, RobotSpeed b)
        {
            return new RobotSpeed(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.T - b.T);
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + "," + Z + "," + T + ")";
        }
        
        /// <summary>
        /// コピーを返す
        /// </summary>
        /// <returns></returns>
        public RobotSpeed GetCopy()
        {
            return new RobotSpeed(X, Y, Z, T);
        }

        public void Copy(ref RobotSpeed dest)
        {
            dest.X = this.X;
            dest.Y = this.Y;
            dest.Z = this.Z;
            dest.T = this.T;

        }
    }
}
