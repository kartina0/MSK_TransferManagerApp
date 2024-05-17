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
using System.Windows.Controls;
using System.Windows.Input;

using ServerModule;
using SystemConfig;
using DL_Logger;
using ErrorCodeDefine;
using ShareResource;


namespace TransferManagerApp
{

    /// <summary>
    /// ucTab_WorkOrderStatus.xaml の相互作用ロジック
    /// </summary>
    public partial class ucTab_WorkStatus : UserControl, IDisposable
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "ucTab_WorkStatus";

        /// <summary>
        /// ボタン連続操作防止フラグ
        /// </summary>
        private bool _buttonLock = false;

        /// <summary>
        /// 初期化完了
        /// </summary>
        private bool _initialized = false;


        /// <summary>
        /// 仕分進捗情報リスト(全体)
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindWorkInfo> _workStatusList = new ObservableCollection<BindWorkInfo>();
        /// <summary> コンボボックスバインド 便No </summary>
        private ObservableCollection<BindComboPostNo> _comboPostNoList;
        /// <summary> コンボボックスバインド ステーションNo </summary>
        private ObservableCollection<BindComboStationNo> _comboStationNoList;
        /// <summary> コンボボックスバインド アイルNo </summary>
        private ObservableCollection<BindComboAisleNo> _comboAisleNoList;
        /// <summary> コンボボックスバインド 仕分完了/未完了No </summary>
        private ObservableCollection<BindComboStatus> _comboStatusList;
        /// <summary> コンボボックスバインド 商品名 </summary>
        private ObservableCollection<BindComboWorkName> _comboWorkNameList;
        /// <summary> コンボボックスバインド 取引先名 </summary>
        private ObservableCollection<BindComboSupplierName> _comboSupplierNameList;

        /// <summary>
        /// 店舗一覧画面オブジェクト
        /// </summary>
        private windowWorkStoreCount _wWorkStoreCount = null;


        /// 店舗一覧画面表示用の変数
        private DateTime _orderDate = DateTime.Now;
        private int _postNo = 0;
        private int _aisleNo = 0;
        private string _workName = "";


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ucTab_WorkStatus()
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

                // ListView初期化
                rc = InitListView();
                // 商品名コンボ更新
                rc = UpdateWorkNameComboBox();

                // 検索
                rc = Search();

                _initialized = true;

                if (!STATUS_SUCCESS(rc))
                    Resource.ErrorHandler(rc, true);
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
        public void Dispose()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                if(_wWorkStoreCount != null)
                    _wWorkStoreCount.Close();
                _wWorkStoreCount = null;
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
            if (_buttonLock)
                return;
            _buttonLock = true;

            UInt32 rc = 0;
            Button ctrl = (Button)sender;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {ctrl.Name}");
            try
            {
                if (ctrl == btnSearch)
                {// 検索
                    rc = Search();
                }
                //else if (ctrl == btnUpdate)
                //{// 更新
                //    rc = Update();
                //}

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
        /// コンボボックス選択イベント
        /// ※仕分日付、便、アイルを選択した際に、商品名コンボの内容を更新する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UInt32 rc = 0;
            if (!_initialized)
                return;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() " +
                $"日付:{dpOrderDate.SelectedDate.ToString()} 便:{_comboPostNoList[comboPostNo.SelectedIndex].DisplayValue} アイル:{_comboAisleNoList[comboAisleNo.SelectedIndex].DisplayValue}");
            try
            {
                // 商品名コンボ更新
                UpdateWorkNameComboBox();
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
        }
        /// <summary>
        /// ListView アイテム行 ダブルクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UInt32 rc = 0;
            try
            {
                ListViewItem l = (ListViewItem)sender;
                BindWorkInfo item = (BindWorkInfo)l.DataContext;
                Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {item.WorkName} {item.PostNo}便 アイル{item.AisleNo} をダブルクリック");

                _wWorkStoreCount = new windowWorkStoreCount(_orderDate, item.PostNo, item.AisleNo, item.JanCode, item.WorkName);
                _wWorkStoreCount.ShowDialog();
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
        }


        /// <summary>
        /// ListView初期化
        /// </summary>
        /// <returns></returns>
        private UInt32 InitListView()
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                DateTime dt = DateTime.Now;
                // 日付切り替わり処理
                if (dt < DateTime.Today.AddHours(IniFile.DateChangeTime.Hour).AddMinutes(IniFile.DateChangeTime.Minute))
                {
                    dt = dt.AddDays(-1);
                }

                dpOrderDate.SelectedDate = dt;

