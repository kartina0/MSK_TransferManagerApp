// ----------------------------------------------
// Copyright © 2017 DATALINK
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.IO.Compression;
using System.Drawing.Imaging;
using Microsoft.VisualBasic;
using ErrorCodeDefine;

namespace DL_CommonLibrary
{
    /// <summary>
    /// GetFileListで取得されるファイルリスト順
    /// </summary>
    public enum FileSortType
    {
        /// <summary>名前順</summary>
        Name = 0,
        /// <summary>ファイル更新日(新しい順)</summary>
        UpdateDateTime_New,
        /// <summary>ファイル更新日(古い順)</summary>
        UpdateDateTime_Old,

    }
    /// <summary>
    /// FIleIo.cs  ファイルR/W機能
    /// 2009/11/25 new
    ///
    /// </summary>
    public class FileIo
    {
        /// <summary>
        /// INIファイルからの読み込みAPIの宣言
        /// </summary>
        /// <param name="lpAppName"></param>
        /// <param name="lpKeyName"></param>
        /// <param name="lpDefault"></param>
        /// <param name="lpReturnString"></param>
        /// <param name="nSize"></param>
        /// <param name="lpFilename"></param>
        /// <returns></returns>
        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringW",
                                   SetLastError = true,
                                   CharSet = CharSet.Unicode,
                                   ExactSpelling = true,
                                   CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPrivateProfileString(string lpSectionName,
                                                          string lpKeyName,
                                                          string lpDefault,
                                                          string lpReturnString,
                                                          int nSize,
                                                          string lpFilename);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpAppName"></param>
        /// <param name="lpKeyName"></param>
        /// <param name="lpDefault"></param>
        /// <param name="lpReturnedString"></param>
        /// <param name="nSize"></param>
        /// <param name="lpFileName"></param>
        /// <returns></returns>
        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringA")]
        public static extern uint
          GetPrivateProfileStringByByteArray(string lpAppName,
          string lpKeyName, string lpDefault,
          byte[] lpReturnedString, uint nSize,
          string lpFileName);

        /// <summary>
        /// INIファイルへの書き込みAPIの宣言
        /// </summary>
        /// <param name="lpAppName"></param>
        /// <param name="lpKeyName"></param>
        /// <param name="lpString"></param>
        /// <param name="lpFilename"></param>
        /// <returns></returns>
        [DllImport("KERNEL32.DLL", EntryPoint = "WritePrivateProfileStringW",
                                   SetLastError = true,
                                   CharSet = CharSet.Unicode,
                                   ExactSpelling = true,
                                   CallingConvention = CallingConvention.StdCall)]
        private static extern int WritePrivateProfileString(string lpSectionName,
                                                            string lpKeyName,
                                                            string lpString,
                                                            string lpFilename);



        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr PathCombine([Out] StringBuilder lpszDest, string lpszDir, string lpszFile);


        /// <summary>
        /// INIファイル一括読み込み用バッファ
        /// </summary>
        private static string m_ReadBuf = "";

        //private static string[] m_LineBuf = new string[0];
        /// <summary>
        ///  INIファイル一括読み込みファイル名
        /// </summary>
        private static string m_ReadFileName = "";

