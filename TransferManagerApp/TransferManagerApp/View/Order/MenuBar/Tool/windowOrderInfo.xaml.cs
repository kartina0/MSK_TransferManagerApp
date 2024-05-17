//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;

using SystemConfig;
using DL_Logger;
using ErrorCodeDefine;
using ServerModule;
using ShareResource;
using TransferManagerApp_Debug;


namespace TransferManagerApp
{
    /// <summary>
    /// windowOrderInfo.xaml の相互作用ロジック
    /// </summary>
    public partial class windowOrderInfo : Window
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "windowOrderInfo";

        /// <summary>
        /// ウィンドウ表示中フラグ
        /// </summary>
        public bool isShowing = false;

        /// <summary>
        /// 商品ヘッダ
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindWorkHeader> _workHeaderList = new ObservableCollection<BindWorkHeader>();
        /// <summary>
        /// 店別小仕分け
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindStoreOrder> _storeOrderList = new ObservableCollection<BindStoreOrder>();
        /// <summary>
        /// 商品ヘッダ実績
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindWorkHeaderExecute> _workHeaderExecuteList = new ObservableCollection<BindWorkHeaderExecute>();
        /// <summary>
        /// 店別小仕分け実績
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindStoreOrderExecute> _storeOrderExecuteList = new ObservableCollection<BindStoreOrderExecute>();
        /// <summary>
        /// 棚マスター変換
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindBatchConvert> _batchConvertList = new ObservableCollection<BindBatchConvert>();

        /// <summary>
        /// マテハン向け仕分リスト行数
        /// </summary>
        private int _count = 0;



        /// <summary>
        /// コンストラクタ
        /// </summary>
        public windowOrderInfo()
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
                if (DesignerProperties.GetIsInDesignMode(this))
                    return;

                // タイトルバーを消しても画面移動可能にする処理
                this.MouseLeftButtonDown += delegate { DragMove(); };

                // ウィンドウ表示中
                isShowing = true;

                // ボタン背景色設定
                btnWorkHeader.Background = Brushes.CornflowerBlue;
                btnStoreOrder.Background = Brushes.SlateGray;
                btnWorkHeaderExecute.Background = Brushes.SlateGray;
                btnStoreOrderExecute.Background = Brushes.SlateGray;
                btnStoreOrder_batchConv.Background = Brushes.SlateGray;
                // 先頭行を選択
                dataGridWorkHeader.SelectedIndex = 0;
                dataGridStoreOrder.SelectedIndex = 0;
                dataGridWorkHeaderExecute.SelectedIndex = 0;
                dataGridStoreOrderExecute.SelectedIndex = 0;
                dataGridBatchConvert.SelectedIndex = 0;

