//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;

using SystemConfig;
using DL_Logger;
using ErrorCodeDefine;
using ShareResource;


namespace TransferManagerApp
{
    /// <summary>
    /// ucKeyControl.xaml の相互作用ロジック
    /// </summary>
    public partial class ucKeyControl : UserControl, IDisposable
    {
        private const string THIS_NAME = "ucKeyControl";


        /// <summary>
        /// 画面更新タイマー
        /// </summary>
        private DispatcherTimer _tmrUpdateDisplay = null;
        /// <summary>
        /// タイマー重複ロック
        /// </summary>
        private bool _timeLock = false;

        /// <summary>
        /// キーボード 処理階層
        /// </summary>
        public KEY_SEQUENCE sequence = KEY_SEQUENCE.MAIN;

        /// <summary>
        /// F1~F12のマウスダウンの重複防止
        /// </summary>
        private bool _mouseDownLock = false;



        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ucKeyControl()
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
            try
            {

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, false);
            }
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
                //rc = (Int32)ErrorCodeList.EXCEPTION;
                //Resource.ErrorHandler(ex);
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

                string[] keyTextArray = new string[12];
                for (int i = 0; i < keyTextArray.Length; i++)
                    keyTextArray[i] = "";
                if (sequence == KEY_SEQUENCE.MAIN)
                {// ホーム画面
                    keyTextArray[0] = "仕分";
                    keyTextArray[1] = "照会/登録";
                    keyTextArray[10] = "エラー確認";
                    keyTextArray[11] = "アプリ終了";
                }
                else if (sequence == KEY_SEQUENCE.ORDER)
                {// 仕分け画面
                    keyTextArray[0] = "作業モニター";
                    keyTextArray[1] = "進捗モニター";
                    keyTextArray[2] = "商品仕分状況";
                    keyTextArray[3] = "エラー履歴";
                    keyTextArray[5] = "1便";
                    keyTextArray[6] = "2便";
                    keyTextArray[7] = "3便";
                    keyTextArray[10] = "エラー確認";
                    keyTextArray[11] = "閉じる";
                }
                else if (sequence == KEY_SEQUENCE.REGIST)
                {// 照会/登録
                    keyTextArray[0] = "棚マスター";
                    keyTextArray[10] = "エラー確認";
                    keyTextArray[11] = "閉じる";
                }

                lblF1.Content = keyTextArray[0];
                lblF2.Content = keyTextArray[1];
                lblF3.Content = keyTextArray[2];
                lblF4.Content = keyTextArray[3];
                lblF5.Content = keyTextArray[4];
                lblF6.Content = keyTextArray[5];
                lblF7.Content = keyTextArray[6];
                lblF8.Content = keyTextArray[7];
                lblF9.Content = keyTextArray[8];
                lblF10.Content = keyTextArray[9];
                lblF11.Content = keyTextArray[10];
                lblF12.Content = keyTextArray[11];

