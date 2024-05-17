using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using ErrorCodeDefine;

namespace DL_CommonLibrary
{
    public static class NetMisc
    {

        static System.Threading.CountdownEvent _pingCountDownEv = new System.Threading.CountdownEvent(0);

        /// <summary>
        /// IPアドレスとして正しい文字列化確認(IP4)
        /// </summary>
        /// <param name="ipaddr"></param>
        /// <returns></returns>
        public static bool IsIPAddress(string ipaddr)
        {
            bool ok = true;
            if (ipaddr == null) return false;
            ipaddr = ipaddr.Trim();

            // 4つ確認
            string[] separate = ipaddr.Split('.');
            if (ok && separate.Length != 4) ok = false;

            // 数値か確認
            for (int i = 0; i < separate.Length; i++)
            {
                int v = 0;
                if (!ok) break;
                ok = int.TryParse(separate[i], out v);
            }

            return ok;
        }
        /// <summary>
        /// 自ＰＣのＩＰアドレスを取得
        /// ※１番目のIPアドレスだけ取得
        /// </summary>
        /// <returns></returns>
        public static string GetIPAddress()
        {
            string ipaddress = "";
            try
            {

                IPHostEntry ipentry = Dns.GetHostEntry(Dns.GetHostName());

                foreach (IPAddress ip in ipentry.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        ipaddress = ip.ToString();
                        break;
                    }
                }
            }
            catch { }
            return ipaddress;
        }
        /// <summary>
        /// 指定したIPアドレスが自PCのIPアドレスに設定されているか確認
        /// </summary>
        /// <param name="ipAddr">IPアドレス</param>
        /// <returns></returns>
        public static bool IsExistIpAddress(string ipAddr)
        {
            try
            {
                IPHostEntry ipentry = Dns.GetHostEntry(Dns.GetHostName());

                foreach (IPAddress ip in ipentry.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        if (ip.ToString() == ipAddr)
                            return true;
                    }
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// サーバーポートのステータスを取得する
        /// ネットワークが切断されたら Listen => Establishedになる
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public static TcpState GetServerPortStatus(string ip, int port)
        {
            TcpState status = TcpState.Unknown;
            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            //IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            
            //TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Address.ToString() == ip && tcpi.LocalEndPoint.Port == port)
                {
                    return tcpi.State;
                }
            }
            return status;
        }



        /// <summary>
        /// Ping送信
        /// </summary>
        /// <param name="targetIP"></param>
        /// <returns></returns>
        public static UInt32 Ping(string targetIP, int timeout = 5000)
        {
            UInt32 rc = 0;
            //Pingオブジェクトの作成
            System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
            try
            {
                // Pingを送信する
                System.Net.NetworkInformation.PingReply reply = p.Send(targetIP, timeout);

                //結果を取得
                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    //Console.WriteLine("Reply from {0}:bytes={1} time={2}ms TTL={3}", reply.Address, reply.Buffer.Length, reply.RoundtripTime, reply.Options.Ttl);
                }
                else
                {
                    Console.WriteLine("Ping送信に失敗。({0} : {1})", targetIP, reply.Status);
                    rc = (UInt32)ErrorCodeList.NET_NOT_CONNECTION;
                }
            }
            catch(Exception ex)
            {
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            p.Dispose();
            return rc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetList"></param>
        /// <returns></returns>
        public static void Ping(string[] targetList, int timeout , out bool[] connected)
        {
            UInt32 rc = 0;
            int count = targetList.Length;
            connected = new bool[count];
            bool[] b = new bool[targetList.Length];
            Parallel.For(0, count, i =>
            {
                b[i] = Ping(targetList[i], timeout) == 0;
            });

            for (int i = 0; i < count; i++)
            {
                connected[i] = b[i];
            }

        }


    }
}
