using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Threading;
using System.IO.Ports;

namespace DL_CommonLibrary
{
    public static class SystemDevice
    {

        /// <summary>
        ///  System.Managementを使用してデバイスの取得
        /// </summary>
        public static void GetDevice()
        {
            // 情報取得コマンド
            ManagementObjectSearcher MyOCS = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");
            // 取得Object格納
            ManagementObjectCollection MyOC;

            //接続されてるPnPDeviceの総数
            int CountDevice = MyOCS.Get().Count;

            //1Scan前のPnPDeviceの総数
            int CountDeviceBefour = CountDevice;

            MyOC = MyOCS.Get();
            CountDevice = MyOC.Count;

            string[] Array_DeviceID;//取得ID分解用配列
            foreach (ManagementObject MyObject in MyOC)
            {
                Array_DeviceID = MyObject["DeviceID"].ToString().Split('\\');
                System.Diagnostics.Debug.Print(Array_DeviceID[0] + "," + Array_DeviceID[1] + "," + Array_DeviceID[2]);
                if (Array_DeviceID[0].Contains("USB"))
                {
                    //ここで検出時の処理を行う。
                }
            }


            if (CountDevice != CountDeviceBefour)
            {
                //ここでデバイス数変化時の処理を行う。

            }

            CountDeviceBefour = CountDevice;
        }


        public static bool IsPortExist(string portName)
        {
            bool exist = false;
            try
            {
                
                string[] ports = SerialPort.GetPortNames();
                int index = Array.IndexOf(ports, portName);
                if (index >= 0) exist = true;
            }
            catch { }
            return exist;
        }
    }
}
