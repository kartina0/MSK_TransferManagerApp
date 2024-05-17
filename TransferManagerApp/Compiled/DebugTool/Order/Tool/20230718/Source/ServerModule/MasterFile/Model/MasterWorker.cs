using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerModule
{
    /// <summary>
    /// 作業者マスタファイル データ
    /// </summary>
    public class MasterWorker
    {
        /// <summary>
        /// 作業者No(代表)
        /// </summary>
        public int workerNo = 0;
        /// <summary>
        /// 作業者名
        /// </summary>
        public string workerName = "";
        /// <summary>
        /// 作業者N0 1便
        /// </summary>
        public int workerNo_1 = 0;
        /// <summary>
        /// 作業者No 2便
        /// </summary>
        public int workerNo_2 = 0;
        /// <summary>
        /// 作業者No 3便
        /// </summary>
        public int workerNo_3 = 0;
        /// <summary>
        /// 作業者No 当日3便
        /// </summary>
        public int workerNo_3_Actual = 0;


        /// <summary>
        /// データをセット
        /// </summary>
        /// <returns></returns>
        public UInt32 Set(Dictionary<string, object> dataDic)
        {
            UInt32 rc = 0;
            try
            {

            }
            catch (Exception ex)
            {
            }
            return rc;
        }
    }
}
