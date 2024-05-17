//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Input;

using SystemConfig;
using ShareResource;
using DL_Logger;
using ErrorCodeDefine;
using TransferManagerApp_Debug;
using PLCModule;


namespace TransferManagerApp
{
    /// <summary>
    /// windowOrder.xaml の相互作用ロジック
    /// </summary>
    public partial class windowOrder : Window
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "windowOrder";

        /// <summary>
        /// ウィンドウ表示中フラグ
        /// </summary>
        public bool isShowing = false;

        /// <summary>
        /// 画面更新タイマー
        /// </summary>
        private DispatcherTimer _tmrUpdateDisplay = null;
        /// <summary>
        /// タイマー重複ロック
        /// </summary>
        private bool _timeLock = false;

        /// <summary>
        /// 仕分作業完了報告書出力 ウィンドウオブジェクト
        /// </summary>
        private windowOutputOrderReport _windowOutputOrderReport = null;
        /// <summary>
        /// システム設定 ウィンドウオブジェクト
        /// </summary>
        private windowSystemSetting _windowSystemSetting = null;
        /// <summary>
        /// 権限 ウィンドウオブジェクト
        /// </summary>
        private windowAuthority _windowAuthority = null;
        /// <summary>
        /// 仕分データ一覧 ウィンドウオブジェクト
        /// </summary>
        private windowOrderInfo _windowOrderInfo = null;
        /// <summary>
        /// マスターファイル一覧 ウィンドウオブジェクト
        /// </summary>
        private windowMasterFile _windowMasterFile = null;
        /// <summary>
        /// PLCシミュレーター ウィンドウオブジェクト
        /// </summary>
        private Debug_windowPlcSimulator _windowPlcSimulator = null;
        /// <summary>
        /// マスターファイル手動操作ツール ウィンドウオブジェクト
        /// </summary>
        private Debug_windowMasterFileManualTool _windowMasterFileManualTool = null;

        /// <summary>
        /// ボタン連続操作防止フラグ
        /// </summary>
        private bool _buttonLock = false;

        


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public windowOrder()
        {
            InitializeComponent();

        }
        /// <summary>
        /// ウィンドウロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // タイトルバーを消しても画面移動可能にする処理
                this.MouseLeftButtonDown += delegate { DragMove(); };

                // 画面表示中フラグ true
                isShowing = true;

                // ボタン色セット
                btnOperationMonitor.Background = Brushes.CornflowerBlue;
                btnProgressMonitor.Background = Brushes.SlateGray;
                btnWorkOrderStatus.Background = Brushes.SlateGray;
                btnErrorHistory.Background = Brushes.SlateGray;

                // キーボード表示テキスト
                ucKeyControl.sequence = KEY_SEQUENCE.ORDER;
                //ucKeyControl.MouseDown_Operation += Window_KeyDown;
                // ユーザーコントロールで発火するイベントハンドラを追加
                //ucKeyControl.OnMouseDon += Window_KeyUp;


