//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Reflection;

using DL_PlcInterfce;
using DL_Logger;
using SystemConfig;
using ErrorCodeDefine;


namespace PLCModule
{
    /// <summary>
    /// PLCアクセス
    /// </summary>
    public class PLCAccess
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "PLCAccess";
        /// <summary>
        /// PLC IF オブジェクト
        /// </summary>
        private PLC_IF _plcIf = new PLC_IF();
        /// <summary>
        /// 接続文字列
        /// </summary>
        private string _connectionString = "";



        /// <summary>
        /// PLC ダミーモード確認
        /// </summary>
        public bool isDummy 
        {
            get 
            {
                return _plcIf.IsDummy();
            } 
        }


        #region 設備ステータス アドレス
        /// <summary> 
        /// 設備ステータス デバイス種別
        /// </summary>
        private DeviceType _deviceType_MachineStatus = DeviceType.ExtendDataMemory;
        /// <summary> 
        /// 設備ステータス 先頭アドレス 
        /// </summary>
        private int _startAddr_MachineStatus = 0;
        /// <summary>
        /// 設備ステータス メモリ割り当て
        /// </summary>
        private enum PLC_ADDR_MACHINE_STATUS
        {
            /// <summary> アイル自動運転 可能 </summary>
            AISLE_AUTO_READY = 0,
            /// <summary> アイル自動運転 運転中 </summary>
            AISLE_AUTO_RUNNING,
            /// <summary> アイル自動運転 停止中 </summary>
            AISLE_AUTO_STOP,
            /// <summary> アイル自動運転 開始要求 </summary>
            AISLE_AUTO_RUNNING_REQUEST,
            /// <summary> アイル自動運転 停止要求 </summary>
            AISLE_AUTO_STOP_REQUEST,

            /// <summary> 作業中の仕分登録No (L) </summary>
            CURRENT_REGIST_NO_L = 10,
            /// <summary> 作業中の仕分登録No (R) </summary>
            CURRENT_REGIST_NO_R,

            /// <summary> 番重供給機ステータス (L) </summary>
            SUPPLY_UNIT_STATUS_L = 20,
            /// <summary> 番重供給機ステータス (R) </summary>
            SUPPLY_UNIT_STATUS_R,
            /// <summary> 番重供給機エラーコード (L) </summary>
            SUPPLY_UNIT_ERROR_CODE_L,
            /// <summary> 番重供給機エラーコード (R) </summary>
            SUPPLY_UNIT_ERROR_CODE_R,

            /// <summary> ユニット01ステータス (L) </summary>
            UNIT01_STATUS_L = 30,
            /// <summary> ユニット01ステータス (R) </summary>
            UNIT01_STATUS_R,
            /// <summary> ユニット02ステータス (L) </summary>
            UNIT02_STATUS_L,
            /// <summary> ユニット02ステータス (R) </summary>
            UNIT02_STATUS_R,
            /// <summary> ユニット03ステータス (L) </summary>
            UNIT03_STATUS_L,
            /// <summary> ユニット03ステータス (R) </summary>
            UNIT03_STATUS_R,

            /// <summary> ユニット01エラーコード (L) </summary>
            UNIT01_ERROR_CODE_L = 40,
            /// <summary> ユニット01エラーコード (R) </summary>
            UNIT01_ERROR_CODE_R,
            /// <summary> ユニット02エラーコード (L) </summary>
            UNIT02_ERROR_CODE_L,
            /// <summary> ユニット02エラーコード (R) </summary>
            UNIT02_ERROR_CODE_R,
            /// <summary> ユニット03エラーコード (L) </summary>
            UNIT03_ERROR_CODE_L,
            /// <summary> ユニット03エラーコード (R) </summary>
            UNIT03_ERROR_CODE_R,

            /// <summary> スロットステータス </summary>
            SLOT_STATUS = 50,
            /// <summary> スロットエラーコード </summary>
            SLOT_ERROR_CODE = 100,
        }