                // バインドデータ入力
                rc = Update();

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
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // ウィンドウ表示終了
                isShowing = false;

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
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
                if (ctrl == btnUpdate)
                {// 更新
                    rc = Update();
                }
                else if (ctrl == btnClose)
                {// 閉じる
                    this.Close();
                }
                else if (ctrl == btnWorkHeader)
                {// 商品ヘッダ 表示
                    // ボタン背景色設定
                    btnWorkHeader.Background = (SolidColorBrush)Application.Current.Resources["ButtonColorOrder"];
                    btnStoreOrder.Background = Brushes.SlateGray;
                    btnWorkHeaderExecute.Background = Brushes.SlateGray;
                    btnStoreOrderExecute.Background = Brushes.SlateGray;
                    btnStoreOrder_batchConv.Background = Brushes.SlateGray;
                    // DateGrid 表示切替
                    dataGridWorkHeader.Visibility = Visibility.Visible;
                    dataGridStoreOrder.Visibility = Visibility.Hidden;
                    dataGridWorkHeaderExecute.Visibility = Visibility.Hidden;
                    dataGridStoreOrderExecute.Visibility = Visibility.Hidden;
                    dataGridBatchConvert.Visibility = Visibility.Hidden;
                    // 行数更新
                    lblTotalCount.Content = _workHeaderList.Count.ToString() + "行";
                }
                else if (ctrl == btnStoreOrder)
                {// 店別小仕分け 表示
                    // ボタン背景色設定
                    btnWorkHeader.Background = Brushes.SlateGray;
                    btnStoreOrder.Background = (SolidColorBrush)Application.Current.Resources["ButtonColorOrder"];
                    btnWorkHeaderExecute.Background = Brushes.SlateGray;
                    btnStoreOrderExecute.Background = Brushes.SlateGray;
                    btnStoreOrder_batchConv.Background = Brushes.SlateGray;
                    // DateGrid 表示切替
                    dataGridWorkHeader.Visibility = Visibility.Hidden;
                    dataGridStoreOrder.Visibility = Visibility.Visible;
                    dataGridWorkHeaderExecute.Visibility = Visibility.Hidden;
                    dataGridStoreOrderExecute.Visibility = Visibility.Hidden;
                    dataGridBatchConvert.Visibility = Visibility.Hidden;
                    // 行数更新
                    lblTotalCount.Content = _storeOrderList.Count.ToString() + "行";
                }
                else if (ctrl == btnWorkHeaderExecute)
                {// 商品ヘッダ実績 表示
                    // ボタン背景色設定
                    btnWorkHeader.Background = Brushes.SlateGray;
                    btnStoreOrder.Background = Brushes.SlateGray;
                    btnWorkHeaderExecute.Background = (SolidColorBrush)Application.Current.Resources["ButtonColorOrder"];
                    btnStoreOrderExecute.Background = Brushes.SlateGray;
                    btnStoreOrder_batchConv.Background = Brushes.SlateGray;
                    // DateGrid 表示切替
                    dataGridWorkHeader.Visibility = Visibility.Hidden;
                    dataGridStoreOrder.Visibility = Visibility.Hidden;
                    dataGridWorkHeaderExecute.Visibility = Visibility.Visible;
                    dataGridStoreOrderExecute.Visibility = Visibility.Hidden;
                    dataGridBatchConvert.Visibility = Visibility.Hidden;
                    // 行数更新
                    lblTotalCount.Content = _workHeaderExecuteList.Count.ToString() + "行";
                }
                else if (ctrl == btnStoreOrderExecute)
                {// 店別小仕分け実績 表示
                    // ボタン背景色設定
                    btnWorkHeader.Background = Brushes.SlateGray;
                    btnStoreOrder.Background = Brushes.SlateGray;
                    btnWorkHeaderExecute.Background = Brushes.SlateGray;
                    btnStoreOrderExecute.Background = (SolidColorBrush)Application.Current.Resources["ButtonColorOrder"];
                    btnStoreOrder_batchConv.Background = Brushes.SlateGray;
                    // DateGrid 表示切替
                    dataGridWorkHeader.Visibility = Visibility.Hidden;
                    dataGridStoreOrder.Visibility = Visibility.Hidden;
                    dataGridWorkHeaderExecute.Visibility = Visibility.Hidden;
                    dataGridStoreOrderExecute.Visibility = Visibility.Visible;
                    dataGridBatchConvert.Visibility = Visibility.Hidden;
                    // 行数更新
                    lblTotalCount.Content = _storeOrderExecuteList.Count.ToString() + "行";
                }
                else if (ctrl == btnStoreOrder_batchConv)
                {// 棚マスタ変換 表示
                    // ボタン背景色設定
                    btnWorkHeader.Background = Brushes.SlateGray;
                    btnStoreOrder.Background = Brushes.SlateGray;
                    btnWorkHeaderExecute.Background = Brushes.SlateGray;
                    btnStoreOrderExecute.Background = Brushes.SlateGray;
                    btnStoreOrder_batchConv.Background = (SolidColorBrush)Application.Current.Resources["ButtonColorOrder"];
                    // DateGrid 表示切替
                    dataGridWorkHeader.Visibility = Visibility.Hidden;
                    dataGridStoreOrder.Visibility = Visibility.Hidden;
                    dataGridWorkHeaderExecute.Visibility = Visibility.Hidden;
                    dataGridStoreOrderExecute.Visibility = Visibility.Hidden;
                    dataGridBatchConvert.Visibility = Visibility.Visible;
                    // 行数更新
                    lblTotalCount.Content = _count.ToString() + "行";
                }

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        private UInt32 Update()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // ---------------------------------------------------------
                // 商品ヘッダ
                int count = 0;
                _workHeaderList.Clear();
                for (int postIndex = 0; postIndex < Resource.Server.OrderInfo.OrderDataList.Length; postIndex++)
                {
                    foreach (OrderData orderData in Resource.Server.OrderInfo.OrderDataList[postIndex])
                    {
                        count++;
                        BindWorkHeader b = new BindWorkHeader()
                        {
                            No = count,
                            OrderDate = $"{orderData.orderDate.Year.ToString("D4")}{orderData.orderDate.Month.ToString("D2")}{orderData.orderDate.Day.ToString("D2")}",
                            PostNo = orderData.postNo.ToString(),
                            OrderDateRequest = $"{orderData.orderDate.Year.ToString("D4")}{orderData.orderDate.Month.ToString("D2")}{orderData.orderDate.Day.ToString("D2")}",
                            PostNoRequest = orderData.postNoRequest.ToString(),
                            WorkCode = orderData.workCode,
                            Index = orderData.index.ToString(),
                            WorkName = orderData.workName.Trim(),
                            JANCode = orderData.JANCode,
                            CaseVolume = orderData.caseVolume.ToString(),
                            OrderCountTotal = orderData.orderCountTotal.ToString(),
                            WorkNameKana = orderData.workNameKana.Trim(),
                            MaxStackNum = orderData.maxStackNum.ToString(),
                            SalesPrice = orderData.salesPrice.ToString(),
                            Process = ((int)(orderData.process)).ToString(),
                            OrderCount01 = orderData.orderCount[0].ToString(),
                            OrderCount02 = orderData.orderCount[1].ToString(),
                            OrderCount03 = orderData.orderCount[2].ToString(),
                            OrderCount04 = orderData.orderCount[3].ToString(),
                            OrderCount05 = orderData.orderCount[4].ToString(),
                            OrderCount06 = orderData.orderCount[5].ToString(),
                            OrderCount07 = orderData.orderCount[6].ToString(),
                            OrderCount08 = orderData.orderCount[7].ToString(),
                            OrderCount09 = orderData.orderCount[8].ToString(),
                            StoreCount01 = orderData.storeCount[0].ToString(),
                            StoreCount02 = orderData.storeCount[1].ToString(),
                            StoreCount03 = orderData.storeCount[2].ToString(),
                            StoreCount04 = orderData.storeCount[3].ToString(),
                            StoreCount05 = orderData.storeCount[4].ToString(),
                            StoreCount06 = orderData.storeCount[5].ToString(),
                            StoreCount07 = orderData.storeCount[6].ToString(),
                            StoreCount08 = orderData.storeCount[7].ToString(),
                            StoreCount09 = orderData.storeCount[8].ToString(),
                            CreateDateTime = $"{orderData.createDateTime.Year.ToString("D4")}{orderData.createDateTime.Month.ToString("D2")}{orderData.createDateTime.Day.ToString("D2")} 000000",
                            CreateLoginId = orderData.createLoginId,
                            UpdateDateTimea = $"{orderData.updateDateTime.Year.ToString("D4")}{orderData.updateDateTime.Month.ToString("D2")}{orderData.updateDateTime.Day.ToString("D2")} 000000",
                            UpdateLoginId = orderData.updateLoginId
                        };
                        _workHeaderList.Add(b);
                    };
                }
                dataGridWorkHeader.ItemsSource = _workHeaderList;
                // 行数更新
                if (dataGridWorkHeader.Visibility == Visibility.Visible)
                    lblTotalCount.Content = _workHeaderList.Count.ToString() + "行";


                // ---------------------------------------------------------
                // 店別小仕分け
                count = 0;
                _storeOrderList.Clear();
                for (int postIndex = 0; postIndex < Resource.Server.OrderInfo.OrderDataList.Length; postIndex++)
                {
                    foreach (OrderData orderData in Resource.Server.OrderInfo.OrderDataList[postIndex])
                    {
                        foreach (OrderStoreData storeData in orderData.storeDataList)
                        {
                            count++;
                            BindStoreOrder b = new BindStoreOrder()
                            {
                                No = count.ToString(),
                                OrderDate = $"{storeData.orderDate.Year.ToString("D4")}{storeData.orderDate.Month.ToString("D2")}{storeData.orderDate.Day.ToString("D2")}",
                                PostNo = storeData.postNo.ToString(),
                                OrderDateRequest = $"{storeData.orderDate.Year.ToString("D4")}{storeData.orderDate.Month.ToString("D2")}{storeData.orderDate.Day.ToString("D2")}",
                                PostNoRequest = storeData.postNoRequest.ToString(),
                                WorkCode = storeData.workCode,
                                Index = storeData.index.ToString(),
                                StoreCode = storeData.storeCode,
                                StationNo = storeData.stationNo.ToString(),
                                AisleNo = storeData.aisleNo.ToString(),
                                SlotNo = storeData.slotNo.ToString(),
                                CaseVolume = storeData.caseVolume.ToString(),
                                OrderCount = storeData.orderCount.ToString(),
                                CreateDateTime = $"{storeData.createDateTime.Year.ToString("D4")}{storeData.createDateTime.Month.ToString("D2")}{storeData.createDateTime.Day.ToString("D2")} 000000",
                                CreateLoginId = storeData.createLoginId,
                                UpdateDateTimea = $"{storeData.updateDateTime.Year.ToString("D4")}{storeData.updateDateTime.Month.ToString("D2")}{storeData.updateDateTime.Day.ToString("D2")} 000000",
                                UpdateLoginId = storeData.updateLoginId
                            };
                            _storeOrderList.Add(b);
                        }
                    }
                }
                dataGridStoreOrder.ItemsSource = _storeOrderList;
                // 行数更新
                if (dataGridStoreOrder.Visibility == Visibility.Visible)
                    lblTotalCount.Content = _storeOrderList.Count.ToString() + "行";


