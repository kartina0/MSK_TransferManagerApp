using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Shapes;
using System.Windows.Media;

using PLCModule;
using DL_Logger;
using SystemConfig;
using ShareResource;
using ErrorCodeDefine;


namespace TransferManagerApp
{
    /// <summary>
    /// ucAisleTopView.xaml の相互作用ロジック
    /// </summary>
    public partial class ucAisleTopView : UserControl, IDisposable
    {
        private const string THIS_NAME = "ucAisleTopView";


        /// <summary>
        /// アイルIndex
        /// </summary>
        public int aisleIndex = 0;

        /// <summary>
        /// 画面更新タイマー
        /// </summary>
        private DispatcherTimer _tmrUpdateDisplay = null;
        /// <summary>
        /// タイマー重複ロック
        /// </summary>
        private bool _timeLock = false;
        /// <summary>
        /// 起動時フラグ
        /// </summary>
        private bool _init = true;


        /// <summary>
        /// [コントロール] スロット01
        /// </summary>
        private Rectangle[] controlRectSlot01 = null;
        /// <summary>
        /// [コントロール] 番重交換サイン01
        /// </summary>
        private Polygon[] controlPolyArrow01 = null;

        /// <summary>
        /// [コントロール] スロット02
        /// </summary>
        private Rectangle[] controlRectSlot02 = null;
        /// <summary>
        /// [コントロール] 番重交換サイン02
        /// </summary>
        private Polygon[] controlPolyArrow02 = null;

        /// <summary>
        /// [コントロール] スロット03
        /// </summary>
        private Rectangle[] controlRectSlot03 = null;
        /// <summary>
        /// [コントロール] 番重交換サイン03
        /// </summary>
        private Polygon[] controlPolyArrow03 = null;




        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ucAisleTopView()
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
            //Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                    return;


                // ----------------------------------
                // ユニット01
                // ----------------------------------
                // スロットRECT
                int index = 0;
                controlRectSlot01 = new Rectangle[Const.MaxSlotCount];
                for (int row = 1; row < gridUnit01.RowDefinitions.Count; row += 2)
                {
                    for (int column = 1; column < gridUnit01.ColumnDefinitions.Count; column += 3)
                    {
                        controlRectSlot01[index] = new Rectangle();
                        controlRectSlot01[index].Fill = System.Windows.Media.Brushes.DimGray;
                        controlRectSlot01[index].SizeChanged += recSlot_SizeChanged;
                        controlRectSlot01[index].Stroke = Brushes.Black;
                        controlRectSlot01[index].StrokeThickness = 1;
                        Grid.SetRow(controlRectSlot01[index], row);
                        Grid.SetColumn(controlRectSlot01[index], column);
                        gridUnit01.Children.Add(controlRectSlot01[index]);

                        index++;
                    }
                }
                // 番重交換Arrow
                index = 0;
                controlPolyArrow01 = new Polygon[Const.MaxSlotCount];
                for (int row = 0; row < gridUnit01.RowDefinitions.Count; row += 3)
                {
                    for (int column = 1; column < gridUnit01.ColumnDefinitions.Count; column += 3)
                    {
                        controlPolyArrow01[index] = new Polygon();
                        controlPolyArrow01[index].Fill = System.Windows.Media.Brushes.OrangeRed;
                        controlPolyArrow01[index].HorizontalAlignment = HorizontalAlignment.Center;
                        controlPolyArrow01[index].Stroke = Brushes.Black;
                        controlPolyArrow01[index].StrokeThickness = 1;
                        controlPolyArrow01[index].Visibility = Visibility.Hidden;

                        if (row == 0)
                            controlPolyArrow01[index].VerticalAlignment = VerticalAlignment.Top;
                        else
                            controlPolyArrow01[index].VerticalAlignment = VerticalAlignment.Bottom;

                        controlPolyArrow01[index].Points.Add(new Point(5, 10));
                        controlPolyArrow01[index].Points.Add(new Point(0, 10));
                        controlPolyArrow01[index].Points.Add(new Point(10, 0));
                        controlPolyArrow01[index].Points.Add(new Point(20, 10));
                        controlPolyArrow01[index].Points.Add(new Point(15, 10));
                        controlPolyArrow01[index].Points.Add(new Point(15, 25));
                        controlPolyArrow01[index].Points.Add(new Point(20, 25));
                        controlPolyArrow01[index].Points.Add(new Point(10, 35));
                        controlPolyArrow01[index].Points.Add(new Point(0, 25));
                        controlPolyArrow01[index].Points.Add(new Point(5, 25));

                        Grid.SetRow(controlPolyArrow01[index], row);
                        Grid.SetRowSpan(controlPolyArrow01[index], 2);
                        Grid.SetColumn(controlPolyArrow01[index], column);

                        gridUnit01.Children.Add(controlPolyArrow01[index]);

                        index++;
                    }
                }

