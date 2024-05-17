//----------------------------------------------------------
// Copyright © 2017 DATALINK
//----------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace DL_Socket
{
    public delegate string SERVER_RECEIVED_CALLBACK(String strData);
    public delegate string SERVER_RECEIVED_CALLBACK2(String pSocketID, Byte[] byData);
    public delegate string SERVER_RECEIVED_CALLBACK3(String pSocketID, String strData);
    public delegate string SERVER_RECEIVED_CALLBACK4(String pSocketID, String pDataType, Byte[] pData, ref Socket pSocket);

    public delegate void SERVER_CONNECT_ACCEPT();
    public class SckServer
    {
        #region "variables/instances"
        private SERVER_RECEIVED_CALLBACK cbServerReceiveData;
        private SERVER_RECEIVED_CALLBACK2 cbServerReceiveData2;
        private SERVER_RECEIVED_CALLBACK3 cbServerReceiveData3;
        private SERVER_RECEIVED_CALLBACK4 cbServerReceiveData4;
        public SERVER_CONNECT_ACCEPT cbConnectCallback;
        private Socket sckServer;
        private String strEndString;
        private AsyncCallback asynCB;
        private Byte[] byDataBuff = new Byte[0];
        private Common common = new Common();
        private String strSocketID = "";
        private String ipAdd;

        private ConcurrentDictionary<string, Byte[]> _backupBuffer = new ConcurrentDictionary<string, byte[]>();

        private class SocketData
        {
            public Socket mySocket;
            public Byte[] byData = new Byte[1024];
        }

        /// <summary>
        /// 
        /// </summary>
        private object _connectionLock = new object();
        #endregion

        #region "property"
        public string SocketID
        {
            set { strSocketID = value; }
        }
        #endregion

        #region "constructor"
        public SckServer(String pIPAddress, int pPort, int pBackLog, String EndString, SERVER_RECEIVED_CALLBACK cb, out int RetVal)
        {
            IPEndPoint ipe      = new IPEndPoint(IPAddress.Parse(pIPAddress), pPort);
            cbServerReceiveData = cb;
            strEndString        = EndString;
            RetVal = StartServer(ipe, pBackLog);
        }
        public SckServer(String pSocketID, String pIPAddress, int pPort, int pBackLog, String EndString, SERVER_RECEIVED_CALLBACK2 cb, out int RetVal)
        {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(pIPAddress), pPort);
            strSocketID = pSocketID;
            cbServerReceiveData2 = cb;
            strEndString = EndString;
            RetVal = StartServer(ipe, pBackLog);
        }
        
        public SckServer(String pSocketID, String pIPAddress, int pPort, int pBackLog, String EndString, SERVER_RECEIVED_CALLBACK3 cb, out int RetVal)
        {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(pIPAddress), pPort);
            strSocketID = pSocketID;
            cbServerReceiveData3 = cb;
            strEndString = EndString;
            RetVal = StartServer(ipe, pBackLog);
        }

        public SckServer(String pSocketID, String pIPAddress, int pPort, int pBackLog, String EndString, SERVER_RECEIVED_CALLBACK4 cb, out int RetVal)
        {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(pIPAddress), pPort);
            ipAdd = pIPAddress;
            strSocketID = pSocketID;
            cbServerReceiveData4 = cb;
            strEndString = EndString;
            RetVal = StartServer(ipe, pBackLog);
        }
        #endregion

        #region "methods"
        /////////////////////////////////////////////////////////////////
        // Close listening socket
        public void CloseServer()
        {
            if (sckServer != null)
            {
                sckServer.Close();
                sckServer = null;
            }
            _backupBuffer.Clear();
        }
        //
        /////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////
        // start and open a socket to listen to a specific port
        private int StartServer(IPEndPoint pIpe, int iBackLog)
        {
            int Status = 0;
            sckServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _backupBuffer.Clear();
                sckServer.Bind(pIpe);
                //sckServer.SetSocketOption(SocketOptionLevel.IP,SocketOptionName.AddMembership,new MulticastOption(IPAddress.Parse(ipAdd)));
                sckServer.Listen(iBackLog);

                //Begin Asynchronous Callback Operation
                sckServer.BeginAccept(new AsyncCallback(onClientConnect), null);
            }
            catch (SocketException)
            {
                sckServer.Close();
                sckServer = null;
                Status = -1;
            }
            return Status;
        }
        //
        /////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////
        // AsyncCallback on client connection
        private void onClientConnect(IAsyncResult asynRes)
        {
            try
            {
                lock (_connectionLock)
                {
                    //System.Diagnostics.Debug.Print("onClientConnect");
                    if (sckServer != null)
                    {
                        Socket sck = sckServer.EndAccept(asynRes);
                        _backupBuffer[sck.RemoteEndPoint.ToString()] = new byte[0];
                        WaitData(sck);
                        sckServer.BeginAccept(new AsyncCallback(onClientConnect), null);
                    }
                }
                if (cbConnectCallback != null) cbConnectCallback();
            }
            catch (ObjectDisposedException) { }
            catch (SocketException) 
            {
                sckServer.Close();
                sckServer = null;               
            }
        }
        //
        /////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////
        // wait for data to arrive from a connected client (BeginReceive)
        private void WaitData(Socket sck)
        {
            if (asynCB == null)
            {
                asynCB = new AsyncCallback(onDataReceived);
            }

            SocketData sckData = new SocketData();
            sckData.mySocket = sck;

            sck.BeginReceive(sckData.byData, 0, sckData.byData.Length, SocketFlags.None, asynCB, sckData);
        }
        //
        /////////////////////////////////////////////////////////////////

        /////////////////////////////////////////////////////////////////
        // AsyncCallBack for receiving data
        private void onDataReceived(IAsyncResult asynRes)
        {
            SocketData sckData = (SocketData)asynRes.AsyncState;
            //string strTemp = "";
            Byte[] byTemp = new Byte[0];

            if (sckData.mySocket != null)
            {
                try
                {
                    int irx = sckData.mySocket.EndReceive(asynRes);
                    byTemp = new Byte[irx];
                    if (irx == 0)
                    {
                        sckData.mySocket.Close();
                        sckData.mySocket = null;
                    }
                    else
                    {
#if false
                        //Data was successfully received
                        Array.Copy(sckData.byData, 0, byTemp, 0, irx);
                        string strTemp = System.Text.Encoding.Default.GetString(byTemp);
                        byDataBuff = appLibCommon.CombineByteArray(byDataBuff, byTemp);
                        if (strTemp.EndsWith(strEndString))
                        {
                            //send to callback
                            string strRetVal = cbServerReceiveData(Encoding.Default.GetString(byDataBuff));
                            byDataBuff = new Byte[0];

                            sckData.mySocket.Send(System.Text.Encoding.Default.GetBytes(strRetVal));
                        }
                        WaitData(sckData.mySocket);
#else
                        Socket sckRcv = sckData.mySocket;

                        //Data was successfully received
                        Array.Copy(sckData.byData, 0, byTemp, 0, irx);
                       // if (irx > 70) irx = irx;
                        //string strTemp = System.Text.Encoding.Default.GetString(byTemp);                        
                        byte[] backupBuf = _backupBuffer[sckData.mySocket.RemoteEndPoint.ToString()];

                        //DMM 2008.01.15
                        string strTemp = common.ByteArrayToString(byTemp);
                        backupBuf = common.CombineByteArray(backupBuf, byTemp);
                        _backupBuffer[sckData.mySocket.RemoteEndPoint.ToString()] = backupBuf;
                        //DMM 2008.01.11
                        //string strTemp = appLibCommon.ByteArrayToString(byTemp);
                        //byDataBuff = appLibCommon.CombineByteArray(byDataBuff, byTemp);
                        if (cbServerReceiveData4 != null)
                        {
                            //String[] arrParse = common.ByteArrayToString(byDataBuff).Split(';');
                            String[] arrParse = common.ByteArrayToString(backupBuf).Split(';');
                            if (arrParse.Length > 3)
                            {
                                String sData = "";
                                for (int i = 3; i < arrParse.Length; i++)
                                {
                                    if (sData == "")
                                        sData = arrParse[i];
                                    else
                                        sData = sData + ";" + arrParse[i];
                                }
                                if (Convert.ToUInt32(arrParse[2]) == sData.Length - (strEndString.Length))
                               {
                                    WaitData(sckRcv);
                                    string strRetVal = "";
                                    Byte[] bySckData = common.StringToByteArray(sData.Substring(0, sData.Length - strEndString.Length));
                                    strRetVal = cbServerReceiveData4(arrParse[0].ToLower().Replace("cmd(", ""), arrParse[1], bySckData, ref sckData.mySocket);
                                    byDataBuff = new Byte[0];
                                    _backupBuffer[sckData.mySocket.RemoteEndPoint.ToString()] = new Byte[0];
                                    if (strRetVal != "")
                                    {
                                        sckData.mySocket.Send(System.Text.Encoding.Default.GetBytes(strRetVal));
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (strTemp.EndsWith(strEndString))
                            {
                                string strRetVal = "";
                                //Byte[] byData = byDataBuff;
                                Byte[] byData = backupBuf;
                                byDataBuff = new Byte[0];
                                _backupBuffer[sckData.mySocket.RemoteEndPoint.ToString()] = new Byte[0];
                                //send data to callback
                                if (strSocketID == "")
                                {
                                    if (byData.Length > 100) irx = irx;
                                    //WaitData(sckRcv);
                                    strRetVal = cbServerReceiveData(Encoding.Default.GetString(byData));
                                    if (strRetVal != "")
                                    {
                                        sckData.mySocket.Send(System.Text.Encoding.Default.GetBytes(strRetVal));
                                    }

                                    WaitData(sckRcv);
                                    return;
                                }
                                else
                                {
                                    if (cbServerReceiveData2 != null)
                                    {
                                        cbServerReceiveData2(strSocketID, byData);

                                        if (strRetVal != "")
                                        {
                                            sckData.mySocket.Send(System.Text.Encoding.Default.GetBytes(strRetVal));
                                        }

                                        WaitData(sckRcv);
                                        return;
                                    }
                                    else if (cbServerReceiveData3 != null)
                                    {
                                         //@@temp
                                        //WaitData(sckRcv);
                                        strRetVal = cbServerReceiveData3(strSocketID, common.ByteArrayToString(byData));

                                        if (strRetVal != "")
                                        {
                                            sckData.mySocket.Send(System.Text.Encoding.Default.GetBytes(strRetVal));
                                        }

                                        WaitData(sckRcv);
                                        return;
                                    }
                                }

                                if (strRetVal != "")
                                {
                                    sckData.mySocket.Send(System.Text.Encoding.Default.GetBytes(strRetVal));
                                }
                            }
                            //added 20080623
                            else
                            {
                                WaitData(sckRcv);
                            }
                        }


                        //WaitData(sckData.mySocket);                        
#endif
                    }
                }
                catch (SocketException)
                {
                    sckData.mySocket.Close();
                    sckData.mySocket = null;
                }
                catch (InvalidOperationException)
                {
                    sckData.mySocket.Close();
                    sckData.mySocket = null;
                }
            }
        }
        //
        /////////////////////////////////////////////////////////////////

        #endregion
    }
}

