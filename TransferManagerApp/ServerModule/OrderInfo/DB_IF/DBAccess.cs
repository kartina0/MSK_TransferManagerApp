//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;

using DL_Logger;
using ErrorCodeDefine;


namespace ServerModule
{
    #region DBアクセスクラス
    /// <summary>
    /// DBアクセスクラス
    /// </summary>
    public class DBAccess : IDisposable
    {
        private const string THIS_NAME = "DBAccess";


        #region 定数
        /// <summary>
        /// リトライ回数既定値
        /// </summary>
        public const int DEFAULT_RETRY_CCOUNT = 3;
        #endregion

        #region 変数
        /// <summary>
        /// 接続用変数
        /// </summary>
        private NpgsqlConnection connection;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DBAccess()
        {
        }
        #endregion

        #region 破棄
        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            Close();
        }
        #endregion

        #region コネクションオープン
        /// <summary>
        /// コネクションオープン
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        public void Open(string connectionString)
        {
            //Logger.WriteLog(LogType.METHOD_OUT, $"DBAccess Open({connectionString})");

            try
            {
                connection = new NpgsqlConnection(connectionString);

                switch(connection.State)
                {
                    case System.Data.ConnectionState.Closed:
                        connection.Open();
                        break;
                    case System.Data.ConnectionState.Broken:
                        connection.Close();
                        connection.Open();
                        break;
                    default:
                        break;
                }
            }
            catch(Exception)
            {
                if (connection != null)
                    try { connection.Dispose(); } catch { }
                throw;
            }

            //Logger.WriteLog(LogType.METHOD_OUT, $"DBAccess Open");
        }
        #endregion

        #region コネクションクローズ
        /// <summary>
        /// コネクションクローズ
        /// </summary>
        public void Close()
        {
            //Logger.WriteLog(LogType.METHOD_IN, $"DBAccess Close");

            try
            {
                if (connection != null)
                {
                    if (connection.State != System.Data.ConnectionState.Closed)
                    {
                        try { connection.Close(); } catch { }
                        try { connection.Dispose(); } catch { }
                        connection = null;
                    }
                }
            }
            catch { }
            
            //Logger.WriteLog(LogType.METHOD_OUT, $"DBAccess Close");
        }
        #endregion

