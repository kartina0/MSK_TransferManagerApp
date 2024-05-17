//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using DL_CommonLibrary;
using SystemConfig;
using DL_Logger;
using ErrorCodeDefine;
using ShareResource;


namespace TransferManagerApp
{
    /// <summary>
    /// ucFooter.xaml の相互作用ロジック
    /// </summary>
    public partial class ucFooter : UserControl, IDisposable
    {
        private const string THIS_NAME = "ucFooter";


        /// <summary>
        /// 画面更新タイマー
        /// </summary>
        private DispatcherTimer _tmrUpdateDisplay = null;
        /// <summary>
        /// タイマー重複ロック
        /// </summary>
        private bool _timeLock = false;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ucFooter()
        {
            InitializeComponent();
        }
        /// <summary>
        /// ウィンドウロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.CONTROL, $"{GetType().Name} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                    return;

                // 画面更新タイマー初期化
                rc = InitTimer();
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Resource.ErrorHandler(rc, true);
            }
        }
        /// <summary>
        /// ウィンドウクローズ
        /// </summary>
        public void Dispose() 
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.CONTROL, $"{GetType().Name} {MethodBase.GetCurrentMethod().Name}()");


        }

        /// <summary>
        /// 画面更新タイマー初期化
        /// </summary>
        private UInt32 InitTimer()
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // タイマのインスタンスを生成
                _tmrUpdateDisplay = new DispatcherTimer();
                // インターバルを設定
                _tmrUpdateDisplay.Interval = TimeSpan.FromMilliseconds(100);
                // タイマメソッドを設定
                _tmrUpdateDisplay.Tick += new EventHandler(tmrUpdateDisplay_tick);
                // タイマを開始
                _tmrUpdateDisplay.Start();
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{GetType().Name}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 画面更新タイマー トリガー
        /// </summary>
        private void tmrUpdateDisplay_tick(object sender, EventArgs e)
        {
            UpdateDisplay();
        }
        /// <summary>
        /// 画面更新
        /// </summary>
        private void UpdateDisplay()
        {
            UInt32 rc = 0;
            try
            {
                // 重複防止
                if (_timeLock)
                    return;
                _timeLock = true;

                //初期化完了確認
                if (!Resource.SystemStatus.initialize_Completed) return;

                // エラー発生状況
                // 登録されている最新のエラー情報を表示
                ErrorDetail errInfo = Resource.SystemStatus.Error.GetLatestErrorInfo();
                if (errInfo.occurTime.Ticks > 0)
                {
                    // メッセージ
                    lblCurrentError.Content = string.Format("{0},{1}", errInfo.occurTime.ToString(FormatConst.MessageDateTimeFormat), errInfo.message);

                    // 文字色
                    if (errInfo.IsReset)
                        lblCurrentError.Foreground = Brushes.Black;
                    else
                        lblCurrentError.Foreground = Brushes.Red;
                }
                else
                {
                    // メッセージ
                    lblCurrentError.Content = Resource.SystemStatus.alarmMessage;
                    // 文字色
                    lblCurrentError.Foreground = Brushes.Black;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                Resource.ErrorHandler(ex);
            }
            finally
            {
                _timeLock = false;
            }
        }




        /// <summary>
        /// Check Error State
        /// </summary>
        private static bool STATUS_SUCCESS(UInt32 err)
        {
            return err == (int)ErrorCodeList.STATUS_SUCCESS;
        }


    }
}
