using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemConfig;


namespace ServerModule
{
    internal class PickData
    {
    }



    /// <summary>
    /// PICKDATA 先頭レコード
    /// </summary>
    public class FirstRecord
    {
        /// <summary>
        /// 連番
        /// </summary>
        public int index = 0;
        /// <summary>
        /// 処理日付
        /// </summary>
        public DateTime processDate = DateTime.MinValue;
        /// <summary>
        /// 便
        /// </summary>
        public int postNo = 0;
        /// <summary>
        /// 納品日
        /// </summary>
        public DateTime orderDate = DateTime.MinValue;
        /// <summary>
        /// 中仕分リスト
        /// </summary>
        public List<MiddleRecord> middleRecordList = new List<MiddleRecord>();


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FirstRecord()
        {

        }
    }



    /// <summary>
    /// PICKDATA 中仕分レコード
    /// </summary>
    public class MiddleRecord
    {
        /// <summary>
        /// 便
        /// </summary>
        public int postNo = 0;
        /// <summary>
        /// 社区分
        /// </summary>
        public int companyType = 0;
        /// <summary>
        /// JANコード
        /// </summary>
        public string JANCode = "";
        /// <summary>
        /// 商品コード
        /// </summary>
        public string workCode = "";
        /// <summary>
        /// 商品名(漢字)
        /// </summary>
        public string workName = "";
        /// <summary>
        /// 商品名(カナ)
        /// </summary>
        public string workNameKana = "";
        /// <summary>
        /// MAX
        /// </summary>
        public int max = 0;
        /// <summary>
        /// センター入数
        /// </summary>
        public int centerCount = 0;
        /// <summary>
        /// 売価
        /// </summary>
        public int price = 0;
        /// <summary>
        /// 各ステーション数量リスト
        /// </summary>
        public MiddleRecordCount[] countList = new MiddleRecordCount[9];
        /// <summary>
        /// 小仕分レコードリスト
        /// </summary>
        public List<MinimumRecord> minimumRecordList = new List<MinimumRecord>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MiddleRecord()
        {
            for (int i = 0; i < 9; i++)
                countList[i] = new MiddleRecordCount();
        }
    }
    /// <summary>
    /// PICKDATA 中仕分レコード 各ステーション数量
    /// </summary>
    public class MiddleRecordCount
    {
        /// <summary>
        /// 数量
        /// </summary>
        public int count = 0;
        /// <summary>
        /// 店舗数
        /// </summary>
        public int storeCount = 0;
    }



    /// <summary>
    /// PICKDATA 小仕分レコード
    /// </summary>
    public class MinimumRecord
    {
        /// <summary>
        /// アイル
        /// </summary>
        public int aisleNo = 0;
        /// <summary>
        /// スロット
        /// </summary>
        public int slotNo = 0;
        /// <summary>
        /// 数量
        /// </summary>
        public int count = 0;
    }



    /// <summary>
    /// PICKDATA 最終レコード
    /// </summary>
    public class EndRecord
    {
        /// <summary>
        /// ID
        /// </summary>
        public int id = 0;
        /// <summary>
        /// 件数
        /// </summary>
        public int count = 0;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EndRecord()
        {

        }
    }

}
