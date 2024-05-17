//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using DL_Logger;
using SystemConfig;
using ErrorCodeDefine;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;


namespace ServerModule
{

    // ★★★下記クラス内の各関数の処理を作成ください。


    /// <summary>
    /// 仕分データインターフェース DB
    /// </summary>
    public class OrderInfo_IF_DB : IOrderInfo
    {
        private const string THIS_NAME = "OrderInfo_IF_DB";


        #region 列名構造体
        #region 仕分データ商品ヘッダ列名
        /// <summary>
        /// 仕分データ商品ヘッダ列名
        /// </summary>
        private struct ColumnNamePickHead
        {
            /// <summary>
            /// 仕分納品日
            /// </summary>
            public const string ORDER_DATE = "delivery_date";
            /// <summary>
            /// 仕分便No
            /// </summary>
            public const string POST_NO = "post_no";
            /// <summary>
            /// 発注納品日
            /// </summary>
            public const string ORDER_DATE_REQUEST = "delivery_date_order";
            /// <summary>
            /// 発注便No
            /// </summary>
            public const string POST_NO_REQUEST = "post_no_order";
            /// <summary>
            /// 商品コード
            /// </summary>
            public const string WORK_CODE = "sku_code";
            /// <summary>
            /// 連番
            /// </summary>
            public const string INDEX = "pd_count";
            /// <summary>
            /// 商品名(漢字)
            /// </summary>
            public const string WORK_NAME = "sku_name";
            /// <summary>
            /// JANコード
            /// </summary>
            public const string JAN_CODE = "jan_code";
            /// <summary>
            /// ケース入数
            /// </summary>
            public const string CASE_VOLUME = "case_volume";
            /// <summary>
            /// 仕分け数合計
            /// </summary>
            public const string ORDER_COUNT_TOTAL = "pieces_num_total";
            /// <summary>
            /// 商品名(カナ)
            /// </summary>
            public const string WORK_NAME_KANA = "sku_kana";
            /// <summary>
            /// MAX積み付け段数
            /// </summary>
            public const string MAX_STACK_NUM = "max_stack_num";
            /// <summary>
            /// 売価１
            /// </summary>
            public const string SALES_PRICE = "sales_price";
            /// <summary>
            /// 仕分け作業状況
            /// </summary>
            public const string PROCESS = "pick_class";
            /// <summary>
            /// 仕分け数 (ステーションごと)
            /// </summary>
            public const string ORDER_COUNT = "pieces_num_st";
            /// <summary>
            /// 店舗数 (ステーションごと)
            /// </summary>
            public const string STORE_COUNT = "store_num_st";
            /// <summary>
            /// 登録日付
            /// </summary>
            public const string CREATE_DATE = "create_date";
            /// <summary>
            /// 登録時刻
            /// </summary>
            public const string CREATE_TIME = "create_time";
            /// <summary>
            /// 登録ログインID
            /// </summary>
            public const string CREATE_LOGIN_ID = "create_login_id";
            /// <summary>
            /// 更新日付
            /// </summary>
            public const string UPDATE_DATE = "renew_date";
            /// <summary>
            /// 更新時刻
            /// </summary>
            public const string UPDATE_TIME = "renew_time";
            /// <summary>
            /// 更新ログインID
            /// </summary>
            public const string UPDATE_LOGIN_ID = "renew_login_id";
        }
        #endregion

        #region 仕分データ店別小仕分列名
        /// <summary>
        /// 仕分データ店別小仕分列名
        /// </summary>
        private struct ColumnNamePickDetail
        {
            /// <summary>
            /// 仕分納品日
            /// </summary>
            public const string ORDER_DATE = "delivery_date";
            /// <summary>
            /// 仕分便No
            /// </summary>
            public const string POST_NO = "post_no";
            /// <summary>
            /// 発注納品日
            /// </summary>
            public const string ORDER_DATE_REQUEST = "delivery_date_order";
            /// <summary>
            /// 発注便No
            /// </summary>
            public const string POST_NO_REQUEST = "post_no_order";
            /// <summary>
            /// 商品コード
            /// </summary>
            public const string WORK_CODE = "sku_code";
            /// <summary>
            /// 連番
            /// </summary>
            public const string INDEX = "pd_count";
            /// <summary>
            /// 店コード
            /// </summary>
            public const string STORE_CODE = "store_code";
            /// <summary>
            /// ステーションNo
            /// </summary>
            public const string STATION_NO = "station_no";
            /// <summary>
            /// アイルNo
            /// </summary>
            public const string AISLE_NO = "aisle_no";
            /// <summary>
            /// スロットNo
            /// </summary>
            public const string SLOT_NO = "slot_no";
            /// <summary>
            /// ケース入数
            /// </summary>
            public const string CASE_VOLUME = "case_volume";
            /// <summary>
            /// 仕分け数
            /// </summary>
            public const string ORDER_COUNT = "pieces_num";
            /// <summary>
            /// 登録日付
            /// </summary>
            public const string CREATE_DATE = "create_date";
            /// <summary>
            /// 登録時刻
            /// </summary>
            public const string CREATE_TIME = "create_time";
            /// <summary>
            /// 登録ログインID
            /// </summary>
            public const string CREATE_LOGIN_ID = "create_login_id";
            /// <summary>
            /// 更新日付
            /// </summary>
            public const string UPDATE_DATE = "renew_date";
            /// <summary>
            /// 更新時刻
            /// </summary>
            public const string UPDATE_TIME = "renew_time";
            /// <summary>
            /// 更新ログインID
            /// </summary>
            public const string UPDATE_LOGIN_ID = "renew_login_id";
        }
        #endregion

        #region 仕分完了データ商品ヘッダ列名
        /// <summary>
        /// 仕分完了データ商品ヘッダ列名
        /// </summary>
        private struct ColumnNameCompHead
        {
            /// <summary>
            /// 仕分納品日
            /// </summary>
            public const string ORDER_DATE = "delivery_date";
            /// <summary>
            /// 仕分便No
            /// </summary>
            public const string POST_NO = "post_no";
            /// <summary>
            /// 発注納品日
            /// </summary>
            public const string ORDER_DATE_REQUEST = "delivery_date_order";
            /// <summary>
            /// 発注便No
            /// </summary>
            public const string POST_NO_REQUEST = "post_no_order";
            /// <summary>
            /// 商品コード
            /// </summary>
            public const string WORK_CODE = "sku_code";
            /// <summary>
            /// 連番
            /// </summary>
            public const string INDEX = "pd_count";
            /// <summary>
            /// JANコード
            /// </summary>
            public const string JAN_CODE = "jan_code";
            /// <summary>
            /// 仕分け数合計
            /// </summary>
            public const string ORDER_COUNT_TOTAL = "pieces_num_total";
            /// <summary>
            /// 仕分け完了数合計
            /// </summary>
            public const string ORDER_COMP_COUNT_TOTAL = "comp_num_total";
            /// <summary>
            /// 取込日時
            /// </summary>
            public const string LOAD_DATE_TIME = "slip_date";
            /// <summary>
            /// 登録日付
            /// </summary>
            public const string CREATE_DATE = "create_date";
            /// <summary>
            /// 登録時刻
            /// </summary>
            public const string CREATE_TIME = "create_time";
            /// <summary>
            /// 登録ログインID
            /// </summary>
            public const string CREATE_LOGIN_ID = "create_login_id";
            /// <summary>
            /// 更新日付
            /// </summary>
            public const string UPDATE_DATE = "renew_date";
            /// <summary>
            /// 更新時刻
            /// </summary>
            public const string UPDATE_TIME = "renew_time";
            /// <summary>
            /// 更新ログインID
            /// </summary>
            public const string UPDATE_LOGIN_ID = "renew_login_id";
        }
        #endregion

        #region 仕分データ店別小仕分列名
        /// <summary>
        /// 仕分データ店別小仕分列名
        /// </summary>
        private struct ColumnNameCompDetail
        {
            /// <summary>
            /// 仕分納品日
            /// </summary>
            public const string ORDER_DATE = "delivery_date";
            /// <summary>
            /// 仕分便No
            /// </summary>
            public const string POST_NO = "post_no";
            /// <summary>
            /// 発注納品日
            /// </summary>
            public const string ORDER_DATE_REQUEST = "delivery_date_order";
            /// <summary>
            /// 発注便No
            /// </summary>
            public const string POST_NO_REQUEST = "post_no_order";
            /// <summary>
            /// 商品コード
            /// </summary>
            public const string WORK_CODE = "sku_code";
            /// <summary>
            /// 連番
            /// </summary>
            public const string INDEX = "pd_count";
            /// <summary>
            /// 店コード
            /// </summary>
            public const string STORE_CODE = "store_code";
            /// <summary>
            /// ステーションNo
            /// </summary>
            public const string STATION_NO = "station_no";
            /// <summary>
            /// アイルNo
            /// </summary>
            public const string AISLE_NO = "aisle_no";
            /// <summary>
            /// スロットNo
            /// </summary>
            public const string SLOT_NO = "slot_no";
            /// <summary>
            /// 仕分け数
            /// </summary>
            public const string ORDER_COUNT = "pieces_num";
            /// <summary>
            /// 仕分け完了数
            /// </summary>
            public const string ORDER_COMP_COUNT = "comp_num";
            /// <summary>
            /// 登録日付
            /// </summary>
            public const string CREATE_DATE = "create_date";
            /// <summary>
            /// 登録時刻
            /// </summary>
            public const string CREATE_TIME = "create_time";
            /// <summary>
            /// 登録ログインID
            /// </summary>
            public const string CREATE_LOGIN_ID = "create_login_id";
            /// <summary>
            /// 更新日付
            /// </summary>
            public const string UPDATE_DATE = "renew_date";
            /// <summary>
            /// 更新時刻
            /// </summary>
            public const string UPDATE_TIME = "renew_time";
            /// <summary>
            /// 更新ログインID
            /// </summary>
            public const string UPDATE_LOGIN_ID = "renew_login_id";
        }
        #endregion
        #endregion

        #region 定数
        /// <summary>
        /// 日付フォーマット
        /// </summary>
        private const string DateFormat = "yyyyMMdd";
        /// <summary>
        /// 時刻フォーマット
        /// </summary>
        private const string TimeFormat = "HHmmss";
        /// <summary>
        /// 接続文字列
        /// </summary>
        private readonly string connectionString = IniFile.DB_SQL_Connection;
        /// <summary>
        /// リトライ回数
        /// </summary>
        private readonly int retryCount = DBAccess.DEFAULT_RETRY_CCOUNT;
        #endregion

        #region 日時変換
        /// <summary>
        /// 日時変換
        /// </summary>
        /// <param name="date">日付オブジェクト</param>
        /// <param name="time">時刻オブジェクト</param>
        /// <returns>日時</returns>
        private DateTime ConvertDateTime(object date, object time)
        {
            var _date = date.ToString() ?? DateTime.MinValue.ToString(DateFormat);
            if (_date == "" || _date == "0")
                _date = DateTime.MinValue.ToString(DateFormat);
            var _time = time.ToString() ?? DateTime.MinValue.ToString(TimeFormat);
            if (_time == "" || _time == "0")
                _time = DateTime.MinValue.ToString(TimeFormat);

            return DateTime.ParseExact($"{_date}{_time}", $"{DateFormat}{TimeFormat}", null);
        }
        #endregion


