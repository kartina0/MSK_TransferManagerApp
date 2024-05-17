//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using SystemConfig;


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
        public DateTime orderDate = DateTime.MinValue;
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
        /// <summary>
        /// 電話番号
        /// </summary>
        public string phoneNumber = "";
        /// <summary>
        /// 便情報リスト
        /// </summary>
        public MasterStorePost[] postInfo = new MasterStorePost[Const.MaxPostCount];


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MasterStore() 
        {
            for (int i = 0; i < Const.MaxPostCount; i++)
                postInfo[i] = new MasterStorePost();
        }
    }

    /// <summary>
    /// 店マスタファイル 便情報データ
    /// </summary>
    public class MasterStorePost
    {
        /// <summary>
        /// コース
        /// </summary>
        public int course = 0;
        /// <summary>
        /// 順
        /// </summary>
        public int process = 0;
        /// <summary>
        /// ST
        /// </summary>
        public int station = 0;
        /// <summary>
        /// アイル
        /// </summary>
        public int aisle = 0;
        /// <summary>
        /// スロット
        /// </summary>
        public int slot = 0;
        /// <summary>
        /// ドッグNo
        /// </summary>
        public string dogNo = "";
        /// <summary>
        /// 搬入条件
        /// </summary>
        public string condition = "";
        /// <summary>
        /// 到着
        /// </summary>
        public string timeArrive = "";
        /// <summary>
        /// 入場
        /// </summary>
        public string timeEntry = "";
        /// <summary>
        /// 出発
        /// </summary>
        public string timeDepart = "";
        /// <summary>
        /// 終了
        /// </summary>
        public string timeFinish = "";
        /// <summary>
        /// 運送会社
        /// </summary>
        public string company = "";
        /// <summary>
        /// 運送会社名
        /// </summary>
        public string companyName = "";
        /// <summary>
        /// 車種
        /// </summary>
        public string carType = "";
    }
}
