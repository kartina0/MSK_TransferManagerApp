using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_CommonLibrary.NET
{
    /// <summary>
    /// 三菱PLC FTPClientクラス
    /// 三菱はドライブ指定しないとSDカード内が見れないので
    /// FtpWebRequestクラスを利用できない
    /// </summary>
    public class MelsecFTP
    {
        /// <summary>
        /// 接続ホスト名
        /// </summary>
        private string _hostName = "192.168.0.10";

        /// <summary>
        /// FTPログインユーザー名
        /// </summary>
        private string _logInUserName = "";

        /// <summary>
        /// FTPログインパスワード
        /// </summary>
        private string _logInPass = "";

        /// <summary>
        /// 文字コード
        /// </summary>
        private Encoding _encoding = System.Text.Encoding.GetEncoding("shift_jis");
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public MelsecFTP(string hostName, string userName, string password)
        {
            _hostName = hostName;
            _logInUserName = userName;
            _logInPass = password;
        }
        
    }
}