        #region 商品ヘッダテーブル
        /// <summary>
        /// 商品ヘッダテーブル
        /// 1便読み出し
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="orderDateTime">仕分納品日</param>
        /// <param name="postNo">仕分便No</param>
        /// <param name="process">仕分け作業状況</param>
        /// <param name="itemList">1便分のデータリスト</param>
        /// <returns>エラーコード</returns>
        public UInt32 GetOrderItemList(string tableName, DateTime orderDateTime, int postNo, ORDER_PROCESS process, out List<OrderData> itemList) 
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"OrderInfo_IF_DB GetOrderItemList({tableName}, {orderDateTime}, {postNo}, {process})");
            itemList = null;
            try
            {
                var sql = new StringBuilder();
                sql.Append(GetOrderItemSelectSql(tableName, orderDateTime, postNo, process));
                sql.Append($"AND ");
                sql.Append(GetOrderItemMaxSql(tableName, process));
                sql.Append(GetOrderItemOrderSql(tableName));
                using (var dbAccess = new DBAccess())
                {
                    rc = dbAccess.ExecuteReader(connectionString, sql.ToString(), out var dataList, retryCount);
                    if (rc == (UInt32)ErrorCodeList.STATUS_SUCCESS)
                    {
                        if (dataList != null && dataList.Count > 0)
                        {
                            itemList = new List<OrderData>();
                            foreach (var data in dataList)
                            {
                                DataToOrderItem(data, out var orderData);
                                itemList.Add(orderData);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                itemList = null;
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"OrderInfo_IF_DB GetOrderItemList : {(ErrorCodeList)rc}");
            return rc;
        }

        /// <summary>
        /// 商品ヘッダテーブル
        /// 1行分読み出し
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="orderDateTime">仕分納品日</param>
        /// <param name="postNo">仕分便No</param>
        /// <param name="workCode">商品コード</param>
        /// <param name="process">仕分け作業状況</param>
        /// <param name="item">1行データ</param>
        /// <returns>エラーコード</returns>
        public UInt32 GetOrderItem(string tableName, DateTime orderDateTime, int postNo, string workCode, ORDER_PROCESS process, out OrderData item) 
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"OrderInfo_IF_DB.GetOrderItem({tableName}, {orderDateTime}, {postNo}, {workCode}, {process})");
            item = null;
            try
            {
                var sql = new StringBuilder();
                sql.Append(GetOrderItemSelectSql(tableName, orderDateTime, postNo, process));
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNamePickHead.WORK_CODE} = '{workCode}' ");
                sql.Append($"AND ");
                sql.Append(GetOrderItemMaxSql(tableName, process));
                sql.Append(GetOrderItemOrderSql(tableName));
                using (var dbAccess = new DBAccess())
                {
                    rc = dbAccess.ExecuteReader(connectionString, sql.ToString(), out var dataList, retryCount);
                    if (rc == (UInt32)ErrorCodeList.STATUS_SUCCESS)
                    {
                        if (dataList != null && dataList.Count > 0)
                            DataToOrderItem(dataList[0], out item);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"OrderInfo_IF_DB.GetOrderItem : {(ErrorCodeList)rc}");
            return rc;
        }

        /// <summary>
        /// 商品ヘッダテーブル
        /// 1行分の 仕分作業状況/更新日付/更新時刻/更新ログインID を更新
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="orderDateTime">仕分納品日</param>
        /// <param name="postNo">仕分便No</param>
        /// <param name="orderDateRequest">発注納品日</param>
        /// <param name="postNoRequest">発注便No</param>
        /// <param name="workCode">商品コード</param>
        /// <param name="index">連番</param>
        /// <param name="process">仕分け作業状況</param>
        /// <param name="updateLoginId">更新ログインID</param>
        /// <returns>エラーコード</returns>
        public UInt32 SetOrderItem(string tableName, DateTime orderDateTime, int postNo, DateTime orderDateRequest, int postNoRequest, string workCode, int index, ORDER_PROCESS process, string updateLoginId)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"OrderInfo_IF_DB.SetOrderItem({tableName}, {orderDateTime}, {postNo}, {workCode}, {process}, {updateLoginId})");
            try
            {
                var now = DateTime.Now;

                var sql = new StringBuilder();
                sql.Append($"UPDATE {tableName} ");
                sql.Append($"SET ");
                sql.Append($"{ColumnNamePickHead.PROCESS} = '{(int)process}', ");
                sql.Append($"{ColumnNamePickHead.UPDATE_DATE} = '{now.ToString(DateFormat)}', ");
                sql.Append($"{ColumnNamePickHead.UPDATE_TIME} = '{now.ToString(TimeFormat)}', ");
                sql.Append($"{ColumnNamePickHead.UPDATE_LOGIN_ID} = '{updateLoginId}' ");
                sql.Append($"WHERE ");
                sql.Append($"{ColumnNamePickHead.ORDER_DATE} = '{orderDateTime.ToString(DateFormat)}' ");
                sql.Append($"AND ");
                sql.Append($"{ColumnNamePickHead.POST_NO} = '{postNo}' ");
                sql.Append($"AND ");
                sql.Append($"{ColumnNamePickHead.ORDER_DATE_REQUEST} = '{orderDateRequest.ToString(DateFormat)}' ");
                //sql.Append($"{ColumnNamePickHead.ORDER_DATE_REQUEST} = '{orderDateRequest.AddDays(1).ToString(DateFormat)}' ");
                sql.Append($"AND ");
                sql.Append($"{ColumnNamePickHead.POST_NO_REQUEST} = '{postNoRequest}' ");
                sql.Append($"AND ");
                sql.Append($"{ColumnNamePickHead.WORK_CODE} = '{workCode}' ");
                sql.Append($"AND ");
                sql.Append($"{ColumnNamePickHead.INDEX} = {index} ");
                using (var dbAccess = new DBAccess())
                {
                    rc = dbAccess.ExecuteNonSQL(connectionString, sql.ToString(), retryCount);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"OrderInfo_IF_DB.SetOrderItem : {(ErrorCodeList)rc}");
            return rc;
        }

        /// <summary>
        /// 商品ヘッダテーブル
        /// 1行書込み(PICKDATAから)
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="orderDateTime">仕分納品日</param>
        /// <param name="postNo">仕分便No</param>
        /// <param name="orderDateRequest">発注納品日</param>
        /// <param name="postNoRequest">発注便No</param>
        /// <param name="workCode">商品コード</param>
        /// <param name="index">連番</param>
        /// <param name="process">仕分け作業状況</param>
        /// <param name="updateLoginId">更新ログインID</param>
        /// <returns>エラーコード</returns>
        public UInt32 SetOrderItemFromPickData(string tableName, OrderData item)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"OrderInfo_IF_DB.SetOrderItem({tableName}, {orderDateTime}, {postNo}, {workCode}, {process}, {updateLoginId})");
            try
            {
                //var now = DateTime.Now;


                // Postgresqlのバージョンが8.3以前の場合は、SELECT→INSERT or UPDATEの2回実行する必要がある
                // SELECT文
                var selectSql = new StringBuilder();
                selectSql.Append($"SELECT ");
                selectSql.Append($"{ColumnNamePickHead.ORDER_DATE} ");
                selectSql.Append($"FROM ");
                selectSql.Append($"{tableName} ");
                selectSql.Append($"WHERE ");
                selectSql.Append($"{ColumnNamePickHead.ORDER_DATE} = '{item.orderDate.ToString(DateFormat)}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNamePickHead.POST_NO} = '{item.postNo}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNamePickHead.ORDER_DATE_REQUEST} = '{item.orderDateRequest.ToString(DateFormat)}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNamePickHead.POST_NO_REQUEST} = '{item.postNoRequest}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNamePickHead.WORK_CODE} = '{item.workCode}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNamePickHead.INDEX} = {item.index} ");
                // INSERT文
                var insertSql = new StringBuilder();
                insertSql.Append($"INSERT INTO {tableName} ");
                insertSql.Append($"( ");
                insertSql.Append($"{ColumnNamePickHead.ORDER_DATE}, ");
                insertSql.Append($"{ColumnNamePickHead.POST_NO}, ");
                insertSql.Append($"{ColumnNamePickHead.ORDER_DATE_REQUEST}, ");
                insertSql.Append($"{ColumnNamePickHead.POST_NO_REQUEST}, ");
                insertSql.Append($"{ColumnNamePickHead.WORK_CODE}, ");
                insertSql.Append($"{ColumnNamePickHead.INDEX}, ");
                insertSql.Append($"{ColumnNamePickHead.WORK_NAME}, ");
                insertSql.Append($"{ColumnNamePickHead.JAN_CODE}, ");
                insertSql.Append($"{ColumnNamePickHead.CASE_VOLUME}, ");
                insertSql.Append($"{ColumnNamePickHead.ORDER_COUNT_TOTAL}, ");
                insertSql.Append($"{ColumnNamePickHead.WORK_NAME_KANA}, ");
                insertSql.Append($"{ColumnNamePickHead.MAX_STACK_NUM}, ");
                insertSql.Append($"{ColumnNamePickHead.SALES_PRICE}, ");
                insertSql.Append($"{ColumnNamePickHead.PROCESS}, ");
                for (var i = 1; i <= Const.MaxStationCount; i++)
                    insertSql.Append($"{ColumnNamePickHead.ORDER_COUNT}{i}, ");
                for (var i = 1; i <= Const.MaxStationCount; i++)
                    insertSql.Append($"{ColumnNamePickHead.STORE_COUNT}{i}, ");
                insertSql.Append($"{ColumnNamePickHead.CREATE_DATE}, ");
                insertSql.Append($"{ColumnNamePickHead.CREATE_TIME}, ");
                insertSql.Append($"{ColumnNamePickHead.CREATE_LOGIN_ID} ");
                //insertSql.Append($"{ColumnNamePickHead.UPDATE_LOGIN_ID} ");
                insertSql.Append($") VALUES ( ");
                insertSql.Append($"'{item.orderDate.ToString(DateFormat)}', ");
                insertSql.Append($"'{item.postNo}', ");
                insertSql.Append($"'{item.orderDateRequest.ToString(DateFormat)}', ");
                insertSql.Append($"'{item.postNoRequest}', ");
                insertSql.Append($"'{item.workCode}', ");
                insertSql.Append($"{item.index}, ");
                insertSql.Append($"'{item.workName}', ");
                insertSql.Append($"'{item.JANCode}', ");
                insertSql.Append($"{item.caseVolume}, ");
                insertSql.Append($"{item.orderCountTotal}, ");
                insertSql.Append($"'{item.workNameKana}', ");
                insertSql.Append($"'{item.maxStackNum}', ");
                insertSql.Append($"{item.salesPrice}, ");
                insertSql.Append($"'{(int)item.process}', ");
                for (var i = 0; i < Const.MaxStationCount; i++)
                    insertSql.Append($"{item.orderCount[i]}, ");
                for (var i = 0; i < Const.MaxStationCount; i++)
                    insertSql.Append($"{item.storeCount[i]}, ");
                insertSql.Append($"'{item.createDateTime.ToString(DateFormat)}', ");
                insertSql.Append($"'{item.createDateTime.ToString(TimeFormat)}', ");
                insertSql.Append($"'{item.createLoginId}' ");
                //insertSql.Append($"'{item.updateLoginId}' ");
                insertSql.Append($") ");
                // UPDATE文
                var updateSql = new StringBuilder();
                updateSql.Append($"UPDATE {tableName} ");
                updateSql.Append($"SET ");
                updateSql.Append($"{ColumnNamePickHead.ORDER_COUNT_TOTAL} = {item.orderCountTotal}, ");
                for (var i = 0; i < Const.MaxStationCount; i++)
                    updateSql.Append($"a.{ColumnNamePickHead.ORDER_COUNT}{i + 1} = {item.orderCount[i]}, ");
                for (var i = 0; i < Const.MaxStationCount; i++)
                    updateSql.Append($"a.{ColumnNamePickHead.STORE_COUNT}{i + 1} = {item.storeCount[i]}, ");
                updateSql.Append($"{ColumnNamePickHead.UPDATE_DATE} = '{item.updateDateTime.ToString(DateFormat)}', ");
                updateSql.Append($"{ColumnNamePickHead.UPDATE_TIME} = '{item.updateDateTime.ToString(TimeFormat)}', ");
                updateSql.Append($"{ColumnNamePickHead.UPDATE_LOGIN_ID} = '{item.updateLoginId}' ");
                updateSql.Append($"WHERE ");
                updateSql.Append($"{ColumnNamePickHead.ORDER_DATE} = '{item.orderDate.ToString(DateFormat)}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNamePickHead.POST_NO} = '{item.postNo}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNamePickHead.ORDER_DATE_REQUEST} = '{item.orderDateRequest.ToString(DateFormat)}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNamePickHead.POST_NO_REQUEST} = '{item.postNoRequest}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNamePickHead.WORK_CODE} = '{item.workCode}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNamePickHead.INDEX} = {item.index} ");
                using (var dbAccess = new DBAccess())
                {
                    rc = dbAccess.Upsert(connectionString, selectSql.ToString(), insertSql.ToString(), updateSql.ToString(), retryCount);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"OrderInfo_IF_DB.SetOrderItem : {(ErrorCodeList)rc}");
            return rc;
        }


