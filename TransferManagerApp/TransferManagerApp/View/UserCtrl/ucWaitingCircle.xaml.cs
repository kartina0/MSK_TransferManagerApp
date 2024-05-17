using DL_Logger;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

using ErrorCodeDefine;
using ShareResource;


namespace TransferManagerApp
{
    /// <summary>
    /// ucWaitingCircle.xaml の相互作用ロジック
    /// </summary>
    public partial class ucWaitingCircle : UserControl, IDisposable
    {
        private const string THIS_NAME = "ucWaitingCircle";


        /// <summary>
        /// 画面更新タイマー
        /// </summary>
        private DispatcherTimer _tmrUpdateDisplay = null;
        /// <summary>
        /// タイマー重複ロック
        /// </summary>
        private bool _timeLock = false;

        /// <summary>
        /// Canvas現在角度
        /// </summary>
        private double rotationAngle = 0;
        /// <summary>
        /// Canvas回転角度
        /// </summary>
        private const double rotationSpeed = 1;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ucWaitingCircle()
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
            Logger.WriteLog(LogType.CONTROL, $"{GetType().Name} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                    return;

                // Canvas初期化
                rc = InitCanvas();
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
            Logger.WriteLog(LogType.CONTROL, $"{GetType().Name} {MethodBase.GetCurrentMethod().Name}()");

        }
        /// <summary>
        /// Canvas初期化
        /// </summary>
        private UInt32 InitCanvas()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                double x = 0;
                double y = 0;
                double length = 35.0;

                int radius = 15;
                double angle = Math.PI / 6.0;
                for (int i = 0; i < 10; i++)
                {
                    Ellipse ellipse = new Ellipse
                    {
                        Width = radius,
                        Height = radius,
                        //Stroke = Brushes.AliceBlue,
                        Fill = Brushes.AliceBlue,
                        Opacity = 1.0 - (0.12 * i),
                    };
                    x = length * Math.Cos(angle * i);
                    y = length * Math.Sin(angle * i);
                    Canvas.SetLeft(ellipse, (canvas01.Width / 2.0) + x - (ellipse.Width / 2.0));
                    Canvas.SetTop(ellipse, (canvas01.Height / 2.0) - y - (ellipse.Height / 2.0));
                    canvas01.Children.Add(ellipse);

                    radius--;
                }

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{GetType().Name}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 画面更新タイマー初期化
        /// </summary>
        private UInt32 InitTimer()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // タイマのインスタンスを生成
                _tmrUpdateDisplay = new DispatcherTimer();
                // インターバルを設定
                _tmrUpdateDisplay.Interval = TimeSpan.FromMilliseconds(10);
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
            Logger.WriteLog(LogType.METHOD_OUT, $"{GetType().Name}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
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

                // Canvasの中心座標を取得
                double centerX = canvas01.ActualWidth / 2;
                double centerY = canvas01.ActualHeight / 2;
                // Canvas角度をインクリメント
                rotationAngle += rotationSpeed;

                // 回転の原点をCanvasの中心に設定するための変換
                RotateTransform rotateTransform = new RotateTransform(rotationAngle, centerX, centerY);
                // キャンバスを回転
                canvas01.RenderTransform = rotateTransform;

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