        /// <summary> データ長 設備ステータス全体 </summary>
        private int _dataSize_MachineStatus_Total = 500;
        /// <summary> データ長 スロットステータス </summary>
        private int _dataSize_SlotStatus = 36;
        /// <summary> データ長 スロットエラーコード </summary>
        private int _dataSize_SlotErrorCode = 36;
        #endregion


        #region 仕分ステータス アドレス
        /// <summary> 
        /// 仕分ステータス デバイス種別
        /// </summary>
        private DeviceType _deviceType_OrderStatus = DeviceType.ExtendDataMemory;
        /// <summary> 
        /// 仕分ステータス 先頭アドレス 
        /// </summary>
        private int _startAddr_OrderStatus = 30000;
        /// <summary>
        /// 仕分ステータス メモリ割り当て
        /// </summary>
        private enum PLC_ADDR_ORDER_STATUS 
        {
            /// <summary> 仕分登録No </summary>
            REGISTRY_NO = 0,
            /// <summary> 仕分日(MMdd) </summary>
            ORDER_DATE,
            /// <summary> 仕分便No </summary>
            POST_NO,
            /// <summary> 総仕分数量 </summary>
            ORDER_COUNT_TOTAL,
            /// <summary> 総投入数量 </summary>
            ORDER_COUNT_INPUT_TOTAL,
            /// <summary> 総仕分済数量 </summary>
            ORDER_COUNT_COMP_TOTAL,
            /// <summary> 仕分実行フラグ (0=未登録)</summary>
            SEQUENCE,

            /// <summary> JANコード </summary>
            JAN_CODE = 10,
            /// <summary> 商品名 </summary>
            WORK_NAME = 20,
            /// <summary> ケース入数 </summary>
            CASE_VOLUME = 49,
            /// <summary> 仕分数量(スロット) </summary>
            ORDER_COUNT_SLOT = 50,
            /// <summary> 仕分数量(ロボット) </summary>
            ORDER_COUNT_ROBOT = 90,
            ///// <summary> 投入済数量(スロット) </summary>
            //ORDER_COUNT_INPUT_SLOT = 50,
            ///// <summary> 投入済数量(ロボット) </summary>
            //ORDER_COUNT_INPUT_ROBOT = 90,
            /// <summary> 仕分済数量(スロット) </summary>
            ORDER_COUNT_COMP_SLOT = 150,
            /// <summary> 仕分済数量(ロボット) </summary>
            ORDER_COUNT_COMP_ROBOT = 190,
            /// <summary> ワーク取出対象スロット </summary>
            TARGET_SLOT = 200,
        }

        /// <summary> データ長 仕分ステータス(1ワーク分) </summary>
        private int _dataSize_OrderStatus_Total = 1500;
        /// <summary> データ長 仕分情報 </summary>
        private int _dataSize_Order = 7;
        /// <summary> データ長 JANコード </summary>
        private int _dataSize_JANCode = 14;
        /// <summary> データ長 商品名(カナ) </summary>
        private int _dataSize_WorkName = 40;
        /// <summary> データ長 仕分数量(スロット) </summary>
        private int _dataSize_OrderCountSlot = 36;
        /// <summary> データ長 仕分数量(ロボット) </summary>
        private int _dataSize_OrderCountRobot = 6;
        /// <summary> データ長 仕分済数量(スロット) </summary>
        private int _dataSize_OrderCompCountSlot = 36;
        /// <summary> データ長 仕分済数量(ロボット) </summary>
        private int _dataSize_OrderCountComp = 6;
        /// <summary> データ長 ワーク取出対象スロット </summary>
        private int _dataSize_TargetSlot = 300;

        #endregion



        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PLCAccess(string connectionString) 
        {
            _connectionString = connectionString;
        }



