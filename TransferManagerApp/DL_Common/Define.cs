//----------------------------------------------------------
// Copyright © 2017 DATALINK
//----------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace DL_CommonLibrary
{
	/// <summary>
	/// エラーコード
	/// </summary>
	public enum ERROR_7CODE : uint
	{
		// --------------------------------------
		// 共通エラーコード
        // --------------------------------------
        #region "共通エラー"
        /// <summary>
		/// 正常終了・エラー無し
		/// </summary>
		STATUS_SUCCESS          = 0,
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
        /// 未登録エラー
        /// </summary>
        UNKNOW,

        /// <summary>
        /// INIファイルバージョン異常
        /// </summary>
        INI_FILE_VERSION_ERROR,


        #endregion

        // ------------------------------------------------
        // Thread Error
        // ------------------------------------------------
        #region "Thread Error"
        /// <summary>
        /// スレッド既にあり
        /// </summary>
        THREAD_EXIST            = 100,
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
        // File Error
        // ------------------------------------------------
        #region "File Error"
        /// <summary>
        /// ファイルアクセスエラー
        /// </summary>
        FILE_NOT_FOUND          = 150,
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
        /// <summary>
        /// ファイルオープンエラー
        /// </summary>
        FILE_OPEN_ERROR,
        /// <summary>
        /// ファイル読み込みエラー
        /// </summary>
        FILE_READ_ERROR,

        /// <summary>
        /// ファイル書き込みエラー
        /// </summary>
        FILE_WRITE_ERROR,
        #endregion

        // ------------------------------------------------
        // Directory Error
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
        // NTP Error
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
        // NET Work Error
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
        /// ソケット受信ﾃﾞｰﾀフォーマットエラー
        /// </summary>
        SocketRecvFormatError,
        /// <summary>
        /// ソケットリターンコードエラー
        /// </summary>
        SocketReturnError,

        /// <summary>
        /// ソケットリターンコマンド不一致
        /// </summary>
        SocketUnmatchReturn,
        #endregion
        // ------------------------------------------------
        // PLC
        // ------------------------------------------------
        #region "PLC Error"
        /// <summary>
        /// 受信データフォーマット異常
        /// </summary>
        PLC_RECV_FORMAT_ERROR = 350,

        /// <summary>
        /// PLCから異なる設備種別のコマンドを受信した
        /// </summary>
        PLC_MACHINE_TYPE_ERROR,

        /// <summary>
        /// PLC番号が範囲外
        /// </summary>
        PLC_INDEX_OUT_OF_RANGE,
        #endregion

        // ------------------------------------------------
        // Axis
        // ------------------------------------------------
        #region "Axis Error"
        /// <summary>
        /// 全軸ｺﾏﾝﾄﾞ未対応
        /// </summary>
        AXIS_NOT_SUPPORT_ALL_AXIS_CMD = 400,

        /// <summary>
        /// 軸制御通信タイムアウト
        /// </summary>
        AXIS_RECV_TIMEOUT,

        /// <summary>
        /// 未接続 
        /// </summary>
        AXIS_NOT_CONNECT,

        /// <summary>
        /// コントローラからNAK応答
        /// </summary>
        AXIS_NAK_RESPONSE,

        /// <summary>リミットを超えている</summary>
        AXIS_LIMIT_OVER,

        #endregion

        // ------------------------------------------------
        // Log File Error
        // ------------------------------------------------
        #region "Log File Error"
        /// <summary>
        /// ログファイル作成異常
        /// </summary>
        SYSTEM_LOG_CREATE_ERROR = 450,

        #endregion

        // ------------------------------------------------
        // PARAMETER
        // ------------------------------------------------
        #region "Parameter Error"
        /// <summary>
        /// パラメータ未登録
        /// </summary>
        PARAMETER_NOT_FOUND = 500,



        #endregion
        // ------------------------------------------------
        // Robot
        // ------------------------------------------------
        #region "Robot Error"
        /// <summary>
        /// ロボット初期化されていない
        /// </summary>
        ROBOT_NOT_INITIALIZED = 550,

        /// <summary>
        /// ロボット未接続
        /// </summary>
        ROBOT_NOT_CONNECTED,

        /// <summary>
        /// 搬送中の為、受付けられない
        /// </summary>
        ROBOT_NOW_TRANSFER_RUNNNING,

        /// <summary>
        /// ワーク情報番号オーバー
        /// </summary>
        ROBOT_ILLEGAL_WORK_INFO_INDEX,

        /// <summary>
        /// ロボットから戻り値が異常
        /// </summary>
        ROBOT_RETURN_ERROR,

        /// <summary>
        /// コンベアからワーク搬送指示時
        /// ワークがまだ開始位置まで到達していない
        /// </summary>
        ROBOT_WORK_LESS_THAN_STARTPOS,

        /// <summary>
        /// コンベアからワーク搬送指示時
        /// ワークがすでに取れない位置まで移動してしまった
        /// </summary>
        ROBOT_WORK_MORE_THAN_ENDPOS,

        /// <summary>
        /// ロボット運転動作停止タイムアウト
        /// </summary>
        ROBOT_STOP_TIMEOUT,

        /// <summary>
        /// コンベアレジスタ ACK タイムアウト
        /// </summary>
        ROBOT_CONV_RESET_ACK_ON_TIMEOUT,
        /// <summary>
        /// コンベアレジスタ ACK タイムアウト
        /// </summary>
        ROBOT_CONV_RESET_ACK_OFF_TIMEOUT,

        #endregion

        // ------------------------------------------------
        // Layout
        // ------------------------------------------------
        LAYOUT_NO_SPACE = 600,

    }


}
