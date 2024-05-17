//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;

using SystemConfig;
using DL_CommonLibrary;


namespace ShareResource
{
    /// <summary>
    /// 共有ステータス
    /// </summary>
    public class CommonStatus
    {
        /// <summary>
        /// PC名
        /// </summary>
        public string MyPcName = "";
        /// <summary>
        /// 権限タイプ
        /// </summary>
        public AUTHORITY Authority = AUTHORITY.OPERATOR;

        /// <summary>
        /// 現在日時
        /// </summary>
        public DateTime CurrentDateTime = DateTime.MinValue;

        /// <summary>
        /// 運転/停止
        /// </summary>
        public AUTO_STATUS AutoStatus = AUTO_STATUS.STOP;
        /// <summary>
        /// 自動/手動 (各アイル)
        /// </summary>
        public OPERATION_TYPE[] OperationType = null;
        /// <summary>
        /// サイクルステータス (各アイル)
        /// </summary>
        public CYCLE_STATUS[] CycleStatus = null;
        ///// <summary>
        ///// アイルステータス (各アイル)
        ///// </summary>
        //public AISLE_STATUS[] AisleStatus = null;
        ///// <summary>
        ///// スロットステータス (各アイル)
        ///// </summary>
        //public SLOT_STATUS[] SlotStatus = null;
        /// <summary>
        /// 現在の便Index (各アイル)
        /// </summary>
        public int CurrentPostIndex = 0;
        /// <summary>
        /// 現在のバッチIndex (各アイル)
        /// </summary>
        public int[] CurrentBatchIndex = null;


        /// <summary>
        /// 初期化完了フラグ
        /// </summary>
        public bool initialize_Completed = false;
        ///// <summary>
        ///// 初回サーバーデータ読み込み完了
        ///// </summary>
        //public bool Load_Server_complete = false;

        /// <summary>
        /// 接続確認中フラグ
        /// </summary>
        public bool ConnectionCheckRunning = false;
        /// <summary>
        /// IPアドレス
        /// </summary>
        public string MyIpAddress = "";
        /// <summary>
        /// PLC PING接続
        /// </summary>
        public bool[] PlcPingConnection = null;
        /// <summary>
        /// Server PING接続
        /// </summary>
        public bool ServerPingConnection = false;

        ///// <summary>
        ///// 当日のPICKDATA読込済フラグ
        ///// </summary>
        //public bool[] IsLoadedTodayPickData = null;
        /// <summary>
        /// 当日の商品マスター読込済フラグ
        /// </summary>
        public bool IsLoadedTodayMasterWork = false;
        /// <summary>
        /// 当日店マスター読込済フラグ
        /// </summary>
        public bool IsLoadedTodayMasterStore = false;
        /// <summary>
        /// 当日の作業者マスター読込済フラグ
        /// </summary>
        public bool IsLoadedTodayMasterWorker = false;

        /// <summary>
        /// サーバー ロックオブジェクト
        /// </summary>
        public object Lock_Server = new object();

        /// <summary>
        /// メンテナンスモード
        /// </summary>
        public bool MaintenanceMode = false;







        /// <summary>
        /// 画面下に表示するアラームメッセージ
        /// </summary>
        public string alarmMessage = "";
        /// <summary>
        /// 画面上に黄色で表示されるメッセージ
        /// </summary>
        public string popupMessage = "";

        /// <summary>
        /// 500ms クロック
        /// 500msec間隔でON/OFFを繰り返す
        /// </summary>
        public bool systemClock_500ms = false;
        /// <summary>
        /// 1000ms クロック
        /// 1000msec間隔でON/OFFを繰り返す
        /// </summary>
        public bool systemClock_1000ms = false;
        /// <summary>
        /// 起動3秒後にTRUE
        /// </summary>
        public bool ondelay_3000msec = false;

        /// <summary>
        /// エラーステータス
        /// </summary>
        public ErrorStatus Error = new ErrorStatus();


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommonStatus()
        {
            OperationType = new OPERATION_TYPE[Const.MaxAisleCount];
            for (int i = 0; i < Const.MaxAisleCount; i++)
                OperationType[i] = OPERATION_TYPE.AUTO;

            CycleStatus = new CYCLE_STATUS[Const.MaxAisleCount];
            for (int i = 0; i < Const.MaxAisleCount; i++)
                CycleStatus[i] = CYCLE_STATUS.None;
            CurrentBatchIndex = new int[Const.MaxAisleCount];

            PlcPingConnection = new bool[Const.MaxAisleCount];
            for(int i = 0; i < Const.MaxAisleCount; i++)
                PlcPingConnection[i] = false;

            //IsLoadedTodayPickData = new bool[Const.MaxPostCount];
            //for (int i = 0; i < Const.MaxPostCount; i++)
            //    IsLoadedTodayPickData[i] = false;
        }

        /// <summary>
        /// エラー有無確認
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        private bool STATUS_SUCCESS(UInt32 rc)
        {
            return rc == 0;
        }

    }
}
