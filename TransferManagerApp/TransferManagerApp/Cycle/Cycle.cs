//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

using DL_CommonLibrary;
using DL_Logger;
using SystemConfig;
using ShareResource;
using ErrorCodeDefine;


namespace TransferManagerApp
{
    /// <summary>
    /// サイクル管理
    /// </summary>
    public static class Cycle
    {
        private const string THIS_NAME = "Cycle";

        /// <summary>
        /// サイクル管理スレッド
        /// </summary>
        private static ThreadInfo _cycleThread = new ThreadInfo(THREAD_SEQUENCE_TYPE.CONTINUOUS);
        /// <summary>
        /// スレッド シャットダウン
        /// </summary>
        private static bool _shutDown = false;
        /// <summary>
        /// スレッド シャットダウン完了
        /// </summary>
        private static bool _shutDownComp = false;
        
        /// <summary>
        /// 各アイルのサイクル
        /// </summary>
        public static CycleAisle[] cycleAisles = null;



        /// <summary>
        /// 開始
        /// </summary>
        /// <returns></returns>
        public static UInt32 Start() 
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // サイクルスレッド起動
                _cycleThread.CreateThread(Thread_Cycle, _cycleThread, ThreadPriority.Lowest);
                _cycleThread.Interval = 1000;
                _cycleThread.Release();


                // 各アイルごとのサイクルスレッド起動
                cycleAisles = new CycleAisle[Const.MaxAisleCount];
                for (int i = 0; i < Const.MaxAisleCount; i++) 
                {
                    if (IniFile.AisleEnable[i]) 
                    {
                        cycleAisles[i] = new CycleAisle();
                        cycleAisles[i].Start(i, Resource.Plc[i]);
                    }
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
        /// 終了
        /// </summary>
        /// <returns></returns>
        public static UInt32 Close()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // サイクルスレッド終了
                if (_cycleThread != null)
                {
                    _shutDown = true;
                    while (!_shutDownComp)
                        Thread.Sleep(100);
                }
                _cycleThread = null;


                // 各アイルごとのサイクルスレッド終了
                Logger.WriteLog(LogType.INFO, string.Format("サイクル管理スレッド 終了"));
                if (cycleAisles != null) 
                {
                    for (int i = 0; i < Const.MaxAisleCount; i++) 
                    {
                        if(cycleAisles[i] != null)
                            cycleAisles[i].Close();
                    }
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
        /// サイクルスレッド
        /// </summary>
        private static void Thread_Cycle(object arg)
        {
            UInt32 rc = 0;
            ThreadInfo info = (ThreadInfo)arg;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                bool exit = false;
                int interval = 5000;
                DateTime preDateTime = Resource.SystemStatus.CurrentDateTime;

                while (true)
                {
                    rc = 0;
                    if (_shutDown)
                        break;

                    // Waits Any Event
                    THREAD_WAIT_RESULT index = info.WaitAnyEvent(100);

                    if (index == THREAD_WAIT_RESULT.SHUTDOWN)
                    {
                        exit = true;
                    }
                    if (index == THREAD_WAIT_RESULT.REQUEST)
                    {

                        // -----------------------------------------------
                        // 日付切り替わり処理
                        // -----------------------------------------------
                        // 現在日付取得
                        DateTime currentDateTime = DateTime.ParseExact($"{(DateTime.Now).Year.ToString("D4")}" +
                              $"{(DateTime.Now).Month.ToString("D2")}" +
                              $"{(DateTime.Now).Day.ToString("D2")}" +
                              $"000000", "yyyyMMddHHmmss", null);

                        // 日付切り替わりチェック
                        DateTime currentDt = DateTime.Now;
                        if (preDateTime < DateTime.Today.AddHours(IniFile.DateChangeTime.Hour).AddMinutes(IniFile.DateChangeTime.Minute) &&
                            currentDt >= DateTime.Today.AddHours(IniFile.DateChangeTime.Hour).AddMinutes(IniFile.DateChangeTime.Minute))
                        {

                            // 日付切り替わりメッセージ表示
                            string message = "日付が切り替わりました。仕分データ更新のためアプリを再起動してください。";
                            MessageBox.Show(message, "確認", MessageBoxButton.OK);



                            //// ファイル読み込み済フラグをクリアする
                            //for (int i = 0; i < Const.MaxPostCount; i++)
                            //    Resource.SystemStatus.IsLoadedTodayPickData[i] = false;
                            //Resource.SystemStatus.IsLoadedTodayMasterWork = false;
                            //Resource.SystemStatus.IsLoadedTodayMasterStore = false;
                            //Resource.SystemStatus.IsLoadedTodayMasterWorker = false;

                            //for (int i = 0; i < Const.MaxPostCount; i++)
                            //{
                            //    if (PreStatus.PostStartDt[i] != DateTime.MinValue
                            //    {
                            //        PreStatus.PostStartDt[i] = DateTime.MinValue;
                            //        string fileName = System.IO.Path.Combine(Const.IniDir, Const.PreStatusFileName);
                            //        PreStatus.Save(fileName);
                            //    }
                            //    if (PreStatus.PostEndDt[i] != DateTime.MinValue)
                            //    {
                            //        PreStatus.PostEndDt[i] = DateTime.MinValue;
                            //        string fileName = System.IO.Path.Combine(Const.IniDir, Const.PreStatusFileName);
                            //        PreStatus.Save(fileName);
                            //    }
                            //}

                        }

                        preDateTime = Resource.SystemStatus.CurrentDateTime;



                    }
                    if (exit) break;
                    Thread.Sleep(interval);
                }

            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            _shutDownComp = true;
        }

    }
}
