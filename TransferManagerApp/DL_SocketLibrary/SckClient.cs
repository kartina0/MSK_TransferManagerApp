//----------------------------------------------------------
// Copyright © 2017 DATALINK
//----------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
namespace DL_Socket
{

    public delegate void CLIENT_RECEIVED_CALLBACK(String strData);
    public delegate void CLIENT_RECEIVED_CALLBACK2(String pDevID, String pDataType, String strData);

    public class SckClient
    {
        #region "variables/instances"
        private IPEndPoint ipe;
        private CLIENT_RECEIVED_CALLBACK cbClientReceiveData;
        private CLIENT_RECEIVED_CALLBACK2 cbClientReceiveData2;
        private Socket sckClient;
        private AsyncCallback pfnCallBack;
        private string strEndString;
        private String DevID;

        private Byte[] byDataBuff = new Byte[0];
        private Common common = new Common();

        private Boolean isConnected = false;
        private String mIPAddress = "";
        private int mPort;
        private class SocketData
        {
            public Socket mySocket;
            public Byte[] byData = new Byte[1024];
        }
        #endregion

        #region "public property"
        public Boolean Connected
        {
            get { return isConnected; }
        }

        public String ServerIP
        {
            get { return mIPAddress; }
        }

        public int ServerPort
        {
            get { return mPort; }
        }

        #endregion

