// ----------------------------------------------
// Copyright © 2017 DATALINK
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DL_CommonLibrary
{
    public static class SoundPlay
    {
        private static System.Media.SoundPlayer player = null;

        //WAVEファイルを再生する
        public static void PlaySound(string waveFile)
        {

            if (!FileIo.ExistFile(waveFile))
            {
                Console.Beep(3000, 300);
            }
            else
            {
                //再生されているときは止める
                if (player != null)
                    StopSound();

                //読み込む
                player = new System.Media.SoundPlayer(waveFile);
                //非同期再生する
                player.Play();
            }

            //次のようにすると、ループ再生される
            //player.PlayLooping();

            //次のようにすると、最後まで再生し終えるまで待機する
            //player.PlaySync();
        }

        //再生されている音を止める
        public static void StopSound()
        {
            if (player != null)
            {
                player.Stop();
                player.Dispose();
                player = null;
            }
        }
    }
}
