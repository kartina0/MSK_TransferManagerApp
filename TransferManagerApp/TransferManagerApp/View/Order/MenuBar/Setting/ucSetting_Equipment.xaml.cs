//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using SystemConfig;
using DL_Logger;
using ErrorCodeDefine;
using ShareResource;
using System.Data.Common;


namespace TransferManagerApp
{
    /// <summary>
    /// ucSetting_Equipment.xaml の相互作用ロジック
    /// </summary>
    public partial class ucSetting_Equipment : UserControl, IDisposable
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "ucSetting_Equipment";

        /// <summary>
        /// 画面更新タイマー
        /// </summary>
        private DispatcherTimer _tmrUpdateDisplay = null;
        /// <summary>
        /// タイマー重複ロック
        /// </summary>
        private bool _timeLock = false;

        /// <summary>
        /// アイル 有効/無効
        /// ボタンコントロールの動的作成
        /// </summary>
        private Button[] _btnAisleEnable = null;
        /// <summary>
        /// ユニット 有効/無効
        /// ボタンコントロールの動的作成
        /// </summary>
        private Button[][] _btnUnitEnable = null;



        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ucSetting_Equipment()
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
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                    return;

                // ------------------------------
                // アイル有効無効ボタン
                // 動的作成
                // ------------------------------
                _btnAisleEnable = new Button[Const.MaxAisleCount];
                for (int i = 0; i < Const.MaxAisleCount; i++)
                {
                    Button b = new Button();
                    b.Content = $"アイル{i + 1}";
                    b.Margin = new Thickness(5, 0, 5, 0);
                    b.Width = 90;
                    b.Height = 45;
                    b.Background = Brushes.Cyan;
                    b.BorderBrush = Brushes.Black;
                    b.BorderThickness = new Thickness(1, 1, 1, 1);
                    b.FontSize = 20;
                    b.HorizontalContentAlignment = HorizontalAlignment.Center;
                    b.VerticalContentAlignment = VerticalAlignment.Center;
                    b.Click += Button_Click;
                    if (!panelAisleEnable.Children.Contains(b))
                        panelAisleEnable.Children.Add(b);

                    // 有効無効の背景色設定
                    if (IniFile.AisleEnable[i])
                        b.Background = Brushes.SpringGreen;
                    else
                        b.Background = Brushes.Gray;

                    _btnAisleEnable[i] = b;
                }

                // ------------------------------
                // ユニット有効無効ボタン
                // 動的作成
                // ------------------------------
                StackPanel[] panels = new StackPanel[Const.MaxAisleCount] 
                {
                    panelAisle01, 
                    panelAisle02, 
                    panelAisle03, 
                    panelAisle04,
                };
                _btnUnitEnable = new Button[Const.MaxAisleCount][];
                for (int i = 0; i < Const.MaxAisleCount; i++) 
                {
                    _btnUnitEnable[i] = new Button[Const.MaxUnitCount];
                    for (int j = 0; j < Const.MaxUnitCount; j++) 
                    {
                        Button b = new Button();
                        b.Content = $"ユニット{j + 1}";
                        b.Margin = new Thickness(5, 0, 5, 0);
                        b.Width = 90;
                        b.Height = 45;
                        b.Background = Brushes.Cyan;
                        b.BorderBrush = Brushes.Black;
                        b.BorderThickness = new Thickness(1, 1, 1, 1);
                        b.FontSize = 20;
                        b.HorizontalContentAlignment = HorizontalAlignment.Center;
                        b.VerticalContentAlignment = VerticalAlignment.Center;
                        b.Click += Button_Click;
                        if (!panels[i].Children.Contains(b))
                            panels[i].Children.Add(b);

                        // 有効無効の背景色設定
                        if (IniFile.UnitEnable[i][j])
                            b.Background = Brushes.SpringGreen;
                        else
                            b.Background = Brushes.Gray;

                        _btnUnitEnable[i][j] = b;
                    }
                    
                }


                // 初期化


                // 画面更新タイマー初期化
                rc = InitTimer();

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, false);
            }
        }
        /// <summary>
        /// ウィンドウクローズ
        /// </summary>
        public void Dispose()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
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

                if (_btnAisleEnable.Contains(ctrl)) 
                {
                    for (int i = 0; i < Const.MaxAisleCount; i++) 
                    {
                        if (ctrl == _btnAisleEnable[i]) 
                        {
                            if(ctrl.Background == Brushes.SpringGreen)
                                _btnAisleEnable[i].Background = Brushes.Gray;
                            else
                                _btnAisleEnable[i].Background = Brushes.SpringGreen;
                        }
                    }
                }

                for (int i = 0; i < Const.MaxAisleCount; i++) 
                {
                    if (_btnUnitEnable[i].Contains(ctrl))
                    {
                        for (int j = 0; j < Const.MaxUnitCount; j++)
                        {
                            if (ctrl == _btnUnitEnable[i][j]) 
                            {
                                if (ctrl.Background == Brushes.SpringGreen)
                                    _btnUnitEnable[i][j].Background = Brushes.Gray;
                                else
                                    _btnUnitEnable[i][j].Background = Brushes.SpringGreen;
                            }
                        }
                        break;
                    }
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
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
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
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
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
        /// パラメータ保存
        /// </summary>
        /// <returns></returns>
        public UInt32 SaveParameter()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // アイル有効無効
                for (int i = 0; i < Const.MaxAisleCount; i++)
                {
                    if (_btnAisleEnable[i].Background == Brushes.SpringGreen)
                        IniFile.AisleEnable[i] = true;
                    else
                        IniFile.AisleEnable[i] = false;
                }

                // ユニット有効無効ボタン
                for (int i = 0; i < Const.MaxAisleCount; i++)
                {
                    for (int j = 0; j < Const.MaxUnitCount; j++)
                    {
                        if (_btnUnitEnable[i][j].Background == Brushes.SpringGreen)
                            IniFile.UnitEnable[i][j] = true;
                        else
                            IniFile.UnitEnable[i][j] = false;
                    }
                }

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
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
