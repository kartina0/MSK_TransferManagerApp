//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Timers;

using ServerModule;
using PLCModule;
using DL_CommonLibrary;
using DL_Logger;
using SystemConfig;
using ShareResource;
using ErrorCodeDefine;

namespace TransferManagerApp
{
    /// <summary>
    /// ucAisle.xaml の相互作用ロジック
    /// </summary>
    public partial class ucAisle : UserControl, IDisposable
    {
        private const string THIS_NAME = "ucAisle";

        /// <summary>
        /// アイルIndex
        /// </summary>
        public int AisleIndex { get; set; }

        /// <summary>
        /// リストの最大保持数
        /// </summary>
        private int _maxEntryCount = 5;
        
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
        /// PLC読み出し用タイマー
        /// ※PLC読み出しを画面更新タイマーでやると固まるので
        /// </summary>
        private System.Threading.Timer _tmrPlcRead = null;
        /// <summary>
        /// タイマー重複ロック
        /// </summary>
        private bool _timeLockPlc = false;


        /// <summary>
        /// PLC設備ステータス
        /// </summary>
        PLCMachineStatus _machineStatus = null;
        /// <summary>
        /// PLC仕分ステータス
        /// </summary>
        List<PLCWorkOrder> _allOrderStatus = null;


        /// <summary>
        /// バッチNoコンボボックス バインド用リスト
        /// </summary>
        private ObservableCollection<int> _batchNo = new ObservableCollection<int>();
        /// <summary>
        /// エントリーワーク情報リスト
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindRegistWork> _entryWorkList = new ObservableCollection<BindRegistWork>();
        /// <summary>
        /// 画面更新中フラグ
        /// </summary>
        private bool _updating = false; 



        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ucAisle()
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

                _machineStatus = new PLCMachineStatus();
                _allOrderStatus = new List<PLCWorkOrder>();
                for (int i = 0; i < Const.MaxWorkRegisterCount; i++)
                {
                    PLCWorkOrder work = new PLCWorkOrder();
                    _allOrderStatus.Add(work);
                }

                // ListView初期化
                rc = InitListView();

                // ユーザーコントロールに入力
                ucAisleTopView.aisleIndex = AisleIndex;
                //ucUnitView01.AisleIndex = AisleIndex;
                //ucUnitView01.UnitIndex = 0;
                //ucUnitView02.AisleIndex = AisleIndex;
                //ucUnitView02.UnitIndex = 1;
                //ucUnitView03.AisleIndex = AisleIndex;
                //ucUnitView03.UnitIndex = 2;

                //// バッチNoコンボボックス
                //int batchCount = Resource.batch.BatchInfoCurrent.Post[0].Aisle[AisleIndex].Batch.Count;
                //for(int i = 0; i < batchCount; i++) _batchNo.Add(i);
                //comboBatchNo.ItemsSource = _batchNo;
                //comboBatchNo.SelectedIndex = 0;



                // タイマー初期化
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

