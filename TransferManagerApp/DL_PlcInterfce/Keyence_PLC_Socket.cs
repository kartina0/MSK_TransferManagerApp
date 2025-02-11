﻿// ----------------------------------------------
// Copyright © 2021 DATALINK
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using DL_CommonLibrary;
using ErrorCodeDefine;
using DL_Socket;

namespace DL_PlcInterfce
{
    class Keyence_PLC_Socket : IPlc
    {

        #region Private Variable


        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "PLC_IF";

        /// <summary>
        /// PLCレスポンス受信イベント
        /// </summary>
        private ManualResetEvent _respDetect = new ManualResetEvent(false);
        /// <summary>
        /// PLCレスポンスバッファ
        /// </summary>
        private volatile string _response = "";
        /// <summary>
        /// Sokcet Lockオブジェクト
        /// </summary>
        private object _sockLock = new object();

        /// <summary>
        /// 接続パラメータ
        /// </summary>
        private string _connectionParam = "";

        /// <summary>
        /// 接続状態
        /// </summary>
        private bool _connected = false;

        /// <summary>
        /// PLC IPアドレス
        /// </summary>
        private string _ipAddress = "127.0.0.1";
        /// <summary>
        /// PLCﾎﾟｰﾄ番号
        /// </summary>
        private int _port = 8501;

        /// <summary>
        /// 通信ﾀｲﾑｱｳﾄ
        /// </summary>
        private int _timeout = 10000;

        /// <summary>
        /// 毎回接続・切断しないフラグ
        /// </summary>
        private bool _keepConnection = true;

        /// <summary>
        /// ソケット
        /// </summary>
        private SckClient _sock = null;

        #endregion

        /// <summary>
        /// ダミーモードか確認
        /// </summary>
        /// <returns></returns>
        public bool IsDummy()
        {
            return false;
        }
        /// <summary>
        /// 接続
        /// </summary>
        /// <param name="connectionParam">接続パラメータ</param>
        /// <returns></returns>
        public UInt32 Open(string connectionParam)
        {
            UInt32 rc = 0;

            _connectionParam = connectionParam;
            string[] param = connectionParam.Split(';');
            try
            {
                _ipAddress = param[2];
                _port = int.Parse(param[3]);

                int ret = 0;
                _sock = new SckClient(_ipAddress, _port, "\r\n", new CLIENT_RECEIVED_CALLBACK(_SocketRecv), out ret);
                if (ret == 0) _connected = true;
                if (_sock != null && !_keepConnection)
                {
                    _sock.CloseConnection();
                    _sock = null;
                }

                if (!_connected)
                    rc = (UInt32)ErrorCodeList.ROBOT_IF_NOT_INITIALIZED;

            }
            catch { rc = (UInt32)ErrorCodeList.PLC_INIT_ERROR; }

            return rc;
        }
        /// <summary>
        /// 切断
        /// </summary>
        /// <returns></returns>
        public UInt32 Close()
        {
            UInt32 rc = 0;
            try
            {
                if (_sock != null)
                {
                    _sock.CloseConnection();
                    _sock = null;
                }
            }
            catch { rc = (UInt32)ErrorCodeList.EXCEPTION; }

            return rc;
        }

