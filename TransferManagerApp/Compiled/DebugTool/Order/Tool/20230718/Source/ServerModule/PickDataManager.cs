using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

using DL_DBAccess;
using ErrorCodeDefine;
using System.Net;
using System.Collections;


namespace Server
{
    /// <summary>
    /// 上位サーバーの仕分けテーブルデータの管理
    /// ※データはDBかCSVか未定。このクラスの下でDBとCSVの処理を切り替え
    /// </summary>
    public class PickDataManager
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
        /// 商品ヘッダデータ一覧
        /// </summary>
        public List<WorkHeader> WorkHeaderList = new List<WorkHeader>();

        /// <summary>
        /// 店別小仕分けデータ一覧
        /// </summary>
        public List<StorePick> StorePickList = new List<StorePick>();



        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PickDataManager(string ipAddress, string connectionStr)
        {
            try
            {
                _ipAddress = ipAddress;
                _connectionString = connectionStr;
            }
            catch (Exception ex) 
            {
            }
        }

        #region DBAccessインスタンス作成
        /// <summary>
        /// DBAccessインスタンス作成
        /// </summary>
        /// <returns>DBAccessインスタンス</returns>
        private DBAccess CreateDBAccess()
        {
            var result = new DBAccess(_connectionString);
            //if (ConnectionIpAddress != "")
            //    result.ConnectionIpAddress = ConnectionIpAddress;

            return result;
        }
        #endregion




        /// <summary>
        /// データ更新確認
        /// おそらく登録日付をチェックする??
        /// </summary>
        /// <returns></returns>
        public bool IsUpdatedTable() 
        {
            bool updated = false;
            try
            {
            }
            catch (Exception ex) 
            {
            }
            return updated;
        }



        #region 商品ヘッダ
        /// <summary>
        /// 商品ヘッダデータ 読み出し
        /// </summary>
        /// <returns>エラーコード</returns>
        public uint ReadWorkheaderData()
        {
            UInt32 rc = 0;
            try
            {
                string tableName = "sample01";

                // 実行するSQL
                var sql = $"SELECT * FROM {tableName}";

                var dbAccess = CreateDBAccess();
                var result = dbAccess.ExecuteReaderSQL(sql, out List<Dictionary<string, object>> dataList);

                WorkHeaderList.Clear();
                foreach (var data in dataList) 
                {
                    WorkHeader workInfo = new WorkHeader();
                    workInfo.Set(data);
                    WorkHeaderList.Add(workInfo);
                }
            }
            catch (Exception ex)
            {

                rc = (uint)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        #endregion

        #region 店別小仕分け
        /// <summary>
        /// 店別小仕分けデータ 読み出し
        /// </summary>
        /// <returns>エラーコード</returns>
        public uint StoreInfo_ReadOneLine()
        {
            UInt32 rc = 0;
            try
            {

            }
            catch (Exception ex)
            {

                rc = (uint)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        #endregion


        #region 商品ヘッダ 実績
        /// <summary>
        /// 商品ヘッダデータ 読み出し
        /// </summary>
        /// <returns>エラーコード</returns>
        public uint WriteWorkheaderResult()
        {
            UInt32 rc = 0;
            try
            {
                string tableName = "sample01";

                // 実行するSQL
                var sql = $"SELECT * FROM {tableName}";

                var dbAccess = CreateDBAccess();
                var result = dbAccess.ExecuteReaderSQL(sql, out List<Dictionary<string, object>> dataList);

                WorkHeaderList.Clear();
                foreach (var data in dataList)
                {
                    WorkHeader workInfo = new WorkHeader();
                    workInfo.Set(data);
                    WorkHeaderList.Add(workInfo);
                }
            }
            catch (Exception ex)
            {

                rc = (uint)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        #endregion


        #region 店別小仕分け 実績
        /// <summary>
        /// 店別小仕分けデータ 読み出し
        /// </summary>
        /// <returns>エラーコード</returns>
        public uint WriteStorePickResult()
        {
            UInt32 rc = 0;
            try
            {

            }
            catch (Exception ex)
            {

                rc = (uint)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        #endregion



        #region ソート&抽出
        /// <summary>
        /// 1商品の仕分情報を取得
        /// </summary>
        /// <returns>エラーコード</returns>
        public UInt32 GetWorkPostInfo(int deliveryDate, int deliveryNo, int batchNo, int aisleNo, string workCode, out WorkSlotInfo[] workPickInfo)
        {
            UInt32 rc = 0;
            workPickInfo = null;
            try
            {
                var l = StorePickList.Where(n => n.pickDate == deliveryDate && n.aisleNo == aisleNo && n.workCode == workCode);
                int count = l.Count();
                workPickInfo = new WorkSlotInfo[count];




            }
            catch (Exception ex)
            {

                rc = (uint)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }


        #endregion


    }
}
