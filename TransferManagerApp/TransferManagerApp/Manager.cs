// -------------------------------------------------------
// Copyright © 2022 DATALINK
//---------------------------------------------------------
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;

using DL_CommonLibrary;
using DL_PlcInterfce;
using PLCModule;
using DL_Logger;
using ShareResource;
using SystemConfig;
using ErrorCodeDefine;
using System.Windows.Documents;


namespace TransferManagerApp
{
    /// <summary>
    /// マネージャクラス
    /// </summary>
    internal static class Manager
    {
        private const string THIS_NAME = "Manager";

        /// <summary>
        /// システムスレッド
        /// </summary>
        private static ThreadInfo _systemThread = new ThreadInfo(THREAD_SEQUENCE_TYPE.CONTINUOUS);
        ///// <summary>
        ///// モニタスレッド
        ///// </summary>
        //private static ThreadInfo _monitorThread = new ThreadInfo(THREAD_SEQUENCE_TYPE.CONTINUOUS);
        /// <summary>
        /// 接続確認スレッド
        /// </summary>
        private static ThreadInfo _connectionCheckThread = new ThreadInfo(THREAD_SEQUENCE_TYPE.CONTINUOUS);
        /// <summary>
        /// ファイル削除スレッド
        /// </summary>
        private static ThreadInfo _fileCleaner = new ThreadInfo(THREAD_SEQUENCE_TYPE.CONTINUOUS);
        /// <summary>
        /// エラー履歴ファイル出力スレッド
        /// </summary>
        private static ThreadInfo _errorHistory = new ThreadInfo(THREAD_SEQUENCE_TYPE.CONTINUOUS);


        /// <summary>
        /// 起動
        /// </summary>
        /// <returns></returns>
        public static UInt32 Start() 
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // システムスレッド起動
                Logger.WriteLog(LogType.INFO, string.Format("システムスレッド 起動"));
                _systemThread.CreateThread(Thread_System, _systemThread, ThreadPriority.Lowest);
                _systemThread.Interval = 100;
                _systemThread.Release();

                //// モニタスレッド起動
                //Logger.WriteLog(LogType.INFO, string.Format("モニタスレッド 起動"));
                //_monitorThread.CreateThread(Thread_Monitor, _monitorThread, ThreadPriority.Lowest);
                //_monitorThread.Interval = 100;
                //_monitorThread.Release();

                // 接続確認スレッド起動
                Logger.WriteLog(LogType.INFO, string.Format("接続確認スレッド 起動"));
                _connectionCheckThread.CreateThread(Thread_ConnectionCheck, _connectionCheckThread, ThreadPriority.Lowest);
                _connectionCheckThread.Interval = 1000;
                _connectionCheckThread.Release();

                // ファイルクリーナースレッド起動
                Logger.WriteLog(LogType.INFO, string.Format("ファイルクリーナスレッド 起動"));
                _fileCleaner.CreateThread(Thread_FileCleaner, _fileCleaner, ThreadPriority.Lowest);
                _fileCleaner.Interval = 1000;
                _fileCleaner.Release();

                // エラー履歴ファイル出力スレッド起動
                Logger.WriteLog(LogType.INFO, string.Format("エラー履歴ファイル出力スレッド 起動"));
                _errorHistory.CreateThread(Thread_ErrorHistory, _errorHistory, ThreadPriority.Lowest);
                _errorHistory.Interval = 1000;
                _errorHistory.Release();
            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }


