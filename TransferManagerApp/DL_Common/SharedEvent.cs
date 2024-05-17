using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace DL_CommonLibrary
{
    /// <summary>
    /// 共有イベント
    /// </summary>
    public class SharedEvent
    {
        #region "DLL Inports"
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        [DllImport("kernel32.dll")]
        static extern bool SetEvent(IntPtr hEvent);

        [DllImport("kernel32.dll")]
        static extern bool ResetEvent(IntPtr hEvent);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr hObject);

        const int ERROR_ALREADY_EXISTS = 183;
        #endregion

        #region "private valiable"
        /// <summary>
        /// Manual Reset Events
        /// </summary>
        private ManualResetEvent m_events = null;
        /// <summary>
        /// イベント名称
        /// </summary>
        private string mstrKey = "";
        #endregion


        /// <summary>
        /// コンストラクタ<BR/>
        /// 指定イベントを作成します<BR/>
        /// </summary>
        /// <param name="EventName">
        /// グローバルの場合は"Global\\"を先頭に指定します<BR/>
        /// </param>
        /// <param name="Initial">Initial setting</param>
        public SharedEvent(string EventName, bool Initial)
        {
            this.mstrKey = EventName.Replace("\\", "");
            this.CreateEvents(Initial);
        }
        /// <summary>
        /// デストラクタ
        /// </summary>
        ~SharedEvent()
        {
            this.CloseEvent();
        }



        #region "public property"
        /// <summary>
        /// イベント名称の設定/取得<BR/>
        /// グローバルの場合は"Global\\"を先頭に指定します<BR/>
        /// </summary>
        public string Name
        {
            get
            {
                return mstrKey;
            }

        }

        #endregion


        #region "public method"
        /// <summary>
        /// get Manual Event
        /// </summary>
        /// <returns></returns>
        public ManualResetEvent GetEvent()
        {
            try
            {
                return m_events;
            }
            catch (Exception ex)
            {
                ErrorManager(new System.Diagnostics.StackTrace(), ex.Message.ToString());
            }
            return null;
        }

        /// <summary>
        /// Close events.<BR/>
        /// </summary>
        /// <returns></returns>
        public bool CloseEvent()
        {
            try
            {

                if (m_events != null)
                {


                    m_events.Close();
                    m_events = null;
                    try
                    {
                        System.Threading.Thread.Sleep(1);
                    }
                    catch { }
                }

            }
            catch (Exception ex)
            {
                ErrorManager(new System.Diagnostics.StackTrace(), ex.Message.ToString());
                return false;
            }
            return true;
        }

        //class err
        //{
        //    int code = 0;
        //    int[] discription;
        //}

        /// <summary>
        /// Create all events<BR/>
        /// <seealso cref="GetEvent"/>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CreateEvents(bool Initial)
        {
            bool rs = false;
            try
            {




                // case of the owner. close all events. 
                //if (owner)
                // {
                //    CloseEvent();
                //}

                IntPtr handle;
                bool CreateNew;
                // get event
                CreateManagedEvent(mstrKey, out handle, out CreateNew);
                // initial state is reset. 
                m_events = new ManualResetEvent(false);
                //m_events.Handle = handle;

                m_events.SafeWaitHandle = new Microsoft.Win32.SafeHandles.SafeWaitHandle(handle, true);
                if (CreateNew && Initial)
                {
                    m_events.Set();
                }
                rs = true;
            }
            catch (Exception ex)
            {
                ErrorManager(new System.Diagnostics.StackTrace(), ex.Message.ToString());
            }
            return rs;
        }
        #endregion

        #region "Private method"
        /// <summary>
        /// Creates a named event in the win32 space. This is a manual<BR/>
        /// reset event that is initially not signalled.<BR/>
        /// </summary>
        /// <param name="name">イベント名称</param>
        /// <param name="handle"></param>
        /// <param name="createdNew">true if it created a new one, false if <BR/>
        /// it opened an existing one</param>
        /// <returns>true if it succeeds, otherwise false</returns>
        private bool CreateManagedEvent(string name, out IntPtr handle, out bool createdNew)
        {

            createdNew = false;
            handle = CreateEvent(IntPtr.Zero, true, false, name);
            int error = Marshal.GetLastWin32Error();
            if (handle == IntPtr.Zero)
                return false;
            createdNew = (error != ERROR_ALREADY_EXISTS);

            return true;
        }

        /// <summary>
        /// Error Manager
        /// </summary>
        /// <param name="st">Stack frame</param>
        /// <param name="msg">error message</param>
        private void ErrorManager(System.Diagnostics.StackTrace st, string msg)
        {
            try
            {
                System.Diagnostics.Trace.Fail(st.GetFrame(0).ToString() + System.Environment.NewLine + msg);
            }
            catch
            {

            }
        }
        #endregion

    }
}