                // 画面更新タイマー初期化
                rc = InitTimer();

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
        }
        /// <summary>
        /// ウィンドウクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                if (_tmrUpdateDisplay != null)
                    _tmrUpdateDisplay.Stop();
                _tmrUpdateDisplay = null;

                // ユーザーコントロールを閉じる
                ucTab_OperationMonitor.Dispose();
                ucTab_OperationMonitor = null;
                ucTab_ProgressMonitor.Dispose();
                ucTab_ProgressMonitor = null;
                ucTab_WorkStatus.Dispose();
                ucTab_WorkStatus = null;
                ucTab_ErrorHistory.Dispose();
                ucTab_ErrorHistory = null;

                // メニュー画面等を閉じる
                if (_windowOutputOrderReport != null) 
                {
                    if (_windowOutputOrderReport.isShowing)
                        _windowOutputOrderReport.Close();
                    _windowOutputOrderReport = null;
                }
                if (_windowSystemSetting != null) 
                {
                    if (_windowSystemSetting.isShowing)
                        _windowSystemSetting.Close();
                    _windowSystemSetting = null;
                }
                if (_windowAuthority != null)
                { 
                    if (_windowAuthority.isShowing)
                        _windowAuthority.Close();
                    _windowAuthority = null;
                }
                if (_windowOrderInfo != null) 
                {
                    if (_windowOrderInfo.isShowing)
                        _windowOrderInfo.Close();
                    _windowOrderInfo = null;
                }
                if (_windowMasterFile != null) 
                {
                    if (_windowMasterFile.isShowing)
                        _windowMasterFile.Close();
                    _windowMasterFile = null;
                }
                if (_windowPlcSimulator != null)
                {
                    if (_windowPlcSimulator.isShowing)
                        _windowPlcSimulator.Close();
                    _windowPlcSimulator = null;
                }
                if (_windowMasterFileManualTool != null) 
                {
                    if (_windowMasterFileManualTool.isShowing)
                        _windowMasterFileManualTool.Close();
                    _windowMasterFileManualTool = null;
                }


                // 画面表示中フラグ false
                isShowing = false;

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, false);
            }
        }

        /// <summary>
        /// ボタン クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UInt32 rc = 0;
            Button ctrl = (Button)sender;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {ctrl.Name}");
            try
            {
                COMMAND command = COMMAND.NONE;

                if (ctrl == btnClose)
                {// 閉じる
                    this.Close();
                }

                else if (ctrl == btnOperationMonitor)
                {// 作業モニター 選択
                    command = COMMAND.TAB_OPERATION;
                }
                else if (ctrl == btnProgressMonitor)
                {// 進捗モニター 選択
                    command = COMMAND.TAB_PROGRESS;
                }
                else if (ctrl == btnWorkOrderStatus)
                {// 商品仕分状況 選択
                    command = COMMAND.TAB_WORK_STATUS;
                }
                else if (ctrl == btnErrorHistory)
                {// エラー履歴 選択
                    command = COMMAND.TAB_ERROR_HISTORY;
                }

                else if (ctrl == btnPostNo1)
                {// 便No1
                    command = COMMAND.POST_01;
                }
                else if (ctrl == btnPostNo2)
                {// 便No2
                    command = COMMAND.POST_02;
                }
                else if (ctrl == btnPostNo3)
                {// 便No3
                    command = COMMAND.POST_03;
                }


                if (command != COMMAND.NONE)
                    rc = Function(command);

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
        }
        /// <summary>
        /// メニューバー クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            UInt32 rc = 0;
            MenuItem ctrl = (MenuItem)sender;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {ctrl.Name}");
            try
            {
                COMMAND command = COMMAND.NONE;

                if (ctrl == menuFile_SaveOrderData)
                {// 仕分実績バックアップのDB反映
                    MessageBoxResult result = MessageBox.Show("現在の仕分実績データのバックアップファイルを作成します\r\nよろしいですか？", "確認", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        lock (Resource.SystemStatus.Lock_Server) 
                        {
                            // 仕分実績バックアップファイルをDBに反映
                            rc = Resource.Server.OrderInfo.TransferExecuteDataBackup();
                        }

                    }
                }
                else if (ctrl == menuFile_OutputOrderReport) 
                {// 仕分作業完了報告書出力
                    // 重複表示防止
                    if (_windowOutputOrderReport == null || !_windowOutputOrderReport.isShowing)
                    {
                        _windowOutputOrderReport = new windowOutputOrderReport();
                        _windowOutputOrderReport.Show();
                    }
                }
                //else if (ctrl == menuSetting_SystemSetting)
                //{// システム設定
                //    // 重複表示防止
                //    if (_windowSystemSetting == null || !_windowSystemSetting.isShowing)
                //    {
                //        _windowSystemSetting = new windowSystemSetting();
                //        _windowSystemSetting.Show();
                //    }
                //}
                //else if (ctrl == menuSetting_Authority)
                //{// 権限モード
                //    // 重複表示防止
                //    if (_windowAuthority == null || !_windowAuthority.isShowing)
                //    {
                //        _windowAuthority = new windowAuthority();
                //        _windowAuthority.Show();
                //    }
                //}
                else if (ctrl == menuTool_OrderInfo)
                {// 仕分データ一覧
                    // 重複表示防止
                    if (_windowOrderInfo == null || !_windowOrderInfo.isShowing)
                    {
                        _windowOrderInfo = new windowOrderInfo();
                        _windowOrderInfo.Show();
                    }
                }
                else if (ctrl == menuTool_MasterFile)
                {// マスターファイル一覧
                    // 重複表示防止
                    if (_windowMasterFile == null || !_windowMasterFile.isShowing)
                    {
                        _windowMasterFile = new windowMasterFile();
                        _windowMasterFile.Show();
                    }
                }
                else if (ctrl == menuDebug_PlcSimulator)
                {// シミュレーター
                    // PLCがダミーモードか確認
                    bool plcDummy = true;
                    for (int i = 0; i < Const.MaxAisleCount; i++)
                    {
                        if (Resource.Plc[i] != null)
                        {
                            if (!Resource.Plc[i].Access.isDummy)
                                plcDummy = false;
                        }
                    }
                    // ダミーじゃなければreturn
                    if (!plcDummy)
                    {
                        MessageBox.Show("PLCがダミーモードではありません。", Const.Title, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
                        return;
                    }

                    // 重複表示防止
                    if (_windowPlcSimulator == null || !_windowPlcSimulator.isShowing)
                    {
                        _windowPlcSimulator = new Debug_windowPlcSimulator();
                        _windowPlcSimulator.Show();
                    }
                }


                if (command != COMMAND.NONE)
                    rc = Function(command);

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
        }

        /// <summary>
        /// ユーザー操作処理まとめ
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private UInt32 Function(COMMAND command)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {command.ToString()}");
            try
            {
                if (command == COMMAND.TAB_OPERATION)
                {// 作業モニター 選択
                    btnOperationMonitor.Background = (SolidColorBrush)Application.Current.Resources["ButtonColorOrder"];
                    btnProgressMonitor.Background = Brushes.SlateGray;
                    btnWorkOrderStatus.Background = Brushes.SlateGray;
                    btnErrorHistory.Background = Brushes.SlateGray;

                    ucTab_OperationMonitor.Visibility = Visibility.Visible;
                    ucTab_ProgressMonitor.Visibility = Visibility.Hidden;
                    ucTab_WorkStatus.Visibility = Visibility.Hidden;
                    ucTab_ErrorHistory.Visibility = Visibility.Hidden;
                }
                else if (command == COMMAND.TAB_PROGRESS)
                {// 進捗モニター 選択
                    btnOperationMonitor.Background = Brushes.SlateGray;
                    btnProgressMonitor.Background = (SolidColorBrush)Application.Current.Resources["ButtonColorOrder"];
                    btnWorkOrderStatus.Background = Brushes.SlateGray;
                    btnErrorHistory.Background = Brushes.SlateGray;

                    ucTab_OperationMonitor.Visibility = Visibility.Hidden;
                    ucTab_ProgressMonitor.Visibility = Visibility.Visible;
                    ucTab_WorkStatus.Visibility = Visibility.Hidden;
                    ucTab_ErrorHistory.Visibility = Visibility.Hidden;
                }
                else if (command == COMMAND.TAB_WORK_STATUS)
                {// 商品仕分状況 選択
                    btnOperationMonitor.Background = Brushes.SlateGray;
                    btnProgressMonitor.Background = Brushes.SlateGray;
                    btnWorkOrderStatus.Background = (SolidColorBrush)Application.Current.Resources["ButtonColorOrder"];
                    btnErrorHistory.Background = Brushes.SlateGray;

                    ucTab_OperationMonitor.Visibility = Visibility.Hidden;
                    ucTab_ProgressMonitor.Visibility = Visibility.Hidden;
                    ucTab_WorkStatus.Visibility = Visibility.Visible;
                    ucTab_ErrorHistory.Visibility = Visibility.Hidden;
                }
                else if (command == COMMAND.TAB_ERROR_HISTORY)
                {// エラー履歴 選択
                    btnOperationMonitor.Background = Brushes.SlateGray;
                    btnProgressMonitor.Background = Brushes.SlateGray;
                    btnWorkOrderStatus.Background = Brushes.SlateGray;
                    btnErrorHistory.Background = (SolidColorBrush)Application.Current.Resources["ButtonColorOrder"];

                    ucTab_OperationMonitor.Visibility = Visibility.Hidden;
                    ucTab_ProgressMonitor.Visibility = Visibility.Hidden;
                    ucTab_WorkStatus.Visibility = Visibility.Hidden;
                    ucTab_ErrorHistory.Visibility = Visibility.Visible;
                }

                else if (command == COMMAND.POST_01)
                {// 1便
                    rc = ChangePostNo(0);
                }
                else if (command == COMMAND.POST_02)
                {// 2便
                    rc = ChangePostNo(1);
                }
                else if (command == COMMAND.POST_03)
                {// 3便
                    rc = ChangePostNo(2);
                }

                else if (command == COMMAND.ERROR_CHECKED)
                {// エラー確認
                    Resource.ClearError();
                }
                else if (command == COMMAND.CLOSE)
                {// 閉じる
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
            return rc;
        }

        /// <summary>
        /// 便No切り替え
        /// </summary>
        private UInt32 ChangePostNo(int postIndex)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            string message = "";
            try
            {
                if (Resource.SystemStatus.CurrentPostIndex == postIndex)
                    return rc;

                // 便切り替え可能か確認(仕分中なら切り替え不可)
                bool possible = true;
                for (int aisleIndex = 0; aisleIndex < Const.MaxAisleCount; aisleIndex++) 
                {
                    if (IniFile.AisleEnable[aisleIndex]) 
                    {
                        for (int workIndex = 0; workIndex < Const.MaxWorkRegisterCount; workIndex++) 
                        {
                            rc = Resource.Plc[aisleIndex].Access.GetOrderStatus(workIndex, out PLCWorkOrder orderStatus);
                            if (orderStatus.sequence != PLC_ORDER_SEQUENCE.NOT_REGISTER)
                                possible = false;
                            if (!possible)
                                break;
                        }
                    }
                    if (!possible)
                        break;
                }

                if (!possible) 
                {
                    message = "仕分作業中の商品があるため、便の切り替えができません";
                    Logger.WriteLog(LogType.INFO, message);
                    MessageBox.Show(message, "確認", MessageBoxButton.OK);
                    return rc;
                }

                // 切り替え
                message = $"{postIndex + 1}便へ切り替えます。よろしいですか？";
                Logger.WriteLog(LogType.INFO, message);
                MessageBoxResult result = MessageBox.Show(message, "確認", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Logger.WriteLog(LogType.INFO, "=> はい");

                    Resource.SystemStatus.CurrentPostIndex = postIndex;
                    PreStatus.PostIndex = postIndex;
                    string preStatusFileName = System.IO.Path.Combine(Const.IniDir, Const.PreStatusFileName);
                    rc = PreStatus.Save(preStatusFileName);
                    if (STATUS_SUCCESS(rc)) 
                    {
                        Logger.WriteLog(LogType.INFO, $"便Noを切り替えました ->{postIndex + 1}便");
                        message = $"{postIndex + 1}便を切り替えました";
                        Logger.WriteLog(LogType.INFO, message);
                        MessageBox.Show(message, "確認", MessageBoxButton.OK);
                    }
                }
                else 
                {
                    Logger.WriteLog(LogType.INFO, "=> いいえ");
                }

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 画面更新タイマー初期化
        /// </summary>
        private UInt32 InitTimer()
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // タイマのインスタンスを生成lbl
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
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
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
            bool isConnected = false;
            try
            {
                // 重複防止
                if (_timeLock)
                    return;
                _timeLock = true;
                //初期化完了確認
                if (!Resource.SystemStatus.initialize_Completed) return;


                // 便No
                if (Resource.SystemStatus.CurrentPostIndex == 0)
                {
                    btnPostNo1.Background = Brushes.LightSkyBlue;
                    btnPostNo2.Background = Brushes.SlateGray;
                    btnPostNo3.Background = Brushes.SlateGray;
                }
                else if (Resource.SystemStatus.CurrentPostIndex == 1)
                {
                    btnPostNo1.Background = Brushes.SlateGray;
                    btnPostNo2.Background = Brushes.LightSkyBlue;
                    btnPostNo3.Background = Brushes.SlateGray;
                }
                else if (Resource.SystemStatus.CurrentPostIndex == 2)
                {
                    btnPostNo1.Background = Brushes.SlateGray;
                    btnPostNo2.Background = Brushes.SlateGray;
                    btnPostNo3.Background = Brushes.LightSkyBlue;
                }


                if (IniFile.DebugMode)
                {
                    menuDebug.Visibility = Visibility.Visible;
                }
                else 
                {
                    menuDebug.Visibility = Visibility.Hidden;
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



        #region キーボード処理
        /// <summary>
        /// ユーザー操作処理 種別
        /// </summary>
        private enum COMMAND
        {
            NONE = 0,

            TAB_OPERATION,
            TAB_PROGRESS,
            TAB_WORK_STATUS,
            TAB_ERROR_HISTORY,
            POST_01,
            POST_02,
            POST_03,
            ERROR_CHECKED,
            CLOSE,

        }
        /// <summary>
        /// キーボード連続操作防止フラグ
        /// </summary>
        private bool _keyLock = false;

        /// <summary>
        /// キーボード キーダウンイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {e.Key}");
            try
            {
                if (!_keyLock)
                {
                    // 連続押下防止
                    _keyLock = true;
                    // 画面アニメーション
                    ucKeyControl.KeyDown_Operation(e);
                }
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
        }
        /// <summary>
        /// キーボード キーアップイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {e.Key}");
            try
            {
                // 画面アニメーション
                ucKeyControl.KeyUp_Operation(e);
                COMMAND command = COMMAND.NONE;
                if (ucKeyControl.sequence == KEY_SEQUENCE.ORDER)
                {// 仕分画面
                    if (e.Key == Key.F1)
                    {// 作業モニタータブ
                        command = COMMAND.TAB_OPERATION;
                    }
                    else if (e.Key == Key.F2)
                    {// 進捗モニタータブ
                        command = COMMAND.TAB_PROGRESS;
                    }
                    else if (e.Key == Key.F3)
                    {// 商品仕分一覧タブ
                        command = COMMAND.TAB_WORK_STATUS;
                    }
                    else if (e.Key == Key.F4)
                    {// エラー履歴タブ
                        command = COMMAND.TAB_ERROR_HISTORY;
                    }
                    else if (e.Key == Key.F6)
                    {// 便選択 1便
                        command = COMMAND.POST_01;
                    }
                    else if (e.Key == Key.F7)
                    {// 便選択 2便
                        command = COMMAND.POST_02;
                    }
                    else if (e.Key == Key.F8)
                    {// 便選択 3便
                        command = COMMAND.POST_03;
                    }
                    else if (e.Key == Key.F11)
                    {// エラー確認
                        command = COMMAND.ERROR_CHECKED;
                    }
                    else if (e.Key == Key.F12)
                    {// 閉じる
                        command = COMMAND.CLOSE;
                    }
                }


                if (command != COMMAND.NONE)
                    Function(command);

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
            finally
            {
                _keyLock = false;
            }
        }
        #endregion


        /// <summary>
        /// Check Error State
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        private static bool STATUS_SUCCESS(UInt32 err) { return err == (int)ErrorCodeList.STATUS_SUCCESS; }


    }
}
