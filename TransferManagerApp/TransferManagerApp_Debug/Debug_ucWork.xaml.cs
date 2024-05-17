//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading;

using PLCModule;
using ServerModule;
using ShareResource;
using SystemConfig;
using DL_Logger;
using ErrorCodeDefine;


namespace TransferManagerApp_Debug
{
    /// <summary>
    /// ucOrderInfo.xaml の相互作用ロジック
    /// </summary>
    public partial class Debug_ucWork : UserControl, IDisposable
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "ucOrderInfo";

        /// <summary>
        /// アイルIndex
        /// </summary>
        public int aisleIndex = 0;
        /// <summary>
        /// ワークIndex 1~10
        /// </summary>
        public int workIndex { get; set; }
        /// <summary>
        /// 商品投入フラグ
        /// </summary>
        public bool isInput = false;

        #region 仕分情報
        /// <summary>
        /// 仕分登録No
        /// </summary>
        public int registNo = 0; 
        /// <summary>
        /// 仕分け実行フラグ
        /// </summary>
        public PLC_ORDER_SEQUENCE sequence = PLC_ORDER_SEQUENCE.NOT_REGISTER;
        /// <summary>
        /// 現在ワーク情報
        /// </summary>
        private OrderData _work = new OrderData();
        #endregion

        /// <summary>
        /// 画面更新タイマー
        /// </summary>
        private DispatcherTimer _tmrUpdateDisplay = null;
        /// <summary>
        /// タイマー重複ロック
        /// </summary>
        private bool _timeLock = false;

        /// <summary>
        /// PLC仕分シミュレータースレッド
        /// </summary>
        private Thread _threadPlcOrderSimulator = null;
        /// <summary>
        /// スレッド終了フラグ
        /// </summary>
        private bool _shutdown = false;

        /// <summary>
        /// 仕分け作業ストップウォッチ
        /// </summary>
        private Stopwatch _sw = new Stopwatch();
        /// <summary>
        /// ストップウォッチ待機時間(ms)
        /// </summary>
        private int _waitTime = 300;

        /// <summary>
        /// 仕分開始 準備完了
        /// </summary>
        public bool startOrderReady = false;


        /// <summary>
        /// 店別仕分数リスト
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindOrderCount> _orderCountList = new ObservableCollection<BindOrderCount>();
        /// <summary>
        /// 店別仕分済数リスト
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindOrderCompCount> _orderCompCountList = new ObservableCollection<BindOrderCompCount>();
        /// <summary>
        /// 取出対象スロット
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindTargetSlot> _targetSlotList = new ObservableCollection<BindTargetSlot>();


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Debug_ucWork()
        {
            InitializeComponent();
        }
        /// <summary>
        /// フォームロード
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

                // 画面更新タイマー初期化
                rc = InitTimer();

                // PLC仕分スレッド起動
                _threadPlcOrderSimulator = new Thread(Thread_PlcOrderSimulator);
                _threadPlcOrderSimulator.Start();

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
                // PLCシミュレータースレッド終了
                _shutdown = true;

                if (_tmrUpdateDisplay != null)
                    _tmrUpdateDisplay.Stop();
                _tmrUpdateDisplay = null;

