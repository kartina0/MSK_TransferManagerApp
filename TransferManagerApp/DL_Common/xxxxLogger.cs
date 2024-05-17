// ----------------------------------------------
// Copyright © 2017 DATALINK
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;


// @@20230510
namespace DL_CommonLibrary
{
    ///// <summary>
    ///// ログ項目名
    ///// </summary>
    //public enum xxxxLogType
    //{
    //    /// <summary>システムログ</summary>
    //    SYSTEM,
    //    /// <summary>エラーログ</summary>
    //    ERROR,
    //    /// <summary>アラームログ</summary>
    //    ALARM,
    //    /// <summary>通過実績等の履歴ログ</summary>
    //    HISTORY,
        
    //    /// <summary>操作ログ</summary>
    //    CONTROL,
    //    /// <summary>サーバーへの送信ログ</summary>
    //    SERVER_SEND,
    //    /// <summary>サーバーからの受信ログ</summary>
    //    SERVER_RECV,
    //    /// <summary>送信ログ</summary>
    //    SEND,
    //    /// <summary>受信ログ</summary>
    //    RECV,
    //    /// <summary>関数IN時のログ</summary>
    //    METHOD_IN,
    //    /// <summary>関数OUT時のログ</summary>
    //    METHOD_OUT,
    //    /// <summary>いろいろ</summary>
    //    INFO,

    //    /// <summary>マスタ情報</summary>
    //    MASTER_INFO,

    //    /// <summary>
    //    /// @@20190128
    //    /// タクト
    //    /// </summary>
    //    TACT,
    //    /// <summary>デバッグ用ログ</summary>
    //    DEBUG,
    //}

    public class xxxxLogger
    {

        ///// <summary>
        ///// ログインデックス番号
        ///// </summary>
        //private Int32 _index = 0;

        ///// <summary>
        ///// ログ初期化フラグ
        ///// </summary>
        //private bool _initialized = false;
        ///// <summary>
        ///// フォルダ
        ///// </summary>
        //private string _dir = "";

        ///// <summary>
        ///// ファイル名
        ///// </summary>
        //private string _fileName = "";

        //[DllImport("LOG_LIB.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //private extern static Int32 LOG_Open(Int32 index, string dirName, string fileName, Int32 maxLine, Int32 maxFileCount);

        //[DllImport("LOG_LIB.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //private extern static Int32 LOG_Close(int index);

        //[DllImport("LOG_LIB.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //private extern static Int32 LOG_Put(int index, string buf, int size);

        //[DllImport("LOG_LIB.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //private extern static Int32 LOG_PutMsg(int index, string buf, int size);


        //[DllImport("LOG_LIB.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //private extern static Int32 LOG_IsExist(int index);

        //[DllImport("LOG_LIB.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //private extern static Int32 LOG_GetMaxCount();

        //[DllImport("LOG_LIB.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //private extern static Int32 LOG_GetFileName(Int32 index, string dirName, string fileName);




        ///// <summary>
        ///// @@20110812
        ///// 同一ログファイルを使用する場合に
        ///// すでにOpenしているLOG ID取得設定を行う
        ///// </summary>
        //public int LOG_ID
        //{
        //    get { return _index; }
        //    set
        //    {
        //        try
        //        {
        //            if (_initialized && _index != value && LOG_IsExist((UInt16)value) != 0)
        //            {   // すでにログオープンしている場合はCloseする
        //                this.Close();
        //            }

        //            if (LOG_IsExist((UInt16)value) != 0)
        //            {
        //                _index = (UInt16)value;
        //                _initialized = true;
        //            }
        //        }
        //        catch { _initialized = false; }
         
        //    }
        //}


        ///// <summary>
        ///// @@20110812
        ///// </summary>
        ///// <returns></returns>
        //public bool INITIALIZED()
        //{
        //    return _initialized;

        //}
        ///// <summary>
        ///// ログファイルオープン
        ///// </summary>
        ///// <param name="dir"></param>
        ///// <param name="file"></param>
        ///// <param name="maxFileCount"></param>
        ///// <param name="maxLine"></param>
        ///// <returns></returns>
        //public bool Open(string dir, string file, int maxFileCount, int maxLine)
        //{
        //    bool rs = false;
        //    try
        //    {
        //        int stat = 0;

        //        // @@20110813
        //        if (dir == "" || file == "")
        //            return false;
        //        // @@20110813
        //        if (!System.IO.Directory.Exists(dir))
        //            return false;


        //        _dir = dir;
        //        _fileName = file;

        //        for (int i = 0; i < LOG_GetMaxCount(); i++)
        //        {
        //            if (LOG_IsExist(i) == 0)
        //            {
        //                stat = LOG_Open(i, dir, file, maxLine, maxFileCount);
        //                if (stat == 0)
        //                {
        //                    rs = true;
        //                    _index = (UInt16)i;
        //                    _initialized = true;
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Trace.Fail(ex.Message);
        //        rs = false;
        //        _initialized = false;
        //    }
        //    return rs;
        //}

        ///// <summary>
        ///// ログファイルクローズ
        ///// </summary>
        ///// <returns></returns>
        //public bool Close()
        //{
        //    bool rs = true;
        //    try
        //    {
        //        if (_initialized)
        //        {
        //            LOG_Close(_index);
        //            _initialized = false;
        //            _index = 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Trace.Fail(ex.Message);
        //        rs = false;
        //        _initialized = false;
        //    }
        //    return rs;
        //}

        ///// <summary>
        ///// ログ書き込み
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="buf"></param>
        ///// <returns></returns>
        //public bool LogWrite(string buf, bool printDateTime=true)
        //{
        //    bool rs = true;
        //    try
        //    {
        //        if (!_initialized)
        //            return false;

        //        string temp = string.Format("{0}", buf);

        //        if (printDateTime)
        //            LOG_Put(_index, temp, temp.Length);
        //        else
        //            LOG_PutMsg(_index, temp, temp.Length);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Trace.Fail(ex.Message);
        //        rs = false;
        //        _initialized = false;
        //    }
        //    return rs;
        //}
        ///// <summary>
        ///// ログ書き込み
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="buf"></param>
        ///// <returns></returns>
        //public bool LogWrite(LogType type, string buf)
        //{
        //    bool rs = true;
        //    try
        //    {
        //        if (!_initialized)
        //            return false;

        //        string temp = string.Format("{0},{1}", type.ToString(), buf);
        //        LOG_Put(_index, temp, temp.Length);

        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Trace.Fail(ex.Message);
        //        rs = false;
        //        _initialized = false;
        //    }
        //    return rs;
        //}

        ///// <summary>
        ///// 指定日より前のログを削除
        ///// </summary>
        ///// <param name="time"></param>
        ///// <returns></returns>
        //public bool DeleteLogFile(DateTime time)
        //{
        //    bool rs = false;
        //    try
        //    {
        //        string ext = System.IO.Path.GetExtension(_fileName);
        //        string fname = System.IO.Path.GetFileNameWithoutExtension(_fileName);

        //        // ファイルリストを取得
        //        //files = GetFileList(_dir, _fileName, _extension, FILE_SORT_TYPE.UpdateDateTime_New);
        //        string[] files = FileIo.GetFileList(_dir, fname, ext, time);

        //        // File Delete
        //        for (int i = 0; i < files.Length; i++)
        //        {
        //            string path = System.IO.Path.Combine(_dir, files[i]);
        //            System.IO.File.Delete(path);
        //        }

        //        rs = true;
        //    }
        //    catch { rs = false; }
        //    return rs;
        //}
    }
}