                // コンボボックス 便No
                _comboPostNoList = new ObservableCollection<BindComboPostNo>
                {
                    new BindComboPostNo{ Value = 0, DisplayValue = "1" },
                    new BindComboPostNo{ Value = 1, DisplayValue = "2" },
                    new BindComboPostNo{ Value = 2, DisplayValue = "3" }
                };
                comboPostNo.ItemsSource = _comboPostNoList;
                comboPostNo.SelectedIndex = Resource.SystemStatus.CurrentPostIndex;
                // コンボボックス アイル
                _comboAisleNoList = new ObservableCollection<BindComboAisleNo>();
                _comboAisleNoList.Add(new BindComboAisleNo { Value = -1, DisplayValue = "全て" });
                for (int i = 0; i < Const.MaxAisleCount; i++)
                {
                    if(IniFile.AisleEnable[i])
                        _comboAisleNoList.Add(new BindComboAisleNo { Value = i, DisplayValue = $"{i + 1}" });
                }
                comboAisleNo.ItemsSource = _comboAisleNoList;
                comboAisleNo.SelectedIndex = 0;
                // コンボボックス 仕分け完了/未完了
                _comboStatusList = new ObservableCollection<BindComboStatus>
                {
                    new BindComboStatus{ Value = -1, DisplayValue = "全て" },
                    new BindComboStatus{ Value = 0, DisplayValue = "仕分け未完了" },
                    new BindComboStatus{ Value = 1, DisplayValue = "仕分け完了" },
                };
                comboStatus.ItemsSource = _comboStatusList;
                comboStatus.SelectedIndex = 0;

                // コンボボックス 商品名/取引先名
                //rc = UpdateWorkNameList();
                _comboWorkNameList = new ObservableCollection<BindComboWorkName>
                {
                    new BindComboWorkName{ Value = -1, DisplayValue = "全て" },
                };
                comboWorkName.ItemsSource = _comboWorkNameList;
                comboWorkName.SelectedIndex = 0;

                // 表の各カラムのWidthを調整
                double w = listviewWorkStatus.ActualWidth;
                columnPostNo.Width = Math.Floor(w * 0.03);
                columnAisleNo.Width = Math.Floor(w * 0.03);
                columnJanCode.Width = Math.Floor(w * 0.1);
                columnWorkCode.Width = Math.Floor(w * 0.06);
                columnWorkName.Width = Math.Floor(w * 0.27);
                columnSupplierCode.Width = Math.Floor(w * 0.07);
                columnSupplierName.Width = Math.Floor(w * 0.2);
                columnOrderCount.Width = Math.Floor(w * 0.05);
                columnOrderCompCount.Width = Math.Floor(w * 0.05);
                columnOrderRemainCount.Width = Math.Floor(w * 0.05);
                columnStatus.Width = Math.Floor(w * 0.09);

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
        /// 商品名コンボボックス 更新
        /// </summary>
        /// <returns></returns>
        public UInt32 UpdateWorkNameComboBox()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                //if (comboPostNo == null)
                //    return;
                //if (comboAisleNo == null)
                //    return;


                _comboWorkNameList = null;
                _comboWorkNameList = new ObservableCollection<BindComboWorkName>();
                List<string> janCodeList = new List<string>();

                DateTime selectedDt = (dpOrderDate.SelectedDate.Value);
                int postIndex = (int)comboPostNo.SelectedValue;
                int aisleIndex = (int)comboAisleNo.SelectedValue;

                // --------------------------------------------------------------
                // 仕分納品日、便
                List<ExecuteData> list_PostSorted = Resource.Server.OrderInfo.ExecuteDataList[postIndex].Where(x => x.orderDate == selectedDt).ToList();


                foreach (ExecuteData work in list_PostSorted)
                {
                    // --------------------------------------------------------------
                    // アイル
                    for (int i = 0; i < Const.MaxAisleCount; i++)
                    {
                        if (aisleIndex != -1)
                        {
                            if (aisleIndex != i)
                                continue;
                        }

                        List<ExecuteStoreData> storelist = work.storeDataList.Where(x => x.aisleNo_MH == i + 1).ToList();
                        if (storelist == null || storelist.Count <= 0)
                        {
                            continue;
                        }
                        else
                        {
                            janCodeList.Add(work.JANCode);
                        }
                    }
                }


                BindComboWorkName b = new BindComboWorkName
                {
                    Value = -1,
                    DisplayValue = "全て"
                };
                _comboWorkNameList.Add(b);

