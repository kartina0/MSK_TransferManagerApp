//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using DL_Logger;
using ErrorCodeDefine;
using SystemConfig;
using ShareResource;
using PLCModule;
using ServerModule;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace TransferManagerApp_Debug
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class Debug_windowPlcSimulator : Window
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "Debug_windowPlcSimulator";

        /// <summary>
        /// アイルIndex
        /// </summary>
        private int _aisleIndex = 0;
        /// <summary>
        /// バッチIndex
        /// </summary>
        private int _batchIndex = 0;
        /// <summary>
        /// 読み込んだした商品数をカウント
        /// </summary>
        private int _workRegistCount = 0;
        /// <summary>
        /// ウィンドウ表示中フラグ
        /// </summary>
        public bool isShowing = false;

        /// <summary>
        /// 画面更新タイマー
        /// </summary>
        private DispatcherTimer _tmrUpdateDisplay = null;
        /// <summary>
        /// タイマー重複ロック
        /// </summary>
        private bool _timeLock = false;

        /// <summary>
        /// PLCシミュレータースレッド
        /// </summary>
        private Thread _threadPlcSimulator = null;
        /// <summary>
        /// スレッド終了フラグ
        /// </summary>
        private bool _shutdown = false;

        /// <summary>
        /// ユーザーコントロール配列
        /// </summary>
        private Debug_ucWork[] ucWorks = null;
        //private List<ucWork> ucWorkList = new List<ucWork>();

        /// <summary>
        /// 直近の仕分登録No L
        /// </summary>
        private int _latestRegistryNo_L = 0;
        /// <summary>
        /// 直近の仕分登録No R
        /// </summary>
        private int _latestRegistryNo_R = 0;



        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Debug_windowPlcSimulator()
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

                // ウィンドウ表示中
                isShowing = true;

                // タイトルバーを消しても画面移動可能にする処理
                this.MouseLeftButtonDown += delegate { DragMove(); };

                // ユーザーコントロール作成
                ucWorks = new Debug_ucWork[Const.MaxWorkRegisterCount];
                Thickness pad = new Thickness(0, 0, 0, 0);
                int index = 0;
                for (int row = 0; row < 2; row++) 
                {
                    for (int column = 0; column < 5; column++)
                    {
                        Debug_ucWork uc = new Debug_ucWork()
                        {
                            Padding = pad,
                            workIndex = index,
                            aisleIndex = _aisleIndex
                        };
                        uc.lblAisleNo.Content = $"ワーク0{index + 1}";
                        uc.SetValue(Grid.RowProperty, row);
                        uc.SetValue(Grid.ColumnProperty, column);
                        if (!gridWork.Children.Contains(uc))
                            gridWork.Children.Add(uc);

                        ucWorks[index] = uc;
                        index++;
                    }
                }

                // PLCデータメモリ全クリア
                rc = Resource.Plc[_aisleIndex].Access.Debug_Clear();

                // 画面更新タイマー初期化
                rc = InitTimer();

                // PLCスレッド起動
                _threadPlcSimulator = new Thread(Thread_PlcSimulator);
                _threadPlcSimulator.Start();

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
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // PLCシミュレータースレッド終了
                _shutdown = true;

                // 画面更新タイマー終了
                if (_tmrUpdateDisplay != null)
                    _tmrUpdateDisplay.Stop();
                _tmrUpdateDisplay = null;

                // ユーザーコントロールを閉じる
                foreach (Debug_ucWork uc in ucWorks)
                    uc.Dispose();

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
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                System.Windows.Controls.Button ctrl = (System.Windows.Controls.Button)sender;

                if (ctrl == btnAuto)
                {// 運転
                    rc = Resource.Plc[_aisleIndex].Access.Debug_SetAisleAutoRunning();
                }
                else if (ctrl == btnStop)
                {// 停止
                    rc = Resource.Plc[_aisleIndex].Access.Debug_SetAisleAutoStop();
                }
                else if (ctrl == btnReadWork)
                {// 商品読み込み

                    // 1~10のどのメモリに商品情報を書き込むか算出
                    int workAddrIndex = _workRegistCount % Const.MaxWorkRegisterCount;      // 1~10のどこの
                    // 仕分情報を取得
                    rc = Resource.Plc[_aisleIndex].Access.GetOrderStatus(workAddrIndex, out PLCWorkOrder orderStatus);
                    int postNo = orderStatus.postNo;


                    // 現在の便NO及びバッチNoに該当するワークを全て取得
                    List<OrderData> works = new List<OrderData>(Resource.Server.OrderInfo.OrderDataList[postNo - 1]);   // ※アイルNoはバッチファイル情報で勝手に選別されるはず
                    List<OrderData> worksOrder = new List<OrderData>();
                    // バッチ情報を取得
                    string[] currentbatchArray =
                                    Resource.batch.BatchInfoCurrent.Post[postNo - 1].Aisle[_aisleIndex].Batch[_batchIndex].OutputToArray(IniFile.UnitEnable[_aisleIndex]);
                    // 商品の仕分先の店コードがバッチ情報の店コードが含まれていれば、この商品は仕分対象
                    foreach (OrderData work in works)
                    {
                        foreach (string storeCode in currentbatchArray)
                        {
                            bool exist = work.storeDataList.Any(x => x.storeCode == storeCode);
                            if (exist)
                            {
                                worksOrder.Add(work);
                                break;
                            }
                        }
                    }

                    // 今回のワークをセット
                    if (_workRegistCount < worksOrder.Count)
                    {
                        ucWorks[workAddrIndex].SetWork(worksOrder[_workRegistCount++], _workRegistCount);
                    }
                    else
                    {
                        // 現在バッチ完了

                    }

                }
                else if (ctrl == btnReadWorkJan)
                {// 商品読み込み(JAN指定)

                    // 1~10のどのメモリに商品情報を書き込むか算出
                    int workAddrIndex = _workRegistCount % Const.MaxWorkRegisterCount;      // 1~10のどこの
                    // 仕分情報を取得
                    rc = Resource.Plc[_aisleIndex].Access.GetOrderStatus(workAddrIndex, out PLCWorkOrder orderStatus);
                    int postNo = orderStatus.postNo;

                    string text = txtJanCode.Text;
                    DateTime currentDateTime = DateTime.ParseExact($"{(DateTime.Now).Year.ToString("D4")}" +
                                                                  $"{(DateTime.Now).Month.ToString("D2")}" +
                                                                  $"{(DateTime.Now).Day.ToString("D2")}" +
                                                                  $"000000", "yyyyMMddHHmmss", null);
                    rc = Resource.Server.OrderInfo.SelectOrderData(currentDateTime, postNo - 1, text, out OrderData data);

                    ucWorks[workAddrIndex].SetWork(data, _workRegistCount + 1);

                }
                else if (ctrl == btnClose)
                {// 閉じる
                    this.Close();
                }
                else if (ctrl == btnAutoReady)
                {// PLC 設備ステータス読み出し

                    rc = Resource.Plc[_aisleIndex].Access.GetMachineStatus(out PLCMachineStatus status);
                    if (status.AisleAutoReady)
                    {// 可能
                        // 自動運転 不可
                        rc = Resource.Plc[_aisleIndex].Access.Debug_SetAisleAutoReady(false);
                    }
                    else 
                    {// 不可
                        // 自動運転 可能
                        rc = Resource.Plc[_aisleIndex].Access.Debug_SetAisleAutoReady(true);
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


                //int[] aaa = new int[5] { 1,0,2,0,3};
                //int aaaa1 = aaa.OrderBy(x => x).FirstOrDefault();
                //int aaaa2 = aaa.OrderBy(x => x > 0).FirstOrDefault();

                //int[] aaa = new int[5] { 1, 0, 2, 0, 3 };

                //// 0を除外し、昇順で要素を取得
                //int[] sortedValues = aaa.Where(x => x != 0).OrderBy(x => x).ToArray();


                //// 現在仕分中の商品以外は仕分開始しない
                //bool ok = ucWorks.Any(x => x.registNo != 0);
                //Debug_ucWork ucInputting = null;
                //if (ok)
                //{
                //    ucInputting = ucWorks.Where(x => x.registNo != 0 && (x.sequence >= PLC_ORDER_SEQUENCE.COMP_REGISTER && x.sequence <= PLC_ORDER_SEQUENCE.START_ORDER))
                //                         .OrderBy(x => x.registNo)
                //                         .FirstOrDefault();
                //    //ucWork ucInputting = ucWorks.OrderBy(x => x.registNo > 0).FirstOrDefault();
                //    foreach (Debug_ucWork work in ucWorks) 
                //    {
                //        //int a = work.registNo;
                //        //int b = ucInputting.registNo;
                //        if (work == ucInputting) work.isInput = true;
                //        else work.isInput = false;
                //    }
                //}
                //else 
                //{
                //}






                // PLC 設備ステータス読み出し
                rc = Resource.Plc[_aisleIndex].Access.GetMachineStatus(out PLCMachineStatus status);



                if (status.AisleAutoReady)
                    btnAutoReady.Background = Brushes.Yellow;
                else
                    btnAutoReady.Background = Brushes.Gray;

                if (status.AisleAutoRunningRequest)
                    lblAutoRunningReq.Background = Brushes.Yellow;
                else
                    lblAutoRunningReq.Background = Brushes.Gray;

                if (status.AisleAutoStopRequest)
                    lblAutoStopReq.Background = Brushes.Yellow;
                else
                    lblAutoStopReq.Background = Brushes.Gray;

                if (status.AisleAutoRunning)
                    lblAutoRunning.Background = Brushes.Yellow;
                else
                    lblAutoRunning.Background = Brushes.Gray;

                if (status.AisleAutoStop)
                    lblAutoStop.Background = Brushes.Yellow;
                else
                    lblAutoStop.Background = Brushes.Gray;

                lblCurrentRegistNoL.Content = status.CurrentRegistNo_L;
                lblCurrentRegistNoL.Background = Brushes.Yellow;
                lblCurrentRegistNoR.Content = status.CurrentRegistNo_R;
                lblCurrentRegistNoR.Background = Brushes.Yellow;

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
        /// PLCシミュレーター
        /// </summary>
        /// <returns></returns>
        private void Thread_PlcSimulator()
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                bool init = true;
                bool preAisleAutoRunning = false;
                Stopwatch sw_RunningReq = new Stopwatch();
                Stopwatch sw_StopReq = new Stopwatch();


                while (true) 
                {
                    if (_shutdown)
                        break;

                    if (init) 
                    {// 初期化
                        init = false;

                        // PLC設備ステータス 停止
                        rc = Resource.Plc[_aisleIndex].Access.Debug_SetAisleAutoStop();
                        rc = Resource.Plc[_aisleIndex].Access.Debug_SetCurrentRegistNoL(0);
                        rc = Resource.Plc[_aisleIndex].Access.Debug_SetCurrentRegistNoR(0);
                        rc = Resource.Plc[_aisleIndex].Access.Debug_SetSupplyUnitStatus(SUPPLY_UNIT_STATUS.STOP);
                        rc = Resource.Plc[_aisleIndex].Access.Debug_SetUnitStatus(UNIT_STATUS.STOP, IniFile.UnitEnable[_aisleIndex]);
                        rc = Resource.Plc[_aisleIndex].Access.Debug_SetSlotStatus(SLOT_STATUS.STOP, IniFile.UnitEnable[_aisleIndex]);
                    }


                    // ---------------------------------------------
                    // PLC 設備ステータス読み出し
                    rc = Resource.Plc[_aisleIndex].Access.GetMachineStatus(out PLCMachineStatus status);

                    // 最後に登録された仕分登録No
                    if(status.CurrentRegistNo_L != 0)
                        _latestRegistryNo_L = status.CurrentRegistNo_L;
                    if (status.CurrentRegistNo_R != 0)
                        _latestRegistryNo_R = status.CurrentRegistNo_R;


                    // ---------------------------------------------
                    // 運転停止 要求
                    if (status.AisleAutoRunningRequest) 
                    {
                        if(!sw_RunningReq.IsRunning) sw_RunningReq.Restart();
                        if (sw_RunningReq.ElapsedMilliseconds > 3000) 
                        {// 運転
                            sw_RunningReq.Reset();
                            rc = Resource.Plc[_aisleIndex].Access.Debug_SetAisleAutoRunning();
                        }
                    }
                    if (status.AisleAutoStopRequest)
                    {
                        if (!sw_StopReq.IsRunning) sw_StopReq.Restart();
                        if (sw_StopReq.ElapsedMilliseconds > 3000)
                        {// 停止
                            sw_StopReq.Reset();
                            rc = Resource.Plc[_aisleIndex].Access.Debug_SetAisleAutoStop();
                        }
                    }

                    // ---------------------------------------------
                    // 運転停止 切り替え
                    if (status.AisleAutoRunning && !preAisleAutoRunning)
                    {// 運転
                        preAisleAutoRunning = true;

                        // PLC設備ステータス 運転
                        rc = Resource.Plc[_aisleIndex].Access.Debug_SetSupplyUnitStatus(SUPPLY_UNIT_STATUS.NORMAL);
                        rc = Resource.Plc[_aisleIndex].Access.Debug_SetUnitStatus(UNIT_STATUS.NORMAL, IniFile.UnitEnable[_aisleIndex]);
                        rc = Resource.Plc[_aisleIndex].Access.Debug_SetSlotStatus(SLOT_STATUS.NORMAL, IniFile.UnitEnable[_aisleIndex]);


                    }
                    if (status.AisleAutoStop && preAisleAutoRunning)
                    {// 停止
                        preAisleAutoRunning = false;

                        // PLC設備ステータス 停止
                        rc = Resource.Plc[_aisleIndex].Access.Debug_SetSupplyUnitStatus(SUPPLY_UNIT_STATUS.STOP);
                        rc = Resource.Plc[_aisleIndex].Access.Debug_SetUnitStatus(UNIT_STATUS.STOP, IniFile.UnitEnable[_aisleIndex]);
                        rc = Resource.Plc[_aisleIndex].Access.Debug_SetSlotStatus(SLOT_STATUS.STOP, IniFile.UnitEnable[_aisleIndex]);
                    }

                    // ---------------------------------------------
                    // 仕分完了->未登録
                    int L = status.CurrentRegistNo_L;
                    int R = status.CurrentRegistNo_R;
                    if (L != 0) 
                    {
                        for (int i = 0; i < Const.MaxWorkRegisterCount; i++)
                        {
                            if (ucWorks[i].registNo == L)
                            {
                                // 仕分完了していたら、作業中仕分登録Noをクリア
                                if(ucWorks[i].sequence == PLC_ORDER_SEQUENCE.COMP_ORDER || ucWorks[i].sequence == PLC_ORDER_SEQUENCE.CANCEL_ORDER)
                                    rc = Resource.Plc[_aisleIndex].Access.Debug_SetCurrentRegistNoL(0);
                            }
                        }
                    }
                    if (R != 0)
                    {
                        for (int i = 0; i < Const.MaxWorkRegisterCount; i++)
                        {
                            if (ucWorks[i].registNo == R)
                            {
                                // 仕分完了していたら、作業中仕分登録Noをクリア
                                if (ucWorks[i].sequence == PLC_ORDER_SEQUENCE.COMP_ORDER || ucWorks[i].sequence == PLC_ORDER_SEQUENCE.CANCEL_ORDER)
                                    rc = Resource.Plc[_aisleIndex].Access.Debug_SetCurrentRegistNoR(0);
                            }
                        }
                    }


                    // ---------------------------------------------
                    // 登録完了->仕分開始
                    int lastRegistNo = Math.Max(_latestRegistryNo_L, _latestRegistryNo_R);
                    int nextRegistNo = lastRegistNo + 1;

                    if (L == 0)
                    {
                        for (int i = 0; i < Const.MaxWorkRegisterCount; i++)
                        {
                            if (ucWorks[i].registNo == nextRegistNo)
                            {
                                // 仕分完了していたら、作業中仕分登録Noをクリア
                                if (ucWorks[i].sequence == PLC_ORDER_SEQUENCE.COMP_REGISTER && ucWorks[i].startOrderReady)
                                {
                                    // 仕分開始
                                    rc = Resource.Plc[_aisleIndex].Access.Debug_SetCurrentRegistNoL(nextRegistNo);
                                    rc = Resource.Plc[_aisleIndex].Access.SetOrderSequence(i, PLC_ORDER_SEQUENCE.START_ORDER);
                                }
                            }
                        }
                    }
                    else if (R == 0) 
                    {
                        for (int i = 0; i < Const.MaxWorkRegisterCount; i++)
                        {
                            if (ucWorks[i].registNo == nextRegistNo)
                            {
                                // 仕分完了していたら、作業中仕分登録Noをクリア
                                if (ucWorks[i].sequence == PLC_ORDER_SEQUENCE.COMP_REGISTER && ucWorks[i].startOrderReady)
                                {
                                    // 仕分開始
                                    rc = Resource.Plc[_aisleIndex].Access.Debug_SetCurrentRegistNoR(nextRegistNo);
                                    rc = Resource.Plc[_aisleIndex].Access.SetOrderSequence(i, PLC_ORDER_SEQUENCE.START_ORDER);
                                }
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return;
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
