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

using SystemConfig;
using DL_CommonLibrary;
using DL_Logger;
using ErrorCodeDefine;
using ShareResource;


namespace TransferManagerApp
{
    /// <summary>
    /// ユーザーコントロール
    /// ヘッダー
    /// </summary>
    public partial class ucHeader : UserControl, IDisposable
    {
        private const string THIS_NAME = "ucHeader";


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
        public ucHeader()
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

                // タイトル
                lblTitle.Content = Const.Title;
                // バージョン
                string fullname = typeof(App).Assembly.Location;
                var info = System.Diagnostics.FileVersionInfo.GetVersionInfo(fullname);
                string fileVersion = info.FileVersion;
                lblVersion.Content = fileVersion;

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
                if(!STATUS_SUCCESS(rc))
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
            try
            {
                if (_tmrUpdateDisplay != null)
                    _tmrUpdateDisplay.Stop();
                _tmrUpdateDisplay = null;
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, false);
            }
        }

        /// <summary>
        /// ラベル ダブルクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UInt32 rc = 0;
            Label ctrl = (Label)sender;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {ctrl.Name}");
            try
            {
                if (ctrl == lblError)
                {// 運転
                    Resource.ClearError();
                }

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
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

                // 現在日時表示
                lblCurrentDatetime.Content = DateTime.Now.ToString("yyyy/MM/dd HH:mm");

                // PLC PING接続状態
                bool connect = true;
                for (int i = 0; i < Resource.SystemStatus.PlcPingConnection.Length; i++) 
                {
                    if (IniFile.AisleEnable[i]) 
                    {
                        if (!Resource.SystemStatus.PlcPingConnection[i]) 
                        {
                            connect = false;
                            break;
                        }
                    }
                }
                lblPlcPingConnection.Background = (connect) ? Brushes.Lime : Brushes.Red;

                // Server PING接続状態
                connect = true;
                if (!Resource.SystemStatus.ServerPingConnection)
                    connect = false;
                lblServerPingConnection.Background = (connect) ? Brushes.Lime : Brushes.Red;

                // エラー発生状況
                ErrorDetail errInfo = Resource.SystemStatus.Error.GetLatestErrorInfo();
                lblError.Background = (errInfo.code == 0 || errInfo.IsReset) ? Brushes.Gray : Brushes.Red;
                //lblError.Background = (currentErrorCode == 0) ? Brushes.Gray : Brushes.Red;

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
