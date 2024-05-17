//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System.Text;
using System.Drawing;


namespace SystemConfig
{
    /// <summary>
    /// 定数定義
    /// </summary>
    public static class Const
    {
        /// <summary>
        /// Application Title
        /// </summary>
        public const string Title = "TransferManagerApp";
        /// <summary>
        /// 対応INIファイルバージョン
        /// </summary>
        public const int IniFileVersion = 1;
        /// <summary>
        /// INIファイル格納フォルダ
        /// </summary>
        public const string IniDir = ".\\Ini";
        /// <summary>
        /// INIファイル名
        /// </summary>
        public const string IniFileName = "TransferManagerApp.INI";
        /// <summary>
        /// PreStatusファイル名
        /// </summary>
        public const string PreStatusFileName = "PreStatus.INI";
        /// <summary>
        /// エラー履歴最大行数
        /// </summary>
        public const int ErrorHistorymxCount = 200;

        /// <summary>
        /// 便最大数
        /// </summary>
        public const int MaxPostCount = 3;
        /// <summary>
        /// ステーション最大数
        /// </summary>
        public const int MaxStationCount = 9;
        /// <summary>
        /// アイル最大数 (1ステーションあたり)
        /// </summary>
        public const int MaxAisleCount = 4;
        /// <summary>
        /// ユニット最大数 (1アイルあたり)
        /// </summary>
        public const int MaxUnitCount = 3;
        /// <summary>
        /// スロット最大数 (1ユニットあたり)
        /// </summary>
        public const int MaxSlotCount = 12;
        /// <summary>
        /// バッチ最大数 (1アイル辺り)
        /// </summary>
        public const int MaxBatchCount = 2;
        /// <summary>
        /// PLCワーク登録最大数
        /// </summary>
        public const int MaxWorkRegisterCount = 10;

        /// <summary>
        /// キーボード入力 キー数
        /// </summary>
        public const int KeyCount = 12;


        /// <summary>
        /// ダミー用の接続パラメータ
        /// ※生育はフロアに複数あるためINIファイルではなくこれを使用している
        /// </summary>
        public const string SocketConnection_Dummy_PLC = "DUMMY;PLC_{0}";




        #region "ログ関連"
        /// <summary>
        /// システムログ ファイル名
        /// </summary>
        public const string FilePrefix_SystemLog = "System.log";

        /// <summary>
        /// アラーム履歴ログ ファイル名
        /// </summary>
        public const string FilePrefix_AlarmLog = "Alarm.log";

        /// <summary>
        /// 履歴ログ ファイル名
        /// </summary>
        public const string FilePrefix_HistoryLog = "History.log";
        /// <summary>
        /// 操作ログ ファイル名
        /// </summary>
        public const string FilePrefix_OperationLog = "Operation.log";

        /// <summary>
        /// ログファイルの最大行数
        /// </summary>
        public const int LogMaxLineCount = 100000;

        /// <summary>
        /// ログファイルの最大数
        /// </summary>
        public const int LogMaxFileCount = 30;
        #endregion

        #region "メッセージ文字色"
        /// <summary>
        /// エラーメッセージ文字色
        /// </summary>
        public static System.Drawing.Color MsgForeColor_Error = Color.Red;
        /// <summary>
        /// エラーメッセージ文字色
        /// </summary>
        public static Color MsgForeColor_Normal = Color.Lime;

        #endregion

        #region "テキスト背景色"
        /// <summary>
        /// エラーメッセージ文字色
        /// </summary>
        public static Color TextBoxBackColor_Error = Color.Red;
        /// <summary>
        /// エラーメッセージ文字色
        /// </summary>
        public static Color TextBoxBackColor_Normal = Color.White;
        #endregion

    }



    /// <summary>
    /// 権限タイプ
    /// </summary>
    public enum AUTHORITY
    {
        /// <summary> 作業者 </summary>
        OPERATOR  = 0,
        /// <summary> 管理者 </summary>
        MANAGER = 1,
        /// <summary> 開発者 </summary>
        DEVELOPER = 2,
    }


    /// <summary>
    /// 自動/手動
    /// </summary>
    public enum OPERATION_TYPE
    {
        AUTO = 0,
        MANUAL = 1,
    }
    /// <summary>
    /// 運転/停止
    /// </summary>
    public enum AUTO_STATUS
    {
        RUNNING,
        STOP,
    }
    /// <summary>
    /// サイクルステータス
    /// </summary>
    public enum CYCLE_STATUS
    {
        None = 0,
        WAITING,
        PICKING,
        COMP,
    }
    ///// <summary>
    ///// アイルステータス
    ///// </summary>
    //public enum AISLE_STATUS
    //{
    //    /// <summary> NONE </summary>
    //    NONE = 0,
    //    /// <summary> 異常 </summary>
    //    ERROR,
    //    /// <summary> 正常 </summary>
    //    OK,
    //    /// <summary> 停止中 </summary>
    //    OFF,
    //    /// <summary> 仕分開始待ち </summary>
    //    WAITING,
    //    /// <summary> 仕分中 </summary>
    //    PICKING,
    //    /// <summary> 仕分完了 </summary>
    //    COMP,
    //    /// <summary> 店舗入替中 </summary>
    //    SLOT_CHANGE,
    //}
    /// <summary>
    /// ユニットステータス
    /// </summary>
    public enum UNIT_STATUS
    {
        /// <summary> NONE </summary>
        NONE = 0,
        /// <summary> 正常 </summary>
        NORMAL,
        /// <summary> 停止中 </summary>
        STOP,
        /// <summary> ティーチング </summary>
        TEACHING,
        /// <summary> 異常 </summary>
        ERROR = 10,
    }
    ///// <summary>
    ///// ロボットステータス
    ///// </summary>
    //public enum ROBOT_STATUS
    //{
    //    /// <summary> NONE </summary>
    //    NONE = 0,
    //    /// <summary> 異常 </summary>
    //    ERROR,
    //    /// <summary> 正常 </summary>
    //    OK,
    //    /// <summary> 停止中 </summary>
    //    OFF,
    //    /// <summary> ティーチング中 </summary>
    //    TEACHING,
    //    /// <summary> ハンド交換中 </summary>
    //    HAND_CHANGE,
    //}
    /// <summary>
    /// 番重供給機ステータス
    /// </summary>
    public enum SUPPLY_UNIT_STATUS
    {
        /// <summary> NONE </summary>
        NONE = 0,
        /// <summary> 正常 </summary>
        NORMAL,
        /// <summary> 停止中 </summary>
        STOP,
        /// <summary> 空番重無し </summary>
        EMPTY,
        /// <summary> 異常 </summary>
        ERROR = 10,
    }
    /// <summary>
    /// スロットステータス
    /// </summary>
    public enum SLOT_STATUS
    {
        /// <summary> NONE </summary>
        NONE = 0,
        /// <summary> 正常 </summary>
        NORMAL,
        /// <summary> 停止中 </summary>
        STOP,
        /// <summary> 番重満 </summary>
        FULL,
        /// <summary> 番重交換中 </summary>
        CHANGING,
        /// <summary> 満杯 </summary>
        RACK_FULL,
        /// <summary> 異常 </summary>
        ERROR = 10,
    }