                // ----------------------------------
                // ユニット02
                // ----------------------------------
                // スロットRECT
                index = 0;
                controlRectSlot02 = new Rectangle[Const.MaxSlotCount];
                for (int row = 1; row < gridUnit02.RowDefinitions.Count; row += 2)
                {
                    for (int column = 1; column < gridUnit02.ColumnDefinitions.Count; column += 3)
                    {
                        controlRectSlot02[index] = new Rectangle();
                        controlRectSlot02[index].Fill = System.Windows.Media.Brushes.DimGray;
                        controlRectSlot02[index].SizeChanged += recSlot_SizeChanged;
                        controlRectSlot02[index].Stroke = Brushes.Black;
                        controlRectSlot02[index].StrokeThickness = 1;
                        Grid.SetRow(controlRectSlot02[index], row);
                        Grid.SetColumn(controlRectSlot02[index], column);
                        gridUnit02.Children.Add(controlRectSlot02[index]);

                        index++;
                    }
                }
                // 番重交換Arrow
                index = 0;
                controlPolyArrow02 = new Polygon[Const.MaxSlotCount];
                for (int row = 0; row < gridUnit02.RowDefinitions.Count; row += 3)
                {
                    for (int column = 1; column < gridUnit02.ColumnDefinitions.Count; column += 3)
                    {
                        controlPolyArrow02[index] = new Polygon();
                        controlPolyArrow02[index].Fill = System.Windows.Media.Brushes.OrangeRed;
                        controlPolyArrow02[index].HorizontalAlignment = HorizontalAlignment.Center;
                        controlPolyArrow02[index].Stroke = Brushes.Black;
                        controlPolyArrow02[index].StrokeThickness = 1;
                        controlPolyArrow02[index].Visibility = Visibility.Hidden;

                        if (row == 0)
                            controlPolyArrow02[index].VerticalAlignment = VerticalAlignment.Top;
                        else
                            controlPolyArrow02[index].VerticalAlignment = VerticalAlignment.Bottom;

                        controlPolyArrow02[index].Points.Add(new Point(5, 10));
                        controlPolyArrow02[index].Points.Add(new Point(0, 10));
                        controlPolyArrow02[index].Points.Add(new Point(10, 0));
                        controlPolyArrow02[index].Points.Add(new Point(20, 10));
                        controlPolyArrow02[index].Points.Add(new Point(15, 10));
                        controlPolyArrow02[index].Points.Add(new Point(15, 25));
                        controlPolyArrow02[index].Points.Add(new Point(20, 25));
                        controlPolyArrow02[index].Points.Add(new Point(10, 35));
                        controlPolyArrow02[index].Points.Add(new Point(0, 25));
                        controlPolyArrow02[index].Points.Add(new Point(5, 25));

                        Grid.SetRow(controlPolyArrow02[index], row);
                        Grid.SetRowSpan(controlPolyArrow02[index], 2);
                        Grid.SetColumn(controlPolyArrow02[index], column);

                        gridUnit02.Children.Add(controlPolyArrow02[index]);

                        index++;
                    }
                }

