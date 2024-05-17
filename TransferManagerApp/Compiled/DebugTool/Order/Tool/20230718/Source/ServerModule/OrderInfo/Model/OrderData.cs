using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SystemConfig;


namespace ServerModule
{
    /// <summary>
    /// 商品ヘッダテーブルデータ
    /// </summary>
    public class OrderData
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
        /// 商品名(漢字)
        /// </summary>
        public string workName = "";
        /// <summary>
        /// JANコード
        /// </summary>
        public string JANCode = "";
        /// <summary>
        /// ケース入数
        /// </summary>
        public double caseVolume = 0;
        /// <summary>
        /// 仕分け数合計
        /// </summary>
        public double orderCountTotal = 0;
        /// <summary>
        /// 商品名(カナ)
        /// </summary>
        public string workNameKana = "";
        /// <summary>
        /// MAX積み付け段数
        /// </summary>
        public int maxStackNum = 0;
        /// <summary>
        /// 売価１
        /// </summary>
        public double salesPrice = 0;
        /// <summary>
        /// 仕分け作業状況
        /// </summary>
        public Const.ORDER_PROCESS process = Const.ORDER_PROCESS.UNLOAD;
        /// <summary>
        /// 仕分け数 (ステーションごと)
        /// </summary>
        public double[] orderCount = new double[Const.MaxStationCount];
        /// <summary>
        /// 店舗数 (ステーションごと)
        /// </summary>
        public int[] storeCount = new int[Const.MaxStationCount];
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
        /// 店別小仕分けリスト
        /// </summary>
        public List<OrderStoreData> storeInfo = new List<OrderStoreData>();

    }


    /// <summary>
    /// 店別小仕分けテーブル データ
    /// </summary>
    public class OrderStoreData
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
        /// ケース入数
        /// </summary>
        public double caseVolume = 0;
        /// <summary>
        /// 仕分け数
        /// </summary>
        public double orderCount = 0;
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