    /// <summary>
    /// アイルエラーコード
    /// </summary>
    public enum AISLE_ERRORCODE
    {
        /// <summary> NONE </summary>
        NONE = 0,
    }
    /// <summary>
    /// ユニットエラーコード
    /// </summary>
    public enum UNIT_ERROR_CODE
    {
        /// <summary> NONE </summary>
        NONE = 0,
    }
    /// <summary>
    /// ロボットエラーコード
    /// </summary>
    public enum ROBOT_ERRORCODE
    {
        /// <summary> NONE </summary>
        NONE = 0,
    }
    /// <summary>
    /// 番重供給機エラーコード
    /// </summary>
    public enum SUPPLY_UNIT_ERROR_CODE
    {
        /// <summary> NONE </summary>
        NONE = 0,
    }
    /// <summary>
    /// スロットエラーコード
    /// </summary>
    public enum SLOT_ERROR_CODE
    {
        /// <summary> NONE </summary>
        NONE = 0,
    }

    /// <summary>
    /// 仕分データ形式
    /// </summary>
    public enum ORDER_INFO_TYPE
    {
        /// <summary> DB形式 </summary>
        DB = 0,
        /// <summary> CSV形式 </summary>
        CSV = 1,
    }
    /// <summary>
    /// 仕分け作業状況
    /// </summary>
    public enum ORDER_PROCESS
    {
        NONE = -1,
        /// <summary> 未取込 </summary>
        UNLOAD = 0,
        /// <summary> 取込中 </summary>
        LOADING = 1,
        /// <summary> 取込済 </summary>
        LOADED = 2,
        /// <summary> 取込開放 </summary>
        RELEASE = 3,
    }
    /// <summary>
    /// 仕分け未完了/完了
    /// </summary>
    public enum ORDER_OR_COMP
    {
        /// <summary> 全て </summary>
        ALL = -1,
        /// <summary> 未完了 </summary>
        NOT_COMP = 0,
        /// <summary> 完了 </summary>
        COMP = 1,
    }

    /// <summary>
    /// キーボード 階層
    /// </summary>
    public enum KEY_SEQUENCE
    {
        MAIN = 0,
        ORDER = 1,
        REGIST = 2,
    }



    /// <summary>
    /// フォーマット用定数
    /// </summary>
    public static class FormatConst
    {
        /// <summary>
        /// エンコーディング
        /// </summary>
        public static Encoding FileEncoding = System.Text.Encoding.GetEncoding("shift_jis");

        /// <summary>
        /// エンコーディング
        /// </summary>
        public static Encoding QrCodeEncoding = System.Text.Encoding.UTF8;

        /// <summary>
        /// 画面日時表示書式
        /// </summary>
        public static string DateTimeFormat = "yy/MM/dd HH:mm";

        /// <summary>
        /// 日時ファイル名書式
        /// </summary>
        public static string DateTimeFileNameFormat = "yyyyMMdd_HHmmss";

        /// <summary>
        /// メッセージ 日時表示書式
        /// </summary>
        public static string MessageDateTimeFormat = "HH:mm:ss";

        /// <summary>
        /// 経過時間表示書式
        /// </summary>
        public static string ElapsedTime_Format = "{0:0.0}";

        /// <summary>
        /// トレイ情報日時表示書式
        /// </summary>
        public static string TrayDateTimeFormat = "yyyy/MM/dd HH:mm:ss";

        /// <summary>
        /// 棚無効リストのフォーマット
        /// </summary>
        public static string DisableRackKeyFormat = "{0:00}-{1:00}-{2:00}";

        /// <summary>
        /// 時刻のみフォーマット
        /// </summary>
        public static string TimeOnlyFormat = "HH:mm";
    }


    /// <summary>
    /// レプリケーション状態
    /// </summary>
    public enum REPLICATION_STATUS
    {
        None,
        /// <summary>
        /// スタンドアローン
        /// </summary>
        StandAlone,
        /// <summary>
        /// マスタ 同期
        /// </summary>
        MasterSync,
        /// <summary>
        /// マスタ 非同期
        /// </summary>
        MasterAsync,
        /// <summary>
        /// スレーブ
        /// </summary>
        Slave,
    }


}