                // ---------------------------------------------------------
                // 商品ヘッダ実績
                count = 0;
                _workHeaderExecuteList.Clear();
                for (int postIndex = 0; postIndex < Resource.Server.OrderInfo.ExecuteDataList.Length; postIndex++)
                {
                    foreach (ExecuteData executeData in Resource.Server.OrderInfo.ExecuteDataList[postIndex])
                    {
                        count++;
                        BindWorkHeaderExecute b = new BindWorkHeaderExecute()
                        {
                            No = count.ToString(),
                            OrderDate = $"{executeData.orderDate.Year.ToString("D4")}{executeData.orderDate.Month.ToString("D2")}{executeData.orderDate.Day.ToString("D2")}",
                            PostNo = executeData.postNo.ToString(),
                            OrderDateRequest = $"{executeData.orderDate.Year.ToString("D4")}{executeData.orderDate.Month.ToString("D2")}{executeData.orderDate.Day.ToString("D2")}",
                            PostNoRequest = executeData.postNoRequest.ToString(),
                            WorkCode = executeData.workCode,
                            Index = executeData.index.ToString(),
                            JANCode = executeData.JANCode,
                            OrderCountTotal = executeData.orderCountTotal.ToString(),
                            OrderCompCountTotal = executeData.orderCompCountTotal.ToString(),
                            LoadDateTime = executeData.loadDateTime,
                            //LoadDateTime = executeData.loadDateTime.ToString(),
                            CreateDateTime = $"{executeData.createDateTime.Year.ToString("D4")}{executeData.createDateTime.Month.ToString("D2")}{executeData.createDateTime.Day.ToString("D2")} 000000",
                            CreateLoginId = executeData.createLoginId,
                            UpdateDateTimea = $"{executeData.updateDateTime.Year.ToString("D4")}{executeData.updateDateTime.Month.ToString("D2")}{executeData.updateDateTime.Day.ToString("D2")} 000000",
                            UpdateLoginId = executeData.updateLoginId
                        };
                        _workHeaderExecuteList.Add(b);
                    };
                }
                dataGridWorkHeaderExecute.ItemsSource = _workHeaderExecuteList;
                // 行数更新
                if (dataGridWorkHeaderExecute.Visibility == Visibility.Visible)
                    lblTotalCount.Content = count.ToString() + "行";


                // ---------------------------------------------------------
                // 店別小仕分け実績
                count = 0;
                _storeOrderExecuteList.Clear();
                for (int postIndex = 0; postIndex < Resource.Server.OrderInfo.ExecuteDataList.Length; postIndex++)
                {
                    foreach (ExecuteData executeData in Resource.Server.OrderInfo.ExecuteDataList[postIndex])
                    {
                        foreach (ExecuteStoreData storeData in executeData.storeDataList)
                        {
                            count++;
                            BindStoreOrderExecute b = new BindStoreOrderExecute()
                            {
                                No = count.ToString(),
                                OrderDate = $"{storeData.orderDate.Year.ToString("D4")}{storeData.orderDate.Month.ToString("D2")}{storeData.orderDate.Day.ToString("D2")}",
                                PostNo = storeData.postNo.ToString(),
                                OrderDateRequest = $"{storeData.orderDate.Year.ToString("D4")}{storeData.orderDate.Month.ToString("D2")}{storeData.orderDate.Day.ToString("D2")}",
                                PostNoRequest = storeData.postNoRequest.ToString(),
                                WorkCode = storeData.workCode,
                                Index = storeData.index.ToString(),
                                StoreCode = storeData.storeCode,
                                StationNo = storeData.stationNo.ToString(),
                                AisleNo = storeData.aisleNo.ToString(),
                                SlotNo = storeData.slotNo.ToString(),
                                OrderCount = storeData.orderCount.ToString(),
                                OrderCompCount = storeData.orderCompCount.ToString(),
                                CreateDateTime = $"{storeData.createDateTime.Year.ToString("D4")}{storeData.createDateTime.Month.ToString("D2")}{storeData.createDateTime.Day.ToString("D2")} 000000",
                                CreateLoginId = storeData.createLoginId,
                                UpdateDateTimea = $"{storeData.createDateTime.Year.ToString("D4")}{storeData.createDateTime.Month.ToString("D2")}{storeData.createDateTime.Day.ToString("D2")} 000000",
                                UpdateLoginId = storeData.updateLoginId
                            };
                            _storeOrderExecuteList.Add(b);
                        }
                    }
                }
                dataGridStoreOrderExecute.ItemsSource = _storeOrderExecuteList;
                // 行数更新
                if (dataGridStoreOrderExecute.Visibility == Visibility.Visible)
                    lblTotalCount.Content = _storeOrderExecuteList.Count.ToString() + "行";


                // ---------------------------------------------------------
                // 棚マスター変換
                count = 0;
                _batchConvertList.Clear();

                for (int postIndex = 0; postIndex < Const.MaxPostCount; postIndex++) 
                {
                    
                    foreach (ExecuteData work in Resource.Server.OrderInfo.ExecuteDataList[postIndex])
                    {
                        string janCode = work.JANCode;
                        string workNameKana = Resource.Server.OrderInfo.OrderDataList[postIndex].Find(x => x.JANCode == janCode).workNameKana;

                        for (int aisleIndex = 0; aisleIndex < Const.MaxAisleCount; aisleIndex++)
                        {
                            for (int batchIndex = 0; batchIndex < Const.MaxBatchCount; batchIndex++)
                            {
                                List<ExecuteStoreData> storeList = work.storeDataList.Where(x => x.aisleNo_MH == aisleIndex + 1 && x.batchNo_MH == batchIndex + 1)
                                                                                 .OrderBy(x => x.slotNo_MH)
                                                                                 .ToList();

                                foreach (ExecuteStoreData store in storeList) 
                                {
                                    count++;

                                    BindBatchConvert b = new BindBatchConvert()
                                    {
                                        No = count.ToString(),
                                        OrderDate = $"{store.orderDate.Year.ToString("D4")}{store.orderDate.Month.ToString("D2")}{store.orderDate.Day.ToString("D2")}",
                                        PostNo = store.postNo.ToString(),
                                        OrderDateRequest = $"{store.orderDate.Year.ToString("D4")}{store.orderDate.Month.ToString("D2")}{store.orderDate.Day.ToString("D2")}",
                                        PostNoRequest = store.postNoRequest.ToString(),
                                        WorkCode = store.workCode,
                                        StoreCode = store.storeCode,
                                        StationNo_Original = store.stationNo.ToString(),
                                        AisleNo_Original = store.aisleNo.ToString(),
                                        SlotNo_Original = store.slotNo.ToString(),
                                        OrderCount = store.orderCount.ToString(),
                                        OrderCompCount = store.orderCompCount.ToString(),
                                        StationNo_Convert = store.stationNo.ToString(),
                                        AisleNo_Convert = store.aisleNo_MH.ToString(),
                                        SlotNo_Convert = store.slotNo_MH.ToString(),

                                        JANCode = janCode,
                                        WorkNameKana = workNameKana,
                                    };
                                    _batchConvertList.Add(b);
                                }
                            }
                        }
                    }

                }
                dataGridBatchConvert.ItemsSource = _batchConvertList;
                // 行数更新
                _count = count;
                if (dataGridBatchConvert.Visibility == Visibility.Visible)
                    lblTotalCount.Content = _batchConvertList.Count.ToString() + "行";

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
    /// 商品ヘッダ バインド
    /// </summary>
    public class BindWorkHeader : INotifyPropertyChanged
    {
        private int _no;
        /// <summary>
        /// No
        /// </summary>
        public int No
        {
            get { return _no; }
            set
            {
                if (_no != value)
                {
                    _no = value;
                    OnPropertyChanged(nameof(No));
                }
            }
        }

