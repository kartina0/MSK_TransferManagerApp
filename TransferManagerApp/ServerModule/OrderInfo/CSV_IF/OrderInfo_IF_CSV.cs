﻿//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

using SystemConfig;
using DL_Logger;
using ErrorCodeDefine;


namespace ServerModule
{
    /// <summary>
    /// 仕分データインターフェース CSV
    /// </summary>
    public class OrderInfo_IF_CSV : IOrderInfo
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "OrderInfo_IF_CSV";

        /// <summary>
        /// 仕分データCSVファイルの格納ディレクトリ
        /// </summary>
        private string _dir = "..\\Compiled\\Order\\CSV";


        /// <summary>
        /// CSVファイルデータ 商品ヘッダ
        /// </summary>
        private List<string[]> _workHeaderList = new List<string[]>();
        /// <summary>
        /// CSVファイルデータ 店別小仕分け
        /// </summary>
        private List<string[]> _storeOrderList = new List<string[]>();

        /// <summary>
        /// 読み出し済フラグ 商品ヘッダ
        /// </summary>
        private bool _workHeaderLoadedFlg = false;
        /// <summary>
        /// 読み出し済フラグ 店別小仕分け
        /// </summary>
        private bool _storeOrderLoadedFlg = false;



        #region 商品ヘッダテーブル
        /// <summary>
        /// 商品ヘッダテーブル
        /// 1便分を読み出し
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
            itemList = null;
            try
            {



            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
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
        public UInt32 GetOrderItem(string tableName, DateTime orderDateTime, int postNo, string workCode, ORDER_PROCESS process, out OrderData item)
        {
            UInt32 rc = 0;
            item = null;
            try
            {
                // 既に読み出し済かどうか
                if (!_workHeaderLoadedFlg) 
                {
                    // CSVファイルを全て読み出し
                    string filePath = _dir + "\\" + tableName + ".csv";
                    StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("SHIFT-JIS"));
                    {
                        // 末尾まで繰り返す
                        while (!sr.EndOfStream)
                        {
                            // CSVファイルの一行を読み込む
                            string line = sr.ReadLine();
                            // 読み込んだ一行をカンマ毎に分けて配列に格納する
                            string[] values = line.Split(',');
                            // リストに格納する
                            _workHeaderList.Add(values);
                        }

                        _workHeaderLoadedFlg = true;
                    }
                }


                // 該当するデータを検索して抽出
                string orderDt = $"{orderDateTime.Year.ToString("D4")}{orderDateTime.Month.ToString("D2")}{orderDateTime.Day.ToString("D2")}";
                string[] data = _workHeaderList.Find(arr => arr[0] == orderDt && arr[1] == postNo.ToString() && arr[4] == workCode && arr[13] == ((int)process).ToString());

                if (data != null) 
                {// 該当する商品データがあったら

                    //  クラスに変換
                    int i = 0;
                    item = new OrderData();

                    item.orderDate = DateTime.ParseExact($"{data[i++]}000000", "yyyyMMddHHmmss", null);
                    item.postNo = int.Parse(data[i++]);
                    item.orderDateRequest = DateTime.ParseExact($"{data[i++]}000000", "yyyyMMddHHmmss", null);
                    item.postNoRequest = int.Parse(data[i++]);
                    item.workCode = data[i++];
                    item.index = int.Parse(data[i++]);
                    item.workName = data[i++];
                    item.JANCode = data[i++];
                    item.caseVolume = double.Parse(data[i++]);
                    item.orderCountTotal = double.Parse(data[i++]);
                    item.workNameKana = data[i++];
                    item.maxStackNum = int.Parse(data[i++]);
                    item.salesPrice = double.Parse(data[i++]);
                    for (int j = 0; j < 9; j++) 
                        item.orderCount[j] = double.Parse(data[i++]);
                    for (int j = 0; j < 9; j++)
                        item.storeCount[j] = int.Parse(data[i++]);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// 商品ヘッダテーブル
        /// 1商品の 仕分作業状況/更新日付/更新時刻/更新ログインID を更新
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
            try
            {



            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
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



            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
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
        /// <param name="itemList">1商品分のデータリスト</param>
        /// <returns>エラーコード</returns>
        public UInt32 GetExecuteItem(string tableName, DateTime orderDateTime, int postNo, out List<ExecuteData> itemList)
        {
            UInt32 rc = 0;
            itemList = null;
            try
            {



            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
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
            item = null;
            try
            {



            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
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
        public UInt32 SetExecuteItem(string tableName, ExecuteData item, string updateLoginId)
        {
            UInt32 rc = 0;
            item = null;
            try
            {



            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
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
        /// <param name="itemList">1商品分のデータリスト</param>
        /// <returns>エラーコード</returns>
        public UInt32 GetExecStoreItems(string tableName, DateTime orderDateTime, int postNo, string workCode, out List<ExecuteStoreData> itemList)
        {
            UInt32 rc = 0;
            itemList = null;
            try
            {



            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// 店別小仕分け実績テーブル
        /// 1商品分読み出し
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
            item = null;
            try
            {



            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// 店別小仕分け実績テーブル
        /// 1行書込み
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="item">1行データ</param>
        /// <returns>エラーコード</returns>
        public UInt32 SetExecStoreItems(string tableName, ExecuteStoreData item, string updateLoginId)
        {
            UInt32 rc = 0;
            item = null;
            try
            {



            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        #endregion
    }
}
