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

using SystemConfig;
using DL_Logger;
using ErrorCodeDefine;
using ServerModule;
using ShareResource;
using System.Security.Cryptography.X509Certificates;


namespace TransferManagerApp
{
    /// <summary>
    /// windowWorkStoreCount.xaml の相互作用ロジック
    /// </summary>
    public partial class windowWorkStoreCount : Window
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "windowWorkStoreCount";

        /// <summary>
        /// 仕分日
        /// </summary>
        private DateTime _orderDate = DateTime.Now;
        /// <summary>
        /// 便No
        /// </summary>
        private int _postNo = 0;
        /// <summary>
        /// アイルNo
        /// </summary>
        private int _aisleNo = 0;
        /// <summary>
        /// JANコード
        /// </summary>
        private string _janCode = "";
        /// <summary>
        /// 商品名
        /// </summary>
        private string _workName = "";

        /// <summary>
        /// 店別仕分数リスト
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindWorkStoreCount> _workStoreCountList = new ObservableCollection<BindWorkStoreCount>();


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public windowWorkStoreCount(DateTime orderDate, int postNo, int aisleNo, string janCode, string workName)
        {
            InitializeComponent();

            _orderDate = orderDate;
            _postNo = postNo;
            _aisleNo = aisleNo;
            _janCode = janCode;
            _workName = workName;
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

                // ListView初期化
                rc = InitListView();

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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
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
        /// ボタン クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            UInt32 rc = 0;
            Button ctrl = (Button)sender;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {ctrl.Name}");
            try
            {
                if (ctrl == btnClose)
                {
                    this.Close();
                }

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
                lblOrderDate.Content = $"{_orderDate.Year.ToString("D4")}/{_orderDate.Month.ToString("D2")}/{_orderDate.Day.ToString("D2")}";
                lblPostNo.Content = _postNo.ToString();
                lblAisleNo.Content = _aisleNo.ToString();
                lblWorkName.Content = _workName.ToString();



                _workStoreCountList = null;
                //_workStoreCountList = new ObservableCollection<BindWorkStoreCount>();
                ObservableCollection<BindWorkStoreCount> _list = new ObservableCollection<BindWorkStoreCount>();

                //// 商品名から商品コードを取得
                //rc = Resource.Server.MasterFile.GetJanCodeFromWorkName(_postNo, _workName, out string janCode);
                // 仕分け実績リストから該当するデータのリストを取得
                rc = Resource.Server.OrderInfo.SelectExecuteData(_orderDate, _postNo - 1, _janCode, out ExecuteData executeData);

                // バッチファイルから、該当アイルの店コード一覧を取得

                List<ExecuteStoreData> storeList = executeData.storeDataList.Where(x => x.aisleNo_MH == _aisleNo).ToList();
                foreach (ExecuteStoreData store in storeList)
                {
                    string batchNo = store.batchNo_MH.ToString();
                    string slotNo = store.slotNo_MH.ToString();
                    string storeCode = store.storeCode;
                    rc = Resource.Server.MasterFile.GetStoreNameFromStoreCode(storeCode, out string storeName);
                    storeName = storeName.Trim();
                    int orderCount = (int)store.orderCount;
                    int orderCompCount = (int)store.orderCompCount;
                    int orderRemainCount = orderCount - orderCompCount;
                    string status = "";

                    // 1行作成
                    BindWorkStoreCount b = new BindWorkStoreCount()
                    {
                        BatchNo = batchNo,
                        SlotNo = slotNo,
                        StoreName = storeName,
                        StoreCode = storeCode,
                        OrderCount = orderCount,
                        OrderCompCount = orderCompCount,
                        OrderRemainCount = orderRemainCount,
                        Status = ""
                    };
                    _list.Add(b);
                }
                _workStoreCountList = new ObservableCollection<BindWorkStoreCount>(_list.OrderBy(x => (int.Parse)(x.SlotNo)));
                //_workStoreCountList = _list.OrderBy(x => x.SlotNo).ToList();
                //_workStoreCountList.Sort((p1, p2) => p1.SlotNo.CompareTo(p2.Age));
                listviewWorkStoreCount.ItemsSource = _workStoreCountList;


                // 表の各カラムのWidthを調整
                double w = listviewWorkStoreCount.ActualWidth;
                columnBatchNo.Width = Math.Floor(w * 0.08);
                columnSlotNo.Width = Math.Floor(w * 0.08);
                columnStoreName.Width = Math.Floor(w * 0.34);
                columnStoreCode.Width = Math.Floor(w * 0.1);
                columnOrderCount.Width = Math.Floor(w * 0.09);
                columnOrderCompCount.Width = Math.Floor(w * 0.09);
                columnOrderRemainCount.Width = Math.Floor(w * 0.09);
                columnStatus.Width = Math.Floor(w * 0.12);

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
    public class BindWorkStoreCount : INotifyPropertyChanged
    {
        private string _batchNo;
        /// <summary>
        /// バッチNo
        /// </summary>
        public string BatchNo
        {
            get { return _batchNo; }
            set
            {
                if (_batchNo != value)
                {
                    _batchNo = value;
                    OnPropertyChanged(nameof(BatchNo));
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

        private string _storeName;
        /// <summary>
        /// 店名
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

}
