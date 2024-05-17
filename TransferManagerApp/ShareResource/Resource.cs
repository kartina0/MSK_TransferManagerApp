//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;

using BatchModule;
using PLCModule;
using ServerModule;
using DL_CommonLibrary;
using DL_PlcInterfce;
using DL_Logger;
using SystemConfig;
using ErrorCodeDefine;


namespace ShareResource
{
    /// <summary>
    /// 共有リソースクラス
    /// 
    /// アプリケーション内で共有するクラス
    /// 必要なものはすべてこのクラスで初期化を行う
    /// </summary>
    public static class Resource
    {
        /// <summary>
        /// アプリケーションパス
        /// </summary>
        public static string AppPath = "";
        /// <summary>
        /// System Status
        /// </summary>
        private static CommonStatus _systemStatus = new CommonStatus();
        /// <summary>
        /// ログイン情報
        /// </summary>
        public static LoginUserInformation LoginInfo = new LoginUserInformation();

        /// <summary>
        /// アイルPLC
        /// </summary>
        public static PLC[] Plc = null;
        /// <summary>
        /// サーバー
        /// </summary>
        public static Server Server = null;
        /// <summary>
        /// バッチファイル
        /// </summary>
        public static BatchFile batch = null;

        /// <summary>
        /// キーボード 処理階層
        /// </summary>
        public static KEY_SEQUENCE Sequence = KEY_SEQUENCE.MAIN;


        #region "Property"
        /// <summary>
        /// System Status
        /// </summary>
        public static CommonStatus SystemStatus
        {
            get { return _systemStatus; }
            set { _systemStatus = value; }
        }

        /// <summary>
        /// 最新のエラーコードを取得
        /// </summary>
        public static UInt32 LastErrorCode
        {
            get
            {
                ErrorDetail info = _systemStatus.Error.GetLatestErrorInfo();
                return info.code;
            }
        }
        #endregion
        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public static UInt32 Open()
        {
            UInt32 rc = 0;
            bool ret = false;
            try
            {
                // ------------------------------
                // PLC
                // 設備の電源が入っていないかもしれないのでここでの接続エラー確認は行わない
                // ------------------------------
                Plc = new PLC[Const.MaxAisleCount];
                for (int aisleIndex = 0; aisleIndex < Const.MaxAisleCount; aisleIndex++) 
                {
                    if (!STATUS_SUCCESS(rc))
                        break;

                    if (IniFile.AisleEnable[aisleIndex]) 
                    {// アイル有効ならば
                        Plc[aisleIndex] = new PLC(string.Format(IniFile.PlcConnectionString[aisleIndex], IniFile.PlcIpAddress[aisleIndex]));
                        string connectionParam = string.Format(IniFile.PlcConnectionString[aisleIndex], IniFile.PlcIpAddress[aisleIndex]);
                        rc = Plc[aisleIndex].Open(connectionParam);
                    }
                }

                // ------------------------------
                // SERVER
                // ------------------------------
                if(STATUS_SUCCESS(rc))
                    Server = new Server();

                // ------------------------------
                // BATCH
                // ------------------------------
                if (STATUS_SUCCESS(rc))
                    batch = new BatchFile();

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Logger.WriteLog(LogType.ERROR, ex.Message);

                // Resorce Open/Closeでの例外は何が起きているか不明なので
                // ResourceのOpenCloseではErrorManagerのErrorHandlerを直接コールする 
                ErrorManager.ErrorHandler(ex);
            }
            return rc;
        }


        /// <summary>
        /// クローズ
        /// </summary>
        /// <returns></returns>
        public static UInt32 Close()
        {
            UInt32 rc = 0;
            try
            {
                // ------------------------------
                // PLCクローズ
                // ------------------------------
                if (Plc != null)
                for (int aisleIndex = 0; aisleIndex < Const.MaxAisleCount; aisleIndex++)
                {
                        if(Plc[aisleIndex] != null)
                            Plc[aisleIndex].Close();
                        Plc[aisleIndex] = null;
                }
                Plc = null;
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Logger.WriteLog(LogType.ERROR, ex.Message);

                // Resorce Open/Closeでの例外は何が起きているか不明なので
                // ResourceのOpenCloseではErrorManagerのErrorHandlerを直接コールする 
                ErrorManager.ErrorHandler(ex);
            }
            return rc;
        }


