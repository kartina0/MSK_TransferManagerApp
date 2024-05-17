using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections;
using Npgsql;

using SystemConfig;
using ErrorCodeDefine;


namespace ServerModule
{
    /// <summary>
    /// 仕分データ管理クラス
    /// ※データはDBかCSVか未定。このクラスの下でDBとCSVの処理を切り替え
    /// </summary>
    public class OrderInfoManager : IOrderInfo
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "DB";

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
        /// </summary>
        public List<OrderData> OrderDataList = new List<OrderData>();


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OrderInfoManager()
        {
            try
            {
                _ipAddress = IniFile.ServerIpAddress;
                _connectionString = IniFile.DB_SQL_Connection;

                if (IniFile.OrderInfoType == Const.ORDER_INFO_TYPE.DB)
                    OrderData_IF = new OrderInfo_IF_DB();
                else if (IniFile.OrderInfoType == Const.ORDER_INFO_TYPE.CSV)
                    OrderData_IF = new OrderInfo_IF_CSV();
            }
            catch (Exception ex) 
            {
            }
        }




        #region 商品ヘッダテーブル
        /// <summary>
        /// 商品ヘッダテーブル
        /// 1商品分を読み出し
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="orderDateTime">仕分納品日</param>
        /// <param name="postNo">仕分便No</param>
        /// <param name="process">仕分け作業状況</param>
        /// <param name="itemList">1便分のデータリスト</param>
        /// <returns>エラーコード</returns>
        public UInt32 GetOrderItemList(string tableName, DateTime orderDateTime, int postNo, Const.ORDER_PROCESS process, out List<OrderData> itemList)
        {
            UInt32 rc = 0;
            itemList = null;
            try
            {

                rc = OrderData_IF.GetOrderItemList(tableName, orderDateTime, postNo, process, out itemList);

            }
            catch (Exception ex)
            {

            }
            return rc;
        }
        /// <summary>
        /// 商品ヘッダテーブル
        /// 1商品分を読み出し
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="orderDateTime">仕分納品日</param>
        /// <param name="postNo">仕分便No</param>
        /// <param name="workCode">商品コード</param>
        /// <param name="process">仕分け作業状況</param>
        /// <param name="item">1行データ</param>
        /// <returns>エラーコード</returns>
        public UInt32 GetOrderItem(string tableName, DateTime orderDateTime, int postNo, string workCode, Const.ORDER_PROCESS process, out OrderData item)
        {
            UInt32 rc = 0;
            item = null;
            try
            {

                rc = OrderData_IF.GetOrderItem(tableName, orderDateTime, postNo, workCode, process, out item);

            }
            catch (Exception ex)
            {

            }
            return rc;
        }
        /// <summary>
        /// 商品ヘッダテーブル
        /// 読み出した1商品の仕分作業状況、更新日時を更新
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
        public UInt32 SetOrderItem(string tableName, DateTime orderDateTime, int postNo, DateTime orderDateRequest, int postNoRequest, string workCode, int index, Const.ORDER_PROCESS process, string updateLoginId)
        {
            UInt32 rc = 0;
            try
            {

                rc = OrderData_IF.SetOrderItem(tableName, orderDateTime, postNo, orderDateRequest, postNoRequest, workCode, index, process, updateLoginId);

            }
            catch (Exception ex)
            {

            }
            return rc;
        }
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
            itemList = null;
            try
            {

                rc = OrderData_IF.GetOrderStoreItem(tableName, orderDateTime, postNo, workCode, out itemList);

            }
            catch (Exception ex)
            {

            }
            return rc;
        }
        #endregion



        #region 商品ヘッダ実績テーブル
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
            item = null;
            try
            {

                rc = OrderData_IF.GetExecuteItem(tableName, orderDateTime, postNo, workCode, out item);

            }
            catch (Exception ex)
            {

            }
            return rc;
        }
        /// <summary>
        /// 商品ヘッダ実績テーブル
        /// 1行書込み
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="item">1行データ</param>
        /// <returns>エラーコード</returns>
        public UInt32 SetExecuteItem(string tableName, ExecuteData item)
        {
            UInt32 rc = 0;
            try
            {

                rc = OrderData_IF.SetExecuteItem(tableName, item);

            }
            catch (Exception ex)
            {

            }
            return rc;
        }
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
        /// <param name="itemList">1商品分のデータリスト</param>/// <returns>エラーコード</returns>
        public UInt32 GetExecStoreItems(string tableName, DateTime orderDateTime, int postNo, string workCode, out List<ExecuteStoreData> itemList)
        {
            UInt32 rc = 0;
            itemList = null;
            try
            {

                rc = OrderData_IF.GetExecStoreItems(tableName, orderDateTime, postNo, workCode, out itemList);

            }
            catch (Exception ex)
            {

            }
            return rc;
        }
        /// <summary>
        /// 店別小仕分け実績テーブル
        /// 1行書込み
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="item">1行データ</param>/// <returns>エラーコード</returns>
        public UInt32 SetExecStoreItems(string tableName, ExecuteStoreData item)
        {
            UInt32 rc = 0;
            try
            {

                rc = OrderData_IF.SetExecStoreItems(tableName, item);

            }
            catch (Exception ex)
            {

            }
            return rc;
        }
        #endregion



    }

}
