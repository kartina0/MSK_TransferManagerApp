using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerModule
{
    /// <summary>
    /// 店マスタファイル データ
    /// </summary>
    public class MasterStore
    {
        /// <summary>
        /// 納品日
        /// </summary>
        public DateTime pickDate = DateTime.MinValue;
        /// <summary>
        /// 社区分
        /// </summary>
        public int companyType = 0;
        /// <summary>
        /// 店コード
        /// </summary>
        public string storeCode = "";
        /// <summary>
        /// 店名(漢字)
        /// </summary>
        public string storeName = "";

        // 1便
        /// <summary>
        /// コース
        /// </summary>
        public int course_1 = 0;
        /// <summary>
        /// 順
        /// </summary>
        public int turn_1 = 0;
        /// <summary>
        /// ST
        /// </summary>
        public int ST_1 = 0;
        /// <summary>
        /// アイル
        /// </summary>
        public int ailse_1 = 0;
        /// <summary>
        /// スロット
        /// </summary>
        public int slot_1 = 0;

        // 2便
        /// <summary>
        /// コース
        /// </summary>
        public int course_2 = 0;
        /// <summary>
        /// 順
        /// </summary>
        public int turn_2 = 0;
        /// <summary>
        /// ST
        /// </summary>
        public int ST_2 = 0;
        /// <summary>
        /// アイル
        /// </summary>
        public int ailse_2 = 0;
        /// <summary>
        /// スロット
        /// </summary>
        public int slot_2 = 0;

        // 3便
        /// <summary>
        /// コース
        /// </summary>
        public int course_3 = 0;
        /// <summary>
        /// 順
        /// </summary>
        public int turn_3 = 0;
        /// <summary>
        /// ST
        /// </summary>
        public int ST_3 = 0;
        /// <summary>
        /// アイル
        /// </summary>
        public int ailse_3 = 0;
        /// <summary>
        /// スロット
        /// </summary>
        public int slot_3 = 0;


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
