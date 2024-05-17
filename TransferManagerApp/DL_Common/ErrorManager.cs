//----------------------------------------------------------
// Copyright © 2017 DATALINK
//----------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using ErrorCodeDefine;

namespace DL_CommonLibrary
{
    /// <summary>
    /// エラー処理
    /// </summary>
    public class ErrorManager
    {

        /// <summary>
        /// 例外処理
        /// </summary>
        /// <param name="ex"></param>
        static public void ErrorHandler(Exception ex)
        {
            ErrorHandler(null, ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="ex"></param>
        static public void ErrorHandler(IWin32Window owner, Exception ex)
        {
            try
            {
                StackFrame callStack = new StackFrame(2, true);
                string msg = "";
                string sourceFile = callStack.GetFileName();
                int sourceLine = callStack.GetFileLineNumber();
                if (sourceFile != null)
                {
                    msg = string.Format("{0}\rFile : {1} ,Line : {2}",
                                            ex.Message,
                                            sourceFile,
                                            sourceLine
                                            );
                }
                else
                {
                    msg = ex.Message;
                }
                if (msg != "")
                    MessageBox.Show(owner, msg);
            }
            catch { }
        }

        /// <summary>
        /// エラー処理
        /// </summary>
        /// <param name="err"></param>
        static public void ErrorHandler(ErrorCodeList err)
        {

            if (err != ErrorCodeList.STATUS_SUCCESS)
            {
                // エラーメッセージを取得
                string msg = GetErrorMessage(err);
                if (msg == "")
                {
                    MessageBox.Show(string.Format("ErrorCode = 0x{0:X08} , {1}\r{2}", (uint)err, err.ToString(), msg));
                }
                else
                {
                    MessageBox.Show(string.Format("{0}", msg));
                }
            }
        }

        /// <summary>
        /// エラー処理
        /// </summary>
        /// <param name="err"></param>
        static public void ErrorHandler(UInt32 err)
        {

            if (err != 0)
            {
                // エラーメッセージを取得
                string msg = GetErrorMessage(err);
                if (msg == "")
                {
                    MessageBox.Show(string.Format("ErrorCode = 0x{0:X08} , {1}\r{2}", (uint)err, err.ToString(), msg));
                }
                else
                {
                    MessageBox.Show(string.Format("{0}", msg));
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="err"></param>
        static public void ErrorHandler(IWin32Window owner, ErrorCodeList err)
        {

            if (err != ErrorCodeList.STATUS_SUCCESS)
            {
                // エラーメッセージを取得
                string msg = GetErrorMessage(err);
                if (msg == "")
                {
                    MessageBox.Show(owner, string.Format("ErrorCode = 0x{0:X08} , {1}\r{2}", (uint)err, err.ToString(), msg));
                }
                else
                {
                    MessageBox.Show(owner, string.Format("{0}", msg));
                }
            }
        }
        /// <summary>
        /// メッセージボックス表示
        /// </summary>
        /// <param name="msg">表示メッセージ</param>
        /// <param name="icon">情報アイコン</param>
        static public void ShowMessage(IWin32Window owner, string msg, MessageBoxIcon icon)
        {
            MessageBox.Show(owner, msg, Application.ProductName, MessageBoxButtons.OK, icon);
        }

        /// <summary>
        /// エラーメッセージ取得
        /// </summary>
        /// <param name="err">エラーコード</param>
        /// <returns>エラーメッセージ</returns>
        static public string GetErrorMessage(ErrorCodeList err)
        {
            string msg = "";
            try
            {
                //INIファイルから設定を読み込みます
                string fname = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\ErrorMsg.ini";

                // ファイルが無い場合はINIフォルダを探す
                if(!FileIo.ExistFile(fname))
                    fname = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\INI" + "\\ErrorMsg.ini";

                string section = "MESSAGE";

                string key = string.Format("{0}", err);
                FileIo.ReadIniFile<string>(fname, section, key, ref msg);

                // DEC
                if (msg == "")
                {   // 登録されていない場合は番号で検索
                    key = string.Format("{0}", (UInt32)err);
                    FileIo.ReadIniFile<string>(fname, section, key, ref msg);
                }

                // HEX
                if (msg == "")
                {   // 登録されていない場合は番号で検索
                    key = string.Format("0x{0:X08}", (UInt32)err);
                    FileIo.ReadIniFile<string>(fname, section, key, ref msg);
                }
                // DEC
                if (msg == "")
                {   // 登録されていない場合は番号で検索
                    key = string.Format("{0:00000}", (UInt32)err);
                    FileIo.ReadIniFile<string>(fname, section, key, ref msg);
                }

                // @@20171104-3
                if (msg == "")
                {
                    msg = err.ToString();
                }

                // 特殊キーを変換
                if (msg != "")
                {
                    msg = msg.Replace("<\\r>", "\r");
                    msg = msg.Replace("<\\t>", "\t");
                }



            }
            catch (Exception ex) { ErrorHandler(ex); }
            return msg;
        }

        /// <summary>
        /// エラーメッセージ取得
        /// </summary>
        /// <param name="err">エラーコード</param>
        /// <returns>エラーメッセージ</returns>
        static public string GetErrorMessage(UInt32 err)
        {
            string msg = "";
            try
            {
                //INIファイルから設定を読み込みます
                string fname = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\ErrorMsg.ini";
                // ファイルが無い場合はINIフォルダを探す
                if (!FileIo.ExistFile(fname))
                    fname = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\INI" + "\\ErrorMsg.ini";

                string section = "MESSAGE";

                string key = "";
 
                // DEC
                if (msg == "")
                {   // 登録されていない場合は番号で検索
                    key = string.Format("{0}", (UInt32)err);
                    FileIo.ReadIniFile<string>(fname, section, key, ref msg);
                }

                // HEX
                if (msg == "")
                {   // 登録されていない場合は番号で検索
                    key = string.Format("0x{0:X08}", (UInt32)err);
                    FileIo.ReadIniFile<string>(fname, section, key, ref msg);
                }
                // DEC
                if (msg == "")
                {   // 登録されていない場合は番号で検索
                    key = string.Format("{0:00000}", (UInt32)err);
                    FileIo.ReadIniFile<string>(fname, section, key, ref msg);
                }

                // @@20171104-3
                if (msg == "")
                {
                    msg = err.ToString();
                }

                // 特殊キーを変換
                if (msg != "")
                {
                    msg = msg.Replace("<\\r>", "\r");
                    msg = msg.Replace("<\\t>", "\t");
                }



            }
            catch (Exception ex) { ErrorHandler(ex); }
            return msg;
        }
    }
}
