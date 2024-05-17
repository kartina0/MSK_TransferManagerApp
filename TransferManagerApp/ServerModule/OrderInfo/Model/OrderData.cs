//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Collections.Generic;

using SystemConfig;
using DL_Logger;
using ErrorCodeDefine;


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
        public ORDER_PROCESS process = ORDER_PROCESS.UNLOAD;
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
        public List<OrderStoreData> storeDataList = new List<OrderStoreData>();


        /// <summary>
        /// 値渡しコピー
        /// </summary>
        /// <returns></returns>
        public OrderData Copy() 
        {
            OrderData orderData = null;
            try
            {
                orderData = new OrderData() 
                {
                    orderDate = this.orderDate,
                    postNo = this.postNo,
                    orderDateRequest = this.orderDateRequest,
                    postNoRequest = this.postNoRequest,
                    workCode = this.workCode,
                    index = this.index,
                    workName = this.workName,
                    JANCode = this.JANCode,
                    caseVolume = this.caseVolume,
                    orderCountTotal = this.orderCountTotal,
                    workNameKana = this.workNameKana,
                    maxStackNum = this.maxStackNum,
                    salesPrice = this.salesPrice,
                    process = this.process,
                    createDateTime = this.createDateTime,
                    createLoginId = this.createLoginId,
                    updateDateTime = this.updateDateTime,
                    updateLoginId = this.updateLoginId,
                };

                for (int i = 0; i < Const.MaxStationCount; i++) 
                {
                    orderData.orderCount[i] = this.orderCount[i];
                }
                for (int i = 0; i < Const.MaxStationCount; i++)
                { 
                    orderData.storeCount[i] = this.storeCount[i];
                }
                foreach (OrderStoreData storeData in this.storeDataList) 
                {
                    OrderStoreData orderStoreData = new OrderStoreData();
                    orderStoreData = storeData.Copy();
                    orderData.storeDataList.Add(orderStoreData);
                }

            }
            catch (Exception ex) 
            {
                Logger.WriteLog(LogType.ERROR, ex.Message);
            }
            return orderData;
        }

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
        /// 値渡しコピー
        /// </summary>
        /// <returns></returns>
        public OrderStoreData Copy() 
        {
            OrderStoreData orderStoreData = null;
            try
            {
                orderStoreData = new OrderStoreData()
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
                    caseVolume = this.caseVolume,
                    orderCount = this.orderCount,
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
            return orderStoreData;
        }

    }

}
