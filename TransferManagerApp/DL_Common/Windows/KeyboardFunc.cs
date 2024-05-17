using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DL_CommonLibrary
{
    public static class KeyboardFunc
    {
        const int WM_SYSCOMMAND = 0x12;
        const int SC_CLOSE      = 0xF060;
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);


        /// <summary>
        /// ソフトウェアキーボード表示
        /// </summary>
        /// <param name="show"></param>
        public static void ShowSoftKeybpord(bool show)
        {
            try
            {
                string procName = "osk";
                System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(procName);
                if (processes.Length == 0)
                {
                    System.Diagnostics.Process.Start(procName + ".exe");
                }

            }
            catch { }
        }
        /// <summary>
        /// ソフトウェアキーボード表示
        /// </summary>
        /// <param name="show"></param>
        public static void ShowSoftKeybpordWin10(bool show)
        {
            try
            {
                string procName = @"c:\program files\common files\microsoft shared\ink\TabTip.exe";
                System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("tabtip");
                if (show)
                {
                    //if (processes.Length == 0)
                    //{
                        System.Diagnostics.Process.Start(procName);
                    //}
                }
                else
                {
                    IntPtr hWnd = FindWindow("IPTip_Main_Window", "");
                    if (processes.Length > 0)
                        SendMessage(hWnd, WM_SYSCOMMAND, SC_CLOSE, 0);
                }
            }
            catch(Exception ex) { }
        }



    }
}