                // ----------------------------------
                // ユニット03
                // ----------------------------------
                // スロットRECT
                index = 0;
                controlRectSlot03 = new Rectangle[Const.MaxSlotCount];
                for (int row = 1; row < gridUnit03.RowDefinitions.Count; row += 2)
                {
                    for (int column = 1; column < gridUnit03.ColumnDefinitions.Count; column += 3)
                    {
                        controlRectSlot03[index] = new Rectangle();
                        controlRectSlot03[index].Fill = System.Windows.Media.Brushes.DimGray;
                        controlRectSlot03[index].SizeChanged += recSlot_SizeChanged;
                        controlRectSlot03[index].Stroke = Brushes.Black;
                        controlRectSlot03[index].StrokeThickness = 1;
                        Grid.SetRow(controlRectSlot03[index], row);
                        Grid.SetColumn(controlRectSlot03[index], column);
                        gridUnit03.Children.Add(controlRectSlot03[index]);

                        index++;
                    }
                }
                // 番重交換Arrow
                index = 0;
                controlPolyArrow03 = new Polygon[Const.MaxSlotCount];
                for (int row = 0; row < gridUnit03.RowDefinitions.Count; row += 3)
                {
                    for (int column = 1; column < gridUnit03.ColumnDefinitions.Count; column += 3)
                    {
                        controlPolyArrow03[index] = new Polygon();
                        controlPolyArrow03[index].Fill = System.Windows.Media.Brushes.OrangeRed;
                        controlPolyArrow03[index].HorizontalAlignment = HorizontalAlignment.Center;
                        controlPolyArrow03[index].Stroke = Brushes.Black;
                        controlPolyArrow03[index].StrokeThickness = 1;
                        controlPolyArrow03[index].Visibility = Visibility.Hidden;

                        if (row == 0)
                            controlPolyArrow03[index].VerticalAlignment = VerticalAlignment.Top;
                        else
                            controlPolyArrow03[index].VerticalAlignment = VerticalAlignment.Bottom;

                        controlPolyArrow03[index].Points.Add(new Point(5, 10));
                        controlPolyArrow03[index].Points.Add(new Point(0, 10));
                        controlPolyArrow03[index].Points.Add(new Point(10, 0));
                        controlPolyArrow03[index].Points.Add(new Point(20, 10));
                        controlPolyArrow03[index].Points.Add(new Point(15, 10));
                        controlPolyArrow03[index].Points.Add(new Point(15, 25));
                        controlPolyArrow03[index].Points.Add(new Point(20, 25));
                        controlPolyArrow03[index].Points.Add(new Point(10, 35));
                        controlPolyArrow03[index].Points.Add(new Point(0, 25));
                        controlPolyArrow03[index].Points.Add(new Point(5, 25));

                        Grid.SetRow(controlPolyArrow03[index], row);
                        Grid.SetRowSpan(controlPolyArrow03[index], 2);
                        Grid.SetColumn(controlPolyArrow03[index], column);

                        gridUnit03.Children.Add(controlPolyArrow03[index]);

                        index++;
                    }
                }


                // 画面更新タイマー初期化
                rc = InitTimer();
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                //Resource.ErrorHandler(ex, true);

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
            //Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
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
                _tmrUpdateDisplay.Interval = TimeSpan.FromMilliseconds(300);
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
            PLCMachineStatus status = new PLCMachineStatus();
            try
            {
                // 重複防止
                if (_timeLock)
                    return;
                _timeLock = true;


                // PLC設備ステータス 読み取り
                if(IniFile.AisleEnable[aisleIndex])
                    rc = Resource.Plc[aisleIndex].Access.GetMachineStatus(out status);

                // コントロール背景色 更新
                if (STATUS_SUCCESS(rc))
                {
                    // ----------------------------------
                    // 番重供給ユニット
                    polySupply01.Fill = GetColor_SupplyUnit(IniFile.AisleEnable[aisleIndex], status.SupplyUnitStatus_L);
                    polySupply02.Fill = GetColor_SupplyUnit(IniFile.AisleEnable[aisleIndex], status.SupplyUnitStatus_R);


                    // ----------------------------------
                    // アイル
                    recUnit01L.Fill = GetColor_Unit(IniFile.AisleEnable[aisleIndex] && IniFile.UnitEnable[aisleIndex][0], status.Unit01Status_L);
                    recUnit01R.Fill = GetColor_Unit(IniFile.AisleEnable[aisleIndex] && IniFile.UnitEnable[aisleIndex][0], status.Unit01Status_R);
                    recUnit02L.Fill = GetColor_Unit(IniFile.AisleEnable[aisleIndex] && IniFile.UnitEnable[aisleIndex][1], status.Unit02Status_L);
                    recUnit02R.Fill = GetColor_Unit(IniFile.AisleEnable[aisleIndex] && IniFile.UnitEnable[aisleIndex][1], status.Unit02Status_R);
                    recUnit03L.Fill = GetColor_Unit(IniFile.AisleEnable[aisleIndex] && IniFile.UnitEnable[aisleIndex][2], status.Unit03Status_L);
                    recUnit03R.Fill = GetColor_Unit(IniFile.AisleEnable[aisleIndex] && IniFile.UnitEnable[aisleIndex][2], status.Unit03Status_R);

                    // ----------------------------------
                    // スロット
                    for (int i = 0; i < Const.MaxSlotCount; i++)
                    {
                        controlRectSlot01[i].Fill = GetColor_Slot(IniFile.AisleEnable[aisleIndex] && IniFile.UnitEnable[aisleIndex][0], status.SlotStatus[i]);
                        controlRectSlot02[i].Fill = GetColor_Slot(IniFile.AisleEnable[aisleIndex] && IniFile.UnitEnable[aisleIndex][1], status.SlotStatus[i + 12]);
                        controlRectSlot03[i].Fill = GetColor_Slot(IniFile.AisleEnable[aisleIndex] && IniFile.UnitEnable[aisleIndex][2], status.SlotStatus[i + 24]);
                    }


                }

            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
            }
            finally
            {
                _timeLock = false;
            }
        }






