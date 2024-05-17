using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorCodeDefine;

namespace DL_CommonLibrary
{
    /// <summary>
    /// アプリ起動など
    /// </summary>
    public class ShellMisc
    {
        /// <summary>
        /// 関連付けられたアプリケーションでファイルを開く
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static UInt32 OpenFileWithDefaultApp(string fileName)
        {
            UInt32 rc = 0;
            try
            {
                //"C:\test\1.txt"を関連付けられたアプリケーションで開く
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(fileName);
            }
            catch { rc = (UInt32)ErrorCodeList.EXCEPTION; }
            return rc;
        }
    }
}