        /// <summary>
        /// ポップアップメッセージ表示
        /// </summary>
        /// <param name="color"></param>
        /// <param name="msg"></param>
        public static void ShowPopupMessage(System.Drawing.Color color, string msg)
        {
            string buf = string.Format("{0},{1},{2}\r@", color.Name, 3000, msg);
            string fname = System.IO.Path.Combine(IniFile.MessageDir, DateTime.Now.ToString(FormatConst.DateTimeFileNameFormat) + ".msg");
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fname, false, FormatConst.FileEncoding))
            {
                sw.Write(buf);
                sw.Close();
            }

        }

        /// <summary>
        /// エラーコード設定
        /// ※メッセージを表示する場合は
        /// </summary>
        public static void SetError(UInt32 rc)
        {
            if (!STATUS_SUCCESS(rc))
                _systemStatus.Error.SetLatestErrorInfo(rc);
        }

        /// <summary>
        /// エラーコード設定
        /// </summary>
        public static void SetError(UInt32 rc, string msg)
        {
            if (!STATUS_SUCCESS(rc))
                _systemStatus.Error.SetLatestErrorInfo(rc, msg);
        }

        /// <summary>
        /// エラークリア
        /// </summary>
        public static void ClearError()
        {
            _systemStatus.Error.ResetLatestError();
        }

        /// <summary>
        /// エラーコード取得
        /// </summary>
        /// <returns></returns>
        public static UInt32 GetError()
        {
            return LastErrorCode;
        }

        /// <summary>
        /// エラーか確認
        /// </summary>
        /// <returns></returns>
        public static bool IsError()
        {
            return LastErrorCode != 0;
        }

        /// <summary>
        /// エラーログ書き込み
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public static void ErrorHandler(UInt32 rc, bool dispWindow = false)
        {
            if (!STATUS_SUCCESS(rc))
            {
                // エラー登録 100件まで保持
                SetError(rc);

                // 登録したエラー情報を取得
                ErrorDetail info = _systemStatus.Error.GetLatestErrorInfo();

                // メッセージ表示
                string errMsg = info.message;
                //string errMsg = "aaa";

                //// 音声でエラー読み上げ
                //if (errMsg != "") VoiceMessage.Speaker(errMsg, true);

                // 
                if (errMsg == "") errMsg = string.Format("エラーコード：{0}", rc);
                Logger.WriteLog(LogType.ERROR, errMsg);

                if (dispWindow)
                    Dialogs.ShowInformationMessage(errMsg, Const.Title, System.Drawing.SystemIcons.Error);
            }
        }

        /// <summary>
        /// エラーログ書き込み
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public static void ErrorHandler(Exception ex, bool dispWindow = false)
        {
            UInt32 rc = (UInt32)ErrorCodeList.EXCEPTION;
            dispWindow = false;
            // エラー登録 100件まで保持
            SetError(rc);
            //SetError(123456789, ex.StackTrace);   

            // 登録したエラー情報を取得
            ErrorDetail info = _systemStatus.Error.GetLatestErrorInfo();

            // メッセージ表示
            string errMsg = info.message;
            if (errMsg == "")
                errMsg = string.Format("エラーコード：{0}", rc) + "," + ex.Message;
            else
                errMsg = errMsg + "," + ex.Message;

            Logger.WriteLog(LogType.ERROR, ex.Message);

            //// 音声でエラー読み上げ
            //VoiceMessage.Speaker(ex.Message, true);

            if (dispWindow)
                Dialogs.ShowInformationMessage(errMsg, Const.Title, System.Drawing.SystemIcons.Error);

        }

        /// <summary>
        /// エラーログ書き込み
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public static void ErrorHandler(UInt32 rc, string msg, bool dispWindow = false)
        {

            // エラー登録 100件まで保持
            SetError(rc, msg);

            // 登録したエラー情報を取得
            ErrorDetail info = _systemStatus.Error.GetLatestErrorInfo();

            // メッセージ表示
            string errMsg = info.message;
            if (errMsg == "")
                errMsg = string.Format("エラーコード：{0}", rc) + "," + msg;

            Logger.WriteLog(LogType.ERROR, errMsg);

            if (dispWindow)
                Dialogs.ShowInformationMessage(errMsg, Const.Title, System.Drawing.SystemIcons.Error);
        }
        /// <summary>
        /// エラーログ書き込み
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public static void ErrorHandler(string msg, bool dispWindow = false)
        {
            UInt32 rc = (UInt32)ErrorCodeList.UNKNOW;

            //// 音声でエラー読み上げ
            //VoiceMessage.Speaker(msg, true);

            // エラー登録 100件まで保持
            SetError(rc, msg);

            // 
            Logger.WriteLog(LogType.ERROR, msg);

            if (dispWindow)
                Dialogs.ShowInformationMessage(msg, Const.Title, System.Drawing.SystemIcons.Error);
        }
        /// <summary>
        /// エラー有無確認
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        private static bool STATUS_SUCCESS(UInt32 rc)
        {
            return rc == 0;
        }
    }
}
