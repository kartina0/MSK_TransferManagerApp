// ----------------------------------------------
// Copyright © 2017 DATALINK
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ErrorCodeDefine;

namespace DL_CommonLibrary
{

    /// <summary>
    /// スレッド動作モード
    /// </summary>
    public enum THREAD_SEQUENCE_TYPE
    {
        /// <summary>
        /// 連続動作
        /// </summary>
        CONTINUOUS = 0,
        /// <summary>
        /// 1回のリクエストで1スキャンのみ動作
        /// </summary>
        SINGLE,
    }

    /// <summary>
    /// イベント待ち結果
    /// </summary>
    public enum THREAD_WAIT_RESULT
    {
        SHUTDOWN = 0,
        REQUEST,
        ABORT,
        TIMEOUT,
        UNKNOW,
    }

    /// <summary>
    /// スレッド停止要求
    /// </summary>
    public enum THREAD_STOP_REQUEST
    {
        NONE = 0,
        SUSPEND,
        ABORT,
    }

    /// <summary>
    /// 各スレッド制御クラス
    /// ※Unitスレッドなどで細かな
    ///   イベント待ちなどは各ユニットで実装する事
    /// </summary>
    public class ThreadInfo
    {

        #region "Private Variable"
        /// <summary>
        /// スレッド動作タイプ
        /// </summary>
        private THREAD_SEQUENCE_TYPE m_Type = THREAD_SEQUENCE_TYPE.CONTINUOUS;

        /// <summary>
        /// スレッドハンドル
        /// </summary>
        private volatile Thread hThread = null;

        /// <summary>
        /// スレッド終了要求
        /// </summary>
        private volatile ManualResetEvent m_EvShutDown = new ManualResetEvent(false);

        /// <summary>
        /// スレッド動作開始要求
        /// </summary>
        private volatile ManualResetEvent m_EvRequest = new ManualResetEvent(false);

        /// <summary>
        /// スレッド動作途中停止要求
        /// ※Not Use
        /// </summary>
        private volatile ManualResetEvent m_EvSuspend = new ManualResetEvent(false);

        /// <summary>
        /// スレッド動作強制終了 要求
        /// </summary>
        private volatile ManualResetEvent m_EvAbort = new ManualResetEvent(false);

        /// <summary>
        /// スレッド動作中通知イベント
        /// </summary>
        private volatile ManualResetEvent m_EvRunning = new ManualResetEvent(false);

        /// <summary>
        /// スレッド動作完了通知イベント
        /// </summary>
        private volatile ManualResetEvent m_EvComplete = new ManualResetEvent(false);

        #endregion

        #region "Public Variable"
        
        /// <summary>
        /// スレッドインターバル
        /// </summary>
        public volatile Int32 Interval = 10;

        /// <summary>
        /// スレッドステータス(ERROR_CODE)
        /// </summary>
        public volatile ErrorCodeList ThreadStatus = ErrorCodeList.STATUS_SUCCESS;

        /// <summary>
        /// 要求処理パラメータ
        /// </summary>
        public RequestFunctionInfo FuncInfo = new RequestFunctionInfo();

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="type"></param>
        public ThreadInfo(THREAD_SEQUENCE_TYPE type)
        {
            m_Type = type;
        }


