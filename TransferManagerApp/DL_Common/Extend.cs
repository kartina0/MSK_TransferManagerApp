// ----------------------------------------------
// Copyright © 2017 DATALINK
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DL_CommonLibrary
{
    using System.Collections.Generic;
    /// <summary>
    /// IDictionary 拡張
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// キーが登録されているか確認
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsExist<TKey, TValue>(
          this IDictionary<TKey, TValue> source, string key)
        {
            bool ret = false;
            TKey[] keys = source.Keys.ToArray();

            if (Array.IndexOf(keys, key) >= 0)
                ret = true;

            return ret;
        }

        /// <summary>
        /// キーが登録されているか確認
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsExist<TKey, TValue>(
          this IDictionary<TKey, TValue> source, TKey key)
        {
            bool ret = false;
            TKey[] keys = source.Keys.ToArray();

            if (Array.IndexOf(keys, key) >= 0)
                ret = true;

            return ret;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(
          this IDictionary<TKey, TValue> source, TKey key
        )
        {
            if (source == null || source.Count == 0)
                return default(TValue);

            TValue result;
            if (source.TryGetValue(key, out result))
                return result;
            return default(TValue);
        }


    }

    /// <summary>
    /// String クラス拡張
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// string.format を簡略化
        /// </summary>
        /// <param name="format"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Formats(this string format, params object[] values)
        {
            return String.Format(format, values);
        }

        /// <summary>
        /// 20190304-2
        /// 元データの指定バイト位置から指定バイト数分の文字列取得
        /// </summary>
        /// <param name="buf">元データ</param>
        /// <param name="start">開始バイト位置 0～</param>
        /// <param name="len">バイト長</param>
        /// <returns></returns>
        public static string Substring(this string buf, int start, int len, Encoding enc)
        {
            string s = "";
            int count = enc.GetByteCount(buf);

            if (count < start + len) return "";
            if (count < 0) return "";

            byte[] bufBytes = enc.GetBytes(buf);
            byte[] bytes = new byte[len];

            for (int i = 0; i < len; i++)
                bytes[i] = bufBytes[i + start];

            s = enc.GetString(bytes);

            return s;
        }

        /// <summary>
        /// 指定したバイト数の文字列を返す
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static string ToString(this string buf, int byteLen, Encoding enc)
        {
            string s = "";
            bool leftPading = byteLen > 0;
            byteLen = Math.Abs(byteLen);

            int count = enc.GetByteCount(buf);
            byte[] bufBytes = enc.GetBytes(buf);
            byte[] bytes = new byte[byteLen];
            int bufIndex = 0;
            int spaceCnt = byteLen - count;

            for (int i = 0; i < byteLen; i++)
            {
                if (leftPading)
                {   // 左詰め
                    if (i < count)
                    {
                        bytes[i] = bufBytes[bufIndex];
                        bufIndex++;
                    }
                    else
                    {
                        bytes[i] = 0x20;    // スペース
                    }
                }
                else
                {   // 右詰め
                    if (i >= spaceCnt)
                    {
                        bytes[i] = bufBytes[bufIndex];
                        bufIndex++;
                    }
                    else
                    {
                        bytes[i] = 0x20;    // スペース
                    }
                }
            }

            s = enc.GetString(bytes);

            return s;
        }

        /// <summary>
        /// 文字列をワード単位に変換
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static Int16[] ToWord(this string buf, Encoding enc)
        {
            List<Int16> list = new List<short>();

            if (enc == null)
                enc = System.Text.Encoding.GetEncoding("shift_jis");

            byte[] b = enc.GetBytes(buf);

            // バイト -> Int16に格納する為、偶数サイズに変換する
            if (b.Length % 2 != 0)
            {
                Array.Resize<byte>(ref b, b.Length + 1);
            }

            int wordSize = b.Length / 2;    // 変換後のワード数

            for (int i = 0; i < wordSize; i++)
            {
                Int16 data = BitConverter.ToInt16(b, i * 2);
                list.Add(data);
            }

            return list.ToArray();
        }

        /// <summary>
        /// 文字列をワード単位に変換
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static string FromWord(this string s, Int16[] wordBuf, int offset, int len, Encoding enc)
        {
            List<byte> list = new List<byte>();

            if (enc == null)
                enc = System.Text.Encoding.GetEncoding("shift_jis");

            // 長さが足りないときの処理
            if (wordBuf.Length <= offset)
            {
                return "";
            }

            if (wordBuf.Length <= offset + len)
            {
                len = wordBuf.Length - offset - 1;
            }


            for (int i = offset; i < offset + len; i++)
            {
                byte l = (byte)(wordBuf[i] & 0xFF);
                byte h = (byte)((wordBuf[i] & 0xFF00) >> 8);

                if (l == 0) break;      // NULLで終了
                list.Add(l);
                if (h == 0) break;      // NULLで終了
                list.Add(h);

            }

            return enc.GetString(list.ToArray());
        }



        /// <summary>
        /// 数値を表しているのか調べる
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string buf)
        {
            double v = 0;
            return double.TryParse(buf, out v);
        }
        /// <summary>
        /// 日時を表しているのか調べる
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        public static bool IsDateTime(this string buf)
        {
            DateTime v;
            return DateTime.TryParse(buf, out v);
        }

        /// <summary>
        /// 引数の文字列が半角英数字のみで構成されているかを調べる。
        /// </summary>
        /// <param name="text">チェック対象の文字列。</param>
        /// <returns>引数が英数字のみで構成されていればtrue、そうでなければfalseを返す。</returns>
        public static bool IsOnlyAlphanumeric(this string buf)
        {
            // 文字列の先頭から末尾までが、英数字のみとマッチするかを調べる。
            return (Regex.IsMatch(buf, @"^[0-9a-zA-Z]+$"));
        }
    }

    /// <summary>
    /// Int関連のクラス拡張
    /// </summary>
    public static class IntExtecsions
    {

        /// <summary>
        /// 指定したワードデータを文字列に変換
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToString(this Int16[] buf, int offset, int len, Encoding enc)
        {
            string s = "";
            List<byte> list = new List<byte>();

            if (enc == null)
                enc = System.Text.Encoding.GetEncoding("shift_jis");

            // 長さが足りないときの処理
            if (buf.Length <= offset) return "";
            if (buf.Length <= offset + len)
            {
                len = buf.Length - offset - 1;
            }


            for (int i = offset; i < offset + len; i++)
            {
                byte l = (byte)(buf[i] & 0xFF);
                byte h = (byte)((buf[i] & 0xFF00) >> 8);

                if (l == 0) break;      // NULLで終了
                list.Add(l);
                if (h == 0) break;      // NULLで終了
                list.Add(h);

            }

            s = enc.GetString(list.ToArray());
            return s;
        }
    }

    /// <summary>
    /// 他拡張
    /// </summary>
    public static class ObjectExtension
    {

        /// <summary>
        /// ディープコピーの複製を作る
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T src)
        {
            using (var memoryStream = new System.IO.MemoryStream())
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                
                binaryFormatter.Serialize(memoryStream, src);           // シリアライズ
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);

                return (T)binaryFormatter.Deserialize(memoryStream);    // デシリアライズ
            }
        }
    }
}