        /// <summary>
        /// Rectangleの縦横比調整
        /// 形状を正方形にする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recSlot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UInt32 rc = 0;
            Rectangle ctrl = (Rectangle)sender;
            //Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {ctrl.Name}");
            try
            {
                for (int i = 0; i < Const.MaxSlotCount; i++)
                {
                    if (ctrl.Name == controlRectSlot01[i].Name)
                    {
                        // コントロールのHeightをWidthに合わせる
                        controlRectSlot01[i].Height = controlRectSlot01[i].ActualWidth;
                    }
                    if (ctrl.Name == controlRectSlot02[i].Name)
                    {
                        // コントロールのHeightをWidthに合わせる
                        controlRectSlot02[i].Height = controlRectSlot02[i].ActualWidth;
                    }
                    if (ctrl.Name == controlRectSlot03[i].Name)
                    {
                        // コントロールのHeightをWidthに合わせる
                        controlRectSlot03[i].Height = controlRectSlot03[i].ActualWidth;
                    }
                }

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
            return;
        }


        /// <summary>
        /// 番重供給ユニット 背景色設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private SolidColorBrush GetColor_SupplyUnit(bool enable, SUPPLY_UNIT_STATUS status)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {ctrl.Name}");
            SolidColorBrush color = null;
            try
            {
                if (!enable)
                {// 無効
                    color = Brushes.DimGray;
                }
                else 
                {// 有効
                    if (status == SUPPLY_UNIT_STATUS.NONE || status == SUPPLY_UNIT_STATUS.STOP)
                        color = Brushes.MediumBlue;
                    else if (status == SUPPLY_UNIT_STATUS.NORMAL)
                        color = Brushes.ForestGreen;
                    else if (status == SUPPLY_UNIT_STATUS.ERROR)
                        color = Brushes.Red;
                }

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, false);
            }
            return color;
        }
        /// <summary>
        /// ユニット 背景色設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private SolidColorBrush GetColor_Unit(bool enable, UNIT_STATUS status)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {ctrl.Name}");
            SolidColorBrush color = null;
            try
            {
                if (!enable)
                {// 無効
                    color = Brushes.DarkGray;
                }
                else 
                {// 有効
                    if (status == UNIT_STATUS.NONE || status == UNIT_STATUS.STOP)
                    {
                        color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5080F0"));
                        //color = Brushes.RoyalBlue;
                    }
                    else if (status == UNIT_STATUS.NORMAL)
                    {
                        color = Brushes.LimeGreen;
                    }
                    else if (status == UNIT_STATUS.ERROR) 
                    {
                        color = Brushes.Red;
                    }
                }

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, false);
            }
            return color;
        }
        /// <summary>
        /// スロット 背景色設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private SolidColorBrush GetColor_Slot(bool enable, SLOT_STATUS status)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {ctrl.Name}");
            SolidColorBrush color = null;
            try
            {
                if (!enable)
                {// 無効
                    color = Brushes.DimGray;
                }
                else 
                {// 有効
                    if (status == SLOT_STATUS.NONE || status == SLOT_STATUS.STOP)
                        color = Brushes.MediumBlue;
                    else if (status == SLOT_STATUS.NORMAL)
                        color = Brushes.ForestGreen;
                    else if (status == SLOT_STATUS.ERROR)
                        color = Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, false);
            }
            return color;
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
