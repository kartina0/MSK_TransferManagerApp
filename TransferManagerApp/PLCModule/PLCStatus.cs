//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using SystemConfig;

namespace PLCModule
{

    /// <summary>
    /// PLCステータス
    /// </summary>
    public class PLCStatus
    {
        

    }



    public class PLCMachineStatus 
    {
        /// <summary>
        /// PLCが1秒に1インクリメントする生存確認用デバイス
        /// </summary>
        public int LifeCounter = 0;

        /// <summary>
        /// 接続状態
        /// </summary>
        public bool IsConnected = false;

        /// <summary>
        /// アイル自動運転 可能
        /// </summary>
        public bool AisleAutoReady = false;
        /// <summary>
        /// アイル自動運転 運転中
        /// </summary>
        public bool AisleAutoRunning = false;
        /// <summary>
        /// アイル自動運転 停止中
        /// </summary>
        public bool AisleAutoStop = false;
        /// <summary>
        /// アイル自動運転 運転要求
        /// </summary>
        public bool AisleAutoRunningRequest = false;
        /// <summary>
        /// アイル自動運転 停止要求
        /// </summary>
        public bool AisleAutoStopRequest = false;

        /// <summary>
        /// 作業中の仕分登録No (L)
        /// </summary>
        public int CurrentRegistNo_L = 0;
        /// <summary>
        /// 作業中の仕分登録No (R)
        /// </summary>
        public int CurrentRegistNo_R = 0;

        /// <summary>
        /// ユニット01ステータス (L)
        /// </summary>
        public UNIT_STATUS Unit01Status_L = UNIT_STATUS.NONE;
        /// <summary>
        /// ユニット01ステータス (R)
        /// </summary>
        public UNIT_STATUS Unit01Status_R = UNIT_STATUS.NONE;
        /// <summary>
        /// ユニット02ステータス (L)
        /// </summary>
        public UNIT_STATUS Unit02Status_L = UNIT_STATUS.NONE;
        /// <summary>
        /// ユニット02ステータス (R)
        /// </summary>
        public UNIT_STATUS Unit02Status_R = UNIT_STATUS.NONE;
        /// <summary>
        /// ユニット03ステータス (L)
        /// </summary>
        public UNIT_STATUS Unit03Status_L = UNIT_STATUS.NONE;
        /// <summary>
        /// ユニット03ステータス (R)
        /// </summary>
        public UNIT_STATUS Unit03Status_R = UNIT_STATUS.NONE;
        /// <summary>
        /// ユニット01エラーコード (L)
        /// </summary>
        public UNIT_ERROR_CODE Unit01ErrorCode_L = UNIT_ERROR_CODE.NONE;
        /// <summary>
        /// ユニット01エラーコード (R)
        /// </summary>
        public UNIT_ERROR_CODE Unit01ErrorCode_R = UNIT_ERROR_CODE.NONE;
        /// <summary>
        /// ユニット02エラーコード (L)
        /// </summary>
        public UNIT_ERROR_CODE Unit02ErrorCode_L = UNIT_ERROR_CODE.NONE;
        /// <summary>
        /// ユニット02エラーコード (R)
        /// </summary>
        public UNIT_ERROR_CODE Unit02ErrorCode_R = UNIT_ERROR_CODE.NONE;
        /// <summary>
        /// ユニット03エラーコード (L)
        /// </summary>
        public UNIT_ERROR_CODE Unit03ErrorCode_L = UNIT_ERROR_CODE.NONE;
        /// <summary>
        /// ユニット03エラーコード (R)
        /// </summary>
        public UNIT_ERROR_CODE Unit03ErrorCode_R = UNIT_ERROR_CODE.NONE;

        /// <summary>
        /// スロットステータス
        /// </summary>
        public SLOT_STATUS[] SlotStatus = null;
        /// <summary>
        /// スロットエラーコード
        /// </summary>
        public SLOT_ERROR_CODE[] SlotErrorCode = null;

        /// <summary>
        /// 番重供給機ステータス (L)
        /// </summary>
        public SUPPLY_UNIT_STATUS SupplyUnitStatus_L = SUPPLY_UNIT_STATUS.NONE;
        /// <summary>
        /// 番重供給機ステータス (R)
        /// </summary>
        public SUPPLY_UNIT_STATUS SupplyUnitStatus_R = SUPPLY_UNIT_STATUS.NONE;
        /// <summary>
        /// 番重供給機エラーコード (L)
        /// </summary>
        public SUPPLY_UNIT_ERROR_CODE SupplyUnitErrorCode_L = SUPPLY_UNIT_ERROR_CODE.NONE;
        /// <summary>
        /// 番重供給機エラーコード (R)
        /// </summary>
        public SUPPLY_UNIT_ERROR_CODE SupplyUnitErrorCode_R = SUPPLY_UNIT_ERROR_CODE.NONE;

