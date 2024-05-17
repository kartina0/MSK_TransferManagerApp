// ----------------------------------------------
// Copyright © 2021 DATALINK
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using DL_CommonLibrary;
using ErrorCodeDefine;
using DL_Logger;

namespace DL_PlcInterfce
{
    public class PLC_IF
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "PLC_IF";

        /// <summary>
        /// 接続パラメータ
        /// </summary>
        public string ConnectionParam { get; set; }="";

        /// <summary>
        /// PLCインターフェース
        /// </summary>
        IPlc _PLC = null;

        /// <summary>
        /// ダミーモードか確認
        /// </summary>
        /// <returns></returns>
        public bool IsDummy()
        {
            if (_PLC == null) return true;
            return _PLC.IsDummy();
        }

        /// <summary>
        /// 接続
        /// </summary>
        /// <param name="connectionParam">
        /// 接続パラメータ
        /// Keyence : ETHER,KV8000
        /// </param>
        /// <returns></returns>
        public UInt32 Open(string connectionParam)
        {
            UInt32 rc = 0;
            WriteLog(LogType.METHOD_IN, string.Format("{0}.Open({1})", THIS_NAME, connectionParam));
            try
            {

                ConnectionParam = connectionParam;

                // とりあえずダミー
                if (_PLC == null)
                {
                    string[] param = connectionParam.Split(';');

                    if (connectionParam == "" || param.Length <= 0 || param[0] == "DUMMY")
                    {
                        _PLC = new Dummy_PLC();
                    }
                    else if (param[0].IndexOf("KV") == 0 )
                    {
                        _PLC = new Keyence_PLC_Socket();
                        if(param.Length < 4)
                            rc = (UInt32)ErrorCodeList.INVALID_PARAMETER;
                    }
                    else
                        rc = (UInt32)ErrorCodeList.INVALID_PARAMETER;
                }

                if (STATUS_SUCCESS(rc))
                    rc = _PLC.Open(connectionParam);

            }
            catch (Exception ex)
            {
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            WriteLog(LogType.METHOD_OUT, string.Format("{0}.Open : {1}", THIS_NAME, (ErrorCodeList)rc));
            return rc;
        }

        /// <summary>
        /// 切断
        /// </summary>
        /// <returns></returns>
        public UInt32 Close()
        {

            UInt32 rc = 0;
            WriteLog(LogType.METHOD_IN, string.Format("{0}.Close()", THIS_NAME));
            try
            {
                if (_PLC == null) rc = (UInt32)ErrorCodeList.PLC_NOT_INITIALIZED;

                if (STATUS_SUCCESS(rc))
                {
                    rc = _PLC.Close();
                    _PLC = null;
                }
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                ErrorHandler(ex, false);
            }
            WriteLog(LogType.METHOD_OUT, string.Format("{0}.Close : {1}", THIS_NAME, (ErrorCodeList)rc));
            return rc;

        }

        /// <summary>
        /// 接続確認
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            if (_PLC == null) return false;
            return _PLC.IsConnected();
        }
        /// <summary>
        /// 単一デバイス読み出し
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Read(DeviceType deviceType, UInt32 addr, ref int value)
        {
            UInt32 rc = 0;
            //WriteLog(LogType.METHOD_IN, string.Format("{0}.Read16({1},{2})", THIS_NAME, deviceType, addr));
            value = 0;
            try
            {
                if (_PLC == null) rc = (UInt32)ErrorCodeList.PLC_NOT_INITIALIZED;

                if (STATUS_SUCCESS(rc))
                {
                    rc = _PLC.Read(deviceType, addr, ref value);
                }
            }
            catch (Exception ex)
            {
                rc = (UInt32)ErrorCodeList.PLC_READ_ERROR;
                ErrorHandler(ex, false);
            }
            //WriteLog(LogType.METHOD_OUT, string.Format("{0}.Read16 : {1}", THIS_NAME, (ErrorCodeList)rc));
            return rc;
        }
        /// <summary>
        /// 単一デバイス読み出し
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Read32(DeviceType deviceType, UInt32 addr, ref Int32 value)
        {
            UInt32 rc = 0;
            // WriteLog(LogType.METHOD_IN, string.Format("{0}.Read({1},{2})", THIS_NAME, deviceType, addr));
            value = 0;
            try
            {
                if (_PLC == null) rc = (UInt32)ErrorCodeList.PLC_NOT_INITIALIZED;

                if (STATUS_SUCCESS(rc))
                {
                    rc = _PLC.Read32(deviceType, addr, ref value);
                }
            }
            catch (Exception ex)
            {
                rc = (UInt32)ErrorCodeList.PLC_READ_ERROR;
                ErrorHandler(ex, false);
            }
            //WriteLog(LogType.METHOD_OUT, string.Format("{0}.Read : {1}", THIS_NAME, (ErrorCodeList)rc));
            return rc;
        }

