//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Runtime.InteropServices;

using DL_CommonLibrary;
using ErrorCodeDefine;


namespace DL_Logger
{
    public static class Logger
    {
        private const string THIS_NAME = "Logger";


        /// <summary>
        /// システムログ
        /// 全ログ集約
        /// </summary>
        private static LoggingModule _systemLog = null;
        /// <summary>
        /// 履歴用ログ
        /// </summary>
        private static LoggingModule _historyLog = null;
        /// <summary>
        /// アラーム履歴ログ
        /// </summary>
        private static LoggingModule _alarmLog = null;
        /// <summary>
        /// 操作ログ
        /// </summary>
        private static LoggingModule _opeLog = null;

        /// <summary>
        /// システムログ ファイル名
        /// </summary>
        public static string systemlogFileName = "System.log";
        /// <summary>
        /// 履歴用ログ ファイル名
        /// </summary>
        public static string historylogFileName = "History.log";
        /// <summary>
        /// アラーム履歴ログ ファイル名
        /// </summary>
        public static string alarmlogFileName = "Alarm.log";
        /// <summary>
        /// 操作ログ ファイル名
        /// </summary>
        public static string operationlogFileName = "Operation.log";


        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public static UInt32 Open(string logDir, string title, int LogMaxFileCount, int LogMaxLineCount)
        {
            UInt32 rc = 0;
            bool ret = false;
            try
            {
                // ------------------------------
                // ログオープン
                // ------------------------------
                if (_systemLog == null)
                    _systemLog = new LoggingModule();
                ret = _systemLog.Open(logDir, systemlogFileName, LogMaxFileCount, LogMaxLineCount);
                FileIo.DeleteLogFile(logDir, systemlogFileName, DateTime.Now.AddDays(-10));   // 10日以上前のログは削除
                if (!ret)
                {
                    Dialogs.ShowInformationMessage("システムログをオープンできませんでした", title, System.Drawing.SystemIcons.Error);
                    rc = (UInt32)ErrorCodeList.FILE_CREATE_ERROR;
                }

                if (_alarmLog == null)
                    _alarmLog = new LoggingModule();
                ret = _alarmLog.Open(logDir, alarmlogFileName, LogMaxFileCount, LogMaxLineCount);
                FileIo.DeleteLogFile(logDir, alarmlogFileName, DateTime.Now.AddDays(-10));    // 10日上前のログは削除
                if (!ret)
                {
                    Dialogs.ShowInformationMessage("アラームログをオープンできませんでした", title, System.Drawing.SystemIcons.Error);
                    rc = (UInt32)ErrorCodeList.FILE_CREATE_ERROR;
                }

                if (_historyLog == null)
                    _historyLog = new LoggingModule();
                ret = _historyLog.Open(logDir, historylogFileName, LogMaxFileCount, LogMaxLineCount);
                FileIo.DeleteLogFile(logDir, historylogFileName, DateTime.Now.AddMonths(-1));    // 1カ月以上前のログは削除
                if (!ret)
                {
                    Dialogs.ShowInformationMessage("履歴ログをオープンできませんでした", title, System.Drawing.SystemIcons.Error);
                    rc = (UInt32)ErrorCodeList.FILE_CREATE_ERROR;
                }

                if (_opeLog == null)
                    _opeLog = new LoggingModule();
                ret = _opeLog.Open(logDir, operationlogFileName, LogMaxFileCount, LogMaxLineCount);
                FileIo.DeleteLogFile(logDir, operationlogFileName, DateTime.Now.AddMonths(-1));    // 1カ月以上前のログは削除
                if (!ret)
                {
                    Dialogs.ShowInformationMessage("操作ログをオープンできませんでした", title, System.Drawing.SystemIcons.Error);
                    rc = (UInt32)ErrorCodeList.FILE_CREATE_ERROR;
                }

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;

                //// Resorce Open/Closeでの例外は何が起きているか不明なので
                //// ResourceのOpenCloseではErrorManagerのErrorHandlerを直接コールする 
                //ErrorManager.ErrorHandler(ex);
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
                if (_historyLog != null) _historyLog.Close();
                if (_alarmLog != null) _alarmLog.Close();
                if (_systemLog != null) _systemLog.Close();
                if (_opeLog != null) _opeLog.Close();

                _historyLog = null;
                _alarmLog = null;
                _systemLog = null;
                _opeLog = null;
            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
            }
            return rc;
        }


