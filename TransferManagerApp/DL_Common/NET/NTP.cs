using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using ErrorCodeDefine;

namespace DL_CommonLibrary
{
    /// <summary>
    /// 時刻同期
    /// </summary>
    public static class NTP
    {
        /// <summary>
        /// NTP 設定(サーバ)
        /// Windows7のNTPサーバー機能をONにする設定とサービスの起動を行う
        /// </summary>
        /// <returns></returns>
        public static UInt32 StartService()
        {
            UInt32 rs = 0;

            try
            {
                
                //Processオブジェクトを作成
                Process p = new Process();

                string results = ""; 

                //コマンドプロンプトでサービスを実行
                //ComSpec(cmd.exe)のパスを取得して、FileNameプロパティに指定
                p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
                //出力を読み取れるようにする
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = false;
                //ウィンドウを表示しないようにする
                p.StartInfo.CreateNoWindow = true;

                //W32Timeを停止
                //コマンドラインを指定（"/c"は実行後閉じるために必要）コマンドを書き込む
                p.StartInfo.Arguments = @"/c net stop w32time";
                //起動
                p.Start();
                //出力を読み取る
                results = p.StandardOutput.ReadToEnd();
                //プロセス終了まで待機する
                //WaitForExitはReadToEndの後である必要がある
                //(親プロセス、子プロセスでブロック防止のため)
                p.WaitForExit();
                p.Close();


                //レジストリの確認
                //レジストリの中身をアクセスするために必要なパスを書く
                string Enable_path = @"SYSTEM\CurrentControlSet\Services\W32Time\TimeProviders\NtpServer";
                string AnnounceFlags_path = @"SYSTEM\CurrentControlSet\Services\W32Time\Config";

                //レジストリの中身を開く
                //最後尾のbool値をfalseで読み取り専用にできる
                RegistryKey Enable_key = Registry.LocalMachine.OpenSubKey(Enable_path, true);
                RegistryKey AnnounceFlags_key = Registry.LocalMachine.OpenSubKey(AnnounceFlags_path, true);
                //レジストリキーがない場合はnullが返される
                if (Enable_key == null)
                {
                    Dialogs.ShowInformationMessage("Enableキーが存在していません", "NTP Service" , SystemIcons.Error);
                    rs = (UInt32)ErrorCodeList.NTP_NOT_REGISTORY_KEY;
                    return rs;
                }
                if (AnnounceFlags_key == null)
                {
                    Dialogs.ShowInformationMessage("AnnounceFlagsキーが存在していません", "NTP Service", SystemIcons.Error);
                    rs = (UInt32)ErrorCodeList.NTP_NOT_REGISTORY_KEY;
                    return rs;
                }

                //整数（REG_DWORD）を読み込む
                //intに型を設定して指定キーの中身を読み込む
                int Enabled_reve = (int)Enable_key.GetValue("Enabled");
                int AnnounceFlags_reve = (int)AnnounceFlags_key.GetValue("AnnounceFlags");

                if (Enabled_reve != 1)
                {
                    Enable_key.SetValue("Enabled", 1);
                }
                if (AnnounceFlags_reve != 5)
                {
                    AnnounceFlags_key.SetValue("AnnounceFlags", 5);
                }


                //コマンドプロンプトでサービスを実行
                //コマンドラインを指定（"/c"は実行後閉じるために必要）コマンドを書き込む
                p.StartInfo.Arguments = @"/c net start w32time";
                //起動
                p.Start();
                //出力を読み取る
                results = p.StandardOutput.ReadToEnd();
                //プロセス終了まで待機する
                //WaitForExitはReadToEndの後である必要がある
                //(親プロセス、子プロセスでブロック防止のため)
                p.WaitForExit();
                p.Close();
            }
            catch(Exception ex)
            {

                ErrorManager.ErrorHandler(ex);
                rs = (UInt32)ErrorCodeList.NTP_OPEN_ERROR;

            }

            return rs;
        }

        /// <summary>
        /// NTP蔵クライアント起動
        /// </summary>
        /// <returns></returns>
        public static UInt32 StartClient(string host)
        {

            UInt32 rs = 0;

            try
            {

                //Processオブジェクトを作成
                Process p = new Process();

                string results = "";

                //コマンドプロンプトでサービスを実行
                //ComSpec(cmd.exe)のパスを取得して、FileNameプロパティに指定
                p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
                //出力を読み取れるようにする
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = true;
                //ウィンドウを表示しないようにする
                p.StartInfo.CreateNoWindow = false;

                ////W32Timeを停止
                ////コマンドラインを指定（"/c"は実行後閉じるために必要）コマンドを書き込む
                //p.StartInfo.Arguments = @"/c net stop w32time";
                ////起動
                //p.Start();
                ////出力を読み取る
                //results = p.StandardOutput.ReadToEnd();
                ////プロセス終了まで待機する
                ////WaitForExitはReadToEndの後である必要がある
                ////(親プロセス、子プロセスでブロック防止のため)
                //p.WaitForExit();
                //p.Close();


                //コマンドプロンプトでサービスを実行
                //コマンドラインを指定（"/c"は実行後閉じるために必要）コマンドを書き込む
                p.StartInfo.Arguments = string.Format("w32tm /config /syncfromflags:manual /manualpeerlist:{0} /update", host);
                //起動
                p.Start();
                //出力を読み取る
                results = p.StandardOutput.ReadToEnd();
                //プロセス終了まで待機する
                //WaitForExitはReadToEndの後である必要がある
                //(親プロセス、子プロセスでブロック防止のため)
                p.WaitForExit();
                p.Close();

                //コマンドプロンプトでサービスを実行
                //コマンドラインを指定（"/c"は実行後閉じるために必要）コマンドを書き込む
                p.StartInfo.Arguments = string.Format("w32tm /resync");
                //起動
                p.Start();
                //出力を読み取る
                results = p.StandardOutput.ReadToEnd();
                //プロセス終了まで待機する
                //WaitForExitはReadToEndの後である必要がある
                //(親プロセス、子プロセスでブロック防止のため)
                p.WaitForExit();
                p.Close();


                ////コマンドプロンプトでサービスを実行
                ////コマンドラインを指定（"/c"は実行後閉じるために必要）コマンドを書き込む
                //p.StartInfo.Arguments = @"/c net start w32time";
                ////起動
                //p.Start();
                ////出力を読み取る
                //results = p.StandardOutput.ReadToEnd();
                ////プロセス終了まで待機する
                ////WaitForExitはReadToEndの後である必要がある
                ////(親プロセス、子プロセスでブロック防止のため)
                //p.WaitForExit();
                //p.Close();
            }
            catch (Exception ex)
            {

                ErrorManager.ErrorHandler(ex);
                rs = (UInt32)ErrorCodeList.NTP_OPEN_ERROR;

            }

            return rs;
        }


    }
}
