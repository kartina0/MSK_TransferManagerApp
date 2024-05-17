//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using SystemConfig;
using DL_Logger;
using ErrorCodeDefine;
using ShareResource;
using BatchModule;
using TransferManagerApp_Debug;


namespace TransferManagerApp
{
    /// <summary>
    /// UserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class ucTab_ShelfMaster : UserControl, IDisposable
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "ucTab_ShelfMaster";

        /// <summary>
        /// バッチボタン配列
        /// ボタンコントロールの動的作成
        /// </summary>
        private Button[] _btnBatchs = null;
        /// <summary>
        /// スロット情報配列
        /// スロット情報テーブルを各種コントロールで動的作成
        /// </summary>
        private BindSlotInfo[] _bindSlots = null;

        /// <summary>
        /// 有効開始日
        /// </summary>
        private DateTime _orderStartDt = DateTime.MinValue;

        /// <summary>
        /// 選択した便No
        /// </summary>
        private int _selectedPostIndex = 0;
        /// <summary>
        /// 選択したアイルNo
        /// </summary>
        private int _selectedAisleIndex = 0;
        /// <summary>
        /// 選択したバッチNo
        /// </summary>
        private int _selectedBatchIndex = 0;
        /// <summary>
        /// 選択したユニットNo
        /// </summary>
        private int _selectedUnitIndex = 0;

        /// <summary>
        /// スロット情報テーブル(右画面) 更新フラグ
        /// </summary>
        private bool _selectBatchChanged = false;

        /// <summary>
        /// 現在編集中のバッチ情報を保持しておく
        /// </summary>
        private Batch _currentEditBatch = null;

        /// <summary>
        /// 当期が選択されているか
        /// </summary>
        private bool _currentBatchSelected = true;

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
        public ucTab_ShelfMaster()
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


                // バッチボタンコントロール 動的作成
                _btnBatchs = new Button[Const.MaxPostCount * Const.MaxAisleCount * Const.MaxBatchCount];
                Thickness pad = new Thickness(0, 0, 0, 0);
                int index = 0;
                for (int row = 0; row < Const.MaxPostCount * Const.MaxAisleCount; row++)
                {
                    for (int column = 0; column < Const.MaxBatchCount; column++)
                    {
                        Button btnBatch = new Button();
                        btnBatch.Padding = pad;
                        btnBatch.Content = $"{column + 1}";
                        btnBatch.Background = Brushes.Orange;
                        btnBatch.BorderBrush = Brushes.Gray;
                        btnBatch.BorderThickness = new Thickness(1, 1, 1, 1);
                        btnBatch.FontSize = 16;
                        btnBatch.HorizontalContentAlignment = HorizontalAlignment.Center;
                        btnBatch.VerticalContentAlignment = VerticalAlignment.Center;
                        btnBatch.IsEnabled = false;
                        btnBatch.Click += BtnBatch_Click;
                        btnBatch.SetValue(Grid.RowProperty, row + 1);
                        btnBatch.SetValue(Grid.ColumnProperty, column + 3);
                        if (!gridBatchTable.Children.Contains(btnBatch))
                            gridBatchTable.Children.Add(btnBatch);

                        _btnBatchs[index] = btnBatch;
                        index++;
                    }
                }