        /// <summary>
        /// 終了
        /// </summary>
        public static UInt32 Close()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try 
            {
                // ----------------------------------
                // スレッド終了
                // ----------------------------------
                // 先に終了をコールする
                if (_systemThread != null) _systemThread.ShutDown(0);
                //if (_monitorThread != null) _monitorThread.ShutDown(0);
                if (_connectionCheckThread != null) _connectionCheckThread.ShutDown(0);
                if (_fileCleaner != null) _fileCleaner.ShutDown(0);
                if (_errorHistory != null) _errorHistory.ShutDown(0);


                if (_systemThread != null)
                {
                    Logger.WriteLog(LogType.INFO, string.Format("システムスレッド 終了"));
                    _systemThread.ShutDown(10000);
                    _systemThread = null;
                }

                //if (_monitorThread != null)
                //{
                //    Logger.WriteLog(LogType.INFO, string.Format("モニタスレッド 終了"));
                //    _monitorThread.ShutDown(1000);
                //    _monitorThread = null;
                //}

                if (_connectionCheckThread != null)
                {
                    Logger.WriteLog(LogType.INFO, string.Format("接続確認スレッド 終了"));
                    _connectionCheckThread.ShutDown(10000);
                    _connectionCheckThread = null;
                }
                
                if (_fileCleaner != null)
                {
                    Logger.WriteLog(LogType.INFO, string.Format("ファイルクリーナスレッド 終了"));
                    _fileCleaner.ShutDown(10000);
                    _fileCleaner = null;
                }

                if (_errorHistory != null)
                {
                    Logger.WriteLog(LogType.INFO, string.Format("エラー履歴ファイル出力スレッド 終了"));
                    _errorHistory.ShutDown(10000);
                    _errorHistory = null;
                }
            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }


        /// <summary>
        /// システムスレッド
        /// 
        /// [内容]
        /// システムクロック更新
        /// </summary>
        private static void Thread_System(object arg)
        {
            UInt32 rc = 0;
            ThreadInfo info = (ThreadInfo)arg;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                bool exit = false;

                while (true)
                {
                    // Waits Any Event
                    THREAD_WAIT_RESULT index = info.WaitAnyEvent(100);
                    rc = 0;

                    if (index == THREAD_WAIT_RESULT.SHUTDOWN)
                    {
                        exit = true;
                    }
                    if (index == THREAD_WAIT_RESULT.REQUEST)
                    {

                        CommonStatus sysStatus = Resource.SystemStatus;

                        // 現在日時を取得
                        sysStatus.CurrentDateTime = DateTime.Now;

                    }

                    // Thread interval
                    Thread.Sleep(info.Interval);

                    if (exit) break;
                }

            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
        }


        ///// <summary>
        ///// モニタスレッド
        ///// 
        ///// [内容]
        ///// 各機器の状態を取得する等、モニタ値を取得するスレッド
        ///// </summary>
        //private static void Thread_Monitor(object arg)
        //{
        //    ThreadInfo info = (ThreadInfo)arg;
        //    Logger.WriteLog(LogType.METHOD_IN, string.Format("Thread_Monitor"));


        //    try
        //    {
        //        bool exit = false;

        //        while (true)
        //        {
        //            // Waits Any Event
        //            THREAD_WAIT_RESULT index = info.WaitAnyEvent(100);
        //            UInt32 rc = 0;

        //            if (index == THREAD_WAIT_RESULT.SHUTDOWN)
        //            {
        //                exit = true;
        //            }
        //            if (index == THREAD_WAIT_RESULT.REQUEST)
        //            {
        //                // Thread interval
        //                Thread.Sleep(info.Interval);
        //                // アプリケーション初期化完了まで待つ
        //                if (!Resource.SysStatus.initialize_Completed)
        //                {
        //                    Thread.Sleep(info.Interval);
        //                    continue;
        //                }

        //                //// ----------------------
        //                //// PLC状態取得
        //                //PLC_IF Plc = Resource.PLC;

        //                //// PLCステータス取得
        //                //if (Plc.IsConnected())
        //                //{

        //                //}

        //                // 初回モニタデータ取得完了フラグ ON
        //                Resource.SysStatus.initial_monitor_complete = true;

        //            }
        //            if (exit) break;
        //            Thread.Sleep(info.Interval);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteLog(LogType.METHOD_OUT, string.Format("Thread_Monitor 例外処理 : {0}", ex.Message));
        //    }
        //    Logger.WriteLog(LogType.METHOD_OUT, string.Format("Thread_Monitor"));
        //}


