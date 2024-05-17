// ----------------------------------------------
// Copyright © 2017 DATALINK
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ErrorCodeDefine;

namespace DL_CommonLibrary
{
    /// <summary>
    /// 外部プログラムのコントロールを操作する
    /// ControlIDはSpy++等のツールを使用して調べる
    /// </summary>
    public class ExtAppControl
    {
        /// <summary>
        /// パス
        /// </summary>
        private string m_Path = "";

        /// <summary>
        /// 対象ウィンドウテキスト
        /// </summary>
        private string m_appName = "";

        /// <summary>
        /// アプリケーション ウィンドウハンドル
        /// </summary>
        private IntPtr m_hWnd = IntPtr.Zero;

        private System.Diagnostics.Process m_Process = new System.Diagnostics.Process();

        /// <summary>
        /// コントロール情報
        /// </summary>
        private SortedList<Int32, CONTROL_INFO> m_List = new SortedList<int, CONTROL_INFO>();

        /// <summary>
        /// コントロール情報
        /// </summary>
        private struct CONTROL_INFO
        {
            public IntPtr hWnd;
            public string className;
            public string text;
        }


        #region "DLL Import"

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hWnd, Int32 hwndChildAfter, string lpszClass, string lpszWindow);

        /// <summary>
        /// コールバック
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        delegate int WNDENUMPROC(IntPtr hwnd, Int32 lParam);

        /// <summary>
        /// 子ウィンドウハンドルの列挙
        /// </summary>
        /// <param name="hWndParent"></param>
        /// <param name="lpEnumFunc"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int EnumChildWindows(IntPtr hWndParent, WNDENUMPROC lpEnumFunc, Int32 lParam);

        /// <summary>
        /// ハンドルからコントロールID取得
        /// </summary>
        /// <param name="hwndCtl"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "GetDlgCtrlID")]
        private static extern Int32 GetDlgCtrlID(IntPtr hwndCtl);