        #region 接続/切断
        /// <summary>
        /// 接続
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public UInt32 Open(string connectionString)
        {
            UInt32 rc = 0;
            try
            {
                rc = _plcIf.Open(connectionString);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}"); 
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// 切断
        /// </summary>
        /// <returns></returns>
        public UInt32 Close()
        {
            UInt32 rc = 0;
            try
            {
                _plcIf.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        #endregion


        #region -------- 設備ステータス --------

        #region 読み出し
        /// <summary>
        /// 読み出し
        /// 設備ステータス
        /// </summary>
        /// <returns></returns>
        public UInt32 GetMachineStatus(out PLCMachineStatus machineStatus)
        {
            UInt32 rc = 0;

            int count = 0;
            int addr = 0;
            int[] val = null;
            machineStatus = new PLCMachineStatus();
            try
            {
                // 接続確認
                if (!_plcIf.IsConnected()) 
                {
                    Close();
                    Open(_connectionString);
                }


                // ----------------------------------
                // 読み出し
                // ----------------------------------
                addr = _startAddr_MachineStatus;
                count = _dataSize_MachineStatus_Total;
                val = new int[count];
                rc = _plcIf.Read(_deviceType_MachineStatus, (uint)addr, count, ref val);
                if (STATUS_SUCCESS(rc))
                {
                    // アイル自動運転 可能
                    machineStatus.AisleAutoReady = (val[(int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_READY] == 1);
                    // アイル自動運転 運転中
                    machineStatus.AisleAutoRunning = (val[(int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_RUNNING] == 1);
                    // アイル自動運転 停止中
                    machineStatus.AisleAutoStop = (val[(int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_STOP] == 1);
                    // アイル自動運転 開始要求
                    machineStatus.AisleAutoRunningRequest = (val[(int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_RUNNING_REQUEST] == 1);
                    // アイル自動運転 停止要求
                    machineStatus.AisleAutoStopRequest = (val[(int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_STOP_REQUEST] == 1);

                    // 作業中の仕分登録No (L)
                    machineStatus.CurrentRegistNo_L = val[(int)PLC_ADDR_MACHINE_STATUS.CURRENT_REGIST_NO_L];
                    // 作業中の仕分登録No (R)
                    machineStatus.CurrentRegistNo_R = val[(int)PLC_ADDR_MACHINE_STATUS.CURRENT_REGIST_NO_R];

                    // 番重供給機ステータス (L)
                    machineStatus.SupplyUnitStatus_L = (SUPPLY_UNIT_STATUS)val[(int)PLC_ADDR_MACHINE_STATUS.SUPPLY_UNIT_STATUS_L];
                    // 番重供給機ステータス (R)
                    machineStatus.SupplyUnitStatus_R = (SUPPLY_UNIT_STATUS)val[(int)PLC_ADDR_MACHINE_STATUS.SUPPLY_UNIT_STATUS_R];
                    // 番重供給機エラーコード (L)
                    machineStatus.SupplyUnitErrorCode_L = (SUPPLY_UNIT_ERROR_CODE)val[(int)PLC_ADDR_MACHINE_STATUS.SUPPLY_UNIT_ERROR_CODE_L];
                    // 番重供給機エラーコード (R)
                    machineStatus.SupplyUnitErrorCode_R = (SUPPLY_UNIT_ERROR_CODE)val[(int)PLC_ADDR_MACHINE_STATUS.SUPPLY_UNIT_ERROR_CODE_R];

                    // ユニット01ステータス (L)
                    machineStatus.Unit01Status_L = (UNIT_STATUS)val[(int)PLC_ADDR_MACHINE_STATUS.UNIT01_STATUS_L];
                    // ユニット01ステータス (R)
                    machineStatus.Unit01Status_R = (UNIT_STATUS)val[(int)PLC_ADDR_MACHINE_STATUS.UNIT01_STATUS_R];
                    // ユニット02ステータス (L)
                    machineStatus.Unit02Status_L = (UNIT_STATUS)val[(int)PLC_ADDR_MACHINE_STATUS.UNIT02_STATUS_L];
                    // ユニット02ステータス (R)
                    machineStatus.Unit02Status_R = (UNIT_STATUS)val[(int)PLC_ADDR_MACHINE_STATUS.UNIT02_STATUS_R];
                    // ユニット03ステータス (L)
                    machineStatus.Unit03Status_L = (UNIT_STATUS)val[(int)PLC_ADDR_MACHINE_STATUS.UNIT03_STATUS_L];
                    // ユニット03ステータス (R)
                    machineStatus.Unit03Status_R = (UNIT_STATUS)val[(int)PLC_ADDR_MACHINE_STATUS.UNIT03_STATUS_R];

                    // ユニット01エラーコード (L)
                    machineStatus.Unit01ErrorCode_L = (UNIT_ERROR_CODE)val[(int)PLC_ADDR_MACHINE_STATUS.UNIT01_ERROR_CODE_L];
                    // ユニット01エラーコード (R)
                    machineStatus.Unit01ErrorCode_R = (UNIT_ERROR_CODE)val[(int)PLC_ADDR_MACHINE_STATUS.UNIT01_ERROR_CODE_R];
                    // ユニット02エラーコード (L)
                    machineStatus.Unit02ErrorCode_L = (UNIT_ERROR_CODE)val[(int)PLC_ADDR_MACHINE_STATUS.UNIT02_ERROR_CODE_L];
                    // ユニット02エラーコード (R)
                    machineStatus.Unit02ErrorCode_R = (UNIT_ERROR_CODE)val[(int)PLC_ADDR_MACHINE_STATUS.UNIT02_ERROR_CODE_R];
                    // ユニット03エラーコード (L)
                    machineStatus.Unit03ErrorCode_L = (UNIT_ERROR_CODE)val[(int)PLC_ADDR_MACHINE_STATUS.UNIT03_ERROR_CODE_L];
                    // ユニット03エラーコード (R)
                    machineStatus.Unit03ErrorCode_R = (UNIT_ERROR_CODE)val[(int)PLC_ADDR_MACHINE_STATUS.UNIT03_ERROR_CODE_R];

                    // スロットステータス
                    for (int i = 0; i < _dataSize_SlotStatus; i++)
                        machineStatus.SlotStatus[i] = (SLOT_STATUS)val[(int)PLC_ADDR_MACHINE_STATUS.SLOT_STATUS + i];
                    // スロットエラーコード
                    for (int i = 0; i < _dataSize_SlotStatus; i++)
                        machineStatus.SlotErrorCode[i] = (SLOT_ERROR_CODE)val[(int)PLC_ADDR_MACHINE_STATUS.SLOT_ERROR_CODE + i];
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally 
            {
                if(!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }
        #endregion

        #region 書き込み
        /// <summary>
        /// 書込み
        /// アイル自動運転 運転要求
        /// </summary>
        /// <returns></returns>
        public UInt32 SetAisleAutoRunningReq()
        {
            UInt32 rc = 0;
            int addr = 0;
            try
            {
                // 接続確認
                if (!_plcIf.IsConnected())
                {
                    Close();
                    Open(_connectionString);
                }

                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_STOP_REQUEST;
                rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, 0);
                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_RUNNING_REQUEST;
                rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, 1);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }
        /// <summary>
        /// 書込み
        /// アイル自動運転 停止要求
        /// <returns></returns>
        public UInt32 SetAisleAutoStopReq()
        {
            UInt32 rc = 0;
            int addr = 0;
            try
            {
                // 接続確認
                if (!_plcIf.IsConnected())
                {
                    Close();
                    Open(_connectionString);
                }

                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_RUNNING_REQUEST;
                rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, 0);
                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_STOP_REQUEST;
                rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, 1);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }
        /// <summary>
        /// エラー発生フラグ STOP 書込み
        /// </summary>
        /// <param name="plc"></param>
        /// <param name="allStatus">全ステータス読込</param>
        /// <param name="status"></param>
        /// <returns></returns>
        public UInt32 SetErrorFlgOFF()
        {
            UInt32 rc = 0;
            int startAddr = 0;
            int count = 0;
            int addr = 0;
            try
            {

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }
        #endregion

        #endregion


        #region -------- 仕分ステータス --------

        #region 読み出し
        /// <summary>
        /// 仕分ステータス 読み出し
        /// </summary>
        /// <param name="workIndex">1~10のどこか</param>
        /// <returns></returns>
        public UInt32 GetOrderStatus(int workIndex, out PLCWorkOrder orderStatus)
        {
            UInt32 rc = 0;
            orderStatus = new PLCWorkOrder();
            int addr = 0;
            int count = 0;
            try
            {
                // 接続確認
                if (!_plcIf.IsConnected())
                {
                    Close();
                    Open(_connectionString);
                }

                // ----------------------------------
                // 仕分情報
                // ----------------------------------
                addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex);
                count = _dataSize_Order;                      // 読み出しワード数
                int[] val = new int[count];
                rc = _plcIf.Read(_deviceType_OrderStatus, (uint)addr, count, ref val);
                if (STATUS_SUCCESS(rc)) 
                {
                    orderStatus.registryNo = val[(int)PLC_ADDR_ORDER_STATUS.REGISTRY_NO];

                    int MMdd = 0;
                    if (val[(int)PLC_ADDR_ORDER_STATUS.ORDER_DATE] == 0) MMdd = 0101;
                    else MMdd = val[(int)PLC_ADDR_ORDER_STATUS.ORDER_DATE];
                    orderStatus.orderDate = DateTime.ParseExact($"{(DateTime.Now).Year.ToString("D4")}{MMdd.ToString("D4")}000000", "yyyyMMddHHmmss", null);
                    orderStatus.postNo = val[(int)PLC_ADDR_ORDER_STATUS.POST_NO];
                    orderStatus.orderCountTotal = val[(int)PLC_ADDR_ORDER_STATUS.ORDER_COUNT_TOTAL];
                    orderStatus.orderCountCompTotal = val[(int)PLC_ADDR_ORDER_STATUS.ORDER_COUNT_COMP_TOTAL];
                    orderStatus.sequence = (PLC_ORDER_SEQUENCE)val[(int)PLC_ADDR_ORDER_STATUS.SEQUENCE];
                }

                // ----------------------------------
                // JANコード
                // ----------------------------------
                if (STATUS_SUCCESS(rc)) 
                {
                    string janCode = "";
                    addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.JAN_CODE;
                    count = _dataSize_JANCode;
                    rc = _plcIf.Read(_deviceType_OrderStatus, (uint)addr, count, ref janCode);
                    if (STATUS_SUCCESS(rc)) 
                        orderStatus.JANCode = janCode;
                }

                // ----------------------------------
                // 商品名(カナ)
                // ----------------------------------
                if (STATUS_SUCCESS(rc)) 
                {
                    string workName = "";
                    addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.WORK_NAME;
                    count = _dataSize_WorkName;
                    rc = _plcIf.Read(_deviceType_OrderStatus, (uint)addr, count, ref workName);
                    if (STATUS_SUCCESS(rc))
                        orderStatus.workName = workName;
                }

                // ----------------------------------
                // 仕分数量
                // ----------------------------------
                if (STATUS_SUCCESS(rc)) 
                {
                    addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.ORDER_COUNT_SLOT;
                    count = _dataSize_OrderCountSlot;
                    int[] orderCount = new int[count];
                    rc = _plcIf.Read(_deviceType_OrderStatus, (uint)addr, count, ref orderCount);
                    if (STATUS_SUCCESS(rc))
                    { 
                        for (int i = 0; i < _dataSize_OrderCountSlot; i++)
                        orderStatus.orderCount[i] = orderCount[i];
                    }
                }

                // ----------------------------------
                // 仕分済数量
                // ----------------------------------
                if (STATUS_SUCCESS(rc)) 
                {
                    addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.ORDER_COUNT_COMP_SLOT;
                    count = _dataSize_OrderCompCountSlot;
                    int[] orderCountComp = new int[count];
                    rc = _plcIf.Read(_deviceType_OrderStatus, (uint)addr, count, ref orderCountComp);
                    if (STATUS_SUCCESS(rc)) 
                    {
                        for (int i = 0; i < _dataSize_OrderCompCountSlot; i++)
                            orderStatus.orderCountComp[i] = orderCountComp[i];
                    }
                }

                // ----------------------------------
                // ワーク取出対象スロット
                // ----------------------------------
                if (STATUS_SUCCESS(rc)) 
                {
                    addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.TARGET_SLOT;
                    count = _dataSize_TargetSlot;
                    int[] targetSlot = new int[count];
                    rc = _plcIf.Read(_deviceType_OrderStatus, (uint)addr, count, ref targetSlot);
                    if (STATUS_SUCCESS(rc)) 
                    {
                        for (int i = 0; i < _dataSize_TargetSlot; i++)
                            orderStatus.targetSlot[i] = targetSlot[i];
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}"); 
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }
        #endregion

        #region 書き込み
        /// <summary>
        /// 仕分ステータス クリア 書込み
        /// </summary>
        public UInt32 SetOrderStatusClear(int workIndex)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                // 接続確認
                if (!_plcIf.IsConnected())
                {
                    Close();
                    Open(_connectionString);
                }


                addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex);
                //count = _dataSize_OrderStatus_Total;
                //count = 10;                      // 読み出しワード数
                count = 300;                      // 読み出しワード数
                int[] val = new int[count];
                rc = _plcIf.Write(_deviceType_OrderStatus, (uint)addr, count, val);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }

        /// <summary>
        /// 仕分日(MMdd) 書込み
        /// </summary>
        public UInt32 SetOrderDate(int workIndex, int orderDate)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.ORDER_DATE;
                rc = _plcIf.Write(_deviceType_OrderStatus, (uint)addr, orderDate);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }
        /// <summary>
        /// 仕分便No 書込み
        /// </summary>
        public UInt32 SetOrderPostNo(int workIndex, int postNo)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.POST_NO;
                rc = _plcIf.Write(_deviceType_OrderStatus, (uint)addr, postNo);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }
        /// <summary>
        /// 総仕分数量 書込み
        /// </summary>
        public UInt32 SetOrderCountTotal(int workIndex, int orderCountTotal)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.ORDER_COUNT_TOTAL;
                rc = _plcIf.Write(_deviceType_OrderStatus, (uint)addr, orderCountTotal);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }
        /// <summary>
        /// 総仕分済数量 書込み
        /// </summary>
        public UInt32 SetOrderCompCountTotal(int workIndex, int orderCompCountTotal)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.ORDER_COUNT_COMP_TOTAL;
                rc = _plcIf.Write(_deviceType_OrderStatus, (uint)addr, orderCompCountTotal);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }
        /// <summary>
        /// 仕分実行フラグ 書込み
        /// </summary>
        public UInt32 SetOrderSequence(int workIndex, PLC_ORDER_SEQUENCE sequence)
        {
            UInt32 rc = 0;
            int addr = 0;
            try
            {
                addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.SEQUENCE;
                rc = _plcIf.Write(_deviceType_OrderStatus, (uint)addr, (int)sequence);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }
        /// <summary>
        /// 商品名(ｶﾅ) 書込み
        /// </summary>
        public UInt32 SetWorkNameKana(int workIndex, string workNameKana)
        {
            UInt32 rc = 0;
            int addr = 0;
            try
            {
                addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.WORK_NAME;
                rc = _plcIf.Write(_deviceType_OrderStatus, (uint)addr, workNameKana);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }
        /// <summary>
        /// ケース入数(パック入数) 書込み
        /// </summary>
        public UInt32 SetCaseVolume(int workIndex, int caseVolume)
        {
            UInt32 rc = 0;
            int addr = 0;
            try
            {
                addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.CASE_VOLUME;
                rc = _plcIf.Write(_deviceType_OrderStatus, (uint)addr, caseVolume);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }
        /// <summary>
        /// 仕分数量(スロット別) 書込み
        /// </summary>
        public UInt32 SetOrderCountSlot(int workIndex, int[] slotOrderCount)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                int[] array = new int[_dataSize_OrderCountSlot];
                for (int i = 0; i < _dataSize_OrderCountSlot; i++)
                {
                    if (i > slotOrderCount.Length - 1)
                        break;
                    array[i] = slotOrderCount[i];
                }

                addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.ORDER_COUNT_SLOT;
                count = _dataSize_OrderCountSlot;
                rc = _plcIf.Write(_deviceType_OrderStatus, (uint)addr, count, array);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }
        /// <summary>
        /// 仕分済数量(スロット別) 書込み
        /// </summary>
        public UInt32 SetOrderCompCountSlot(int workIndex, int[] slotOrderCompCount)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                int[] array = new int[_dataSize_OrderCompCountSlot];
                for (int i = 0; i < _dataSize_OrderCompCountSlot; i++)
                {
                    if (i > slotOrderCompCount.Length - 1)
                        break;
                    array[i] = slotOrderCompCount[i];
                }

                addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.ORDER_COUNT_COMP_SLOT;
                count = _dataSize_OrderCompCountSlot;
                rc = _plcIf.Write(_deviceType_OrderStatus, (uint)addr, count, array);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }
        /// <summary>
        /// 仕分数量(ロボット別) 書込み
        /// </summary>
        public UInt32 SetOrderCountRobot(int workIndex, int[] robotOrderCount)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.ORDER_COUNT_ROBOT;
                //count = _dataSize_OrderCount;
                count = _dataSize_OrderCountRobot;
                rc = _plcIf.Write(_deviceType_OrderStatus, (uint)addr, count, robotOrderCount);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }
        #endregion

        #endregion



        #region デバッグ (本来PLCが内部で実行する処理)
        /// <summary>
        /// デバッグ
        /// アイル自動運転 可能
        /// </summary>
        public UInt32 Debug_SetAisleAutoReady(bool ready)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                // クリア
                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_RUNNING_REQUEST;
                rc = _plcIf.Write(DeviceType.DataMemory, (uint)addr, 0);
                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_STOP_REQUEST;
                rc = _plcIf.Write(DeviceType.DataMemory, (uint)addr, 0);
                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_RUNNING;
                rc = _plcIf.Write(DeviceType.DataMemory, (uint)addr, 0);
                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_STOP;
                rc = _plcIf.Write(DeviceType.DataMemory, (uint)addr, 0);

                if (ready) 
                {
                    // 可能
                    addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_READY;
                    rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, 1);
                }
                else
                {
                    // 不可
                    addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_READY;
                    rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, 0);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// デバッグ
        /// アイル自動運転 運転
        /// </summary>
        public UInt32 Debug_SetAisleAutoRunning()
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                // クリア
                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_RUNNING_REQUEST;
                rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, 0);
                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_STOP_REQUEST;
                rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, 0);
                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_STOP;
                rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, 0);

                // 運転
                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_RUNNING;
                rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, 1);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// デバッグ
        /// アイル自動運転 停止
        /// </summary>
        public UInt32 Debug_SetAisleAutoStop()
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                // クリア
                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_RUNNING_REQUEST;
                rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, 0);
                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_STOP_REQUEST;
                rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, 0);
                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_RUNNING;
                rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, 0);

                // 停止
                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.AISLE_AUTO_STOP;
                rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, 1);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// デバッグ
        /// 作業中仕分登録No L セット
        /// </summary>
        public UInt32 Debug_SetCurrentRegistNoL(int no)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.CURRENT_REGIST_NO_L;
                rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, no);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// デバッグ
        /// 作業中仕分登録No R セット
        /// </summary>
        public UInt32 Debug_SetCurrentRegistNoR(int no)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.CURRENT_REGIST_NO_R;
                rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, no);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        /// <summary>
        /// デバッグ
        /// 番重供給ユニットステータス セット
        /// </summary>
        public UInt32 Debug_SetSupplyUnitStatus(SUPPLY_UNIT_STATUS status)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.SUPPLY_UNIT_STATUS_L;
                rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, (int)status);
                addr = _startAddr_MachineStatus + (int)PLC_ADDR_MACHINE_STATUS.SUPPLY_UNIT_STATUS_R;
                rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, (int)status);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// デバッグ
        /// ユニットステータス セット
        /// </summary>
        public UInt32 Debug_SetUnitStatus(UNIT_STATUS status, bool[] unitEnable)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                for (int i = 0; i < Const.MaxUnitCount; i++)
                {
                    if (unitEnable[i]) 
                    {
                        addr = _startAddr_MachineStatus + ((int)PLC_ADDR_MACHINE_STATUS.UNIT01_STATUS_L + (i * 2));
                        rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, (int)status);
                        addr = _startAddr_MachineStatus + ((int)PLC_ADDR_MACHINE_STATUS.UNIT01_STATUS_R + (i * 2));
                        rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, (int)status);
                    }
                }

            }
            catch (Exception ex)
            {
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// デバッグ
        /// スロットステータス セット
        /// </summary>
        public UInt32 Debug_SetSlotStatus(SLOT_STATUS status, bool[] unitEnable)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                for (int i = 0; i < Const.MaxUnitCount; i++)
                {
                    if (unitEnable[i])
                    {
                        for (int j = 0; j < Const.MaxSlotCount; j++)
                        {
                            addr = _startAddr_MachineStatus + ((int)PLC_ADDR_MACHINE_STATUS.SLOT_STATUS + (i * Const.MaxSlotCount) + j);
                            rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, (int)status);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }


        /// <summary>
        /// デバッグ
        /// 仕分登録No 書込み
        /// </summary>
        public UInt32 Debug_SetRegistNo(int workIndex, int registNo)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.REGISTRY_NO;
                rc = _plcIf.Write(_deviceType_OrderStatus, (uint)addr, registNo);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// デバッグ
        /// 商品名(ｶﾅ)/JANコード 書込み
        /// </summary>
        public UInt32 Debug_SetWorkName(int workIndex, string workNameKana, string janCode)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                // 商品名(ｶﾅ)
                addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.WORK_NAME;
                rc = _plcIf.Write(_deviceType_OrderStatus, (uint)addr, workNameKana);
                // JANコード
                addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.JAN_CODE;
                rc = _plcIf.Write(_deviceType_OrderStatus, (uint)addr, janCode);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// デバッグ
        /// 仕分完了数(トータル) 書込み
        /// </summary>
        public UInt32 Debug_SetOrderCompCountTotal(int workIndex, int compCount)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.ORDER_COUNT_COMP_TOTAL;
                rc = _plcIf.Write(_deviceType_OrderStatus, (uint)addr, compCount);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// デバッグ
        /// 仕分完了数(スロット) 書込み
        /// </summary>
        public UInt32 Debug_SetOrderCompCount(int workIndex, int slotNo, int compCount)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.ORDER_COUNT_COMP_SLOT + (slotNo - 1);
                rc = _plcIf.Write(_deviceType_OrderStatus, (uint)addr, compCount);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// デバッグ
        /// 取出対象スロット 書込み
        /// </summary>
        public UInt32 Debug_SetOrderTargetSlot(int workIndex, int[] targetSlotNo)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                addr = _startAddr_OrderStatus + (_dataSize_OrderStatus_Total * workIndex) + (int)PLC_ADDR_ORDER_STATUS.TARGET_SLOT;
                count = _dataSize_TargetSlot;
                rc = _plcIf.Write(_deviceType_OrderStatus, (uint)addr, count, targetSlotNo);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        /// <summary>
        /// デバッグ
        /// クリア
        /// </summary>
        public UInt32 Debug_Clear()
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            int[] val = null;
            try
            {
                // 設備ステータス
                val = null;
                val = new int[_dataSize_MachineStatus_Total];
                addr = _startAddr_MachineStatus;
                count = _dataSize_MachineStatus_Total;
                rc = _plcIf.Write(_deviceType_MachineStatus, (uint)addr, count, val);

                // 仕分ステータス
                val = null;
                val = new int[_dataSize_OrderStatus_Total];
                for (int i = 0; i < Const.MaxWorkRegisterCount; i++) 
                {
                    addr = (_dataSize_OrderStatus_Total * i) + _startAddr_OrderStatus;
                    count = _dataSize_OrderStatus_Total;
                    rc = _plcIf.Write(_deviceType_OrderStatus, (uint)addr, count, val);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        #endregion




        /// <summary>
        /// Check Error State
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        private static bool STATUS_SUCCESS(UInt32 err)
        {
            return err == (int)ErrorCodeList.STATUS_SUCCESS;
        }

    }

}