        #region DB接続・切断
        /// <summary>
        /// DB接続・切断
        /// ※接続がプールされるため、ネットワーク異常・DB切換えした際に1回接続に失敗する
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        public uint OpenClose(string connectionString)
        {
            //Logger.WriteLog(LogType.METHOD_IN, $"DBAccess OpenClose({connectionString})");

            var result = (uint)0;

            try
            {
                // DB接続
                Open(connectionString);

                try
                {
                    using (var command = new NpgsqlCommand("SELECT version()"))
                    {
                        command.Connection = connection;
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    result = (uint)ErrorCodeList.DB_READ_WRITE_ERROR;
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                }
                finally
                {
                    // DB切断
                    Close();
                }
            }
            catch (Exception ex)
            {
                result = (uint)ErrorCodeList.DB_OPEN_ERROR;
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
            }

            //Logger.WriteLog(LogType.METHOD_OUT, $"DBAccess OpenClose : {(ErrorCodeList)result}");

            return result;
        }
        #endregion

        #region コマンドを実行し、読み込みデータ一覧を取得する
        /// <summary>
        /// コマンドを実行し、読み込みデータ一覧を取得する
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        /// <param name="sql">SQL文</param>
        /// <param name="dataList">読み込みデータ辞書一覧</param>
        /// <param name="retryCount">リトライ回数</param>
        /// <returns>エラーコード</returns>
        public uint ExecuteReader(string connectionString, string sql, out List<Dictionary<string, object>> dataList, int retryCount = DEFAULT_RETRY_CCOUNT)
        {
            //Logger.WriteLog(LogType.METHOD_IN, $"DBAccess ExecuteReader({connectionString}, {sql}, out dataList, {retryCount})");

            var result = (uint)0;
            dataList = new List<Dictionary<string, object>>();

            for (var i = 0; i <= retryCount; i++)
            {
                try
                {
                    // DB接続
                    Open(connectionString);

                    result = ExecuteReader(sql, out dataList, 0);
                }
                catch (Exception ex)
                {
                    result = (uint)ErrorCodeList.DB_OPEN_ERROR;
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                }
                finally
                {
                    // DB切断
                    Close();
                }

                if (result == (uint)ErrorCodeList.STATUS_SUCCESS)
                    break;

                if (retryCount > 0 && i < retryCount)
                {
                    Logger.WriteLog(LogType.DEBUG, $"ExecuteReader() Retry {i + 1}/{retryCount}");
                    Thread.Sleep(100);
                }
            }

            //Logger.WriteLog(LogType.METHOD_OUT, $"DBAccess ExecuteReader : {(ErrorCodeList)result} [out dataList.Count : {dataList.Count}]");

            return result;
        }
        /// <summary>
        /// コマンドを実行し、読み込みデータ一覧を取得する
        /// ※DB接続・切断は上位クラスで実施
        /// </summary>
        /// <param name="sql">SQL文</param>
        /// <param name="dataList">読み込みデータ辞書一覧</param>
        /// <param name="retryCount">リトライ回数</param>
        /// <returns>エラーコード</returns>
        public uint ExecuteReader(string sql, out List<Dictionary<string, object>> dataList, int retryCount = DEFAULT_RETRY_CCOUNT)
        {
            //Logger.WriteLog(LogType.METHOD_IN, $"DBAccess ExecuteReader({sql}, out dataList, {retryCount})");

            var result = (uint)0;
            dataList = new List<Dictionary<string, object>>();

            for (var i = 0; i <= retryCount; i++)
            {
                try
                {
                    if (connection == null || connection.State == System.Data.ConnectionState.Closed || connection.State == System.Data.ConnectionState.Broken)
                    {
                        result = (uint)ErrorCodeList.DB_NO_OPEN_ERROR;
                        break;
                    }

                    try
                    {
                        using (var command = new NpgsqlCommand(sql))
                        {
                            command.Connection = connection;
                            using (var dataReader = command.ExecuteReader())
                            {
                                if (dataReader != null && dataReader.HasRows)
                                {
                                    while (dataReader.Read())
                                    {
                                        var dataDic = new Dictionary<string, object>();
                                        for (var j = 0; j < dataReader.FieldCount; j++)
                                            dataDic[dataReader.GetName(j)] = dataReader[j];
                                        dataList.Add(dataDic);
                                    }
                                }
                            }
                        }
                        result = (uint)ErrorCodeList.STATUS_SUCCESS;
                    }
                    catch (Exception ex)
                    {
                        result = (uint)ErrorCodeList.DB_READ_WRITE_ERROR;
                        Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");

                    }
                }
                catch (Exception ex)
                {
                    result = (uint)ErrorCodeList.DB_OPEN_ERROR;
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                }

                if (result == (uint)ErrorCodeList.STATUS_SUCCESS)
                    break;

                if (retryCount > 0 && i < retryCount)
                {
                    Logger.WriteLog(LogType.DEBUG, $"Retry {i + 1}/{retryCount}");
                    Thread.Sleep(100);
                }
            }

            //Logger.WriteLog(LogType.METHOD_OUT, $"DBAccess ExecuteReader : {(ErrorCodeList)result} [out dataList.Count : {dataList.Count}]");

            return result;
        }
        #endregion

        #region コマンド実行
        /// <summary>
        /// コマンド実行
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        /// <param name="sql">SQL文</param>
        /// <param name="retryCount">リトライ回数</param>
        /// <returns>エラーコード</returns>
        public uint ExecuteNonSQL(string connectionString, string sql, int retryCount = DEFAULT_RETRY_CCOUNT)
        {
            //Logger.WriteLog(LogType.METHOD_IN, $"DBAccess ExecuteNonSQL({connectionString}, {sql}, {retryCount})");

            var result = (uint)0;

            for (var i = 0; i <= retryCount; i++)
            {
                try
                {
                    // DB接続
                    Open(connectionString);
                
                    result = ExecuteNonSQL(sql, 0);
                }
                catch (Exception ex)
                {
                    result = (uint)ErrorCodeList.DB_OPEN_ERROR;
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                }
                finally
                {
                    // DB切断
                    Close();
                }

                if (result == (uint)ErrorCodeList.STATUS_SUCCESS)
                    break;

                if (retryCount > 0 && i < retryCount)
                {
                    Logger.WriteLog(LogType.DEBUG, $"Retry {i + 1}/{retryCount}");
                    Thread.Sleep(100);
                }
            }

            //Logger.WriteLog(LogType.METHOD_OUT, $"DBAccess ExecuteNonSQL : {(ErrorCodeList)result}");

            return result;
        }
        /// <summary>
        /// コマンド実行
        /// ※DB接続・切断は上位クラスで実施
        /// </summary>
        /// <param name="sql">SQL文</param>
        /// <param name="retryCount">リトライ回数</param>
        /// <returns>エラーコード</returns>
        public uint ExecuteNonSQL(string sql, int retryCount = DEFAULT_RETRY_CCOUNT)
        {
            //Logger.WriteLog(LogType.METHOD_IN, $"DBAccess ExecuteNonSQL({sql}, {retryCount})");

            var result = (uint)0;

            for (var i = 0; i <= retryCount; i++)
            {
                try
                {
                    if (connection == null || connection.State == System.Data.ConnectionState.Closed || connection.State == System.Data.ConnectionState.Broken)
                    {
                        result = (uint)ErrorCodeList.DB_NO_OPEN_ERROR;
                        break;
                    }

                    try
                    {
                        using (var command = new NpgsqlCommand(sql))
                        {
                            command.Connection = connection;
                            // コマンド実行
                            command.ExecuteNonQuery();
                        }

                        result = (uint)ErrorCodeList.STATUS_SUCCESS;
                    }
                    catch (Exception ex)
                    {
                        result = (uint)ErrorCodeList.DB_READ_WRITE_ERROR;
                        Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                    }
                }
                catch (Exception ex)
                {
                    result = (uint)ErrorCodeList.DB_OPEN_ERROR;
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                }

                if (result == (uint)ErrorCodeList.STATUS_SUCCESS)
                    break;

                if (retryCount > 0 && i < retryCount)
                {
                    Logger.WriteLog(LogType.DEBUG, $"Retry {i + 1}/{retryCount}");
                    Thread.Sleep(100);
                }
            }

            //Logger.WriteLog(LogType.METHOD_OUT, $"DBAccess ExecuteNonSQL : {(ErrorCodeList)result}");

            return result;
        }
        #endregion

        #region UPDATE/INSERT実行
        /// <summary>
        /// UPDATE/INSERT実行
        /// SELECT文を実行し、データがあればUPDATE、なければINSERT文を実行する
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        /// <param name="selectSql">SELECT文</param>
        /// <param name="insertSql">INSERT文</param>
        /// <param name="updateSql">UPDATE文</param>
        /// <param name="retryCount">リトライ回数</param>
        /// <returns>エラーコード</returns>
        public uint Upsert(string connectionString, string selectSql, string insertSql, string updateSql, int retryCount = DEFAULT_RETRY_CCOUNT)
        {
            //Logger.WriteLog(LogType.METHOD_IN, $"DBAccess ExecuteNonSQL({connectionString}, {selectSql}, {insertSql}, {updateSql}, {retryCount})");

            var result = (uint)0;

            for (var i = 0; i <= retryCount; i++)
            {
                try
                {
                    // DB接続
                    Open(connectionString);

                    result = ExecuteReader(selectSql, out var dataList, 0);
                    if (result == (uint)ErrorCodeList.STATUS_SUCCESS)
                    {
                        if (dataList.Count == 0)
                            result = ExecuteNonSQL(insertSql, 0);   // データがない場合はINSERT
                        else
                            result = ExecuteNonSQL(updateSql, 0);   // データがある場合はUPDATE
                    }
                }
                catch (Exception ex)
                {
                    result = (uint)ErrorCodeList.DB_OPEN_ERROR;
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                }
                finally
                {
                    // DB切断
                    Close();
                }

                if (result == (uint)ErrorCodeList.STATUS_SUCCESS)
                    break;

                if (retryCount > 0 && i < retryCount)
                {
                    Logger.WriteLog(LogType.DEBUG, $"Retry {i + 1}/{retryCount}");
                    Thread.Sleep(100);
                }
            }

            //Logger.WriteLog(LogType.METHOD_OUT, $"DBAccess ExecuteNonSQL : {(ErrorCodeList)result}");

            return result;
        }
        #endregion
    }
    #endregion
}