                // スロット情報テーブル 動的作成
                _bindSlots = new BindSlotInfo[Const.MaxSlotCount];
                for (int i = 0; i < Const.MaxSlotCount; i++) 
                {
                    BindSlotInfo slotInfo = new BindSlotInfo();

                    // スロットNo
                    slotInfo.lblSlotNo = new Label();
                    slotInfo.lblSlotNo.Content = i + 1;
                    slotInfo.lblSlotNo.Background = Brushes.AntiqueWhite;
                    slotInfo.lblSlotNo.BorderBrush = Brushes.Gray;
                    slotInfo.lblSlotNo.BorderThickness = new Thickness(1, 1, 1, 1);
                    slotInfo.lblSlotNo.FontSize = 16;
                    slotInfo.lblSlotNo.HorizontalContentAlignment = HorizontalAlignment.Center;
                    slotInfo.lblSlotNo.VerticalContentAlignment = VerticalAlignment.Center;
                    slotInfo.lblSlotNo.SetValue(Grid.RowProperty, i + 1);
                    slotInfo.lblSlotNo.SetValue(Grid.ColumnProperty, 0);
                    if (!gridSlotInfo.Children.Contains(slotInfo.lblSlotNo))
                        gridSlotInfo.Children.Add(slotInfo.lblSlotNo);

                    // 店コード
                    slotInfo.txtStoreCode = new TextBox();
                    slotInfo.txtStoreCode.Text = "";
                    slotInfo.txtStoreCode.Background = Brushes.White;
                    slotInfo.txtStoreCode.BorderBrush = Brushes.Gray;
                    slotInfo.txtStoreCode.BorderThickness = new Thickness(1, 1, 1, 1);
                    slotInfo.txtStoreCode.FontSize = 16;
                    slotInfo.txtStoreCode.HorizontalContentAlignment = HorizontalAlignment.Center;
                    slotInfo.txtStoreCode.VerticalContentAlignment = VerticalAlignment.Center;
                    slotInfo.txtStoreCode.SetValue(Grid.RowProperty, i + 1);
                    slotInfo.txtStoreCode.SetValue(Grid.ColumnProperty, 1);
                    if (!gridSlotInfo.Children.Contains(slotInfo.txtStoreCode))
                        gridSlotInfo.Children.Add(slotInfo.txtStoreCode);

                    // 店名
                    slotInfo.lblStoreName = new Label();
                    slotInfo.lblStoreName.Content = "";
                    slotInfo.lblStoreName.Background = Brushes.White;
                    slotInfo.lblStoreName.BorderBrush = Brushes.Gray;
                    slotInfo.lblStoreName.BorderThickness = new Thickness(1, 1, 1, 1);
                    slotInfo.lblStoreName.FontSize = 16;
                    slotInfo.lblStoreName.HorizontalContentAlignment = HorizontalAlignment.Center;
                    slotInfo.lblStoreName.VerticalContentAlignment = VerticalAlignment.Center;
                    slotInfo.lblStoreName.SetValue(Grid.RowProperty, i + 1);
                    slotInfo.lblStoreName.SetValue(Grid.ColumnProperty, 2);
                    if (!gridSlotInfo.Children.Contains(slotInfo.lblStoreName))
                        gridSlotInfo.Children.Add(slotInfo.lblStoreName);

                    _bindSlots[i] = slotInfo;
                }
                borderSlotInfo.Visibility = Visibility.Hidden;

                // ユニットコンボ


                // 当期or次期
                _currentBatchSelected = true;