                if (_tmrPlcRead != null)
                    _tmrPlcRead.Change(Timeout.Infinite, Timeout.Infinite);
                _tmrPlcRead = null;
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, false);
            }
        }

        /// <summary>
        /// 便選択ボタン クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPost_Click(object sender, RoutedEventArgs e)
        {
            UInt32 rc = 0;
            Button ctrl = (Button)sender;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {ctrl.Name}");
            try
            {
                //if (ctrl == btnPost01)
                //{
                //    Resource.SystemStatus.CurrentPostIndex[AisleIndex] = 0;
                //    _batchNo.Clear();
                //    int batchCount = Resource.batch.BatchInfoCurrent.Post[Resource.SystemStatus.CurrentPostIndex[AisleIndex]].Aisle[AisleIndex].Batch.Count;
                //    for (int i = 0; i < batchCount; i++) _batchNo.Add(i);
                //    comboBatchNo.SelectedIndex = 0;
                //}
                //else if (ctrl == btnPost02)
                //{
                //    Resource.SystemStatus.CurrentPostIndex[AisleIndex] = 1;
                //    _batchNo.Clear();
                //    int batchCount = Resource.batch.BatchInfoCurrent.Post[Resource.SystemStatus.CurrentPostIndex[AisleIndex]].Aisle[AisleIndex].Batch.Count;
                //    for (int i = 0; i < batchCount; i++) _batchNo.Add(i);
                //    comboBatchNo.SelectedIndex = 0;
                //}
                //else if (ctrl == btnPost03)
                //{
                //    Resource.SystemStatus.CurrentPostIndex[AisleIndex] = 2;
                //    _batchNo.Clear();
                //    int batchCount = Resource.batch.BatchInfoCurrent.Post[Resource.SystemStatus.CurrentPostIndex[AisleIndex]].Aisle[AisleIndex].Batch.Count;
                //    for (int i = 0; i < batchCount; i++) _batchNo.Add(i);
                //    comboBatchNo.SelectedIndex = 0;
                //}
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                //Resource.ErrorHandler(ex, true);
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
                // 表へのデータバインド処理
                // 空のデータを5つ作っておいて、常に画面の表が5行になるようにしておく。
                for (int i = 0; i < _maxEntryCount; i++)
                {
                    _entryWorkList.Add(new BindRegistWork
                    {
                        //No = i + 1,
                        IsVisible = false,
                        WorkName = "",
                        JANCode = "",
                        OrderCount = 0,
                        OrderCompCount = 0,
                        OrderRemainCount = 0,
                        OrderProgress = 0,
                        Status = "",
                    });
                }
                listviewEntryWorkInfo.ItemsSource = _entryWorkList;

                // 表の各カラムのWidthを調整
                double w = listviewEntryWorkInfo.ActualWidth;
                columnWorkName.Width = Math.Floor(w * 0.30);
                columnJanCode.Width = Math.Floor(w * 0.16);
                columnOrderCount.Width = Math.Floor(w * 0.08);
                columnOrderCompCount.Width = Math.Floor(w * 0.08);
                columnOrderRemainCount.Width = Math.Floor(w * 0.04);
                columnOrderProgress.Width = Math.Floor(w * 0.18);
                columnStatus.Width = Math.Floor(w * 0.14);

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
                _tmrUpdateDisplay.Interval = TimeSpan.FromMilliseconds(200);
                // タイマメソッドを設定
                _tmrUpdateDisplay.Tick += new EventHandler(tmrUpdateDisplay_tick);
                // タイマを開始
                _tmrUpdateDisplay.Start();

                // タイマのインスタンス生成、タイマースタート
                _tmrPlcRead = new System.Threading.Timer(new TimerCallback(tmrReadPlc_tick), null, 1000, 400);
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
            try
            {
                // 重複防止
                if (_timeLock)
                    return;
                _timeLock = true;

                // PLCを読み出してなかったら終了
                if (_machineStatus == null || _allOrderStatus == null)
                    return;
                
                // 画面更新中
                _updating = true;

                //Logger.WriteLog(LogType.DEBUG, "★★★★★    画面更新");

                // アイルNo
                lblAisleNo.Content = $"アイル {AisleIndex + 1}";

                // アイル背景色
                if (IniFile.AisleEnable[AisleIndex] == false) 
                {// 無効
                    bdWhole.Background = System.Windows.Media.Brushes.Silver;
                    return;
                }
                else 
                {// 有効
                    if (!_machineStatus.AisleAutoRunning)
                    {// アイル停止中
                        bdWhole.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B0D0FF"));
                        //bdWhole.Background = Brushes.CornflowerBlue;
                    }
                    else
                    {// アイル運転
                        //bdWhole.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#d0ffd0"));
                        bdWhole.Background = System.Windows.Media.Brushes.PaleGreen;
                    }
                }

                // 運転/停止ラベル
                if (_machineStatus.AisleAutoRunning)
                {
                    lblAutoStatus.Content = "運転中";
                    lblAutoStatus.Background = System.Windows.Media.Brushes.SpringGreen;
                }
                else 
                {
                    lblAutoStatus.Content = "停止中";
                    lblAutoStatus.Background = System.Windows.Media.Brushes.Khaki;
                }

                //// 便Noラジオボタン 背景色
                //for (int i = 0; i < Const.MaxPostCount; i++) 
                //{
                //    string ctrlStr = $"btnPost0{i + 1}";
                //    Button r = (Button)FindName(ctrlStr);

                //    if (Resource.SystemStatus.CurrentPostIndex[AisleIndex] == i)
                //    {
                //        if (Resource.SystemStatus.CycleStatus[AisleIndex] == CYCLE_STATUS.WAITING)
                //        {
                //            // 開始待ち
                //            r.Background = System.Windows.Media.Brushes.MediumSpringGreen;
                //        }
                //        else if (Resource.SystemStatus.CycleStatus[AisleIndex] == CYCLE_STATUS.PICKING)
                //        {
                //            // 仕分中
                //            r.Background = System.Windows.Media.Brushes.Yellow;
                //        }
                //        else if (Resource.SystemStatus.CycleStatus[AisleIndex] == CYCLE_STATUS.COMP)
                //        {
                //            // 便完了
                //            r.Background = System.Windows.Media.Brushes.RoyalBlue;
                //        }
                //        else 
                //        {
                //            r.Background = System.Windows.Media.Brushes.Snow;
                //        }
                //    }
                //    else
                //    {
                //        r.Background = System.Windows.Media.Brushes.Gray;
                //    }
                //}




                //Resource.SystemStatus.CurrentBatchIndex[AisleIndex] = comboBatchNo.SelectedIndex;




                // ----------------------------------
                // 仕分登録商品ListView
                // ----------------------------------
                int count = 0;

                // まず仕分中の商品を取得
                List<PLCWorkOrder> workStartOrder = _allOrderStatus.Where(x => x.sequence == PLC_ORDER_SEQUENCE.START_ORDER)
                                                                  .OrderBy(x => x.registryNo)
                                                                  .ToList();
                if (workStartOrder != null) 
                {
                    for (int i = 0; i < workStartOrder.Count; i++)
                    {
                        _entryWorkList[i].WorkName = workStartOrder[i].workName;
                        _entryWorkList[i].JANCode = workStartOrder[i].JANCode;
                        _entryWorkList[i].OrderCount = workStartOrder[i].orderCountTotal;
                        _entryWorkList[i].OrderCompCount = workStartOrder[i].orderCountCompTotal;
                        _entryWorkList[i].OrderRemainCount = workStartOrder[i].orderCountTotal - workStartOrder[i].orderCountCompTotal;
                        if (workStartOrder[i].orderCountTotal != 0)
                            _entryWorkList[i].OrderProgress = (double)((int)((workStartOrder[i].orderCountCompTotal * 100.0) / ((double)workStartOrder[i].orderCountTotal)));

                        string s = "";
                        if (workStartOrder[i].sequence == PLC_ORDER_SEQUENCE.START_ORDER) 
                            s = "仕分中";
                        else if (workStartOrder[i].sequence == PLC_ORDER_SEQUENCE.REQ_REGISTER)
                            s = "仕分情報登録待ち";
                        else if (workStartOrder[i].sequence == PLC_ORDER_SEQUENCE.COMP_REGISTER)
                            s = "仕分開始待ち";
                        else if (workStartOrder[i].sequence == PLC_ORDER_SEQUENCE.CANCEL_ORDER)
                            s = "仕分キャンセル";
                        else if(workStartOrder[i].sequence == PLC_ORDER_SEQUENCE.COMP_REGISTER)
                            s = "仕分完了";
                        _entryWorkList[i].Status = s;

                        _entryWorkList[i].IsVisible = true;
                        //ChangeRowColor(i, System.Windows.Media.Brushes.PaleGreen);
                        //ChangeRowColor(i, (SolidColorBrush)(new BrushConverter().ConvertFrom("#70FF70")));
                        ChangeRowColor(i, System.Windows.Media.Brushes.MediumSpringGreen);

                        count++;
                    }
                }

                // 仕分中ではない商品を取得
                List<PLCWorkOrder> workOther = _allOrderStatus.Where(x => x.sequence != PLC_ORDER_SEQUENCE.START_ORDER && x.sequence != PLC_ORDER_SEQUENCE.NOT_REGISTER)
                                                                  .OrderBy(x => x.registryNo)
                                                                  .ToList();
                if (workOther != null)
                {
                    for (int i = 0; i < workOther.Count; i++)
                    {
                        _entryWorkList[count].WorkName = workOther[i].workName;
                        _entryWorkList[count].JANCode = workOther[i].JANCode;
                        _entryWorkList[count].OrderCount = workOther[i].orderCountTotal;
                        _entryWorkList[count].OrderCompCount = workOther[i].orderCountCompTotal;
                        _entryWorkList[count].OrderRemainCount = workOther[i].orderCountTotal - workOther[i].orderCountCompTotal;
                        if (workOther[i].orderCountTotal != 0)
                            _entryWorkList[count].OrderProgress = (workOther[i].orderCountCompTotal * 100) / workOther[i].orderCountTotal;

                        string s = "";
                        if (workOther[i].sequence == PLC_ORDER_SEQUENCE.START_ORDER)
                            s = "仕分中";
                        else if (workOther[i].sequence == PLC_ORDER_SEQUENCE.REQ_REGISTER)
                            s = "仕分情報登録待ち";
                        else if (workOther[i].sequence == PLC_ORDER_SEQUENCE.COMP_REGISTER)
                            s = "仕分開始待ち";
                        else if (workOther[i].sequence == PLC_ORDER_SEQUENCE.CANCEL_ORDER)
                            s = "仕分キャンセル";
                        else if (workOther[i].sequence == PLC_ORDER_SEQUENCE.COMP_ORDER)
                            s = "仕分完了";
                        _entryWorkList[count].Status = s;

                        _entryWorkList[count].IsVisible = true;
                        ChangeRowColor(count, System.Windows.Media.Brushes.DarkSeaGreen);

                        count++;
                        if (count >= 5)
                            break;
                    }
                }

                // 残りの欄はクリア
                for (int i = count; i < 5; i++) 
                {
                    _entryWorkList[i].WorkName = "";
                    _entryWorkList[i].JANCode = "";
                    _entryWorkList[i].OrderCount = 0;
                    _entryWorkList[i].OrderCompCount = 0;
                    _entryWorkList[i].OrderRemainCount = 0;
                    _entryWorkList[i].OrderProgress = 0;
                    _entryWorkList[i].Status = "";
                    _entryWorkList[i].IsVisible = false;
                    ChangeRowColor(i, System.Windows.Media.Brushes.Gray);
                }

            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
            }
            finally
            {
                _updating = false;
                _timeLock = false;
            }
        }

        /// <summary>
        /// 画面更新タイマー トリガー
        /// </summary>
        private void tmrReadPlc_tick(object sender)
        {
            ReadPlc();
        }
        /// <summary>
        /// PLC読み出し
        /// </summary>
        private void ReadPlc()
        {
            UInt32 rc = 0;

            // 重複防止
            if (_timeLockPlc)
                return;
            _timeLockPlc = true;

            try
            {
                //Logger.WriteLog(LogType.DEBUG, "★★★★★    PLC読出");

                // PLC
                if (!_updating) 
                {
                    if (IniFile.AisleEnable[AisleIndex]) 
                    {
                        // PLC 設備ステータス読み出し
                        rc = Resource.Plc[AisleIndex].Access.GetMachineStatus(out PLCMachineStatus machineStatus);
                        _machineStatus.Read(machineStatus);

                        // PLC 仕分ステータス読み出し
                        for (int i = 0; i < Const.MaxWorkRegisterCount; i++)
                        {
                            rc = Resource.Plc[AisleIndex].Access.GetOrderStatus(i, out PLCWorkOrder orderStatus);
                            _allOrderStatus[i].registryNo = orderStatus.registryNo;
                            _allOrderStatus[i].orderDate = orderStatus.orderDate;
                            _allOrderStatus[i].postNo = orderStatus.postNo;
                            _allOrderStatus[i].orderCountTotal = orderStatus.orderCountTotal;
                            _allOrderStatus[i].orderCountInputTotal = orderStatus.orderCountInputTotal;
                            _allOrderStatus[i].orderCountCompTotal = orderStatus.orderCountCompTotal;
                            _allOrderStatus[i].sequence = orderStatus.sequence;
                            _allOrderStatus[i].JANCode = orderStatus.JANCode;
                            _allOrderStatus[i].workName = orderStatus.workName;
                            for (int unit = 0; unit < Const.MaxUnitCount; unit++)
                            {
                                for (int slot = 0; slot < Const.MaxSlotCount; slot++)
                                {
                                    _allOrderStatus[i].orderCount[unit * Const.MaxSlotCount + slot] = orderStatus.orderCount[unit * Const.MaxSlotCount + slot];
                                    _allOrderStatus[i].orderCountComp[unit * Const.MaxSlotCount + slot] = orderStatus.orderCountComp[unit * Const.MaxSlotCount + slot];
                                }
                            }
                            for (int slot = 0; slot < 999; slot++)
                            {
                                _allOrderStatus[i].targetSlot[slot] = orderStatus.targetSlot[slot];
                            }
                        }
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
                _timeLockPlc = false;
            }
        }



        /// <summary>
        /// 表の指定行の色を変更
        /// </summary>
        private UInt32 ChangeRowColor(int rowIndex, SolidColorBrush color)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                if (listviewEntryWorkInfo.Items.Count > 0)
                {
                    ListViewItem row = (ListViewItem)listviewEntryWorkInfo.ItemContainerGenerator.ContainerFromIndex(rowIndex);
                    if (row != null)
                    {
                        row.Background = color;
                    }
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
        /// Check Error State
        /// </summary>
        private static bool STATUS_SUCCESS(UInt32 err)
        {
            return err == (int)ErrorCodeList.STATUS_SUCCESS;
        }
    }


    /// <summary>
    /// 仕分登録ワーク情報
    /// 画面の表にデータバインドするようのクラス
    /// </summary>
    public class BindRegistWork : INotifyPropertyChanged
    {
        private bool _isVisible;
        /// <summary>
        /// データ行の表示非表示
        /// ※今回はListViewを5行固定で表示させるため、空のデータ行は非表示にしたい。
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged(nameof(IsVisible));
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
        
        private double _orderProgress;
        /// <summary>
        /// 進捗 (％)
        /// </summary>
        public double OrderProgress
        {
            get { return _orderProgress; }
            set
            {
                if (_orderProgress != value)
                {
                    _orderProgress = value;
                    OnPropertyChanged(nameof(OrderProgress));
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