        private string _orderDate;
        /// <summary>
        /// 仕分納品日
        /// </summary>
        public string OrderDate
        {
            get { return _orderDate; }
            set
            {
                if (_orderDate != value)
                {
                    _orderDate = value;
                    OnPropertyChanged(nameof(OrderDate));
                }
            }
        }

        private string _postNo;
        /// <summary>
        /// 仕分便No
        /// </summary>
        public string PostNo
        {
            get { return _postNo; }
            set
            {
                if (_postNo != value)
                {
                    _postNo = value;
                    OnPropertyChanged(nameof(PostNo));
                }
            }
        }

        private string _orderDateRequest;
        /// <summary>
        /// 発注納品日
        /// </summary>
        public string OrderDateRequest
        {
            get { return _orderDateRequest; }
            set
            {
                if (_orderDateRequest != value)
                {
                    _orderDateRequest = value;
                    OnPropertyChanged(nameof(OrderDateRequest));
                }
            }
        }

        private string _postNoRequest;
        /// <summary>
        /// 発注便No
        /// </summary>
        public string PostNoRequest
        {
            get { return _postNoRequest; }
            set
            {
                if (_postNoRequest != value)
                {
                    _postNoRequest = value;
                    OnPropertyChanged(nameof(PostNoRequest));
                }
            }
        }

        private string _workCode;
        /// <summary>
        /// 商品コード
        /// </summary>
        public string WorkCode
        {
            get { return _workCode; }
            set
            {
                if (_workCode != value)
                {
                    _workCode = value;
                    OnPropertyChanged(nameof(WorkCode));
                }
            }
        }

        private string _index;
        /// <summary>
        /// 連番
        /// </summary>
        public string Index
        {
            get { return _index; }
            set
            {
                if (_index != value)
                {
                    _index = value;
                    OnPropertyChanged(nameof(Index));
                }
            }
        }

        private string _workName;
        /// <summary>
        /// 商品名(漢字)
        /// </summary>
        public string WorkName
        {
            get { return _workName; }
            set
            {
                if (_workName != value)
                {
                    _workName = value;
                    OnPropertyChanged(nameof(WorkName));
                }
            }
        }

        private string _janCode;
        /// <summary>
        /// JANコード
        /// </summary>
        public string JANCode
        {
            get { return _janCode; }
            set
            {
                if (_janCode != value)
                {
                    _janCode = value;
                    OnPropertyChanged(nameof(JANCode));
                }
            }
        }

        private string _caseVolume;
        /// <summary>
        /// ケース入数
        /// </summary>
        public string CaseVolume
        {
            get { return _caseVolume; }
            set
            {
                if (_caseVolume != value)
                {
                    _caseVolume = value;
                    OnPropertyChanged(nameof(CaseVolume));
                }
            }
        }

        private string _orderCountTotal;
        /// <summary>
        /// 仕分け数合計
        /// </summary>
        public string OrderCountTotal
        {
            get { return _orderCountTotal; }
            set
            {
                if (_orderCountTotal != value)
                {
                    _orderCountTotal = value;
                    OnPropertyChanged(nameof(OrderCountTotal));
                }
            }
        }

        private string _workNameKana;
        /// <summary>
        /// 商品名(カナ)
        /// </summary>
        public string WorkNameKana
        {
            get { return _workNameKana; }
            set
            {
                if (_workNameKana != value)
                {
                    _workNameKana = value;
                    OnPropertyChanged(nameof(WorkNameKana));
                }
            }
        }

        private string _maxStackNum;
        /// <summary>
        /// MAX積み付け段数
        /// </summary>
        public string MaxStackNum
        {
            get { return _maxStackNum; }
            set
            {
                if (_maxStackNum != value)
                {
                    _maxStackNum = value;
                    OnPropertyChanged(nameof(MaxStackNum));
                }
            }
        }

        private string _salesPrice;
        /// <summary>
        /// 売価１
        /// </summary>
        public string SalesPrice
        {
            get { return _salesPrice; }
            set
            {
                if (_salesPrice != value)
                {
                _salesPrice = value;
                    OnPropertyChanged(nameof(SalesPrice));
                }
            }
        }

        private string _process;
        /// <summary>
        /// 仕分け作業状況
        /// </summary>
        public string Process
        {
            get { return _process; }
            set
            {
                if (_process != value)
                {
                    _process = value;
                    OnPropertyChanged(nameof(Process));
                }
            }
        }

        private string _orderCount01;
        /// <summary>
        /// 仕分け数 ステーション01
        /// </summary>
        public string OrderCount01
        {
            get { return _orderCount01; }
            set
            {
                if (_orderCount01 != value)
                {
                    _orderCount01 = value;
                    OnPropertyChanged(nameof(OrderCount01));
                }
            }
        }

        private string _orderCount02;
        /// <summary>
        /// 仕分け数 ステーション02
        /// </summary>
        public string OrderCount02
        {
            get { return _orderCount02; }
            set
            {
                if (_orderCount02 != value)
                {
                    _orderCount02 = value;
                    OnPropertyChanged(nameof(OrderCount02));
                }
            }
        }

        private string _orderCount03;
        /// <summary>
        /// 仕分け数 ステーション03
        /// </summary>
        public string OrderCount03
        {
            get { return _orderCount03; }
            set
            {
                if (_orderCount03 != value)
                {
                    _orderCount03 = value;
                    OnPropertyChanged(nameof(OrderCount03));
                }
            }
        }

        private string _orderCount04;
        /// <summary>
        /// 仕分け数 ステーション04
        /// </summary>
        public string OrderCount04
        {
            get { return _orderCount04; }
            set
            {
                if (_orderCount04 != value)
                {
                    _orderCount04 = value;
                    OnPropertyChanged(nameof(OrderCount04));
                }
            }
        }

        private string _orderCount05;
        /// <summary>
        /// 仕分け数 ステーション05
        /// </summary>
        public string OrderCount05
        {
            get { return _orderCount05; }
            set
            {
                if (_orderCount05 != value)
                {
                    _orderCount05 = value;
                    OnPropertyChanged(nameof(OrderCount05));
                }
            }
        }

