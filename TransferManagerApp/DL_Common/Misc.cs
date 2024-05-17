// ----------------------------------------------
// Copyright © 2017 DATALINK
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DL_CommonLibrary
{
    public static class Misc
    {

        /// <summary>
        /// yyyyMMddHHmmssをDateTime型に変換
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static DateTime ConvToDateTime(string s)
        {
            int year    = 0;
            int month   = 0;
            int day     = 0;
            int hour    = 0;
            int minute  = 0;
            int sec     = 0;

            if (s.Length >= 4)
                year = int.Parse(s.Substring(0, 4));
            if (s.Length >= 6)
                month = int.Parse(s.Substring(4, 2));
            if (s.Length >= 8)
                day = int.Parse(s.Substring(6, 2));
            if (s.Length >= 10)
                hour = int.Parse(s.Substring(8, 2));
            if (s.Length >= 12)
                minute = int.Parse(s.Substring(10, 2));
            if (s.Length >= 14)
                sec = int.Parse(s.Substring(12, 2));

            return new DateTime(year, month, day, hour, minute, sec);
        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     文字列が数値であるかどうかを返します。</summary>
        /// <param name="stTarget">
        ///     検査対象となる文字列。<param>
        /// <returns>
        ///     指定した文字列が数値であれば true。それ以外は false。</returns>
        /// -----------------------------------------------------------------------------
        public static bool IsNumeric(string str)
        {
            double dNullable;

            return double.TryParse(
                str,
                System.Globalization.NumberStyles.Any,
                null,
                out dNullable
            );
        }
        /// <summary>
        /// グレイスケールより疑似カラー取得
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Color GetRGBFrom16bit(int v)
        {
            int r, g, b;
            if (v > 20000) v = v;
            int step = 0x10000 / 4;

            if (v < step * 2)
                r = 0;
            else
                r = (v - step * 2) * 4;

            if (v < step * 3)
                g = v * 4;
            else
                g = 255 - ((v - step * 3) * 4);

            if (v < step)
                b = 255;
            else
                b = 255 - ((v - step) * 4);


            if (r < 0)
                r = 0;
            if (r > 255)
                r = 255;
            if (g < 0)
                g = 0;
            if (g > 255)
                g = 255;
            if (b < 0)
                b = 0;
            if (b > 255)
                b = 255;

            return Color.FromArgb(r, g, b);
        }


        /// <summary>
        /// グレイスケールより疑似カラー取得
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Color GetRGB(int v)
        {
            int r, g, b;
            if (v < 127)
                r = 0;
            else
                r = (v - 127) * 4;

            if (v < 191)
                g = v * 4;
            else
                g = 255 - ((v - 191) * 4);

            if (v < 63)
                b = 255;
            else
                b = 255 - ((v - 63) * 4);


            if (r < 0)
                r = 0;
            if (r > 255)
                r = 255;
            if (g < 0)
                g = 0;
            if (g > 255)
                g = 255;
            if (b < 0)
                b = 0;
            if (b > 255)
                b = 255;

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// グレイスケールより疑似カラー取得
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Color GetRGB(double v, double min, double max)
        {
            double r, g, b;

            if (v < min) v = min;
            if (v > max) v = max;

            double steps = 255 / (max - min);

            v = (v - min) * steps;


            if (v < 127)
                r = 0;
            else
                r = (v - 127) * 4;

            if (v < 191)
                g = v * 4;
            else
                g = 255 - ((v - 191) * 4);

            if (v < 63)
                b = 255;
            else
                b = 255 - ((v - 63) * 4);


            if (r < 0)
                r = 0;
            if (r > 255)
                r = 255;
            if (g < 0)
                g = 0;
            if (g > 255)
                g = 255;
            if (b < 0)
                b = 0;
            if (b > 255)
                b = 255;

            return Color.FromArgb((int)r, (int)g, (int)b);
        }

        /// <summary>
        /// p1-p2間の角度を求める
        /// </summary>
        /// <param name="p1">基準点</param>
        /// <param name="p2">計測点</param>
        /// <returns></returns>
        public static double GetDegree(PointF p1, PointF p2)
        {
            double deg = 0;
            try
            {
                double diff_x = p2.X - p1.X;
                double diff_y = p1.Y - p2.Y;    // 画像は上がY0なので
                if (diff_x == 0 && diff_y == 0)
                    deg = 0;
                else if (diff_x == 0 && diff_y > 0)
                    deg = 90;
                else if (diff_x == 0 && diff_y < 0)
                    deg = -90;
                else
                    deg = RadianToDegree(Math.Atan(diff_y / diff_x));


            }
            catch { deg = 0; }
            return deg;
        }
        /// <summary>
        /// p1-p2間の角度を求める
        /// </summary>
        /// <param name="p1">基準点</param>
        /// <param name="p2">計測点</param>
        /// <returns></returns>
        public static double GetDegree_Bmp(PointDbl p1, PointDbl p2)
        {
            double deg = 0;
            try
            {
                double diff_x = p2.X - p1.X;
                double diff_y = p1.Y - p2.Y;    // 画像は上がY0なので
                if (diff_x == 0 && diff_y == 0)
                    deg = 0;
                else if (diff_x == 0 && diff_y > 0)
                    deg = 90;
                else if (diff_x == 0 && diff_y < 0)
                    deg = -90;
                else
                    deg = RadianToDegree(Math.Atan(diff_y / diff_x));


            }
            catch { deg = 0; }
            return deg;
        }

        /// <summary>
        /// p1-p2間の角度を求める
        /// </summary>
        /// <param name="p1">基準点</param>
        /// <param name="p2">計測点</param>
        /// <returns></returns>
        public static double GetDegree(PointDbl p1, PointDbl p2)
        {
            double deg = 0;
            try
            {
                double diff_x = p2.X - p1.X;
                double diff_y = p2.Y - p1.Y;
                if (diff_x == 0 && diff_y == 0)
                    deg = 0;
                else if (diff_x == 0 && diff_y > 0)
                    deg = 90;
                else if (diff_x == 0 && diff_y < 0)
                    deg = -90;
                else
                    deg = RadianToDegree(Math.Atan(diff_y / diff_x));


            }
            catch { deg = 0; }
            return deg;
        }

        /// <summary>
        /// p1-p2間の角度を求める
        /// </summary>
        /// <param name="p1">基準点</param>
        /// <param name="p2">計測点</param>
        /// <returns></returns>
        public static double GetDegree2(PointDbl p1, PointDbl p2)
        {
            double deg = 0;
            try
            {
                double diff_x = p2.X - p1.X;
                double diff_y = (p2.Y - p1.Y) * -1;
                if (diff_x == 0 && diff_y == 0)
                    deg = 0;
                else if (diff_x == 0 && diff_y > 0)
                    deg = 90;
                else if (diff_x == 0 && diff_y < 0)
                    deg = -90;
                else
                {
                    double rad = Math.Atan2(diff_y, diff_x);
                    //if (rad < 0)
                    //{
                    //    rad = rad * 2 * Math.PI;
                    //}

                    deg = RadianToDegree(rad);
                }




            }
            catch { deg = 0; }
            return deg;
        }


        /// <summary>
        /// p1-p2間の角度を求める
        /// </summary>
        /// <param name="p1">基準点</param>
        /// <param name="p2">計測点</param>
        /// <returns></returns>
        public static double GetDegree360(PointDbl p1, PointDbl p2)
        {
            double deg = 0;
            try
            {

                double radian = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
                if (radian < 0) radian = radian + 2 * Math.PI;
                deg =  Math.Floor(radian * 360 / (2 * Math.PI));
            }
            catch { deg = 0; }
            return deg;
        }
        /// <summary>
        /// p1-p2間の角度を求める
        /// ※ビットマップ形式の座標系 (原点左上 右が X+ 下が　Y+)
        /// </summary>
        /// <param name="p1">基準点</param>
        /// <param name="p2">計測点</param>
        /// <returns></returns>
        public static double GetDegree360_Bmp(PointDbl p1, PointDbl p2)
        {
            double deg = 0;
            try
            {

                double radian = Math.Atan2(p1.Y- p2.Y, p2.X - p1.X);
                if (radian < 0) radian = radian + 2 * Math.PI;
                deg = Math.Floor(radian * 360 / (2 * Math.PI));
            }
            catch { deg = 0; }
            return deg;
        }
        /// <summary>
        /// p1-p2間の角度を求める
        /// ※ビットマップ形式の座標系 (原点左上 右が X+ 下が　Y+)
        /// </summary>
        /// <param name="p1">基準点</param>
        /// <param name="p2">計測点</param>
        /// <returns></returns>
        public static double GetDegree180_Bmp(PointDbl p1, PointDbl p2)
        {
            double deg = 0;
            try
            {

                double radian = Math.Atan2(p1.Y - p2.Y, p2.X - p1.X);
                if (radian < 0) radian = radian + 2 * Math.PI;
                deg = Math.Floor(radian * 360 / (2 * Math.PI));
                if (deg > 180)
                    deg -= 360;
            }
            catch { deg = 0; }
            return deg;
        }

        /// <summary>
        /// 直線abとbcの角度を求める
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="ca"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double GetDegree(PointDbl a, PointDbl b, PointDbl c, PointDbl d)
        {
            double deg = 0;
            try
            {

                PointDbl ab = b - a;
                PointDbl cd = d - c;

                // ベクトルの長さ
                double vec_len_ab = GetDistance(a, b);
                double vec_len_cd = GetDistance(c, d);


                //内積とベクトル長さを使ってcosθを求める
                // ベクトルabとcdの内積を求める
                double product = ab.X * cd.X + ab.Y * cd.Y;

                // cosθ
                double cos = product / (vec_len_ab * vec_len_cd);

                // cosθからθを求める
                double theta = Math.Acos(cos);

                deg = Misc.RadianToDegree(theta);

            }
            catch { deg = 0; }
            return deg;
        }

        /// <summary>
        /// p1-p2間の距離を求める
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double GetDistance(PointF p1, PointF p2)
        {
            double distance = 0;
            try
            {
                double diff_x = p2.X - p1.X;
                double diff_y = p1.Y - p2.Y;    // 画像は上がY0なので
                distance = Math.Sqrt(Math.Pow(diff_x, 2) + (Math.Pow(diff_y, 2)));


            }
            catch { distance = 0; }
            return distance;
        }

        /// <summary>
        /// p1-p2間の距離を求める
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double GetDistance(PointDbl p1, PointDbl p2)
        {
            double distance = 0;
            try
            {
                double diff_x = p2.X - p1.X;
                double diff_y = p1.Y - p2.Y;    // 画像は上がY0なので
                distance = Math.Sqrt(Math.Pow(diff_x, 2) + (Math.Pow(diff_y, 2)));


            }
            catch { distance = 0; }
            return distance;
        }

        /// <summary>
        /// p1-p2間の距離を求める
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double GetDistance(RobotPos p1, RobotPos p2)
        {
            double distance = 0;
            try
            {
                double diff_x = p2.X - p1.X;
                double diff_y = p2.Y - p1.Y;
                distance = Math.Sqrt(Math.Pow(diff_x, 2) + (Math.Pow(diff_y, 2)));
            }
            catch { distance = 0; }
            return distance;
        }

        /// <summary>
        /// p1-p2間の距離を求める
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double GetDistance(PointDbl p1, RobotPos p2)
        {
            double distance = 0;
            try
            {
                double diff_x = p2.X - p1.X;
                double diff_y = p2.Y - p1.Y;
                distance = Math.Sqrt(Math.Pow(diff_x, 2) + (Math.Pow(diff_y, 2)));
            }
            catch { distance = 0; }
            return distance;
        }
        /// <summary>
        /// 回転中心とポイントを指定して回転後のポイントを取得する
        /// </summary>
        /// <param name="center">回転中心</param>
        /// <param name="point">回転させたいポイント</param>
        /// <param name="deg">回転させる角度(オフセット)</param>
        /// <returns></returns>
        public static PointDbl RotatePoint(PointDbl center, PointDbl point, double deg)
        {
            double distance = GetDistance(center, point);
            double curDeg = GetDegree2(center, point);
            double offset_x, offset_y;
            PointDbl rp = new PointDbl();
            GetOffset(distance, curDeg, curDeg + deg, out offset_x, out offset_y);
            rp.X = point.X + offset_x;
            rp.Y = point.Y - offset_y;

            return rp;
        }

        /// <summary>
        /// 指定したポイント配列を指定して回転後のポイント配列を取得する
        /// </summary>
        /// <param name="center"></param>
        /// <param name="target"></param>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static PointDbl[] RotatePoint(PointDbl center, PointDbl[] points, double deg)
        {
            PointDbl[] dest = new PointDbl[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                dest[i] = RotatePoint(center, points[i], deg);
            }
            return dest;
        }
        /// <summary>
        /// srcDegの座標をdestDegに移動する際のX,Yオフセット量を求める
        /// </summary>
        /// <param name="srcDeg"></param>
        /// <param name="destDeg"></param>
        /// <param name="offset_x"></param>
        /// <param name="offset_y"></param>
        public static void GetOffset(double distance, double srcDeg, double destDeg, out double offset_x, out double offset_y)
        {
            offset_x = 0;
            offset_y = 0;

            double src_x = distance * Math.Cos(DegreeToRadian(srcDeg));
            double src_y = distance * Math.Sin(DegreeToRadian(srcDeg));

            double dest_x = distance * Math.Cos(DegreeToRadian(destDeg));
            double dest_y = distance * Math.Sin(DegreeToRadian(destDeg));

            offset_x = dest_x - src_x;
            offset_y = dest_y - src_y;
        }
        /// <summary>
        /// srcDegの座標をdestDegに移動する際のX,Yオフセット量を求める
        /// </summary>
        /// <param name="srcDeg"></param>
        /// <param name="destDeg"></param>
        /// <param name="offset_x"></param>
        /// <param name="offset_y"></param>
        public static void GetOffset(PointDbl offset, double srcDeg, double destDeg, out double offset_x, out double offset_y)
        {
            double distance = Math.Sqrt(Math.Pow(offset.X, 2) + (Math.Pow(offset.Y, 2)));

            offset_x = 0;
            offset_y = 0;

            double src_x = distance * Math.Cos(DegreeToRadian(srcDeg));
            double src_y = distance * Math.Sin(DegreeToRadian(srcDeg));

            double dest_x = distance * Math.Cos(DegreeToRadian(destDeg));
            double dest_y = distance * Math.Sin(DegreeToRadian(destDeg));

            offset_x = dest_x - src_x;
            offset_y = dest_y - src_y;
        }


        /// <summary>
        /// 原点(0,0)からの offset座標 の点をdeg分回転させたときのoffset座標を取得する
        /// </summary>
        /// <param name="srcDeg"></param>
        /// <param name="destDeg"></param>
        /// <param name="offset_x"></param>
        /// <param name="offset_y"></param>
        public static void GetOffset(PointDbl before_offset, double deg,out PointDbl after_offset)
        {
            double rad = DegreeToRadian(deg);
            after_offset = new PointDbl();

            after_offset.X = before_offset.X * Math.Cos(rad) - before_offset.Y * Math.Sin(rad);
            after_offset.Y = before_offset.X * Math.Sin(rad) + before_offset.Y * Math.Cos(rad);
        }
        /// <summary>
        /// ラジアン=>度 変換
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static double RadianToDegree(double radian)
        {
            double rs = 0;
            rs = radian * (180 / Math.PI);
            return rs;
        }
        /// <summary>
        /// 度=>ラジアン 変換
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static double DegreeToRadian(double deg)
        {
            double rs = 0;
            rs = deg / (180.0 / Math.PI);
            return rs;
        }
        /// <summary>
        /// ビットマップイメージのコピー
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public static void CopyImage(Bitmap src, ref Bitmap dest)
        {
            int src_width = src.Width;
            int src_height = src.Height;
            int dest_width = dest.Width;
            int dest_height = dest.Height;
            int src_byteSize = GetBitmapPixelSize(src);
            int dest_byteSize = GetBitmapPixelSize(dest);

            int size = src_width * src_height * src_byteSize;

            // Bitmapのフォーマットが違う場合はDestを再作成
            if (src_width != dest_width || src_height != dest_height || src_byteSize != dest_byteSize)
            {
                if (dest != null) dest.Dispose();
                dest = new Bitmap(src_width, src_height, src.PixelFormat);
            }

            // Bitmapをロックし、BitmapDataを取得する
            BitmapData srcBitmapData = null;
            BitmapData destBitmapData = null;
            try
            {
                srcBitmapData = src.LockBits(new Rectangle(0, 0, src_width, src_height), ImageLockMode.WriteOnly, src.PixelFormat);
                destBitmapData = dest.LockBits(new Rectangle(0, 0, dest_width, dest_height), ImageLockMode.WriteOnly, dest.PixelFormat);
                byte[] temp = new byte[size];

                // Srcからマネージド配列へコピー
                System.Runtime.InteropServices.Marshal.Copy(srcBitmapData.Scan0, temp, 0, size);
                // マネージド配列からDestへコピー
                System.Runtime.InteropServices.Marshal.Copy(temp, 0, destBitmapData.Scan0, size);
            }
            catch(Exception ex) { }
            finally
            {
                // BitmapDataのロックを解除する
                if (srcBitmapData != null) src.UnlockBits(srcBitmapData);
                if (destBitmapData != null) dest.UnlockBits(destBitmapData);
            }
        }
        /// <summary>
        /// Bitmapの１ピクセル当たりのバイトサイズを返す
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static int GetBitmapPixelSize(Bitmap bmp)
        {
            int size = 0;
            switch (bmp.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    size = 1;
                    break;
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppGrayScale:
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565:
                    size = 2;
                    break;
                case PixelFormat.Format24bppRgb:
                    size = 3;
                    break;
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb:
                    size = 4;
                    break;
                default:
                    size = 4;
                    break;
            }
            return size;
        }

        /// <summary>
        /// ビットマップから緑色のみ抽出
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static byte[] GetBitmapByte_Green(Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            int srcPixSize = GetBitmapPixelSize(bmp);

            byte[] dest = new byte[width * height];
            byte[] src = new byte[width * height * srcPixSize];
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height),
                                           ImageLockMode.ReadWrite, bmp.PixelFormat);
            Marshal.Copy(data.Scan0, src, 0, src.Length);

            int targetColor = 0;

            // 緑を取得する ※BGRかRGB以外だったらおかしなデータになるので注意
            if (srcPixSize >= 3) targetColor = 1;

#if true
            Parallel.For(0, height, h =>
            {
                for (int w = 0; w < width; w++)
                {
                    int dest_offset = h * width + w;
                    int src_offset = (h * width* srcPixSize) + (w * srcPixSize);
                    dest[dest_offset] = src[src_offset + targetColor];
                }
            });

            bmp.UnlockBits(data);
#endif
            return dest;
        }


        /// <summary>
        /// ビットマップから緑色のみ抽出
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static void CopyBitmap(Bitmap srcBmp, out Bitmap destBmp)
        {
           
            BitmapData imgData = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
            destBmp = new Bitmap(srcBmp.Width, srcBmp.Height, imgData.Stride, srcBmp.PixelFormat, imgData.Scan0);
            srcBmp.UnlockBits(imgData);

        }


        /// <summary>
        /// 中央値を取得する
        /// </summary>
        /// <param name="list"></param>
        public static float GetMedian(float[] list)
        {
            int cnt = list.Length;
            SortedList<string, float> sort = new SortedList<string, float>();
            float median = 0;
            for (int i = 0; i < cnt; i++)
            {
                string key = string.Format("{0}", list[i]);
                while (sort.IsExist(key))
                {
                    key += "_";
                }

                sort.Add(key, list[i]);
            }

            var sortValues = sort.ToArray();
            bool addValue = (cnt % 2 != 0);      // 配列数が奇数/偶数を調べる
            int medianIndex = cnt / 2;

            float min = sortValues[0].Value;
            float max = sortValues[cnt - 1].Value;

            if (addValue)
                median = sortValues[medianIndex].Value;
            else
                median = (sortValues[medianIndex - 1].Value + sortValues[medianIndex].Value) / 2;
            sort.Clear();
            return median;
        }

        /// <summary>
        /// 多角形に対する点の内外判定
        /// 
        /// 内外判定の基本的な考え方として、「内外を判定したい点から発するレイ（ray：一条の光）を仮定し、
        /// レイが多角形の辺を何回横切るかを数え、偶数回横切るとき、点は多角形の外側、奇数回横切るとき、
        /// 点は多角形の内側と判定することができる
        /// </summary>
        /// <param name="points">多角形の点</param>
        /// <param name="target">検査する点</param>
        /// <returns></returns>
        public static bool IsIncludePoint_InPolygon(PointF[] points, PointF target)
        {
            int iCountCrossing = 0;

            PointF point0 = points[0];
            bool bFlag0x = (target.X <= point0.X);
            bool bFlag0y = (target.Y <= point0.Y);

            // レイの方向は、Ｘプラス方向
            for (uint ui = 1; ui < points.Length + 1; ui++)
            {
                PointF point1 = points[ui % points.Length];  // 最後は始点が入る（多角形データの始点と終点が一致していないデータ対応）
                bool bFlag1x = (target.X <= point1.X);
                bool bFlag1y = (target.Y <= point1.Y);

                if (bFlag0y != bFlag1y)
                {   // 線分はレイを横切る可能性あり。
                    if (bFlag0x == bFlag1x)
                    {   // 線分の２端点は対象点に対して両方右か両方左にある
                        if (bFlag0x)
                        {   // 完全に右。⇒線分はレイを横切る
                            iCountCrossing += (bFlag0y ? -1 : 1);   // 上から下にレイを横切るときには、交差回数を１引く、下から上は１足す。
                        }
                    }
                    else
                    {   // レイと交差するかどうか、対象点と同じ高さで、対象点の右で交差するか、左で交差するかを求める。
                        if (target.X <= (point0.X + (point1.X - point0.X) * (target.Y - point0.Y) / (point1.Y - point0.Y)))
                        {   // 線分は、対象点と同じ高さで、対象点の右で交差する。⇒線分はレイを横切る
                            iCountCrossing += (bFlag0y ? -1 : 1);   // 上から下にレイを横切るときには、交差回数を１引く、下から上は１足す。
                        }
                    }
                }
                // 次の判定のために、
                point0 = point1;
                bFlag0x = bFlag1x;
                bFlag0y = bFlag1y;
            }

            // クロスカウントがゼロのとき外、ゼロ以外のとき内。
            return (0 != iCountCrossing);
        }

        /// <summary>
        /// 多角形に対する点の内外判定
        /// 
        /// 内外判定の基本的な考え方として、「内外を判定したい点から発するレイ（ray：一条の光）を仮定し、
        /// レイが多角形の辺を何回横切るかを数え、偶数回横切るとき、点は多角形の外側、奇数回横切るとき、
        /// 点は多角形の内側と判定することができる
        /// </summary>
        /// <param name="points">多角形の点</param>
        /// <param name="target">検査する点</param>
        /// <returns></returns>
        public static bool IsIncludePoint_InPolygon(PointDbl[] points, PointDbl target)
        {
            int iCountCrossing = 0;

            PointDbl point0 = points[0];
            bool bFlag0x = (target.X <= point0.X);
            bool bFlag0y = (target.Y <= point0.Y);

            // レイの方向は、Ｘプラス方向
            for (uint ui = 1; ui < points.Length + 1; ui++)
            {
                PointDbl point1 = points[ui % points.Length];  // 最後は始点が入る（多角形データの始点と終点が一致していないデータ対応）
                bool bFlag1x = (target.X <= point1.X);
                bool bFlag1y = (target.Y <= point1.Y);

                if (bFlag0y != bFlag1y)
                {   // 線分はレイを横切る可能性あり。
                    if (bFlag0x == bFlag1x)
                    {   // 線分の２端点は対象点に対して両方右か両方左にある
                        if (bFlag0x)
                        {   // 完全に右。⇒線分はレイを横切る
                            iCountCrossing += (bFlag0y ? -1 : 1);   // 上から下にレイを横切るときには、交差回数を１引く、下から上は１足す。
                        }
                    }
                    else
                    {   // レイと交差するかどうか、対象点と同じ高さで、対象点の右で交差するか、左で交差するかを求める。
                        if (target.X <= (point0.X + (point1.X - point0.X) * (target.Y - point0.Y) / (point1.Y - point0.Y)))
                        {   // 線分は、対象点と同じ高さで、対象点の右で交差する。⇒線分はレイを横切る
                            iCountCrossing += (bFlag0y ? -1 : 1);   // 上から下にレイを横切るときには、交差回数を１引く、下から上は１足す。
                        }
                    }
                }
                // 次の判定のために、
                point0 = point1;
                bFlag0x = bFlag1x;
                bFlag0y = bFlag1y;
            }

            // クロスカウントがゼロのとき外、ゼロ以外のとき内。
            return (0 != iCountCrossing);
        }

        /// <summary>
        /// ﾘﾝｸﾞﾊﾞｯﾌｧ内の移動平均を求める
        /// </summary>
        /// <param name="data"></param>
        /// <param name="aveCount"></param>
        /// <returns></returns>
        public static double[] GetMovingAverage(double[] data, int aveCount)
        {

            double[] ave = new double[data.Length];
            int sp = aveCount / 2 * -1;
            int ep = aveCount / 2;
            int count = data.Length;

            for (int i = 0; i < count; i++)
            {
                double sum = 0;
                for (int x = sp; x < ep; x++)
                {
                    try
                    {
                        int index = i + x;
                        if (index < 0)
                            index = count + i + x;
                        else if (index >= count)
                            index = index - count;
                        sum += data[index];
                    }
                    catch { }

                }
                ave[i] = sum / 5;
            }
            return ave;

        }

        /// <summary>
        /// 波形のピーク点を取得する
        /// </summary>
        /// <param name="data"></param>
        /// <param name="peakIndex"></param>
        /// <param name="riseIndex"></param>
        /// <param name="downIndex"></param>
        public static double GetPeak(double[] data, out int peakIndex, out int riseIndex, out int fallIndex)
        {

            // 最大値を持つｲﾝﾃﾞｯｸｽを取得
            double max = double.MinValue;
            int maxIndex = 0;
            double[] diff = new double[data.Length];
            int count = data.Length;
            peakIndex = -1; riseIndex = -1; fallIndex = -1;

            for (int i = 0; i < count; i++)
            {
                if (Math.Abs(data[i]) > max)
                {
                    max = data[i];
                    peakIndex = i;
                }

                // 微分
                if (i == 0)
                    diff[i] = data[i] - data[count - 1];
                else
                    diff[i] = data[i] - data[i - 1];
            }

            double prev = data[count - 1];

            int index = 0;
            int maxLoopCount = (int)(count * 1.5);     // カウントの1.5倍は検査する
            while (true)
            {
                if (index >= count) index = 0;

                bool up = prev < data[index];
                if (riseIndex == -1 && up && data[index] == max)
                    riseIndex = index;

                if (riseIndex >= 0 && fallIndex == -1 && !up && data[index] < max)
                {
                    fallIndex = index;
                    break;
                }
                prev = data[index];
                index++;
                maxLoopCount--;
                if (maxLoopCount <= 0)
                {
                    peakIndex = -1;
                    riseIndex = -1;
                    fallIndex = -1;

                    break;
                }
            }

            if (riseIndex >= 0 && fallIndex >= 0)
            {
                if (riseIndex > fallIndex)
                    peakIndex = (fallIndex + (count - riseIndex)) / 2;
                else
                    peakIndex = (fallIndex + riseIndex) / 2;
            }
            return max;
        }

        /// <summary>
        /// 直線p1-p3 と p2-p4の交点を求める
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static PointDbl GetCrossPoint(PointDbl p1, PointDbl p2, PointDbl p3, PointDbl p4)
        {
            PointDbl cp = new PointDbl();
            double s1 = 0;
            double s2 = 0;
            s1 = ((p4.X - p2.X) * (p1.Y - p2.Y) - (p4.Y - p2.Y) * (p1.X - p2.X)) / 2;
            s2 = ((p4.X - p2.X) * (p2.Y - p3.Y) - (p4.Y - p2.Y) * (p2.X - p3.X)) / 2;

            cp.X = p1.X + (p3.X - p1.X) * s1 / (s1 + s2);
            cp.Y = p1.Y + (p3.Y - p1.Y) * s1 / (s1 + s2);

            return cp;

        }

        /// <summary>
        /// 4点の座標で囲まれた面積を求める
        /// </summary>
        /// <param name="p1">左上</param>
        /// <param name="p3">左下</param>
        /// <param name="p2">右下</param>
        /// <param name="p4">右上</param>
        /// <returns></returns>
        public static double CalcAreaSize(PointDbl p1, PointDbl p3, PointDbl p2, PointDbl p4)
        {
            double s1 = 0;
            double s2 = 0;

            s1 = ((p4.X - p2.X) * (p1.Y - p2.Y) - (p4.Y - p2.Y) * (p1.X - p2.X)) / 2;
            s2 = ((p4.X - p2.X) * (p2.Y - p3.Y) - (p4.Y - p2.Y) * (p2.X - p3.X)) / 2;

            return s1 + s2;
        }


        /// <summary>
        /// 指定した矩形のラインインデックスが目標矩形で指定された矩形のどのラインに一致しているのか取得する
        /// 
        /// </summary>
        /// <param name="findLine">見つけたいベース矩形のラインインデックス</param>
        /// <param name="baseRectangle">ベース矩形ポイント *4ポイントのみ</param>
        /// <param name="targetRectangle">目標矩形ポイント *4ポイントのみ</param>
        /// <returns>※一致しない場合は-1</returns>
        public static int GetMatchLine(int findIndex, PointDbl[] baseRectangle, PointDbl[] targetRectangle)
        {

            const int pointCount = 4;   // ポイント数

            int resultIndex = -1;
            int baseIndex = 0;
            int baseNextIndex = 0;

            double deg_base = 0;

            if (baseRectangle.Length != pointCount || targetRectangle.Length != pointCount) return -1;


            if (findIndex < pointCount - 1)
                baseNextIndex = findIndex + 1;
            else
                baseNextIndex = 0;


            // ベース矩形の指定されたラインの角度を取得
            deg_base = Misc.GetDegree180_Bmp(baseRectangle[findIndex], baseRectangle[baseNextIndex]);


            // ベース矩形の指定されたラインの方向を取得
            double diff_x_ref = baseRectangle[findIndex].X - baseRectangle[baseNextIndex].X;
            double diff_y_ref = baseRectangle[findIndex].Y - baseRectangle[baseNextIndex].Y;



            int[] hitCount          = new int[pointCount];           // 
            double[] deg_rect_list  = new double[pointCount];        // 



            // 
            for (int i = 0; i < hitCount.Length; i++)
            {
                double diff_x_rect = 0;
                double diff_y_rect = 0;
                double deg_rect = 0;
                int targetNextIndex = 0;


                if (i < pointCount - 1)
                    targetNextIndex = i + 1;
                else
                    targetNextIndex = 0;


                diff_x_rect = targetRectangle[i].X - targetRectangle[targetNextIndex].X;                    // 線の方向を取得
                diff_y_rect = targetRectangle[i].Y - targetRectangle[targetNextIndex].Y;                    // 線の方向を取得
                deg_rect = Misc.GetDegree180_Bmp(targetRectangle[i], targetRectangle[targetNextIndex]);     // 線の角度を取得

                // 各線の角度を保持する
                deg_rect_list[i] = deg_rect;
                double deg_diff = Math.Abs(Math.Abs(deg_rect) - Math.Abs(deg_base));
                // 向きが一致していたらヒット数をカウントアップ
                // ※60度以上角度に差がある場合はカウントアップしない
                if (deg_diff < 60 && diff_x_ref <= 0 && diff_x_rect <= 0)
                    hitCount[i]++;
                else if (deg_diff < 60 && diff_x_ref >= 0 && diff_x_rect >= 0)
                    hitCount[i]++;

                if (deg_diff < 60 && diff_y_ref <= 0 && diff_y_rect <= 0)
                    hitCount[i]++;
                else if (deg_diff < 60 && diff_y_ref >= 0 && diff_y_rect >= 0)
                    hitCount[i]++;

            }

            //　どの線が最も一致しているのかを取得する
            int maxHitCount = 0;
            int maxHitIndex = -1;
            for (int i = 0; i < hitCount.Length; i++)
            {
                if (hitCount[i] > 0 && maxHitCount <= hitCount[i])
                {
                    maxHitCount = hitCount[i];
                    resultIndex = i;
                }
            }


            // 一致した線の数をカウントする
            List<int> hitList = new List<int>();
            for (int i = 0; i < hitCount.Length; i++)
            {
                if (maxHitCount == hitCount[i])
                {
                    hitList.Add(i);
                }
            }

            // 一致した線分が複数ある場合、角度が近いほうを使用する
            if (hitList.Count >= 2)
            {
                double deg_min_diff = double.MaxValue;

                for (int i = 0; i < hitList.Count; i++)
                {
                    int index = hitList[i];
                    if (Math.Abs(deg_min_diff) > Math.Abs(deg_rect_list[index] - deg_base))
                    {
                        deg_min_diff = Math.Abs(deg_rect_list[index] - deg_base);
                        resultIndex = index;
                    }
                }
            }

            return resultIndex;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr1"></param>
        /// <param name="arr2"></param>
        /// <returns></returns>
        public static bool SameArray<T>(T[] arr1, T[]arr2)
        {
            bool same = true;
            if (arr1 == null && arr2 == null) return true;
            if (arr1 == null || arr2 == null) return false;
            if(arr1.Length == arr2.Length)
            {
                for(int i=0; i < arr1.Length;i++)
                {
                    if(!arr1[i].Equals(arr2[i]))
                    {
                        same = false;
                        break;
                    }
                }
            }
            else
            {
                same = false;
            }
            return same;
        }

        /// <summary>
        /// 指定した文字列をワードで表す配列に変換
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Int16[] ConvertToArray(string s, Encoding enc)
        {
            List<Int16> list = new List<short>();

            if (enc == null)
                enc = System.Text.Encoding.GetEncoding("shift_jis");

            byte[] b = enc.GetBytes(s);

            // バイト -> Int16に格納する為、偶数サイズに変換する
            if (b.Length % 2 != 0)
            {
                Array.Resize<byte>(ref b, b.Length + 1);
            }

            int wordSize = b.Length / 2;    // 変換後のワード数

            for (int i = 0; i < wordSize;i++)
            {
                Int16 data = BitConverter.ToInt16(b, i * 2);
                list.Add(data);
            }

            return list.ToArray();
        }


        /// <summary>
        /// 指定したワードデータを文字列に変換
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ConvertToString(Int16[] buf, int offset, int len, Encoding enc)
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


            for (int i= offset; i < offset + len; i++)
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

        /// <summary>
        /// 指定した文字列をバイトで表す配列に変換
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] ConvertToByteArray(string s, Encoding enc)
        {
            List<byte> list = new List<byte>();

            if (enc == null)
                enc = System.Text.Encoding.GetEncoding("shift_jis");

            byte[] b = enc.GetBytes(s);

            return b;
        }


        /// <summary>
        /// 指定したワードデータを文字列に変換
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ConvertToString(byte[] buf, int offset, int len, Encoding enc)
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
                if (l == 0) break;      // NULLで終了
                list.Add(l);
            }

            s = enc.GetString(list.ToArray());
            return s;
        }


        /// <summary>
        /// バイト長を指定して文字列の先頭を切り出す
        /// </summary>
        /// <param name="value">対象となる文字列</param>
        /// <param name="encode">文字コードを指定する</param>
        /// <param name="size">バイト長</param>
        /// <returns></returns>
        public static string[] SubstringByByte(string buf, System.Text.Encoding encoding, int size)
        {
            List<string> line = new List<string>();
            byte[] b = encoding.GetBytes(buf);
            int bSize = b.Length;
            int sp = 0;
            
            while (true)
            {
                int ep = sp + size;
                if (sp >= bSize) break;

                if (bSize < ep) size = bSize - sp;

                string s = encoding.GetString(b, sp, size);
                line.Add(s);

                sp += size;
            }

            return line.ToArray();
        }




        public static string SubstringByByte(string buf, Encoding encoding, ref int startNum, int count)
        {
            string result = "";
            try
            {
                byte[] b = encoding.GetBytes(buf);
                result = encoding.GetString(b, startNum, count);
                startNum += count;
            }
            catch
            {

            }
            return result;
        }

        /// <summary>
        /// 最大公約数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GetGcd(int a, int b)
        {
            var x = 0;

            while (true)
            {
                // 割り切れたら
                if (a % b == 0)
                {
                    return b;
                }
                else
                {
                    x = a % b;
                    a = b;
                    b = x;

                }
            }

        }
        /// <summary>
        /// 最大公約数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GetGcd(int[] data)
        {
            int a = data[0];
            for (int i = 0; i < data.Length; i++)
                a = GetGcd(a, data[i]);
            return a;
        }
        /// <summary>
        /// 最小公倍数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GetLcm(int a, int b)
        {
            var x = GetGcd(a, b);

            return a * b / x;
        }

        /// <summary>
        /// 最小公倍数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GetLcm(int[] data)
        {
            int a = data[0];
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != 0)
                {
                    a = data[i];
                    break;
                }
            }
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != 0) a = GetGcd(a, data[i]);
            }

            return a;
        }


        /// <summary>
        /// 配列を初期化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <param name="initialValue"></param>
        /// <returns></returns>
        public static T[] CreateArray<T>(T initialValue, int count)
        {
            return Enumerable.Repeat<T>(initialValue, count).ToArray();
        }
    }
}
