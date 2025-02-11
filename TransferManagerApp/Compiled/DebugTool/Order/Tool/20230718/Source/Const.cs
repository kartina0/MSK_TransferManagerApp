﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public const string IniDir = ".\\INI";
        /// <summary>
        /// INIファイル名
        /// </summary>
        public const string IniFileName = "TransferManagerApp.INI";

        /// <summary>
        /// ステーション最大数
        /// </summary>
        public const int MaxStationCount = 9;
        /// <summary>
        /// アイル最大数
        /// </summary>
        public const int MaxAisleCount =4;
        /// <summary>
        /// ユニット最大数
        /// </summary>
        public const int MaxUnitCount = 3;
        /// <summary>
        /// スロット最大数
        /// </summary>
        public const int MaxSlotCount = 12;
        /// <summary>
        /// PLCワーク登録最大数
        /// </summary>
        public const int MaxWorkRegisterCount = 10;



        /// <summary>
        /// ダミー用の接続パラメータ
        /// ※生育はフロアに複数あるためINIファイルではなくこれを使用している
        /// </summary>
        public const string SocketConnection_Dummy_PLC = "DUMMY;PLC_{0}";




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
            /// <summary> 未取込 </summary>
            LOADING = 1,
            /// <summary> 取込中 </summary>
            LOADED = 2,
            /// <summary> 取込開放 </summary>
            RELEASE = 3,
        }


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
        public const int LogMaxFileCount = 100;
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