        private string _orderCount06;
        /// <summary>
        /// 仕分け数 ステーション06
        /// </summary>
        public string OrderCount06
        {
            get { return _orderCount06; }
            set
            {
                if (_orderCount06 != value)
                {
                    _orderCount06 = value;
                    OnPropertyChanged(nameof(OrderCount06));
                }
            }
        }

        private string _orderCount07;
        /// <summary>
        /// 仕分け数 ステーション07
        /// </summary>
        public string OrderCount07
        {
            get { return _orderCount07; }
            set
            {
                if (_orderCount07 != value)
                {
                    _orderCount07 = value;
                    OnPropertyChanged(nameof(OrderCount07));
                }
            }
        }

        private string _orderCount08;
        /// <summary>
        /// 仕分け数 ステーション08
        /// </summary>
        public string OrderCount08
        {
            get { return _orderCount08; }
            set
            {
                if (_orderCount08 != value)
                {
                    _orderCount08 = value;
                    OnPropertyChanged(nameof(OrderCount08));
                }
            }
        }

        private string _orderCount09;
        /// <summary>
        /// 仕分け数 ステーション09
        /// </summary>
        public string OrderCount09
        {
            get { return _orderCount09; }
            set
            {
                if (_orderCount09 != value)
                {
                    _orderCount09 = value;
                    OnPropertyChanged(nameof(OrderCount09));
                }
            }
        }

        private string _storeCount01;
        /// <summary>
        /// 店舗数 ステーション01
        /// </summary>
        public string StoreCount01
        {
            get { return _storeCount01; }
            set
            {
                if (_storeCount01 != value)
                {
                    _storeCount01 = value;
                    OnPropertyChanged(nameof(StoreCount01));
                }
            }
        }

        private string _storeCount02;
        /// <summary>
        /// 店舗数 ステーション02
        /// </summary>
        public string StoreCount02
        {
            get { return _storeCount02; }
            set
            {
                if (_storeCount02 != value)
                {
                    _storeCount02 = value;
                    OnPropertyChanged(nameof(StoreCount02));
                }
            }
        }

        private string _storeCount03;
        /// <summary>
        /// 店舗数 ステーション03
        /// </summary>
        public string StoreCount03
        {
            get { return _storeCount03; }
            set
            {
                if (_storeCount03 != value)
                {
                    _storeCount03 = value;
                    OnPropertyChanged(nameof(StoreCount03));
                }
            }
        }

        private string _storeCount04;
        /// <summary>
        /// 店舗数 ステーション04
        /// </summary>
        public string StoreCount04
        {
            get { return _storeCount04; }
            set
            {
                if (_storeCount04 != value)
                {
                    _storeCount04 = value;
                    OnPropertyChanged(nameof(StoreCount04));
                }
            }
        }

        private string _storeCount05;
        /// <summary>
        /// 店舗数 ステーション05
        /// </summary>
        public string StoreCount05
        {
            get { return _storeCount05; }
            set
            {
                if (_storeCount05 != value)
                {
                    _storeCount05 = value;
                    OnPropertyChanged(nameof(StoreCount05));
                }
            }
        }

        private string _storeCount06;
        /// <summary>
        /// 店舗数 ステーション06
        /// </summary>
        public string StoreCount06
        {
            get { return _storeCount06; }
            set
            {
                if (_storeCount06 != value)
                {
                    _storeCount06 = value;
                    OnPropertyChanged(nameof(StoreCount06));
                }
            }
        }

        private string _storeCount07;
        /// <summary>
        /// 店舗数 ステーション07
        /// </summary>
        public string StoreCount07
        {
            get { return _storeCount07; }
            set
            {
                if (_storeCount07 != value)
                {
                    _storeCount07 = value;
                    OnPropertyChanged(nameof(StoreCount07));
                }
            }
        }

        private string _storeCount08;
        /// <summary>
        /// 店舗数 ステーション08
        /// </summary>
        public string StoreCount08
        {
            get { return _storeCount08; }
            set
            {
                if (_storeCount08 != value)
                {
                    _storeCount08 = value;
                    OnPropertyChanged(nameof(StoreCount08));
                }
            }
        }

        private string _storeCount09;
        /// <summary>
        /// 店舗数 ステーション09
        /// </summary>
        public string StoreCount09
        {
            get { return _storeCount09; }
            set
            {
                if (_storeCount09 != value)
                {
                    _storeCount09 = value;
                    OnPropertyChanged(nameof(StoreCount09));
                }
            }
        }

        private string _createDateTime;
        /// <summary>
        /// 登録日時
        /// </summary>
        public string CreateDateTime
        {
            get { return _createDateTime; }
            set
            {
                if (_createDateTime != value)
                {
                    _createDateTime = value;
                    OnPropertyChanged(nameof(CreateDateTime));
                }
            }
        }

        private string _createLoginId;
        /// <summary>
        /// 登録ログインID
        /// </summary>
        public string CreateLoginId
        {
            get { return _createLoginId; }
            set
            {
                if (_createLoginId != value)
                {
                    _createLoginId = value;
                    OnPropertyChanged(nameof(CreateLoginId));
                }
            }
        }

        private string _updateDateTimea;
        /// <summary>
        /// 更新日時
        /// </summary>
        public string UpdateDateTimea
        {
            get { return _updateDateTimea; }
            set
            {
                if (_updateDateTimea != value)
                {
                    _updateDateTimea = value;
                    OnPropertyChanged(nameof(UpdateDateTimea));
                }
            }
        }

        private string _updateLoginId;
        /// <summary>
        /// 更新ログインID
        /// </summary>
        public string UpdateLoginId
        {
            get { return _updateLoginId; }
            set
            {
                if (_updateLoginId != value)
                {
                    _updateLoginId = value;
                    OnPropertyChanged(nameof(UpdateLoginId));
                }
            }
        }


        // ListViewへバインディングしたデータの値が変更されたときに、
        // ListViewに変更を通知して反映させる仕組み。
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// 店別小仕分け バインド
    /// </summary>
    public class BindStoreOrder : INotifyPropertyChanged
    {
        private string _no;
        /// <summary>
        /// No
        /// </summary>
        public string No
        {
            get { return _no; }
            set
            {
                if (_no != value)
                {
                    _no = value;
                    OnPropertyChanged(nameof(No));
                }
            }
        }

        private string _orderDate;
        /// <summary>
        /// 仕分納品日
        /// </summary>
        public string OrderDate
        {
            get { return _orderDate; }
            set
            {
                if (_orderDate != value)
                {
                    _orderDate = value;
                    OnPropertyChanged(nameof(OrderDate));
                }
            }
        }

        private string _postNo;
        /// <summary>
        /// 仕分便No
        /// </summary>
        public string PostNo
        {
            get { return _postNo; }
            set
            {
                if (_postNo != value)
                {
                    _postNo = value;
                    OnPropertyChanged(nameof(PostNo));
                }
            }
        }

