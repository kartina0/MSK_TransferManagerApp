using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace DL_CommonLibrary
{
    public static class WindowsControl
    {
        public enum ExitWindows : uint
        {
            EWX_LOGOFF = 0x00,
            EWX_SHUTDOWN = 0x01,
            EWX_REBOOT = 0x02,
            EWX_POWEROFF = 0x08,
            EWX_RESTARTAPPS = 0x40,
            EWX_FORCE = 0x04,
            EWX_FORCEIFHUNG = 0x10,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TOKEN_PRIVILEGES
        {
            public int PrivilegeCount;
            public long Luid;
            public int Attributes;
        }

        [FlagsAttribute]
        public enum ExecutionState : uint
        {
	        // 関数が失敗した時の戻り値
	        Null = 0,
	        // スタンバイを抑止
	        SystemRequired = 1,
	        // 画面OFFを抑止
	        DisplayRequired = 2,
	        // 効果を永続させる。ほかオプションと併用する。
	        Continuous = 0x80000000,
        }

        [DllImport("user32.dll")]
        public extern static bool ShutdownBlockReasonCreate(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string pwszReason);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SetProcessShutdownParameters(int dwLevel, int dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessShutdownParameters(ref int lpdwLevel, ref int lpdwFlags);

        public const int WM_QUERYENDSESSION = 0x0011;
        public const int WM_ENDSESSION      = 0x0016;
        public const int WM_POWERBROADCAST  = 0x0218;



        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ExitWindowsEx(ExitWindows uFlags, int dwReason);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle,
            uint DesiredAccess,
            out IntPtr TokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool LookupPrivilegeValue(string lpSystemName,
            string lpName,
            out long lpLuid);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(IntPtr TokenHandle,
            bool DisableAllPrivileges,
            ref TOKEN_PRIVILEGES NewState,
            int BufferLength,
            IntPtr PreviousState,
            IntPtr ReturnLength);

        [DllImport("kernel32.dll")]
        extern static ExecutionState SetThreadExecutionState(ExecutionState esFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        /// <summary>
        /// シャットダウンするためのセキュリティ特権を有効にする
        /// </summary>
        private static void AdjustToken()
        {
            const uint TOKEN_ADJUST_PRIVILEGES = 0x20;
            const uint TOKEN_QUERY = 0x8;
            const int SE_PRIVILEGE_ENABLED = 0x2;
            const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;

            IntPtr procHandle = GetCurrentProcess();

            //トークンを取得する
            IntPtr tokenHandle;
            OpenProcessToken(procHandle,
                TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out tokenHandle);
            //LUIDを取得する
            TOKEN_PRIVILEGES tp = new TOKEN_PRIVILEGES();
            tp.Attributes = SE_PRIVILEGE_ENABLED;
            tp.PrivilegeCount = 1;
            LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, out tp.Luid);
            //特権を有効にする
            AdjustTokenPrivileges(
                tokenHandle, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);

            //閉じる
            CloseHandle(tokenHandle);
        }

        /// <summary>
        /// ウィンドウズ シャットダウン
        /// </summary>
        public static void WindowsShutDown()
        {
            try
            {
                AdjustToken();
                ExitWindowsEx(ExitWindows.EWX_POWEROFF, 0);
            }
            catch { }
        }

        /// <summary>
        /// Windows Sleep有効/無効
        /// </summary>
        /// <param name="enable"></param>
        public static void WindowsSleep(bool enable)
        {
            if(enable)
            {
                SetThreadExecutionState(ExecutionState.Continuous);
            }
            else
            {
                SetThreadExecutionState(ExecutionState.SystemRequired | ExecutionState.Continuous);
            }
        }

        /// <summary>
        /// モニタの電源をOFFにする
        /// </summary>
        public static void MonitorPowerOff()
        {
            SendMessage(-1, 0x112, 0xf170, 2);
        }

        /// <summary>
        /// モニタの電源をONにする
        /// </summary>
        public static void MonitorPowerOn()
        {
            SendMessage(-1, 0x112, 0xf170, -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowHandle"></param>
        public static void DisablePowerSwitch(IntPtr windowHandle, string shutDownConfirMessage)
        {
            SetProcessShutdownParameters(0x4FF, 0); // システム内の他のプロセスから見た相対的なシャットダウンの優先順序を上げる
            
            // シャットダウン画面でのメッセージ
            ShutdownBlockReasonCreate(windowHandle, shutDownConfirMessage);
        }

        

    }
}