        /// <summary>
        /// 接続確認スレッド
        /// 
        /// [内容]
        /// 接続状態の確認を行う
        /// </summary>
        private static void Thread_ConnectionCheck(object arg)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            ThreadInfo info = (ThreadInfo)arg;
            try
            {
                bool exit = false;

                while (true)
                {
                    // Waits Any Event
                    THREAD_WAIT_RESULT index = info.WaitAnyEvent(100);
                    rc = 0;

                    if (index == THREAD_WAIT_RESULT.SHUTDOWN)
                    {
                        exit = true;
                    }
                    if (index == THREAD_WAIT_RESULT.REQUEST)
                    {

                        // アプリケーション初期化完了まで待つ
                        if (!Resource.SystemStatus.initialize_Completed)
                        {
                            Thread.Sleep(info.Interval);
                            continue;
                        }
                        CommonStatus sysStatus = Resource.SystemStatus;

                        // 接続確認実行中
                        sysStatus.ConnectionCheckRunning = true;



                        // ------------------------------
                        // PLC
                        // ------------------------------
                        // 接続確認
                        string[] plc_IpAddr = null;
                        bool[] plc_Connected = null;
                        // PLC IPアドレス
                        List<string> ip_list = new List<string>();
                        for (int i = 0; i < Const.MaxAisleCount; i++) 
                        {
                            if(IniFile.AisleEnable[i])
                                ip_list.Add(IniFile.PlcIpAddress[i]);
                        }
                        plc_IpAddr = ip_list.ToArray();
                        // PING送付
                        NetMisc.Ping(plc_IpAddr, 3000, out plc_Connected);
                        // ステータスに反映させる
                        for (int i = 0; i < plc_IpAddr.Length; i++) 
                        {
                            // サーバー接続状態の切り替わりのログを出力
                            if (sysStatus.PlcPingConnection[i] != plc_Connected[i])
                            {
                                if (plc_Connected[i])
                                    Logger.WriteLog(LogType.INFO, $"ネットワーク：PLC{i + 1}に接続されました ： {plc_IpAddr[i]}");
                                else
                                    Logger.WriteLog(LogType.INFO, $"ネットワーク：PLC{i + 1}が切断されました ： {plc_IpAddr[i]}");
                            }
                            sysStatus.PlcPingConnection[i] = plc_Connected[i];
                         }


                        // ------------------------------
                        // SERVER
                        // ------------------------------
                        // 接続確認
                        string[] server_IpAddr = new string[1];
                        bool[] server_Connected = new bool[1];
                        // SERVER IPアドレス
                        server_IpAddr[0] = IniFile.DBIpAddress;
                        // PING送付
                        NetMisc.Ping(server_IpAddr, 3000, out server_Connected);
                        // ステータスに反映させる
                        if (sysStatus.ServerPingConnection != server_Connected[0])
                        {
                            // サーバー接続状態の切り替わりのログを出力
                            if (server_Connected[0])
                                Logger.WriteLog(LogType.INFO, $"ネットワーク：Serverに接続されました ： {server_IpAddr[0]}");
                            else
                                Logger.WriteLog(LogType.INFO, $"ネットワーク：Serverが切断されました ： {server_IpAddr[0]}");
                        }
                        sysStatus.ServerPingConnection = server_Connected[0];



                        // 接続確認実行完了
                        sysStatus.ConnectionCheckRunning = false;
                    }
                    Thread.Sleep(info.Interval);

                    if (exit) break;
                }

            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");

        }