                if(_sw != null)
                    _sw.Stop();
                _sw = null;
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, false);
            }
        }
        /// <summary>
        /// 仕分キャンセルボタン クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOrderCancel_Click(object sender, RoutedEventArgs e)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                rc = Resource.Plc[aisleIndex].Access.SetOrderSequence(workIndex, PLC_ORDER_SEQUENCE.CANCEL_ORDER);
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
        }

        ///// <summary>
        ///// ボタン クリックイベント
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void btnReadBarcode_Click(object sender, RoutedEventArgs e)
        //{
        //    UInt32 rc = 0;
        //    Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
        //    try
        //    {
        //        int registNo = 0;
        //        int postIndex = 0;
        //        rc = Resource.Plc[aisleIndex].Access.Debug_SetRegistNo(workIndex, registNo);
        //        rc = Resource.Plc[aisleIndex].Access.Debug_SetWorkName(workIndex,
        //                                                               Resource.Server.OrderInfo.OrderDataList[postIndex][0].workNameKana,
        //                                                               Resource.Server.OrderInfo.OrderDataList[postIndex][0].workCode);
        //        //rc = Resource.Plc[aisleIndex].Access.Debug_SetWorkName(workIndex,
        //        //                                       "ｱｱｱｱｱ",
        //        //                                       "111111");
        //        _sequence = PLC_ORDER_SEQUENCE.REQ_REGISTER;
        //    }
        //    catch (Exception ex)
        //    {
        //        rc = (Int32)ErrorCodeList.EXCEPTION;
        //        Resource.ErrorHandler(ex, true);
        //    }
        //}

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
                _orderCountList.Clear();
                for (int i = 0; i < Const.MaxUnitCount * Const.MaxSlotCount; i++) 
                {
                    BindOrderCount order = new BindOrderCount()
                    {
                        SlotNo = (i + 1).ToString(),
                        OrderCount = "0"
                    };
                    _orderCountList.Add(order);
                }
                listviewOrderCount.ItemsSource = _orderCountList;

                _orderCompCountList.Clear();
                for (int i = 0; i < Const.MaxUnitCount * Const.MaxSlotCount; i++)
                {
                    BindOrderCompCount orderComp = new BindOrderCompCount()
                    {
                        SlotNo = (i + 1).ToString(),
                        OrderCompCount = "0"
                    };
                    _orderCompCountList.Add(orderComp);
                }
                listviewOrderCompCount.ItemsSource = _orderCompCountList;

                _targetSlotList.Clear();
                for (int i = 0; i < 999; i++)
                {
                    BindTargetSlot targetSlot = new BindTargetSlot()
                    {
                        No = (i + 1).ToString(),
                        TargetSlot = "0"
                    };
                    _targetSlotList.Add(targetSlot);
                }
                listviewTargetSlotNo.ItemsSource = _targetSlotList;

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





                // ---------------------------------------------
                // PLC 設備ステータス読み出し
                rc = Resource.Plc[aisleIndex].Access.GetMachineStatus(out PLCMachineStatus status);
                // ---------------------------------------------
                // PLC 仕分ステータス読み出し
                rc = Resource.Plc[aisleIndex].Access.GetOrderStatus(workIndex, out PLCWorkOrder orderStatus);


                int b = 0;
                if (workIndex == 0)
                    b = 1;

                // 停止中なら終了
                if (!status.AisleAutoRunning)
                    return;


                int a = 0;
                if (aisleIndex == 0)
                    a = 0;


                // ------------------------------
                // 画面表示更新
                // ------------------------------
                lblRegistNo.Content = orderStatus.registryNo;
                lblOrderDate.Content = $"{orderStatus.orderDate.Year.ToString("D4")}{orderStatus.orderDate.Month.ToString("D2")}{orderStatus.orderDate.Day.ToString("D2")}";
                lblPostNo.Content = orderStatus.postNo;
                lblOrderCountTotal.Content = orderStatus.orderCountTotal;
                lblOrderCompTotal.Content = orderStatus.orderCountCompTotal;
                lblWorkName.Content = orderStatus.workName;
                lblJANCode.Content = orderStatus.JANCode;
                for (int i = 0; i < _orderCountList.Count; i++)
                {
                    _orderCountList[i].OrderCount = orderStatus.orderCount[i].ToString();
                    _orderCompCountList[i].OrderCompCount = orderStatus.orderCountComp[i].ToString();
                }
                for (int i = 0; i < _targetSlotList.Count; i++)
                {
                    _targetSlotList[i].TargetSlot = orderStatus.targetSlot[i].ToString();
                }

                sequence = orderStatus.sequence;
                if (sequence == PLC_ORDER_SEQUENCE.NOT_REGISTER)
                {
                    // 未登録
                    lblExecuteFlg_NoRegist.Background = Brushes.MediumAquamarine;
                    lblExecuteFlg_ReqRegist.Background = Brushes.Gray;
                    lblExecuteFlg_CompResist.Background = Brushes.Gray;
                    lblExecuteFlg_StartOrder.Background = Brushes.Gray;
                    lblExecuteFlg_CancelOrder.Background = Brushes.Gray;
                    lblExecuteFlg_CompOrder.Background = Brushes.Gray;
                }
                else if (sequence == PLC_ORDER_SEQUENCE.REQ_REGISTER)
                {
                    // 登録要求
                    lblExecuteFlg_NoRegist.Background = Brushes.Gray;
                    lblExecuteFlg_ReqRegist.Background = Brushes.MediumAquamarine;
                    lblExecuteFlg_CompResist.Background = Brushes.Gray;
                    lblExecuteFlg_StartOrder.Background = Brushes.Gray;
                    lblExecuteFlg_CancelOrder.Background = Brushes.Gray;
                    lblExecuteFlg_CompOrder.Background = Brushes.Gray;
                }
                else if (sequence == PLC_ORDER_SEQUENCE.COMP_REGISTER)
                {
                    // 登録完了
                    lblExecuteFlg_NoRegist.Background = Brushes.Gray;
                    lblExecuteFlg_ReqRegist.Background = Brushes.Gray;
                    lblExecuteFlg_CompResist.Background = Brushes.MediumAquamarine;
                    lblExecuteFlg_StartOrder.Background = Brushes.Gray;
                    lblExecuteFlg_CancelOrder.Background = Brushes.Gray;
                    lblExecuteFlg_CompOrder.Background = Brushes.Gray;
                }
                else if (sequence == PLC_ORDER_SEQUENCE.START_ORDER)
                {
                    // 仕分開始
                    lblExecuteFlg_NoRegist.Background = Brushes.Gray;
                    lblExecuteFlg_ReqRegist.Background = Brushes.Gray;
                    lblExecuteFlg_CompResist.Background = Brushes.Gray;
                    lblExecuteFlg_StartOrder.Background = Brushes.MediumAquamarine;
                    lblExecuteFlg_CancelOrder.Background = Brushes.Gray;
                    lblExecuteFlg_CompOrder.Background = Brushes.Gray;
                }
                else if (sequence == PLC_ORDER_SEQUENCE.CANCEL_ORDER)
                {
                    // 仕分キャンセル
                    lblExecuteFlg_NoRegist.Background = Brushes.Gray;
                    lblExecuteFlg_ReqRegist.Background = Brushes.Gray;
                    lblExecuteFlg_CompResist.Background = Brushes.Gray;
                    lblExecuteFlg_StartOrder.Background = Brushes.Gray;
                    lblExecuteFlg_CancelOrder.Background = Brushes.MediumAquamarine;
                    lblExecuteFlg_CompOrder.Background = Brushes.Gray;
                }
                else if (sequence == PLC_ORDER_SEQUENCE.COMP_ORDER)
                {
                    // 仕分完了
                    lblExecuteFlg_NoRegist.Background = Brushes.Gray;
                    lblExecuteFlg_ReqRegist.Background = Brushes.Gray;
                    lblExecuteFlg_CompResist.Background = Brushes.Gray;
                    lblExecuteFlg_StartOrder.Background = Brushes.Gray;
                    lblExecuteFlg_CancelOrder.Background = Brushes.Gray;
                    lblExecuteFlg_CompOrder.Background = Brushes.MediumAquamarine;
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
        /// スレッド
        /// PLC仕分シミュレーター
        /// </summary>
        /// <returns></returns>
        private void Thread_PlcOrderSimulator()
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                bool init = true;
                Stopwatch sw_ToStartOrder = new Stopwatch();
                Stopwatch sw_ToNoRegister = new Stopwatch();
                Stopwatch sw_OrderCountIncrement = new Stopwatch();

                while (true)
                {
                    Thread.Sleep(200);

                    if (_shutdown)
                        break;

                    if (init)
                    {// 初期化
                        init = false;


                    }


                    // ---------------------------------------------
                    // PLC 設備ステータス読み出し
                    rc = Resource.Plc[aisleIndex].Access.GetMachineStatus(out PLCMachineStatus status);
                    // ---------------------------------------------
                    // PLC 仕分ステータス読み出し
                    rc = Resource.Plc[aisleIndex].Access.GetOrderStatus(workIndex, out PLCWorkOrder orderStatus);


                    // 商品投入開始かどうか
                    if (!status.AisleAutoRunning)
                        continue;

                    // ------------------------------
                    // 仕分実行フラグごとの処理
                    // ------------------------------
                    if (sequence == PLC_ORDER_SEQUENCE.NOT_REGISTER)
                    {// 未登録

                    }
                    else if (sequence == PLC_ORDER_SEQUENCE.REQ_REGISTER)
                    {// 登録要求

                    }
                    else if (sequence == PLC_ORDER_SEQUENCE.COMP_REGISTER)
                    {// 登録完了

                        // 設置対象スロット情報
                        ///とりあえず1ユニットのみ。左右のロボットが交互に動作するようなスロット順にしたい。あとﾊﾝﾄﾞ交換とかもあるよ。
                        int[] putSlotList = new int[orderStatus.orderCountTotal];
                        int cnt = 0;
                        // スロット1から順番に仕分け数分設置させていくやり方
                        for (int unitIndex = 0; unitIndex < Const.MaxUnitCount; unitIndex++)
                        {
                            if (IniFile.UnitEnable[aisleIndex][unitIndex])
                            {
                                for (int slotIndex = unitIndex * 12; slotIndex < (unitIndex + 1) * 12; slotIndex++)
                                {
                                    for (int i = 0; i < orderStatus.orderCount[slotIndex]; i++)
                                        putSlotList[cnt++] = slotIndex + 1;
                                }
                            }
                        }
                        rc = Resource.Plc[aisleIndex].Access.Debug_SetOrderTargetSlot(workIndex, putSlotList);


                        // 仕分開始へ
                        if(!sw_ToStartOrder.IsRunning)
                            sw_ToStartOrder.Start();
                        if (sw_ToStartOrder.ElapsedMilliseconds > 2000) 
                        {
                            // 仕分開始 準備完了
                            startOrderReady = true;


                            //// 左右どっちか空いてる方の作業中仕分登録Noに登録する
                            //if (status.CurrentRegistNo_L == 0 || status.CurrentRegistNo_R == 0) 
                            //{
                            //    if(status.CurrentRegistNo_L == 0) rc = Resource.Plc[aisleIndex].Access.Debug_SetCurrentRegistNoL(orderStatus.registryNo);
                            //    else if (status.CurrentRegistNo_R == 0) rc = Resource.Plc[aisleIndex].Access.Debug_SetCurrentRegistNoR(orderStatus.registryNo);

                            //    sw_ToStartOrder.Reset();
                            //    rc = Resource.Plc[aisleIndex].Access.SetOrderSequence(workIndex, PLC_ORDER_SEQUENCE.START_ORDER);
                            //}
                        }
                    }
                    else if (sequence == PLC_ORDER_SEQUENCE.START_ORDER)
                    {// 仕分開始

                        if (!sw_OrderCountIncrement.IsRunning)
                            sw_OrderCountIncrement.Start();

                        if (sw_OrderCountIncrement.ElapsedMilliseconds > 1000) 
                        {
                            sw_OrderCountIncrement.Restart();
                            if (orderStatus.orderCountCompTotal < orderStatus.orderCountTotal)
                            {
                                int totalCompCount = orderStatus.orderCountCompTotal;
                                int targetSlotNo = orderStatus.targetSlot[totalCompCount];
                                int targetSlotCompCount = orderStatus.orderCountComp[targetSlotNo - 1];
                                Resource.Plc[aisleIndex].Access.Debug_SetOrderCompCountTotal(workIndex, totalCompCount + 1);
                                Resource.Plc[aisleIndex].Access.Debug_SetOrderCompCount(workIndex, targetSlotNo, targetSlotCompCount + 1);
                                //Logger.WriteLog(LogType.ERROR, $"★★★{targetSlotNo} {targetSlotCompCount + 1}");
                            }
                            else
                            {
                                //Logger.WriteLog(LogType.ERROR, $"★★★comp");
                                sw_OrderCountIncrement.Reset();
                                rc = Resource.Plc[aisleIndex].Access.SetOrderSequence(workIndex, PLC_ORDER_SEQUENCE.COMP_ORDER);
                            }
                        }

                    }
                    else if (sequence == PLC_ORDER_SEQUENCE.CANCEL_ORDER)
                    {// 仕分キャンセル



                    }
                    else if (sequence == PLC_ORDER_SEQUENCE.COMP_ORDER)
                    {// 仕分完了


                        //// 未登録へ
                        //if (!sw_ToNoRegister.IsRunning)
                        //    sw_ToNoRegister.Start();
                        //if (sw_ToNoRegister.ElapsedMilliseconds > 10000)
                        //{
                        //    sw_ToNoRegister.Reset();
                        //    rc = Resource.Plc[aisleIndex].Access.SetOrderSequence(workIndex, PLC_ORDER_SEQUENCE.NOT_REGISTER);

                        //    // 仕分情報をクリア
                        //    rc = Resource.Plc[aisleIndex].Access.Debug_SetRegistNo(workIndex, 0);
                        //    rc = Resource.Plc[aisleIndex].Access.Debug_SetWorkName(workIndex, "", "");
                        //    rc = Resource.Plc[aisleIndex].Access.SetOrderCountTotal(workIndex, 0);
                        //    int[] orderCount = new int[Const.MaxUnitCount * Const.MaxSlotCount];
                        //    rc = Resource.Plc[aisleIndex].Access.SetOrderCountSlot(workIndex, orderCount);
                        //    Resource.Plc[aisleIndex].Access.Debug_SetOrderCompCountTotal(workIndex, 0);
                        //    for (int i = 0; i < Const.MaxUnitCount * Const.MaxSlotCount; i++)
                        //        rc = Resource.Plc[aisleIndex].Access.Debug_SetOrderCompCount(workIndex, i + 1, 0);
                        //    int[] slotNo = new int[1000];
                        //    rc = Resource.Plc[aisleIndex].Access.Debug_SetOrderTargetSlot(workIndex, slotNo);
                        //}

                    }

                }

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return;
        }


        /// <summary>
        /// 商品登録
        /// 商品バーコードが読み込まれた際の開始処理
        /// </summary>
        /// <param name="work"></param>
        /// <param name="workRegistNo"></param>
        /// <returns></returns>
        public UInt32 SetWork(OrderData work, int workRegistNo) 
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                _work = null;
                _work = new OrderData();
                _work = work;

                // 商品情報を書き込み
                rc = Resource.Plc[aisleIndex].Access.Debug_SetRegistNo(workIndex, workRegistNo);
                registNo = workRegistNo;
                rc = Resource.Plc[aisleIndex].Access.Debug_SetWorkName(workIndex, _work.workNameKana, _work.JANCode);

                // 登録要求へ
                Resource.Plc[aisleIndex].Access.SetOrderSequence(workIndex, PLC_ORDER_SEQUENCE.REQ_REGISTER);
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }


        ///// <summary>
        ///// ストップウォッチ待機
        ///// 指定時間経過したらtrueを返す
        ///// </summary>
        ///// <returns></returns>
        //private bool WaitSW() 
        //{
        //    bool ok = false;
        //    try
        //    {
        //        if (!_sw.IsRunning)
        //            _sw.Start();

        //        if (_sw.ElapsedMilliseconds > _waitTime) 
        //        {
        //            ok = true;
        //            _sw.Restart();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Resource.ErrorHandler(ex);
        //    }
        //    return ok;
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
    public class BindOrderCount : INotifyPropertyChanged
    {
        private string _slotNo;
        /// <summary>
        /// SlotNo
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
        /// 仕分け数量
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


        // ListViewへバインディングしたデータの値が変更されたときに、
        // ListViewに変更を通知して反映させる仕組み。
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// エントリーワーク情報
    /// 画面の表にデータバインドするようのクラス
    /// </summary>
    public class BindOrderCompCount : INotifyPropertyChanged
    {
        private string _slotNo;
        /// <summary>
        /// SlotNo
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

        private string _orderCompCount;
        /// <summary>
        /// 仕分け数量
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

    /// <summary>
    /// エントリーワーク情報
    /// 画面の表にデータバインドするようのクラス
    /// </summary>
    public class BindTargetSlot : INotifyPropertyChanged
    {
        private string _No;
        /// <summary>
        /// SlotNo
        /// </summary>
        public string No
        {
            get { return _No; }
            set
            {
                if (_No != value)
                {
                    _No = value;
                    OnPropertyChanged(nameof(No));
                }
            }
        }

        private string _targetSlot;
        /// <summary>
        /// 仕分け数量
        /// </summary>
        public string TargetSlot
        {
            get { return _targetSlot; }
            set
            {
                if (_targetSlot != value)
                {
                    _targetSlot = value;
                    OnPropertyChanged(nameof(TargetSlot));
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