        /// <summary>
        /// ログ書き込み
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public static void WriteLog(LogType type, string msg, bool dispBottomMessage = false)
        {
            // システムログ
            if (_systemLog != null)
                _systemLog.LogWrite(type, msg);

            if (type == LogType.HISTORY)
            {   // 履歴ログ
                if (_historyLog != null)
                    _historyLog.LogWrite(msg);
            }
            else if (type == LogType.ALARM)
            {   // アラーム履歴ログ
                if (_alarmLog != null)
                    _alarmLog.LogWrite(msg);
            }
            else if (type == LogType.ERROR)
            {   // アラームログ
                if (_alarmLog != null)
                    _alarmLog.LogWrite(msg);
            }
            else if (type == LogType.CONTROL)
            {   // 操作ログ
                if (_opeLog != null)
                    _opeLog.LogWrite(msg);
            }

            //if (dispBottomMessage)
            //{
            //    _systemStatus.alarmMessage = msg;
            //}
        }

    }


    

    /// <summary>
    /// ログモジュール
    /// </summary>
    public class LoggingModule
    {

        /// <summary>
        /// ログインデックス番号
        /// </summary>
        private Int32 _index = 0;

        /// <summary>
        /// ログ初期化フラグ
        /// </summary>
        private bool _initialized = false;
        /// <summary>
        /// フォルダ
        /// </summary>
        private string _dir = "";

        /// <summary>
        /// ファイル名
        /// </summary>
        private string _fileName = "";

