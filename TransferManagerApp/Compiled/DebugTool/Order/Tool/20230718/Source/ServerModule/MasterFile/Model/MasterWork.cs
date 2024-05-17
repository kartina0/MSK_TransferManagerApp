using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerModule
{
    /// <summary>
    /// 商品マスタファイル データ
    /// </summary>
    public class MasterWork
    {
        /// <summary>
        /// 納品日
        /// </summary>
        public DateTime pickDate = DateTime.MinValue;
        /// <summary>
        /// 取引先コード
        /// </summary>
        public string supplierCode = "";
        /// <summary>
        /// VDRコード
        /// </summary>
        public string VDRCode = "";
        /// <summary>
        /// 取引先名(漢字)
        /// </summary>
        public string supplierName = "";
        /// <summary>
        /// 商品コード
        /// </summary>
        public string workCode = "";
        /// <summary>
        /// 便No
        /// </summary>
        public int deliveryNo = 0;
        /// <summary>
        /// JANコード
        /// </summary>
        public string JANCode = "";
        /// <summary>
        /// JAN区分
        /// </summary>
        public int JANType = 0;
        /// <summary>
        /// 商品名(漢字)
        /// </summary>
        public string workName = "";
        /// <summary>
        /// センター入数
        /// </summary>
        public double centerCount = 0;
        /// <summary>
        /// パック入数
        /// </summary>
        public double packCount = 0;
        /// <summary>
        /// JANコード(下4桁)
        /// </summary>
        public string JANCode4digits = "";
        /// <summary>
        /// 商品名(カナ)
        /// </summary>
        public string workNameKana = "";


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