        private string _orderDateRequest;
        /// <summary>
        /// 発注納品日
        /// </summary>
        public string OrderDateRequest
        {
            get { return _orderDateRequest; }
            set
            {
                if (_orderDateRequest != value)
                {
                    _orderDateRequest = value;
                    OnPropertyChanged(nameof(OrderDateRequest));
                }
            }
        }

        private string _postNoRequest;
        /// <summary>
        /// 発注便No
        /// </summary>
        public string PostNoRequest
        {
            get { return _postNoRequest; }
            set
            {
                if (_postNoRequest != value)
                {
                    _postNoRequest = value;
                    OnPropertyChanged(nameof(PostNoRequest));
                }
            }
        }

        private string _workCode;
        /// <summary>
        /// 商品コード
        /// </summary>
        public string WorkCode
        {
            get { return _workCode; }
            set
            {
                if (_workCode != value)
                {
                    _workCode = value;
                    OnPropertyChanged(nameof(WorkCode));
                }
            }
        }

        private string _index;
        /// <summary>
        /// 連番
        /// </summary>
        public string Index
        {
            get { return _index; }
            set
            {
                if (_index != value)
                {
                    _index = value;
                    OnPropertyChanged(nameof(Index));
                }
            }
        }

        private string _storeCode;
        /// <summary>
        /// 店コード
        /// </summary>
        public string StoreCode
        {
            get { return _storeCode; }
            set
            {
                if (_storeCode != value)
                {
                    _storeCode = value;
                    OnPropertyChanged(nameof(StoreCode));
                }
            }
        }

        private string _stationNo;
        /// <summary>
        /// ステーションNo
        /// </summary>
        public string StationNo
        {
            get { return _stationNo; }
            set
            {
                if (_stationNo != value)
                {
                    _stationNo = value;
                    OnPropertyChanged(nameof(StationNo));
                }
            }
        }

        private string _aisleNo;
        /// <summary>
        /// アイルNo
        /// </summary>
        public string AisleNo
        {
            get { return _aisleNo; }
            set
            {
                if (_aisleNo != value)
                {
                    _aisleNo = value;
                    OnPropertyChanged(nameof(AisleNo));
                }
            }
        }

        private string _slotNo;
        /// <summary>
        /// スロットNo
        /// </summary>
        public string SlotNo
        {
            get { return _slotNo; }
            set
            {
                if (_slotNo != value)
                {
                    _slotNo = value;
                    OnPropertyChanged(nameof(SlotNo));
                }
            }
        }

        private string _caseVolume;
        /// <summary>
        /// ケース入数
        /// </summary>
        public string CaseVolume
        {
            get { return _caseVolume; }
            set
            {
                if (_caseVolume != value)
                {
                    _caseVolume = value;
                    OnPropertyChanged(nameof(CaseVolume));
                }
            }
        }

        private string _orderCount;
        /// <summary>
        /// 仕分け数
        /// </summary>
        public string OrderCount
        {
            get { return _orderCount; }
            set
            {
                if (_orderCount != value)
                {
                    _orderCount = value;
                    OnPropertyChanged(nameof(OrderCount));
                }
            }
        }

        private string _createDateTime;
        /// <summary>
        /// 登録日時
        /// </summary>
        public string CreateDateTime
        {
            get { return _createDateTime; }
            set
            {
                if (_createDateTime != value)
                {
                    _createDateTime = value;
                    OnPropertyChanged(nameof(CreateDateTime));
                }
            }
        }

        private string _createLoginId;
        /// <summary>
        /// 登録ログインID
        /// </summary>
        public string CreateLoginId
        {
            get { return _createLoginId; }
            set
            {
                if (_createLoginId != value)
                {
                    _createLoginId = value;
                    OnPropertyChanged(nameof(CreateLoginId));
                }
            }
        }

        private string _updateDateTimea;
        /// <summary>
        /// 更新日時
        /// </summary>
        public string UpdateDateTimea
        {
            get { return _updateDateTimea; }
            set
            {
                if (_updateDateTimea != value)
                {
                    _updateDateTimea = value;
                    OnPropertyChanged(nameof(UpdateDateTimea));
                }
            }
        }

        private string _updateLoginId;
        /// <summary>
        /// 更新ログインID
        /// </summary>
        public string UpdateLoginId
        {
            get { return _updateLoginId; }
            set
            {
                if (_updateLoginId != value)
                {
                    _updateLoginId = value;
                    OnPropertyChanged(nameof(UpdateLoginId));
                }
            }
        }


        // ListViewへバインディングしたデータの値が変更されたときに、
        // ListViewに変更を通知して反映させる仕組み。
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// 商品ヘッダ実績 バインド
    /// </summary>
    public class BindWorkHeaderExecute : INotifyPropertyChanged
    {
        private string _no;
        /// <summary>
        /// No
        /// </summary>
        public string No
        {
            get { return _no; }
            set
            {
                if (_no != value)
                {
                    _no = value;
                    OnPropertyChanged(nameof(No));
                }
            }
        }

        private string _orderDate;
        /// <summary>
        /// 仕分納品日
        /// </summary>
        public string OrderDate
        {
            get { return _orderDate; }
            set
            {
                if (_orderDate != value)
                {
                    _orderDate = value;
                    OnPropertyChanged(nameof(OrderDate));
                }
            }
        }

        private string _postNo;
        /// <summary>
        /// 仕分便No
        /// </summary>
        public string PostNo
        {
            get { return _postNo; }
            set
            {
                if (_postNo != value)
                {
                    _postNo = value;
                    OnPropertyChanged(nameof(PostNo));
                }
            }
        }

        private string _orderDateRequest;
        /// <summary>
        /// 発注納品日
        /// </summary>
        public string OrderDateRequest
        {
            get { return _orderDateRequest; }
            set
            {
                if (_orderDateRequest != value)
                {
                    _orderDateRequest = value;
                    OnPropertyChanged(nameof(OrderDateRequest));
                }
            }
        }

        private string _postNoRequest;
        /// <summary>
        /// 発注便No
        /// </summary>
        public string PostNoRequest
        {
            get { return _postNoRequest; }
            set
            {
                if (_postNoRequest != value)
                {
                    _postNoRequest = value;
                    OnPropertyChanged(nameof(PostNoRequest));
                }
            }
        }

        private string _workCode;
        /// <summary>
        /// 商品コード
        /// </summary>
        public string WorkCode
        {
            get { return _workCode; }
            set
            {
                if (_workCode != value)
                {
                    _workCode = value;
                    OnPropertyChanged(nameof(WorkCode));
                }
            }
        }

        private string _index;
        /// <summary>
        /// 連番
        /// </summary>
        public string Index
        {
            get { return _index; }
            set
            {
                if (_index != value)
                {
                    _index = value;
                    OnPropertyChanged(nameof(Index));
                }
            }
        }

        private string _janCode;
        /// <summary>
        /// JANコード
        /// </summary>
        public string JANCode
        {
            get { return _janCode; }
            set
            {
                if (_janCode != value)
                {
                    _janCode = value;
                    OnPropertyChanged(nameof(JANCode));
                }
            }
        }