        /// <summary>
        /// ファイル削除スレッド
        /// 
        /// [内容]
        /// 指定日より前のファイルをたまに消す
        /// </summary>
        private static void Thread_FileCleaner(object arg)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            ThreadInfo info = (ThreadInfo)arg;
            try
            {
                bool exit = false;
                Stopwatch sw = new Stopwatch();
                int step = 0;
                // プログラム起動後直ぐに動作しないようにする
                Thread.Sleep(5000);

                // ストップウォッチ開始
                sw.Start();

                while (true)
                {
                    // Waits Any Event
                    THREAD_WAIT_RESULT index = info.WaitAnyEvent(100);
                    rc = 0;

                    if (index == THREAD_WAIT_RESULT.SHUTDOWN)
                    {
                        exit = true;
                    }
                    if (index == THREAD_WAIT_RESULT.REQUEST)
                    {
                        // アプリケーション初期化完了まで待つ
                        if (!Resource.SystemStatus.initialize_Completed)
                        {
                            Thread.Sleep(info.Interval);
                            continue;
                        }

                        // 60分に１回動作させる
                        // ※Intervalを60分にしたらスレッドが停止できなくなってしまうので
                        //   経過時間で監視する。  ※１種のファイルを1時間間隔で削除する
                        //if (sw.Elapsed.Minutes > 60)
                        if (sw.Elapsed.Minutes >= 60)
                        {
                            // 10日以上前のログは削除
                            if (step == 0)
                                FileIo.DeleteLogFile(IniFile.LogDir, Logger.systemlogFileName, DateTime.Now.AddDays(-10));
                            // 4カ月以上前のログは削除
                            if (step == 1)
                                FileIo.DeleteLogFile(IniFile.LogDir, Logger.alarmlogFileName, DateTime.Now.AddMonths(-4));
                            // 1カ月以上前のログは削除
                            if (step == 2)
                                FileIo.DeleteLogFile(IniFile.LogDir, Logger.historylogFileName, DateTime.Now.AddMonths(-1));
                            // 1カ月以上前のログは削除
                            if (step == 2)
                                FileIo.DeleteLogFile(IniFile.LogDir, Logger.operationlogFileName, DateTime.Now.AddMonths(-1));

                            step++;

                            if (step >= 4) step = 0;   // 12時間で1サイクル

                            sw.Restart();
                        }


                    }
                    Thread.Sleep(info.Interval);

                    if (exit) break;
                }

            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
        }

