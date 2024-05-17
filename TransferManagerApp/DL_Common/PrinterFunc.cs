using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DL_CommonLibrary
{
    /// <summary>
    /// プリンタ操作クラス
    /// </summary>
    public static class PrinterFunc
    {
        /// <summary>
        /// ADOBE Acrobat Readerを使用して
        /// PDFファイルを直接印刷する
        /// 
        ///     /n-既に開いている場合でもReaderの新しいインスタンスを起動します
        /// /s-スプラッシュ画面を表示しない
        /// /o-ファイルを開くダイアログを表示しない
        /// /h-最小化されたウィンドウとして開く
        /// /p<filename>-印刷ダイアログを開いてすぐに進みます
        /// /t<filename> <printername> <drivername> <portname>-指定したプリンターでファイルを印刷します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="printerName"></param>
        public static bool SendPdfFileToPrinter(string filePath, string printerName, bool waitComp=false)
        {
            bool ret = true;
            try
            {
                Stopwatch sw = new Stopwatch();
                string arg = string.Format("/h /t /s {0} \"{1}\"", filePath, printerName);
                // 印刷実行前の印刷キューにたまっているドキュメント数を取得
                int prevCount = PrinterFunc.PrintQueueCount(printerName);
                ProcessStartInfo ps = new ProcessStartInfo("AcroRd32.exe");
                ps.Arguments = arg;
                ps.CreateNoWindow = true;
                ps.WindowStyle = ProcessWindowStyle.Minimized;
                //using (Process exeProcess = System.Diagnostics.Process.Start("AcroRd32.exe", arg))
                using (Process exeProcess = System.Diagnostics.Process.Start(ps))
                {
                    int afterCount = 0;
                    exeProcess.WaitForExit(100);
                    sw.Start();
                    while (true)
                    {
                        // 印刷キューに転送されるまで待つ
                        afterCount = PrinterFunc.PrintQueueCount(printerName);
                        if (afterCount - prevCount > 0) break;
                        // 20秒過ぎたらキューを見逃したかもしれないのでタイムアウト
                        if (sw.ElapsedMilliseconds > 20000)
                        {
                            ret = false;
                            break;
                        }
                        System.Threading.Thread.Sleep(50);
                    }

                    if (!exeProcess.HasExited)
                    {
                        // 印刷キューから無くなるまで待つ
                        sw.Restart();
                        while (waitComp)
                        {
                            afterCount = PrinterFunc.PrintQueueCount(printerName);
                            if (afterCount == 0) break;
                            // 20秒でタイムアウト
                            if (sw.ElapsedMilliseconds > 20000)
                            {
                                ret = false;
                                break;
                            }
                            System.Threading.Thread.Sleep(50);
                        }

                        // 直ぐにプロセスを消すと印刷されないので少し待つ
                        System.Threading.Thread.Sleep(800);
                        exeProcess.Kill();      // Killしないと何故かPrintQueueCountでカウントできない
                    }
                }
            }
            catch { }
            return ret;
        }

        /// <summary>
        /// インストールされているプリンタ名を取得
        /// </summary>
        /// <returns></returns>
        public static string[] GetPrinterNames()
        {
            List<string> list = new List<string>();
            foreach (string p in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                list.Add(p);
            }
            return list.ToArray();
        }


        /// <summary>
        /// 指定されたプリンタキューにたまっているドキュメント数を取得
        /// </summary>
        /// <returns></returns>
        public static int PrintQueueCount(string printerName)
        {
            int cnt = 0;
            try
            {
                System.Printing.LocalPrintServer pt = new System.Printing.LocalPrintServer();
                System.Printing.PrintQueue queue = new System.Printing.PrintQueue(pt, printerName);
                queue.Refresh();
                cnt = queue.GetPrintJobInfoCollection().Count();
            }
            catch { }
            return cnt;
        }

        /// <summary>
        /// 指定されたプリンタキューの印刷ジョブがなくなるまで待つ
        /// </summary>
        /// <returns></returns>
        public static bool WaitPrinterQueueEmpty(string printerName, int timeOut)
        {
            try
            {
                System.Printing.LocalPrintServer pt = new System.Printing.LocalPrintServer();
                System.Printing.PrintQueue queue = new System.Printing.PrintQueue(pt, printerName);

                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                while (true)
                {
                    queue.Refresh();
                    int cnt = queue.GetPrintJobInfoCollection().Count();
                    if (cnt <= 0) break;
                    if (sw.ElapsedMilliseconds > timeOut) return false;
                    // @@20190507-1
                    //System.Threading.Thread.Sleep(100);
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// 指定したプリンタがインストールされているのか確認
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns></returns>
        public static bool IsExist(string printerName)
        {
            string[] list = GetPrinterNames();
            int index = Array.IndexOf(list, printerName);
            return index >= 0;
        }

        /// <summary>
        /// プリンタキューのJOBを全て削除する
        /// </summary>
        public static void ClearPrinterQueue(string printerName)
        {
            try
            {
                System.Printing.LocalPrintServer pt = new System.Printing.LocalPrintServer();
                System.Printing.PrintQueue queue = new System.Printing.PrintQueue(pt, printerName);
                foreach(var job in queue.GetPrintJobInfoCollection())
                {
                    job.Cancel();
                }
            }
            catch { }
        }

    }
}
