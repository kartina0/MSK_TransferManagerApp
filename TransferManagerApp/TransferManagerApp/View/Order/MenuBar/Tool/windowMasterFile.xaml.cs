using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;

using DL_Logger;
using ErrorCodeDefine;
using ServerModule;
using SystemConfig;
using ShareResource;


namespace TransferManagerApp
{
    /// <summary>
    /// windowMasterFile.xaml の相互作用ロジック
    /// </summary>
    public partial class windowMasterFile : Window
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
        /// 商品マスター
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindWorkMaster> _workMasterList = new ObservableCollection<BindWorkMaster>();
        /// <summary>
        /// 店マスター
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindStoreMaster> _storeMasterList = new ObservableCollection<BindStoreMaster>();
        /// <summary>
        /// 作業者マスター
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindWorkerMaster> _workerMasterList = new ObservableCollection<BindWorkerMaster>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public windowMasterFile()
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

                // ラジオボタン背景色設定
                btnWorkMaster.Background = Brushes.CornflowerBlue;
                btnStoreMaster.Background = Brushes.SlateGray;
                btnWorkerMaster.Background = Brushes.SlateGray;

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
                else if (ctrl == btnWorkMaster)
                {// 商品ヘッダ 表示
                    // ボタン背景色設定
                    btnWorkMaster.Background = (SolidColorBrush)Application.Current.Resources["ButtonColorOrder"];
                    btnStoreMaster.Background = Brushes.SlateGray;
                    btnWorkerMaster.Background = Brushes.SlateGray;
                    // DateGrid 表示切替
                    dataGridWorkMaster.Visibility = Visibility.Visible;
                    dataGridStoreMaster.Visibility = Visibility.Hidden;
                    dataGridWorkerMaster.Visibility = Visibility.Hidden;
                    // 行数更新
                    lblTotalCount.Content = _workMasterList.Count.ToString() + "行";
                }
                else if (ctrl == btnStoreMaster)
                {// 店マスタ 表示
                    // ボタン背景色設定
                    btnWorkMaster.Background = Brushes.SlateGray;
                    btnStoreMaster.Background = (SolidColorBrush)Application.Current.Resources["ButtonColorOrder"];
                    btnWorkerMaster.Background = Brushes.SlateGray;
                    // DateGrid 表示切替
                    dataGridWorkMaster.Visibility = Visibility.Hidden;
                    dataGridStoreMaster.Visibility = Visibility.Visible;
                    dataGridWorkerMaster.Visibility = Visibility.Hidden;
                    // 行数更新
                    lblTotalCount.Content = _storeMasterList.Count.ToString() + "行";
                }
                else if (ctrl == btnWorkerMaster)
                {// 作業者マスタ 表示
                    // ボタン背景色設定
                    btnWorkMaster.Background = Brushes.SlateGray;
                    btnStoreMaster.Background = Brushes.SlateGray;
                    btnWorkerMaster.Background = (SolidColorBrush)Application.Current.Resources["ButtonColorOrder"];
                    // DateGrid 表示切替
                    dataGridWorkMaster.Visibility = Visibility.Hidden;
                    dataGridStoreMaster.Visibility = Visibility.Hidden;
                    dataGridWorkerMaster.Visibility = Visibility.Visible;
                    // 行数更新
                    lblTotalCount.Content = _workerMasterList.Count.ToString() + "行";
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
                // 商品マスター
                int count = 0;
                _workMasterList.Clear();
                for (int postIndex = 0; postIndex < Const.MaxPostCount; postIndex++)
                {
                    foreach (MasterWork work in Resource.Server.MasterFile.MasterWorkList[postIndex])
                    {
                        count++;
                        BindWorkMaster b = new BindWorkMaster()
                        {
                            No = count,
                            OrderDate = $"{work.orderDate.Year.ToString("D4")}{work.orderDate.Month.ToString("D2")}{work.orderDate.Day.ToString("D2")}",
                            SupplierCode = work.supplierCode,
                            VDRCode = work.VDRCode,
                            SupplierName = work.supplierName.Trim(),
                            WorkCode = work.workCode,
                            PostNo = work.postNo.ToString(),
                            JANCode = work.JANCode,
                            JANClass = work.JANClass.ToString(),
                            WorkName = work.workName.Trim(),
                            DeptClass = work.deptClass.ToString(),
                            CenterCount = work.centerCount.ToString(),
                            PackCount = work.packCount.ToString(),
                            JANCode4digits = work.JANCode4digits.ToString(),
                            WorkNameKana = work.workNameKana.Trim(),
                        };
                        _workMasterList.Add(b);
                    };
                }
                dataGridWorkMaster.ItemsSource = _workMasterList;
                // 行数更新
                if (dataGridWorkMaster.Visibility == Visibility.Visible)
                    lblTotalCount.Content = _workMasterList.Count.ToString() + "行";


                // ---------------------------------------------------------
                // 店マスター
                count = 0;
                _storeMasterList.Clear();
                foreach (MasterStore store in Resource.Server.MasterFile.MasterStoreList)
                {
                    count++;
                    BindStoreMaster b = new BindStoreMaster()
                    {
                        No = count,
                        OrderDate = $"{store.orderDate.Year.ToString("D4")}{store.orderDate.Month.ToString("D2")}{store.orderDate.Day.ToString("D2")}",
                        CompanyType = store.companyType.ToString(),
                        StoreCode = store.storeCode.ToString(),
                        StoreName = store.storeName.Trim(),
                        PhoneNumber = store.phoneNumber.Trim(),

                        // 便01
                        Course01 = store.postInfo[0].course.ToString(),
                        Process01 = store.postInfo[0].process.ToString(),
                        Station01 = store.postInfo[0].station.ToString(),
                        Aisle01 = store.postInfo[0].aisle.ToString(),
                        Slot01 = store.postInfo[0].slot.ToString(),
                        DogNo01 = store.postInfo[0].dogNo,
                        Condition01 = store.postInfo[0].condition,
                        TimeArrive01 = store.postInfo[0].timeArrive,
                        TimeEntry01 = store.postInfo[0].timeEntry,
                        TimeDepart01 = store.postInfo[0].timeDepart,
                        TimeFinish01 = store.postInfo[0].timeFinish,
                        Company01 = store.postInfo[0].company,
                        CompanyName01 = store.postInfo[0].companyName.Trim(),
                        CarType01 = store.postInfo[0].carType,

                        // 便02
                        Course02 = store.postInfo[1].course.ToString(),
                        Process02 = store.postInfo[1].process.ToString(),
                        Station02 = store.postInfo[1].station.ToString(),
                        Aisle02 = store.postInfo[1].aisle.ToString(),
                        Slot02 = store.postInfo[1].slot.ToString(),
                        DogNo02 = store.postInfo[1].dogNo,
                        Condition02 = store.postInfo[1].condition,
                        TimeArrive02 = store.postInfo[1].timeArrive,
                        TimeEntry02 = store.postInfo[1].timeEntry,
                        TimeDepart02 = store.postInfo[1].timeDepart,
                        TimeFinish02 = store.postInfo[1].timeFinish,
                        Company02 = store.postInfo[1].company,
                        CompanyName02 = store.postInfo[1].companyName.Trim(),
                        CarType02 = store.postInfo[1].carType,

                        // 便03
                        Course03 = store.postInfo[2].course.ToString(),
                        Process03 = store.postInfo[2].process.ToString(),
                        Station03 = store.postInfo[2].station.ToString(),
                        Aisle03 = store.postInfo[2].aisle.ToString(),
                        Slot03 = store.postInfo[2].slot.ToString(),
                        DogNo03 = store.postInfo[2].dogNo,
                        Condition03 = store.postInfo[2].condition,
                        TimeArrive03 = store.postInfo[2].timeArrive,
                        TimeEntry03 = store.postInfo[2].timeEntry,
                        TimeDepart03 = store.postInfo[2].timeDepart,
                        TimeFinish03 = store.postInfo[2].timeFinish,
                        Company03 = store.postInfo[2].company,
                        CompanyName03 = store.postInfo[2].companyName.Trim(),
                        CarType03 = store.postInfo[2].carType,
                    };
                    _storeMasterList.Add(b);
                };
                dataGridStoreMaster.ItemsSource = _storeMasterList;
                // 行数更新
                if (dataGridStoreMaster.Visibility == Visibility.Visible)
                    lblTotalCount.Content = _storeMasterList.Count.ToString() + "行";


                // 作業者マスター
                count = 0;
                _workerMasterList.Clear();
                foreach (MasterWorker worker in Resource.Server.MasterFile.MasterWorkerList)
                {
                    count++;
                    BindWorkerMaster b = new BindWorkerMaster()
                    {
                        No = count,
                        WorkerChiefNo = worker.workerChiefNo.ToString(),
                        WorkerChiefName = worker.workerChiefName.Trim(),
                        WorkerNo01 = worker.workerNo[0].ToString(),
                        WorkerNo02 = worker.workerNo[1].ToString(),
                        WorkerNo03 = worker.workerNo[2].ToString(),
                        WorkerNo03_today = worker.workerNo[3].ToString(),
                    };
                    _workerMasterList.Add(b);
                };
                dataGridWorkerMaster.ItemsSource = _workerMasterList;
                // 行数更新
                if (dataGridWorkerMaster.Visibility == Visibility.Visible)
                    lblTotalCount.Content = _workerMasterList.Count.ToString() + "行";


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
    /// 商品マスター バインド
    /// </summary>
    public class BindWorkMaster : INotifyPropertyChanged
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
        /// 納品日
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

        private string _supplierCode;
        /// <summary>
        /// 取引先コード
        /// </summary>
        public string SupplierCode
        {
            get { return _supplierCode; }
            set
            {
                if (_supplierCode != value)
                {
                    _supplierCode = value;
                    OnPropertyChanged(nameof(SupplierCode));
                }
            }
        }

        private string _vdrCode;
        /// <summary>
        /// VDRコード
        /// </summary>
        public string VDRCode
        {
            get { return _vdrCode; }
            set
            {
                if (_vdrCode != value)
                {
                    _vdrCode = value;
                    OnPropertyChanged(nameof(VDRCode));
                }
            }
        }

        private string _supplierName;
        /// <summary>
        /// 取引先名(漢字)
        /// </summary>
        public string SupplierName
        {
            get { return _supplierName; }
            set
            {
                if (_supplierName != value)
                {
                    _supplierName = value;
                    OnPropertyChanged(nameof(SupplierName));
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

        private string _postNo;
        /// <summary>
        /// 便No
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

        private string _janClass;
        /// <summary>
        /// JAN区分
        /// </summary>
        public string JANClass
        {
            get { return _janClass; }
            set
            {
                if (_janClass != value)
                {
                    _janClass = value;
                    OnPropertyChanged(nameof(JANClass));
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

        private string _deptClass;
        /// <summary>
        /// DEPT-CLASS
        /// </summary>
        public string DeptClass
        {
            get { return _deptClass; }
            set
            {
                if (_deptClass != value)
                {
                    _deptClass = value;
                    OnPropertyChanged(nameof(DeptClass));
                }
            }
        }

        private string _centerCount;
        /// <summary>
        /// センター入数
        /// </summary>
        public string CenterCount
        {
            get { return _centerCount; }
            set
            {
                if (_centerCount != value)
                {
                    _centerCount = value;
                    OnPropertyChanged(nameof(CenterCount));
                }
            }
        }

        private string _packCount;
        /// <summary>
        /// パック入数
        /// </summary>
        public string PackCount
        {
            get { return _packCount; }
            set
            {
                if (_packCount != value)
                {
                    _packCount = value;
                    OnPropertyChanged(nameof(PackCount));
                }
            }
        }

        private string _janCode4digits;
        /// <summary>
        /// JANコード(下4桁)
        /// </summary>
        public string JANCode4digits
        {
            get { return _janCode4digits; }
            set
            {
                if (_janCode4digits != value)
                {
                    _janCode4digits = value;
                    OnPropertyChanged(nameof(JANCode4digits));
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


        // ListViewへバインディングしたデータの値が変更されたときに、
        // ListViewに変更を通知して反映させる仕組み。
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// 店マスター バインド
    /// </summary>
    public class BindStoreMaster : INotifyPropertyChanged
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
        /// 納品日
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

        private string _companyType;
        /// <summary>
        /// 社区分
        /// </summary>
        public string CompanyType
        {
            get { return _companyType; }
            set
            {
                if (_companyType != value)
                {
                    _companyType = value;
                    OnPropertyChanged(nameof(CompanyType));
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

        private string _storeName;
        /// <summary>
        /// 店名(漢字)
        /// </summary>
        public string StoreName
        {
            get { return _storeName; }
            set
            {
                if (_storeName != value)
                {
                    _storeName = value;
                    OnPropertyChanged(nameof(StoreName));
                }
            }
        }

        private string _phoneNumber;
        /// <summary>
        /// 電話番号
        /// </summary>
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                    OnPropertyChanged(nameof(PhoneNumber));
                }
            }
        }

        #region 便01
        private string _course01;
        /// <summary>
        /// コース01
        /// </summary>
        public string Course01
        {
            get { return _course01; }
            set
            {
                if (_course01 != value)
                {
                    _course01 = value;
                    OnPropertyChanged(nameof(Course01));
                }
            }
        }

        private string _process01;
        /// <summary>
        /// 順01
        /// </summary>
        public string Process01
        {
            get { return _process01; }
            set
            {
                if (_process01 != value)
                {
                    _process01 = value;
                    OnPropertyChanged(nameof(Process01));
                }
            }
        }

        private string _station01;
        /// <summary>
        /// ST01
        /// </summary>
        public string Station01
        {
            get { return _station01; }
            set
            {
                if (_station01 != value)
                {
                    _station01 = value;
                    OnPropertyChanged(nameof(Station01));
                }
            }
        }

        private string _aisle01;
        /// <summary>
        /// アイル01
        /// </summary>
        public string Aisle01
        {
            get { return _aisle01; }
            set
            {
                if (_aisle01 != value)
                {
                    _aisle01 = value;
                    OnPropertyChanged(nameof(Aisle01));
                }
            }
        }

        private string _slot01;
        /// <summary>
        /// スロット01
        /// </summary>
        public string Slot01
        {
            get { return _slot01; }
            set
            {
                if (_slot01 != value)
                {
                    _slot01 = value;
                    OnPropertyChanged(nameof(Slot01));
                }
            }
        }

        private string _dogNo01;
        /// <summary>
        /// ドッグNo01
        /// </summary>
        public string DogNo01
        {
            get { return _dogNo01; }
            set
            {
                if (_dogNo01 != value)
                {
                    _dogNo01 = value;
                    OnPropertyChanged(nameof(DogNo01));
                }
            }
        }

        private string _condition01;
        /// <summary>
        /// 搬入条件01
        /// </summary>
        public string Condition01
        {
            get { return _condition01; }
            set
            {
                if (_condition01 != value)
                {
                    _condition01 = value;
                    OnPropertyChanged(nameof(Condition01));
                }
            }
        }

        private string _timeArrive01;
        /// <summary>
        /// 到着01
        /// </summary>
        public string TimeArrive01
        {
            get { return _timeArrive01; }
            set
            {
                if (_timeArrive01 != value)
                {
                    _timeArrive01 = value;
                    OnPropertyChanged(nameof(TimeArrive01));
                }
            }
        }

        private string _timeEntry01;
        /// <summary>
        /// 入場01
        /// </summary>
        public string TimeEntry01
        {
            get { return _timeEntry01; }
            set
            {
                if (_timeEntry01 != value)
                {
                    _timeEntry01 = value;
                    OnPropertyChanged(nameof(TimeEntry01));
                }
            }
        }

        private string _timeDepart01;
        /// <summary>
        /// 出発01
        /// </summary>
        public string TimeDepart01
        {
            get { return _timeDepart01; }
            set
            {
                if (_timeDepart01 != value)
                {
                    _timeDepart01 = value;
                    OnPropertyChanged(nameof(TimeDepart01));
                }
            }
        }

        private string _timeFinish01;
        /// <summary>
        /// 終了01
        /// </summary>
        public string TimeFinish01
        {
            get { return _timeFinish01; }
            set
            {
                if (_timeFinish01 != value)
                {
                    _timeFinish01 = value;
                    OnPropertyChanged(nameof(TimeFinish01));
                }
            }
        }

        private string _company01;
        /// <summary>
        /// 運送会社01
        /// </summary>
        public string Company01
        {
            get { return _company01; }
            set
            {
                if (_company01 != value)
                {
                    _company01 = value;
                    OnPropertyChanged(nameof(Company01));
                }
            }
        }

        private string _companyName01;
        /// <summary>
        /// 運送会社名01
        /// </summary>
        public string CompanyName01
        {
            get { return _companyName01; }
            set
            {
                if (_companyName01 != value)
                {
                    _companyName01 = value;
                    OnPropertyChanged(nameof(CompanyName01));
                }
            }
        }

        private string _carType01;
        /// <summary>
        /// 車種01
        /// </summary>
        public string CarType01
        {
            get { return _carType01; }
            set
            {
                if (_carType01 != value)
                {
                    _carType01 = value;
                    OnPropertyChanged(nameof(CarType01));
                }
            }
        }
        #endregion

        #region 便02
        private string _course02;
        /// <summary>
        /// コース02
        /// </summary>
        public string Course02
        {
            get { return _course02; }
            set
            {
                if (_course02 != value)
                {
                    _course02 = value;
                    OnPropertyChanged(nameof(Course02));
                }
            }
        }

        private string _process02;
        /// <summary>
        /// 順02
        /// </summary>
        public string Process02
        {
            get { return _process02; }
            set
            {
                if (_process02 != value)
                {
                    _process02 = value;
                    OnPropertyChanged(nameof(Process02));
                }
            }
        }

        private string _station02;
        /// <summary>
        /// ST02
        /// </summary>
        public string Station02
        {
            get { return _station02; }
            set
            {
                if (_station02 != value)
                {
                    _station02 = value;
                    OnPropertyChanged(nameof(Station02));
                }
            }
        }

        private string _aisle02;
        /// <summary>
        /// アイル02
        /// </summary>
        public string Aisle02
        {
            get { return _aisle02; }
            set
            {
                if (_aisle02 != value)
                {
                    _aisle02 = value;
                    OnPropertyChanged(nameof(Aisle02));
                }
            }
        }

        private string _slot02;
        /// <summary>
        /// スロット02
        /// </summary>
        public string Slot02
        {
            get { return _slot02; }
            set
            {
                if (_slot02 != value)
                {
                    _slot02 = value;
                    OnPropertyChanged(nameof(Slot02));
                }
            }
        }

        private string _dogNo02;
        /// <summary>
        /// ドッグNo02
        /// </summary>
        public string DogNo02
        {
            get { return _dogNo02; }
            set
            {
                if (_dogNo02 != value)
                {
                    _dogNo02 = value;
                    OnPropertyChanged(nameof(DogNo02));
                }
            }
        }

        private string _condition02;
        /// <summary>
        /// 搬入条件02
        /// </summary>
        public string Condition02
        {
            get { return _condition02; }
            set
            {
                if (_condition02 != value)
                {
                    _condition02 = value;
                    OnPropertyChanged(nameof(Condition02));
                }
            }
        }

        private string _timeArrive02;
        /// <summary>
        /// 到着02
        /// </summary>
        public string TimeArrive02
        {
            get { return _timeArrive02; }
            set
            {
                if (_timeArrive02 != value)
                {
                    _timeArrive02 = value;
                    OnPropertyChanged(nameof(TimeArrive02));
                }
            }
        }

        private string _timeEntry02;
        /// <summary>
        /// 入場02
        /// </summary>
        public string TimeEntry02
        {
            get { return _timeEntry02; }
            set
            {
                if (_timeEntry02 != value)
                {
                    _timeEntry02 = value;
                    OnPropertyChanged(nameof(TimeEntry02));
                }
            }
        }

        private string _timeDepart02;
        /// <summary>
        /// 出発02
        /// </summary>
        public string TimeDepart02
        {
            get { return _timeDepart02; }
            set
            {
                if (_timeDepart02 != value)
                {
                    _timeDepart02 = value;
                    OnPropertyChanged(nameof(TimeDepart02));
                }
            }
        }

        private string _timeFinish02;
        /// <summary>
        /// 終了02
        /// </summary>
        public string TimeFinish02
        {
            get { return _timeFinish02; }
            set
            {
                if (_timeFinish02 != value)
                {
                    _timeFinish02 = value;
                    OnPropertyChanged(nameof(TimeFinish02));
                }
            }
        }

        private string _company02;
        /// <summary>
        /// 運送会社02
        /// </summary>
        public string Company02
        {
            get { return _company02; }
            set
            {
                if (_company02 != value)
                {
                    _company02 = value;
                    OnPropertyChanged(nameof(Company02));
                }
            }
        }

        private string _companyName02;
        /// <summary>
        /// 運送会社名02
        /// </summary>
        public string CompanyName02
        {
            get { return _companyName02; }
            set
            {
                if (_companyName02 != value)
                {
                    _companyName02 = value;
                    OnPropertyChanged(nameof(CompanyName02));
                }
            }
        }

        private string _carType02;
        /// <summary>
        /// 車種02
        /// </summary>
        public string CarType02
        {
            get { return _carType02; }
            set
            {
                if (_carType02 != value)
                {
                    _carType02 = value;
                    OnPropertyChanged(nameof(CarType02));
                }
            }
        }
        #endregion

        #region 便03
        private string _course03;
        /// <summary>
        /// コース03
        /// </summary>
        public string Course03
        {
            get { return _course03; }
            set
            {
                if (_course03 != value)
                {
                    _course03 = value;
                    OnPropertyChanged(nameof(Course03));
                }
            }
        }

        private string _process03;
        /// <summary>
        /// 順03
        /// </summary>
        public string Process03
        {
            get { return _process03; }
            set
            {
                if (_process03 != value)
                {
                    _process03 = value;
                    OnPropertyChanged(nameof(Process03));
                }
            }
        }

        private string _station03;
        /// <summary>
        /// ST03
        /// </summary>
        public string Station03
        {
            get { return _station03; }
            set
            {
                if (_station03 != value)
                {
                    _station03 = value;
                    OnPropertyChanged(nameof(Station03));
                }
            }
        }

        private string _aisle03;
        /// <summary>
        /// アイル03
        /// </summary>
        public string Aisle03
        {
            get { return _aisle03; }
            set
            {
                if (_aisle03 != value)
                {
                    _aisle03 = value;
                    OnPropertyChanged(nameof(Aisle03));
                }
            }
        }

        private string _slot03;
        /// <summary>
        /// スロット03
        /// </summary>
        public string Slot03
        {
            get { return _slot03; }
            set
            {
                if (_slot03 != value)
                {
                    _slot03 = value;
                    OnPropertyChanged(nameof(Slot03));
                }
            }
        }

        private string _dogNo03;
        /// <summary>
        /// ドッグNo03
        /// </summary>
        public string DogNo03
        {
            get { return _dogNo03; }
            set
            {
                if (_dogNo03 != value)
                {
                    _dogNo03 = value;
                    OnPropertyChanged(nameof(DogNo03));
                }
            }
        }

        private string _condition03;
        /// <summary>
        /// 搬入条件03
        /// </summary>
        public string Condition03
        {
            get { return _condition03; }
            set
            {
                if (_condition03 != value)
                {
                    _condition03 = value;
                    OnPropertyChanged(nameof(Condition03));
                }
            }
        }

        private string _timeArrive03;
        /// <summary>
        /// 到着03
        /// </summary>
        public string TimeArrive03
        {
            get { return _timeArrive03; }
            set
            {
                if (_timeArrive03 != value)
                {
                    _timeArrive03 = value;
                    OnPropertyChanged(nameof(TimeArrive03));
                }
            }
        }

        private string _timeEntry03;
        /// <summary>
        /// 入場03
        /// </summary>
        public string TimeEntry03
        {
            get { return _timeEntry03; }
            set
            {
                if (_timeEntry03 != value)
                {
                    _timeEntry03 = value;
                    OnPropertyChanged(nameof(TimeEntry03));
                }
            }
        }

        private string _timeDepart03;
        /// <summary>
        /// 出発03
        /// </summary>
        public string TimeDepart03
        {
            get { return _timeDepart03; }
            set
            {
                if (_timeDepart03 != value)
                {
                    _timeDepart03 = value;
                    OnPropertyChanged(nameof(TimeDepart03));
                }
            }
        }

        private string _timeFinish03;
        /// <summary>
        /// 終了03
        /// </summary>
        public string TimeFinish03
        {
            get { return _timeFinish03; }
            set
            {
                if (_timeFinish03 != value)
                {
                    _timeFinish03 = value;
                    OnPropertyChanged(nameof(TimeFinish03));
                }
            }
        }

        private string _company03;
        /// <summary>
        /// 運送会社03
        /// </summary>
        public string Company03
        {
            get { return _company03; }
            set
            {
                if (_company03 != value)
                {
                    _company03 = value;
                    OnPropertyChanged(nameof(Company03));
                }
            }
        }

        private string _companyName03;
        /// <summary>
        /// 運送会社名03
        /// </summary>
        public string CompanyName03
        {
            get { return _companyName03; }
            set
            {
                if (_companyName03 != value)
                {
                    _companyName03 = value;
                    OnPropertyChanged(nameof(CompanyName03));
                }
            }
        }

        private string _carType03;
        /// <summary>
        /// 車種03
        /// </summary>
        public string CarType03
        {
            get { return _carType03; }
            set
            {
                if (_carType03 != value)
                {
                    _carType03 = value;
                    OnPropertyChanged(nameof(CarType03));
                }
            }
        }
        #endregion


        // ListViewへバインディングしたデータの値が変更されたときに、
        // ListViewに変更を通知して反映させる仕組み。
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// 作業者マスター バインド
    /// </summary>
    public class BindWorkerMaster : INotifyPropertyChanged
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

        private string _workerChiefNo;
        /// <summary>
        /// 作業者No(代表)
        /// </summary>
        public string WorkerChiefNo
        {
            get { return _workerChiefNo; }
            set
            {
                if (_workerChiefNo != value)
                {
                    _workerChiefNo = value;
                    OnPropertyChanged(nameof(WorkerChiefNo));
                }
            }
        }

        private string _workerChiefName;
        /// <summary>
        /// 作業者名(代表)
        /// </summary>
        public string WorkerChiefName
        {
            get { return _workerChiefName; }
            set
            {
                if (_workerChiefName != value)
                {
                    _workerChiefName = value;
                    OnPropertyChanged(nameof(WorkerChiefName));
                }
            }
        }

        private string _workerNo01;
        /// <summary>
        /// 作業者名 1便
        /// </summary>
        public string WorkerNo01
        {
            get { return _workerNo01; }
            set
            {
                if (_workerNo01 != value)
                {
                    _workerNo01 = value;
                    OnPropertyChanged(nameof(WorkerNo01));
                }
            }
        }

        private string _workerNo02;
        /// <summary>
        /// 作業者名 2便
        /// </summary>
        public string WorkerNo02
        {
            get { return _workerNo02; }
            set
            {
                if (_workerNo02 != value)
                {
                    _workerNo02 = value;
                    OnPropertyChanged(nameof(WorkerNo02));
                }
            }
        }

        private string _workerNo03;
        /// <summary>
        /// 作業者名 3便
        /// </summary>
        public string WorkerNo03
        {
            get { return _workerNo03; }
            set
            {
                if (_workerNo03 != value)
                {
                    _workerNo03 = value;
                    OnPropertyChanged(nameof(WorkerNo03));
                }
            }
        }

        private string _workerNo03_today;
        /// <summary>
        /// 作業者名 当日3便
        /// </summary>
        public string WorkerNo03_today
        {
            get { return _workerNo03_today; }
            set
            {
                if (_workerNo03_today != value)
                {
                    _workerNo03_today = value;
                    OnPropertyChanged(nameof(WorkerNo03_today));
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
