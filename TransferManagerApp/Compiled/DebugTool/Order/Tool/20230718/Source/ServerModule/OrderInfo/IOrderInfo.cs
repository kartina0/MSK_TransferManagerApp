﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemConfig;

namespace ServerModule
{

    public interface IOrderInfo
    {
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
        UInt32 GetOrderItemList(string tableName, DateTime orderDateTime, int postNo, Const.ORDER_PROCESS process, out List<OrderData> itemList);
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
        UInt32 GetOrderItem(string tableName, DateTime orderDateTime, int postNo, string workCode, Const.ORDER_PROCESS process, out OrderData item);
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
        /// <param name="process">仕分け作業状況</param>
        /// <param name="index">連番</param>
        /// <param name="updateLoginId">更新ログインID</param>
        /// <returns>エラーコード</returns>
        UInt32 SetOrderItem(string tableName, DateTime orderDateTime, int postNo, DateTime orderDateRequest, int postNoRequest, string workCode, int index, Const.ORDER_PROCESS process, string updateLoginId);
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
        UInt32 GetOrderStoreItem(string tableName, DateTime orderDateTime, int postNo, string workCode, out List<OrderStoreData> itemList);
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
        UInt32 GetExecuteItem(string tableName, DateTime orderDateTime, int postNo, string workCode, out ExecuteData item);
        /// <summary>
        /// 商品ヘッダ実績テーブル
        /// 1行書込み
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="item">1行データ</param>
        /// <returns>エラーコード</returns>
        UInt32 SetExecuteItem(string tableName, ExecuteData item);
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
        UInt32 GetExecStoreItems(string tableName, DateTime orderDateTime, int postNo, string workCode, out List<ExecuteStoreData> itemList);
        /// <summary>
        /// 店別小仕分け実績テーブル
        /// 1行書込み
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="item">1行データ</param>
        /// <returns>エラーコード</returns>
        UInt32 SetExecStoreItems(string tableName, ExecuteStoreData item);
        #endregion

    }
}
