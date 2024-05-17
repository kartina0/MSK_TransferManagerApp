// ----------------------------------------------
// Copyright © 2017 DATALINK
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DL_CommonLibrary
{       
    [StructLayout(LayoutKind.Sequential)]
    public struct CURRENT_LOCALTIME
    {
        public int usec;    /* micro second [0-999]*/
        public int msec;    /* millisecond - [0-999] */
        public int sec;     /* seconds after the minute - [0,59] */
        public int min;     /* minutes after the hour - [0,59] */
        public int hour;    /* hours since midnight - [0,23] */
        public int day;
        public int mon;     /* months since January - [0,11] */
        public int year;    /* years  */
    }


    public class TraceTime
    {
        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceCounter(ref long x);

        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceFrequency(ref long x);

        [DllImport("winmm.dll")]
        static extern Int32 timeBeginPeriod(Int32 period);

        [DllImport("winmm.dll")]
        static extern Int32 timeEndPeriod(Int32 period);

        [DllImport("winmm.dll")]
        static extern Int32 timeGetTime();

        [DllImport("winmm.dll", SetLastError = true)]
        static extern UInt32 timeGetSystemTime(ref MMTIME mmTime, UInt32 sizeMmTime);

        [StructLayout(LayoutKind.Explicit)]
        struct MMTIME
        {
            [FieldOffset(0)]
            public uint wType;
            [FieldOffset(4)]
            public uint ms;
            [FieldOffset(4)]
            public uint sample;
            [FieldOffset(4)]
            public uint cb;
            [FieldOffset(4)]
            public uint ticks;
            [FieldOffset(4)]
            public ushort hour;
            [FieldOffset(6)]
            public ushort min;
            [FieldOffset(8)]
            public ushort sec;
            [FieldOffset(10)]
            public ushort frame;
            [FieldOffset(12)]
            public ushort fps;
            [FieldOffset(14)]
            public ushort dummy;
            [FieldOffset(16)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public ushort[] pad;
            [FieldOffset(4)]
            public uint songptrpos;
        }

        private static long m_freq          = 0;
        private static double m_offset      = 0;
        private static bool m_calibrated    = false;


        public static void CopyTimeStruct(CURRENT_LOCALTIME src, ref CURRENT_LOCALTIME dest)
        {
            try
            {
                dest.year = src.year;
                dest.mon = src.mon;
                dest.day = src.day;
                dest.hour = src.hour;
                dest.min = src.min;
                dest.sec = src.sec;
                dest.msec = src.msec;
                dest.usec = src.usec;

            }
            catch { }
        }

        public static void ClearTimeStruct(ref CURRENT_LOCALTIME t)
        {
            try
            {
                t.year  = 0;
                t.mon   = 0;
                t.day   = 0;
                t.hour  = 0;
                t.min   = 0;
                t.sec   = 0;
                t.msec  = 0;
                t.usec  = 0;

            }
            catch { }
        }

        /// <summary>
        /// t2とt1の時間差(msec)を取得する
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static UInt32 DiffTime(CURRENT_LOCALTIME t1, CURRENT_LOCALTIME t2)
        {
            UInt32 t1_msec = 0;
            UInt32 t2_msec = 0;

            UInt32 msec = 0;
            try
            {

                t1_msec = (UInt32)((t1.hour * 3600000) + (t1.min * 60000) + (t1.sec * 1000) + t1.msec);
                t2_msec = (UInt32)((t2.hour * 3600000) + (t2.min * 60000) + (t2.sec * 1000) + t2.msec);
                msec = t2_msec - t1_msec;

            }
            catch { msec = 0; }
            return msec;
        }


        /// <summary>
        /// t2とt1の時間差(msec)を取得する
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static Double DiffTime2(CURRENT_LOCALTIME t1, CURRENT_LOCALTIME t2)
        {
            double msec = 0;
            try
            {
                msec = (double)DiffTimeUsec(t1, t2) / 1000;
            }
            catch { msec = 0; }
            return msec;
        }
        /// <summary>
        /// t2とt1の時間差(usec)を取得する
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static UInt32 DiffTimeUsec(CURRENT_LOCALTIME t1, CURRENT_LOCALTIME t2)
        {
            UInt32 t1_msec = 0;
            UInt32 t2_msec = 0;

            UInt32 msec = 0;
            try
            {

                t1_msec = (UInt32)((t1.hour * 3600000000) + (t1.min * 60000000) + (t1.sec * 1000000) + (t1.msec*1000) + t1.usec);
                t2_msec = (UInt32)((t2.hour * 3600000000) + (t2.min * 60000000) + (t2.sec * 1000000) + (t2.msec*1000) + t2.usec);

                msec = t2_msec - t1_msec;

            }
            catch { msec = 0; }
            return msec;
        }
        /// <summary>
        /// ミリ秒以下を補正
        /// </summary>
        public static void Calibrate()
        {
            DateTime old = DateTime.Now;
            double temp = Now;

            while (true)
            {
                DateTime dt = DateTime.Now;
                if (old.Second != dt.Second)
                {

                    m_offset = Now;
                    break;
                }
                else
                {
                    old = dt;
                }
            }

            m_calibrated = true;

        }

        public static double Now
        {
            get
            {
                long cnt = 0;
                if (m_freq == 0)
                {
                    QueryPerformanceFrequency(ref m_freq);
                }

                QueryPerformanceCounter(ref cnt);
                return ((double)cnt / (double)m_freq) % 1 * 1000;
            }
        }


        /// <summary>
        /// 現在時刻を取得
        /// </summary>
        /// <returns></returns>
        public static CURRENT_LOCALTIME GetTime()
        {
            CURRENT_LOCALTIME t = new CURRENT_LOCALTIME();
            MMTIME mt = new MMTIME();
           
            try
            {

                mt.wType = 0x1;
                //timeBeginPeriod(1);
                //timeGetSystemTime(ref mt, 20);
                DateTime dt = DateTime.Now;
                double msec = Now;
                // timeEndPeriod(1);
                t.year = dt.Year;
                t.mon = dt.Month;
                t.day = dt.Day;
                t.hour = dt.Hour;
                t.min = dt.Minute;
                t.sec = dt.Second;
                int temp = dt.Millisecond;

                if (msec > m_offset)
                {
                    t.msec = (int)(msec - m_offset);
                    t.usec = (int)((msec - m_offset) % 1 * 1000);
                }
                else
                {
                    t.msec = (int)(1000 - (m_offset - msec)) % 1000;
                    t.usec = (int)(1000 - ((m_offset - msec) % 1 * 1000)) % 1000;
                }
            }
            catch { }
            return t;
        }
    }
}
