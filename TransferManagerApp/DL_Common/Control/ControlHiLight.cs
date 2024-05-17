using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace DL_CommonLibrary
{
    /// <summary>
    /// 
    /// </summary>
    class ControlHiLightInfo
    {
        public System.Threading.Timer timer = null;
        /// <summary>
        /// 
        /// </summary>
        public Control control = null;
        /// <summary>
        /// 
        /// </summary>
        public Color orgColor = Color.LightGray;
    }

    /// <summary>
    /// 指定したコントロールを一定時間色を変更する
    /// </summary>
    public class ControlHiLight
    {
        /// <summary>
        /// タイマリスト
        /// </summary>
        private ConcurrentDictionary<string, ControlHiLightInfo> _info = new ConcurrentDictionary<string, ControlHiLightInfo>();

        /// <summary>
        /// オリジナルカラー
        /// </summary>
        private static ConcurrentDictionary<string, Color> _orgColor = new ConcurrentDictionary<string, Color>();

        /// <summary>
        /// 指定した時間コントロールの色を変更
        /// </summary>
        /// <param name="ctrl">対象コントロール</param>
        /// <param name="targetColor">変更する色</param>
        /// <param name="returnColorTime">元の色に戻すまでの時間[ms]</param>
        public void Set(Control ctrl, Color targetColor, int returnColorTime)
        {
            string key = ctrl.Name;
            System.Threading.Timer timer = null;
            try
            {
                if (_info.IsExist(key))
                {   // 動作中は登録しない
                    return;
                }
                else
                {
                    
                    ControlHiLightInfo ctrlInfo = new ControlHiLightInfo();
                    timer = new System.Threading.Timer(_TimerCallback, ctrlInfo, returnColorTime, Timeout.Infinite);
                    ctrlInfo.timer = timer;
                    ctrlInfo.orgColor = ctrl.BackColor;
                    ctrlInfo.control = ctrl;

                    //@@20181219
                    //ctrl.BackColor = targetColor;
                    Control parent = ctrlInfo.control.Parent;
                    if (parent != null)
                    {
                        parent.Invoke((MethodInvoker)delegate
                        {
                            ctrl.BackColor = targetColor;
                        });
                    }
                   
                    _info[key] = ctrlInfo;

                }
            }
            catch { }
        }
        /// <summary>
        /// System Clock Timer
        /// </summary>
        /// <param name="state"></param>
        private void _TimerCallback(object state)
        {
            try
            {
                ControlHiLightInfo info = (ControlHiLightInfo)state;
                //@@20181219
                //info.control.BackColor = info.orgColor;
                Control parent = info.control.Parent;
                if (parent != null)
                {
                    parent.Invoke((MethodInvoker)delegate
                    {
                        info.control.BackColor = info.orgColor;
                    });
                }

                info.timer.Change(Timeout.Infinite, Timeout.Infinite);
                info.timer.Dispose();
                info.timer = null;
                _info.TryRemove(info.control.Name, out info);
            }
            catch { }
        }
    }
}