        private string _orderCountTotal;
        /// <summary>
        /// 仕分け数合計
        /// </summary>
        public string OrderCountTotal
        {
            get { return _orderCountTotal; }
            set
            {
                if (_orderCountTotal != value)
                {
                    _orderCountTotal = value;
                    OnPropertyChanged(nameof(OrderCountTotal));
                }
            }
        }

        private string _orderCompCountTotal;
        /// <summary>
        /// 仕分け完了数合計
        /// </summary>
        public string OrderCompCountTotal
        {
            get { return _orderCompCountTotal; }
            set
            {
                if (_orderCompCountTotal != value)
                {
                    _orderCompCountTotal = value;
                    OnPropertyChanged(nameof(OrderCompCountTotal));
                }
            }
        }

        private string _loadDateTime;
        /// <summary>
        /// 取込日時
        /// </summary>
        public string LoadDateTime
        {
            get { return _loadDateTime; }
            set
            {
                if (_loadDateTime != value)
                {
                    _loadDateTime = value;
                    OnPropertyChanged(nameof(LoadDateTime));
                }
            }
        }

        private string _createDateTime;
        /// <summary>
        /// 登録日時
        /// </summary>
        public string CreateDateTime
        {
            get { return _createDateTime; }
            set
            {
                if (_createDateTime != value)
                {
                    _createDateTime = value;
                    OnPropertyChanged(nameof(CreateDateTime));
                }
            }
        }

        private string _createLoginId;
        /// <summary>
        /// 登録ログインID
        /// </summary>
        public string CreateLoginId
        {
            get { return _createLoginId; }
            set
            {
                if (_createLoginId != value)
                {
                    _createLoginId = value;
                    OnPropertyChanged(nameof(CreateLoginId));
                }
            }
        }

        private string _updateDateTimea;
        /// <summary>
        /// 更新日時
        /// </summary>
        public string UpdateDateTimea
        {
            get { return _updateDateTimea; }
            set
            {
                if (_updateDateTimea != value)
                {
                    _updateDateTimea = value;
                    OnPropertyChanged(nameof(UpdateDateTimea));
                }
            }
        }

        private string _updateLoginId;
        /// <summary>
        /// 更新ログインID
        /// </summary>
        public string UpdateLoginId
        {
            get { return _updateLoginId; }
            set
            {
                if (_updateLoginId != value)
                {
                    _updateLoginId = value;
                    OnPropertyChanged(nameof(UpdateLoginId));
                }
            }
        }


        // ListViewへバインディングしたデータの値が変更されたときに、
        // ListViewに変更を通知して反映させる仕組み。
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// 店別小仕分け実績 バインド
    /// </summary>
    public class BindStoreOrderExecute : INotifyPropertyChanged
    {
        private string _no;
        /// <summary>
        /// No
        /// </summary>
        public string No
        {
            get { return _no; }
            set
            {
                if (_no != value)
                {
                    _no = value;
                    OnPropertyChanged(nameof(No));
                }
            }
        }

        private string _orderDate;
        /// <summary>
        /// 仕分納品日
        /// </summary>
        public string OrderDate
        {
            get { return _orderDate; }
            set
            {
                if (_orderDate != value)
                {
                    _orderDate = value;
                    OnPropertyChanged(nameof(OrderDate));
                }
            }
        }

        private string _postNo;
        /// <summary>
        /// 仕分便No
        /// </summary>
        public string PostNo
        {
            get { return _postNo; }
            set
            {
                if (_postNo != value)
                {
                    _postNo = value;
                    OnPropertyChanged(nameof(PostNo));
                }
            }
        }

        private string _orderDateRequest;
        /// <summary>
        /// 発注納品日
        /// </summary>
        public string OrderDateRequest
        {
            get { return _orderDateRequest; }
            set
            {
                if (_orderDateRequest != value)
                {
                    _orderDateRequest = value;
                    OnPropertyChanged(nameof(OrderDateRequest));
                }
            }
        }

        private string _postNoRequest;
        /// <summary>
        /// 発注便No
        /// </summary>
        public string PostNoRequest
        {
            get { return _postNoRequest; }
            set
            {
                if (_postNoRequest != value)
                {
                    _postNoRequest = value;
                    OnPropertyChanged(nameof(PostNoRequest));
                }
            }
        }

        private string _workCode;
        /// <summary>
        /// 商品コード
        /// </summary>
        public string WorkCode
        {
            get { return _workCode; }
            set
            {
                if (_workCode != value)
                {
                    _workCode = value;
                    OnPropertyChanged(nameof(WorkCode));
                }
            }
        }

        private string _index;
        /// <summary>
        /// 連番
        /// </summary>
        public string Index
        {
            get { return _index; }
            set
            {
                if (_index != value)
                {
                    _index = value;
                    OnPropertyChanged(nameof(Index));
                }
            }
        }

        private string _storeCode;
        /// <summary>
        /// 店コード
        /// </summary>
        public string StoreCode
        {
            get { return _storeCode; }
            set
            {
                if (_storeCode != value)
                {
                    _storeCode = value;
                    OnPropertyChanged(nameof(StoreCode));
                }
            }
        }

        private string _stationNo;
        /// <summary>
        /// ステーションNo
        /// </summary>
        public string StationNo
        {
            get { return _stationNo; }
            set
            {
                if (_stationNo != value)
                {
                    _stationNo = value;
                    OnPropertyChanged(nameof(StationNo));
                }
            }
        }

        private string _aisleNo;
        /// <summary>
        /// アイルNo
        /// </summary>
        public string AisleNo
        {
            get { return _aisleNo; }
            set
            {
                if (_aisleNo != value)
                {
                    _aisleNo = value;
                    OnPropertyChanged(nameof(AisleNo));
                }
            }
        }

        private string _slotNo;
        /// <summary>
        /// スロットNo
        /// </summary>
        public string SlotNo
        {
            get { return _slotNo; }
            set
            {
                if (_slotNo != value)
                {
                    _slotNo = value;
                    OnPropertyChanged(nameof(SlotNo));
                }
            }
        }

        private string _orderCount;
        /// <summary>
        /// 仕分け数
        /// </summary>
        public string OrderCount
        {
            get { return _orderCount; }
            set
            {
                if (_orderCount != value)
                {
                    _orderCount = value;
                    OnPropertyChanged(nameof(OrderCount));
                }
            }
        }

        private string _orderCompCount;
        /// <summary>
        /// 仕分け完了数
        /// </summary>
        public string OrderCompCount
        {
            get { return _orderCompCount; }
            set
            {
                if (_orderCompCount != value)
                {
                    _orderCompCount = value;
                    OnPropertyChanged(nameof(OrderCompCount));
                }
            }
        }

        private string _createDateTime;
        /// <summary>
        /// 登録日時
        /// </summary>
        public string CreateDateTime
        {
            get { return _createDateTime; }
            set
            {
                if (_createDateTime != value)
                {
                    _createDateTime = value;
                    OnPropertyChanged(nameof(CreateDateTime));
                }
            }
        }

