// ----------------------------------------------
// Copyright © 2017 DATALINK
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DL_CommonLibrary
{
    public class Timers
    {
        /// <summary>
        /// ストップウォッチ
        /// </summary>
        private Stopwatch m_sw = new Stopwatch();

        /// <summary>
        /// ラップタイム
        /// </summary>
        private List<long> m_LapTime = new List<long>();

        private long m_timeout = 0;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Timers()
        {

        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Timers(long timeout)
        {
            m_timeout = timeout;
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~Timers()
        {
            m_LapTime.Clear();
            m_sw.Stop();
            m_LapTime = null;
            m_sw = null;
        }


        /// <summary>
        /// 開始
        /// </summary>
        public void Start()
        {
            m_LapTime.Clear();
            m_sw.Reset();
            m_sw.Start();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            m_sw.Stop();
            
        }

        /// <summary>
        /// タイムアウト確認
        /// </summary>
        /// <returns></returns>
        public bool IsTimeout()
        {
            if (m_sw.ElapsedMilliseconds >= m_timeout) 
                return true;
            return false;
        }

        /// <summary>
        /// LapTime追加
        /// </summary>
        public void AddLapTime()
        {
            m_LapTime.Add(m_sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// LapTimeの取得
        /// </summary>
        public long[] GetLapTime()
        {
            return m_LapTime.ToArray();
        }
    }
}
