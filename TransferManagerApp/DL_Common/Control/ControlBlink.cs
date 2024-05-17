// ----------------------------------------------
// Copyright © 2017 DATALINK
//
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Collections.Concurrent;

namespace DL_CommonLibrary
{
    class ControlBlinkInfo
    {
        /// <summary>
        /// Blink Control ON/OFF
        /// </summary>
        public bool enableBlink = false;
        /// <summary>
        /// Owner Control
        /// </summary>
        public Control ownerControl = null;
        /// <summary>
        /// Blink Target Control
        /// </summary>
        public Control targetControl = null;
        /// <summary>
        /// ON Color
        /// </summary>
        public Color onColor = Color.Aqua;
        /// <summary>
        /// OFF Color
        /// </summary>
        public Color offColor = Color.Gray;
    }

    /// <summary>
    /// ボタンなどの背景色を指定色で切り替えるクラス
    /// ※
    /// </summary>
    public class ControlBlink : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        private static ConcurrentDictionary<string, ControlBlinkInfo> _ControlList = new ConcurrentDictionary<string, ControlBlinkInfo>();
        //private static Dictionary<string, ControlBlinkInfo> _ControlList = new Dictionary<string, ControlBlinkInfo>();
        private static ConcurrentDictionary<string, bool> _prevBlinkSetting = new ConcurrentDictionary<string, bool>();
        private static ConcurrentDictionary<string, bool> _blinkOffFirstUpdate = new ConcurrentDictionary<string, bool>();
        private static ConcurrentDictionary<string, System.Threading.Timer> _BlinkInterval = new ConcurrentDictionary<string, System.Threading.Timer>();

        /// <summary>
        /// 
        /// </summary>
        private static System.Threading.Timer _SystemClock = new System.Threading.Timer(SystemTimerCallback, 0, 0, 500);

        /// <summary>
        /// 500msec Clock
        /// </summary>
        private static bool _sysClock_500ms = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public ControlBlink()
        {

        }

        public void Dispose()
        {
       //     System.Diagnostics.Debug.Print ("lllllllllllllllllllll");
        }



        /// <summary>
        /// Run Control
        /// </summary>
        public void BlinkControl(Control ownerForm, Control targetCtrl, bool on)
        {
            try
            {
                string key = ownerForm.Name + "." + targetCtrl.Name;
               
                if (_ControlList.IsExist(key))
                {
                    ControlBlinkInfo info = _ControlList[key];

                    info.enableBlink = on;
                    if (!on && _prevBlinkSetting[key] != on)
                        _blinkOffFirstUpdate[key] = false;
                    _prevBlinkSetting[key] = on;
                }
            }
            catch { }
        }
        /// <summary>
        /// Run Control
        /// </summary>
        public void BlinkControl(Control ownerForm, Control targetCtrl, int cell, bool on)
        {
            try
            {
                string key = ownerForm.Name + "." + targetCtrl.Name + "." + cell.ToString();

                if (_ControlList.IsExist(key))
                {
                    ControlBlinkInfo info   = _ControlList[key];
          
                    info.enableBlink = on;
                    if (!on && _prevBlinkSetting[key])    // @@20170325-2
                        _blinkOffFirstUpdate[key] = false;
                    _prevBlinkSetting[key] = on;
                }
            }
            catch { }
        }


        /// <summary>
        /// Color Change
        /// </summary>
        public void ChangeColor(Control ownerForm, Control targetCtrl, Color onColor, Color offColor)
        {
            try
            {
                string key = ownerForm.Name + "." + targetCtrl.Name;

                if (_ControlList.IsExist(key))
                {
                    ControlBlinkInfo info = _ControlList[key];
                    info.onColor = onColor;
                    info.offColor = offColor;
                }
            }
            catch { }
        }

        /// <summary>
        /// Color Change
        /// </summary>
        public void ChangeColor(Control ownerForm, Control targetCtrl, int cell, Color onColor, Color offColor)
        {
            try
            {
                string key = ownerForm.Name + "." + targetCtrl.Name + "." + cell.ToString();

                if (_ControlList.IsExist(key))
                {
                    ControlBlinkInfo info = _ControlList[key];
                    info.onColor = onColor;
                    info.offColor = offColor;
                }
            }
            catch { }
        }


        /// <summary>
        /// Get Blink ON/OFF
        /// </summary>
        /// <param name="ownerForm"></param>
        /// <param name="targetCtrl"></param>
        /// <returns></returns>
        public bool IsBlinkOn(Control ownerForm, Control targetCtrl)
        {
            bool on = false;    
            try
            {
                string key = ownerForm.Name + "." + targetCtrl.Name;

                if (_ControlList.IsExist(key))
                {
                    ControlBlinkInfo info = _ControlList[key];
                    on = info.enableBlink;
                }
            }
            catch { }
            return on;
        }

