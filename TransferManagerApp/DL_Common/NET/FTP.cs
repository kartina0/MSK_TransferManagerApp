using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorCodeDefine;


namespace DL_CommonLibrary
{
    /// <summary>
    /// FTP Client機能
    /// 文字コードは Shift-JIS
    /// </summary>
    public class FTP_Client
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
        public FTP_Client(string hostName, string userName, string password)
        {
            _hostName       = hostName;
            _logInUserName  = userName;
            _logInPass      = password;
        }

        /// <summary>
        /// ファイルを削除する
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UInt32 DeleteFile(string path)
        {
            UInt32 rc = 0;
            try
            {
                // パスをURI形式に変換
                path = path.Replace('\\', '/');
                // URI作成
                string uri = string.Format("ftp://{0}/{1}", _hostName, path);

                // 削除するファイルのURI
                Uri u = new Uri(uri);

                // FtpWebRequestの作成
                System.Net.FtpWebRequest ftpReq = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(u);

                //ログインユーザー名とパスワードを設定
                ftpReq.Credentials = new System.Net.NetworkCredential(_logInUserName, _logInPass);
                //MethodにWebRequestMethods.Ftp.DeleteFile(DELE)を設定
                ftpReq.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;

                //FtpWebResponseを取得
                System.Net.FtpWebResponse ftpRes = (System.Net.FtpWebResponse)ftpReq.GetResponse();

                //FTPサーバーから送信されたステータスを表示
                Console.WriteLine("{0}: {1}", ftpRes.StatusCode, ftpRes.StatusDescription);

                //閉じる
                ftpRes.Close();

            }
            catch(Exception ex) { rc = (UInt32)ErrorCodeList.EXCEPTION; }

            return rc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uriDir">ファイル一覧を取得するディレクトリのURI</param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string[] GetFileList(string path)
        {
            List<string> list = new List<string>();
            try
            {
                // パスをURI形式に変換
                path = path.Replace('\\','/');
                // URI作成
                string uri = string.Format("ftp://{0}/{1}", _hostName, path);

                //ファイル一覧を取得するディレクトリのURI
                Uri u = new Uri(uri);

                // FtpWebRequestの作成
                System.Net.FtpWebRequest ftpReq = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(u);

                //ログインユーザー名とパスワードを設定
                ftpReq.Credentials = new System.Net.NetworkCredential(_logInUserName, _logInPass);

                //MethodにWebRequestMethods.Ftp.ListDirectoryDetails("LIST")を設定
                //ftpReq.Method = System.Net.WebRequestMethods.Ftp.ListDirectoryDetails;
                ftpReq.Method = System.Net.WebRequestMethods.Ftp.ListDirectory;
                
                //要求の完了後に接続を閉じる
                ftpReq.KeepAlive = false;
                //PASSIVEモードを無効にする
                ftpReq.UsePassive = false;
                
                //FtpWebResponseを取得
                System.Net.FtpWebResponse ftpRes = (System.Net.FtpWebResponse)ftpReq.GetResponse();

                //FTPサーバーから送信されたデータを取得
                System.IO.StreamReader sr = new System.IO.StreamReader(ftpRes.GetResponseStream(), _encoding);

                while (!sr.EndOfStream)
                {
                    string fileName = sr.ReadLine();
                    if (fileName != "") list.Add(fileName);
                }
                sr.Close();
                
                //FTPサーバーから送信されたステータスを表示
                Console.WriteLine("{0}: {1}", ftpRes.StatusCode, ftpRes.StatusDescription);
                //閉じる
                ftpRes.Close();

            }
            catch(Exception ex)
            { }
            return list.ToArray();
        }

        /// <summary>
        /// サーバーからファイルをダウンロードする
        /// </summary>
        /// <param name="srcPath">ダウンロード元のファイルパス(FTPサーバー側)</param>
        /// <param name="destPath">ダウンロード先のファイルパス(ローカルファイル)</param>
        /// <returns></returns>
        public UInt32 DownLoadFile(string srcPath, string destPath)
        {
            UInt32 rc = 0;
            try
            {
                // パスをURI形式に変換
                srcPath = srcPath.Replace('\\', '/');
                // URI作成
                string uri = string.Format("ftp://{0}/{1}", _hostName, srcPath);

                //ダウンロードするファイルのURI
                Uri u = new Uri(uri);
                //ダウンロードしたファイルの保存先
                string downFile = destPath;

                //FtpWebRequestの作成
                System.Net.FtpWebRequest ftpReq = (System.Net.FtpWebRequest)
                    System.Net.WebRequest.Create(u);
                //ログインユーザー名とパスワードを設定
                ftpReq.Credentials = new System.Net.NetworkCredential(_logInUserName, _logInPass);
                //MethodにWebRequestMethods.Ftp.DownloadFile("RETR")を設定
                ftpReq.Method = System.Net.WebRequestMethods.Ftp.DownloadFile;
                //要求の完了後に接続を閉じる
                ftpReq.KeepAlive = false;
                //ASCIIモードで転送する
                ftpReq.UseBinary = false;
                //PASSIVEモードを無効にする
                ftpReq.UsePassive = false;

                //FtpWebResponseを取得
                System.Net.FtpWebResponse ftpRes = (System.Net.FtpWebResponse)ftpReq.GetResponse();
                //ファイルをダウンロードするためのStreamを取得
                System.IO.Stream resStrm = ftpRes.GetResponseStream();
                //ダウンロードしたファイルを書き込むためのFileStreamを作成
                System.IO.FileStream fs = new System.IO.FileStream(downFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                //ダウンロードしたデータを書き込む
                byte[] buffer = new byte[1024];
                while (true)
                {
                    int readSize = resStrm.Read(buffer, 0, buffer.Length);
                    if (readSize == 0)
                        break;
                    fs.Write(buffer, 0, readSize);
                }
                fs.Close();
                resStrm.Close();

                //FTPサーバーから送信されたステータスを表示
                Console.WriteLine("{0}: {1}", ftpRes.StatusCode, ftpRes.StatusDescription);
                //閉じる
                ftpRes.Close();
            }
            catch { rc = (UInt32)ErrorCodeList.EXCEPTION; }
            return rc;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcPath">アップロードするファイル(ローカルファイル)</param>
        /// <param name="destPath">アップロード先のファイルパス(FTPサーバー側)</param>
        /// <returns></returns>
        public UInt32 UpLoadFile(string srcPath, string destPath)
        {
            UInt32 rc = 0;
            try
            {
                // パスをURI形式に変換
                destPath = destPath.Replace('\\', '/');
                // URI作成
                string uri = string.Format("ftp://{0}/{1}", _hostName, destPath);

                //アップロードするファイル
                string upFile = srcPath;
                //アップロード先のURI
                Uri u = new Uri(uri);

                //FtpWebRequestの作成
                System.Net.FtpWebRequest ftpReq = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(u);
                //ログインユーザー名とパスワードを設定
                ftpReq.Credentials = new System.Net.NetworkCredential(_logInUserName, _logInPass);
                //MethodにWebRequestMethods.Ftp.UploadFile("STOR")を設定
                ftpReq.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                //要求の完了後に接続を閉じる
                ftpReq.KeepAlive = false;
                //ASCIIモードで転送する
                ftpReq.UseBinary = false;
                //PASVモードを無効にする
                ftpReq.UsePassive = false;

                //ファイルをアップロードするためのStreamを取得
                System.IO.Stream reqStrm = ftpReq.GetRequestStream();
                //アップロードするファイルを開く
                System.IO.FileStream fs = new System.IO.FileStream(upFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //アップロードStreamに書き込む
                byte[] buffer = new byte[1024];
                while (true)
                {
                    int readSize = fs.Read(buffer, 0, buffer.Length);
                    if (readSize == 0)
                        break;
                    reqStrm.Write(buffer, 0, readSize);
                }
                fs.Close();
                reqStrm.Close();

                //FtpWebResponseを取得
                System.Net.FtpWebResponse ftpRes = (System.Net.FtpWebResponse)ftpReq.GetResponse();
                //FTPサーバーから送信されたステータスを表示
                Console.WriteLine("{0}: {1}", ftpRes.StatusCode, ftpRes.StatusDescription);
                //閉じる
                ftpRes.Close();
            }
            catch(Exception ex) { rc = (UInt32)ErrorCodeList.EXCEPTION; }
            return rc;
        }


        public void TEST()
        {
            //WebClientオブジェクトを作成
            System.Net.WebClient wc = new System.Net.WebClient();
            //ログインユーザー名とパスワードを指定
            wc.Credentials = new System.Net.NetworkCredential("username", "password");
           

            //FTPサーバーからダウンロードする
            wc.DownloadFile("ftp://localhost/test.txt", @"C:\test.txt");
            //解放する
            wc.Dispose();
        }
    }
}