                if (janCodeList.Count > 0)
                {
                    for (int i = 0; i < janCodeList.Count; i++)
                    {
                        rc = Resource.Server.MasterFile.GetWorkNameFromJanCode(postIndex, janCodeList[i], out string workName);
                        BindComboWorkName bWorkName = new BindComboWorkName
                        {
                            Value = i,
                            DisplayValue = workName
                        };
                        _comboWorkNameList.Add(bWorkName);
                    }
                }

                comboWorkName.ItemsSource = _comboWorkNameList;
                comboWorkName.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }

        /// <summary>
        /// 検索
        /// </summary>
        /// <returns></returns>
        public UInt32 Search()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                _workStatusList = null;
                _workStatusList = new ObservableCollection<BindWorkInfo>();

                // --------------------------------------------------------------
                // コンボの値を取得
                DateTime selectedDt = (dpOrderDate.SelectedDate.Value);
                //string dt = $"{selectedDt.Year.ToString("D4")}{selectedDt.Month.ToString("D2")}{selectedDt.Day.ToString("D2")}";
                int postIndex = (int)comboPostNo.SelectedValue;
                //int stationIndex = (int)comboStationNo.SelectedValue;
                int aisleIndex = (int)comboAisleNo.SelectedValue;
                int statusIndex = (int)comboStatus.SelectedValue;
                int workNameIndex = (int)comboWorkName.SelectedValue;
                //int suplierNameIndex = (int)comboSupplierName.SelectedValue;


                // --------------------------------------------------------------
                // 仕分納品日、便
                List<ExecuteData> list_PostSorted = Resource.Server.OrderInfo.ExecuteDataList[postIndex].Where(x => x.orderDate == selectedDt).ToList();

                // --------------------------------------------------------------
                // 商品名
                List<ExecuteData> list_WorkSorted = null;
                if (workNameIndex == -1)
                {// 全商品
                    list_WorkSorted = list_PostSorted;
                }
                else 
                {// 選択された商品のみ
                    // 該当する商品名を取得
                    string workName = (_comboWorkNameList.Where(x => x.Value == workNameIndex).FirstOrDefault()).DisplayValue;
                    // 商品名からJANコードを取得
                    rc = Resource.Server.MasterFile.GetJanCodeFromWorkName(postIndex, workName, out string janCode);
                    //rc = Resource.Server.MasterFile.GetWorkCodeFromJanCode(postIndex, janCode, out workCode);

                    list_WorkSorted = list_PostSorted.Where(x => x.JANCode == janCode).ToList();
                }



                foreach (ExecuteData work in list_WorkSorted)
                {
                    for (int i = 0; i < Const.MaxAisleCount; i++)
                    {
                        if (aisleIndex != -1) 
                        {
                            if (aisleIndex != i)
                                continue;
                        }

                        List<ExecuteStoreData> storelist = work.storeDataList.Where(x => x.aisleNo_MH == i + 1).ToList();
                        if (storelist == null || storelist.Count <= 0)
                            continue;

                        string janCode = work.JANCode;
                        rc = Resource.Server.MasterFile.GetWorkNameFromJanCode(postIndex, janCode, out string workName);
                        workName = workName.Trim();
                        rc = Resource.Server.MasterFile.GetWorkCodeFromJanCode(postIndex, janCode, out string workCode);
                        rc = Resource.Server.MasterFile.GetSuplierFromJanCode(postIndex, janCode, out string supplierCode, out string supplierName);
                        supplierName = supplierName.Trim();
                        int orderCount = (int)storelist.Sum(x => x.orderCount);
                        int orderCompCount = (int)storelist.Sum(x => x.orderCompCount);
                        int orderRemainCount = orderCount - orderCompCount;



                        BindWorkInfo b = new BindWorkInfo() 
                        {
                            PostNo = postIndex + 1,
                            AisleNo = i + 1,
                            JanCode = janCode,
                            WorkCode = workCode,
                            WorkName = workName,
                            SupplierCode = supplierCode,
                            SupplierName = supplierName,
                            OrderCount = orderCount,
                            OrderCompCount = orderCompCount,
                            OrderRemainCount = orderRemainCount
                        };

                        _workStatusList.Add(b);

                    }
                }


                // リストビューにバインド
                listviewWorkStatus.ItemsSource = _workStatusList;

                //// 店舗一覧画面表示向けに値を代入しておく
                //_orderDate = selectedDt;

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }


        ///// <summary>
        ///// 更新
        ///// </summary>
        ///// <returns></returns>
        //private UInt32 Update()
        //{
        //    UInt32 rc = 0;
        //    Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
        //    try
        //    {
        //        // 商品名コンボボックスを更新
        //        rc = Resource.Server.MasterFile.GetAllWorkName(out string[] allWorkNameList);
        //        _comboWorkNameList = null;
        //        _comboWorkNameList = new ObservableCollection<BindComboWorkName> { new BindComboWorkName { Value = -1, DisplayValue = "全て" } };
        //        for (int i = 0; i < allWorkNameList.Length; i++) 
        //            _comboWorkNameList.Add( new BindComboWorkName() { Value = i, DisplayValue = allWorkNameList[i] } );
        //        comboWorkName.ItemsSource = _comboWorkNameList;
        //        comboWorkName.SelectedIndex = 0;

        //        //// 取引先名コンボボックスを更新
        //        //rc = Resource.Server.MasterFile.GetAllSuplierName(out string[] allSuplierNameList);
        //        //_comboSupplierNameList = null;
        //        //_comboSupplierNameList = new ObservableCollection<BindComboSupplierName> { new BindComboSupplierName { Value = -1, DisplayValue = "全て" } };
        //        //for (int i = 0; i < allSuplierNameList.Length; i++)
        //        //    _comboSupplierNameList.Add(new BindComboSupplierName() { Value = i, DisplayValue = allSuplierNameList[i] });
        //        //comboSupplierName.ItemsSource = _comboSupplierNameList;
        //        //comboSupplierName.SelectedIndex = 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        rc = (Int32)ErrorCodeList.EXCEPTION;
        //        Resource.ErrorHandler(ex);
        //    }
        //    Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
        //    return rc;
        //}



        /// <summary>
        /// Check Error State
        /// </summary>
        private static bool STATUS_SUCCESS(UInt32 err)
        {
            return err == (int)ErrorCodeList.STATUS_SUCCESS;
        }

    }






    /// <summary>
    /// エントリーワーク情報
    /// 画面の表にデータバインドするようのクラス
    /// </summary>
    public class BindWorkInfo : INotifyPropertyChanged
    {

        private int _postNo;
        /// <summary>
        /// 便No
        /// </summary>
        public int PostNo
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

        private int _aisleNo;
        /// <summary>
        /// アイルNo
        /// </summary>
        public int AisleNo
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

        private string _janCode;
        /// <summary>
        /// JANコード
        /// </summary>
        public string JanCode
        {
            get { return _janCode; }
            set
            {
                if (_janCode != value)
                {
                    _janCode = value;
                    OnPropertyChanged(nameof(JanCode));
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

        private string _workName;
        /// <summary>
        /// 商品名
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

        private string _supplierName;
        /// <summary>
        /// 取引先名
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

        private double _orderCount;
        /// <summary>
        /// 仕分予定数
        /// </summary>
        public double OrderCount
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

        private double _orderCompCount;
        /// <summary>
        /// 仕分完了数
        /// </summary>
        public double OrderCompCount
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

        private double _orderRemainCount;
        /// <summary>
        /// 残り数
        /// </summary>
        public double OrderRemainCount
        {
            get { return _orderRemainCount; }
            set
            {
                if (_orderRemainCount != value)
                {
                    _orderRemainCount = value;
                    OnPropertyChanged(nameof(OrderRemainCount));
                }
            }
        }

        private string _status;
        /// <summary>
        /// ステータス
        /// </summary>
        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
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
    /// コンボボックスバインド
    /// 便No
    /// </summary>
    public class BindComboPostNo
    {
        public int Value { get; set; }
        public string DisplayValue { get; set; }
    }
    /// <summary>
    /// コンボボックスバインド
    /// ステーションNo
    /// </summary>
    public class BindComboStationNo
    {
        public int Value { get; set; }
        public string DisplayValue { get; set; }
    }
    /// <summary>
    /// コンボボックスバインド
    /// アイルNo
    /// </summary>
    public class BindComboAisleNo
    {
        public int Value { get; set; }
        public string DisplayValue { get; set; }
    }
    /// <summary>
    /// コンボボックスバインド
    /// 仕分完了/未完了
    /// </summary>
    public class BindComboStatus
    {
        public int Value { get; set; }
        public string DisplayValue { get; set; }
    }
    /// <summary>
    /// コンボボックスバインド
    /// 商品名
    /// </summary>
    public class BindComboWorkName
    {
        public int Value { get; set; }
        public string DisplayValue { get; set; }
    }
    /// <summary>
    /// コンボボックスバインド
    /// 取引先名
    /// </summary>
    public class BindComboSupplierName
    {
        public int Value { get; set; }
        public string DisplayValue { get; set; }
    }

}
