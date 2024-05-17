//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DL_CommonLibrary;
using ServerModule;
using SystemConfig;
using ShareResource;
using DL_Logger;
using ErrorCodeDefine;


namespace TransferManagerApp
{
    /// <summary>
    /// サーバー
    /// </summary>
    public static class ServerControl
    {
        private const string THIS_NAME = "ServerControl";

        /// <summary>
        /// 現在便No
        /// </summary>
        private static int _currentPostIndex = 0;

        /// <summary>
        /// 仕分データ管理
        /// </summary>
        private static Server _server = null;
        /// <summary>
        /// サーバースレッド
        /// </summary>
        private static ThreadInfo _serverThread = new ThreadInfo(THREAD_SEQUENCE_TYPE.CONTINUOUS);
        /// <summary>
        /// スレッド シャットダウン
        /// </summary>
        private static bool _shutDown = false;
        /// <summary>
        /// スレッド シャットダウン完了
        /// </summary>
        private static bool _shutDownComp = false;

        /// <summary>
        /// 初期フラグ
        /// </summary>
        public static bool init = true;

        /// <summary>
        /// プロセス 実績データ書き込み
        /// </summary>
        public static Step[] process_WriteExecuteUpData = new Step[Const.MaxWorkRegisterCount];



        /// <summary>
        /// 起動
        /// </summary>
        /// <returns></returns>
        public static UInt32 Start(Server server)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 仕分データ管理クラス
                _server = server;

                // スレッド起動
                _serverThread.CreateThread(Thread_Server, _serverThread, ThreadPriority.Lowest);
                _serverThread.Interval = 10000;
                _serverThread.Release();
            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                rc = (Int32)ErrorCodeList.EXCEPTION;
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
                if (_serverThread != null)
                {
                    _shutDown = true;
                    while (!_shutDownComp)
                        Thread.Sleep(100);
                }
                _serverThread = null;
                Logger.WriteLog(LogType.INFO, string.Format("サイクル管理スレッド 終了"));



                //if (_serverThread != null)
                //{
                //    // 先に終了をコールする
                //    _serverThread.ShutDown(0);

                //    Logger.WriteLog(LogType.INFO, string.Format("サイクル管理スレッド 終了"));
                //    _serverThread.ShutDown(10000);
                //    _serverThread = null;
                //}

            }
            catch (Exception ex) 
            {
                Resource.ErrorHandler(ex);
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }


        /// <summary>
        /// サーバースレッド
        /// </summary>
        private static void Thread_Server(object arg)
        {
            UInt32 rc = 0;
            ThreadInfo info = (ThreadInfo)arg;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                bool exit = false;
                int interval = 10000;
                while (true)
                {
                    // Waits Any Event
                    THREAD_WAIT_RESULT index = info.WaitAnyEvent(100);
                    rc = 0;
                    if (_shutDown)
                        break;

                    if (index == THREAD_WAIT_RESULT.SHUTDOWN)
                    {
                        exit = true;
                    }
                    if (index == THREAD_WAIT_RESULT.REQUEST)
                    {

                        lock (Resource.SystemStatus.Lock_Server) 
                        {

                            // ------------------------------
                            // マスターファイル読み出し
                            // ------------------------------
                            if (STATUS_SUCCESS(rc))
                            {
                                // 商品マスタ
                                if (!Resource.SystemStatus.IsLoadedTodayMasterWork) 
                                {
                                    // サーバーからローカルにコピー
                                    bool isTodaysFileLocal = false;
                                    rc = Resource.Server.MasterFile.CopyMasterWorkFile(out isTodaysFileLocal);
                                    if (!STATUS_SUCCESS(rc))
                                    {
                                        Resource.ErrorHandler(rc);
                                        rc = 0;
                                    }
                                    else if (isTodaysFileLocal)
                                    {
                                        // 読み出し
                                        rc = Resource.Server.MasterFile.ReadMasterWork();
                                        if (!STATUS_SUCCESS(rc))
                                        {
                                            Resource.ErrorHandler(rc);
                                            rc = 0;
                                        }
                                        else
                                        {
                                            Resource.SystemStatus.IsLoadedTodayMasterWork = true;
                                            Logger.WriteLog(LogType.INFO, string.Format("商品マスター読み出し完了"));
                                        }
                                    }
                                }

                                // 店マスタ
                                if (!Resource.SystemStatus.IsLoadedTodayMasterWork) 
                                {
                                    // サーバーからローカルにコピー
                                    bool isTodaysFileLocal = false;
                                    rc = Resource.Server.MasterFile.CopyMasterStoreFile(out isTodaysFileLocal);
                                    if (!STATUS_SUCCESS(rc))
                                    {
                                        Resource.ErrorHandler(rc);
                                        rc = 0;
                                    }
                                    else if (isTodaysFileLocal)
                                    {
                                        // 読み出し
                                        rc = Resource.Server.MasterFile.ReadMasterStore();
                                        if (!STATUS_SUCCESS(rc))
                                        {
                                            Resource.ErrorHandler(rc);
                                            rc = 0;
                                        }
                                        else
                                        {
                                            Resource.SystemStatus.IsLoadedTodayMasterStore = true;
                                            Logger.WriteLog(LogType.INFO, string.Format("店マスター読み出し完了"));
                                        }
                                    }
                                }

                                // 作業者マスタ
                                if (!Resource.SystemStatus.IsLoadedTodayMasterWorker) 
                                {
                                    // サーバーからローカルにコピー
                                    bool isTodaysFileLocal = false;
                                    rc = Resource.Server.MasterFile.CopyMasterWorkerFile(out isTodaysFileLocal);
                                    if (!STATUS_SUCCESS(rc))
                                    {
                                        Resource.ErrorHandler(rc);
                                        rc = 0;
                                    }
                                    else if (isTodaysFileLocal)
                                    {
                                        // 読み出し
                                        rc = Resource.Server.MasterFile.ReadMasterWorker();
                                        if (!STATUS_SUCCESS(rc))
                                        {
                                            Resource.ErrorHandler(rc);
                                            rc = 0;
                                        }
                                        else
                                        {
                                            Resource.SystemStatus.IsLoadedTodayMasterWorker = true;
                                            Logger.WriteLog(LogType.INFO, string.Format("作業者マスター読み出し完了"));
                                        }
                                    }
                                }
                            }


                            // ------------------------------
                            // PICKDATA
                            // ------------------------------
                            if (IniFile.PickDataEnable)
                            {
                                if (STATUS_SUCCESS(rc))
                                {
                                    // ------------------------------
                                    // PICKDATA読み出し
                                    // ------------------------------
                                    for (int i = 0; i < Const.MaxPostCount; i++)
                                    {
                                        // サーバーのPICKDATAが更新されていたらローカルにコピー
                                        rc = Resource.Server.PickData.CopyPickDataFile(i, out bool isTodaysFileLocal, out bool isServerUpdate);
                                        if (!STATUS_SUCCESS(rc))
                                        {
                                            Resource.ErrorHandler(rc);
                                            rc = 0;
                                        }
                                        else if (isTodaysFileLocal && isServerUpdate) 
                                        {
                                            // PICKDATA 読み出し
                                            rc = Resource.Server.PickData.Read(i);
                                            if (!STATUS_SUCCESS(rc))
                                            {
                                                Resource.ErrorHandler(rc);
                                                string message = $"{i + 1}便のPICKDATAファイル読み出し異常";
                                                Logger.WriteLog(LogType.INFO, message);
                                                rc = 0;
                                            }
                                            else
                                            {
                                                string message = $"{i + 1}便のPICKDATA読み出し完了";
                                                Logger.WriteLog(LogType.INFO, message);

                                                // DB書込み
                                                rc = Resource.Server.PickData.WriteDB(i, Resource.Server.MasterFile.MasterStoreList);
                                            }
                                        }
                                    }

                                }
                            }


                            // ------------------------------
                            // 仕分データ取得
                            // ------------------------------
                            // 1~3便
                            for (int postIndex = 0; postIndex < Const.MaxPostCount; postIndex++)
                            {
                                // 仕分データ全て読み出し
                                // ※商品一覧を画面表示することも考えて、全便読み出す
                                Logger.WriteLog(LogType.INFO, $"ServerControl 仕分データ取得中 {postIndex + 1}便");
                                rc = _server.OrderInfo.ReadOrderData(postIndex, ORDER_PROCESS.UNLOAD);
                                // 起動時のみ、取込済の行も取得
                                if (init)
                                {
                                    rc = _server.OrderInfo.ReadOrderData(postIndex, ORDER_PROCESS.LOADED);
                                }
                            }
                            Logger.WriteLog(LogType.INFO, string.Format("ServerControl 仕分データ取得 完了"));


                            // ------------------------------
                            // 仕分実績データ取得 (起動時だけ)
                            // ------------------------------
                            if (init)
                            {
                                // 1~3便
                                for (int postIndex = 0; postIndex < Const.MaxPostCount; postIndex++)
                                {
                                    Logger.WriteLog(LogType.INFO, $"ServerControl 仕分実績データ取得中 {postIndex + 1}便");
                                    rc = _server.OrderInfo.ReadExecuteData(postIndex);
                                }
                                Logger.WriteLog(LogType.INFO, string.Format("ServerControl 仕分実績データ取得 完了"));
                            }


                            // ------------------------------
                            // 仕分データにマテハン変換情報を追加
                            // ------------------------------
                            for (int postIndex = 0; postIndex < Const.MaxPostCount; postIndex++)
                            {
                                for (int aisleIndex = 0; aisleIndex < Const.MaxAisleCount; aisleIndex++)
                                {
                                    if (!IniFile.AisleEnable[aisleIndex])
                                        continue;

                                    for (int batchIndex = 0; batchIndex < Const.MaxBatchCount; batchIndex++)
                                    {
                                        string[] currentBatchArray =
                                            Resource.batch.BatchInfoCurrent.Post[postIndex].Aisle[aisleIndex].Batch[batchIndex].OutputToArray(IniFile.UnitEnable[aisleIndex]);
                                        rc = Resource.Server.OrderInfo.ConvertOrderDataList(postIndex, aisleIndex, batchIndex, currentBatchArray);
                                    }
                                }
                            }


                            // 仕分実績データ書き込み中
                            for (int i = 0; i < Const.MaxWorkRegisterCount; i++)
                            {
                                if (process_WriteExecuteUpData[i] == Step.REQUEST)
                                    process_WriteExecuteUpData[i] = Step.ACTIVE;
                            }


                            // ------------------------------
                            // 仕分実績データリスト 作成/更新
                            // ------------------------------
                            for (int postIndex = 0; postIndex < Const.MaxPostCount; postIndex++)
                            {
                                Logger.WriteLog(LogType.INFO, $"ServerControl 仕分実績データリスト作成/更新中 {postIndex + 1}便");
                                rc = _server.OrderInfo.CreateExecuteDataList(postIndex);
                            }
                            Logger.WriteLog(LogType.INFO, string.Format("ServerControl 仕分実績データリスト作成/更新 完了"));


                            // ------------------------------
                            // 仕分実績データ 書込み
                            // 現在の便だけ
                            // ------------------------------
                            for (int postIndex = 0; postIndex < Const.MaxPostCount; postIndex++)
                            {
                                Logger.WriteLog(LogType.INFO, $"ServerControl 仕分実績データ書込み中 {postIndex + 1}便");
                                rc = _server.OrderInfo.WriteExecuteData(postIndex);
                            }
                            Logger.WriteLog(LogType.INFO, string.Format("ServerControl 仕分実績データ書込み 完了"));


                            // 仕分データ実績書き込み完了
                            // PLCの仕分完了をクリア
                            for (int i = 0; i < Const.MaxWorkRegisterCount; i++)
                            {
                                if (process_WriteExecuteUpData[i] == Step.ACTIVE)
                                    process_WriteExecuteUpData[i] = Step.COMP;
                            }
                        

                            if (exit) break;
                            init = false;
                            Thread.Sleep(interval);

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");

            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            _shutDownComp = true;
        }



        /// <summary>
        /// Check Error State
        /// </summary>
        private static bool STATUS_SUCCESS(UInt32 err) { return err == (int)ErrorCodeList.STATUS_SUCCESS; }
    }




    public enum Step 
    {
        NONE = 0,
        REQUEST,
        ACTIVE,
        COMP,
    }


}