        /// <summary>
        /// スレッド作成
        /// </summary>
        /// <returns></returns>
        //public UInt32 CreateThread(ParameterizedThreadStart threadProc, object arg, ThreadPriority priority)
        public UInt32 CreateThread(ParameterizedThreadStart threadProc, object arg, ThreadPriority priority, bool isBackground = false, bool showDialogInThisThread = false )
        {
            UInt32 rs = 0;
            try
            {
                if (this.hThread != null)
                    rs = (UInt32)ErrorCodeList.THREAD_EXIST;

                if (rs == 0)
                {
                    this.m_EvShutDown.Reset();
                    this.m_EvRequest.Reset();

                    this.hThread = new Thread(threadProc);


                    // @@20190415
                    if (isBackground)
                        this.hThread.IsBackground = true;
                    // @@20190415
                    if (showDialogInThisThread)
                        this.hThread.SetApartmentState(ApartmentState.STA);

                    this.hThread.Start(arg);
                    this.hThread.Priority = priority;



                    Thread.Sleep(10);
                    while (true)
                    {
                        Thread.Sleep(1);
                        ThreadState s = hThread.ThreadState;
                        if (s == ThreadState.Running || s == ThreadState.WaitSleepJoin)
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
                rs = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rs;
        }

        /// <summary>
        /// スレッド側が終了した場合に呼び出す
        /// </summary>
        public void ThreadEndReport()
        {
            try
            {
                hThread = null;
            }
            catch { }
        }

        /// <summary>
        /// スレッドが起動しているか確認
        /// </summary>
        /// <returns></returns>
        public bool IsAlive()
        {
            if (hThread == null) return false;
            return hThread.IsAlive;
        }

        /// <summary>
        /// スレッド動作中か確認
        /// </summary>
        /// <returns></returns>
        public bool IsBusy()
        {
            return m_EvRunning.WaitOne(0, false);
        }

        /// <summary>
        /// スレッド終了
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public UInt32 ShutDown(Int32 timeout)
        {
            UInt32 rs = 0;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            m_EvShutDown.Set();
            Thread.Sleep(1);
            while (true)
            {
                if (hThread == null || !hThread.IsAlive)
                    break;

                if (sw.ElapsedMilliseconds > timeout)
                {
                    rs = (UInt32)ErrorCodeList.THREAD_END_TIMEOUT;
                    break;
                }
                Thread.Sleep(100);
            }
            sw.Reset();

            if (rs != 0 && this.hThread != null)
            {
                this.hThread.Abort();
                this.hThread = null;
            }
            this.hThread = null;
            return rs;
        }

        /// <summary>
        /// スレッド動作許可
        /// </summary>
        public void Release()
        {
            m_EvAbort.Reset();
            m_EvComplete.Reset();
            m_EvRunning.Reset();
            m_EvShutDown.Reset();
            m_EvSuspend.Reset();

            m_EvRequest.Set();
        }

        /// <summary>
        /// スレッド動作禁止
        /// </summary>
        public void Reset()
        {
            m_EvRequest.Reset();
        }

        /// <summary>
        /// スレッド動作強制停止
        /// </summary>
        public void Abort()
        {
            m_EvAbort.Set();
        }
        /// <summary>
        /// スレッド一時停止
        /// </summary>
        public void Suspend()
        {
            m_EvSuspend.Set();
        }
        /// <summary>
        /// スレッド一時停止解除
        /// </summary>
        public void Resume()
        {
            m_EvSuspend.Reset();
        }

        /// <summary>
        /// イベント待ち
        /// ※Threadで使用
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns>0 ShutDownイベント,1 動作要求イベント,256 タイムアウト</returns>
        public THREAD_WAIT_RESULT WaitAnyEvent(int timeout)
        {
            THREAD_WAIT_RESULT rs = THREAD_WAIT_RESULT.TIMEOUT;
            int index = 0;
            WaitHandle[] waits = new WaitHandle[2];
            WaitHandle[] waits2 = new WaitHandle[1];
            try
            {
                waits.SetValue(m_EvShutDown, 0);
                waits.SetValue(m_EvAbort, 1);

                waits2.SetValue(m_EvRequest, 0);

                // m_EvRunning.Reset();    
                m_EvComplete.Set();     // 動作完了イベント

                // イベント待ち
                index = WaitHandle.WaitAny(waits, 0);

                // 結果
                if (index == 0)
                {
                    rs = THREAD_WAIT_RESULT.SHUTDOWN;
                }
                else if (index == 1)
                {
                    rs = THREAD_WAIT_RESULT.ABORT;
                    m_EvRunning.Reset();      // 動作中通知イベントON
                }
                else if (index == WaitHandle.WaitTimeout)
                {
                    rs = THREAD_WAIT_RESULT.TIMEOUT;
                    m_EvRunning.Reset();    // 動作中通知イベントOFF
                }

                if (rs == THREAD_WAIT_RESULT.TIMEOUT)
                {
                    // イベント待ち
                   // m_EvRequest.Reset();
                    index = WaitHandle.WaitAny(waits2, timeout);
                    if (index == 0)
                    {
                        rs = THREAD_WAIT_RESULT.REQUEST;
                        m_EvComplete.Reset();   // 動作完了イベントOFF
                        m_EvRunning.Set();      // 動作中通知イベントON
                    }
                    else if (index == WaitHandle.WaitTimeout)
                    {
                        rs = THREAD_WAIT_RESULT.TIMEOUT;
                        m_EvRunning.Reset();    // 動作中通知イベントOFF
                    }
                    else
                    {
                        rs = THREAD_WAIT_RESULT.UNKNOW;
                    }

                }






                // SINGLEモード時はイベントをリセット
                if (rs == THREAD_WAIT_RESULT.REQUEST && m_Type == THREAD_SEQUENCE_TYPE.SINGLE)
                {
                    m_EvRequest.Reset();
                }


            }
            catch { }

            waits = null;
            return rs;
        }

        /// <summary>
        /// スレッド停止要求取得
        /// ※Threadで使用
        /// </summary>
        /// <returns></returns>
        public THREAD_STOP_REQUEST CheckStopRequest()
        {
            THREAD_STOP_REQUEST rs = THREAD_STOP_REQUEST.NONE;
            try
            {
                //if (m_EvStop.WaitOne(0, false))
                //    rs = THREAD_STOP_REQUEST.PAUSE;

                if (m_EvAbort.WaitOne(0, false))
                    rs = THREAD_STOP_REQUEST.ABORT;

                if (rs == THREAD_STOP_REQUEST.NONE && m_EvSuspend.WaitOne(0, false))
                    rs = THREAD_STOP_REQUEST.SUSPEND;

            }
            catch { }

            return rs;
        }

        /// <summary>
        /// スレッド動作待ち
        /// </summary>
        /// <returns></returns>
        public bool WaitBusy(int timeout)
        {
            return m_EvRunning.WaitOne(timeout, false);
        }

        /// <summary>
        /// スレッド動作完了待ち
        /// </summary>
        /// <returns></returns>
        public bool WaitComplete(int timeout)
        {
            return m_EvComplete.WaitOne(timeout, false);
        }

        /// <summary>
        /// スレッド動作完了待ちイベントクリア
        /// </summary>
        public void ResetWaitComplete()
        {
            m_EvComplete.Reset();
        }
    }

    /// <summary>
    /// スレッドへ要求する処理パラメータ
    /// </summary>
    public class RequestFunctionInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int ManualCmd = 0;
        /// <summary>
        /// パラメータ
        /// IDにより異なるので気をつけて
        /// ID毎に勝手に決めていいよ
        /// </summary>
        public Int32 lParam1 = 0;
        public Int32 lParam2 = 0;
        public Int32 lParam3 = 0;
        public Int32 lParam4 = 0;       // @@20180818-1
        public Int32 lParam5 = 0;
        public Int32 lParam6 = 0;
        public Int32 lParam7 = 0;
        public Int32 lParam8 = 0;
        public Int32 lParam9 = 0;
        public Int32 lParam10 = 0;
        public Int32 lParam11 = 0;
        public Int32 lParam12 = 0;
        public Int32 lParam13 = 0;
        public Int32 lParam14 = 0;
        public Int32 lParam15 = 0;
        public Int32 lParam16 = 0;


        public double dParam1 = 0;
        public double dParam2 = 0;
        public double dParam3 = 0;
        public double dParam4 = 0;      // @@20180818-1
        public double dParam5 = 0;
        public double dParam6 = 0;
        public double dParam7 = 0;
        public double dParam8 = 0;
        public double dParam9 = 0;
        public double dParam10 = 0;
        public double dParam11 = 0;
        public double dParam12 = 0;
        public double dParam13 = 0;
        public double dParam14 = 0;
        public double dParam15 = 0;
        public double dParam16 = 0;


        public string sParam1 = "";
        public string sParam2 = "";
        public string sParam3 = "";
        public string sParam4 = "";     // @@20180818-1
        public string sParam5 = "";
        public string sParam6 = "";
        public string sParam7 = "";
        public string sParam8 = "";
        public string sParam9 = "";
        public string sParam10 = "";
        public string sParam11 = "";
        public string sParam12 = "";
        public string sParam13 = "";
        public string sParam14 = "";
        public string sParam15 = "";
        public string sParam16 = "";

    }



}