        /// <summary>
        /// 接続確認
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return _connected;
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
            try
            {
                string cmd = "";
                string resp = "";
                string typeName = GetDeviceTypeName(deviceType);
                cmd = string.Format("RD {0}{1}", typeName, addr);
                rc = Execute(cmd, ref resp);

                if (STATUS_SUCCESS(rc))
                {
                    try
                    {
                        long temp = long.Parse(resp);
                        value = (int)temp;
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
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
            try
            {
                string cmd = "";
                string resp = "";
                string typeName = GetDeviceTypeName(deviceType);
                cmd = string.Format("RD {0}{1}.L", typeName, addr);
                rc = Execute(cmd, ref resp);

                if (STATUS_SUCCESS(rc))
                {
                    try
                    {
                        long temp = long.Parse(resp);
                        value = (Int32)temp;
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
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

            buf = "";

            string cmd = "";
            string resp = "";
            string typeName = GetDeviceTypeName(deviceType);
            int count = (int)((float)len / 2 + 0.5);
            int[] val = new int[count];
            byte[] tempBuf = new byte[len];
            cmd = string.Format("RDS {0}{1} {2}", typeName, addr, count);
            rc = Execute(cmd, ref resp);

            if (STATUS_SUCCESS(rc))
            {
                try
                {
                    string temp = resp.Trim(); ;

                    string[] items = temp.Split(' ');
                    for (int i = 0; i < items.Length; i++)
                    {
                        val[i] = Int32.Parse(items[i]);
                    }


                    //for (int i = 0; i < count; i++)
                    //{
                    //    temp = FileIo.DeleteAfter(resp, " ");
                    //    if (temp == "") break;

                    //    val[i] = Int32.Parse(temp);
                    //    resp = FileIo.DeleteBefore(resp, " ");
                    //}

                    int bIndex = 0;
                    for (int i = 0; i < count; i++)
                    {
                        tempBuf[bIndex++] = (byte)(val[i] >> 8 & 0xFF);
                        tempBuf[bIndex++] = (byte)(val[i] & 0xFF);

                    }
                    buf = Encoding.GetEncoding("Shift-JIS").GetString(tempBuf);
                    //buf = ASCIIEncoding.ASCII.GetString(tempBuf);
                    buf = FileIo.DeleteAfter(buf, "\0");

                }
                catch { }
            }
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

            try
            {
                string cmd = "";
                string resp = "";
                int totalSize = count;
                int readCount = 0;
                string typeName = GetDeviceTypeName(deviceType);

                while (true)
                {

                    if (!STATUS_SUCCESS(rc)) break;

                    // 一度に読み込めるのは1000ワードまで
                    int readSize = 0;
                    if (totalSize > 1000)
                        readSize = 1000;
                    else
                        readSize = totalSize % 1000;

                    if (readSize <= 0) break;

                    cmd = string.Format("RDS {0}{1} {2}", typeName, addr + readCount, readSize);
                    rc = Execute(cmd, ref resp);

                    if (STATUS_SUCCESS(rc))
                    {
                        try
                        {
                            string temp = resp.Trim(); ;
                            string[] items = temp.Split(' ');
                            for (int i = 0; i < items.Length; i++)
                            {
                                value[readCount + i] = Int32.Parse(items[i]);
                            }

                            //for (int i = 0; i < readSize; i++)
                            //{
                            //    temp = FileIo.DeleteAfter(resp, " ");
                            //    if (temp == "") break;

                            //    value[readCount + i] = Int32.Parse(temp);
                            //    resp = FileIo.DeleteBefore(resp, " ");
                            //}

                        }
                        catch { }
                    }

                    readCount += readSize;
                    totalSize -= readSize;

                }



            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        /// <summary>
        /// 単一デバイス書込み(16Bit)
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Write(DeviceType deviceType, UInt32 addr, int value)
        {
            UInt32 rc = 0;

            try
            {
                string cmd = "";
                string resp = "";
                string typeName = GetDeviceTypeName(deviceType);

                cmd = string.Format("WR {0}{1} {2}", typeName, addr, value);
                rc = Execute(cmd, ref resp);
            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }

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

            try
            {
                string cmd = "";
                string resp = "";
                string typeName = GetDeviceTypeName(deviceType);

                cmd = string.Format("WR {0}{1}.L {2}", typeName, addr, value);
                rc = Execute(cmd, ref resp);
            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }

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

            try
            {
                string cmd = "";
                string resp = "";
                string typeName = GetDeviceTypeName(deviceType);

                cmd = string.Format("WRS {0}{1} {2}", typeName, addr, count);
                for (int i = 0; i < count; i++)
                {
                    cmd += string.Format(" {0}", value[i]);
                }
                rc = Execute(cmd, ref resp);

            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }

            return rc;
        }

        /// <summary>
        /// 文字書込み
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="buf">文字列</param>
        /// <returns></returns>
        public UInt32 Write(DeviceType deviceType, UInt32 addr, string buf)
        {
            UInt32 rc = 0;
            try
            {
                string cmd = "";
                string resp = "";
                string typeName = GetDeviceTypeName(deviceType);

                int count = (int)((float)buf.Length / 2 + 0.5);
                int[] val = new int[count];
                byte[] byteBuf = Encoding.GetEncoding("Shift-JIS").GetBytes(buf);
                //byte[] byteBuf2 = Encoding.UTF8.GetBytes(buf);
                //byte[] byteBuf = ASCIIEncoding.ASCII.GetBytes(buf);
                string data = "";
                int bIndex = 0;
                cmd = string.Format("WRS {0}{1} {2}", typeName, addr, count);

                for (int i = 0; i < count; i++)
                {
                    if (byteBuf.Length > bIndex)
                        val[i] = (int)(byteBuf[bIndex++] << 8) | val[i];
                    if (byteBuf.Length > bIndex)
                        val[i] = (int)(byteBuf[bIndex++] | val[i]);

                    data += string.Format(" {0}", val[i]);
                }

                rc = Execute(cmd + data, ref resp);


            }
            catch (Exception ex)
            {
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }



            return rc;
        }

        /// <summary>
        /// ｺﾏﾝﾄﾞ送受信
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="resp"></param>
        /// <returns></returns>
        public UInt32 Execute(string cmd, ref string resp)
        {
            Int32 rs = 0;

            lock (_sockLock)
            {
                try
                {
                    _respDetect.Reset();
                    _response = "";
#if !DUMMY_IF
                    if (_sock == null)
                        _sock = new SckClient(_ipAddress, _port, "\r\n", new CLIENT_RECEIVED_CALLBACK(_SocketRecv), out rs);

                    if (_sock != null && _sock.Connected)
                    {
                        rs = _sock.SendData(string.Format("{0}\r", cmd));

                        if (rs == 0)
                        {
                            //System.Threading.Thread.Sleep(1);

                            // 受信待ち
                            if (!_respDetect.WaitOne(_timeout, false))
                            {
                                rs = (Int32)ErrorCodeList.TIMEOUT;
                            }
                        }

                        resp = _response;
                    }
#else
                    resp = "0";
#endif
                }
                catch (Exception ex)
                {
                    Close();
                    ErrorManager.ErrorHandler(ex);
                    rs = (Int32)ErrorCodeList.EXCEPTION;
                }
            }

            if (_sock != null && !_keepConnection)
            {
                Close();
            }
            return (UInt32)rs;
        }
        /// <summary>
        /// 受信イベント
        /// </summary>
        /// <param name="resp"></param>
        private void _SocketRecv(string resp)
        {
            _response += resp;
            _respDetect.Set();
        }

        /// <summary>
        /// デバイスタイプをコマンド用の文字列に変更
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        private string GetDeviceTypeName(DeviceType deviceType)
        {
            string name = "";
            if (deviceType == DeviceType.DataMemory)            name = "DM";
            else if (deviceType == DeviceType.ExtendDataMemory) name = "EM";
            else if (deviceType == DeviceType.FileRegister)     name = "ZF";
            else if (deviceType == DeviceType.InternalRelay)    name = "MR";
            else if (deviceType == DeviceType.LinkRegister)     name = "W";
            else if (deviceType == DeviceType.LinkRelay)        name = "B";
            else if (deviceType == DeviceType.Relay)            name = "R";
            else if (deviceType == DeviceType.Timer)            name = "T";
            else                                                name="";

            return name;
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

    }
}
