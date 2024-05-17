//----------------------------------------------------------
// Copyright © 2019 DATALINK
//----------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorCodeDefine
{
    /// <summary>
    /// エラーコードオフセット
    /// </summary>
    public static class ErrorCodeOffset
    {

    }

    /// <summary>
    /// エラーコード 
    /// 2000以降を使用する事
    /// ＊CommonLibで0～300まで使用している
    /// </summary>
    public enum ErrorCodeList : UInt32
    {
        // --------------------------------------
        // 共通エラーコード(共通)
        // --------------------------------------
        #region "共通エラー"
        /// <summary>
        /// 正常終了・エラー無し
        /// </summary>
        STATUS_SUCCESS = 0,
        /// <summary>
        /// 例外発生、不正な値
        /// </summary>
        EXCEPTION,

        /// <summary>
        /// 初期化されていない
        /// </summary>
        NOT_INITIALIZE,

        /// <summary>
        /// すでにプログラム起動中
        /// </summary>
        APP_ALREADY_STARTED,
        /// <summary>
        /// ユーザーによってキャンセルされた
        /// </summary>
        USER_CANCEL,
        /// <summary>
        /// 強制停止
        /// </summary>
        FORCE_STOP,

        /// <summary>
        /// タイムアウト
        /// </summary>
        TIMEOUT,


        /// <summary>
        /// コントロールが存在しない
        /// </summary>
        CONTROL_NOT_FOUND,
        /// <summary>
        /// パラメータ異常
        /// </summary>
        INVALID_PARAMETER,

        /// <summary>
        /// 未登録エラー
        /// </summary>
        UNKNOW,

        #endregion

        // ------------------------------------------------
        // Thread Error(共通)
        // ------------------------------------------------
        #region "Thread Error"
        /// <summary>
        /// スレッド既にあり
        /// </summary>
        THREAD_EXIST = 100,
        /// <summary>
        /// スレッド開始タイムアウト
        /// </summary>
        THREAD_START_TIMEOUT,
        /// <summary>
        /// スレッド終了タイムアウト
        /// </summary>
        THREAD_END_TIMEOUT,
        /// <summary>
        /// シーケンススレッド強制停止
        /// </summary>
        THREAD_ABORT_SEQUENCE,
        /// <summary>
        /// スレッド未初期化
        /// </summary>
        THREAD_NOT_INITIALIZED,
        /// <summary>
        /// スレッド他処理動作中
        /// </summary>
        THREAD_BUSY,
        /// <summary>
        /// 他動作中
        /// </summary>
        OTHER_FUNCTION_RUNNING,
        /// <summary>
        /// Thread Complete Timeout
        /// </summary>
        THREAD_COMP_TIMEOUT,
        #endregion

        // ------------------------------------------------
        // File Error(共通)
        // ------------------------------------------------
        #region "File Error"
        /// <summary>
        /// ファイルアクセスエラー
        /// </summary>
        FILE_NOT_FOUND = 150,
        /// <summary>
        /// ファイルパスエラー
        /// </summary>
        FILE_PATH_ERROR,

        /// <summary>
        /// ファイル作成エラー
        /// </summary>
        FILE_CREATE_ERROR,

        /// <summary>
        /// ファイル種別エラー
        /// </summary>
        FILE_TYPE_ERROR,

        /// <summary>
        /// ファイル異常
        /// </summary>
        FILE_FORMAT_ERROR,

        #endregion

        // ------------------------------------------------
        // Directory Error(共通)
        // ------------------------------------------------
        #region "Directory Error"
        /// <summary>
        /// フォルダなし
        /// </summary>
        DIR_NOT_FOUND = 200,

        /// <summary>
        /// フォルダパスエラー
        /// </summary>
        DIR_PATH_ERROR,
        /// <summary>
        /// フォルダ作成エラー
        /// </summary>
        DIR_CREATE_ERROR,

        #endregion
        // ------------------------------------------------
        // NTP Error(共通)
        // ------------------------------------------------
        #region "NTP Error"
        /// <summary>
        /// NTP オープンエラー
        /// </summary>
        NTP_OPEN_ERROR = 250,
        /// <summary>
        /// NTP レジストリキーがない
        /// </summary>
        NTP_NOT_REGISTORY_KEY,
        #endregion
        // ------------------------------------------------
        // NET Work Error(共通)
        // ------------------------------------------------
        #region "NET Error"
        /// <summary>
        /// ネットワーク未接続
        /// </summary>
        NET_NOT_CONNECTION = 300,

        /// <summary>
        /// サーバー未接続
        /// </summary>
        SERVER_NOT_CONNECTION,

        /// <summary>
        /// ネットワークアダプタに該当するIPアドレスが存在しない
        /// </summary>
        IpAddress_NotFound_NetworkAdapter,

        /// <summary>
        /// IPアドレスが定義されていない
        /// </summary>
        IpAddress_Not_Defined,

        /// <summary>
        /// ソケットサーバーオープンエラー
        /// </summary>
        ServerPort_OpenError,
        /// <summary>
        /// ソケット接続エラー
        /// </summary>
        SocketConnectionError,

        /// <summary>
        /// ソケット接続エラー
        /// </summary>
        SocketSendError,

        /// <summary>
        /// ソケット受信タイムアウト
        /// </summary>
        SocketRecvTimeout,
        /// <summary>
        /// リターンコードエラー
        /// </summary>
        SocketReturnCodeError,
        /// <summary>
        /// IPアドレスフォーマットエラー
        /// </summary>
        IP_AddressFormaError,
        #endregion
        // ------------------------------------------------
        // Log File Error(共通)
        // ------------------------------------------------
        #region "Log File Error"
        /// <summary>
        /// ログファイル作成異常
        /// </summary>
        SYSTEM_LOG_CREATE_ERROR = 350,

        #endregion

        // ------------------------------------------------
        // PLC通信
        // ------------------------------------------------
        #region "PLC Communication Error"
        PLC_NOT_INITIALIZED = 500,
        /// <summary>
        /// PLC接続 初期化エラー
        /// </summary>
        PLC_INIT_ERROR,
        /// <summary>
        /// PLC接続 エラー
        /// </summary>
        PLC_CONNECT_ERROR,
        /// <summary>
        /// PLC読込 エラー
        /// </summary>
        PLC_READ_ERROR,
        /// <summary>
        /// PLC書込みエラー
        /// </summary>
        PLC_WRITE_ERROR,

        /// <summary>
        /// PLCデバイスタイプエラー
        /// </summary>
        PLC_DEVICE_TYPE_ERROR,
        #endregion




        // ------------------------------------------------
        // SERVER
        // ------------------------------------------------
        /// <summary>
        /// DB接続エラー
        /// </summary>
        DB_OPEN_ERROR = 600,
        /// <summary>
        /// DB未接続エラー
        /// </summary>
        DB_NO_OPEN_ERROR,
        /// <summary>
        /// DBリードライトエラー
        /// </summary>
        DB_READ_WRITE_ERROR,


        /// <summary>
        /// PICKDATAファイルがサーバーに存在しない
        /// </summary>
        PICKDATA_FILE_IS_NOT_EXIST_SERVER,
        /// <summary>
        /// PICKDATAファイルがローカルに存在しない
        /// </summary>
        PICKDATA_FILE_IS_NOT_EXIST_LOCAL,
        /// <summary>
        /// PICKDATAファイル 読み出しエラー
        /// </summary>
        PICKDATA_FILE_READ_ERROR,
        /// <summary>
        /// 商品マスターファイルが存在しない
        /// </summary>
        MASTER_WORK_FILE_IS_NOT_EXIST,
        /// <summary>
        /// 商品マスターファイル 読み出しエラー
        /// </summary>
        MASTER_WORK_FILE_READ_ERROR,
        /// <summary>
        /// 店マスターファイルが存在しない
        /// </summary>
        MASTER_STORE_FILE_IS_NOT_EXIST,
        /// <summary>
        /// 店マスターファイル 読み出しエラー
        /// </summary>
        MASTER_STORE_FILE_READ_ERROR,
        /// <summary>
        /// 作業者マスターファイルが存在しない
        /// </summary>
        MASTER_WORKER_FILE_IS_NOT_EXIST,
        /// <summary>
        /// 作業者マスターファイル 読み出しエラー
        /// </summary>
        MASTER_WORKER_FILE_READ_ERROR,


        ROBOT_IF_NOT_INITIALIZED,
    }

}
