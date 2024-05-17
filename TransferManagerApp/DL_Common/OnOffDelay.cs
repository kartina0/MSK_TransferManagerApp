using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace DL_CommonLibrary
{
    public class OnOffDelay
    {
        /// <summary>
        /// ON/OFF状態
        /// </summary>
        private bool _on = false;
        /// <summary>
        /// ONディレイ計測タイマ
        /// </summary>
        private Stopwatch _onTimer = new Stopwatch();
        /// <summary>
        /// OFFディレイ計測タイマ
        /// </summary>
        private Stopwatch _offTimer = new Stopwatch();
        /// <summary>
        /// 遅延時間[ms]
        /// </summary>
        private int _delayTime = 100;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="delayMs">遅延時間[ms]</param>
        public OnOffDelay(int delayMs)
        {
            _delayTime = delayMs;
        }

        /// <summary>
        /// リセット
        /// </summary>
        public void Reset()
        {
            _onTimer.Reset();
            _onTimer.Stop();
            _offTimer.Reset();
            _offTimer.Stop();
        }

        /// <summary>
        /// ON/OFF状態設定
        /// </summary>
        public bool SetState
        {
            set
            {
                if (value)
                {   // ON時 はOFFタイマを停止
                    _offTimer.Stop();
                    _offTimer.Reset();
                    if (_on != value || !_onTimer.IsRunning)
                    {
                        _onTimer.Reset();
                        _onTimer.Start();
                    }
                }
                else
                {   // OFF時はＯＮタイマを停止し、ＯＦＦタイマを開始
                    _onTimer.Stop();
                    _onTimer.Reset();
                    if (_on != value || !_offTimer.IsRunning)
                    {
                        _offTimer.Reset();
                        _offTimer.Start();
                    }
                }
                _on = value;
            }

        }

        /// <summary>
        /// 遅延ＯＮ状態の取得
        /// </summary>
        public bool On
        {
            get
            {
                if (_onTimer.ElapsedMilliseconds > _delayTime)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// 遅延ＯＮ状態の取得
        /// </summary>
        public bool Off
        {
            get
            {
                if (_offTimer.ElapsedMilliseconds > _delayTime)
                    return true;
                return false;
            }
        }
    }
}