                if (lblF1.Content == "") lblF1.Background = Brushes.DimGray;
                else lblF1.Background = Brushes.LightCyan;
                if (lblF2.Content == "") lblF2.Background = Brushes.DimGray;
                else lblF2.Background = Brushes.LightCyan;
                if (lblF3.Content == "") lblF3.Background = Brushes.DimGray;
                else lblF3.Background = Brushes.LightCyan;
                if (lblF4.Content == "") lblF4.Background = Brushes.DimGray;
                else lblF4.Background = Brushes.LightCyan;
                if (lblF5.Content == "") lblF5.Background = Brushes.DimGray;
                else lblF5.Background = Brushes.LightCyan;
                if (lblF6.Content == "") lblF6.Background = Brushes.DimGray;
                else lblF6.Background = Brushes.LightCyan;
                if (lblF7.Content == "") lblF7.Background = Brushes.DimGray;
                else lblF7.Background = Brushes.LightCyan;
                if (lblF8.Content == "") lblF8.Background = Brushes.DimGray;
                else lblF8.Background = Brushes.LightCyan;
                if (lblF9.Content == "") lblF9.Background = Brushes.DimGray;
                else lblF9.Background = Brushes.LightCyan;
                if (lblF10.Content == "") lblF10.Background = Brushes.DimGray;
                else lblF10.Background = Brushes.LightCyan;
                if (lblF11.Content == "") lblF11.Background = Brushes.DimGray;
                else lblF11.Background = Brushes.LightCyan;
                if (lblF12.Content == "") lblF12.Background = Brushes.DimGray;
                else lblF12.Background = Brushes.LightCyan;

            }
            catch (Exception ex)
            {
                //Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                //Resource.ErrorHandler(ex);
            }
            finally
            {
                _timeLock = false;
            }
        }

        /// <summary>
        /// キーボード キーダウン アニメーション
        /// </summary>
        /// <returns></returns>
        public UInt32 KeyDown_Operation(KeyEventArgs e)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}() {e.Key}");
            try
            {
                if (e.Key == Key.F1)
                {
                    lblF1.Margin = new Thickness(6, 2, 6, 2);
                    lblF1.Opacity = 0.7;
                }
                else if (e.Key == Key.F2)
                {
                    lblF2.Margin = new Thickness(6, 2, 6, 2);
                    lblF2.Opacity = 0.7;
                }
                else if (e.Key == Key.F3)
                {
                    lblF3.Margin = new Thickness(6, 2, 6, 2);
                    lblF3.Opacity = 0.7;
                }
                else if (e.Key == Key.F4)
                {
                    lblF4.Margin = new Thickness(6, 2, 6, 2);
                    lblF4.Opacity = 0.7;
                }
                else if (e.Key == Key.F5)
                {
                    lblF5.Margin = new Thickness(6, 2, 6, 2);
                    lblF5.Opacity = 0.7;
                }
                else if (e.Key == Key.F6)
                {
                    lblF6.Margin = new Thickness(6, 2, 6, 2);
                    lblF6.Opacity = 0.7;
                }
                else if (e.Key == Key.F7)
                {
                    lblF7.Margin = new Thickness(6, 2, 6, 2);
                    lblF7.Opacity = 0.7;
                }
                else if (e.Key == Key.F8)
                {
                    lblF8.Margin = new Thickness(6, 2, 6, 2);
                    lblF8.Opacity = 0.7;
                }
                else if (e.Key == Key.F9)
                {
                    lblF9.Margin = new Thickness(6, 2, 6, 2);
                    lblF9.Opacity = 0.7;
                }
                else if (e.Key == Key.F10)
                {
                    lblF10.Margin = new Thickness(6, 2, 6, 2);
                    lblF10.Opacity = 0.7;
                }
                else if (e.Key == Key.F11)
                {
                    lblF11.Margin = new Thickness(6, 2, 6, 2);
                    lblF11.Opacity = 0.7;
                }
                else if (e.Key == Key.F12)
                {
                    lblF12.Margin = new Thickness(6, 2, 6, 2);
                    lblF12.Opacity = 0.7;
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
        /// キーボード キーアップ アニメーション
        /// </summary>
        /// <returns></returns>
        public UInt32 KeyUp_Operation(KeyEventArgs e)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}() {e.Key}");
            try
            {
                if (e.Key == Key.F1)
                {
                    lblF1.Margin = new Thickness(4, 0, 4, 4);
                    lblF1.Opacity = 1.0;
                }
                else if (e.Key == Key.F2)
                {
                    lblF2.Margin = new Thickness(4, 0, 4, 4);
                    lblF2.Opacity = 1.0;
                }
                else if (e.Key == Key.F3)
                {
                    lblF3.Margin = new Thickness(4, 0, 4, 4);
                    lblF3.Opacity = 1.0;
                }
                else if (e.Key == Key.F4)
                {
                    lblF4.Margin = new Thickness(4, 0, 4, 4);
                    lblF4.Opacity = 1.0;
                }
                else if (e.Key == Key.F5)
                {
                    lblF5.Margin = new Thickness(4, 0, 4, 4);
                    lblF5.Opacity = 1.0;
                }
                else if (e.Key == Key.F6)
                {
                    lblF6.Margin = new Thickness(4, 0, 4, 4);
                    lblF6.Opacity = 1.0;
                }
                else if (e.Key == Key.F7)
                {
                    lblF7.Margin = new Thickness(4, 0, 4, 4);
                    lblF7.Opacity = 1.0;
                }
                else if (e.Key == Key.F8)
                {
                    lblF8.Margin = new Thickness(4, 0, 4, 4);
                    lblF8.Opacity = 1.0;
                }
                else if (e.Key == Key.F9)
                {
                    lblF9.Margin = new Thickness(4, 0, 4, 4);
                    lblF9.Opacity = 1.0;
                }
                else if (e.Key == Key.F10)
                {
                    lblF10.Margin = new Thickness(4, 0, 4, 4);
                    lblF10.Opacity = 1.0;
                }
                else if (e.Key == Key.F11)
                {
                    lblF11.Margin = new Thickness(4, 0, 4, 4);
                    lblF11.Opacity = 1.0;
                }
                else if (e.Key == Key.F12)
                {
                    lblF12.Margin = new Thickness(4, 0, 4, 4);
                    lblF12.Opacity = 1.0;
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
        /// マウスダウン イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseDown_Operation(object sender, MouseButtonEventArgs e)
        {
            UInt32 rc = 0;
            Label ctrl = (Label)sender;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {ctrl.Name}");
            try
            {
                if (!_mouseDownLock)
                {
                    // 連続押下防止
                    _mouseDownLock = true;


                    // 押されたラベルからキーを決定
                    Key key = Key.None;
                    if (ctrl == lblF1) key = Key.F1;
                    else if (ctrl == lblF2) key = Key.F2;
                    else if (ctrl == lblF3) key = Key.F3;
                    else if (ctrl == lblF4) key = Key.F4;
                    else if (ctrl == lblF5) key = Key.F5;
                    else if (ctrl == lblF6) key = Key.F6;
                    else if (ctrl == lblF7) key = Key.F7;
                    else if (ctrl == lblF8) key = Key.F8;
                    else if (ctrl == lblF9) key = Key.F9;
                    else if (ctrl == lblF10) key = Key.F10;
                    else if (ctrl == lblF11) key = Key.F11;
                    else if (ctrl == lblF12) key = Key.F12;
                    // キーボードイベントを生成
                    var down = new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(this), 0, key) { RoutedEvent = Keyboard.KeyDownEvent };
                    var up = new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(this), 0, key) { RoutedEvent = Keyboard.KeyUpEvent };
                    // キーボードイベントを発生させる
                    InputManager.Current.ProcessInput(down);
                    Thread.Sleep(200);
                    InputManager.Current.ProcessInput(up);
                }

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
            }
            finally
            {
                _mouseDownLock = false;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
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
