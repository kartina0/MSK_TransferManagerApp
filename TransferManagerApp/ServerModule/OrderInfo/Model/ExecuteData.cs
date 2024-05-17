//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Collections.Generic;

using DL_Logger;


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
        public string loadDateTime = "00000000000000";
        //public DateTime loadDateTime = DateTime.MinValue;
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
        public List<ExecuteStoreData> storeDataList = new List<ExecuteStoreData>();


        /// <summary>
        /// 更新有無
        /// TrueならDB書込み
        /// </summary>
        public bool isChanged = true;


        /// <summary>
        /// 値渡しコピー
        /// </summary>
        /// <returns></returns>
        public ExecuteData Copy()
        {
            ExecuteData executeData = null;
            try
            {
                executeData = new ExecuteData()
                {
                    orderDate = this.orderDate,
                    postNo = this.postNo,
                    orderDateRequest = this.orderDateRequest,
                    postNoRequest = this.postNoRequest,
                    workCode = this.workCode,
                    index = this.index,
                    JANCode = this.JANCode,
                    orderCountTotal = this.orderCountTotal,
                    orderCompCountTotal = this.orderCompCountTotal,
                    loadDateTime = this.loadDateTime,
                    createDateTime = this.createDateTime,
                    createLoginId = this.createLoginId,
                    updateDateTime = this.updateDateTime,
                    updateLoginId = this.updateLoginId,
                };

                foreach (ExecuteStoreData storeData in this.storeDataList)
                {
                    ExecuteStoreData executeStoreData = new ExecuteStoreData();
                    executeStoreData = storeData.Copy();
                    executeData.storeDataList.Add(executeStoreData);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, ex.Message);
            }
            return executeData;
        }
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

        /// <summary>
        /// バッチNo (マテハン変換後)
        /// </summary>
        public int batchNo_MH = 0;
        /// <summary>
        /// アイルNo (マテハン変換後)
        /// </summary>
        public int aisleNo_MH = 0;
        /// <summary>
        /// スロットNo (マテハン変換後)
        /// </summary>
        public int slotNo_MH = 0;

        /// <summary>
        /// 更新有無
        /// TrueならDB書込み
        /// </summary>
        public bool isChanged = true;


        /// <summary>
        /// 値渡しコピー
        /// </summary>
        /// <returns></returns>
        public ExecuteStoreData Copy()
        {
            ExecuteStoreData executeStoreData = null;
            try
            {
                executeStoreData = new ExecuteStoreData()
                {
                    orderDate = this.orderDate,
                    postNo = this.postNo,
                    orderDateRequest = this.orderDateRequest,
                    postNoRequest = this.postNoRequest,
                    workCode = this.workCode,
                    index = this.index,
                    storeCode = this.storeCode,
                    stationNo = this.stationNo,
                    aisleNo = this.aisleNo,
                    slotNo = this.slotNo,
                    orderCount = this.orderCount,
                    orderCompCount = this.orderCompCount,
                    createDateTime = this.createDateTime,
                    createLoginId = this.createLoginId,
                    updateDateTime = this.updateDateTime,
                    updateLoginId = this.updateLoginId
                };

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, ex.Message);
            }
            return executeStoreData;
        }

    }

}