        private string _createLoginId;
        /// <summary>
        /// 登録ログインID
        /// </summary>
        public string CreateLoginId
        {
            get { return _createLoginId; }
            set
            {
                if (_createLoginId != value)
                {
                    _createLoginId = value;
                    OnPropertyChanged(nameof(CreateLoginId));
                }
            }
        }

        private string _updateDateTimea;
        /// <summary>
        /// 更新日時
        /// </summary>
        public string UpdateDateTimea
        {
            get { return _updateDateTimea; }
            set
            {
                if (_updateDateTimea != value)
                {
                    _updateDateTimea = value;
                    OnPropertyChanged(nameof(UpdateDateTimea));
                }
            }
        }

        private string _updateLoginId;
        /// <summary>
        /// 更新ログインID
        /// </summary>
        public string UpdateLoginId
        {
            get { return _updateLoginId; }
            set
            {
                if (_updateLoginId != value)
                {
                    _updateLoginId = value;
                    OnPropertyChanged(nameof(UpdateLoginId));
                }
            }
        }


        // ListViewへバインディングしたデータの値が変更されたときに、
        // ListViewに変更を通知して反映させる仕組み。
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    /// <summary>
    /// 棚マスター変換 バインド
    /// </summary>
    public class BindBatchConvert : INotifyPropertyChanged
    {
        private string _no;
        /// <summary>
        /// No
        /// </summary>
        public string No
        {
            get { return _no; }
            set
            {
                if (_no != value)
                {
                    _no = value;
                    OnPropertyChanged(nameof(No));
                }
            }
        }

        private string _orderDate;
        /// <summary>
        /// 仕分納品日
        /// </summary>
        public string OrderDate
        {
            get { return _orderDate; }
            set
            {
                if (_orderDate != value)
                {
                    _orderDate = value;
                    OnPropertyChanged(nameof(OrderDate));
                }
            }
        }

        private string _postNo;
        /// <summary>
        /// 仕分便No
        /// </summary>
        public string PostNo
        {
            get { return _postNo; }
            set
            {
                if (_postNo != value)
                {
                    _postNo = value;
                    OnPropertyChanged(nameof(PostNo));
                }
            }
        }

        private string _orderDateRequest;
        /// <summary>
        /// 発注納品日
        /// </summary>
        public string OrderDateRequest
        {
            get { return _orderDateRequest; }
            set
            {
                if (_orderDateRequest != value)
                {
                    _orderDateRequest = value;
                    OnPropertyChanged(nameof(OrderDateRequest));
                }
            }
        }

        private string _postNoRequest;
        /// <summary>
        /// 発注便No
        /// </summary>
        public string PostNoRequest
        {
            get { return _postNoRequest; }
            set
            {
                if (_postNoRequest != value)
                {
                    _postNoRequest = value;
                    OnPropertyChanged(nameof(PostNoRequest));
                }
            }
        }

        private string _workNameKana;
        /// <summary>
        /// 商品名(カナ)
        /// </summary>
        public string WorkNameKana
        {
            get { return _workNameKana; }
            set
            {
                if (_workNameKana != value)
                {
                    _workNameKana = value;
                    OnPropertyChanged(nameof(WorkNameKana));
                }
            }
        }

        private string _workCode;
        /// <summary>
        /// 商品コード
        /// </summary>
        public string WorkCode
        {
            get { return _workCode; }
            set
            {
                if (_workCode != value)
                {
                    _workCode = value;
                    OnPropertyChanged(nameof(WorkCode));
                }
            }
        }

        private string _janCode;
        /// <summary>
        /// JANコード
        /// </summary>
        public string JANCode
        {
            get { return _janCode; }
            set
            {
                if (_janCode != value)
                {
                    _janCode = value;
                    OnPropertyChanged(nameof(JANCode));
                }
            }
        }

        private string _storeCode;
        /// <summary>
        /// 店コード
        /// </summary>
        public string StoreCode
        {
            get { return _storeCode; }
            set
            {
                if (_storeCode != value)
                {
                    _storeCode = value;
                    OnPropertyChanged(nameof(StoreCode));
                }
            }
        }

        private string _stationNo_Original;
        /// <summary>
        /// ステーションNo (PICKDATA)
        /// </summary>
        public string StationNo_Original
        {
            get { return _stationNo_Original; }
            set
            {
                if (_stationNo_Original != value)
                {
                    _stationNo_Original = value;
                    OnPropertyChanged(nameof(StationNo_Original));
                }
            }
        }

        private string _aisleNo_Original;
        /// <summary>
        /// アイルNo (PICKDATA)
        /// </summary>
        public string AisleNo_Original
        {
            get { return _aisleNo_Original; }
            set
            {
                if (_aisleNo_Original != value)
                {
                    _aisleNo_Original = value;
                    OnPropertyChanged(nameof(AisleNo_Original));
                }
            }
        }

        private string _slotNo_Original;
        /// <summary>
        /// スロットNo (PICKDATA)
        /// </summary>
        public string SlotNo_Original
        {
            get { return _slotNo_Original; }
            set
            {
                if (_slotNo_Original != value)
                {
                    _slotNo_Original = value;
                    OnPropertyChanged(nameof(SlotNo_Original));
                }
            }
        }

        private string _stationNo_Convert;
        /// <summary>
        /// ステーションNo (マテハン)
        /// </summary>
        public string StationNo_Convert
        {
            get { return _stationNo_Convert; }
            set
            {
                if (_stationNo_Convert != value)
                {
                    _stationNo_Convert = value;
                    OnPropertyChanged(nameof(StationNo_Convert));
                }
            }
        }

        private string _aisleNo_Convert;
        /// <summary>
        /// アイルNo (マテハン)
        /// </summary>
        public string AisleNo_Convert
        {
            get { return _aisleNo_Convert; }
            set
            {
                if (_aisleNo_Convert != value)
                {
                    _aisleNo_Convert = value;
                    OnPropertyChanged(nameof(AisleNo_Convert));
                }
            }
        }

        private string _slotNo_Convert;
        /// <summary>
        /// スロットNo (マテハン)
        /// </summary>
        public string SlotNo_Convert
        {
            get { return _slotNo_Convert; }
            set
            {
                if (_slotNo_Convert != value)
                {
                    _slotNo_Convert = value;
                    OnPropertyChanged(nameof(SlotNo_Convert));
                }
            }
        }

        private string _orderCount;
        /// <summary>
        /// 仕分け数
        /// </summary>
        public string OrderCount
        {
            get { return _orderCount; }
            set
            {
                if (_orderCount != value)
                {
                    _orderCount = value;
                    OnPropertyChanged(nameof(OrderCount));
                }
            }
        }

        private string _orderCompCount;
        /// <summary>
        /// 仕分け完了数
        /// </summary>
        public string OrderCompCount
        {
            get { return _orderCompCount; }
            set
            {
                if (_orderCompCount != value)
                {
                    _orderCompCount = value;
                    OnPropertyChanged(nameof(OrderCompCount));
                }
            }
        }


        // ListViewへバインディングしたデータの値が変更されたときに、
        // ListViewに変更を通知して反映させる仕組み。
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