        /// <summary>
        /// エラー履歴ファイル出力スレッド
        /// 
        /// [内容]
        /// 発生したエラーをファイルに出力
        /// </summary>
        private static void Thread_ErrorHistory(object arg)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            ThreadInfo info = (ThreadInfo)arg;
            info.Interval = 1000;
            try
            {
                bool exit = false;
                Stopwatch sw = new Stopwatch();
                // プログラム起動後直ぐに動作しないようにする
                Thread.Sleep(5000);

                // ストップウォッチ開始
                sw.Start();

                string dir = System.IO.Path.GetDirectoryName(IniFile.ErrorHistoryFilePath);
                string filePath = IniFile.ErrorHistoryFilePath;
                string filePath_Temp = System.IO.Path.Combine(dir, "temp.csv");
                List<string> errorHistoryList = new List<string>();
                DateTime preErrorDt = DateTime.MinValue;
                bool errorOccured = false;

                while (true)
                {
                    // Waits Any Event
                    THREAD_WAIT_RESULT index = info.WaitAnyEvent(100);
                    rc = 0;

                    if (index == THREAD_WAIT_RESULT.SHUTDOWN)
                    {
                        exit = true;
                    }
                    if (index == THREAD_WAIT_RESULT.REQUEST)
                    {
                        // アプリケーション初期化完了まで待つ
                        if (!Resource.SystemStatus.initialize_Completed)
                        {
                            Thread.Sleep(info.Interval);
                            continue;
                        }

                        errorHistoryList.Clear();

                        // ------------------------------
                        // 新しく発生したエラーがあれば取得
                        // ------------------------------
                        errorOccured = false;

                        // アプリorサーバー
                        ErrorDetail latestError = Resource.SystemStatus.Error.GetLatestErrorInfo();
                        if (preErrorDt < latestError.occurTime) 
                        {// 新しく発生したエラー
                            errorOccured = true;
                            preErrorDt = latestError.occurTime;

                            string line = "";
                            line += preErrorDt.ToString("yyyy/MM/dd HH:mm:ss");
                            line += ",";
                            if (latestError.code >= (UInt32)ErrorCodeList.DB_OPEN_ERROR)
                            {// サーバーエラー
                                line += "サーバー";
                            }
                            else 
                            {// アプリ内エラー
                                line += "アプリケーション";
                            }
                            line += ",";
                            line += latestError.message;

                            errorHistoryList.Add(line);
                        }

                        // PLC(アイル)エラー
                        for (int aisleIndex = 0; aisleIndex < Const.MaxAisleCount; aisleIndex++) 
                        {
                            if (IniFile.AisleEnable[aisleIndex]) 
                            {
                                Resource.Plc[aisleIndex].Access.GetMachineStatus(out PLCMachineStatus status);
                                if (status.ErrorFlg)
                                {// 新しく発生したエラー
                                    errorOccured = true;

                                    string line = "";
                                    line += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                    line += ",";
                                    line += "設備";
                                    line += ",";
                                    string mes = "";
                                    if (status.ErrorType == PLC_ERROR_TYPE.ORDER_ERROR) mes = "仕分け処理異常";
                                    line += mes;
                                
                                    errorHistoryList.Add(line);
                                    Resource.Plc[aisleIndex].Access.SetErrorFlgOFF();
                                }
                            }
                        }

                        // 新しく発生したエラーがなければcontinue
                        if (!errorOccured)
                            continue;


                        // ------------------------------
                        // エラー履歴ファイル
                        // 読み出し
                        // ------------------------------
                        // ファイル存在確認
                        if (!Directory.Exists(dir)) 
                            Directory.CreateDirectory(dir);
                        if(!File.Exists(filePath))
                            File.Create(filePath);
                        // 読み出し
                        using (StreamReader reader = new StreamReader(filePath, Encoding.GetEncoding("shift_jis")))
                        {
                            bool header = true;
                            // 末尾まで繰り返す
                            while (!reader.EndOfStream)
                            {
                                // CSVファイルの一行を読み込む
                                string line = reader.ReadLine();

                                // ヘッダーは無視 
                                if (header) 
                                {
                                    header = false;
                                    continue;
                                }
                                //// 読み込んだ一行をカンマ毎に分けて配列に格納する
                                //string[] values = line.Split(',');

                                // リストに格納
                                errorHistoryList.Add(line);
                            }
                        }
                        // 指定行数になるよう古い履歴を削除
                        if (errorHistoryList.Count > Const.ErrorHistorymxCount) 
                        {
                            int deleteCount = errorHistoryList.Count - Const.ErrorHistorymxCount;
                            errorHistoryList.RemoveRange(200, deleteCount);
                        }


                        // ------------------------------
                        // エラー履歴ファイル
                        // 書き込み
                        // ------------------------------
                        // 一度tempファイルに書き込み
                        using (StreamWriter writer = new StreamWriter(filePath_Temp, false, Encoding.GetEncoding("shift_jis")))
                        {
                            // ヘッダー書き込み
                            string header = "日時,種別,エラー内容";
                            writer.WriteLine(header);

                            // エラーリストを書き込み
                            foreach (string line in errorHistoryList)
                            {
                                writer.WriteLine(line);
                            }
                        }
                        // 元のエラー履歴ファイルを削除
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        // tempファイルをエラー履歴ファイルとしてコピー、tempファイル削除
                        if (File.Exists(filePath_Temp)) 
                        {
                            File.Copy(filePath_Temp, filePath);
                            File.Delete(filePath_Temp);
                        }


                    }
                    Thread.Sleep(info.Interval);

                    if (exit) break;
                }

            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
        }



        /// <summary>
        /// エラー有無確認
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        private static bool STATUS_SUCCESS(UInt32 rs)
        {
            return rs == 0;
        }
    }
}