        #region 商品ヘッダSELECT文取得
        /// <summary>
        /// 商品ヘッダSELECT文取得
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="orderDateTime">仕分納品日</param>
        /// <param name="postNo">仕分便No</param>
        /// <param name="process">仕分け作業状況</param>
        /// <returns>SELECT文</returns>
        private string GetOrderItemSelectSql(string tableName, DateTime orderDateTime, int postNo, ORDER_PROCESS process)
        {
            //Logger.WriteLog(LogType.METHOD_IN, $"OrderInfo_IF_DB.GetOrderItemSelectSql({tableName}, {orderDateTime}, {postNo}, {process})");
            var result = new StringBuilder();
            result.Append($"SELECT ");
            result.Append($"a.{ColumnNamePickHead.ORDER_DATE}, ");
            result.Append($"a.{ColumnNamePickHead.POST_NO}, ");
            result.Append($"a.{ColumnNamePickHead.ORDER_DATE_REQUEST}, ");
            result.Append($"a.{ColumnNamePickHead.POST_NO_REQUEST}, ");
            result.Append($"a.{ColumnNamePickHead.WORK_CODE}, ");
            result.Append($"a.{ColumnNamePickHead.INDEX}, ");
            result.Append($"a.{ColumnNamePickHead.WORK_NAME}, ");
            result.Append($"a.{ColumnNamePickHead.JAN_CODE}, ");
            result.Append($"a.{ColumnNamePickHead.CASE_VOLUME}, ");
            result.Append($"a.{ColumnNamePickHead.ORDER_COUNT_TOTAL}, ");
            result.Append($"a.{ColumnNamePickHead.WORK_NAME_KANA}, ");
            result.Append($"a.{ColumnNamePickHead.MAX_STACK_NUM}, ");
            result.Append($"a.{ColumnNamePickHead.SALES_PRICE}, ");
            result.Append($"a.{ColumnNamePickHead.PROCESS}, ");
            for (var i = 1; i <= Const.MaxStationCount; i++)
                result.Append($"a.{ColumnNamePickHead.ORDER_COUNT}{i}, ");
            for (var i = 1; i <= Const.MaxStationCount; i++)
                result.Append($"a.{ColumnNamePickHead.STORE_COUNT}{i}, ");
            result.Append($"a.{ColumnNamePickHead.CREATE_DATE}, ");
            result.Append($"a.{ColumnNamePickHead.CREATE_TIME}, ");
            result.Append($"a.{ColumnNamePickHead.CREATE_LOGIN_ID}, ");
            result.Append($"a.{ColumnNamePickHead.UPDATE_DATE}, ");
            result.Append($"a.{ColumnNamePickHead.UPDATE_TIME}, ");
            result.Append($"a.{ColumnNamePickHead.UPDATE_LOGIN_ID} ");
            result.Append($"FROM ");
            result.Append($"{tableName} a ");
            result.Append($"WHERE ");
            result.Append($"a.{ColumnNamePickHead.ORDER_DATE} = '{orderDateTime.ToString(DateFormat)}' ");
            result.Append($"AND ");
            result.Append($"a.{ColumnNamePickHead.POST_NO} = '{postNo}' ");
            if (process != ORDER_PROCESS.NONE)
            {
                result.Append($"AND ");
                result.Append($"a.{ColumnNamePickHead.PROCESS} = '{(int)process}' ");
            }

            //Logger.WriteLog(LogType.METHOD_OUT, $"OrderInfo_IF_DB.GetOrderItemSelectSql");
            return result.ToString();
        }
        #endregion

        #region 商品ヘッダMAX文取得
        /// <summary>
        /// 商品ヘッダMAX文取得
        /// (最新連番取得用)
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="process">仕分け作業状況</param>
        /// <returns>MAX文</returns>
        private string GetOrderItemMaxSql(string tableName, ORDER_PROCESS process)
        {
            //Logger.WriteLog(LogType.METHOD_IN, $"OrderInfo_IF_DB.GetOrderItemMaxSql({tableName})");
            var result = new StringBuilder();
            result.Append($"NOT EXISTS ");
            result.Append($"( ");
            result.Append($"SELECT ");
            result.Append($"b.* ");
            result.Append($"FROM ");
            result.Append($"{tableName} b ");
            result.Append($"WHERE ");
            result.Append($"a.{ColumnNamePickHead.ORDER_DATE} = b.{ColumnNamePickHead.ORDER_DATE} ");
            result.Append($"AND ");
            result.Append($"a.{ColumnNamePickHead.POST_NO} = b.{ColumnNamePickHead.POST_NO} ");
            result.Append($"AND ");
            result.Append($"a.{ColumnNamePickHead.ORDER_DATE_REQUEST} = b.{ColumnNamePickHead.ORDER_DATE_REQUEST} ");
            result.Append($"AND ");
            result.Append($"a.{ColumnNamePickHead.POST_NO_REQUEST} = b.{ColumnNamePickHead.POST_NO_REQUEST} ");
            result.Append($"AND ");
            result.Append($"a.{ColumnNamePickHead.WORK_CODE} = b.{ColumnNamePickHead.WORK_CODE} ");
            result.Append($"AND ");
            result.Append($"a.{ColumnNamePickHead.INDEX} < b.{ColumnNamePickHead.INDEX} ");
            if (process != ORDER_PROCESS.NONE)
            {
                result.Append($"AND ");
                result.Append($"a.{ColumnNamePickHead.PROCESS} = b.{ColumnNamePickHead.PROCESS} ");
            }
            result.Append($") ");

            //Logger.WriteLog(LogType.METHOD_OUT, $"OrderInfo_IF_DB.GetOrderItemMaxSql");
            return result.ToString();
        }
        #endregion

        #region 商品ヘッダORDER文取得
        /// <summary>
        /// 商品ヘッダORDER文取得
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <returns>ORDER文</returns>
        private string GetOrderItemOrderSql(string tableName)
        {
            //Logger.WriteLog(LogType.METHOD_IN, $"OrderInfo_IF_DB.GetOrderItemOrderSql({tableName})");
            var result = new StringBuilder();
            result.Append($"ORDER BY ");
            result.Append($"a.{ColumnNamePickHead.ORDER_DATE}, ");
            result.Append($"a.{ColumnNamePickHead.POST_NO}, ");
            result.Append($"a.{ColumnNamePickHead.ORDER_DATE_REQUEST}, ");
            result.Append($"a.{ColumnNamePickHead.POST_NO_REQUEST}, ");
            result.Append($"a.{ColumnNamePickHead.WORK_CODE}, ");
            result.Append($"a.{ColumnNamePickHead.INDEX}");

            //Logger.WriteLog(LogType.METHOD_OUT, $"OrderInfo_IF_DB.GetOrderItemOrderSql");
            return result.ToString();
        }
        #endregion

        #region 読み込みデータを仕分商品ヘッダに変換
        /// <summary>
        /// 読み込みデータを仕分商品ヘッダに変換
        /// </summary>
        /// <param name="data">読み込みデータ</param>
        /// <param name="orderData">仕分商品ヘッダデータ</param>
        private void DataToOrderItem(Dictionary<string, object> data, out OrderData orderData)
        {
            try
            {
                var createDateTime = ConvertDateTime(data[ColumnNamePickHead.CREATE_DATE], data[ColumnNamePickHead.CREATE_TIME]);
                var updateDateTime = ConvertDateTime(data[ColumnNamePickHead.UPDATE_DATE], data[ColumnNamePickHead.UPDATE_TIME]);
                orderData = new OrderData
                {
                    orderDate = DateTime.ParseExact(data[ColumnNamePickHead.ORDER_DATE].ToString(), DateFormat, null),
                    postNo = Convert.ToInt32(data[ColumnNamePickHead.POST_NO]),
                    orderDateRequest = DateTime.ParseExact(data[ColumnNamePickHead.ORDER_DATE_REQUEST].ToString(), DateFormat, null),
                    postNoRequest = Convert.ToInt32(data[ColumnNamePickHead.POST_NO_REQUEST]),
                    workCode = data[ColumnNamePickHead.WORK_CODE].ToString(),
                    index = Convert.ToInt32(data[ColumnNamePickHead.INDEX]),
                    workName = data[ColumnNamePickHead.WORK_NAME].ToString(),
                    JANCode = data[ColumnNamePickHead.JAN_CODE].ToString(),
                    caseVolume = Convert.ToDouble(data[ColumnNamePickHead.CASE_VOLUME]),
                    orderCountTotal = Convert.ToDouble(data[ColumnNamePickHead.ORDER_COUNT_TOTAL]),
                    workNameKana = data[ColumnNamePickHead.WORK_NAME_KANA].ToString(),
                    maxStackNum = Convert.ToInt32(data[ColumnNamePickHead.MAX_STACK_NUM]),
                    salesPrice = Convert.ToDouble(data[ColumnNamePickHead.SALES_PRICE]),
                    process = (ORDER_PROCESS)Convert.ToInt32(data[ColumnNamePickHead.PROCESS]),
                    createDateTime = createDateTime,
                    createLoginId = data[ColumnNamePickHead.CREATE_LOGIN_ID].ToString(),
                    updateDateTime = updateDateTime,
                    updateLoginId = data[ColumnNamePickHead.UPDATE_LOGIN_ID].ToString()
                };
                for (var i = 1; i <= orderData.orderCount.Length; i++)
                    if (data.ContainsKey($"{ColumnNamePickHead.ORDER_COUNT}{i}"))
                        orderData.orderCount[i - 1] = Convert.ToDouble(data[$"{ColumnNamePickHead.ORDER_COUNT}{i}"]);
                for (var i = 1; i <= orderData.storeCount.Length; i++)
                    if (data.ContainsKey($"{ColumnNamePickHead.STORE_COUNT}{i}"))
                        orderData.storeCount[i - 1] = Convert.ToInt32(data[$"{ColumnNamePickHead.STORE_COUNT}{i}"]);
            }
            catch (Exception ex)
            {
                orderData = null;
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                throw;
            }
        }
        #endregion
        #endregion


