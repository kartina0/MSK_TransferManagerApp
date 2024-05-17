//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.IO;

using SystemConfig;
using DL_CommonLibrary;
using DL_Logger;
using ErrorCodeDefine;
using System.Text;
using System.Windows.Documents;

namespace ServerModule
{
    /// <summary>
    /// 仕分データ管理クラス
    /// ※データはDBかCSVか未定。このクラスの下でDBとCSVの処理を切り替え
    /// </summary>
    public class OrderInfoManager
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "OrderInfoManager";

        /// <summary>
        /// IPアドレス
        /// </summary>
        private string _ipAddress = "";
        /// <summary>
        /// 接続文字列
        /// </summary>
        private string _connectionString = "";

        /// <summary>
        /// 仕分けデータインターフェース DB/CSV
        /// </summary>
        public IOrderInfo OrderData_IF = null;


        /// <summary>
        /// 仕分データリスト
        /// 1~3便
        /// </summary>
        public List<OrderData>[] OrderDataList = null;
        /// <summary>
        /// 仕分実績データリスト
        /// 1~3便
        /// </summary>
        public List<ExecuteData>[] ExecuteDataList = null;

        /// <summary>
        /// 商品ヘッダテーブル名
        /// </summary>
        public string _workHeader_TableName = "dp01_pick_head_0";
        /// <summary>
        /// 店別小仕分けテーブル名
        /// </summary>
        public string _storeInfo_TableName = "dp02_pick_detail_0";
        /// <summary>
        /// 商品ヘッダ実績テーブル名
        /// </summary>
        public string _workHeaderExecute_TableName = "dp11_comp_head_0";
        /// <summary>
        /// 店別小仕分け実績テーブル名
        /// </summary>
        public string _storeInfoExecute_TableName = "dp12_comp_detail_0";


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OrderInfoManager()
        {
            try
            {
                _ipAddress = IniFile.DBIpAddress;
                _connectionString = IniFile.DB_SQL_Connection;

                if (IniFile.OrderInfoType == ORDER_INFO_TYPE.DB)
                    OrderData_IF = new OrderInfo_IF_DB();
                else if (IniFile.OrderInfoType == ORDER_INFO_TYPE.CSV)
                    OrderData_IF = new OrderInfo_IF_CSV();

                OrderDataList = new List<OrderData>[Const.MaxPostCount];
                ExecuteDataList = new List<ExecuteData>[Const.MaxPostCount];
                for (int i = 0; i < Const.MaxPostCount; i++) 
                {
                    OrderDataList[i] = new List<OrderData>();
                    ExecuteDataList[i] = new List<ExecuteData>();
                }

                //// デバッグ
                //Debug_CreateData();

            }
            catch (Exception ex) 
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
            }
        }



        #region DB操作
        /// <summary>
        /// 仕分データ 読み出し
        /// 1便分
        /// </summary>
        /// <param name="postIndex">仕分便index</param>
        /// <returns>エラーコード</returns>
        public UInt32 ReadOrderData(int postIndex, ORDER_PROCESS process, DateTime dt = default(DateTime))
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 引き数にdtが指定されてなかったら現在日時とする
                if (dt == default(DateTime))
                    // 日付切り替わり処理
                    dt = DateConvert();


                // ------------------------------
                // 商品ヘッダテーブル
                // 1便分を読み出し
                // ------------------------------
                string workHeader_TableName = $"{_workHeader_TableName}{dt.Year.ToString("D4")}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";
                List<OrderData> readWorks = null;
                rc = OrderData_IF.GetOrderItemList(workHeader_TableName, dt, postIndex + 1, process, out readWorks);


                if (readWorks != null) 
                {
                    // 読み出した商品ループ
                    foreach (OrderData readWork in readWorks) 
                    {
                        // ------------------------------
                        // 商品ヘッダテーブル
                        // 仕分作業状況を更新
                        // ------------------------------
                        rc = OrderData_IF.SetOrderItem(workHeader_TableName, readWork.orderDate, readWork.postNo, readWork.orderDateRequest, readWork.postNoRequest, readWork.workCode, readWork.index, ORDER_PROCESS.LOADED, IniFile.DBLoginId);


                        // ------------------------------
                        // 仕分データリスト更新
                        // ------------------------------
                        // 該当する商品データが既にあるか
                        OrderData existWork = OrderDataList[postIndex].Find(n => n.workCode == readWork.workCode);
                        if (existWork == null)
                        {// なかったら追加
                            OrderDataList[postIndex].Add(readWork);
                        }
                        else
                        {// あったら更新
                            existWork.index = readWork.index;
                            existWork.caseVolume = readWork.caseVolume;
                            existWork.orderCountTotal = readWork.orderCountTotal;
                            existWork.maxStackNum = readWork.maxStackNum;
                            existWork.salesPrice = readWork.salesPrice;
                            for (int i = 0; i < Const.MaxStationCount; i++) 
                            {
                                existWork.orderCount[i] = readWork.orderCount[i];
                                existWork.storeCount[i] = readWork.storeCount[i];
                            }
                        }


                        // ------------------------------
                        // 店別小仕分けテーブル
                        // 1商品分読み出し
                        // ------------------------------
                        string storeOrder_TableName = $"{_storeInfo_TableName}{dt.Year.ToString("D4")}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";
                        List<OrderStoreData> readStores = null;
                        rc = OrderData_IF.GetOrderStoreItem(storeOrder_TableName, dt, postIndex + 1, readWork.workCode, out readStores);


                        // ------------------------------
                        // 仕分データリストの店別小仕分けリストを更新
                        // ------------------------------
                        if (readStores != null)
                        {
                            // 再度、仕分データリストから商品を抽出
                            existWork = OrderDataList[postIndex].Find(n => n.workCode == readWork.workCode);

                            // 各店データを店別小仕分けリストにセット
                            foreach (OrderStoreData readStore in readStores)
                            {
                                // 該当する店データがあるか
                                OrderStoreData existStoreData = existWork.storeDataList.Find(n => n.storeCode == readStore.storeCode);
                                if (existStoreData == null)
                                {// なかったら追加
                                    existWork.storeDataList.Add(readStore);
                                }
                                else
                                {// あったら更新
                                    existStoreData.index = readStore.index;
                                    existStoreData.stationNo = readStore.stationNo;
                                    existStoreData.aisleNo = readStore.aisleNo;
                                    existStoreData.slotNo = readStore.slotNo;
                                    existStoreData.caseVolume = readStore.caseVolume;
                                    existStoreData.orderCount = readStore.orderCount;
                                }
                            }
                        }
                        else
                        {
                            // 店別テーブルにある商品が商品ヘッダテーブルにない！！！

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 仕分データ 読み出し
        /// 1商品分
        /// </summary>
        /// <param name="postIndex">仕分便index</param>
        /// <param name="workCode">商品コード</param>
        /// <returns>エラーコード</returns>
        public UInt32 ReadOrderData(int postIndex, string workCode, ORDER_PROCESS process, DateTime dt = default(DateTime))
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 引き数にdtが指定されてなかったら現在日時とする
                if (dt == default(DateTime))
                    // 日付切り替わり処理
                    dt = DateConvert();


                // ------------------------------
                // 商品ヘッダテーブル
                // 1商品を読み出し
                // ------------------------------
                // 1商品を読み出し
                string workHeader_TableName = $"{_workHeader_TableName}{dt.Year.ToString("D4")}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";
                OrderData readWork = null;
                rc = OrderData_IF.GetOrderItem(workHeader_TableName, dt, postIndex + 1, workCode, process, out readWork);


                if (readWork != null) 
                {
                    // ------------------------------
                    // 商品ヘッダテーブル
                    // 仕分作業状況を更新
                    // ------------------------------
                    rc = OrderData_IF.SetOrderItem(workHeader_TableName, readWork.orderDate, readWork.postNo, readWork.orderDateRequest, readWork.postNoRequest, readWork.workCode, readWork.index, ORDER_PROCESS.LOADED, IniFile.DBLoginId);


                    // ------------------------------
                    // 仕分データリストに商品追加/更新
                    // ------------------------------
                    // 該当する商品が既にあるか
                    OrderData existWork = OrderDataList[postIndex].Find(n => n.workCode == readWork.workCode);
                    if (existWork == null)
                    {// なかったら追加
                        OrderDataList[postIndex].Add(readWork);
                    }
                    else
                    {// あったら更新
                        existWork.index = readWork.index;
                        existWork.caseVolume = readWork.caseVolume;
                        existWork.orderCountTotal = readWork.orderCountTotal;
                        existWork.maxStackNum = readWork.maxStackNum;
                        existWork.salesPrice = readWork.salesPrice;
                        for (int i = 0; i < Const.MaxStationCount; i++)
                        {
                            existWork.orderCount[i] = readWork.orderCount[i];
                            existWork.storeCount[i] = readWork.storeCount[i];
                        }
                    }


                    // ------------------------------
                    // 店別小仕分けテーブル
                    // 1商品分読み出し
                    // ------------------------------
                    // 店別小仕分けテーブルから店データを読みだし
                    string storeOrder_TableName = $"{_storeInfo_TableName}{dt.Year.ToString("D4")}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";
                    List<OrderStoreData> readStores = null;
                    rc = OrderData_IF.GetOrderStoreItem(storeOrder_TableName, dt, postIndex + 1, workCode, out readStores);


                    // ------------------------------
                    // 仕分データリストの店データに商品追加/更新
                    // ------------------------------
                    if (readStores != null)
                    {
                        // 再度、仕分データリストから商品を抽出
                        existWork = OrderDataList[postIndex].Find(n => n.workCode == readWork.workCode);


                        foreach (OrderStoreData readStore in readStores)
                        {
                            // 該当する店データがあるか
                            OrderStoreData existStoreData = existWork.storeDataList.Find(n => n.storeCode == readStore.storeCode);
                            if (existStoreData == null)
                            {// なかったら追加
                                existWork.storeDataList.Add(readStore);
                            }
                            else
                            {// あったら更新
                                existStoreData.index = readStore.index;
                                existStoreData.stationNo = readStore.stationNo;
                                existStoreData.aisleNo = readStore.aisleNo;
                                existStoreData.slotNo = readStore.slotNo;
                                existStoreData.caseVolume = readStore.caseVolume;
                                existStoreData.orderCount = readStore.orderCount;
                            }
                        }
                    }
                    else
                    {
                        // 店別テーブルにある商品が商品ヘッダテーブルにない！！！

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 店別小仕分けデータ 読み出し
        /// 1行分
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="orderDateTime">仕分納品日</param>
        /// <param name="postNo">仕分便No</param>
        /// <param name="workCode">商品コード</param>
        /// <param name="itemList">1商品分のデータリスト</param>
        /// <returns>エラーコード</returns>
        public UInt32 ReadOrderStoreItem(string tableName, int postIndex, string workCode)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 現在日時
                DateTime dt = DateTime.Now;
                // 日付切り替わり処理
                dt = DateConvert();


                // ------------------------------
                // DB読み出し
                // ------------------------------
                // 1商品分の店別小仕分けリストを読み出し
                List<OrderStoreData> reads = null;
                rc = OrderData_IF.GetOrderStoreItem(tableName, dt, postIndex, workCode, out reads);


                // ------------------------------
                // 仕分データリスト内の店別小仕分けリストを更新
                // ------------------------------
                // まず仕分データリストに該当する商品があるのか
                OrderData existData = OrderDataList[postIndex].Find(n => n.workCode == workCode);
                if (existData != null)
                {
                    // 各店データを店別小仕分けリストにセット
                    foreach (OrderStoreData read in reads)
                    {
                        // 該当する店データがあるか
                        OrderStoreData existStoreData = existData.storeDataList.Find(n => n.storeCode == read.storeCode);
                        if (existStoreData == null)
                        {// なかったら
                            // 店データを追加
                            existData.storeDataList.Add(read);
                        }
                        else
                        {// あったら
                            // 更新
                            existStoreData.index = read.index;
                            existStoreData.stationNo = read.stationNo;
                            existStoreData.aisleNo = read.aisleNo;
                            existStoreData.slotNo = read.slotNo;
                            existStoreData.caseVolume = read.caseVolume;
                            existStoreData.orderCount = read.orderCount;
                        }
                    }
                }
                else 
                {
                    // 店別テーブルにある商品が商品ヘッダテーブルにない！！！

                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }

        /// <summary>
        /// 仕分実績データ 読み出し
        /// 1便分
        /// </summary>
        public UInt32 ReadExecuteData(int postIndex, DateTime dt = default(DateTime))
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 引き数にdtが指定されてなかったら現在日時とする
                if (dt == default(DateTime))
                    // 日付切り替わり処理
                    dt = DateConvert();


                // ------------------------------
                // 商品ヘッダ実績テーブル
                // 1便分を読み出し
                // ------------------------------
                string executeWorkHeader_TableName_ = $"{_workHeaderExecute_TableName}{dt.Year.ToString("D4")}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";
                List<ExecuteData> readWorks = null;
                rc = OrderData_IF.GetExecuteItem(executeWorkHeader_TableName_, dt, postIndex + 1, out readWorks);


                if (readWorks != null)
                {
                    foreach (ExecuteData readWork in readWorks) 
                    {
                        // ------------------------------
                        // 仕分実績データリスト更新
                        // ------------------------------
                        // 該当する商品が既にあるか
                        ExecuteData existWork = ExecuteDataList[postIndex].Find(n => n.workCode == readWork.workCode);
                        if (existWork == null)
                        {// なかったら追加
                            ExecuteDataList[postIndex].Add(readWork);
                        }
                        else
                        {// あったら更新
                            existWork.index = readWork.index;
                            existWork.orderCountTotal = readWork.orderCountTotal;
                            existWork.orderCompCountTotal = readWork.orderCompCountTotal;
                        }


                        // ------------------------------
                        // 店別小仕分けテーブル
                        // 1商品分読み出し
                        // ------------------------------
                        // 店別小仕分けテーブルから店データを読みだし
                        string executeStoreInfo_TableNam = $"{_storeInfoExecute_TableName}{dt.Year.ToString("D4")}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";
                        List<ExecuteStoreData> readStores = null;
                        rc = OrderData_IF.GetExecStoreItems(executeStoreInfo_TableNam, dt, postIndex + 1, readWork.workCode, out readStores);


                        // ------------------------------
                        // 仕分データリストの店データに商品追加/更新
                        // ------------------------------
                        if (readStores != null)
                        {
                            // 再度、仕分データリストから商品を抽出
                            existWork = ExecuteDataList[postIndex].Find(n => n.workCode == readWork.workCode);


                            foreach (ExecuteStoreData readStore in readStores)
                            {
                                // 該当する店データがあるか
                                ExecuteStoreData existStoreData = existWork.storeDataList.Find(n => n.storeCode == readStore.storeCode);
                                if (existStoreData == null)
                                {// なかったら追加
                                    existWork.storeDataList.Add(readStore);
                                }
                                else
                                {// あったら更新
                                    existStoreData.index = readStore.index;
                                    existStoreData.stationNo = readStore.stationNo;
                                    existStoreData.aisleNo = readStore.aisleNo;
                                    existStoreData.slotNo = readStore.slotNo;
                                    existStoreData.orderCount = readStore.orderCount;
                                    existStoreData.orderCompCount = readStore.orderCompCount;
                                }
                            }
                        }
                        else
                        {
                            // 店別テーブルにある商品が商品ヘッダテーブルにない！！！

                        }

                    }

                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        
        /// <summary>
        /// 仕分実績データ 書込み
        /// 1便分
        /// </summary>
        public UInt32 WriteExecuteData(int postIndex)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 日付
                DateTime dt = DateTime.Now;
                // 日付切り替わり処理
                dt = DateConvert();


                string executeWorkHeader_TableName_ = $"{_workHeaderExecute_TableName}{dt.Year.ToString("D4")}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";
                string executeStoreInfo_TableNam = $"{_storeInfoExecute_TableName}{dt.Year.ToString("D4")}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";

                foreach (ExecuteData data in ExecuteDataList[postIndex])
                {

                    // ------------------------------
                    // 商品ヘッダ実績
                    // ------------------------------
                    if (data.isChanged)
                    {
                        //Logger.WriteLog(LogType.INFO, $"write  {data.JANCode}.....");

                        // 更新されていたら書込み
                        rc = OrderData_IF.SetExecuteItem(executeWorkHeader_TableName_, data, IniFile.DBLoginId);
                        if (STATUS_SUCCESS(rc))
                        {
                            data.isChanged = false;

                            // バックアップファイルを削除
                            DeleteExecuteDataBackup(data);
                        }
                        else
                        {
                            // バックアップファイル作成
                            rc = WriteExecuteDataBackup(data);
                            if (STATUS_SUCCESS(rc))
                            {
                                data.isChanged = false;
                            }
                        }
                    }


                    // ------------------------------
                    // 店別小仕分け実績
                    // ------------------------------
                    foreach (ExecuteStoreData storeData in data.storeDataList)
                    {
                        if (storeData.isChanged)
                        {
                            // 更新されていたら書込み
                            rc = OrderData_IF.SetExecStoreItems(executeStoreInfo_TableNam, storeData, IniFile.DBLoginId);
                            //Logger.WriteLog(LogType.INFO, $"★★★★書込★★★★  {storeData.storeCode} : {storeData.orderCompCount}");
                            if (STATUS_SUCCESS(rc))
                            {
                                storeData.isChanged = false;

                                // バックアップファイルを削除
                                DeleteExecuteDataBackup(storeData);
                            }
                            else 
                            {
                                // バックアップファイル作成
                                rc = WriteExecuteDataBackup(storeData);
                                if (STATUS_SUCCESS(rc))
                                {
                                    storeData.isChanged = false;
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }





        /// <summary>
        /// 仕分実績データ 書込み バックアップファイル
        /// 商品ヘッダ実績 1行分
        /// </summary>
        public UInt32 WriteExecuteDataBackup(ExecuteData work)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 日付
                DateTime dt = DateTime.Now;
                // 日付切り替わり処理
                dt = DateConvert();


                // 商品名取得
                int postNo = work.postNo;
                string workName = "";
                OrderData d = OrderDataList[postNo - 1].Where(x => x.JANCode == work.JANCode).FirstOrDefault();
                if(d != null)
                    workName = (d.workName).Trim();

                // パス作成
                string executeWorkHeader_TableName = $"{_workHeaderExecute_TableName}{dt.Year.ToString("D4")}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";
                string dir = $"{IniFile.DBBackup}\\{executeWorkHeader_TableName}";
                string fileName = $"{work.orderDate.ToString("yyyyMMdd")}_{work.postNo}_{work.workCode}_{work.JANCode}_{work.index}_{workName}";
                string filePath = System.IO.Path.Combine(dir, $"{fileName}.csv");

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                DateTime currentDt = DateTime.Now;

                // 1行作成
                string row = "";
                row += $"{work.orderDate.ToString("yyyyMMdd")},";
                row += $"{work.postNo},";
                row += $"{work.orderDateRequest.ToString("yyyyMMdd")},";
                row += $"{work.postNoRequest},";
                row += $"{work.workCode},";
                row += $"{work.index},";
                row += $"{work.JANCode},";
                row += $"{work.orderCountTotal},";
                row += $"{work.orderCompCountTotal},";
                row += $"{work.loadDateTime},";
                row += $"{work.createDateTime.ToString("yyyyMMdd")},";
                row += $"{work.createDateTime.ToString("HHmmss")},";
                row += $"{work.createLoginId},";
                row += $"{currentDt.ToString("yyyyMMdd")},";
                row += $"{currentDt.ToString("HHmmss")},";
                row += $"{IniFile.DBLoginId}";

                // ファイル書込み
                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.GetEncoding("shift_jis")))
                {
                    writer.WriteLine(row);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 仕分実績データ 書込み バックアップファイル
        /// 店別小仕分け実績 1行分
        /// </summary>
        public UInt32 WriteExecuteDataBackup(ExecuteStoreData store)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 日付
                DateTime dt = DateTime.Now;
                // 日付切り替わり処理
                dt = DateConvert();


                // 商品名取得
                int postNo = store.postNo;
                string janCode = "";
                string workName = "";
                OrderData d = OrderDataList[postNo - 1].Where(x => x.workCode == store.workCode).FirstOrDefault();
                if (d != null)
                {
                    janCode = d.JANCode;
                    workName = (d.workName).Trim();
                }

                // パス作成
                string executeStoreInfo_TableName = $"{_storeInfoExecute_TableName}{dt.Year.ToString("D4")}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";
                string dir = $"{IniFile.DBBackup}\\{executeStoreInfo_TableName}";
                string fileName = $"{store.orderDate.ToString("yyyyMMdd")}_{store.postNo}_{store.workCode}_{janCode}_{store.storeCode}_{store.index}_{workName}";
                string filePath = System.IO.Path.Combine(dir, $"{fileName}.csv");

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                DateTime currentDt = DateTime.Now;

                // 1行作成
                string row = "";
                row += $"{store.orderDate.ToString("yyyyMMdd")},";
                row += $"{store.postNo},";
                row += $"{store.orderDateRequest.ToString("yyyyMMdd")},";
                row += $"{store.postNoRequest},";
                row += $"{store.workCode},";
                row += $"{store.index},";
                row += $"{store.storeCode},";
                row += $"{store.stationNo},";
                row += $"{store.aisleNo},";
                row += $"{store.slotNo},";
                row += $"{store.orderCount},";
                row += $"{store.orderCompCount},";
                row += $"{store.createDateTime.ToString("yyyyMMdd")},";
                row += $"{store.createDateTime.ToString("HHmmss")},";
                row += $"{store.createLoginId},";
                row += $"{currentDt.ToString("yyyyMMdd")},";
                row += $"{currentDt.ToString("HHmmss")},";
                row += $"{IniFile.DBLoginId}";

                // ファイル書込み
                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.GetEncoding("shift_jis")))
                {
                    writer.WriteLine(row);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 仕分実績データ 削除 バックアップファイル
        /// 商品ヘッダ実績 1行分
        /// </summary>
        public UInt32 DeleteExecuteDataBackup(ExecuteData work)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 日付
                DateTime dt = DateTime.Now;
                // 日付切り替わり処理
                dt = DateConvert();


                // 商品名取得
                int postNo = work.postNo;
                string workName = "";
                OrderData d = OrderDataList[postNo - 1].Where(x => x.JANCode == work.JANCode).FirstOrDefault();
                if (d != null)
                    workName = (d.workName).Trim();

                // パス作成
                string executeWorkHeader_TableName = $"{_workHeaderExecute_TableName}{dt.Year.ToString("D4")}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";
                string dir = $"{IniFile.DBBackup}\\{executeWorkHeader_TableName}";
                string fileName = $"{work.orderDate.ToString("yyyyMMdd")}_{work.postNo}_{work.workCode}_{work.JANCode}_{work.index}_{workName}";
                string filePath = System.IO.Path.Combine(dir, $"{fileName}.csv");

                // ファイルがあれば削除する
                if (!Directory.Exists(dir) || !File.Exists(filePath))
                {
                    return rc;
                }
                else 
                {
                    File.Delete(filePath);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 仕分実績データ 削除 バックアップファイル
        /// 店別小仕分け実績 1行分
        /// </summary>
        public UInt32 DeleteExecuteDataBackup(ExecuteStoreData store)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 日付
                DateTime dt = DateTime.Now;
                // 日付切り替わり処理
                dt = DateConvert();


                // 商品名取得
                int postNo = store.postNo;
                string janCode = "";
                string workName = "";
                OrderData d = OrderDataList[postNo - 1].Where(x => x.workCode == store.workCode).FirstOrDefault();
                if (d != null)
                {
                    janCode = d.JANCode;
                    workName = (d.workName).Trim();
                }

                // パス作成
                string executeStoreInfo_TableName = $"{_storeInfoExecute_TableName}{dt.Year.ToString("D4")}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";
                string dir = $"{IniFile.DBBackup}\\{executeStoreInfo_TableName}";
                string fileName = $"{store.orderDate.ToString("yyyyMMdd")}_{store.postNo}_{store.workCode}_{janCode}_{store.storeCode}_{store.index}_{workName}";
                string filePath = System.IO.Path.Combine(dir, $"{fileName}.csv");

                // ファイルがあれば削除する
                if (!Directory.Exists(dir) || !File.Exists(filePath))
                {
                    return rc;
                }
                else
                {
                    File.Delete(filePath);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 仕分実績データ DBに反映 バックアップファイル
        /// </summary>
        public UInt32 TransferExecuteDataBackup()
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 日付
                DateTime dt = DateTime.Now;
                // 日付切り替わり処理
                dt = DateConvert();


                // --------------------------------------------
                // 商品ヘッダ実績
                // --------------------------------------------
                // パス作成
                string executeWorkHeader_TableName = $"{_workHeaderExecute_TableName}{dt.Year.ToString("D4")}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";
                string executeWorkHeader_Dir = $"{IniFile.DBBackup}\\{executeWorkHeader_TableName}";

                if (Directory.Exists(executeWorkHeader_Dir))
                {
                    List<ExecuteData> works = new List<ExecuteData>();

                    // 全ファイル名を取得
                    string[] files = System.IO.Directory.GetFiles(executeWorkHeader_Dir, "*.csv");
                    for (int i = 0; i < files.Length; i++)
                    {
                        // 全ファイル読み出し
                        using (StreamReader reader = new StreamReader(files[i], Encoding.GetEncoding("shift_jis")))
                        {
                            while (!reader.EndOfStream)
                            {
                                // CSVファイルの一行を読み込む
                                string line = reader.ReadLine();
                                // 読み込んだ一行をカンマ毎に分けて配列に格納する
                                string[] items = line.Split(',');

                                ExecuteData work = new ExecuteData()
                                {
                                    orderDate = DateTime.ParseExact(items[0].ToString(), "yyyyMMdd", null),
                                    postNo = int.Parse(items[1]),
                                    orderDateRequest = DateTime.ParseExact(items[2].ToString(), "yyyyMMdd", null),
                                    postNoRequest = int.Parse(items[3]),
                                    workCode = items[4],
                                    index = int.Parse(items[5]),
                                    JANCode = items[6],
                                    orderCountTotal = int.Parse(items[7]),
                                    orderCompCountTotal = int.Parse(items[8]),
                                    loadDateTime = Convert.ToString(items[9]),
                                    createDateTime = DateTime.ParseExact($"{items[10]}{items[11]}", "yyyyMMddHHssmm", null),
                                    createLoginId = items[12],
                                    updateDateTime = DateTime.ParseExact($"{items[13]}{items[14]}", "yyyyMMddHHssmm", null),
                                    updateLoginId = items[15],
                                };
                                // リストに格納
                                works.Add(work);
                            }
                        }
                    }

                    // DBへ反映
                    foreach (ExecuteData work in works)
                    {
                        // 1商品読み出し
                        rc = OrderData_IF.GetExecuteItem(executeWorkHeader_TableName, work.orderDate, work.postNo, work.workCode, out ExecuteData dbWork);

                        DateTime d = DateTime.MinValue;
                        if (dbWork != null)
                            d = dbWork.updateDateTime;

                        if (work.updateDateTime > d)
                        {// バックアップファイルの情報の方が新しければ、DBを更新する

                            // 商品ヘッダ実績テーブル書込み 1行
                            rc = OrderData_IF.SetExecuteItem(executeWorkHeader_TableName, work, IniFile.DBLoginId);
                            if (STATUS_SUCCESS(rc))
                            {
                                // バックアップファイル削除
                                DeleteExecuteDataBackup(work);
                            }
                        }
                    }

                }


                // --------------------------------------------
                // 店別小仕分け実績
                // --------------------------------------------
                // パス作成
                string executeStoreInfo_TableName = $"{_storeInfoExecute_TableName}{dt.Year.ToString("D4")}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";
                string executeStoreInfo_Dir = $"{IniFile.DBBackup}\\{executeStoreInfo_TableName}";
                
                if (Directory.Exists(executeStoreInfo_Dir))
                {
                    List<ExecuteStoreData> stores = new List<ExecuteStoreData>();

                    // 全ファイル名を取得
                    string[] files = System.IO.Directory.GetFiles(executeStoreInfo_Dir, "*.csv");
                    if (files.Length > 0) 
                    {
                        // 全ファイル読み出し
                        for (int i = 0; i < files.Length; i++)
                        {
                            using (StreamReader reader = new StreamReader(files[i], Encoding.GetEncoding("shift_jis")))
                            {
                                while (!reader.EndOfStream)
                                {
                                    // CSVファイルの一行を読み込む
                                    string line = reader.ReadLine();
                                    // 読み込んだ一行をカンマ毎に分けて配列に格納する
                                    string[] items = line.Split(',');

                                    ExecuteStoreData store = new ExecuteStoreData()
                                    {
                                        orderDate = DateTime.ParseExact(items[0].ToString(), "yyyyMMdd", null),
                                        postNo = int.Parse(items[1]),
                                        orderDateRequest = DateTime.ParseExact(items[2].ToString(), "yyyyMMdd", null),
                                        postNoRequest = int.Parse(items[3]),
                                        workCode = items[4],
                                        index = int.Parse(items[5]),
                                        storeCode = items[6],
                                        stationNo = int.Parse(items[7]),
                                        aisleNo = int.Parse(items[8]),
                                        slotNo = int.Parse(items[9]),
                                        orderCount = int.Parse(items[10]),
                                        orderCompCount = int.Parse(items[11]),
                                        createDateTime = DateTime.ParseExact($"{items[12]}{items[13]}", "yyyyMMddHHssmm", null),
                                        createLoginId = items[14],
                                        updateDateTime = DateTime.ParseExact($"{items[15]}{items[16]}", "yyyyMMddHHssmm", null),
                                        updateLoginId = items[17],
                                    };
                                    // リストに格納
                                    stores.Add(store);
                                }
                            }
                        }

                        // DBへ反映
                        foreach (ExecuteStoreData store in stores)
                        {
                            // 1商品読み出し
                            rc = OrderData_IF.GetExecStoreItems(executeStoreInfo_TableName, store.orderDate, store.postNo, store.workCode, store.storeCode, out ExecuteStoreData dbStore);

                            DateTime d = DateTime.MinValue;
                            if (dbStore != null)
                                d = dbStore.updateDateTime;

                            if (store.updateDateTime > d)
                            {// バックアップファイルの情報の方が新しければ、DBを更新する

                                // 商品ヘッダ実績テーブル書込み 1行
                                rc = OrderData_IF.SetExecStoreItems(executeStoreInfo_TableName, store, IniFile.DBLoginId);
                                if (STATUS_SUCCESS(rc)) 
                                {
                                    // バックアップファイル削除
                                    DeleteExecuteDataBackup(store);
                                }
                            }
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }


        /// <summary>
        /// 日付切り替わり処理
        /// </summary>
        /// <returns></returns>
        private DateTime DateConvert() 
        {
            DateTime dt = DateTime.Now;
            try
            {
                // 日付切り替わり処理
                if (dt < DateTime.Today.AddHours(IniFile.DateChangeTime.Hour).AddMinutes(IniFile.DateChangeTime.Minute))
                    dt = dt.AddDays(-1);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                dt = DateTime.Now;
            }
            return dt;
        }

        #endregion



        #region データリスト操作
        /// <summary>
        /// 仕分データリスト クリア
        /// </summary>
        public UInt32 ClearOrderDataList(int postIndex)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                OrderDataList[postIndex].Clear();
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 仕分実績データリスト クリア
        /// </summary>
        public UInt32 ClearExecuteDataList(int postIndex)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                ExecuteDataList[postIndex].Clear();
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 仕分データのマテハン変換
        /// アイルNo及びSlotNoを、バッチ情報をもとにマテハン向けに変換する
        /// </summary>
        public UInt32 ConvertOrderDataList(int postIndex, int aisleIndex, int batchIndex, string[] storeCode)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {

                foreach (OrderData work in OrderDataList[postIndex]) 
                {
                    for (int slotIndex = 0; slotIndex < storeCode.Length; slotIndex++) 
                    {
                        if (storeCode[slotIndex] == null)
                            continue;

                        OrderStoreData store = work.storeDataList.Where(x => x.storeCode == storeCode[slotIndex]).FirstOrDefault();
                        if (store != null) 
                        {
                            // マテハン向けのアイルNo、SlotNoをセット
                            store.batchNo_MH = batchIndex + 1;
                            store.aisleNo_MH = aisleIndex + 1;
                            store.slotNo_MH = slotIndex + 1;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }


        /// <summary>
        /// 仕分データから1商品を抽出
        /// </summary>
        public UInt32 SelectOrderData(DateTime orderDate, int postIndex, string janCode, out OrderData item)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            item = null;
            try
            {
                item = OrderDataList[postIndex].Where(x => x.orderDate == orderDate
                                             && x.postNo == postIndex + 1
                                             && x.JANCode == janCode).FirstOrDefault();
                                             
                //item = OrderDataList[postIndex].Where(x => x.orderDate == orderDate
                //                             && x.postNo == postIndex + 1
                //                             && x.workCode == workCode).FirstOrDefault()
                //                             .Copy();
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 仕分データから1商品の店別小仕分けデータを抽出
        /// </summary>
        public UInt32 SelectOrderStoreData(DateTime orderDate, int postIndex, string janCode, int stationIndex, int aisleIndex, out List<OrderStoreData> storeDataList)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            storeDataList = null;
            try
            {
                rc = SelectOrderData(orderDate, postIndex, janCode, out OrderData data);

                if (STATUS_SUCCESS(rc))
                {
                    if (data != null)
                    {
                        storeDataList = data.storeDataList.Where(x => x.orderDate == orderDate
                                                                && x.postNo == postIndex + 1
                                                                && x.stationNo == stationIndex + 1
                                                                && x.aisleNo == aisleIndex + 1).ToList();
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 仕分データから1商品の店別小仕分けデータを抽出
        /// 現在バッチ情報の店コードのみ
        /// </summary>
        public UInt32 SelectOrderStoreData(DateTime orderDate, int postIndex, string janCode, string[] batchInfo, out List<OrderStoreData> storeDataList)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            storeDataList = null;
            try
            {
                // 商品ヘッダデータ取得
                rc = SelectOrderData(orderDate, postIndex, janCode, out OrderData data);

                if (STATUS_SUCCESS(rc)) 
                {
                    if (data != null) 
                    {
                        storeDataList = new List<OrderStoreData>();

                        // バッチ情報の店コードのみの店別小仕分けデータを取得
                        // スロット順
                        foreach (string StoreCode in batchInfo) 
                        {
                            // アイルNoやステーションNoはバッチ情報で既に絞られているので、ソートする必要無し
                            List<OrderStoreData> storeData = data.storeDataList.Where(x => x.storeCode == StoreCode).ToList();
                            //List<OrderStoreData> storeData = data.storeDataList.Where(x => x.orderDate == orderDate
                            //                                                            && x.postNo == postIndex + 1
                            //                                                            && x.workCode == workCode
                            //                                                            && x.stationNo == stationIndex + 1
                            //                                                            && x.aisleNo == aisleIndex + 1
                            //                                                            && x.storeCode == StoreCode).ToList();

                            foreach (OrderStoreData sData in storeData)
                                storeDataList.Add(sData);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }

        /// <summary>
        /// 仕分実績データから1商品を抽出
        /// </summary>
        public UInt32 SelectExecuteData(DateTime orderDate, int postIndex, string janCode, out ExecuteData executeData)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            executeData = null;
            try
            {
                executeData = ExecuteDataList[postIndex].Where(x => x.orderDate == orderDate.Date
                                             && x.postNo == postIndex + 1
                                             && x.JANCode == janCode).FirstOrDefault();

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 仕分実績データから商品リストを抽出
        /// </summary>
        public UInt32 SelectExecuteData(DateTime orderDate, int postIndex, ORDER_OR_COMP orderOrComp, string janCode, out List<ExecuteData> executeDataList)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            executeDataList = new List<ExecuteData>();
            try
            {
                var list = ExecuteDataList[postIndex].Where(x => x.orderDate == orderDate).ToList();
                foreach (ExecuteData executeData in list) 
                {
                    ExecuteData data = new ExecuteData();
                    data = executeData.Copy();
                    executeDataList.Add(data);
                }

                //if (stationIndex != -1) 
                //{
                //    executeDataList = executeDataList.Where(x => x.storeDataList.Any(storeData => storeData.stationNo == stationIndex + 1)).ToList();
                //}
                //if (aisleIndex != -1)
                //{
                //    executeDataList = executeDataList.Where(x => x.storeDataList.Any(storeData => storeData.aisleNo == aisleIndex + 1)).ToList();
                //}
                if (orderOrComp != ORDER_OR_COMP.ALL)
                {
                    if (orderOrComp == ORDER_OR_COMP.NOT_COMP)
                        // 仕分け未完了のやつだけ
                        executeDataList = executeDataList.Where(x => x.orderCountTotal != x.orderCompCountTotal).ToList();
                    else
                        // 仕分け完了のやつだけ
                        executeDataList = executeDataList.Where(x => x.orderCountTotal == x.orderCompCountTotal).ToList();
                }
                if (janCode != "")
                {
                    executeDataList = executeDataList.Where(x => x.JANCode == janCode).ToList();
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 仕分実績データから1商品の店別小仕分け実績データを抽出
        /// 現在バッチ情報の店コードのみ
        /// </summary>
        public UInt32 SelectExecuteStoreData(DateTime orderDate, int postIndex, string janCode, string[] batchInfo, out List<ExecuteStoreData> storeDataList)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            storeDataList = null;
            try
            {
                // 仕分実績データ取得
                rc = SelectExecuteData(orderDate, postIndex, janCode, out ExecuteData data);

                if (STATUS_SUCCESS(rc))
                {
                    if (data != null)
                    {
                        storeDataList = new List<ExecuteStoreData>();

                        // バッチ情報の店コードのみの店別小仕分け実績データを取得
                        // スロット順
                        foreach (string StoreCode in batchInfo)
                        {
                            // アイルNoやステーションNoはバッチ情報で既に絞られているので、ソートする必要無し
                            List<ExecuteStoreData> storeData = data.storeDataList.Where(x => x.storeCode == StoreCode).ToList();
                            //List<OrderStoreData> storeData = data.storeDataList.Where(x => x.orderDate == orderDate
                            //                                                            && x.postNo == postIndex + 1
                            //                                                            && x.workCode == workCode
                            //                                                            && x.stationNo == stationIndex + 1
                            //                                                            && x.aisleNo == aisleIndex + 1
                            //                                                            && x.storeCode == StoreCode).ToList();

                            foreach (ExecuteStoreData sData in storeData)
                                storeDataList.Add(sData);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }

        /// <summary>
        /// 仕分実績データリストを作成/更新
        /// 仕分データリストを元に空の仕分実績データリストを作成してしまう
        /// </summary>
        public UInt32 CreateExecuteDataList(int postIndex)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {

                if (OrderDataList[postIndex].Count > 0) 
                {
                    if(ExecuteDataList[postIndex] == null)
                        ExecuteDataList[postIndex] = new List<ExecuteData>();

                    // 仕分データリストの商品分まわす
                    foreach (OrderData orderData in OrderDataList[postIndex]) 
                    {
                        // ------------------------------
                        // 商品ヘッダ実績
                        // ------------------------------
                        // 該当する商品データが仕分実績データリストに既にあるか
                        ExecuteData existData = ExecuteDataList[postIndex].Find(n => n.workCode == orderData.workCode);
                        if (existData == null)
                        {// 商品を追加
                            // 仕分実績データリスtに商品データを追加
                            ExecuteDataList[postIndex].Add(new ExecuteData
                            {
                                orderDate = orderData.orderDate,
                                postNo = orderData.postNo,
                                orderDateRequest = orderData.orderDateRequest,
                                postNoRequest = orderData.postNoRequest,
                                workCode = orderData.workCode,
                                index = orderData.index,
                                JANCode = orderData.JANCode,
                                orderCountTotal = orderData.orderCountTotal,
                                orderCompCountTotal = 0,
                                isChanged = false,
                                //loadDateTime = DateTime.MinValue,
                                //createDateTime = DateTime.MinValue,
                                //createLoginId = "",
                                //updateDateTime = DateTime.MinValue,
                                //updateLoginId = "",
                            });
                        }
                        else 
                        {// 更新
                            // 仕分合計数を更新
                            if (existData.orderCountTotal != orderData.orderCountTotal) 
                            {
                                existData.orderCountTotal = orderData.orderCountTotal;
                                existData.isChanged = true;
                            }
                            // 連番を更新
                            if (existData.index != orderData.index)
                            {
                                existData.index = orderData.index;
                                existData.isChanged = true;
                            }
                        }


                        // ------------------------------
                        // 店別小仕分け実績
                        // ------------------------------
                        existData = ExecuteDataList[postIndex].Find(n => n.workCode == orderData.workCode);
                        foreach (OrderStoreData orderStore in orderData.storeDataList) 
                        {
                            int a = 0;
                            if (orderStore.aisleNo_MH != 0)
                                a = 1;
                            ExecuteStoreData executeData = existData.storeDataList.Find(n => n.storeCode == orderStore.storeCode);
                            if (executeData == null)
                            {// 追加
                                // 仕分実績データリスtに店データを追加
                                existData.storeDataList.Add(new ExecuteStoreData
                                {
                                    orderDate = orderStore.orderDate,
                                    postNo = orderStore.postNo,
                                    orderDateRequest = orderStore.orderDateRequest,
                                    postNoRequest = orderStore.postNoRequest,
                                    workCode = orderStore.workCode,
                                    index = orderStore.index,
                                    storeCode = orderStore.storeCode,
                                    stationNo = orderStore.stationNo,
                                    aisleNo = orderStore.aisleNo,
                                    slotNo = orderStore.slotNo,
                                    orderCount = orderStore.orderCount,
                                    orderCompCount = 0,
                                    batchNo_MH = orderStore.batchNo_MH,
                                    aisleNo_MH = orderStore.aisleNo_MH,
                                    slotNo_MH = orderStore.slotNo_MH,

                                    isChanged = false,
                                    //createDateTime = DateTime.MinValue,
                                    //createLoginId = "",
                                    //updateDateTime = DateTime.MinValue,
                                    //updateLoginId = "",
                                });
                            }
                            else
                            {// 更新
                                executeData.batchNo_MH = orderStore.batchNo_MH;
                                executeData.aisleNo_MH = orderStore.aisleNo_MH;
                                executeData.slotNo_MH = orderStore.slotNo_MH;

                                // 仕分合計数を更新
                                if (existData.orderCountTotal != orderData.orderCountTotal)
                                {
                                    existData.orderCountTotal = orderData.orderCountTotal;
                                    existData.isChanged = true;
                                }
                                // 連番を更新
                                if (existData.index != orderData.index)
                                {
                                    existData.index = orderData.index;
                                    existData.isChanged = true;
                                }
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 仕分実績データリストの仕分完了数を更新
        /// 現在バッチ情報の店コードのみ
        /// </summary>
        public UInt32 UpdateExecuteDataOrderCount(int postIndex, string janCode, int[] compCount, string[] storeCodeList)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                if (ExecuteDataList[postIndex].Count > 0) 
                {
                    ExecuteData work = ExecuteDataList[postIndex].Find(n => n.JANCode == janCode);
                    if (work != null) 
                    {
                        // 店別の仕分済数量
                        for (int i = 0; i < storeCodeList.Length; i++)
                        {
                            ExecuteStoreData store = work.storeDataList.Find(n => n.storeCode == storeCodeList[i]);
                            if (store != null)
                            {
                                if(!store.isChanged)
                                    store.isChanged = (store.orderCompCount != compCount[i]);
                                store.orderCompCount = compCount[i];
                                //if(store.isChanged)
                                //    Logger.WriteLog(LogType.INFO, $"！！！！更新！！！！  {store.storeCode} : {store.orderCompCount}");
                            }
                        }


                        // 総仕分済数量   ※ここでtrue=>falseと書き換えるのはNG。データベースに書き込まれなくなる
                        if(!work.isChanged)
                            work.isChanged = (work.orderCompCountTotal != work.storeDataList.Select(x => x.orderCompCount).Sum());
                        work.orderCompCountTotal = work.storeDataList.Select(x => x.orderCompCount).Sum();
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }

        #endregion



        #region ※デバッグ
        /// <summary>
        /// 仕分データリスト作成
        /// ※デバッグ
        /// </summary>
        public UInt32 Debug_CreateData()
        {
            UInt32 rc = 0;
            try
            {
                // 便
                for (int i = 0; i < Const.MaxPostCount; i++) 
                {
                    // 10商品をリストに登録
                    for (int j = 0; j < 10; j++)
                    {
                        string WorkCode = $"{j + 1}00000";
                        int PostNo = (i + 1);

                        OrderData data = new OrderData();
                        string yyyy = (DateTime.Now).Year.ToString();
                        string MM = "0" + (DateTime.Now).Month.ToString();
                        string dd = (DateTime.Now).Day.ToString();
                        string aaaa = $"{yyyy}{MM}{dd}000000";
                        data.orderDate = DateTime.ParseExact(aaaa, "yyyyMMddHHmmss", null);
                        data.workCode = WorkCode;
                        data.postNo = PostNo;
                        data.orderCountTotal = 100;
                        for (int l = 0; l < Const.MaxAisleCount; l++)
                        {
                            for (int k = 0; k < 200; k++)
                            {
                                OrderStoreData store = new OrderStoreData();
                                store.orderDate = DateTime.ParseExact(aaaa, "yyyyMMddHHmmss", null);
                                store.workCode = WorkCode;
                                store.postNo = PostNo;
                                store.stationNo = 1;
                                store.aisleNo = l + 1;
                                //store.slotNo = k + 1;
                                store.storeCode = $"{k + 1}";

                                data.storeDataList.Add(store);
                            }
                        }
                        
                        OrderDataList[i].Add(data);
                    }

                }



            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        #endregion



        /// <summary>
        /// Check Error State
        /// </summary>
        private static bool STATUS_SUCCESS(UInt32 err) { return err == (int)ErrorCodeList.STATUS_SUCCESS; }

    }

}
