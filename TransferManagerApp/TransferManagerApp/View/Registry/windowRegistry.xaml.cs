//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Input;

using PLCModule;
using ServerModule;
using DL_CommonLibrary;
using DL_Logger;
using SystemConfig;
using ShareResource;
using ErrorCodeDefine;


namespace TransferManagerApp
{
    /// <summary>
    /// windowRegistry.xaml の相互作用ロジック
    /// </summary>
    public partial class windowRegistry : Window
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
        /// コンストラクタ
        /// </summary>
        public windowRegistry()
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

                // キーボード表示テキスト
                ucKeyControl.sequence = KEY_SEQUENCE.REGIST;

                // ボタン色セット
                btnShelfMaster.Background = (SolidColorBrush)Application.Current.Resources["ButtonColorRegistry"];

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
                //if (_tmrUpdateDisplay != null)
                //    _tmrUpdateDisplay.Stop();
                //_tmrUpdateDisplay = null;

                // ユーザーコントロールを閉じる
                //ucTab_OperationMonitor.Dispose();
                //ucTab_ProgressMonitor.Dispose();
                //ucTab_ProgressMonitor.Dispose();
                //ucTab_ErrorHistory.Dispose();
                //ucTab_OperationMonitor = null;
                //ucTab_ProgressMonitor = null;
                //ucTab_WorkStatus = null;
                //ucTab_ErrorHistory = null;

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

                if (ctrl == btnShelfMaster)
                {// 棚マスタ 選択
                    command = COMMAND.TAB_SHELF_MASTER;
                }
                else if (ctrl == btnClose)
                {// 閉じる
                    this.Close();
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
                if (command == COMMAND.TAB_SHELF_MASTER)
                {// 棚マスタ 選択
                    btnShelfMaster.Background = (SolidColorBrush)Application.Current.Resources["ButtonColorRegistry"];
                    ucTab_ShelfMaster.Visibility = Visibility.Visible;
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



        #region キーボード処理
        /// <summary>
        /// ユーザー操作処理 種別
        /// </summary>
        private enum COMMAND
        {
            NONE = 0,

            TAB_SHELF_MASTER,
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
                if (ucKeyControl.sequence == KEY_SEQUENCE.REGIST)
                {// 仕分画面
                    if (e.Key == Key.F1)
                    {// 棚マスタータブ
                        command = COMMAND.TAB_SHELF_MASTER;
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

    }
}