        [DllImport("LOG_LIB.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private extern static Int32 LOG_Open(Int32 index, string dirName, string fileName, Int32 maxLine, Int32 maxFileCount);

        [DllImport("LOG_LIB.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private extern static Int32 LOG_Close(int index);

        [DllImport("LOG_LIB.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private extern static Int32 LOG_Put(int index, string buf, int size);

        [DllImport("LOG_LIB.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private extern static Int32 LOG_PutMsg(int index, string buf, int size);


        [DllImport("LOG_LIB.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private extern static Int32 LOG_IsExist(int index);

        [DllImport("LOG_LIB.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private extern static Int32 LOG_GetMaxCount();

        [DllImport("LOG_LIB.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private extern static Int32 LOG_GetFileName(Int32 index, string dirName, string fileName);




        /// <summary>
        /// @@20110812
        /// 同一ログファイルを使用する場合に
        /// すでにOpenしているLOG ID取得設定を行う
        /// </summary>
        public int LOG_ID
        {
            get { return _index; }
            set
            {
                try
                {
                    if (_initialized && _index != value && LOG_IsExist((UInt16)value) != 0)
                    {   // すでにログオープンしている場合はCloseする
                        this.Close();
                    }

                    if (LOG_IsExist((UInt16)value) != 0)
                    {
                        _index = (UInt16)value;
                        _initialized = true;
                    }
                }
                catch { _initialized = false; }

            }
        }


        /// <summary>
        /// @@20110812
        /// </summary>
        /// <returns></returns>
        public bool INITIALIZED()
        {
            return _initialized;

        }
        /// <summary>
        /// ログファイルオープン
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="file"></param>
        /// <param name="maxFileCount"></param>
        /// <param name="maxLine"></param>
        /// <returns></returns>
        public bool Open(string dir, string file, int maxFileCount, int maxLine)
        {
            bool rs = false;
            try
            {
                int stat = 0;

                // @@20110813
                if (dir == "" || file == "")
                    return false;
                // @@20110813
                if (!System.IO.Directory.Exists(dir))
                    return false;


                _dir = dir;
                _fileName = file;

                for (int i = 0; i < LOG_GetMaxCount(); i++)
                {
                    if (LOG_IsExist(i) == 0)
                    {
                        stat = LOG_Open(i, dir, file, maxLine, maxFileCount);
                        if (stat == 0)
                        {
                            rs = true;
                            _index = (UInt16)i;
                            _initialized = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.Fail(ex.Message);
                rs = false;
                _initialized = false;
            }
            return rs;
        }

        /// <summary>
        /// ログファイルクローズ
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            bool rs = true;
            try
            {
                if (_initialized)
                {
                    LOG_Close(_index);
                    _initialized = false;
                    _index = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.Fail(ex.Message);
                rs = false;
                _initialized = false;
            }
            return rs;
        }

        /// <summary>
        /// ログ書き込み
        /// </summary>
        /// <param name="type"></param>
        /// <param name="buf"></param>
        /// <returns></returns>
        public bool LogWrite(string buf, bool printDateTime = true)
        {
            bool rs = true;
            try
            {
                if (!_initialized)
                    return false;

                string temp = string.Format("{0}", buf);

                if (printDateTime)
                    LOG_Put(_index, temp, temp.Length);
                else
                    LOG_PutMsg(_index, temp, temp.Length);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.Fail(ex.Message);
                rs = false;
                _initialized = false;
            }
            return rs;
        }
        /// <summary>
        /// ログ書き込み
        /// </summary>
        /// <param name="type"></param>
        /// <param name="buf"></param>
        /// <returns></returns>
        public bool LogWrite(LogType type, string buf)
        {
            bool rs = true;
            try
            {
                if (!_initialized)
                    return false;

                string temp = string.Format("{0},{1}", type.ToString(), buf);
                LOG_Put(_index, temp, temp.Length);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.Fail(ex.Message);
                rs = false;
                _initialized = false;
            }
            return rs;
        }

        /// <summary>
        /// 指定日より前のログを削除
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool DeleteLogFile(DateTime time)
        {
            bool rs = false;
            try
            {
                string ext = System.IO.Path.GetExtension(_fileName);
                string fname = System.IO.Path.GetFileNameWithoutExtension(_fileName);

                // ファイルリストを取得
                //files = GetFileList(_dir, _fileName, _extension, FILE_SORT_TYPE.UpdateDateTime_New);
                string[] files = FileIo.GetFileList(_dir, fname, ext, time);

                // File Delete
                for (int i = 0; i < files.Length; i++)
                {
                    string path = System.IO.Path.Combine(_dir, files[i]);
                    System.IO.File.Delete(path);
                }

                rs = true;
            }
            catch { rs = false; }
            return rs;
        }
    }

    /// <summary>
    /// ログ項目名
    /// </summary>
    public enum LogType
    {
        /// <summary>システムログ</summary>
        SYSTEM,
        /// <summary>エラーログ</summary>
        ERROR,
        /// <summary>アラームログ</summary>
        ALARM,
        /// <summary>通過実績等の履歴ログ</summary>
        HISTORY,

        /// <summary>操作ログ</summary>
        CONTROL,
        /// <summary>サーバーへの送信ログ</summary>
        SERVER_SEND,
        /// <summary>サーバーからの受信ログ</summary>
        SERVER_RECV,
        /// <summary>送信ログ</summary>
        SEND,
        /// <summary>受信ログ</summary>
        RECV,
        /// <summary>関数IN時のログ</summary>
        METHOD_IN,
        /// <summary>関数OUT時のログ</summary>
        METHOD_OUT,
        /// <summary>いろいろ</summary>
        INFO,

        /// <summary>マスタ情報</summary>
        MASTER_INFO,

        /// <summary>
        /// @@20190128
        /// タクト
        /// </summary>
        TACT,
        /// <summary>デバッグ用ログ</summary>
        DEBUG,
    }

}