        /// <summary>
        /// エラー発生フラグ
        /// </summary>
        public bool ErrorFlg = false;
        /// <summary>
        /// エラー種別
        /// </summary>
        public PLC_ERROR_TYPE ErrorType = PLC_ERROR_TYPE.NONE;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PLCMachineStatus() 
        {
            SlotStatus = new SLOT_STATUS[Const.MaxUnitCount * Const.MaxSlotCount];
            SlotErrorCode = new SLOT_ERROR_CODE[Const.MaxUnitCount * Const.MaxSlotCount];
            for (int i = 0; i < Const.MaxUnitCount * Const.MaxSlotCount; i++) 
            {
                SlotStatus[i] = SLOT_STATUS.NONE;
                SlotErrorCode[i] = SLOT_ERROR_CODE.NONE;
            }
        }

        /// <summary>
        /// 読み込み
        /// </summary>
        public UInt32 Read(PLCMachineStatus status)
        {
            UInt32 rc = 0;
            try
            {
                LifeCounter = status.LifeCounter;
                IsConnected = status.IsConnected;
                AisleAutoReady = status.AisleAutoReady;
                AisleAutoRunning = status.AisleAutoRunning;
                AisleAutoStop = status.AisleAutoStop;
                AisleAutoRunningRequest = status.AisleAutoRunningRequest;
                AisleAutoStopRequest = status.AisleAutoStopRequest;

                CurrentRegistNo_L = status.CurrentRegistNo_L;
                CurrentRegistNo_R = status.CurrentRegistNo_R;

                Unit01Status_L = status.Unit01Status_L;
                Unit01Status_R = status.Unit01Status_R;
                Unit02Status_L = status.Unit02Status_L;
                Unit02Status_R = status.Unit02Status_R;
                Unit03Status_L = status.Unit03Status_L;
                Unit03Status_R = status.Unit03Status_R;
                Unit01ErrorCode_L = status.Unit01ErrorCode_L;
                Unit01ErrorCode_R = status.Unit01ErrorCode_R;
                Unit02ErrorCode_L = status.Unit02ErrorCode_L;
                Unit02ErrorCode_R = status.Unit02ErrorCode_R;
                Unit03ErrorCode_L = status.Unit03ErrorCode_L;
                Unit03ErrorCode_R = status.Unit03ErrorCode_R;

                for (int i = 0; i < Const.MaxUnitCount * Const.MaxSlotCount; i++) 
                {
                    SlotStatus[i] = status.SlotStatus[i];
                    SlotErrorCode[i] = status.SlotErrorCode[i];
                }

                SupplyUnitStatus_L = status.SupplyUnitStatus_L;
                SupplyUnitStatus_R = status.SupplyUnitStatus_R;
                SupplyUnitErrorCode_L = status.SupplyUnitErrorCode_L;
                SupplyUnitErrorCode_R = status.SupplyUnitErrorCode_R;

                ErrorFlg = status.ErrorFlg;
                ErrorType = status.ErrorType;

            }
            catch (Exception ex)
            {

            }
            return rc;
        }


    }

    /// <summary>
    /// 1商品の仕分情報
    /// </summary>
    public class PLCWorkOrder
    {
        /// <summary>
        /// 仕分登録No
        /// </summary>
        public int registryNo = 0;
        /// <summary>
        /// 仕分日(MMdd)
        /// </summary>
        public DateTime orderDate = DateTime.MinValue;
        /// <summary>
        /// 仕分便No
        /// </summary>
        public int postNo = 0;
        /// <summary>
        /// 総仕分数量
        /// </summary>
        public int orderCountTotal = 0;
        /// <summary>
        /// 総投入数量
        /// </summary>
        public int orderCountInputTotal = 0;
        /// <summary>
        /// 総仕分済数量
        /// </summary>
        public int orderCountCompTotal = 0;
        /// <summary>
        /// 仕分け情報
        /// </summary>
        public PLC_ORDER_SEQUENCE sequence = PLC_ORDER_SEQUENCE.NOT_REGISTER;

        ///// <summary>
        ///// 商品コード
        ///// </summary>
        //public string workCode = "";
        /// <summary>
        /// JANコード
        /// </summary>
        public string JANCode = "";
        /// <summary>
        /// 商品名(カナ)
        /// </summary>
        public string workName = "";
        /// <summary>
        /// 仕分数量(スロットごと)
        /// </summary>
        public int[] orderCount = new int[36];
        /// <summary>
        /// 仕分済数量(スロットごと)
        /// </summary>
        public int[] orderCountComp = new int[36];
        /// <summary>
        /// ワーク取出対象スロット(ワーク個数分)
        /// </summary>
        public int[] targetSlot = new int[999];

    }


    /// <summary>
    /// PLC仕分け実行フラグ
    /// </summary>
    public enum PLC_ORDER_SEQUENCE
    {
        /// <summary>未登録</summary>
        NOT_REGISTER = 0,
        /// <summary>登録要求</summary>
        REQ_REGISTER,
        /// <summary>登録完了</summary>
        COMP_REGISTER,
        /// <summary>仕分開始</summary>
        START_ORDER,
        /// <summary>仕分完了(途中キャンセル)</summary>
        CANCEL_ORDER,
        /// <summary>仕分完了</summary>
        COMP_ORDER,
        /// <summary>登録エラー</summary>
        REGISTER_ERROR = 99,
    }
    /// <summary>
    /// PLCエラー種別
    /// </summary>
    public enum PLC_ERROR_TYPE
    {
        /// <summary>None</summary>
        NONE = 0,
        /// <summary>搬送エラー</summary>
        ORDER_ERROR,
    }


}
