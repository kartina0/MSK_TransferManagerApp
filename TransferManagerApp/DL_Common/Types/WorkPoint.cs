using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_CommonLibrary
{
    public class WorkPoint : RobotPos
    {
        /// <summary>
        /// 有効無効フラグ
        /// </summary>
        public bool enable = false;

        /// <summary>
        /// 取り出し範囲内までワークが来ていない
        /// </summary>
        public bool minRange = true;

        /// <summary>
        /// 取り出し範囲外までワークが行ってしまった
        /// </summary>
        public bool maxRange = true;

        /// <summary>
        /// 完了フラグ
        /// 搬送完了などにしようしてね
        /// </summary>
        public bool complete = false;

        /// <summary>
        /// ロボット位置を返す
        /// </summary>
        public RobotPos Pos
        {
            get { return this; }
            set
            {
                base.X = value.X;
                base.Y = value.Y;
                base.Z = value.Z;
                base.T = value.T;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="pos"></param>
        public WorkPoint()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
            this.T = 0;
            enable = false;
            complete = false;
            maxRange = false;
            minRange = false;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="pos"></param>
        public WorkPoint(string pos)
        {
            pos = pos.Replace("(", "");
            pos = pos.Replace(")", "");

            string[] col = pos.Split(',');
            if (col.Length >= 1) X = double.Parse(col[0]);
            if (col.Length >= 2) Y = double.Parse(col[1]);
            if (col.Length >= 3) Z = double.Parse(col[2]);
            if (col.Length >= 4) T = double.Parse(col[3]);
            if (col.Length >= 5) enable = int.Parse(col[4]) == 1;
            if (col.Length >= 6) complete = int.Parse(col[5]) == 1;
        }

        /// <summary>
        /// データクリア
        /// </summary>
        public new void Clear()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
            this.T = 0;
            enable = false;
            complete = false;
            maxRange = false;
            minRange = false;
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + "," + Z + "," + T + "," + (enable ? "1" : "0") + "," + (complete ? "1" : "0") + ")";
        }

        /// <summary>
        /// コピー
        /// </summary>
        /// <param name="dest"></param>
        public void Copy(ref WorkPoint dest)
        {
            dest.X = this.X;
            dest.Y = this.Y;
            dest.Z = this.Z;
            dest.T = this.T;
            dest.enable = this.enable;
            dest.complete = this.complete;
            dest.minRange = this.minRange;
            dest.maxRange = this.maxRange;
        }
        
    }
}