        #region "constructor"
        public SckClient(String pIPAddress, int pPort, String EndString, CLIENT_RECEIVED_CALLBACK cb, out int RetVal)
        {
            ipe = new IPEndPoint(IPAddress.Parse(pIPAddress), pPort);
            cbClientReceiveData = cb;
            strEndString = EndString;
            RetVal = StartClient();
            if (RetVal == 0)
            {
                isConnected = true;
                mIPAddress = pIPAddress;
                mPort = pPort;
                //WaitData();
            }
            else
            {
                isConnected = false;
                //MessageBox.Show("Cannot connect to " + pIPAddress + ":" + pPort.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public SckClient(String pDevID, String pIPAddress, int pPort, String EndString, CLIENT_RECEIVED_CALLBACK2 cb, out int RetVal)
        {
            DevID = pDevID;
            ipe = new IPEndPoint(IPAddress.Parse(pIPAddress), pPort);
            cbClientReceiveData2 = cb;
            strEndString = EndString;
            RetVal = StartClient();
            if (RetVal == 0)
            {
                isConnected = true;
                mIPAddress = pIPAddress;
                mPort = pPort;
                //WaitData();
            }
            else
            {
                isConnected = false;
                //MessageBox.Show("Cannot connect to " + pIPAddress + ":" + pPort.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public SckClient(String pIPAddress, int pPort, out int RetVal)
        {
            ipe = new IPEndPoint(IPAddress.Parse(pIPAddress), pPort);
            RetVal = StartClient();
            if (RetVal == 0)
            {
                isConnected = true;
                mIPAddress = pIPAddress;
                mPort = pPort;
            }
            else
            {
                isConnected = false;
            }
        }
        #endregion

        #region "methods"
        /////////////////////////////////////////////////////////////////
        // Send Data
        public int SendData(Byte[] byData)
        {
            try
            {
                sckClient.Send(byData);
                return 0;
            }
            catch (SocketException)
            {
                return -1;
            }
            catch (NullReferenceException)
            {
                return -2;
            }
        }

        public int SendData(String strData)
        {
            //Byte[] byData = System.Text.Encoding.Default.GetBytes(strData);

            //DMM 2008.01.15
            //Byte[] byData = appLibCommon.StringToByteArray(strData);
            Byte[] byData = Encoding.Default.GetBytes(strData);
            try
            {
                sckClient.Send(byData);
                return 0;
            }
            catch (SocketException)
            {
                return -1;
            }
            catch (NullReferenceException)
            {
                return -2;
            }
            catch (ObjectDisposedException)
            {
                return -3;
            }
        }
        //
        /////////////////////////////////////////////////////////////////


        /////////////////////////////////////////////////////////////////
        // Close client socket
        public int CloseConnection()
        {
            isConnected = false;
            try
            {
                sckClient.Close();
                sckClient = null;
                return 0;
            }
            catch (SocketException)
            {
                return -1;
            }
            catch (NullReferenceException)
            {
                return -2;
            }
        }
        //
        /////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////
        // Start client application
        public int StartClient()
        {

            int Status = 0;
 
            sckClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                if (isConnected == false)
                {
                    sckClient.Connect(ipe);
                    isConnected = true;
                    WaitData();
                }
            }
            catch (SocketException)
            {
                isConnected = false;
                Status = -1;
                isConnected = false;
            }
            return Status;
        }
        //
        /////////////////////////////////////////////////////////////////        

        /////////////////////////////////////////////////////////////////        
        // Wait for data coming from server (BeginReceive) 
        private void WaitData()
        {
            SocketData sckData = new SocketData();
            try
            {
                if (pfnCallBack == null)
                    pfnCallBack = new AsyncCallback(onDataRecieved);

                sckData.mySocket = sckClient;
                if (sckClient != null)      // @RFS 20080506 Start receiving if sckClient is not null
                {
                    try
                    {
                        sckClient.BeginReceive(sckData.byData, 0, sckData.byData.Length, SocketFlags.None, pfnCallBack, sckData);
                    }
                    catch { }
                }

            }
            catch (SocketException)
            {
                sckData.mySocket.Close();
                sckData.mySocket = null;
            }
        }
        //
        /////////////////////////////////////////////////////////////////        

        /////////////////////////////////////////////////////////////////        
        // Start receiving
        private void onDataRecieved(IAsyncResult asyn)
        {
            SocketData sckData = (SocketData)asyn.AsyncState;
            Byte[] byTemp = new Byte[0];

            if (sckData.mySocket != null)
            {
                try
                {
                    if (!sckData.mySocket.Connected) return;
                    int irx = sckData.mySocket.EndReceive(asyn);
                    byTemp = new Byte[irx];
                    if (irx == 0)
                    {
                        sckData.mySocket.Close();
                        sckData.mySocket = null;
                    }
                    else
                    {
                        //Data was successfully received
                        Array.Copy(sckData.byData, 0, byTemp, 0, irx);
                        string strTemp = System.Text.Encoding.Default.GetString(byTemp);
                        byDataBuff = common.CombineByteArray(byDataBuff, byTemp);


                        if (cbClientReceiveData2 != null)
                        {

                            String[] arrParse = common.ByteArrayToString(byDataBuff).Split(';');
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
                                if (Convert.ToUInt32(arrParse[2]) == sData.Length - strEndString.Length)
                                {
                                    if (cbClientReceiveData != null)
                                    {
                                        cbClientReceiveData(common.ByteArrayToString(byDataBuff));
                                    }
                                    else if (cbClientReceiveData2 != null)
                                    {
                                        cbClientReceiveData2(arrParse[0].ToLower().Replace("cmd(", ""), arrParse[1], sData.Substring(0, sData.Length - strEndString.Length));
                                    }
                                    byDataBuff = new Byte[0];
                                }
                            }
                        }
                        else
                        {
                            if (strTemp.EndsWith(strEndString))
                            {
                                //send to callback
                                cbClientReceiveData(Encoding.Default.GetString(byDataBuff));
                                byDataBuff = new Byte[0];
                                //sckData.mySocket.Send(System.Text.Encoding.Default.GetBytes(strRetVal));
                            }


                        }

                        //if (strTemp.EndsWith(strEndString))
                        //{
                        //    //send to callback
                        //    cbClientReceiveData(Encoding.Default.GetString(byDataBuff));
                        //    byDataBuff = new Byte[0];
                        //    //sckData.mySocket.Send(System.Text.Encoding.Default.GetBytes(strRetVal));
                        //}

                        WaitData();
                    }
                }
                catch (ObjectDisposedException)
                {
                    sckData.mySocket.Close();
                    sckData.mySocket = null;
                }
                catch (SocketException)
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
