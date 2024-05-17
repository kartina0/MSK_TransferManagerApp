//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Reflection;

using SystemConfig;
using ShareResource;
using DL_Logger;
using ErrorCodeDefine;


namespace TransferManagerApp
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class windowMain : Window
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "windowMain";

        /// <summary>
        /// 画面更新タイマー
        /// </summary>
        private DispatcherTimer _tmrUpdateDisplay = null;
        /// <summary>
        /// タイマー重複ロック
        /// </summary>
        private bool _timeLock = false;

        /// <summary>
        /// 仕分け画面オブジェクト
        /// </summary>
        private windowOrder _wOrder = null;
        /// <summary>
        /// 照会/登録画面オブジェクト
        /// </summary>
        private windowRegistry _wRegistry = null;

        /// <summary>
        /// ボタン連続操作防止フラグ
        /// </summary>
        private bool _buttonLock = false;



        /// <summary>
        /// コンストラクタ
        /// </summary>
        public windowMain()
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
            //Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // タイトルバーを消しても画面移動可能にする処理
                this.MouseLeftButtonDown += delegate { DragMove(); };

                // キーボード表示テキスト
                ucKeyControl.sequence = KEY_SEQUENCE.MAIN;

                // 画面更新タイマー初期化
                rc = InitTimer();
            }
            catch (Exception ex) 
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }

            if (!STATUS_SUCCESS(rc)) 
            {
                // アプリ終了 ※このあとロードイベントを抜けた後、クローズイベントが入ってくる 
                Resource.ErrorHandler(rc, true);
                Application.Current.Shutdown();
                return;
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
            //Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 画面更新タイマー終了
                if (_tmrUpdateDisplay != null)
                    _tmrUpdateDisplay.Stop();
                _tmrUpdateDisplay = null;

                // ユーザーコントロール クローズ
                ucHeader.Dispose();
                ucFooter.Dispose();
                ucKeyControl.Dispose();

                // Cycleスレッド終了
                Cycle.Close();
                // Serverスレッド終了
                ServerControl.Close();

                // Manager終了
                Manager.Close();
                // Resource 終了
                Resource.Close();
                // Logger 終了
                Logger.Close();
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// ボタン クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_buttonLock)
                return;
            _buttonLock = true;

            UInt32 rc = 0;
            Button ctrl = (Button)sender;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {ctrl.Name}");
            try
            {
                COMMAND command = COMMAND.NONE;

                if (ctrl == btnOrder)
                {// 仕分
                    command = COMMAND.ORDER;
                }
                else if (ctrl == btnRegistry)
                {// 照会・登録
                    command = COMMAND.REGISTRY;
                }
                else if (ctrl == btnExit)
                {// アプリ終了
                    command = COMMAND.EXIT;
                }

                if (command != COMMAND.NONE)
                    rc = Function(command);

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
            finally 
            {
                _buttonLock = false;
            }
        }
        /// <summary>
        /// ユーザー操作処理
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private UInt32 Function(COMMAND command)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {command.ToString()}");
            try
            {
                if (command == COMMAND.ORDER)
                {// 仕分け画面表示
                    this.IsHitTestVisible = false;
                    this.Visibility = Visibility.Hidden;

                    _wOrder = new windowOrder();
                    _wOrder.isShowing = true;
                    _wOrder.Show();
                }
                else if (command == COMMAND.REGISTRY)
                {// 照会・登録画面表示
                    this.IsHitTestVisible = false;
                    this.Visibility = Visibility.Hidden;

                    _wRegistry = new windowRegistry();
                    _wRegistry.isShowing = true;
                    _wRegistry.Show();
                }
                else if (command == COMMAND.ERROR_CHECKED)
                {// エラー確認
                    Resource.ClearError();
                }
                else if (command == COMMAND.EXIT)
                {// アプリ終了
                    MessageBoxResult result = MessageBox.Show("終了します。よろしいですか？", "確認", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
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
        /// 画面更新タイマー初期化
        /// </summary>
        private UInt32 InitTimer()
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
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

                // 初期化完了確認
                if (!Resource.SystemStatus.initialize_Completed) return;

                // 仕分画面が閉じられたら、この画面をVisibleにする
                if ((_wOrder == null || !_wOrder.isShowing) && (_wRegistry == null || !_wRegistry.isShowing)) 
                {
                    this.IsHitTestVisible = true;
                    this.Visibility = Visibility.Visible;
                }

            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");

                _tmrUpdateDisplay.Stop();
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
            ORDER,
            REGISTRY,
            ERROR_CHECKED,
            EXIT,
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
                if (ucKeyControl.sequence == KEY_SEQUENCE.MAIN)
                {
                    if (e.Key == Key.F1)
                    {// 仕分
                        command = COMMAND.ORDER;
                    }
                    else if (e.Key == Key.F2)
                    {// 照会・登録
                        command = COMMAND.REGISTRY;
                    }
                    else if (e.Key == Key.F11)
                    {// エラー確認
                        command = COMMAND.ERROR_CHECKED;
                    }
                    else if (e.Key == Key.F12)
                    {// アプリ終了
                        command = COMMAND.EXIT;
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
        private static bool STATUS_SUCCESS(UInt32 err) { return err == (int)ErrorCodeList.STATUS_SUCCESS; }

    }



}
