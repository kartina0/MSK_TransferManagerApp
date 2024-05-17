using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech;
using System.Speech.Synthesis;
using System.Globalization;

namespace DL_CommonLibrary
{
    /// <summary>
    /// 文字列読み上げ
    /// </summary>
    public static class VoiceMessage
    {
        /// <summary>
        /// システム既定の音声名を取得する
        /// </summary>
        public static string DefaultVoice
        {
            get
            {
                var synth = new SpeechSynthesizer();
                CultureInfo culture = CultureInfo.CurrentCulture;
                string name = "";
                foreach (var voice in synth.GetInstalledVoices())
                {
                    if (voice.VoiceInfo.Culture.Name == culture.Name)
                    {
                        name = voice.VoiceInfo.Name;
                        break;
                    }
                    // もし言語が無かった場合は始めに見つかった音声名を返す
                    if (name == "")
                        name = name = voice.VoiceInfo.Name;
                }
                return name;
            }
        }

        /// <summary>
        /// 音声読み上げ
        /// </summary>
        /// <param name="msg">読み上げるメッセージ</param>
        /// <param name="async">非同期</param>
        /// <param name="rate">速度(-10～10)</param>
        public static void Speaker(string msg, bool async = false, int rate = 1)
        {
            try
            {
                string name = DefaultVoice;
                if (rate < -10) rate = -10;
                if (rate > 10) rate = 10;

                var t = new System.Threading.Thread(() =>
                {
                    if (name != "")
                    {
                        var synth = new SpeechSynthesizer();
                        synth.SelectVoice(name);
                        //synth.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Child, 1, CultureInfo.CurrentCulture);
                        //synth.SetOutputToDefaultAudioDevice();
                        synth.Rate = rate;
                        synth.Speak(msg);
                    }
                });

                // 音声読み上げスレッド開始
                t.Start();

                // 非同期でない場合は音声が終わるまで待つ
                if (!async) t.Join();

            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured: '{0}'", e);
            }
        }
    }
}
