using ErrorCodeDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.PickData
{
    interface IPickData
    {
        #region 商品ヘッダテーブル
        /// <summary>
        /// 更新確認
        /// おそらく登録日付をチェックする??
        /// </summary>
        /// <returns></returns>
        bool Workheader_IsUpdated();
        /// <summary>
        /// 商品ヘッダテーブル
        /// 1商品分を読み出し
        /// </summary>
        /// <returns>エラーコード</returns>
        UInt32 Workheader_ReadOneWork(DateTime date, string workCode);
        /// <summary>
        /// 商品ヘッダテーブル
        /// 読み出した1商品の仕分作業状況、更新日時を更新
        /// </summary>
        /// <returns>エラーコード</returns>
        UInt32 Workheader_UpdateOneWork();
        #endregion

        #region 店別小仕分けテーブル
        /// <summary>
        /// データ更新確認
        /// おそらく登録日付をチェックする??
        /// </summary>
        /// <returns></returns>
        bool StoreInfo_IsUpdated();
        /// <summary>
        /// 店別小仕分けテーブル
        /// 1行読み出し
        /// </summary>
        /// <returns>エラーコード</returns>
        UInt32 StoreInfo_ReadOneWork();
        /// <summary>
        /// 店別小仕分けテーブル
        /// 読み出した1行の仕分作業状況、更新日時を更新
        /// </summary>
        /// <returns>エラーコード</returns>
        UInt32 StoreInfo_UpdateOneWork();
        #endregion



        #region 商品ヘッダ実績テーブル
        /// <summary>
        /// 商品ヘッダ実績テーブル
        /// 1行読み出し
        /// </summary>
        /// <returns>エラーコード</returns>
        UInt32 WorkheaderResult_ReadOneWork();
        /// <summary>
        /// 商品ヘッダ実績テーブル
        /// 1行書込み
        /// </summary>
        /// <returns>エラーコード</returns>
        UInt32 WorkheaderResult_WriteOneLine();
        #endregion

        #region 店別小仕分け実績テーブル
        /// <summary>
        /// 店別小仕分け実績テーブル
        /// 1行読み出し
        /// </summary>
        /// <returns>エラーコード</returns>
        UInt32 StoreInfoResult_ReadOneWork();
        /// <summary>
        /// 店別小仕分け実績テーブル
        /// 1行書込み
        /// </summary>
        /// <returns>エラーコード</returns>
        UInt32 StoreInfoResult_WriteOneLine();
        #endregion


    }
}