        /// <summary>
        /// 指定ディレクトリの作成
        /// </summary>
        /// <param name="filename">ディレクトリ名</param>
        /// <returns></returns>
        public static bool CreateDir(string filename)
        {
            try
            {
                System.IO.Directory.CreateDirectory(filename);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// ディレクトリの有無取得
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool ExistDir(string fileName)
        {
            try
            {
                return System.IO.Directory.Exists(fileName);
            }
            catch { return false; }
        }

        /// <summary>
        /// ファイルの有無取得
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool ExistFile(string fileName)
        {
            try
            {
                return System.IO.File.Exists(fileName);
            }
            catch { return false; }
        }

        /// <summary>
        /// フォルダリスト取得
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string[] GetDirList(string filename)
        {
            string[] result = new string[0];
            try
            {
                string[] dlist = System.IO.Directory.GetDirectories(filename);
                result = new string[dlist.Length];
                int n = 0;
                foreach (string fname in dlist)
                {
                    result[n] = System.IO.Path.GetFileName(fname);
                    n++;
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 指定フォルダ内のファイル名を取得
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public static string[] GetFileList(string dirName)
        {
            string[] result = new string[0];
            try
            {
                string[] list = System.IO.Directory.GetFiles(dirName);
                result = new string[list.Length];
                int n = 0;
                foreach (string fname in list)
                {
                    result[n] = System.IO.Path.GetFileName(fname);
                    n++;
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 指定フォルダ内のファイル名を取得
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public static string[] GetFileList(string dirName, FileSortType sortType)
        {
            string[] list = System.IO.Directory.GetFiles(dirName);
            string[] result = new string[list.Length];
            List<System.IO.FileInfo> fileInfo = new List<System.IO.FileInfo>();

            int n = 0;
            foreach (string fname in list)
            {
                fileInfo.Add(new System.IO.FileInfo(fname));
            }

            System.IO.FileInfo[] infos = fileInfo.ToArray();

            Array.Sort(infos, delegate(System.IO.FileInfo f1, System.IO.FileInfo f2)
            {

                if (sortType == FileSortType.Name)
                    return f1.Name.CompareTo(f2.Name);
                else if (sortType == FileSortType.UpdateDateTime_New)
                    return -f1.LastWriteTime.CompareTo(f2.LastWriteTime);
                else
                    return f1.LastWriteTime.CompareTo(f2.LastWriteTime);
                
            }
            );
            for (int i = 0; i < infos.Length; i++)
                result[i] = infos[i].Name;

            return result;
        }
        
        /// <summary>
        /// 指定フォルダ内のファイル名を取得
        /// ※名前順でソート
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public static string[] GetFileList(string dirName, string extention)
        {
            if (extention == "") extention = "*";
            string[] list = System.IO.Directory.GetFiles(dirName, string.Format("*.{0}", extention));
            string[] result = new string[list.Length];
            int n = 0;
            foreach (string fname in list)
            {
                result[n] = System.IO.Path.GetFileName(fname);
                n++;
            }

            return result;
        }

        /// <summary>
        /// 指定フォルダ内のファイル名を取得
        /// ※名前順でソート
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public static string[] GetFileList(string dirName, string extention, FileSortType sortType)
        {
            if (extention == "") extention = "*";
            if (!System.IO.Directory.Exists(dirName)) return null;
            string[] list = System.IO.Directory.GetFiles(dirName, string.Format("*.{0}", extention));
            string[] result = new string[list.Length];
            List<System.IO.FileInfo> fileInfo = new List<System.IO.FileInfo>();
            int n = 0;

            foreach (string fname in list)
            {
                fileInfo.Add(new System.IO.FileInfo(fname));
            }

            System.IO.FileInfo[] infos = fileInfo.ToArray();

            Array.Sort(infos, delegate(System.IO.FileInfo f1, System.IO.FileInfo f2)
            {
                if (sortType == FileSortType.Name)
                    return f1.Name.CompareTo(f2.Name);
                else if (sortType == FileSortType.UpdateDateTime_New)
                    return -f1.LastWriteTime.CompareTo(f2.LastWriteTime);
                else
                    return f1.LastWriteTime.CompareTo(f2.LastWriteTime);

            }
            );

            for (int i = 0; i < infos.Length; i++)
                result[i] = infos[i].Name;
            return result;
        }

        /// <summary>
        /// 指定フォルダ内のファイル名を取得
        /// </summary>
        /// <param name="dirName"></param>
        /// <param name="fileName"></param>
        /// <param name="extention"></param>
        /// <param name="sortType"></param>
        /// <returns></returns>
        public static string[] GetFileList(string dirName, string fileName, string extention, FileSortType sortType)
        {
            if (extention == "") extention = "*";
            if (fileName == "") fileName = "*";
            if (!System.IO.Directory.Exists(dirName)) return null;
            string[] list = System.IO.Directory.GetFiles(dirName, string.Format("{0}.{1}",fileName, extention));
            string[] result = new string[list.Length];
            List<System.IO.FileInfo> fileInfo = new List<System.IO.FileInfo>();
            int n = 0;

            foreach (string fname in list)
            {
                fileInfo.Add(new System.IO.FileInfo(fname));
            }

            System.IO.FileInfo[] infos = fileInfo.ToArray();

            Array.Sort(infos, delegate (System.IO.FileInfo f1, System.IO.FileInfo f2)
            {
                if (sortType == FileSortType.Name)
                    return f1.Name.CompareTo(f2.Name);
                else if (sortType == FileSortType.UpdateDateTime_New)
                    return -f1.LastWriteTime.CompareTo(f2.LastWriteTime);
                else
                    return f1.LastWriteTime.CompareTo(f2.LastWriteTime);

            }
            );

            for (int i = 0; i < infos.Length; i++)
                result[i] = infos[i].Name;
            return result;
        }
        /// <summary>
        /// 指定したフォルダ内のフォルダリストを取得
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static string[] GetDirectory(string path)
        {
            if (path == "") return new string[0];
            if (!System.IO.Directory.Exists(path)) return new string[0];

            return System.IO.Directory.GetDirectories(path);

        }        
        /// <summary>
        /// 指定したフォルダ内のフォルダリストを取得
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static string[] GetDirectory(string path, bool nameOnly)
        {
            if (path == "") return new string[0];
            if (!System.IO.Directory.Exists(path)) return new string[0];
            string[] dirs = System.IO.Directory.GetDirectories(path);
            List<string> dirList = new List<string>();
            for (int i = 0; i < dirs.Length; i++)
            {
                if (nameOnly)
                    dirList.Add(System.IO.Path.GetFileName(dirs[i]));
                else
                    dirList.Add(dirs[i]);
            }

            return dirList.ToArray();

        }
        /// <summary>
        /// INIファイルへデータを書き込む
        /// ※DATファイルには書けないので注意
        /// </summary>
        /// <PARAM name="Section">セクション名</PARAM>
        /// <PARAM name="Key">キー名</PARAM>
        /// <PARAM name="Value">書き込む文字列</PARAM>
        /// <returns>String value retrieved</returns>
        public static bool WriteIniValue(string fileName, string section, string key, string value)
        {
            bool rs = false;
            try
            {
                int ret = WritePrivateProfileString(section, key, value, fileName);
                rs = (ret != 0);
            }
            catch { rs = false; }
            return rs;
        }
        /// <summary>
        /// INIファイルへデータを書き込む
        /// ※DATファイルには書けないので注意
        /// </summary>
        /// <PARAM name="Section">セクション名</PARAM>
        /// <PARAM name="Key">キー名</PARAM>
        /// <PARAM name="Value">書き込む文字列</PARAM>
        /// <returns>String value retrieved</returns>
        public static bool WriteIniValue_CsvList<T>(string fileName, string section, string key, T[] value)
        {
            bool rs = false;
            try
            {
                if (value == null)
                {
                    int ret = WritePrivateProfileString(section, key, null, fileName);
                }
                else
                {
                    string s = string.Join(",", value);

                    int ret = WritePrivateProfileString(section, key, s, fileName);
                    rs = (ret != 0);
                }
            }
            catch { rs = false; }
            return rs;
        }


        /// <summary>
        /// INIファイル内のデータを取得する
        /// </summary>
        /// <typeparam name="T">戻り値型</typeparam>
        /// <param name="fileName">ファイル名</param>
        /// <param name="section">セクション名</param>
        /// <param name="key">キー名</param>
        /// <returns>値</returns>
        public static bool ReadIniFile<T>(string fileName, string section, string key, ref T value)
        {
            string temp = new string(' ', 255);
            bool rs = false;
            try
            {
                int i = GetPrivateProfileString(section, key, "", temp, 255, fileName);
                if (i > 0)
                {
                    temp = DeleteAfter(temp, ";");     // コメント文字意以降を削除
                    temp = DeleteAfter(temp, "//");     // コメント文字意以降を削除
                    temp = temp.Trim();
                    temp = temp.Split('\0')[0];
                    if (temp != "")
                        value = ConvType<T>(temp);
                    rs = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Fail(ex.Message);
                rs = false;
            }
            return rs;
        }

        /// <summary>
        /// INIファイル内のデータを取得する
        /// </summary>
        /// <typeparam name="T">戻り値型</typeparam>
        /// <param name="fileName">ファイル名</param>
        /// <param name="section">セクション名</param>
        /// <param name="key">キー名</param>
        /// <param name="commentKey">コメントとして取り扱うキー</param>
        /// <returns>値</returns>
        public static bool ReadIniFile<T>(string fileName, string section, string key, string[] commentKey, ref T value)
        {
            string temp = new string(' ', 255);
            bool rs = false;
            try
            {
                int i = GetPrivateProfileString(section, key, "", temp, 255, fileName);
                if (i > 0)
                {
                    if (commentKey != null)
                    {
                        for (int x = 0; x < commentKey.Length; x++)
                        {
                            if (commentKey[x] != null && commentKey[x] != "")
                                temp = DeleteAfter(temp, commentKey[x]);     // コメント文字意以降を削除
                        }
                    }
                    temp = temp.Trim();
                    temp = temp.Split('\0')[0];
                    if (temp != "")
                        value = ConvType<T>(temp);
                    rs = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Fail(ex.Message);
                rs = false;
            }
            return rs;
        }

        /// <summary>
        /// INIファイル内のデータを取得する
        /// </summary>
        /// <typeparam name="T">戻り値型</typeparam>
        /// <param name="fileName">ファイル名</param>
        /// <param name="section">セクション名</param>
        /// <param name="key">キー名</param>
        /// <returns>値</returns>
        public static bool ReadIniFile_CsvList<T>(string fileName, string section, string key, ref T[] value)
        {
            string temp = new string(' ', 255);
            bool rs = false;
            value = new T[0];
            try
            {
                
                int i = GetPrivateProfileString(section, key, "", temp, 255, fileName);
                if (i > 0)
                {
                    temp = DeleteAfter(temp, ";");     // コメント文字意以降を削除
                    temp = DeleteAfter(temp, "//");     // コメント文字意以降を削除
                    temp = temp.Trim();
                    temp = temp.Split('\0')[0];

                    string[] separate = temp.Split(',');
                    if (temp != "")
                    {
                        value = new T[separate.Length];
                        for (int x = 0; x < separate.Length; x++)
                            value[x] = ConvType<T>(separate[x]);
                    }

                    rs = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Fail(ex.Message);
                rs = false;
            }
            return rs;
        }


        /// <summary>
        /// INIファイル内のデータを取得する
        /// </summary>
        /// <typeparam name="T">戻り値型</typeparam>
        /// <param name="fileName">ファイル名</param>
        /// <param name="section">セクション名</param>
        /// <param name="key">キー名</param>
        /// <returns>値</returns>
        public static bool ReadIniFileEx<T>(string fileName, string section, string key, ref T value)
        {
            string temp = new string(' ', 255);
            bool rs = false;
            try
            {
                if (fileName == "")
                {
                    m_ReadBuf = "";
                    m_ReadFileName = "";
                    return true;
                }

                if (m_ReadFileName != fileName)
                {
                    m_ReadBuf = "";
                    System.IO.StreamReader sr = new System.IO.StreamReader(fileName, Encoding.GetEncoding("shift-jis"));
                    m_ReadBuf = sr.ReadToEnd();

                    m_ReadBuf = m_ReadBuf.Replace("\n", "");
                    sr.Close();

                    if (m_ReadBuf != "")
                        m_ReadFileName = fileName;

                }

                string[] lineBuf = m_ReadBuf.Split('\r');
                string searchSection = string.Format("[{0}]", section);
                string currentSection = "";

                // Section開始行を取得
                int sp = Array.IndexOf(lineBuf, searchSection);

                if (sp < 0) return false;

                for (int i = sp; i < lineBuf.Length; i++)
                {
                    temp = lineBuf[i];
                    temp = DeleteAfter(lineBuf[i], ";");     // コメント文字意以降を削除
                    temp = DeleteAfter(temp, "//");          // コメント文字意以降を削除
                    if (temp != "")
                    {
                        // セクション
                        int p1 = temp.IndexOf('[');
                        int p2 = temp.IndexOf(']');
                        if (p1 == 0 && p2 > 0)
                            currentSection = temp.Substring(p1, p2 - p1 + 1);

                        if (currentSection == searchSection)
                        {
                            temp = DeleteAfter(lineBuf[i], "=");

                            if (temp.Trim() == key)
                            {
                                temp = DeleteBefore(lineBuf[i], "=");
                                temp = temp.Trim();
                                if (temp != "")
                                {
                                    value = ConvType<T>(temp);
                                    rs = true;
                                }
                                break;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Fail(ex.Message);
                rs = false;
            }
            return rs;
        }

        /// <summary>
        /// リスト読み込み
        /// 下記のようなリストを読込む
        /// baseKey[0] 
        /// baseKey[1] 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dest"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public static bool ReadIniFileList<T>(string fileName, string section, string baseKey, int maxCount, ref T[] dest)
        {
            bool rs = true;
            try
            {

                // Load From File
                string key = "";
                string temp = "";
                bool exist = false;
                // Get Full Path.
                fileName = System.IO.Path.GetFullPath(fileName);

                if (!FileIo.ExistFile(fileName)) rs = false;

                dest = new T[0];

                if (rs)
                {

                    for (int i = 0; i < maxCount; i++)
                    {
                        key = string.Format("{0}[{1}]", baseKey, i);
                        exist = FileIo.ReadIniFile<string>(fileName, section, key, ref temp);
                        if (exist)
                        {
                            int cnt = dest.Length;
                            Array.Resize<T>(ref dest, cnt + 1);
                            dest[cnt] = FileIo.ConvType<T>(temp);
                        }
                    }
                }
            }
            catch
            {
                rs = false;
            }
            return rs;
        }


        public static string[] GetKeyList(string fileName, string section)
        {

            // 指定セクションのキーの一覧を得る
            byte[] ar1 = new byte[1024];
            uint resultSize1
                  = GetPrivateProfileStringByByteArray(section, null, "", ar1, (uint)ar1.Length, fileName);
            string result1 = Encoding.Default.GetString(
                                    ar1, 0, (int)resultSize1 - 1);

            string[] keys = result1.Split('\0');

            return keys;

        }


        /// <summary>
        /// 指定したキーがＩＮＩファイル上のあるか確認
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsExistParameter(string fileName, string section, string key)
        {
            string temp = new string(' ', 255);
            bool rs = false;
            try
            {
                int i = GetPrivateProfileString(section, key, "", temp, 255, fileName);
                rs = i > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Fail(ex.Message);
                rs = false;
            }
            return rs;
        }


        /// <summary>
        /// 文字列を指定型へ変換
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="val">文字列</param>
        /// <returns></returns>
        public static T ConvType<T>(String val)
        {
            if (typeof(T) == typeof(char))
                return (T)Convert.ChangeType(val[0], typeof(T));
            if (val == "")
            {
                if (typeof(T) != typeof(String))
                    val = "0";
            }
            if (typeof(T) == typeof(String))
            {
                return (T)Convert.ChangeType(val, typeof(T));
            }
            else if (typeof(T) == typeof(DateTime))
            {
                return (T)Convert.ChangeType(val, typeof(T));
            }
            else if (typeof(T) == typeof(TimeSpan))
            {
                TimeSpan ts = TimeSpan.Parse(val);
                return (T)(object)ts;
            }
            else if (typeof(T).IsEnum)
            {
                val = val.Trim();
                return (T)Enum.Parse(typeof(T), val);
            }
            else
            {
                val = val.ToUpper();
                if (val.IndexOf("0X") >= 0)
                {
                    val = val.Replace("0X", "");
                    val = UInt64.Parse(val, System.Globalization.NumberStyles.HexNumber).ToString();
                }
                try
                {
                    return (T)Convert.ChangeType(double.Parse(val), typeof(T));
                }
                catch
                {
                    return (T)Convert.ChangeType(uint.Parse(val), typeof(T));
                }
            }
        }

        /// <summary>
        /// 指定文字列以降の文字を削除します
        /// </summary>
        /// <param name="strSrc"></param>
        /// <param name="strTarget"></param>
        /// <returns></returns>
        public static String DeleteAfter(String strSrc, String strTarget)
        {
            String sBuf = "";

            try
            {
                int intStart = 0;
                intStart = strSrc.IndexOf(strTarget);
                if (intStart >= 0)
                {
                    sBuf = strSrc.Substring(0, intStart);
                }
                else
                {
                    sBuf = strSrc;
                }
            }
            catch
            {
                return null;
            }
            return sBuf;

        }

        /// <summary>
        /// 指定文字列以前の文字を削除します
        /// </summary>
        /// <param name="strSrc"></param>
        /// <param name="strTarget"></param>
        /// <returns></returns>
        public static String DeleteBefore(String strSrc, String strTarget)
        {
            String sBuf = "";

            try
            {
                int intStart = 0;
                intStart = strSrc.IndexOf(strTarget);
                if (intStart >= 0)
                {
                    sBuf = strSrc.Substring(intStart + 1);
                }
                else
                {
                    sBuf = "";
                }
            }
            catch
            {
                return null;
            }
            return sBuf;
        }


        /// <summary>
        /// CSVを配列に変換
        /// 空白削除、項目なし部は配列に含まない
        /// </summary>
        /// <param name="csv"></param>
        /// <returns></returns>
        public static string[] ConvCSVToArray(string csv)
        {
            string[] list = new string[0];
            try
            {
                csv = csv.Replace(" ", "");
                string[] temp = csv.Split(',');
                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp[i] != "")
                    {
                        Array.Resize<string>(ref list, list.Length + 1);
                        list[list.Length - 1] = temp[i];
                    }
                }
            }
            catch
            {
                list = new string[0];
            }
            return list;
        }

        public static double[] ConvCSVToDoubleArray(string csv)
        {
            double[] list = new double[0];
            try
            {
                csv = csv.Replace(" ", "");
                string[] temp = csv.Split(',');
                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp[i] != "")
                    {
                        Array.Resize<double>(ref list, list.Length + 1);
                        list[list.Length - 1] = double.Parse(temp[i]);
                    }
                }
            }
            catch
            {
                list = new double[0];
            }
            return list;
        }


        public static void ConvCSVToDoubleArray(string csv, ref double[] list)
        {
            try
            {
                list = new double[0];
                csv = csv.Replace(" ", "");
                string[] temp = csv.Split(',');
                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp[i] != "")
                    {
                        Array.Resize<double>(ref list, list.Length + 1);
                        list[list.Length - 1] = double.Parse(temp[i]);
                    }
                }
            }
            catch
            {
                list = new double[0];
            }
        }



        public static void ConvCSVToIntArray(string csv, ref Int32[] list)
        {
            try
            {
                list = new Int32[0];
                csv = csv.Replace(" ", "");
                string[] temp = csv.Split(',');
                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp[i] != "")
                    {
                        Array.Resize<Int32>(ref list, list.Length + 1);
                        list[list.Length - 1] = Int32.Parse(temp[i]);
                    }
                }
            }
            catch
            {
                list = new Int32[0];
            }
        }

        /// <summary>
        /// 文字列を暗号化する
        /// </summary>
        /// <param name="sourceString">暗号化する文字列</param>
        /// <param name="password">暗号化に使用するパスワード</param>
        /// <returns>暗号化された文字列</returns>
        public static string EncryptString(string sourceString, string password)
        {
            //RijndaelManagedオブジェクトを作成
            System.Security.Cryptography.RijndaelManaged rijndael =
                new System.Security.Cryptography.RijndaelManaged();

            //パスワードから共有キーと初期化ベクタを作成
            byte[] key, iv;
            GenerateKeyFromPassword(
                password, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
            rijndael.Key = key;
            rijndael.IV = iv;

            //文字列をバイト型配列に変換する
            byte[] strBytes = System.Text.Encoding.GetEncoding(932).GetBytes(sourceString);
            //対称暗号化オブジェクトの作成
            System.Security.Cryptography.ICryptoTransform encryptor =
                rijndael.CreateEncryptor();
            //バイト型配列を暗号化する
            //復号化に失敗すると例外CryptographicExceptionが発生
            byte[] encBytes = encryptor.TransformFinalBlock(strBytes, 0, strBytes.Length);
            //閉じる
            encryptor.Dispose();

            //バイト型配列を文字列に変換して返す
            return System.Convert.ToBase64String(encBytes);
        }

        /// <summary>
        /// 暗号化された文字列を復号化する
        /// </summary>
        /// <param name="sourceString">暗号化された文字列</param>
        /// <param name="password">暗号化に使用したパスワード</param>
        /// <returns>復号化された文字列</returns>
        public static string DecryptString(string sourceString, string password)
        {
            //RijndaelManagedオブジェクトを作成
            System.Security.Cryptography.RijndaelManaged rijndael =
                new System.Security.Cryptography.RijndaelManaged();

            //パスワードから共有キーと初期化ベクタを作成
            byte[] key, iv;
            GenerateKeyFromPassword(
                password, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
            rijndael.Key = key;
            rijndael.IV = iv;

            //文字列をバイト型配列に戻す
            // byte[] strBytes = Encoding.GetEncoding(932).GetBytes(sourceString);
            byte[] strBytes = System.Convert.FromBase64String(sourceString);

            //対称暗号化オブジェクトの作成
            System.Security.Cryptography.ICryptoTransform decryptor =
                rijndael.CreateDecryptor();
            //バイト型配列を復号化する
            byte[] decBytes = decryptor.TransformFinalBlock(strBytes, 0, strBytes.Length);

            //閉じる
            decryptor.Dispose();

            //バイト型配列を文字列に戻して返す
            return System.Text.Encoding.Default.GetString(decBytes);
        }
        /// <summary>
        /// パスワードから共有キーと初期化ベクタを生成する
        /// </summary>
        /// <param name="password">基になるパスワード</param>
        /// <param name="keySize">共有キーのサイズ（ビット）</param>
        /// <param name="key">作成された共有キー</param>
        /// <param name="blockSize">初期化ベクタのサイズ（ビット）</param>
        /// <param name="iv">作成された初期化ベクタ</param>
        private static void GenerateKeyFromPassword(string password,
            int keySize, out byte[] key, int blockSize, out byte[] iv)
        {
            //パスワードから共有キーと初期化ベクタを作成する
            //saltを決める
            byte[] salt = System.Text.Encoding.UTF8.GetBytes("saltは必ず8バイト以上");
            //Rfc2898DeriveBytesオブジェクトを作成する
            System.Security.Cryptography.Rfc2898DeriveBytes deriveBytes =
                new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt);
            //.NET Framework 1.1以下の時は、PasswordDeriveBytesを使用する
            //System.Security.Cryptography.PasswordDeriveBytes deriveBytes =
            //    new System.Security.Cryptography.PasswordDeriveBytes(password, salt);
            //反復処理回数を指定する デフォルトで1000回
            deriveBytes.IterationCount = 1000;

            //共有キーと初期化ベクタを生成する
            key = deriveBytes.GetBytes(keySize / 8);
            iv = deriveBytes.GetBytes(blockSize / 8);
        }

        /// <summary>
        /// ファイル暗号化
        /// </summary>
        /// <param name="srcFile"></param>
        /// <param name="destFile"></param>
        /// <param name="password">パスワード</param>
        /// <param name="deleteSrcFile">暗号化後元ファイルを削除</param>
        /// <returns></returns>
        public static bool EncryptFile(string srcFile, string destFile, string password, bool deleteSrcFile)
        {
            bool rs = true;
            try
            {
                string srcBuf = "";
                string destBuf = "";

                // 元ファイル読み込み
                srcBuf = System.IO.File.ReadAllText(srcFile, Encoding.GetEncoding(932));
                // 文字列暗号化
                destBuf = EncryptString(srcBuf, password);
                // 暗号化ファイル作成
                System.IO.File.WriteAllText(destFile, destBuf, Encoding.GetEncoding(932));

                // ソースファイルの削除
                if (rs && deleteSrcFile)
                    System.IO.File.Delete(srcFile);
            }
            catch { rs = false; }
            return rs;
        }

        /// <summary>
        /// ファイル復号化
        /// </summary>
        /// <param name="srcFile"></param>
        /// <param name="destFile"></param>
        /// <param name="password"></param>
        /// <param name="deleteSrcFile"></param>
        /// <returns></returns>
        public static bool DecryptFile(string srcFile, string destFile, string password, bool deleteSrcFile)
        {
            bool rs = true;
            try
            {

                string srcBuf = "";
                string destBuf = "";


                // 元ファイル読み込み
                srcBuf = System.IO.File.ReadAllText(srcFile, Encoding.GetEncoding(932));

                // 文字列復号化
                destBuf = DecryptString(srcBuf, password);

                // 暗号化ファイル作成
                System.IO.File.WriteAllText(destFile, destBuf, Encoding.GetEncoding(932));

                // ソースファイルの削除
                if (rs && deleteSrcFile)
                    System.IO.File.Delete(srcFile);
            }
            catch { rs = false; }
            return rs;
        }

        /// <summary>
        /// 複数ファイルコピー
        /// </summary>
        /// <param name="srcPath">コピー元フォルダ</param>
        /// <param name="destPath">コピー先フォルダ</param>
        /// <param name="extention">拡張子</param>
        public static UInt32 CopyFiles(string srcDir, string destDir, string extention)
        {
            UInt32 rs = 0;
            try
            {

                // 同一フォルダ上にあるCSVファイルをBMPﾌｫﾙﾀﾞﾍコピーする
                string[] fileNames = FileIo.GetFileList(srcDir, extention);
                for (int i = 0; i < fileNames.Length; i++)
                {
                    string src_file = System.IO.Path.Combine(srcDir, fileNames[i]);
                    string dest_file = System.IO.Path.Combine(destDir, fileNames[i]);
                    FileIo.CopyFile(src_file, dest_file);
                }

            }
            catch { rs = (UInt32)ErrorCodeList.EXCEPTION; }
            return rs;
        }


        /// <summary>
        /// ファイルコピー
        /// </summary>
        /// <param name="srcPath">コピー元ファイル</param>
        /// <param name="destPath">コピー先ファイル</param>
        public static UInt32 CopyFile(string srcPath, string destPath)
        {
            UInt32 rs = 0;
            try
            {
                if (ExistFile(srcPath))
                    System.IO.File.Copy(srcPath, destPath, true); //上書きできない場合、エラーになる
            }
            catch { rs = (UInt32)ErrorCodeList.EXCEPTION; }
            return rs;
        }




        /// <summary>
        /// ファイル削除
        /// </summary>
        /// <param name="filePath"></param>
        public static UInt32 DeleteFile(string filePath)
        {
            UInt32 rs = 0;
            try
            {
                if (ExistFile(filePath))
                    System.IO.File.Delete(filePath);
            }
            catch { rs = (UInt32)ErrorCodeList.EXCEPTION; }
            return rs;
        }

        /// <summary>
        /// 指定フォルダ内の指定ファイル名を含む　指定日より前に作成されたファイルリストを取得
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public static string[] GetFileList(string dirName, string fileName, string extention, DateTime date)
        {
            List<string> result = new List<string>();
            string[] files = GetFileList(dirName, extention, FileSortType.UpdateDateTime_Old);

            if (files != null)
            {

                // 同種のログファイルを取得する
                for (int i = 0; i < files.Length; i++)
                {
                    string fname = System.IO.Path.Combine(dirName, files[i]);
                    DateTime fdt = System.IO.File.GetLastWriteTime(fname);
                    TimeSpan ts = date - fdt;
                    if (ts.Days > 0)
                    {
                        if (files[i].IndexOf(fileName + "_") == 0)
                            result.Add(files[i]);
                    }
                }
            }

            return result.ToArray();
        }


        /// <summary>
        /// 指定フォルダ内の指定ファイル名を含む　指定日より前に作成されたファイルリストを取得
        /// </summary>
        /// <param name="dirName"></param>
        /// <param name="fileName"></param> *の場合は全てのファイルを取得
        /// <returns></returns>
        public static string[] GetFileListEx(string dirName, string fileName, string extention, DateTime date)
        {
            List<string> result = new List<string>();
            string[] files = GetFileList(dirName, extention, FileSortType.UpdateDateTime_Old);
            if (fileName == "") fileName = "*";

            if (files != null)
            {

                // 同種のファイルを取得する
                for (int i = 0; i < files.Length; i++)
                {
                    string fname = System.IO.Path.Combine(dirName, files[i]);
                    DateTime fdt = System.IO.File.GetLastWriteTime(fname);
                    TimeSpan ts = date - fdt;
                    if (ts.Days > 0)
                    {
                        if (fileName == "*" || files[i].IndexOf(fileName) == 0)
                            result.Add(files[i]);
                    }
                }
            }

            return result.ToArray();
        }


        /// <summary>
        /// 指定フォルダ内の指定日より前に作成されたファイルリストを取得
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public static string[] GetFileList(string dirName, DateTime date)
        {
            List<string> result = new List<string>();
            string[] files = GetFileList(dirName, FileSortType.UpdateDateTime_Old);

            if (files != null)
            {

                // 同種のログファイルを取得する
                for (int i = 0; i < files.Length; i++)
                {
                    string fname = System.IO.Path.Combine(dirName, files[i]);
                    DateTime fdt = System.IO.File.GetLastWriteTime(fname);
                    TimeSpan ts = date - fdt;
                    if (ts.Days > 0)
                        result.Add(files[i]);
                }
            }

            return result.ToArray();
        }
        /// <summary>
        /// 指定日より前のログを削除
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool DeleteFileEx(string dir, string baseFileName, DateTime time)
        {
            bool rs = false;
            try
            {
                string ext = System.IO.Path.GetExtension(baseFileName);
                string fname = System.IO.Path.GetFileNameWithoutExtension(baseFileName);

                if (ext[0] == '.') ext = ext.Remove(0, 1);

                // 14日前のファイルリストを取得
                //files = GetFileList(_dir, _fileName, _extension, FILE_SORT_TYPE.UpdateDateTime_New);
                string[] files = FileIo.GetFileListEx(dir, fname, ext, time);

                // File Delete
                for (int i = 0; i < files.Length; i++)
                {
                    string path = System.IO.Path.Combine(dir, files[i]);
                    System.IO.File.Delete(path);
                }

                rs = true;
            }
            catch { rs = false; }
            return rs;
        }
        /// <summary>
        /// 指定日より前のログを削除
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool DeleteLogFile(string dir, string baseFileName,  DateTime time)
        {
            bool rs = false;
            try
            {
                string ext = System.IO.Path.GetExtension(baseFileName);
                string fname = System.IO.Path.GetFileNameWithoutExtension(baseFileName);

                if (ext[0] == '.') ext = ext.Remove(0, 1);

                // 14日前のファイルリストを取得
                //files = GetFileList(_dir, _fileName, _extension, FILE_SORT_TYPE.UpdateDateTime_New);
                string[] files = FileIo.GetFileList(dir, fname, ext, time);

                // File Delete
                for (int i = 0; i < files.Length; i++)
                {
                    string path = System.IO.Path.Combine(dir, files[i]);
                    System.IO.File.Delete(path);
                }

                rs = true;
            }
            catch { rs = false; }
            return rs;
        }

        /// <summary>
        /// 指定日より前のログを削除
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool DeleteFile(string dir, string baseFileName, DateTime time)
        {
            bool rs = false;
            try
            {
                string ext = System.IO.Path.GetExtension(baseFileName);
                string fname = System.IO.Path.GetFileNameWithoutExtension(baseFileName);

                if (ext[0] == '.') ext = ext.Remove(0, 1);

                // 14日前のファイルリストを取得
                //files = GetFileList(_dir, _fileName, _extension, FILE_SORT_TYPE.UpdateDateTime_New);
                string[] files = FileIo.GetFileList(dir, fname, ext, time);

                // File Delete
                for (int i = 0; i < files.Length; i++)
                {
                    string path = System.IO.Path.Combine(dir, files[i]);
                    System.IO.File.Delete(path);
                }

                rs = true;
            }
            catch { rs = false; }
            return rs;
        }
        
        /// <summary>
        /// 指定日より前のログを削除
        /// </summary>
        /// <param name="dir">検索フォルダ</param>
        /// <param name="time">指定日</param>
        /// <returns></returns>
        public static bool DeleteFile(string dir, DateTime time)
        {
            bool rs = false;
            try
            {

                // 指定日よりファイルリストを取得
                string[] files = FileIo.GetFileList(dir, time);

                // File Delete
                for (int i = 0; i < files.Length; i++)
                {
                    string path = System.IO.Path.Combine(dir, files[i]);
                    System.IO.File.Delete(path);
                }

                rs = true;
            }
            catch { rs = false; }
            return rs;
        }
        /// <summary>
        /// 指定した拡張子のファイルを削除
        /// </summary>
        /// <param name="extntion">拡張子</param>
        /// <returns></returns>
        public static bool DeleteFileSelectExtention(string dir, string extention)
        {
            bool rs = false;
            try
            {
                
                // ファイルリストを取得
                string[] files = FileIo.GetFileList(dir, extention);

                // File Delete
                for (int i = 0; i < files.Length; i++)
                {
                    string path = System.IO.Path.Combine(dir, files[i]);
                    System.IO.File.Delete(path);
                }

                rs = true;
            }
            catch { rs = false; }
            return rs;
        }

        /// <summary>
        /// 指定したファイルパスを指定した形式でソートする
        /// </summary>
        /// <param name="filePathList">ファイルパス リスト</param>
        /// <param name="sortType">ソートタイプ</param>
        /// <returns></returns>
        public static string[] SortFileList(string[] filePathList, FileSortType sortType)
        {

            string[] result = new string[filePathList.Length];
            List<System.IO.FileInfo> fileInfo = new List<System.IO.FileInfo>();
            

            foreach (string fname in filePathList)
            {
                fileInfo.Add(new System.IO.FileInfo(fname));
            }

            System.IO.FileInfo[] infos = fileInfo.ToArray();
            
            Array.Sort(infos, delegate (System.IO.FileInfo f1, System.IO.FileInfo f2)
            {
                if (sortType == FileSortType.Name)
                    return f1.Name.CompareTo(f2.Name);
                else if (sortType == FileSortType.UpdateDateTime_New)
                    return -f1.LastWriteTime.CompareTo(f2.LastWriteTime);
                else
                    return f1.LastWriteTime.CompareTo(f2.LastWriteTime);
            }
            );

            for (int i = 0; i < infos.Length; i++)
                result[i] = infos[i].FullName;

            return result;
        }

        /// <summary>
        /// DOSのXCOPY コマンド
        /// Method to Perform Xcopy to copy files/folders from Source machine to Target Machine
        /// </summary>
        /// <param name="SolutionDirectory"></param>
        /// <param name="TargetDirectory"></param>
        public static void ProcessXcopy(string SolutionDirectory, string TargetDirectory)
        {
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            //Give the name as Xcopy
            startInfo.FileName = "xcopy";
            //make the window Hidden
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //Send the Source and destination as Arguments to the process
            startInfo.Arguments = "\"" + SolutionDirectory + "\"" + " " + "\"" + TargetDirectory + "\"" + @" /e /y /I";
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }

        }

        /// <summary>
        /// バッチファイルを実行する
        /// </summary>
        /// <param name="batFilePath"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void CallBatch(string batFilePath , string param, int timeout = 10000)
        {
            try
            {
                // @@20171121-1
                Stopwatch sw = new Stopwatch();
                sw.Start();

                var startInfo = new ProcessStartInfo();
                startInfo.FileName = System.IO.Path.GetFullPath(batFilePath);
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = true;
                startInfo.Arguments = param;
                startInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(batFilePath);
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    // @@20171121-1
                    //exeProcess.WaitForExit();
                    exeProcess.WaitForExit(timeout);

                    // @@20171121-1
                    if (!exeProcess.HasExited)
                    {
                        exeProcess.Kill();
                    }
                    
                }
            }
            catch(Exception ex) { }
        }

        /// <summary>
        /// @@20190516-1
        /// バッチファイルを実行する
        /// </summary>
        /// <param name="batFilePath"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void CallBatch(string batFilePath, string param, bool visible, bool waitComp, int timeout = 10000)
        {
            try
            {
                // @@20171121-1
                Stopwatch sw = new Stopwatch();
                sw.Start();

                var startInfo = new ProcessStartInfo();
                startInfo.FileName = System.IO.Path.GetFullPath(batFilePath);
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = true;
                startInfo.Arguments = param;
                if (visible)
                    startInfo.WindowStyle = ProcessWindowStyle.Normal;
                else
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(batFilePath);
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    if (waitComp)
                    {
                        // @@20171121-1
                        //exeProcess.WaitForExit();
                        exeProcess.WaitForExit(timeout);

                        // @@20171121-1
                        if (!exeProcess.HasExited)
                        {
                            exeProcess.Kill();
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }


        /// <summary>
        /// @@20190516-1
        /// バッチファイルを実行する
        /// </summary>
        /// <param name="batFilePath"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Process CallBatch(string batFilePath, string param, bool visible)
        {
            var startInfo = new ProcessStartInfo();
            Process exeProcess = null;
            try
            {
                // @@20171121-1
                Stopwatch sw = new Stopwatch();
                sw.Start();

                
                startInfo.FileName = System.IO.Path.GetFullPath(batFilePath);
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = true;
                startInfo.Arguments = param;

                if (visible)
                    startInfo.WindowStyle = ProcessWindowStyle.Normal;
                else
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                startInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(batFilePath);
                // Start the process with the info we specified.
                exeProcess = Process.Start(startInfo);
            }
            catch (Exception ex) { exeProcess = null; }
            return exeProcess;
        }

        /// <summary>
        /// プロセス終了するまで待つ
        /// </summary>
        /// <param name="proc"></param>
        /// <returns></returns>
        public static bool WaitProcessComp(Process proc, int timeout)
        {
            return proc.WaitForExit(timeout);
        }

        /// <summary>
        /// プロセスを強制終了する
        /// </summary>
        /// <param name="proc"></param>
        /// <returns></returns>
        public static void CloseProcess(Process proc)
        {
            try
            {
                if (!proc.HasExited)
                {
                    proc.Kill();
                }
            }
            catch { }
        }



        /// <summary>
        /// @@20180910-1
        /// ファイル名に使用できない文字を_に変換する
        /// </summary>
        /// <param name="orgFileName"></param>
        /// <returns></returns>
        public static string ReplaceInvalidFileChar(string orgFileName)
        {
            string dest = orgFileName;
            char[] invalidCar = System.IO.Path.GetInvalidPathChars();
            foreach (char c in invalidCar)
                dest = dest.Replace(c, '_');
            return dest;
        }


        /// <summary>
        /// @@20190302
        /// ファイル1行ずつ読込(全行)
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="enc">エンコーディング</param>
        /// <returns></returns>
        public static string[] ReadAllLine(string filePath, Encoding enc)
        {
            //1行ずつ読込んだリストを作成する
            List<string> list = new List<string>();
            try
            {
                // EXCELで開いている場合は FileShare.ReadWriteを指定しないと読み込めない
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    //using (StreamReader fileData = new StreamReader(filePath, enc))
                    using (StreamReader fileData = new StreamReader(fs, enc))
                    {
                        string lineBuf = "";
                        //1行ずつの読込(\r\nは表示されない)
                        while ((lineBuf = fileData.ReadLine()) != null)
                        {
                            list.Add(lineBuf);
                        }
                        //クローズ
                        fileData.Close();
                    }
                    fs.Close();
                }
            }
            catch(Exception ex) { }

            return list.ToArray(); ;
        }


        /// <summary>
        /// @@20211005
        /// 指定されたセクション内のデータを全行読込
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="senction">セクション</param>
        /// <param name="enc">エンコーディング</param>
        /// <returns></returns>
        public static string[] ReadAllLine(string filePath, string senction, Encoding enc)
        {
            //1行ずつ読込んだリストを作成する
            List<string> list = new List<string>();
            try
            {
                // まず全行読み出し
                string[] buf = ReadAllLine(filePath, enc);

                // セクション検索
                if (senction.IndexOf('[') < 0)
                    senction = "[" + senction + "]";

                int index = -1;
                for (int i = 0; i < buf.Length; i++)
                {
                    if (buf[i].IndexOf(senction) == 0)
                    {
                        index = i;
                        break;
                    }
                }

                if (index >= 0)
                {
                    for (int i = index + 1; i < buf.Length; i++)
                    {

                        if (buf[i].IndexOf('[') == 0 && buf[i].IndexOf(']') > 0)
                        { //次セクション
                            break;
                        }
                        list.Add(buf[i]);
                    }
                }

            }
            catch { }

            return list.ToArray(); ;
        }
        /// <summary>
        /// 指定ファイルが　指定日以上 指定ファイル 指定日以下　に作成されたかの確認
        /// </summary>
        /// <param name="filePath"></param> 更新日を確認するファイル
        /// <param name="dateMore"></param> 指定日以上
        /// <param name="dateLess"></param> 指定日以下
        /// <returns></returns>
        public static bool DateFileCheck(string filePath, DateTime dateMore, DateTime dateLess)
        {
            bool bl = false;
            try
            {
                if (ExistFile(filePath))
                {
                    FileInfo fi = new FileInfo(filePath);
                    DateTime dt = fi.LastWriteTime;
                    if (dateMore <= dt && dateLess >= dt)
                        bl = true;
                }
            }
            catch { }
            return bl;
        }

        /// <summary>
        /// フォルダ作成
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static bool DirCreat(string dirPath)
        {
            bool bl = true;
            try
            {
                if (!ExistDir(dirPath))
                    Directory.CreateDirectory(dirPath);
            }
            catch { bl = false; }
            return bl;
        }

        /// <summary>
        /// @@20190407
        /// 絶対パスから相対パスを取得します
        /// </summary>
        /// <param name="basePath">基準とするフォルダパス</param>
        /// <param name="absolutePath">絶対パス</param>
        /// <returns>相対パス</returns>
        public static string GetRelativePath(string basePath, string absolutePath)
        {
            int len = basePath.Length;
            int index = basePath.LastIndexOf("\\");

            if(len-1 != index)
            {   // basePathで指定したフォルダの最後は \\ を付けておかないとフォルダとして扱えない
                basePath += "\\";
            }

            Uri u1 = new Uri(basePath);
            Uri u2 = new Uri(absolutePath);

            //絶対Uriから相対Uriを取得する
            Uri relativeUri = u1.MakeRelativeUri(u2);
            //文字列に変換する
            string relativePath = ".\\" + relativeUri.ToString();
            //.NET Framework 1.1以前では次のようにする
            //string relativePath = u1.MakeRelative(u2);

            //"/"を"\"に変換する
            relativePath = relativePath.Replace('/', '\\');
            return relativePath;
        }


        /// <summary>
        /// NULLファイルを作成する
        /// </summary>
        /// <param name="filePath"></param>
        public static void CreateNullFile(string filePath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath, false))
                {
                    sw.Close();
                }
            }
            catch { }
        }

        /// <summary>
        /// @@20190526
        /// フォルダ削除
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="deleteSubDir"></param>
        public static void DeleteDir(string dir, bool deleteSubDir)
        {
            try
            {
                if(ExistDir(dir))
                {
                    System.IO.Directory.Delete(dir, deleteSubDir);
                }
            }
            catch(Exception ex) { }
        }

        /// <summary>
        /// @@20210204
        /// フォルダ削除
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="deleteSubDir"></param>
        public static void DeleteDir(string dir, bool deleteSubDir, bool useTrashBox)
        {
            try
            {
                if (ExistDir(dir))
                {
                    if(useTrashBox)
                        Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(dir, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs , Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                    else
                        System.IO.Directory.Delete(dir, deleteSubDir);
                }
            }
            catch (Exception ex) { }
        }


        /// <summary>
        /// @@20200902-2
        /// 指定したフォルダ内のファイル・フォルダを削除
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        public static void DeleteDirContents(string dir)
        {

            if (!FileIo.ExistDir(dir)) return;

            DirectoryInfo dirInfo = new DirectoryInfo(dir);

            //子ディレクトリの削除
            foreach (System.IO.DirectoryInfo directoryInfo in dirInfo.GetDirectories())
            {
                System.IO.Directory.Delete(directoryInfo.FullName, true);
            }

            //ファイルの削除
            foreach (FileInfo fileInfo in dirInfo.GetFiles())
            {
                fileInfo.Delete();
            }
        }

        /// <summary>
        /// @@20190521 
        /// 指定したファイルをZIPファイルに圧縮する
        /// </summary>
        /// <param name="filePath"></param>
        public static void CreateZipFile(string zipFilePath, string[] filePath, Encoding enc)
        {
            try
            {
                using (ZipArchive zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create,enc))
                {
                    for (int i = 0; i < filePath.Length; i++)
                    {
                        zip.CreateEntryFromFile(filePath[i], System.IO.Path.GetFileName(filePath[i]));
                    }
                }
            }
            catch(Exception ex) { }
        }

        /// <summary>
        /// @@20190521 
        /// 指定したフォルダをZIPファイルに圧縮する
        /// </summary>
        public static void CreateZipFile(string dir, string zipFile)
        {
            try
            {
                ZipFile.CreateFromDirectory(dir, zipFile);
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// @@20200902-1
        /// フォルダ内をすべてコピー
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        public static bool DirectoryCopy(string sourcePath, string destinationPath)
        {
            bool ret = true;
            try
            {
                if (!FileIo.ExistDir(sourcePath)) return false;

                DirectoryInfo sourceDirectory = new DirectoryInfo(sourcePath);
                DirectoryInfo destinationDirectory = new DirectoryInfo(destinationPath);

                //コピー先のディレクトリがなければ作成する
                if (destinationDirectory.Exists == false)
                {
                    destinationDirectory.Create();
                    destinationDirectory.Attributes = sourceDirectory.Attributes;
                }

                //ファイルのコピー
                foreach (FileInfo fileInfo in sourceDirectory.GetFiles())
                {
                    //同じファイルが存在していたら、常に上書きする
                    fileInfo.CopyTo(destinationDirectory.FullName + @"\" + fileInfo.Name, true);
                }

                //ディレクトリのコピー（再帰を使用）
                foreach (System.IO.DirectoryInfo directoryInfo in sourceDirectory.GetDirectories())
                {
                    DirectoryCopy(directoryInfo.FullName, destinationDirectory.FullName + @"\" + directoryInfo.Name);
                }

            }
            catch { ret = false; }

            return ret;
        }


        /// <summary>
        /// Bitmap => JPEGファイル変換
        /// </summary>
        /// <param name="bmpFilepath"></param>
        /// <param name="jpegFilePath"></param>
        /// <param name="quality">0-100 値が小さいほどサイズが小さくなる</param>
        public static bool EncodeToJpeg(string bmpFilepath, string jpegFilePath, long quality = 100)
        {
            bool ret = false;
            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(bmpFilepath);
                ImageCodecInfo jpegEncoder = null;
                foreach (ImageCodecInfo ici in ImageCodecInfo.GetImageEncoders())
                {
                    if (ici.FormatID == System.Drawing.Imaging.ImageFormat.Jpeg.Guid)
                    {
                        jpegEncoder = ici;
                        break;
                    }
                }

                if (jpegEncoder != null)
                {
                    EncoderParameter encParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                    EncoderParameters encParams = new EncoderParameters(1);
                    encParams.Param[0] = encParam;
                    image.Save(jpegFilePath, jpegEncoder, encParams);
                    ret = true;
                }
            }
            catch { ret = false; }
            return ret;
        }
    }
}
