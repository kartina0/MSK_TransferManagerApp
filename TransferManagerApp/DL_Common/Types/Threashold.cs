using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_CommonLibrary
{
    public class Threshold
    {
        public double min = 0;
        public double max = 0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Threshold()
        {
            this.min = 0;
            this.max = 0;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Threshold(double min, double max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Threshold(string treshold)
        {
            treshold = treshold.Replace("(", "");
            treshold = treshold.Replace(")", "");

            string[] col = treshold.Split(',');
            if (col.Length >= 1) this.min = double.Parse(col[0]);
            if (col.Length >= 2) this.max = double.Parse(col[1]);
        }

        public override string ToString()
        {
            return "(" + min + "," + max + ")";
        }

        public bool IsRange(double v)
        {
            return v >= min && v <= max;
        }

        /// <summary>
        /// 閾値を使用するのか取得
        /// </summary>
        public bool IsEnable
        {
            get
            {
                double diff = max - min;
                if (diff > 0 && max != 0)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// コピーを返す
        /// </summary>
        /// <returns></returns>
        public Threshold GetCopy()
        {
            return new Threshold(min, max);
        }
    }
}