                _btnBatchs[0].RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

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
                if (ctrl == btnSave)
                {// 保存
                    // 画面に表示しているユニットNoのバッチ情報を
                    // バッチファイルに上書き保存する
                    rc = SaveBatchInfo();

                    rc = Resource.batch.Save();
                }
                else if (ctrl == btnCurrent)
                {// 当日バッチ 選択
                    btnCurrent.Background = (SolidColorBrush)Application.Current.Resources["ButtonColorRegistry"];
                    btnCurrent.Background = Brushes.SlateGray;
                }
                else if (ctrl == btnNext)
                {// 翌日バッチ 選択
                    btnNext.Background = Brushes.SlateGray;
                    btnNext.Background = (SolidColorBrush)Application.Current.Resources["ButtonColorRegistry"];
                }
                else if (ctrl == btnCancel)
                {// キャンセル
                    borderSlotInfo.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
        }
        /// <summary>
        /// バッチボタン クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBatch_Click(object sender, RoutedEventArgs e)
        {
            UInt32 rc = 0;
            Button ctrl = (Button)sender;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {ctrl.Name}");
            try
            {
                int index = 0;
                for (int postIndex = 0; postIndex < Const.MaxPostCount; postIndex++)
                {
                    for (int aisleIndex = 0; aisleIndex < Const.MaxAisleCount; aisleIndex++)
                    {
                        for (int batchIndex = 0; batchIndex < Const.MaxBatchCount; batchIndex++)
                        {
                            if (ctrl == _btnBatchs[index]) 
                            {
                                // 選択したバッチの情報をセット
                                _selectedPostIndex = postIndex;
                                _selectedAisleIndex = aisleIndex;
                                _selectedBatchIndex = batchIndex;
                                lblPostNo.Content = _selectedPostIndex + 1;
                                lblAisleNo.Content = _selectedAisleIndex + 1;
                                lblBatchNo.Content = _selectedBatchIndex + 1;

                                // 編集用バッチオブジェクト作成
                                _currentEditBatch = null;
                                _currentEditBatch = new Batch();

                                // ユニットNo
                                _selectedUnitIndex = 0;
                                comboUnitNo.SelectedIndex = _selectedUnitIndex;

                                // 新規バッチか、既存のバッチか
                                if (_selectedBatchIndex + 1 > Resource.batch.BatchInfoCurrent.Post[_selectedPostIndex].Aisle[_selectedAisleIndex].Batch.Count)
                                {// 新規バッチ
                                    
                                }
                                else
                                {// 既存のバッチ

                                    // 既存のスロット情報をコピーしておく
                                    string[] slotArray = Resource.batch.BatchInfoCurrent.Post[_selectedPostIndex]
                                                                       .Aisle[_selectedAisleIndex]
                                                                       .Batch[_selectedBatchIndex]
                                                                       .OutputToArray();
                                    _currentEditBatch.InputFromArray(slotArray);
                                }

                                // 右画面のスロット情報テーブルを表示
                                borderSlotInfo.Visibility = Visibility.Visible;
                                // テーブル更新フラグ
                                _selectBatchChanged = true;
                            }
                            index++;
                        }
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
        /// ユニットNo コンボボックス選択イベント 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboUnitNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 画面のスロット情報を保持
                KeepDisplaySlotInfo();

                // ユニットNo変更
                _selectedUnitIndex = comboUnitNo.SelectedIndex;
                // 右画面テーブル更新フラグ
                _selectBatchChanged = true;
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
                //初期化完了確認
                if (!Resource.SystemStatus.initialize_Completed) return;



                ///左画面
                // 当期次期
                if (_currentBatchSelected)
                {
                    btnCurrent.Background = (SolidColorBrush)Application.Current.Resources["ButtonColorRegistry"];
                    btnNext.Background = Brushes.DarkGray;
                }
                else
                {
                    btnCurrent.Background = Brushes.DarkGray;
                    btnNext.Background = (SolidColorBrush)Application.Current.Resources["ButtonColorRegistry"];
                }

                // 有効開始日
                _orderStartDt = (DateTime)dpOrderDate.SelectedDate;
                // バッチボタン更新
                int index = 0;
                for (int postIndex = 0; postIndex < Const.MaxPostCount; postIndex++) 
                {
                    for (int aisleIndex = 0; aisleIndex < Const.MaxAisleCount; aisleIndex++) 
                    {
                        for (int batchIndex = 0; batchIndex < Const.MaxBatchCount; batchIndex++) 
                        {
                            if (IniFile.AisleEnable[aisleIndex])
                            {// アイル有効
                                if (batchIndex + 1 <= Resource.batch.BatchInfoCurrent.Post[postIndex].Aisle[aisleIndex].Batch.Count)
                                {// バッチ有り
                                    if (_selectedPostIndex == postIndex && _selectedAisleIndex == aisleIndex && _selectedBatchIndex == batchIndex)
                                    {// 選択中のボタン
                                        _btnBatchs[index].IsEnabled = true;
                                        Color color = (Color)ColorConverter.ConvertFromString("#FFC040");
                                        _btnBatchs[index].Background = new SolidColorBrush(color);
                                        _btnBatchs[index].BorderBrush = Brushes.DarkOrange;
                                        _btnBatchs[index].BorderThickness = new Thickness(2);
                                        _btnBatchs[index].Content = batchIndex + 1;
                                    }
                                    else 
                                    {
                                        _btnBatchs[index].IsEnabled = true;
                                        _btnBatchs[index].Background = Brushes.Orange;
                                        Color color = (Color)ColorConverter.ConvertFromString("#FF808080");
                                        _btnBatchs[index].BorderBrush = new SolidColorBrush(color);
                                        _btnBatchs[index].BorderThickness = new Thickness(1);
                                        _btnBatchs[index].Content = batchIndex + 1;
                                    }
                                }
                                else if (batchIndex + 1 == Resource.batch.BatchInfoCurrent.Post[postIndex].Aisle[aisleIndex].Batch.Count + 1) 
                                {
                                    _btnBatchs[index].IsEnabled = true;
                                    _btnBatchs[index].Background = Brushes.PaleTurquoise;
                                    _btnBatchs[index].Content = "";
                                }
                                else
                                {// バッチ無し
                                    _btnBatchs[index].IsEnabled = false;
                                    _btnBatchs[index].Content = "";
                                }
                            }
                            else
                            {// アイル無効
                                _btnBatchs[index].IsEnabled = false;
                                _btnBatchs[index].Content = "";
                            }

                            index++;
                        }
                    }
                }


                ///右画面
                if (borderSlotInfo.Visibility == Visibility.Visible) 
                {
                    // 店コード
                    if (_selectBatchChanged) 
                    {
                        // バッチファイルから店コード読み出し
                        string[] storeCodeList = _currentEditBatch.OutputToArray();
                        for (int i = 0; i < Const.MaxSlotCount; i++)
                            _bindSlots[i].txtStoreCode.Text = storeCodeList[ (_selectedUnitIndex * Const.MaxSlotCount) + i ];

                        _selectBatchChanged = false;
                    }

                    // 店名
                    for (int i = 0; i < Const.MaxSlotCount; i++)
                    {
                        string storeCode = _bindSlots[i].txtStoreCode.Text;
                        var storeData = Resource.Server.MasterFile.MasterStoreList.Where(x => x.storeCode == storeCode).FirstOrDefault();
                        if (storeData != null)
                        {
                            _bindSlots[i].txtStoreCode.Background = Brushes.White;

                            string storeName = storeData.storeName;
                            _bindSlots[i].lblStoreName.Content = storeName;
                            _bindSlots[i].lblStoreName.Background = Brushes.White;
                        }
                        else 
                        {
                            _bindSlots[i].txtStoreCode.Background = Brushes.DarkSalmon;
                            _bindSlots[i].lblStoreName.Content = "";
                            _bindSlots[i].lblStoreName.Background = Brushes.DarkGray;
                        }

                    }

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
        /// バッチファイル保存
        /// 現在画面に表示しているスロット情報を上書き保存
        /// </summary>
        private UInt32 SaveBatchInfo()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                int postIndex = _selectedPostIndex;
                int aisleIndex = _selectedAisleIndex;
                int batchIndex = _selectedBatchIndex;
                int unitIndex = _selectedUnitIndex;

                // 現在表示中のスロット情報を保持
                KeepDisplaySlotInfo();

                if (_selectedBatchIndex + 1 > Resource.batch.BatchInfoCurrent.Post[_selectedPostIndex].Aisle[_selectedAisleIndex].Batch.Count)
                {// 新規バッチを追加保存
                    Resource.batch.BatchInfoCurrent.Post[postIndex].Aisle[aisleIndex].Batch.Add(_currentEditBatch);
                }
                else 
                {// 既存のバッチ情報を変更保存
                    string[] currentSlotArray = _currentEditBatch.OutputToArray();
                    Resource.batch.BatchInfoCurrent.Post[postIndex].Aisle[aisleIndex].Batch[batchIndex].InputFromArray(currentSlotArray);
                }

                string message = $"棚マスタ情報を保存しました  便No:{postIndex + 1} アイルNo:{aisleIndex + 1} バッチNo:{batchIndex + 1} ユニット{unitIndex + 1}";
                Logger.WriteLog(LogType.INFO, message);
                MessageBoxResult result = MessageBox.Show(message, "確認", MessageBoxButton.OK);
                return rc;

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
        /// 画面に表示しているスロット情報を保持しておく
        /// </summary>
        /// <returns></returns>
        private UInt32 KeepDisplaySlotInfo() 
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                string[] slotArray = _currentEditBatch.OutputToArray();
                for (int i = 0; i < Const.MaxSlotCount; i++)
                    slotArray[(_selectedUnitIndex * Const.MaxSlotCount) + i] = _bindSlots[i].txtStoreCode.Text;
                _currentEditBatch.InputFromArray(slotArray);
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
        /// スロット情報が変更されているか確認
        /// </summary>
        /// <returns></returns>
        private UInt32 CheckValueChanged()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                string[] resourceSlotArray = Resource.batch.BatchInfoCurrent.Post[_selectedPostIndex].Aisle[_selectedAisleIndex].Batch[_selectedBatchIndex].OutputToArray();
                string[] editSlotArray = _currentEditBatch.OutputToArray();

                bool diff = false;
                for (int i = 0; i < Const.MaxSlotCount * Const.MaxUnitCount; i++) 
                {
                    if (resourceSlotArray[i] != editSlotArray[i]) 
                    {
                        diff = true;
                        break;
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

    }



    /// <summary>
    /// エントリーワーク情報
    /// 画面の表にデータバインドするようのクラス
    /// </summary>
    public class BindSlotInfo
    {
        /// <summary>
        /// スロットNo
        /// </summary>
        public Label lblSlotNo = new Label();
        /// <summary>
        /// 店コード
        /// </summary>
        public TextBox txtStoreCode = new TextBox();
        /// <summary>
        /// 店名
        /// </summary>
        public Label lblStoreName = new Label();

    }


}