        /// <summary>
        /// 文字読み出し
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="len">最大文字数</param>
        /// <param name="buf">文字列</param>
        /// <returns></returns>
        public UInt32 Read(DeviceType deviceType, UInt32 addr, Int32 len, ref string buf)
        {
            UInt32 rc = 0;
            //WriteLog(LogType.METHOD_IN, string.Format("{0}.ReadText({1},{2},{3})", THIS_NAME, deviceType, addr, len));
            buf = "";
            try
            {
                if (_PLC == null) rc = (UInt32)ErrorCodeList.PLC_NOT_INITIALIZED;

                if (STATUS_SUCCESS(rc))
                {
                    rc = _PLC.Read(deviceType, addr, len, ref buf);
                }
            }
            catch (Exception ex)
            {
                rc = (UInt32)ErrorCodeList.PLC_READ_ERROR;
                ErrorHandler(ex, false);
            }
            //WriteLog(LogType.METHOD_OUT, string.Format("{0}.ReadText : {1}", THIS_NAME, (ErrorCodeList)rc));
            return rc;
        }

        /// <summary>
        /// 連続デバイス読み出し
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="count">個数</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Read(DeviceType deviceType, UInt32 addr, Int32 count, ref Int32[] value)
        {
            UInt32 rc = 0;
            //WriteLog(LogType.METHOD_IN, string.Format("{0}.Read({1},{2},{3})", THIS_NAME, deviceType, addr, count));
            try
            {
                if (_PLC == null) rc = (UInt32)ErrorCodeList.PLC_NOT_INITIALIZED;

                if (STATUS_SUCCESS(rc))
                {
                    rc = _PLC.Read(deviceType, addr, count, ref value);
                }
            }
            catch (Exception ex)
            {
                rc = (UInt32)ErrorCodeList.PLC_READ_ERROR;
                ErrorHandler(ex, false);
            }
            //WriteLog(LogType.METHOD_OUT, string.Format("{0}.Read : {1}", THIS_NAME, (ErrorCodeList)rc));
            return rc;
        }
        /// <summary>
        /// 連続デバイス読み出し
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="count">個数</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Read32(DeviceType deviceType, UInt32 addr, Int32 count, ref Int32[] value)
        {
            UInt32 rc = 0;
            //WriteLog(LogType.METHOD_IN, string.Format("{0}.Read32({1},{2},{3})", THIS_NAME, deviceType, addr, count));
            try
            {
                if (_PLC == null) rc = (UInt32)ErrorCodeList.PLC_NOT_INITIALIZED;
                if(value.Length != count) value = new Int32[count];

                int wordCount = count * 2;
                Int32[] temp = new Int32[wordCount];
                if (STATUS_SUCCESS(rc))
                    rc = _PLC.Read(deviceType, addr, wordCount, ref temp);

                if (STATUS_SUCCESS(rc))
                {
                    int index = 0;
                    for (int i = 0; i < count; i++)
                    {
                        value[index++] = (temp[i + 1] << 16) | temp[i];
                    }
                }
            }
            catch (Exception ex)
            {
                rc = (UInt32)ErrorCodeList.PLC_READ_ERROR;
                ErrorHandler(ex, false);
            }
            //WriteLog(LogType.METHOD_OUT, string.Format("{0}.Read32 : {1}", THIS_NAME, (ErrorCodeList)rc));
            return rc;
        }
        /// <summary>
        /// 単一デバイス書込み
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Write(DeviceType deviceType, UInt32 addr, int value)
        {
            UInt32 rc = 0;
            //WriteLog(LogType.METHOD_IN, string.Format("{0}.Write16({1},{2},{3})", THIS_NAME, deviceType, addr, value));
            try
            {
                if (addr == 1044) addr = addr;
                if (_PLC == null) rc = (UInt32)ErrorCodeList.PLC_NOT_INITIALIZED;

                if (STATUS_SUCCESS(rc))
                {
                    rc = _PLC.Write(deviceType, addr, value);
                }
            }
            catch (Exception ex)
            {
                rc = (UInt32)ErrorCodeList.PLC_WRITE_ERROR;
                ErrorHandler(ex, false);
            }
            //WriteLog(LogType.METHOD_OUT, string.Format("{0}.Write16 : {1}", THIS_NAME, (ErrorCodeList)rc));
            return rc;
        }
        /// <summary>
        /// 単一デバイスにパルス出力
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <returns></returns>
        public UInt32 WritePulse(DeviceType deviceType, UInt32 addr)
        {
            UInt32 rc = 0;
            //WriteLog(LogType.METHOD_IN, string.Format("{0}.Write16({1},{2},{3})", THIS_NAME, deviceType, addr, value));
            try
            {
                if (_PLC == null) rc = (UInt32)ErrorCodeList.PLC_NOT_INITIALIZED;

                if (STATUS_SUCCESS(rc))
                {
                    rc = _PLC.Write(deviceType, addr, 0);
                    System.Threading.Thread.Sleep(20);
                    rc = _PLC.Write(deviceType, addr, 1);
                    System.Threading.Thread.Sleep(20);
                    rc = _PLC.Write(deviceType, addr, 0);

                }
            }
            catch (Exception ex)
            {
                rc = (UInt32)ErrorCodeList.PLC_WRITE_ERROR;
                ErrorHandler(ex, false);
            }
            //WriteLog(LogType.METHOD_OUT, string.Format("{0}.Write16 : {1}", THIS_NAME, (ErrorCodeList)rc));
            return rc;
        }
        /// <summary>
        /// 単一デバイス書込み
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Write32(DeviceType deviceType, UInt32 addr, Int32 value)
        {
            UInt32 rc = 0;
            //WriteLog(LogType.METHOD_IN, string.Format("{0}.Write({1},{2},{3})", THIS_NAME, deviceType, addr, value));
            try
            {
                if (_PLC == null) rc = (UInt32)ErrorCodeList.PLC_NOT_INITIALIZED;

                if (STATUS_SUCCESS(rc))
                {
                    rc = _PLC.Write32(deviceType, addr, value);
                }
            }
            catch (Exception ex)
            {
                rc = (UInt32)ErrorCodeList.PLC_WRITE_ERROR;
                ErrorHandler(ex, false);
            }
            //WriteLog(LogType.METHOD_OUT, string.Format("{0}.Write : {1}", THIS_NAME, (ErrorCodeList)rc));
            return rc;
        }
        /// <summary>
        /// 連続デバイス書込み
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="count">個数</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Write(DeviceType deviceType, UInt32 addr, Int32 count, Int32[] value)
        {
            UInt32 rc = 0;
            //WriteLog(LogType.METHOD_IN, string.Format("{0}.Write({1},{2},{3})", THIS_NAME, deviceType, addr, count));
            try
            {
                if (_PLC == null) rc = (UInt32)ErrorCodeList.PLC_NOT_INITIALIZED;

                if (STATUS_SUCCESS(rc))
                {
                    rc = _PLC.Write(deviceType, addr, count, value);
                }
            }
            catch (Exception ex)
            {
                rc = (UInt32)ErrorCodeList.PLC_READ_ERROR;
                ErrorHandler(ex, false);
            }
            //WriteLog(LogType.METHOD_OUT, string.Format("{0}.Write : {1}", THIS_NAME, (ErrorCodeList)rc));
            return rc;
        }
        /// <summary>
        /// 連続デバイス書込み(32bit)
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="count">個数</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Write32(DeviceType deviceType, UInt32 addr, Int32 count, Int32[] value)
        {
            UInt32 rc = 0;
            //WriteLog(LogType.METHOD_IN, string.Format("{0}.Write32({1},{2},{3})", THIS_NAME, deviceType, addr, count));
            try
            {
                if (_PLC == null) rc = (UInt32)ErrorCodeList.PLC_NOT_INITIALIZED;
                int wordCount = count * 2;
                Int32[] temp = new Int32[wordCount];

                int index = 0;
                for (int i = 0; i < wordCount; i+=2)
                {
                    temp[i]     = value[index] & 0xFFFF;
                    temp[i+1]   = (value[index]>>16) & 0xFFFF;
                    index++;
                }

                if (STATUS_SUCCESS(rc))
                    rc = _PLC.Write(deviceType, addr, wordCount, temp);
            }
            catch (Exception ex)
            {
                rc = (UInt32)ErrorCodeList.PLC_READ_ERROR;
                ErrorHandler(ex, false);
            }
            //WriteLog(LogType.METHOD_OUT, string.Format("{0}.Write32 : {1}", THIS_NAME, (ErrorCodeList)rc));
            return rc;
        }
        /// <summary>
        /// 文字書込み
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="len">最大文字数</param>
        /// <param name="buf">文字列</param>
        /// <returns></returns>
        public UInt32 Write(DeviceType deviceType, UInt32 addr, string buf)
        {
            UInt32 rc = 0;
            //WriteLog(LogType.METHOD_IN, string.Format("{0}.WriteText({1},{2},{3})", THIS_NAME, deviceType, addr, buf));
            try
            {
                if (_PLC == null) rc = (UInt32)ErrorCodeList.PLC_NOT_INITIALIZED;

                if (STATUS_SUCCESS(rc))
                {
                    rc = _PLC.Write(deviceType, addr, buf);
                }
            }
            catch (Exception ex)
            {
                rc = (UInt32)ErrorCodeList.PLC_WRITE_ERROR;
                ErrorHandler(ex, false);
            }
            //WriteLog(LogType.METHOD_OUT, string.Format("{0}.WriteText : {1}", THIS_NAME, (ErrorCodeList)rc));
            return rc;
        }

        /// <summary>
        /// エラー有無確認
        /// </summary>
        /// <param name="rc"></param>
        /// <returns></returns>
        private bool STATUS_SUCCESS(UInt32 rc)
        {
            return rc == 0;
        }

        /// <summary>
        /// ログ書込み
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        /// <param name="dispBottomMessage"></param>
        private void WriteLog(LogType type, string msg, bool dispBottomMessage = false)
        {
            Logger.WriteLog(type,msg);
        }

        /// <summary>
        /// 異常処理
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public static void ErrorHandler(Exception ex, bool dispWindow = false)
        {
            System.Diagnostics.Debug.Print(ex.Message);
            //SystemStatus.ErrorHandler(ex, dispWindow);
        }
    }
}
