using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ServerModule
{

    /// <summary>
    /// 商品ヘッダ実績テーブル
    /// </summary>
    public class ExecuteData
    {
        /// <summary>
        /// 仕分納品日
        /// </summary>
        public DateTime orderDate = DateTime.MinValue;
        /// <summary>
        /// 仕分便No
        /// </summary>
        public int postNo = 0;
        /// <summary>
        /// 発注納品日
        /// </summary>
        public DateTime orderDateRequest = DateTime.MinValue;
        /// <summary>
        /// 発注便No
        /// </summary>
        public int postNoRequest = 0;
        /// <summary>
        /// 商品コード
        /// </summary>
        public string workCode = "";
        /// <summary>
        /// 連番
        /// </summary>
        public int index = 0;
        /// <summary>
        /// JANコード
        /// </summary>
        public string JANCode = "";
        /// <summary>
        /// 仕分け数合計
        /// </summary>
        public double orderCountTotal = 0;
        /// <summary>
        /// 仕分け完了数合計
        /// </summary>
        public double orderCompCountTotal = 0;
        /// <summary>
        /// 取込日時
        /// </summary>
        public DateTime loadDateTime = DateTime.MinValue;
        /// <summary>
        /// 登録日時
        /// </summary>
        public DateTime createDateTime = DateTime.MinValue;
        /// <summary>
        /// 登録ログインID
        /// </summary>
        public string createLoginId = "";
        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime updateDateTime = DateTime.MinValue;
        /// <summary>
        /// 更新ログインID
        /// </summary>
        public string updateLoginId = "";

        /// <summary>
        /// 店別小仕分け実績リスト
        /// </summary>
        public List<ExecuteStoreData> storeInfo = new List<ExecuteStoreData>();

    }

    /// <summary>
    /// 店別小仕分け実績テーブル
    /// </summary>
    public class ExecuteStoreData
    {
        /// <summary>
        /// 仕分納品日
        /// </summary>
        public DateTime orderDate = DateTime.MinValue;
        /// <summary>
        /// 仕分便No
        /// </summary>
        public int postNo = 0;
        /// <summary>
        /// 発注納品日
        /// </summary>
        public DateTime orderDateRequest = DateTime.MinValue;
        /// <summary>
        /// 発注便No
        /// </summary>
        public int postNoRequest = 0;
        /// <summary>
        /// 商品コード
        /// </summary>
        public string workCode = "";
        /// <summary>
        /// 連番
        /// </summary>
        public int index = 0;
        /// <summary>
        /// 店コード
        /// </summary>
        public string storeCode = "";
        /// <summary>
        /// ステーションNo
        /// </summary>
        public int stationNo = 0;
        /// <summary>
        /// アイルNo
        /// </summary>
        public int aisleNo = 0;
        /// <summary>
        /// スロットNo
        /// </summary>
        public int slotNo = 0;
        /// <summary>
        /// 仕分け数
        /// </summary>
        public double orderCount = 0;
        /// <summary>
        /// 仕分け完了数
        /// </summary>
        public double orderCompCount = 0;
        /// <summary>
        /// 登録日時
        /// </summary>
        public DateTime createDateTime = DateTime.MinValue;
        /// <summary>
        /// 登録ログインID
        /// </summary>
        public string createLoginId = "";
        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime updateDateTime = DateTime.MinValue;
        /// <summary>
        /// 更新ログインID
        /// </summary>
        public string updateLoginId = "";

    }

}