        /// <summary>
        /// Get Blink ON/OFF
        /// </summary>
        /// <param name="ownerForm"></param>
        /// <param name="targetCtrl"></param>
        /// <returns></returns>
        public bool IsBlinkOn(Control ownerForm, Control targetCtrl, int cell)
        {
            bool on = false;
            try
            {
                string key = ownerForm.Name + "." + targetCtrl.Name + "." + cell.ToString();

                if (_ControlList.IsExist(key))
                {
                    ControlBlinkInfo info = _ControlList[key];
                    on = info.enableBlink;
                }
            }
            catch { }
            return on;
        }

        /// <summary>
        /// Add Target Control
        /// </summary>
        /// <param name="ownerForm"></param>
        /// <param name="targetCtrl"></param>
        /// <param name="onColor"></param>
        /// <param name="offColor"></param>
        /// <returns></returns>
        public UInt32 AddControl(Control ownerForm, Control targetCtrl, Color onColor, Color offColor)
        {
            UInt32 rc = 0;
            string key = ownerForm.Name + "." + targetCtrl.Name;
            bool addList = false;
            ControlBlinkInfo info = new ControlBlinkInfo();

            addList = !_ControlList.IsExist(key);


            info.ownerControl   = ownerForm;
            info.targetControl  = targetCtrl;
            info.onColor        = onColor;
            info.offColor       = offColor;
            // 
            if (addList)
            {
                _ControlList.TryAdd(key, info);
                _blinkOffFirstUpdate.TryAdd(key, false);
                _prevBlinkSetting.TryAdd(key, false);
                _BlinkInterval.TryAdd(key, new System.Threading.Timer(Blink_TimerCallback, key, 0, 100));
               // _BlinkInterval = new System.Threading.Timer(Blink_TimerCallback, key, 0, 100);
                _ControlList[key] = info;
            }
            else
            {
                _ControlList[key].ownerControl  = info.ownerControl;
                _ControlList[key].targetControl = info.targetControl;
                _ControlList[key].onColor       = info.onColor;
                _ControlList[key].offColor      = info.offColor;
            }
            

            return rc;
        }


        /// <summary>
        /// Add Target Control
        /// </summary>
        /// <param name="ownerForm"></param>
        /// <param name="targetCtrl"></param>
        /// <param name="onColor"></param>
        /// <param name="offColor"></param>
        /// <returns></returns>
        public UInt32 AddControl(Control ownerForm, Control targetCtrl, int cell, Color onColor, Color offColor)
        {
            UInt32 rc = 0;
            string key = ownerForm.Name + "." + targetCtrl.Name + "." + cell.ToString();
            bool addList = false;
            ControlBlinkInfo info = new ControlBlinkInfo();

            addList = !_ControlList.IsExist(key);


            info.ownerControl = ownerForm;
            info.targetControl = targetCtrl;
            info.onColor = onColor;
            info.offColor = offColor;
            // 
            if (addList)
            {
                _ControlList.TryAdd(key, info);
                _blinkOffFirstUpdate.TryAdd(key, false);
                _prevBlinkSetting.TryAdd(key, false);
                _BlinkInterval.TryAdd(key, new System.Threading.Timer(Blink_TimerCallback, key, 0, 100));
                //_BlinkInterval = new System.Threading.Timer(Blink_TimerCallback, key, 0, 100);
                _ControlList[key] = info;
            }
            else
            {
                _ControlList[key].ownerControl = info.ownerControl;
                _ControlList[key].targetControl = info.targetControl;
                _ControlList[key].onColor = info.onColor;
                _ControlList[key].offColor = info.offColor;
            }


            return rc;
        }
        
        /// <summary>
        /// Blink Timer Call Back Function
        /// </summary>
        /// <param name="state"></param>
        private void Blink_TimerCallback(object state)
        {
            string key = (string)state;
            ControlBlinkInfo info = (ControlBlinkInfo)_ControlList[key];
            try
            {
                if (!info.enableBlink)
                {
                    if (!_blinkOffFirstUpdate[key])
                        info.targetControl.BackColor = info.offColor;
                    _blinkOffFirstUpdate[key] = true;
                    return;
                }
                info.targetControl.Invoke(new ThreadStart(() =>
                {
                    if (_sysClock_500ms)
                        info.targetControl.BackColor = info.onColor;
                    else
                        info.targetControl.BackColor = info.offColor;

                }));
            }
            catch { _BlinkInterval[key].Dispose(); }

        }
        /// <summary>
        /// System Clock Timer
        /// </summary>
        /// <param name="state"></param>
        private static void SystemTimerCallback(object state)
        {
            _sysClock_500ms = !_sysClock_500ms;

        }


    }
}