        [DllImport("user32.dll", EntryPoint = "GetWindowText")]
        private static extern Int32 GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "GetClassName")]
        private static extern Int32 GetClassName(IntPtr hWnd, StringBuilder buf, int nMaxCount);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern Int32 SendMessageString(IntPtr hWnd, Int32 Msg, Int32 wParam, string lParam);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern Int32 SendMessageStringBuilder(IntPtr hWnd, Int32 Msg, Int32 wParam, StringBuilder lParam);


        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern Int32 SendMessage(IntPtr hWnd, Int32 Msg, Int32 wParam, Int32 lParam);

        [DllImport("User32.dll", EntryPoint = "EnableWindow")]
        private static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        private static extern Int32 PostMessage(IntPtr hWnd, Int32 Msg, Int32 wParam, Int32 lParam);

        private const Int32 WM_CLOSE = 0x0010;


        #region "テキストコントロールメッセージ"
        private const Int32 WM_COPYDATA = 0x004A;
        private const Int32 WM_USER = 0x0400;
        private const Int32 WM_SETTEXT = 0x000C;
        private const Int32 WM_GETTEXT = 0x000D;

        #endregion
        #region "ボタンコントロールメッセージ"

        private const Int32 BM_GETCHECK = 0x00F0;
        private const Int32 BM_SETCHECK = 0x00F1;
        private const Int32 BM_GETSTATE = 0x00F2;
        private const Int32 BM_SETSTATE = 0x00F3;

        private const Int32 BM_SETSTYLE = 0x00F4;
        private const Int32 BM_CLICK = 0x00F5;
        private const Int32 BM_GETIMAGE = 0x00F6;
        private const Int32 BM_SETIMAGE = 0x00F7;

        private const Int32 BST_UNCHECKED = 0x0000;
        private const Int32 BST_CHECKED = 0x0001;
        private const Int32 BST_INDETERMINATE = 0x0002;
        private const Int32 BST_PUSHED = 0x0004;
        private const Int32 BST_FOCUS = 0x0008;

        #endregion

        #region "コンボボックス"
        /// <summary>Index選択 </summary>
        private const Int32 CB_SETCURSE = 0x014E;
        /// <summary>Index取得 </summary>
        private const Int32 CB_GETCURSE = 0x0147;

        #endregion


        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="appName"></param>
        public ExtAppControl(string appName)
        {
            m_appName = appName;

            m_List.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool SearchControl(Int32 controlID)
        {
            bool rs = false;
            try
            {
                // アプリケーションハンドル取得
                IntPtr hWindow = FindWindow(null, m_appName);

                // 子ウィンドウハンドル列挙
                if (hWindow != IntPtr.Zero)
                {
                    this.m_hWnd = hWindow;
                    RegisterControlInfo(hWindow, controlID);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
                rs = false;
            }
            return rs;
        }

        /// <summary>
        /// コントロールID登録処理
        /// </summary>
        /// <param name="hWndParent"></param>
        /// <param name="targetID"></param>
        /// <returns></returns>
        private int RegisterControlInfo(IntPtr hWndParent, Int32 targetID)
        {
            IntPtr hWnd = IntPtr.Zero;
            try
            {
                // 子ウィンドウハンドル列挙
                EnumChildWindows(hWndParent, RegisterControlInfo, targetID);

                // ハンドルから コントロールID取得
                int ctrlID = GetDlgCtrlID(hWndParent);

                // コントロールID比較
                if (ctrlID == targetID)
                    hWnd = hWndParent;

                // コントロールIDが一致した場合情報取得
                if (hWnd != IntPtr.Zero)
                {

                    CONTROL_INFO info = new CONTROL_INFO();
                    StringBuilder wndText = new StringBuilder(100);
                    StringBuilder className = new StringBuilder(100);

                    info.hWnd = hWnd;
                    int nLength = GetWindowText(hWndParent, wndText, wndText.Capacity + 1);
                    if (nLength > 0)
                        info.text = wndText.ToString();

                    // クラス名の取得
                    nLength = GetClassName(hWndParent, className, className.Capacity + 1);
                    if (nLength > 0)
                        info.className = className.ToString();

                    // リストに追加
                    m_List.Add(targetID, info);

                }
            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
                return 0;
            }
            return 1;
        }

        /// <summary>
        /// 指定したコントロールへテキスト入力
        /// </summary>
        public UInt32 SetText(Int32 controlID, string txt)
        {
            UInt32 rs = 0;
            try
            {
                // コントロールIDからリストIndexを取得
                int index = m_List.IndexOfKey(controlID);
                if (index < 0) rs = (UInt32)ErrorCodeList.CONTROL_NOT_FOUND;


                if (STATUS_SUCCESS(rs))
                {
                    CONTROL_INFO info = m_List[controlID];
                    rs = (UInt32)SendMessageString(info.hWnd, WM_SETTEXT, 0, txt);
                    rs = 0;
                }

            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
                rs = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rs;
        }

        /// <summary>
        /// 指定したコントロールへテキスト入力
        /// </summary>
        public UInt32 GetText(Int32 controlID, ref string txt)
        {
            UInt32 rs = 0;
            try
            {
                // コントロールIDからリストIndexを取得
                int index = m_List.IndexOfKey(controlID);
                if (index < 0) rs = (UInt32)ErrorCodeList.CONTROL_NOT_FOUND;


                if (STATUS_SUCCESS(rs))
                {
                    StringBuilder wndText = new StringBuilder(100);
                    CONTROL_INFO info = m_List[controlID];
                    string t = "";
                    int len = SendMessageStringBuilder(info.hWnd, WM_GETTEXT, wndText.Capacity + 1, wndText);
                    if (len > 0)
                        txt = wndText.ToString();

                }

            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
                rs = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rs;
        }

        /// <summary>
        /// ボタンクリック
        /// ※クリック処理完了後に処理が戻る
        /// </summary>
        /// <param name="controlID"></param>
        /// <returns></returns>
        public UInt32 ButtonClick(Int32 controlID)
        {
            UInt32 rs = 0;
            try
            {
                // コントロールIDからリストIndexを取得
                int index = m_List.IndexOfKey(controlID);
                if (index < 0) rs = (UInt32)ErrorCodeList.CONTROL_NOT_FOUND;


                if (STATUS_SUCCESS(rs))
                {
                    CONTROL_INFO info = m_List[controlID];

                    // おまじない
                    // これやらないとイベントが発生しなくなる場合がある
                    EnableWindow(m_hWnd, false);

                    // SensMessageは処理完了後にタスクが戻る
                    rs = (UInt32)SendMessage(info.hWnd, BM_CLICK, 0, 0);

                    EnableWindow(m_hWnd, true);
                }

            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
                rs = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rs;
        }
        

        /// <summary>
        /// Radio/Check Box 操作
        /// </summary>
        /// <param name="controlID"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public UInt32 ControlCheck(Int32 controlID, bool val)
        {
            UInt32 rs = 0;
            try
            {
                // コントロールIDからリストIndexを取得
                int index = m_List.IndexOfKey(controlID);
                if (index < 0) rs = (UInt32)ErrorCodeList.CONTROL_NOT_FOUND;

                if (STATUS_SUCCESS(rs))
                {
                    CONTROL_INFO info = m_List[controlID];
                    // SensMessageは処理完了後にタスクが戻る
                    if (val)
                        rs = (UInt32)SendMessage(info.hWnd, BM_SETCHECK, BST_CHECKED, 0);
                    else
                        rs = (UInt32)SendMessage(info.hWnd, BM_SETCHECK, BST_UNCHECKED, 0);
                }

            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
                rs = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rs;
        }

        /// <summary>
        /// コンボボックス 操作
        /// </summary>
        /// <param name="controlID"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public UInt32 SetComboIndex(Int32 controlID, Int32 selIndex)
        {
            UInt32 rs = 0;
            try
            {
                // コントロールIDからリストIndexを取得
                int index = m_List.IndexOfKey(controlID);
                if (index < 0) rs = (UInt32)ErrorCodeList.CONTROL_NOT_FOUND;

                if (STATUS_SUCCESS(rs))
                {
                    CONTROL_INFO info = m_List[controlID];
                    // SensMessageは処理完了後にタスクが戻る
                    rs = (UInt32)SendMessage(info.hWnd, CB_SETCURSE, selIndex, 0);

                }
            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
                rs = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rs;
        }

        /// <summary>
        /// コンボボックス 取得
        /// </summary>
        /// <param name="controlID"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public UInt32 GetComboIndex(Int32 controlID, ref Int32 selIndex)
        {
            UInt32 rs = 0;
            try
            {
                // コントロールIDからリストIndexを取得
                int index = m_List.IndexOfKey(controlID);
                if (index < 0) rs = (UInt32)ErrorCodeList.CONTROL_NOT_FOUND;

                if (STATUS_SUCCESS(rs))
                {
                    CONTROL_INFO info = m_List[controlID];
                    // SensMessageは処理完了後にタスクが戻る
                    selIndex = (Int32)SendMessage(info.hWnd, CB_GETCURSE, 0, 0);

                }
            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
                rs = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public UInt32 Open(string exeFile, string caption)
        {
            UInt32 rs = 0;
            try
            {
                m_Path = System.IO.Path.GetFullPath(exeFile);

                // アプリケーションハンドル取得
                m_hWnd = FindWindow(null, caption);

                if (m_hWnd == IntPtr.Zero)
                {
                    m_Process.StartInfo.FileName = m_Path;
                    m_Process.Start();
                    m_Process.WaitForInputIdle();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
                rs = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rs;
        }

        /// <summary>
        /// プログラム終了
        /// </summary>
        /// <returns></returns>
        public UInt32 Close()
        {
            UInt32 rs = 0;
            try
            {
                rs = (UInt32)SendMessage(m_hWnd, WM_CLOSE, 0, 0);
                m_List.Clear();
            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
                rs = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rs;
        }

        /// <summary>
        /// 戻り値確認
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        private bool STATUS_SUCCESS(UInt32 rs)
        {
            return rs == 0;
        }
    }
}
