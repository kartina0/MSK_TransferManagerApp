//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Markup;

using BatchModule;
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
    /// 1アイルのサイクル
    /// </summary>
    public class CycleAisle
    {
        private const string THIS_NAME = "CycleAisle";

        /// <summary>
        /// ステーション Index
        /// </summary>
        private int _stationIndex = 0;
        /// <summary>
        /// アイル Index
        /// </summary>
        private int _aisleIndex = 0;
        /// <summary>
        /// 現在の便Index
        /// </summary>
        private int _currentPostIndex = 0;
        /// <summary>
        /// 現在のバッチIndex
        /// </summary>
        private int _currentBatchIndex = 0;


        ///// <summary>
        ///// アイルステータス
        ///// </summary>
        //public AISLE_STATUS aisleStatus = AISLE_STATUS.NONE;
        ///// <summary>
        ///// PLC 仕分実行フラグ (10ワーク分)
        ///// </summary>
        //public PLC_ORDER_SEQUENCE[] plcOrderFlg = null;
        ///// <summary>
        ///// 前回のPLC仕分実行フラグ (10ワーク分)
        ///// </summary>
        //public PLC_ORDER_SEQUENCE[] prePlcOrderFlg = null;

        /// <summary>
        /// サイクル管理スレッド
        /// </summary>
        private ThreadInfo _cycleThread = new ThreadInfo(THREAD_SEQUENCE_TYPE.CONTINUOUS);
        /// <summary>
        /// スレッド シャットダウン
        /// </summary>
        private bool _shutDown = false;
        /// <summary>
        /// スレッド シャットダウン完了
        /// </summary>
        private bool _shutDownComp = false;


        /// <summary>
        /// PLC
        /// </summary>
        public PLC plc = null;
        /// <summary>
        /// Server
        /// </summary>
        public Server server = null;

        /// <summary>
        /// 仕分データ実績リスト
        /// </summary>
        private List<ExecuteData> _executeDataList = new List<ExecuteData>();




        /// <summary>
        /// 起動
        /// </summary>
        /// <returns></returns>
        public UInt32 Start(int aisleIndex, PLC plcManager)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // アイルNo
                _aisleIndex = aisleIndex;
                // サイクル初期化
                _currentPostIndex = 0;
                _currentBatchIndex = 0;

                // PLC管理オブジェクト
                plc = plcManager;
                // Server管理オブジェクト
                server = Resource.Server;
                //// PLC 仕分実行フラグリスト
                //plcOrderFlg = new PLC_ORDER_SEQUENCE[Const.MaxWorkRegisterCount];
                //prePlcOrderFlg = new PLC_ORDER_SEQUENCE[Const.MaxWorkRegisterCount];

                // スレッド起動
                _cycleThread.CreateThread(Thread_Cycle, _cycleThread, ThreadPriority.Lowest);
                _cycleThread.Interval = 1000;
                _cycleThread.Release();


                //rc = Resource.Plc[_aisleIndex].Access.SetOrderPostNo(1, 1);
                
                //rc = Resource.Plc[_aisleIndex].Access.SetOrderPostNo(0, 1);

            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 終了
        /// </summary>
        public UInt32 Close()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()"); 
            try
            {
                // ----------------------------------
                // スレッド終了
                // ----------------------------------
                if (_cycleThread != null) 
                {
                    _shutDown = true;
                    while(!_shutDownComp)
                        Thread.Sleep(100);
                }
                _cycleThread = null;
                Logger.WriteLog(LogType.INFO, string.Format("サイクル管理スレッド 終了"));


                //// 先に終了をコールする
                //_cycleThread.ShutDown(0);
                //Logger.WriteLog(LogType.INFO, string.Format("サイクル管理スレッド 終了"));
                //_cycleThread.ShutDown(1000);
                //_cycleThread = null;
            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }


        /// <summary>
        /// サイクルスレッド
        /// </summary>
        private void Thread_Cycle(object arg)
        {
            UInt32 rc = 0;
            ThreadInfo info = (ThreadInfo)arg;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                bool exit = false;

                // 前回PLC仕分実行フラグ
                PLC_ORDER_SEQUENCE[] pre_sequence = new PLC_ORDER_SEQUENCE[Const.MaxWorkRegisterCount];
                for (int i = 0; i < Const.MaxWorkRegisterCount; i++)
                    pre_sequence[i] = PLC_ORDER_SEQUENCE.NOT_REGISTER;

                
                List<OrderStoreData> storeList_beforeConv = new List<OrderStoreData>();             // PICKDATAそのまま
                List<OrderStoreData> storeList_AfterConv = new List<OrderStoreData>();              // バッチ情報に変換後
                List<ExecuteStoreData> executeStoreList_beforeConv = new List<ExecuteStoreData>();      // PICKDATAそのまま
                List<ExecuteStoreData> executeStoreList_AfterConv = new List<ExecuteStoreData>();       // バッチ情報に変換後


                // 仕分キャンセル用
                // 仕分キャンセル商品を再度仕分ける際に、前回の仕分済数量を保持しておく。(もっとキレイな保持のやり方がないか)
                int[] preSlotOrderCompCount = new int[Const.MaxSlotCount * Const.MaxUnitCount];


                while (true)
                {
                    rc = 0;
                    if (_shutDown)
                        break;

                    // Waits Any Event
                    THREAD_WAIT_RESULT index = info.WaitAnyEvent(100);

                    if (index == THREAD_WAIT_RESULT.SHUTDOWN)
                    {
                        exit = true;
                    }
                    if (index == THREAD_WAIT_RESULT.REQUEST)
                    {

                        // 日付
                        DateTime currentDateTime = DateTime.ParseExact($"{(DateTime.Now).Year.ToString("D4")}" +
                              $"{(DateTime.Now).Month.ToString("D2")}" +
                              $"{(DateTime.Now).Day.ToString("D2")}" +
                              $"000000", "yyyyMMddHHmmss", null);

                        // 日付切り替わり処理
                        DateTime dt = DateTime.Now;
                        if (dt < DateTime.Today.AddHours(IniFile.DateChangeTime.Hour).AddMinutes(IniFile.DateChangeTime.Minute))
                        {
                            currentDateTime = currentDateTime.AddDays(-1);
                        }


                        // PLC 設備ステータス読み出し
                        rc = Resource.Plc[_aisleIndex].Access.GetMachineStatus(out PLCMachineStatus plcMachineStatus);


                        if(plcMachineStatus.AisleAutoRunning || IniFile.DebugMode)
                        {// 運転中

                            // ----------------------------------
                            // 基本情報
                            // ----------------------------------
                            // 現在便No
                            _currentPostIndex = Resource.SystemStatus.CurrentPostIndex;
                            // 現在バッチNo
                            _currentBatchIndex = Resource.SystemStatus.CurrentBatchIndex[_aisleIndex];
                            // 現在便のバッチ数
                            int batchCount = Resource.batch.BatchInfoCurrent.Post[_currentPostIndex].Aisle[_aisleIndex].Batch.Count;
                            // 現在バッチ情報(スロットNo順に並んだ店コードの配列)
                            string[] currentbatchArray = 
                                Resource.batch.BatchInfoCurrent.Post[_currentPostIndex].Aisle[_aisleIndex].Batch[_currentBatchIndex].OutputToArray(IniFile.UnitEnable[_aisleIndex]);


                            // ----------------------------------
                            // 10ワーク分のサイクル処理
                            // ----------------------------------
                            for (int workIndex = 0; workIndex < Const.MaxWorkRegisterCount; workIndex++) 
                            {
                                // ----------------------------------
                                // PLCから仕分情報を取得
                                // ----------------------------------
                                rc = plc.Access.GetOrderStatus(workIndex, out PLCWorkOrder plcOrderStatus);
                                PLC_ORDER_SEQUENCE sequence = plcOrderStatus.sequence;
                                //plcOrderFlg[workIndex] = sequence;


                                if (sequence == PLC_ORDER_SEQUENCE.NOT_REGISTER) 
                                {// 未登録

                                    //Resource.SystemStatus.CycleStatus[_aisleIndex] = CYCLE_STATUS.WAITING;

                                    // ----------------------------------
                                    // PLCへ仕分情報を書込み
                                    // ----------------------------------
                                    // 仕分日
                                    int orderDate = int.Parse($"{DateTime.Now.Month.ToString("D2")}{DateTime.Now.Day.ToString("D2")}");
                                    rc = plc.Access.SetOrderDate(workIndex, orderDate);
                                    // 仕分便No
                                    rc = plc.Access.SetOrderPostNo(workIndex, _currentPostIndex + 1);

                                    continue;
                                }
                                else if (sequence == PLC_ORDER_SEQUENCE.REQ_REGISTER)
                                {// 登録要求

                                    if (sequence != pre_sequence[workIndex])
                                        Logger.WriteLog(LogType.INFO, $"{_currentPostIndex + 1}便 アイル{_aisleIndex + 1} ワーク登録番号{workIndex + 1}  [登録要求]  {plcOrderStatus.JANCode}");

                                    storeList_beforeConv = new List<OrderStoreData>();
                                    storeList_AfterConv = new List<OrderStoreData>();

                                    // ----------------------------------
                                    // JANコードを取得
                                    // ----------------------------------
                                    string janCode = plcOrderStatus.JANCode;

                                    // ----------------------------------
                                    // 仕分データから
                                    // 現在バッチに該当する店別小仕分けデータを抽出
                                    // ----------------------------------
                                    // 店別小仕分けデータから、バッチの店コードをもつデータだけ抽出
                                    // ※currentbatchArrayは当アイルの当便のバッチ情報
                                    // ※storeListはスロット順で出力される
                                    rc = server.OrderInfo.SelectOrderStoreData(currentDateTime, _currentPostIndex, janCode, currentbatchArray, out storeList_beforeConv);
                                    if (storeList_beforeConv == null)
                                    {
                                        // 商品データが見つからない
                                        string message = $"JANコードが見つかりません。仕分データに商品が存在することを確認してください JANコード : {janCode}";
                                        Resource.ErrorHandler(message);
                                        rc = plc.Access.SetOrderSequence(workIndex, PLC_ORDER_SEQUENCE.REGISTER_ERROR);
                                        continue;
                                    }

                                    // 総仕分数量を算出
                                    int orderCountTotal = (int)storeList_beforeConv.Select(x => x.orderCount).Sum();   // ※バッチ内での仕分合計数

                                    // ここで、バッチ情報に変換したデータを複製する
                                    storeList_AfterConv = new List<OrderStoreData>();
                                    foreach (var item in storeList_beforeConv)
                                    {
                                        OrderStoreData storeData = item.Copy();

                                        // 店コードを取得
                                        string storeCode = storeData.storeCode;
                                        // バッチ情報から店コード検索する
                                        int slotIndex = Array.IndexOf(currentbatchArray, storeCode);

                                        int stationNo = 1;
                                        int aisleNo = _aisleIndex + 1;
                                        int slotNo = slotIndex + 1;
                                        storeData.stationNo = stationNo;
                                        storeData.aisleNo = aisleNo;
                                        storeData.slotNo = slotNo;
                                        storeList_AfterConv.Add(storeData);
                                    }



                                    // ----------------------------------
                                    // 仕分実績データから
                                    // 現在バッチに該当する店別小仕分け実績データを抽出
                                    // (仕分キャンセル対応)
                                    // ----------------------------------
                                    // ※currentbatchArrayは当アイルの当便のバッチ情報
                                    // ※storeListはスロット順で出力される
                                    rc = server.OrderInfo.SelectExecuteStoreData(currentDateTime, _currentPostIndex, janCode, currentbatchArray, out executeStoreList_beforeConv);
                                    if (executeStoreList_beforeConv == null)
                                    {
                                        // 商品データが見つからない
                                        string message = $"JANコードが見つかりません。仕分データに商品が存在することを確認してください JANコード : {janCode}";
                                        Resource.ErrorHandler(message);
                                        rc = plc.Access.SetOrderSequence(workIndex, PLC_ORDER_SEQUENCE.REGISTER_ERROR);
                                        continue;
                                    }

                                    // 総仕分済数量を算出
                                    int orderCompCountTotal = (int)executeStoreList_beforeConv.Select(x => x.orderCompCount).Sum();   // ※バッチ内での仕分合計数
                                    if (orderCompCountTotal >= orderCountTotal)
                                    {// エラー
                                        // すでに仕分完了している
                                        string message = $"既に仕分完了している商品です JANコード : {janCode}";
                                        Resource.ErrorHandler(message);
                                        rc = plc.Access.SetOrderSequence(workIndex, PLC_ORDER_SEQUENCE.REGISTER_ERROR);
                                        continue;
                                    }

                                    // ここで、バッチ情報に変換したスロットを複製する
                                    executeStoreList_AfterConv = new List<ExecuteStoreData>();
                                    foreach (var item in executeStoreList_beforeConv)
                                    {
                                        ExecuteStoreData executeData = item.Copy();

                                        // 店コードを取得
                                        string storeCode = executeData.storeCode;
                                        // バッチ情報から店コード検索する
                                        int slotIndex = Array.IndexOf(currentbatchArray, storeCode);

                                        int stationNo = 1;
                                        int aisleNo = _aisleIndex + 1;
                                        int slotNo = slotIndex + 1;
                                        executeData.stationNo = stationNo;
                                        executeData.aisleNo = aisleNo;
                                        executeData.slotNo = slotNo;
                                        executeStoreList_AfterConv.Add(executeData);
                                    }



                                    // ----------------------------------
                                    // PLCへ仕分情報を書込み
                                    // ----------------------------------
                                    // 商品名(ｶﾅ)
                                    string workNameKana = "";
                                    rc = server.OrderInfo.SelectOrderData(currentDateTime, _currentPostIndex, janCode, out OrderData orderData);
                                    workNameKana = orderData.workNameKana;
                                    rc = plc.Access.SetWorkNameKana(workIndex, workNameKana);

                                    // 仕分日
                                    int orderDate = int.Parse($"{DateTime.Now.Month.ToString("D2")}{DateTime.Now.Day.ToString("D2")}");
                                    rc = plc.Access.SetOrderDate(workIndex, orderDate);
                                    
                                    // 仕分便No
                                    rc = plc.Access.SetOrderPostNo(workIndex, _currentPostIndex + 1);

                                    // 総仕分数量 (総仕分数量 - 総仕分済数量)
                                    rc = plc.Access.SetOrderCountTotal(workIndex, orderCountTotal - orderCompCountTotal);

                                    //// 総仕分済数量
                                    //rc = plc.Access.SetOrderCompCountTotal(workIndex, orderCompCountTotal);

                                    // 各スロット仕分数量 (各スロット仕分数量 - 各スロット仕分済数量)
                                    int[] slotOrderCount = new int[Const.MaxSlotCount * Const.MaxUnitCount];
                                    foreach (var store in storeList_AfterConv) 
                                        slotOrderCount[store.slotNo - 1] = (int)store.orderCount;               // 仕分数量
                                    preSlotOrderCompCount = new int[Const.MaxSlotCount * Const.MaxUnitCount];
                                    foreach (var store in executeStoreList_AfterConv)
                                        preSlotOrderCompCount[store.slotNo - 1] = (int)store.orderCompCount;    // 仕分済数量
                                    for (int i = 0; i < Const.MaxSlotCount * Const.MaxUnitCount; i++) 
                                        slotOrderCount[i] = slotOrderCount[i] - preSlotOrderCompCount[i];       // 仕分数量 - 仕分済数量
                                    rc = plc.Access.SetOrderCountSlot(workIndex, slotOrderCount);

                                    //// 各スロット仕分済数量
                                    //int[] slotOrderCompCount = new int[Const.MaxSlotCount * Const.MaxUnitCount];
                                    //foreach (var store in executeStoreList_AfterConv)
                                    //    slotOrderCompCount[store.slotNo - 1] = (int)store.orderCompCount;
                                    //rc = plc.Access.SetOrderCompCountSlot(workIndex, slotOrderCompCount);

                                    // 各ロボット仕分数量
                                    int robotCount = 6;
                                    int[] robotOrderCount = new int[robotCount];
                                    int startIndex = 0;
                                    for (int i = 0; i < robotCount; i++)
                                    {
                                        startIndex = i * robotCount;
                                        for (int j = 0; j < Const.MaxSlotCount / 2; j++)
                                        {
                                            OrderStoreData store = storeList_AfterConv.Where(x => x.storeCode == currentbatchArray[(i * robotCount) + j]).FirstOrDefault();
                                            if (store != null) 
                                            {
                                                int orderCount = (int)store.orderCount;
                                                robotOrderCount[i] += orderCount;
                                            }
                                        }
                                    }
                                    rc = plc.Access.SetOrderCountRobot(workIndex, robotOrderCount);

                                    // ケース入数
                                    MasterWork work = Resource.Server.MasterFile.MasterWorkList[_currentPostIndex].Find(x => x.JANCode == janCode);
                                    if (work != null)
                                    {
                                        // @@20240517_1
                                        //rc = Resource.Plc[_aisleIndex].Access.SetCaseVolume(workIndex, (int)work.packCount);
                                        rc = Resource.Plc[_aisleIndex].Access.SetCaseVolume(workIndex, (int)work.centerCount);
                                    }
                                    else 
                                    {
                                        // 商品マスターにJANコードがない
                                        string message = $"JANコードが見つかりません。商品マスターに商品が存在することを確認してください JANコード : {janCode}";
                                        Resource.ErrorHandler(message);
                                    }


                                    // 仕分作業開始日時を入力
                                    if (PreStatus.PostStartDt[_currentPostIndex] == DateTime.MinValue) 
                                    {
                                        DateTime now = DateTime.Now;
                                        PreStatus.PostStartDt[_currentPostIndex] = now;
                                        string fileName = System.IO.Path.Combine(Const.IniDir, Const.PreStatusFileName);
                                        rc = PreStatus.Save(fileName);
                                    }

                                    if (sequence != pre_sequence[workIndex]) 
                                    {
                                        Logger.WriteLog(LogType.INFO, $"{_currentPostIndex + 1}便 アイル{_aisleIndex + 1} ワーク登録番号{workIndex + 1}\r\n" +
                                            $"商品名:{workNameKana} JANコード:{janCode} 総仕分数:{orderCountTotal} 総仕分済数{orderCompCountTotal}");
                                    }

                                    // ----------------------------------
                                    // PLC 仕分実行フラグ:登録完了 書込み
                                    // ----------------------------------
                                    rc = plc.Access.SetOrderSequence(workIndex, PLC_ORDER_SEQUENCE.COMP_REGISTER);

                                }
                                else if (sequence == PLC_ORDER_SEQUENCE.COMP_REGISTER)
                                {// 登録完了

                                    if (sequence != pre_sequence[workIndex])
                                        Logger.WriteLog(LogType.INFO, $"{_currentPostIndex + 1}便 アイル{_aisleIndex + 1} ワーク登録番号{workIndex + 1}  [登録完了]  {plcOrderStatus.JANCode}");




                                }
                                else if (sequence == PLC_ORDER_SEQUENCE.START_ORDER)
                                {// 仕分開始

                                    if (sequence != pre_sequence[workIndex])
                                        Logger.WriteLog(LogType.INFO, $"{_currentPostIndex + 1}便 アイル{_aisleIndex + 1} ワーク登録番号{workIndex + 1}  [仕分開始]  {plcOrderStatus.JANCode}");

                                    // ----------------------------------
                                    // 仕分実績データリストを更新
                                    // ----------------------------------
                                    // 仕分済数量 = PLCから読み出した仕分済数量 + 前回の仕分済数量(仕分キャンセルを想定)
                                    int[] slotOrderCompCount = new int[Const.MaxSlotCount * Const.MaxUnitCount];
                                    for (int i = 0; i < Const.MaxSlotCount * Const.MaxUnitCount; i++) slotOrderCompCount[i] = plcOrderStatus.orderCountComp[i] + preSlotOrderCompCount[i];
                                    rc = server.OrderInfo.UpdateExecuteDataOrderCount(_currentPostIndex, plcOrderStatus.JANCode, slotOrderCompCount, currentbatchArray);

                                }
                                else if (sequence == PLC_ORDER_SEQUENCE.CANCEL_ORDER)
                                {// 仕分中断

                                    if (sequence != pre_sequence[workIndex])
                                        Logger.WriteLog(LogType.INFO, $"{_currentPostIndex + 1}便 アイル{_aisleIndex + 1} ワーク登録番号{workIndex + 1}  [仕分キャンセル]  {plcOrderStatus.JANCode}");

                                    if (sequence != pre_sequence[workIndex])
                                    {
                                        // ----------------------------------
                                        // 仕分実績データリストを更新
                                        // ----------------------------------
                                        // 仕分済数量 = PLCから読み出した仕分済数量 + 前回の仕分済数量(仕分キャンセルを想定)
                                        int[] slotOrderCompCount = new int[Const.MaxSlotCount * Const.MaxUnitCount];
                                        for (int i = 0; i < Const.MaxSlotCount * Const.MaxUnitCount; i++) slotOrderCompCount[i] = plcOrderStatus.orderCountComp[i] + preSlotOrderCompCount[i];
                                        rc = server.OrderInfo.UpdateExecuteDataOrderCount(_currentPostIndex, plcOrderStatus.JANCode, slotOrderCompCount, currentbatchArray);

                                        // 実績データ書き込みフラグ 要求
                                        ServerControl.process_WriteExecuteUpData[workIndex] = Step.REQUEST;
                                    }

                                    // 実績データ書き込みフラグ 完了待ち
                                    if (ServerControl.process_WriteExecuteUpData[workIndex] == Step.COMP)
                                    {
                                        // 実績データ書き込みフラグ クリア
                                        ServerControl.process_WriteExecuteUpData[workIndex] = Step.NONE;
                                        // PLCの現在ワーク欄をゼロクリア
                                        rc = plc.Access.SetOrderStatusClear(workIndex);

                                        // ----------------------------------
                                        // PLC 仕分実行フラグ:未登録 書込み
                                        // ----------------------------------
                                        rc = plc.Access.SetOrderSequence(workIndex, PLC_ORDER_SEQUENCE.NOT_REGISTER);
                                    }

                                }
                                else if (sequence == PLC_ORDER_SEQUENCE.COMP_ORDER)
                                {// 仕分完了

                                    if (sequence != pre_sequence[workIndex])
                                        Logger.WriteLog(LogType.INFO, $"{_currentPostIndex + 1}便 アイル{_aisleIndex + 1} ワーク登録番号{workIndex + 1}  [仕分完了]  {plcOrderStatus.JANCode}");

                                    if (sequence != pre_sequence[workIndex]) 
                                    {
                                        // ----------------------------------
                                        // 仕分実績データリストを更新
                                        // ----------------------------------
                                        int[] slotOrderCompCount = new int[Const.MaxSlotCount * Const.MaxUnitCount];
                                        for (int i = 0; i < Const.MaxSlotCount * Const.MaxUnitCount; i++) slotOrderCompCount[i] = plcOrderStatus.orderCountComp[i] + preSlotOrderCompCount[i];
                                        rc = server.OrderInfo.UpdateExecuteDataOrderCount(_currentPostIndex, plcOrderStatus.JANCode, slotOrderCompCount, currentbatchArray);

                                        // 実績データ書き込みフラグ 要求
                                        ServerControl.process_WriteExecuteUpData[workIndex] = Step.REQUEST;
                                    }

                                    // 実績データ書き込みフラグ 完了待ち
                                    if (ServerControl.process_WriteExecuteUpData[workIndex] == Step.COMP) 
                                    {
                                        // 実績データ書き込みフラグ クリア
                                        ServerControl.process_WriteExecuteUpData[workIndex] = Step.NONE;
                                        // PLCの現在ワーク欄をゼロクリア
                                        rc = plc.Access.SetOrderStatusClear(workIndex);

                                        // ----------------------------------
                                        // PLC 仕分実行フラグ:未登録 書込み
                                        // ----------------------------------
                                        rc = plc.Access.SetOrderSequence(workIndex, PLC_ORDER_SEQUENCE.NOT_REGISTER);
                                    }

                                    // 仕分作業終了日時を入力
                                    DateTime now = DateTime.Now;
                                    if (PreStatus.PostEndDt[_currentPostIndex] == DateTime.MinValue || PreStatus.PostEndDt[_currentPostIndex] < now)
                                    {
                                        PreStatus.PostEndDt[_currentPostIndex] = now;
                                        string fileName = System.IO.Path.Combine(Const.IniDir, Const.PreStatusFileName);
                                        rc = PreStatus.Save(fileName);
                                    }

                                }


                                // 仕分実行フラグを最新に更新
                                pre_sequence[workIndex] = sequence;

                            }




                            // ----------------------------------
                            // バッチ完了確認
                            // ----------------------------------

                            // ----------------------------------
                            // 便完了確認
                            // ----------------------------------

                            // ----------------------------------
                            // アイルステータス更新
                            // ----------------------------------


                        }

                    }
                    if (exit) break;
                    Thread.Sleep(info.Interval);
                }

            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            _shutDownComp = true;
        }


        /// <summary>
        /// Check Error State
        /// </summary>
        private static bool STATUS_SUCCESS(UInt32 err) { return err == (int)ErrorCodeList.STATUS_SUCCESS; }
    }

}