        #region 店別小仕分けテーブル
        /// <summary>
        /// 店別小仕分けテーブル
        /// 1行読み出し
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="orderDateTime">仕分納品日</param>
        /// <param name="postNo">仕分便No</param>
        /// <param name="workCode">商品コード</param>
        /// <param name="itemList">1商品分のデータリスト</param>
        /// <returns>エラーコード</returns>
        public UInt32 GetOrderStoreItem(string tableName, DateTime orderDateTime, int postNo, string workCode, out List<OrderStoreData> itemList) 
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"OrderInfo_IF_DB.GetOrderStoreItem({tableName}, {orderDateTime}, {postNo}, {workCode})");
            itemList = null;
            try
            {
                var sql = new StringBuilder();
                sql.Append($"SELECT ");
                sql.Append($"a.{ColumnNamePickDetail.ORDER_DATE}, ");
                sql.Append($"a.{ColumnNamePickDetail.POST_NO}, ");
                sql.Append($"a.{ColumnNamePickDetail.ORDER_DATE_REQUEST}, ");
                sql.Append($"a.{ColumnNamePickDetail.POST_NO_REQUEST}, ");
                sql.Append($"a.{ColumnNamePickDetail.WORK_CODE}, ");
                sql.Append($"a.{ColumnNamePickDetail.INDEX}, ");
                sql.Append($"a.{ColumnNamePickDetail.STORE_CODE}, ");
                sql.Append($"a.{ColumnNamePickDetail.STATION_NO}, ");
                sql.Append($"a.{ColumnNamePickDetail.AISLE_NO}, ");
                sql.Append($"a.{ColumnNamePickDetail.SLOT_NO}, ");
                sql.Append($"a.{ColumnNamePickDetail.CASE_VOLUME}, ");
                sql.Append($"a.{ColumnNamePickDetail.ORDER_COUNT}, ");
                sql.Append($"a.{ColumnNamePickDetail.CREATE_DATE}, ");
                sql.Append($"a.{ColumnNamePickDetail.CREATE_TIME}, ");
                sql.Append($"a.{ColumnNamePickDetail.CREATE_LOGIN_ID}, ");
                sql.Append($"a.{ColumnNamePickDetail.UPDATE_DATE}, ");
                sql.Append($"a.{ColumnNamePickDetail.UPDATE_TIME}, ");
                sql.Append($"a.{ColumnNamePickDetail.UPDATE_LOGIN_ID} ");
                sql.Append($"FROM ");
                sql.Append($"{tableName} a ");
                sql.Append($"WHERE ");
                sql.Append($"a.{ColumnNamePickDetail.ORDER_DATE} = '{orderDateTime.ToString(DateFormat)}' ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNamePickDetail.POST_NO} = '{postNo}' ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNamePickDetail.WORK_CODE} = '{workCode}' ");
                sql.Append($"AND ");
                sql.Append($"NOT EXISTS ");
                sql.Append($"( ");
                sql.Append($"SELECT ");
                sql.Append($"b.* ");
                sql.Append($"FROM ");
                sql.Append($"{tableName} b ");
                sql.Append($"WHERE ");
                sql.Append($"a.{ColumnNamePickDetail.ORDER_DATE} = b.{ColumnNamePickDetail.ORDER_DATE} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNamePickDetail.POST_NO} = b.{ColumnNamePickDetail.POST_NO} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNamePickDetail.ORDER_DATE_REQUEST} = b.{ColumnNamePickDetail.ORDER_DATE_REQUEST} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNamePickDetail.POST_NO_REQUEST} = b.{ColumnNamePickDetail.POST_NO_REQUEST} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNamePickDetail.WORK_CODE} = b.{ColumnNamePickDetail.WORK_CODE} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNamePickDetail.INDEX} < b.{ColumnNamePickDetail.INDEX} ");
                sql.Append($") ");
                sql.Append($"ORDER BY ");
                sql.Append($"a.{ColumnNamePickDetail.ORDER_DATE}, ");
                sql.Append($"a.{ColumnNamePickDetail.POST_NO}, ");
                sql.Append($"a.{ColumnNamePickDetail.ORDER_DATE_REQUEST}, ");
                sql.Append($"a.{ColumnNamePickDetail.POST_NO_REQUEST}, ");
                sql.Append($"a.{ColumnNamePickDetail.WORK_CODE}, ");
                sql.Append($"a.{ColumnNamePickDetail.INDEX}");
                using (var dbAccess = new DBAccess())
                {
                    rc = dbAccess.ExecuteReader(connectionString, sql.ToString(), out var dataList, retryCount);
                    if (rc == (UInt32)ErrorCodeList.STATUS_SUCCESS)
                    {
                        if (dataList != null && dataList.Count > 0)
                        {
                            itemList = new List<OrderStoreData>();
                            foreach (var data in dataList)
                            {
                                DataToOrderStoreItem(data, out var orderStoreData);
                                itemList.Add(orderStoreData);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                itemList = null;
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"OrderInfo_IF_DB.GetOrderStoreItem : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 店別小仕分けテーブル
        /// 1行書込み(PICKDATAから)
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="item">1行データ</param>/// <returns>エラーコード</returns>
        public UInt32 SetOrderStoreItemsFromPickData(string tableName, OrderStoreData item)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"OrderInfo_IF_DB.SetExecStoreItems({tableName})");
            try
            {

                var now = DateTime.Now;
                // Postgresqlのバージョンが8.4以降の場合はWITH文が使用できる
                //var sql = GetExecuteWithSql(tableName, now, item);
                //using (var dbAccess = new DBAccess())
                //{
                //    rc = dbAccess.ExecuteNonSQL(connectionString, sql.ToString(), retryCount);
                //}

                // Postgresqlのバージョンが8.3以前の場合は、SELECT→INSERT or UPDATEの2回実行する必要がある
                // SELECT文
                var selectSql = new StringBuilder();
                selectSql.Append($"SELECT ");
                selectSql.Append($"{ColumnNamePickDetail.ORDER_DATE} ");
                selectSql.Append($"FROM ");
                selectSql.Append($"{tableName} ");
                selectSql.Append($"WHERE ");
                selectSql.Append($"{ColumnNamePickDetail.ORDER_DATE} = '{item.orderDate.ToString(DateFormat)}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNamePickDetail.POST_NO} = '{item.postNo}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNamePickDetail.ORDER_DATE_REQUEST} = '{item.orderDateRequest.ToString(DateFormat)}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNamePickDetail.POST_NO_REQUEST} = '{item.postNoRequest}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNamePickDetail.WORK_CODE} = '{item.workCode}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNamePickDetail.INDEX} = {item.index} ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNamePickDetail.STORE_CODE} = '{item.storeCode}' ");
                // INSERT文
                var insertSql = new StringBuilder();
                insertSql.Append($"INSERT INTO {tableName} ");
                insertSql.Append($"( ");
                insertSql.Append($"{ColumnNamePickDetail.ORDER_DATE}, ");
                insertSql.Append($"{ColumnNamePickDetail.POST_NO}, ");
                insertSql.Append($"{ColumnNamePickDetail.ORDER_DATE_REQUEST}, ");
                insertSql.Append($"{ColumnNamePickDetail.POST_NO_REQUEST}, ");
                insertSql.Append($"{ColumnNamePickDetail.WORK_CODE}, ");
                insertSql.Append($"{ColumnNamePickDetail.INDEX}, ");
                insertSql.Append($"{ColumnNamePickDetail.STORE_CODE}, ");
                insertSql.Append($"{ColumnNamePickDetail.STATION_NO}, ");
                insertSql.Append($"{ColumnNamePickDetail.AISLE_NO}, ");
                insertSql.Append($"{ColumnNamePickDetail.SLOT_NO}, ");
                insertSql.Append($"{ColumnNamePickDetail.CASE_VOLUME}, ");
                insertSql.Append($"{ColumnNamePickDetail.ORDER_COUNT}, ");
                insertSql.Append($"{ColumnNamePickDetail.CREATE_DATE}, ");
                insertSql.Append($"{ColumnNamePickDetail.CREATE_TIME}, ");
                insertSql.Append($"{ColumnNamePickDetail.CREATE_LOGIN_ID} ");
                insertSql.Append($") VALUES ( ");
                insertSql.Append($"'{item.orderDate.ToString(DateFormat)}', ");
                insertSql.Append($"'{item.postNo}', ");
                insertSql.Append($"'{item.orderDateRequest.ToString(DateFormat)}', ");
                insertSql.Append($"'{item.postNoRequest}', ");
                insertSql.Append($"'{item.workCode}', ");
                insertSql.Append($"{item.index}, ");
                insertSql.Append($"'{item.storeCode}', ");
                insertSql.Append($"'{item.stationNo}', ");
                insertSql.Append($"'{item.aisleNo}', ");
                insertSql.Append($"'{item.slotNo}', ");
                insertSql.Append($"{item.caseVolume}, ");
                insertSql.Append($"{item.orderCount}, ");
                insertSql.Append($"'{now.ToString(DateFormat)}', ");
                insertSql.Append($"'{now.ToString(TimeFormat)}', ");
                insertSql.Append($"'{item.createLoginId}' ");
                insertSql.Append($") ");
                // UPDATE文
                var updateSql = new StringBuilder();
                updateSql.Append($"UPDATE {tableName} ");
                updateSql.Append($"SET ");
                updateSql.Append($"{ColumnNamePickDetail.ORDER_COUNT} = {item.orderCount}, ");
                updateSql.Append($"{ColumnNamePickDetail.UPDATE_DATE} = '{now.ToString(DateFormat)}', ");
                updateSql.Append($"{ColumnNamePickDetail.UPDATE_TIME} = '{now.ToString(TimeFormat)}', ");
                updateSql.Append($"{ColumnNamePickDetail.UPDATE_LOGIN_ID} = '{item.updateLoginId}' ");
                updateSql.Append($"WHERE ");
                updateSql.Append($"{ColumnNamePickDetail.ORDER_DATE} = '{item.orderDate.ToString(DateFormat)}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNamePickDetail.POST_NO} = '{item.postNo}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNamePickDetail.ORDER_DATE_REQUEST} = '{item.orderDateRequest.ToString(DateFormat)}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNamePickDetail.POST_NO_REQUEST} = '{item.postNoRequest}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNamePickDetail.WORK_CODE} = '{item.workCode}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNamePickDetail.INDEX} = {item.index} ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNamePickDetail.STORE_CODE} = '{item.storeCode}' ");
                using (var dbAccess = new DBAccess())
                {
                    rc = dbAccess.Upsert(connectionString, selectSql.ToString(), insertSql.ToString(), updateSql.ToString(), retryCount);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"OrderInfo_IF_DB.SetExecStoreItems : {(ErrorCodeList)rc}");
            return rc;
        }


        #region 読み込みデータを店別小仕分に変換
        /// <summary>
        /// 読み込みデータを店別小仕分に変換
        /// </summary>
        /// <param name="data">読み込みデータ</param>
        /// <param name="orderStoreData">店別小仕分データ</param>
        private void DataToOrderStoreItem(Dictionary<string, object> data, out OrderStoreData orderStoreData)
        {
            try
            {
                var createDateTime = ConvertDateTime(data[ColumnNamePickDetail.CREATE_DATE], data[ColumnNamePickDetail.CREATE_TIME]);
                var updateDateTime = ConvertDateTime(data[ColumnNamePickDetail.UPDATE_DATE], data[ColumnNamePickDetail.UPDATE_TIME]);
                orderStoreData = new OrderStoreData
                {
                    orderDate = DateTime.ParseExact(data[ColumnNamePickDetail.ORDER_DATE].ToString(), DateFormat, null),
                    postNo = Convert.ToInt32(data[ColumnNamePickDetail.POST_NO]),
                    orderDateRequest = DateTime.ParseExact(data[ColumnNamePickDetail.ORDER_DATE_REQUEST].ToString(), DateFormat, null),
                    postNoRequest = Convert.ToInt32(data[ColumnNamePickDetail.POST_NO_REQUEST]),
                    workCode = data[ColumnNamePickDetail.WORK_CODE].ToString(),
                    index = Convert.ToInt32(data[ColumnNamePickDetail.INDEX]),
                    storeCode = data[ColumnNamePickDetail.STORE_CODE].ToString(),
                    stationNo = Convert.ToInt32(data[ColumnNamePickDetail.STATION_NO]),
                    aisleNo = Convert.ToInt32(data[ColumnNamePickDetail.AISLE_NO]),
                    slotNo = Convert.ToInt32(data[ColumnNamePickDetail.SLOT_NO]),
                    caseVolume = Convert.ToDouble(data[ColumnNamePickDetail.CASE_VOLUME]),
                    orderCount = Convert.ToDouble(data[ColumnNamePickDetail.ORDER_COUNT]),
                    createDateTime = createDateTime,
                    createLoginId = data[ColumnNamePickDetail.CREATE_LOGIN_ID].ToString(),
                    updateDateTime = updateDateTime,
                    updateLoginId = data[ColumnNamePickDetail.UPDATE_LOGIN_ID].ToString()
                };
            }
            catch (Exception ex)
            {
                orderStoreData = null;
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                throw;
            }
        }
        #endregion
        #endregion


        #region 商品ヘッダ実績テーブル
        /// <summary>
        /// 商品ヘッダ実績テーブル
        /// 1便読み出し
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="orderDateTime">仕分納品日</param>
        /// <param name="postNo">仕分便No</param>
        /// <param name="workCode">商品コード</param>
        /// <param name="item">1商品分のデータリスト</param>
        /// <returns>エラーコード</returns>
        public UInt32 GetExecuteItem(string tableName, DateTime orderDateTime, int postNo, out List<ExecuteData> itemList)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"OrderInfo_IF_DB.GetExecuteItem({tableName}, {orderDateTime}, {postNo}, {workCode})");
            itemList = null;
            try
            {
                var sql = new StringBuilder();
                sql.Append($"SELECT ");
                sql.Append($"a.{ColumnNameCompHead.ORDER_DATE}, ");
                sql.Append($"a.{ColumnNameCompHead.POST_NO}, ");
                sql.Append($"a.{ColumnNameCompHead.ORDER_DATE_REQUEST}, ");
                sql.Append($"a.{ColumnNameCompHead.POST_NO_REQUEST}, ");
                sql.Append($"a.{ColumnNameCompHead.WORK_CODE}, ");
                sql.Append($"a.{ColumnNameCompHead.INDEX}, ");
                sql.Append($"a.{ColumnNameCompHead.JAN_CODE}, ");
                sql.Append($"a.{ColumnNameCompHead.ORDER_COUNT_TOTAL}, ");
                sql.Append($"a.{ColumnNameCompHead.ORDER_COMP_COUNT_TOTAL}, ");
                sql.Append($"a.{ColumnNameCompHead.LOAD_DATE_TIME}, ");
                sql.Append($"a.{ColumnNameCompHead.CREATE_DATE}, ");
                sql.Append($"a.{ColumnNameCompHead.CREATE_TIME}, ");
                sql.Append($"a.{ColumnNameCompHead.CREATE_LOGIN_ID}, ");
                sql.Append($"a.{ColumnNameCompHead.UPDATE_DATE}, ");
                sql.Append($"a.{ColumnNameCompHead.UPDATE_TIME}, ");
                sql.Append($"a.{ColumnNameCompHead.UPDATE_LOGIN_ID} ");
                sql.Append($"FROM ");
                sql.Append($"{tableName} a ");
                sql.Append($"WHERE ");
                sql.Append($"a.{ColumnNameCompHead.ORDER_DATE} = '{orderDateTime.ToString(DateFormat)}' ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompHead.POST_NO} = '{postNo}' ");
                //sql.Append($"AND ");
                //sql.Append($"a.{ColumnNameCompHead.WORK_CODE} = '{workCode}' ");
                sql.Append($"AND ");
                sql.Append($"NOT EXISTS ");
                sql.Append($"( ");
                sql.Append($"SELECT ");
                sql.Append($"b.* ");
                sql.Append($"FROM ");
                sql.Append($"{tableName} b ");
                sql.Append($"WHERE ");
                sql.Append($"a.{ColumnNameCompHead.ORDER_DATE} = b.{ColumnNameCompHead.ORDER_DATE} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompHead.POST_NO} = b.{ColumnNameCompHead.POST_NO} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompHead.ORDER_DATE_REQUEST} = b.{ColumnNameCompHead.ORDER_DATE_REQUEST} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompHead.POST_NO_REQUEST} = b.{ColumnNameCompHead.POST_NO_REQUEST} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompHead.WORK_CODE} = b.{ColumnNameCompHead.WORK_CODE} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompHead.INDEX} < b.{ColumnNameCompHead.INDEX} ");
                sql.Append($") ");
                sql.Append($"ORDER BY ");
                sql.Append($"a.{ColumnNameCompHead.ORDER_DATE}, ");
                sql.Append($"a.{ColumnNameCompHead.POST_NO}, ");
                sql.Append($"a.{ColumnNameCompHead.ORDER_DATE_REQUEST}, ");
                sql.Append($"a.{ColumnNameCompHead.POST_NO_REQUEST}, ");
                sql.Append($"a.{ColumnNameCompHead.WORK_CODE}, ");
                sql.Append($"a.{ColumnNameCompHead.INDEX}");
                using (var dbAccess = new DBAccess())
                {
                    rc = dbAccess.ExecuteReader(connectionString, sql.ToString(), out var dataList, retryCount);
                    if (rc == (UInt32)ErrorCodeList.STATUS_SUCCESS)
                    {
                        if (dataList != null && dataList.Count > 0) 
                        {
                            itemList = new List<ExecuteData>();
                            foreach (var data in dataList)
                            {
                                DataToExecuteItem(data, out var executeData);
                                itemList.Add(executeData);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"OrderInfo_IF_DB.GetExecuteItem : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 商品ヘッダ実績テーブル
        /// 1行読み出し
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="orderDateTime">仕分納品日</param>
        /// <param name="postNo">仕分便No</param>
        /// <param name="workCode">商品コード</param>
        /// <param name="item">1商品分のデータリスト</param>
        /// <returns>エラーコード</returns>
        public UInt32 GetExecuteItem(string tableName, DateTime orderDateTime, int postNo, string workCode, out ExecuteData item) 
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"OrderInfo_IF_DB.GetExecuteItem({tableName}, {orderDateTime}, {postNo}, {workCode})");
            item = null;
            try
            {
                var sql = new StringBuilder();
                sql.Append($"SELECT ");
                sql.Append($"a.{ColumnNameCompHead.ORDER_DATE}, ");
                sql.Append($"a.{ColumnNameCompHead.POST_NO}, ");
                sql.Append($"a.{ColumnNameCompHead.ORDER_DATE_REQUEST}, ");
                sql.Append($"a.{ColumnNameCompHead.POST_NO_REQUEST}, ");
                sql.Append($"a.{ColumnNameCompHead.WORK_CODE}, ");
                sql.Append($"a.{ColumnNameCompHead.INDEX}, ");
                sql.Append($"a.{ColumnNameCompHead.JAN_CODE}, ");
                sql.Append($"a.{ColumnNameCompHead.ORDER_COUNT_TOTAL}, ");
                sql.Append($"a.{ColumnNameCompHead.ORDER_COMP_COUNT_TOTAL}, ");
                sql.Append($"a.{ColumnNameCompHead.LOAD_DATE_TIME}, ");
                sql.Append($"a.{ColumnNameCompHead.CREATE_DATE}, ");
                sql.Append($"a.{ColumnNameCompHead.CREATE_TIME}, ");
                sql.Append($"a.{ColumnNameCompHead.CREATE_LOGIN_ID}, ");
                sql.Append($"a.{ColumnNameCompHead.UPDATE_DATE}, ");
                sql.Append($"a.{ColumnNameCompHead.UPDATE_TIME}, ");
                sql.Append($"a.{ColumnNameCompHead.UPDATE_LOGIN_ID} ");
                sql.Append($"FROM ");
                sql.Append($"{tableName} a ");
                sql.Append($"WHERE ");
                sql.Append($"a.{ColumnNameCompHead.ORDER_DATE} = '{orderDateTime.ToString(DateFormat)}' ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompHead.POST_NO} = '{postNo}' ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompHead.WORK_CODE} = '{workCode}' ");
                sql.Append($"AND ");
                sql.Append($"NOT EXISTS ");
                sql.Append($"( ");
                sql.Append($"SELECT ");
                sql.Append($"b.* ");
                sql.Append($"FROM ");
                sql.Append($"{tableName} b ");
                sql.Append($"WHERE ");
                sql.Append($"a.{ColumnNameCompHead.ORDER_DATE} = b.{ColumnNameCompHead.ORDER_DATE} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompHead.POST_NO} = b.{ColumnNameCompHead.POST_NO} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompHead.ORDER_DATE_REQUEST} = b.{ColumnNameCompHead.ORDER_DATE_REQUEST} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompHead.POST_NO_REQUEST} = b.{ColumnNameCompHead.POST_NO_REQUEST} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompHead.WORK_CODE} = b.{ColumnNameCompHead.WORK_CODE} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompHead.INDEX} < b.{ColumnNameCompHead.INDEX} ");
                sql.Append($") ");
                sql.Append($"ORDER BY ");
                sql.Append($"a.{ColumnNameCompHead.ORDER_DATE}, ");
                sql.Append($"a.{ColumnNameCompHead.POST_NO}, ");
                sql.Append($"a.{ColumnNameCompHead.ORDER_DATE_REQUEST}, ");
                sql.Append($"a.{ColumnNameCompHead.POST_NO_REQUEST}, ");
                sql.Append($"a.{ColumnNameCompHead.WORK_CODE}, ");
                sql.Append($"a.{ColumnNameCompHead.INDEX}");
                using (var dbAccess = new DBAccess())
                {
                    rc = dbAccess.ExecuteReader(connectionString, sql.ToString(), out var dataList, retryCount);
                    if (rc == (UInt32)ErrorCodeList.STATUS_SUCCESS)
                    {
                        if (dataList != null && dataList.Count > 0)
                            DataToExecuteItem(dataList[0], out item);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"OrderInfo_IF_DB.GetExecuteItem : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 商品ヘッダ実績テーブル
        /// 1行書込み
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="item">1行データ</param>
        /// <returns>エラーコード</returns>
        public UInt32 SetExecuteItem(string tableName, ExecuteData item, string updateLoginId)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"OrderInfo_IF_DB.SetExecuteItem({tableName})");
            try
            {
                var now = DateTime.Now;
                // Postgresqlのバージョンが8.4以降の場合はWITH文が使用できる
                //var sql = GetExecuteWithSql(tableName, now, item);
                //using (var dbAccess = new DBAccess())
                //{
                //    rc = dbAccess.ExecuteNonSQL(connectionString, sql.ToString(), retryCount);
                //}

                // Postgresqlのバージョンが8.3以前の場合は、SELECT→INSERT or UPDATEの2回実行する必要がある
                // SELECT文
                var selectSql = new StringBuilder();
                selectSql.Append($"SELECT ");
                selectSql.Append($"{ColumnNameCompHead.ORDER_DATE} ");
                selectSql.Append($"FROM ");
                selectSql.Append($"{tableName} ");
                selectSql.Append($"WHERE ");
                selectSql.Append($"{ColumnNameCompHead.ORDER_DATE} = '{item.orderDate.ToString(DateFormat)}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNameCompHead.POST_NO} = '{item.postNo}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNameCompHead.ORDER_DATE_REQUEST} = '{item.orderDateRequest.ToString(DateFormat)}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNameCompHead.POST_NO_REQUEST} = '{item.postNoRequest}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNameCompHead.WORK_CODE} = '{item.workCode}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNameCompHead.INDEX} = {item.index} ");
                // INSERT文
                var insertSql = new StringBuilder();
                insertSql.Append($"INSERT INTO {tableName} ");
                insertSql.Append($"( ");
                insertSql.Append($"{ColumnNameCompHead.ORDER_DATE}, ");
                insertSql.Append($"{ColumnNameCompHead.POST_NO}, ");
                insertSql.Append($"{ColumnNameCompHead.ORDER_DATE_REQUEST}, ");
                insertSql.Append($"{ColumnNameCompHead.POST_NO_REQUEST}, ");
                insertSql.Append($"{ColumnNameCompHead.WORK_CODE}, ");
                insertSql.Append($"{ColumnNameCompHead.INDEX}, ");
                insertSql.Append($"{ColumnNameCompHead.JAN_CODE}, ");
                insertSql.Append($"{ColumnNameCompHead.ORDER_COUNT_TOTAL}, ");
                insertSql.Append($"{ColumnNameCompHead.ORDER_COMP_COUNT_TOTAL}, ");
                insertSql.Append($"{ColumnNameCompHead.LOAD_DATE_TIME}, ");
                insertSql.Append($"{ColumnNameCompHead.CREATE_DATE}, ");
                insertSql.Append($"{ColumnNameCompHead.CREATE_TIME}, ");
                insertSql.Append($"{ColumnNameCompHead.CREATE_LOGIN_ID} ");
                insertSql.Append($") VALUES ( ");
                insertSql.Append($"'{item.orderDate.ToString(DateFormat)}', ");
                insertSql.Append($"'{item.postNo}', ");
                insertSql.Append($"'{item.orderDateRequest.ToString(DateFormat)}', ");
                insertSql.Append($"'{item.postNoRequest}', ");
                insertSql.Append($"'{item.workCode}', ");
                insertSql.Append($"{item.index}, ");
                insertSql.Append($"'{item.JANCode}', ");
                insertSql.Append($"{item.orderCountTotal}, ");
                insertSql.Append($"{item.orderCompCountTotal}, ");
                //insertSql.Append($"'{item.loadDateTime.ToString($"{DateFormat}{TimeFormat}")}', ");
                insertSql.Append($"'{item.loadDateTime}', ");
                insertSql.Append($"'{now.ToString(DateFormat)}', ");
                insertSql.Append($"'{now.ToString(TimeFormat)}', ");
                insertSql.Append($"'{item.createLoginId}' ");
                insertSql.Append($") ");
                // UPDATE文
                var updateSql = new StringBuilder();
                updateSql.Append($"UPDATE {tableName} ");
                updateSql.Append($"SET ");
                updateSql.Append($"{ColumnNameCompHead.JAN_CODE} = '{item.JANCode}', ");
                updateSql.Append($"{ColumnNameCompHead.ORDER_COUNT_TOTAL} = {item.orderCountTotal}, ");
                updateSql.Append($"{ColumnNameCompHead.ORDER_COMP_COUNT_TOTAL} = {item.orderCompCountTotal}, ");
                //updateSql.Append($"{ColumnNameCompHead.LOAD_DATE_TIME} = '{item.loadDateTime.ToString($"{DateFormat}{TimeFormat}")}', ");
                updateSql.Append($"{ColumnNameCompHead.LOAD_DATE_TIME} = '{item.loadDateTime}', ");
                updateSql.Append($"{ColumnNameCompHead.UPDATE_DATE} = '{now.ToString(DateFormat)}', ");
                updateSql.Append($"{ColumnNameCompHead.UPDATE_TIME} = '{now.ToString(TimeFormat)}', ");
                updateSql.Append($"{ColumnNameCompHead.UPDATE_LOGIN_ID} = '{updateLoginId}' ");
                updateSql.Append($"WHERE ");
                updateSql.Append($"{ColumnNameCompHead.ORDER_DATE} = '{item.orderDate.ToString(DateFormat)}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNameCompHead.POST_NO} = '{item.postNo}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNameCompHead.ORDER_DATE_REQUEST} = '{item.orderDateRequest.ToString(DateFormat)}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNameCompHead.POST_NO_REQUEST} = '{item.postNoRequest}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNameCompHead.WORK_CODE} = '{item.workCode}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNameCompHead.INDEX} = {item.index} ");
                using (var dbAccess = new DBAccess())
                {
                    //throw new Exception("エラーが発生しました");
                    rc = dbAccess.Upsert(connectionString, selectSql.ToString(), insertSql.ToString(), updateSql.ToString(), retryCount);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"OrderInfo_IF_DB.SetExecuteItem : {(ErrorCodeList)rc}");
            return rc;
        }


        #region 仕分完了ヘッダWITH文取得
        /// <summary>
        /// 仕分完了ヘッダWITH文取得
        /// ※PostgreSQL8.4以降からしか使用できない
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="now">更新日時</param>
        /// <param name="item">1行データ</param>
        /// <returns></returns>
        private string GetExecuteWithSql(string tableName, DateTime now, ExecuteData item)
        {
            var result = new StringBuilder();
            result.Append($"WITH upsert AS ( ");
            result.Append($"UPDATE {tableName} ");
            result.Append($"SET ");
            result.Append($"{ColumnNameCompHead.JAN_CODE} = '{item.JANCode}', ");
            result.Append($"{ColumnNameCompHead.ORDER_COUNT_TOTAL} = {item.orderCountTotal}, ");
            result.Append($"{ColumnNameCompHead.ORDER_COMP_COUNT_TOTAL} = {item.orderCompCountTotal}, ");
            //result.Append($"{ColumnNameCompHead.LOAD_DATE_TIME} = '{item.loadDateTime.ToString($"{DateFormat}{TimeFormat}")}', ");
            result.Append($"{ColumnNameCompHead.LOAD_DATE_TIME} = '{item.loadDateTime}', ");
            result.Append($"{ColumnNameCompHead.UPDATE_DATE} = '{now.ToString(DateFormat)}', ");
            result.Append($"{ColumnNameCompHead.UPDATE_TIME} = '{now.ToString(TimeFormat)}', ");
            result.Append($"{ColumnNameCompHead.UPDATE_LOGIN_ID} = '{item.updateLoginId}' ");
            result.Append($"WHERE ");
            result.Append($"{ColumnNameCompHead.ORDER_DATE} = '{item.orderDate.ToString(DateFormat)}' ");
            result.Append($"AND ");
            result.Append($"{ColumnNameCompHead.POST_NO} = '{item.postNo}' ");
            result.Append($"AND ");
            result.Append($"{ColumnNameCompHead.ORDER_DATE_REQUEST} = '{item.orderDateRequest.ToString(DateFormat)}' ");
            result.Append($"AND ");
            result.Append($"{ColumnNameCompHead.POST_NO_REQUEST} = '{item.postNoRequest}' ");
            result.Append($"AND ");
            result.Append($"{ColumnNameCompHead.WORK_CODE} = '{item.workCode}' ");
            result.Append($"AND ");
            result.Append($"{ColumnNameCompHead.INDEX} = {item.index} ");
            result.Append($"RETURNING * ");
            result.Append($") ");
            result.Append($"INSERT INTO {tableName} ");
            result.Append($"( ");
            result.Append($"{ColumnNameCompHead.ORDER_DATE}, ");
            result.Append($"{ColumnNameCompHead.POST_NO}, ");
            result.Append($"{ColumnNameCompHead.ORDER_DATE_REQUEST}, ");
            result.Append($"{ColumnNameCompHead.POST_NO_REQUEST}, ");
            result.Append($"{ColumnNameCompHead.WORK_CODE}, ");
            result.Append($"{ColumnNameCompHead.INDEX}, ");
            result.Append($"{ColumnNameCompHead.JAN_CODE}, ");
            result.Append($"{ColumnNameCompHead.ORDER_COUNT_TOTAL}, ");
            result.Append($"{ColumnNameCompHead.ORDER_COMP_COUNT_TOTAL}, ");
            result.Append($"{ColumnNameCompHead.LOAD_DATE_TIME}, ");
            result.Append($"{ColumnNameCompHead.CREATE_DATE}, ");
            result.Append($"{ColumnNameCompHead.CREATE_TIME}, ");
            result.Append($"{ColumnNameCompHead.CREATE_LOGIN_ID} ");
            result.Append($") ");
            result.Append($"SELECT ");
            result.Append($"'{item.orderDate.ToString(DateFormat)}', ");
            result.Append($"'{item.postNo}', ");
            result.Append($"'{item.orderDateRequest.ToString(DateFormat)}', ");
            result.Append($"'{item.postNoRequest}', ");
            result.Append($"'{item.workCode}', ");
            result.Append($"{item.index}, ");
            result.Append($"'{item.JANCode}', ");
            result.Append($"{item.orderCountTotal}, ");
            result.Append($"{item.orderCompCountTotal}, ");
            //result.Append($"'{item.loadDateTime.ToString($"{DateFormat}{TimeFormat}")}', ");
            result.Append($"'{item.loadDateTime}', ");
            result.Append($"'{now.ToString(DateFormat)}', ");
            result.Append($"'{now.ToString(TimeFormat)}', ");
            result.Append($"'{item.createLoginId}' ");
            result.Append($"WHERE ");
            result.Append($"NOT EXISTS (SELECT * FROM upsert) ");

            return result.ToString();
        }
        #endregion

        #region 読み込みデータを仕分完了商品ヘッダに変換
        /// <summary>
        /// 読み込みデータを仕分完了商品ヘッダに変換
        /// </summary>
        /// <param name="data">読み込みデータ</param>
        /// <param name="orderData">仕分完了商品ヘッダデータ</param>
        private void DataToExecuteItem(Dictionary<string, object> data, out ExecuteData orderData)
        {
            try
            {
                var createDateTime = ConvertDateTime(data[ColumnNameCompHead.CREATE_DATE], data[ColumnNameCompHead.CREATE_TIME]);
                var updateDateTime = ConvertDateTime(data[ColumnNameCompHead.UPDATE_DATE], data[ColumnNameCompHead.UPDATE_TIME]);

                //Logger.WriteLog(LogType.INFO, $"@@@START");
                //ExecuteData ed = new ExecuteData();
                //string s1 = data[ColumnNameCompHead.ORDER_DATE].ToString();
                //string s2 = data[ColumnNameCompHead.ORDER_DATE_REQUEST].ToString();
                //string s3 = data[ColumnNameCompHead.LOAD_DATE_TIME].ToString();
                //string s4 = data[ColumnNameCompHead.ORDER_DATE].ToString();
                //string s5 = createDateTime.ToString();
                //string s6 = updateDateTime.ToString();
                //string jan = data[ColumnNameCompHead.JAN_CODE].ToString();
                //Logger.WriteLog(LogType.INFO, $"{jan} {s1} {s2} {s3} {s4} {s5} {s6}");
                //ed.orderDate = DateTime.ParseExact(s1, DateFormat, null);
                //ed.orderDateRequest = DateTime.ParseExact(data[ColumnNameCompHead.ORDER_DATE].ToString(), DateFormat, null);
                //Logger.WriteLog(LogType.INFO, $"@@@END");

                //string load = data[ColumnNameCompHead.LOAD_DATE_TIME].ToString();
                //string jan = data[ColumnNameCompHead.JAN_CODE].ToString();
                //if (jan == "4901306075111")
                //    load = "00000000000000";

                orderData = new ExecuteData
                {
                    orderDate = DateTime.ParseExact(data[ColumnNameCompHead.ORDER_DATE].ToString(), DateFormat, null),
                    postNo = Convert.ToInt32(data[ColumnNameCompHead.POST_NO]),
                    orderDateRequest = DateTime.ParseExact(data[ColumnNameCompHead.ORDER_DATE_REQUEST].ToString(), DateFormat, null),
                    postNoRequest = Convert.ToInt32(data[ColumnNameCompHead.POST_NO_REQUEST]),
                    workCode = data[ColumnNameCompHead.WORK_CODE].ToString(),
                    index = Convert.ToInt32(data[ColumnNameCompHead.INDEX]),
                    JANCode = data[ColumnNameCompHead.JAN_CODE].ToString(),
                    orderCountTotal = Convert.ToDouble(data[ColumnNameCompHead.ORDER_COUNT_TOTAL]),
                    orderCompCountTotal = Convert.ToDouble(data[ColumnNameCompHead.ORDER_COMP_COUNT_TOTAL]),
                    loadDateTime = Convert.ToString(data[ColumnNameCompHead.LOAD_DATE_TIME]),   // loadDateTimeはDB上では初期値が"00000000000000"のため、DateTime.ParseExactはエラーになる
                    //loadDateTime = DateTime.ParseExact(data[ColumnNameCompHead.LOAD_DATE_TIME].ToString(), $"{DateFormat}{TimeFormat}", null),
                    createDateTime = createDateTime,
                    createLoginId = data[ColumnNameCompHead.CREATE_LOGIN_ID].ToString(),
                    updateDateTime = updateDateTime,
                    updateLoginId = data[ColumnNameCompHead.UPDATE_LOGIN_ID].ToString()
                };
            }
            catch (Exception ex)
            {
                orderData = null;
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                throw;
            }
        }
        #endregion
        #endregion


        #region 店別小仕分け実績テーブル
        /// <summary>
        /// 店別小仕分け実績テーブル
        /// 1商品分読み出し
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="orderDateTime">仕分納品日</param>
        /// <param name="postNo">仕分便No</param>
        /// <param name="workCode">商品コード</param>
        /// <param name="itemList">1商品分のデータリスト</param>
        /// <returns>エラーコード</returns>
        public UInt32 GetExecStoreItems(string tableName, DateTime orderDateTime, int postNo, string workCode, out List<ExecuteStoreData> itemList) 
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"OrderInfo_IF_DB.GetExecStoreItems({tableName}, {orderDateTime}, {postNo}, {workCode})");
            itemList = null;
            try
            {
                var sql = new StringBuilder();
                sql.Append($"SELECT ");
                sql.Append($"a.{ColumnNameCompDetail.ORDER_DATE}, ");
                sql.Append($"a.{ColumnNameCompDetail.POST_NO}, ");
                sql.Append($"a.{ColumnNameCompDetail.ORDER_DATE_REQUEST}, ");
                sql.Append($"a.{ColumnNameCompDetail.POST_NO_REQUEST}, ");
                sql.Append($"a.{ColumnNameCompDetail.WORK_CODE}, ");
                sql.Append($"a.{ColumnNameCompDetail.INDEX}, ");
                sql.Append($"a.{ColumnNameCompDetail.STORE_CODE}, ");
                sql.Append($"a.{ColumnNameCompDetail.STATION_NO}, ");
                sql.Append($"a.{ColumnNameCompDetail.AISLE_NO}, ");
                sql.Append($"a.{ColumnNameCompDetail.SLOT_NO}, ");
                sql.Append($"a.{ColumnNameCompDetail.ORDER_COUNT}, ");
                sql.Append($"a.{ColumnNameCompDetail.ORDER_COMP_COUNT}, ");
                sql.Append($"a.{ColumnNameCompDetail.CREATE_DATE}, ");
                sql.Append($"a.{ColumnNameCompDetail.CREATE_TIME}, ");
                sql.Append($"a.{ColumnNameCompDetail.CREATE_LOGIN_ID}, ");
                sql.Append($"a.{ColumnNameCompDetail.UPDATE_DATE}, ");
                sql.Append($"a.{ColumnNameCompDetail.UPDATE_TIME}, ");
                sql.Append($"a.{ColumnNameCompDetail.UPDATE_LOGIN_ID} ");
                sql.Append($"FROM ");
                sql.Append($"{tableName} a ");
                sql.Append($"WHERE ");
                sql.Append($"a.{ColumnNameCompDetail.ORDER_DATE} = '{orderDateTime.ToString(DateFormat)}' ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompDetail.POST_NO} = '{postNo}' ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompDetail.WORK_CODE} = '{workCode}' ");
                sql.Append($"AND ");
                sql.Append($"NOT EXISTS ");
                sql.Append($"( ");
                sql.Append($"SELECT ");
                sql.Append($"b.* ");
                sql.Append($"FROM ");
                sql.Append($"{tableName} b ");
                sql.Append($"WHERE ");
                sql.Append($"a.{ColumnNameCompDetail.ORDER_DATE} = b.{ColumnNameCompDetail.ORDER_DATE} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompDetail.POST_NO} = b.{ColumnNameCompDetail.POST_NO} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompDetail.ORDER_DATE_REQUEST} = b.{ColumnNameCompDetail.ORDER_DATE_REQUEST} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompDetail.POST_NO_REQUEST} = b.{ColumnNameCompDetail.POST_NO_REQUEST} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompDetail.WORK_CODE} = b.{ColumnNameCompDetail.WORK_CODE} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompDetail.INDEX} < b.{ColumnNameCompDetail.INDEX} ");
                sql.Append($") ");
                sql.Append($"ORDER BY ");
                sql.Append($"a.{ColumnNameCompDetail.ORDER_DATE}, ");
                sql.Append($"a.{ColumnNameCompDetail.POST_NO}, ");
                sql.Append($"a.{ColumnNameCompDetail.ORDER_DATE_REQUEST}, ");
                sql.Append($"a.{ColumnNameCompDetail.POST_NO_REQUEST}, ");
                sql.Append($"a.{ColumnNameCompDetail.WORK_CODE}, ");
                sql.Append($"a.{ColumnNameCompDetail.INDEX}");
                using (var dbAccess = new DBAccess())
                {
                    rc = dbAccess.ExecuteReader(connectionString, sql.ToString(), out var dataList, retryCount);
                    if (rc == (UInt32)ErrorCodeList.STATUS_SUCCESS)
                    {
                        if (dataList != null && dataList.Count > 0)
                        {
                            itemList = new List<ExecuteStoreData>();
                            foreach (var data in dataList)
                            {
                                DataToExecuteStoreItem(data, out var executeStoreData);
                                itemList.Add(executeStoreData);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"OrderInfo_IF_DB.GetExecStoreItems : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 店別小仕分け実績テーブル
        /// 1行読み出し
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="orderDateTime">仕分納品日</param>
        /// <param name="postNo">仕分便No</param>
        /// <param name="workCode">商品コード</param>
        /// <param name="storeCode">店コード</param>
        /// <param name="item">1商品分のデータリスト</param>
        /// <returns>エラーコード</returns>
        public UInt32 GetExecStoreItems(string tableName, DateTime orderDateTime, int postNo, string workCode, string storeCode, out ExecuteStoreData item)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"OrderInfo_IF_DB.GetExecStoreItems({tableName}, {orderDateTime}, {postNo}, {workCode})");
            item = null;
            try
            {
                var sql = new StringBuilder();
                sql.Append($"SELECT ");
                sql.Append($"a.{ColumnNameCompDetail.ORDER_DATE}, ");
                sql.Append($"a.{ColumnNameCompDetail.POST_NO}, ");
                sql.Append($"a.{ColumnNameCompDetail.ORDER_DATE_REQUEST}, ");
                sql.Append($"a.{ColumnNameCompDetail.POST_NO_REQUEST}, ");
                sql.Append($"a.{ColumnNameCompDetail.WORK_CODE}, ");
                sql.Append($"a.{ColumnNameCompDetail.INDEX}, ");
                sql.Append($"a.{ColumnNameCompDetail.STORE_CODE}, ");
                sql.Append($"a.{ColumnNameCompDetail.STATION_NO}, ");
                sql.Append($"a.{ColumnNameCompDetail.AISLE_NO}, ");
                sql.Append($"a.{ColumnNameCompDetail.SLOT_NO}, ");
                sql.Append($"a.{ColumnNameCompDetail.ORDER_COUNT}, ");
                sql.Append($"a.{ColumnNameCompDetail.ORDER_COMP_COUNT}, ");
                sql.Append($"a.{ColumnNameCompDetail.CREATE_DATE}, ");
                sql.Append($"a.{ColumnNameCompDetail.CREATE_TIME}, ");
                sql.Append($"a.{ColumnNameCompDetail.CREATE_LOGIN_ID}, ");
                sql.Append($"a.{ColumnNameCompDetail.UPDATE_DATE}, ");
                sql.Append($"a.{ColumnNameCompDetail.UPDATE_TIME}, ");
                sql.Append($"a.{ColumnNameCompDetail.UPDATE_LOGIN_ID} ");
                sql.Append($"FROM ");
                sql.Append($"{tableName} a ");
                sql.Append($"WHERE ");
                sql.Append($"a.{ColumnNameCompDetail.ORDER_DATE} = '{orderDateTime.ToString(DateFormat)}' ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompDetail.POST_NO} = '{postNo}' ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompDetail.WORK_CODE} = '{workCode}' ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompDetail.STORE_CODE} = '{storeCode}' ");
                sql.Append($"AND ");
                sql.Append($"NOT EXISTS ");
                sql.Append($"( ");
                sql.Append($"SELECT ");
                sql.Append($"b.* ");
                sql.Append($"FROM ");
                sql.Append($"{tableName} b ");
                sql.Append($"WHERE ");
                sql.Append($"a.{ColumnNameCompDetail.ORDER_DATE} = b.{ColumnNameCompDetail.ORDER_DATE} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompDetail.POST_NO} = b.{ColumnNameCompDetail.POST_NO} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompDetail.ORDER_DATE_REQUEST} = b.{ColumnNameCompDetail.ORDER_DATE_REQUEST} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompDetail.POST_NO_REQUEST} = b.{ColumnNameCompDetail.POST_NO_REQUEST} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompDetail.WORK_CODE} = b.{ColumnNameCompDetail.WORK_CODE} ");
                sql.Append($"AND ");
                sql.Append($"a.{ColumnNameCompDetail.INDEX} < b.{ColumnNameCompDetail.INDEX} ");
                sql.Append($") ");
                sql.Append($"ORDER BY ");
                sql.Append($"a.{ColumnNameCompDetail.ORDER_DATE}, ");
                sql.Append($"a.{ColumnNameCompDetail.POST_NO}, ");
                sql.Append($"a.{ColumnNameCompDetail.ORDER_DATE_REQUEST}, ");
                sql.Append($"a.{ColumnNameCompDetail.POST_NO_REQUEST}, ");
                sql.Append($"a.{ColumnNameCompDetail.WORK_CODE}, ");
                sql.Append($"a.{ColumnNameCompDetail.INDEX}");
                using (var dbAccess = new DBAccess())
                {
                    rc = dbAccess.ExecuteReader(connectionString, sql.ToString(), out var dataList, retryCount);
                    if (rc == (UInt32)ErrorCodeList.STATUS_SUCCESS)
                    {
                        if (dataList != null && dataList.Count > 0)
                            DataToExecuteStoreItem(dataList[0], out item);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"OrderInfo_IF_DB.GetExecStoreItems : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 店別小仕分け実績テーブル
        /// 1行書込み
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="item">1行データ</param>/// <returns>エラーコード</returns>
        public UInt32 SetExecStoreItems(string tableName, ExecuteStoreData item, string updateLoginId)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"OrderInfo_IF_DB.SetExecStoreItems({tableName})");
            try
            {

                var now = DateTime.Now;
                // Postgresqlのバージョンが8.4以降の場合はWITH文が使用できる
                //var sql = GetExecuteWithSql(tableName, now, item);
                //using (var dbAccess = new DBAccess())
                //{
                //    rc = dbAccess.ExecuteNonSQL(connectionString, sql.ToString(), retryCount);
                //}

                // Postgresqlのバージョンが8.3以前の場合は、SELECT→INSERT or UPDATEの2回実行する必要がある
                // SELECT文
                var selectSql = new StringBuilder();
                selectSql.Append($"SELECT ");
                selectSql.Append($"{ColumnNameCompDetail.ORDER_DATE} ");
                selectSql.Append($"FROM ");
                selectSql.Append($"{tableName} ");
                selectSql.Append($"WHERE ");
                selectSql.Append($"{ColumnNameCompDetail.ORDER_DATE} = '{item.orderDate.ToString(DateFormat)}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNameCompDetail.POST_NO} = '{item.postNo}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNameCompDetail.ORDER_DATE_REQUEST} = '{item.orderDateRequest.ToString(DateFormat)}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNameCompDetail.POST_NO_REQUEST} = '{item.postNoRequest}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNameCompDetail.WORK_CODE} = '{item.workCode}' ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNameCompDetail.INDEX} = {item.index} ");
                selectSql.Append($"AND ");
                selectSql.Append($"{ColumnNameCompDetail.STORE_CODE} = '{item.storeCode}' ");
                // INSERT文
                var insertSql = new StringBuilder();
                insertSql.Append($"INSERT INTO {tableName} ");
                insertSql.Append($"( ");
                insertSql.Append($"{ColumnNameCompDetail.ORDER_DATE}, ");
                insertSql.Append($"{ColumnNameCompDetail.POST_NO}, ");
                insertSql.Append($"{ColumnNameCompDetail.ORDER_DATE_REQUEST}, ");
                insertSql.Append($"{ColumnNameCompDetail.POST_NO_REQUEST}, ");
                insertSql.Append($"{ColumnNameCompDetail.WORK_CODE}, ");
                insertSql.Append($"{ColumnNameCompDetail.INDEX}, ");
                insertSql.Append($"{ColumnNameCompDetail.STORE_CODE}, ");
                insertSql.Append($"{ColumnNameCompDetail.STATION_NO}, ");
                insertSql.Append($"{ColumnNameCompDetail.AISLE_NO}, ");
                insertSql.Append($"{ColumnNameCompDetail.SLOT_NO}, ");
                insertSql.Append($"{ColumnNameCompDetail.ORDER_COUNT}, ");
                insertSql.Append($"{ColumnNameCompDetail.ORDER_COMP_COUNT}, ");
                insertSql.Append($"{ColumnNameCompDetail.CREATE_DATE}, ");
                insertSql.Append($"{ColumnNameCompDetail.CREATE_TIME}, ");
                insertSql.Append($"{ColumnNameCompDetail.CREATE_LOGIN_ID} ");
                insertSql.Append($") VALUES ( ");
                insertSql.Append($"'{item.orderDate.ToString(DateFormat)}', ");
                insertSql.Append($"'{item.postNo}', ");
                insertSql.Append($"'{item.orderDateRequest.ToString(DateFormat)}', ");
                insertSql.Append($"'{item.postNoRequest}', ");
                insertSql.Append($"'{item.workCode}', ");
                insertSql.Append($"{item.index}, ");
                insertSql.Append($"'{item.storeCode}', ");
                insertSql.Append($"'{item.stationNo}', ");
                insertSql.Append($"'{item.aisleNo}', ");
                insertSql.Append($"'{item.slotNo}', ");
                insertSql.Append($"'{item.orderCount}', ");
                insertSql.Append($"'{item.orderCompCount}', ");
                insertSql.Append($"'{now.ToString(DateFormat)}', ");
                insertSql.Append($"'{now.ToString(TimeFormat)}', ");
                insertSql.Append($"'{item.createLoginId}' ");
                insertSql.Append($") ");
                // UPDATE文
                var updateSql = new StringBuilder();
                updateSql.Append($"UPDATE {tableName} ");
                updateSql.Append($"SET ");
                updateSql.Append($"{ColumnNameCompDetail.ORDER_COUNT} = {item.orderCount}, ");
                updateSql.Append($"{ColumnNameCompDetail.ORDER_COMP_COUNT} = {item.orderCompCount}, ");
                updateSql.Append($"{ColumnNameCompDetail.UPDATE_DATE} = '{now.ToString(DateFormat)}', ");
                updateSql.Append($"{ColumnNameCompDetail.UPDATE_TIME} = '{now.ToString(TimeFormat)}', ");
                //updateSql.Append($"{ColumnNameCompDetail.UPDATE_LOGIN_ID} = '{item.updateLoginId}' ");
                updateSql.Append($"{ColumnNameCompDetail.UPDATE_LOGIN_ID} = '{updateLoginId}' ");
                updateSql.Append($"WHERE ");
                updateSql.Append($"{ColumnNameCompDetail.ORDER_DATE} = '{item.orderDate.ToString(DateFormat)}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNameCompDetail.POST_NO} = '{item.postNo}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNameCompDetail.ORDER_DATE_REQUEST} = '{item.orderDateRequest.ToString(DateFormat)}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNameCompDetail.POST_NO_REQUEST} = '{item.postNoRequest}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNameCompDetail.WORK_CODE} = '{item.workCode}' ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNameCompDetail.INDEX} = {item.index} ");
                updateSql.Append($"AND ");
                updateSql.Append($"{ColumnNameCompDetail.STORE_CODE} = '{item.storeCode}' ");
                using (var dbAccess = new DBAccess())
                {
                    //throw new Exception("エラーが発生しました");
                    rc = dbAccess.Upsert(connectionString, selectSql.ToString(), insertSql.ToString(), updateSql.ToString(), retryCount);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"OrderInfo_IF_DB.SetExecStoreItems : {(ErrorCodeList)rc}");
            return rc;
        }


        #region 読み込みデータを仕分完了店別小仕分けに変換
        /// <summary>
        /// 読み込みデータを仕分完了店別小仕分けに変換
        /// </summary>
        /// <param name="data">読み込みデータ</param>
        /// <param name="orderData">仕分完了商品ヘッダデータ</param>
        private void DataToExecuteStoreItem(Dictionary<string, object> data, out ExecuteStoreData executeStoreData)
        {
            try
            {
                var createDateTime = ConvertDateTime(data[ColumnNameCompDetail.CREATE_DATE], data[ColumnNameCompDetail.CREATE_TIME]);
                var updateDateTime = ConvertDateTime(data[ColumnNameCompDetail.UPDATE_DATE], data[ColumnNameCompDetail.UPDATE_TIME]);
                executeStoreData = new ExecuteStoreData
                {
                    orderDate = DateTime.ParseExact(data[ColumnNameCompDetail.ORDER_DATE].ToString(), DateFormat, null),
                    postNo = Convert.ToInt32(data[ColumnNameCompDetail.POST_NO]),
                    orderDateRequest = DateTime.ParseExact(data[ColumnNameCompDetail.ORDER_DATE_REQUEST].ToString(), DateFormat, null),
                    postNoRequest = Convert.ToInt32(data[ColumnNameCompDetail.POST_NO_REQUEST]),
                    workCode = data[ColumnNameCompDetail.WORK_CODE].ToString(),
                    index = Convert.ToInt32(data[ColumnNameCompDetail.INDEX]),
                    storeCode = data[ColumnNameCompDetail.STORE_CODE].ToString(),
                    stationNo = Convert.ToInt32(data[ColumnNameCompDetail.STATION_NO]),
                    aisleNo = Convert.ToInt32(data[ColumnNameCompDetail.AISLE_NO]),
                    slotNo = Convert.ToInt32(data[ColumnNameCompDetail.SLOT_NO]),
                    orderCount = Convert.ToDouble(data[ColumnNameCompDetail.ORDER_COUNT]),
                    orderCompCount = Convert.ToDouble(data[ColumnNameCompDetail.ORDER_COMP_COUNT]),
                    createDateTime = createDateTime,
                    createLoginId = data[ColumnNameCompDetail.CREATE_LOGIN_ID].ToString(),
                    updateDateTime = updateDateTime,
                    updateLoginId = data[ColumnNameCompDetail.UPDATE_LOGIN_ID].ToString()
                };
            }
            catch (Exception)
            {
                executeStoreData = null;
                throw;
            }
        }
        #endregion
        #endregion
    }
}
