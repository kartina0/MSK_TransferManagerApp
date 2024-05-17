using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DL_CommonLibrary
{
    /// <summary>
    /// シリアルポート 関連関数
    /// </summary>
    public static class FuncSerialPort
    {
        /// <summary>
        /// 存在するCOMポートを取得する
        /// </summary>
        /// <returns></returns>
        public static string[] GetExistPortList()
        {
            return SerialPort.GetPortNames();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public static bool IsExist(string portName)
        {
            string[] portList = GetExistPortList();
            int index = Array.IndexOf(portList, portName);
            return index >= 0;
        }
    }
}
