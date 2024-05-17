//----------------------------------------------------------
// Copyright (C) 2017 DATALINK Ltd.
//----------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LogSample
{
    // エラーコード
    using ERROR_CODE = LOG_SAMPLE_ERROR_CODE;

    /// <summary>
    /// File Option
    /// </summary>
    enum LOG_FILE_OPTION
    {
        /// <summary>Multi File Mode 現在未使用</summary>
        ENABLE_MULTI_FILE = 0x01,

        /// <summary>Include DateTime</summary>
        ENABLE_DATETIME = 0x02,
    }

    /// <summary>
    /// GetFileListで取得されるファイルリスト順
    /// </summary>
    enum FILE_SORT_TYPE
    {
        /// <summary>名前順</summary>
        Name = 0,
        /// <summary>ファイル更新日(新しい順)</summary>
        UpdateDateTime_New,
        /// <summary>ファイル更新日(古い順)</summary>
        UpdateDateTime_Old,

    }
    
    /// <summary>
    /// ログ項目名
    /// </summary>
    enum LOG_MSG_TYPE
    {
        SYSTEM      = 0x0000,
        ERROR       = 0x0001,
        CONTROL     = 0x0002,
        EXCEPTION   = 0x0004,
        METHOD_IN   = 0x0010,
        METHOD_OUT  = 0x0020,
        SEND        = 0x0040,       
        RECV        = 0x0080,
        INFO        = 0x1000,
        DEBUG       = 0x2000,
    }

    /// <summary>
    /// Log File Class
    /// </summary>
    class LogFile
    {
        /// <summary>
        /// Current FilePath
        /// </summary>
        private string _currentFilePath = "";

        /// <summary>
        /// Log Directory
        /// </summary>
        private string _dir = "";

        /// <summary>
        /// Log File Name
        /// </summary>
        private string _fileName = "";

        /// <summary>
        /// Log File Extension
        /// </summary>
        private string _extension = "log";

        /// <summary>
        /// Include DateTime 
        /// </summary>
        private bool _enableDateTime = false;

        /// <summary>
        /// DateTime Format String
        /// </summary>
        private string _dateTimeFormatString = "{0:yyyyMMdd HHmmss.fff}";


        /// <summary>
        /// Enable DateTime append to FileName 
        /// </summary>
        private bool _enableFileNameAppendDateTime = true;

        /// <summary>
        /// Enable Multi File 
        /// </summary>
        private bool _enableMultiFile = false;

        /// <summary>
        /// Max Line Count(1File)
        /// </summary>
        private int _maxLineCount = 10000;

        /// <summary>
        /// Max File Count
        /// </summary>
        private int _maxFileCount = 5;


        /// <summary>
        /// Writer
        /// </summary>
        private StreamWriter _writer = null;

        /// <summary>
        /// File Lock object
        /// </summary>
        private object _lock = new object();

        /// <summary>
        /// Current Line Count.
        /// </summary>
        private int _currentLines = 0;

        /// <summary>
        /// Current File Count.
        /// </summary>
        private int _currentFiles = 0;

        #region "Public Property"
        /// <summary>
        /// File FullPath Get/Set 
        /// </summary>
        public string FilePath
        {
            get { return string.Format("{0}\\{1}.{2}", _dir, _fileName, _extension); }
            set 
            {
                string dir = "", fname = "", ext = "";
                try
                {

                    string fullPath = System.IO.Path.GetFullPath(value);
                    dir = System.IO.Path.GetDirectoryName(fullPath);
                    fname = System.IO.Path.GetFileNameWithoutExtension(fullPath);
                    ext = System.IO.Path.GetExtension(fullPath);
                    ext = ext.Replace(".", "");

                    _dir = dir;
                    _fileName = fname;
                    _extension = ext;
                }
                catch { }
            }
        }
        /// <summary>
        /// Log File Directory Get/Set
        /// </summary>
        public string Directory
        {
            get { return _dir; }
            set { _dir = value; }
        }

        /// <summary>
        /// Log File Name Get/Set
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        /// <summary>
        /// Log File Extension Get/Set
        /// </summary>
        public string Extension
        {
            get { return _extension; }
            set { _extension = value; }
        }

        /// <summary>
        /// DateTime Format String Get/Set
        /// </summary>
        public string DateTimeFormatString
        {
            get { return _dateTimeFormatString; }
            set { _dateTimeFormatString = value; }
        }

        /// <summary>
        /// Enable DateTime Get/Set
        /// </summary>
        public bool EnableDateTime
        {
            get { return _enableDateTime; }
            set { _enableDateTime = value; }
        }

        /// <summary>
        /// Enable Multi File Get/Set
        /// </summary>
        public bool EnableMultiFileMode
        {
            get { return _enableMultiFile; }
            set { _enableMultiFile = value; }
        }

        /// <summary>
        /// Max Line Count Get/Set
        /// -1 = none Limit
        /// </summary>
        public int MaxLineCount
        {
            get { return _maxLineCount; }
            set { _maxLineCount = value; }
        }

        /// <summary>
        /// Max File Count Get/Set
        /// </summary>
        public int MaxFileCount
        {
            get { return _maxFileCount; }
            set { _maxFileCount = value; }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public LogFile()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="option">LOG_FILE_OPTION</param>
        /// <seealso cref="LOG_FILE_OPTION"/>
        public LogFile(LOG_FILE_OPTION option)
        {
            _enableDateTime = (option & LOG_FILE_OPTION.ENABLE_DATETIME) > 0;
            _enableMultiFile = (option & LOG_FILE_OPTION.ENABLE_MULTI_FILE) > 0;
        }

        /// <summary>
        /// Open Log File
        /// </summary>
        /// <returns></returns>
        public UInt32 Open(bool append = false)
        {
            UInt32 rc = 0;
            try
            {
                lock(_lock)
                {
                    rc = OpenStream(append);
                }
            }
            catch { rc = (UInt32)ERROR_CODE.EXCEPTION; }
            return rc;

        }

        /// <summary>
        /// Close File
        /// </summary>
        /// <returns></returns>
        public UInt32 Close()
        {
            UInt32 rc = 0;
            try
            {
                lock(_lock)
                {
                    // Close File
                    if (_writer != null)
                        _writer.Close();
                    _writer = null;
                }
            }
            catch { rc = (UInt32)ERROR_CODE.EXCEPTION; }
            return rc;
        }

        /// <summary>
        /// ログ書込み
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public UInt32 Write(LOG_MSG_TYPE type, string msg)
        {
            UInt32 rc = 0;
            try
            {
                rc = Write(type.ToString() + "," + msg);
            }
            catch { rc = (UInt32)ERROR_CODE.EXCEPTION; }
            return rc;
        }

        /// <summary>
        /// ログ書込み
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public UInt32 Write(LOG_MSG_TYPE type, DateTime time, string msg)
        {
            UInt32 rc = 0;
            try
            {
                rc = Write(time, type.ToString() + "," + msg);
            }
            catch { rc = (UInt32)ERROR_CODE.EXCEPTION; }
            return rc;
        }

        /// <summary>
        /// ログ書込み
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public UInt32 Write(string msg)
        {
            UInt32 rc = 0;
            string writeMsg = "";
            try
            {
                if (_writer == null) return (UInt32)ERROR_CODE.FILE_NOT_FOUND;

                lock (_lock)
                {
                    if (STATUS_SUCCESS(rc))
                    {
                        if (_enableDateTime)
                        {
                            writeMsg = string.Format(_dateTimeFormatString, DateTime.Now);
                            writeMsg += "," + msg;
                        }
                        else
                        {
                            writeMsg = msg;
                        }

                        // write line
                        _writer.WriteLine(writeMsg);
                        //_writer.Flush();
                        _currentLines++;

                        // Line Check
                        if (_maxLineCount > 0 && _currentLines >= _maxLineCount)
                            rc = OpenStream();

                    }
                }
            }
            catch { rc = (UInt32)ERROR_CODE.EXCEPTION; }
            return rc;
        }

        /// <summary>
        /// ログ書込み(時刻指定)
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public UInt32 Write(DateTime time, string msg)
        {
            UInt32 rc = 0;
            string writeMsg = "";
            try
            {
                if (_writer == null) return (UInt32)ERROR_CODE.FILE_NOT_FOUND;

                lock (_lock)
                {
                    if (STATUS_SUCCESS(rc))
                    {

                        writeMsg = string.Format(_dateTimeFormatString, time);
                        writeMsg += "," + msg;

                        // write line
                        _writer.WriteLine(writeMsg);
                        //_writer.Flush();
                        _currentLines++;

                        // Line Check
                        if (_maxLineCount > 0 && _currentLines >= _maxLineCount)
                            rc = OpenStream();

                    }
                }
            }
            catch { rc = (UInt32)ERROR_CODE.EXCEPTION; }
            return rc;
        }

        /// <summary>
        /// Open Log File
        /// </summary>
        /// <returns></returns>
        public UInt32 OpenStream(bool append = false)
        {
            UInt32 rc = 0;
            try
            {

                // Close File
                if (_writer != null)
                    _writer.Close();
                _writer = null;
                _currentFilePath = "";
                _currentLines = 0;
                _currentFiles = 0;

                // Create Log Directory
                System.IO.Directory.CreateDirectory(_dir);

                // Get Current File List
                string[] files = GetFileList(_dir, _fileName , _extension, FILE_SORT_TYPE.UpdateDateTime_New);


                bool createNewFile = false;

                // 新規ファイル作成するか確認
                if (!append) createNewFile = true;
                if (files.Length <= 0) createNewFile = true;

                // Get File Name
                if (createNewFile)
                    _currentFilePath = string.Format("{0}\\{1}_{2:yyyyMMddHHmmss}.{3}", _dir, _fileName, DateTime.Now, _extension);
                else
                    _currentFilePath = string.Format("{0}\\{1}", _dir, files[0]);


                // ライン数上限時のファイル作成を行うか確認
                // Get Current Line Count
                if (append)
                {
                    _currentLines = GetLineCount();
                }

                if (!createNewFile && _maxLineCount > 0 && _currentLines >= _maxLineCount) 
                    _currentFilePath = string.Format("{0}\\{1}_{2:yyyyMMddHHmmss}.{3}", _dir, _fileName, DateTime.Now, _extension);

                // 14日前のファイルリストを取得
                //files = GetFileList(_dir, _fileName, _extension, FILE_SORT_TYPE.UpdateDateTime_New);
                files = GetFileList(_dir, _fileName, _extension, DateTime.Now.AddDays(-14));
                
                // File Delete
                _currentFiles = files.Length;
                for (int i = 0; i < files.Length; i++)
                {
                    string path = System.IO.Path.Combine(_dir, files[i]);
                    if (path != _currentFilePath)
                        System.IO.File.Delete(path);
                }

                // File Open
                _writer = new StreamWriter(_currentFilePath, append);
                _writer.AutoFlush = true;
            }
            catch { rc = (UInt32)ERROR_CODE.EXCEPTION; }
            return rc;

        }

        /// <summary>
        /// ファイル行数を取得する
        /// </summary>
        /// <returns></returns>
        private int GetLineCount()
        {
            int lines = 0;
            try
            {
                if (System.IO.File.Exists(_currentFilePath))
                {
                    string[] data = System.IO.File.ReadAllLines(_currentFilePath);
                    lines = data.Length;
                }
            }
            catch { }
            return lines;
        }

        /// <summary>
        /// ファイル数を取得する
        /// </summary>
        /// <returns></returns>
        private int GetFileCount()
        {
            int count = 0;
            try
            {
                string[] files = GetFileList(_dir, _fileName, _extension, FILE_SORT_TYPE.UpdateDateTime_New);
                count = files.Length;
            }
            catch { }
            return count;
        }

        /// <summary>
        /// Check Error State
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        private bool STATUS_SUCCESS(UInt32 err)
        {
            return err == (int)ERROR_CODE.STATUS_SUCCESS;
        }


        /// <summary>
        /// 指定フォルダ内のファイル名を取得
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        private static string[] GetFileList(string dirName)
        {
            string[] list = System.IO.Directory.GetFiles(dirName);
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
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        private static string[] GetFileList(string dirName, FILE_SORT_TYPE sortType)
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
                if (sortType == FILE_SORT_TYPE.Name)
                    return f1.Name.CompareTo(f2.Name);
                else
                    return -f1.LastWriteTime.CompareTo(f2.LastWriteTime);
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
        private static string[] GetFileList(string dirName, string extention)
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
        private static string[] GetFileList(string dirName, string extention, FILE_SORT_TYPE sortType)
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
                if (sortType == FILE_SORT_TYPE.Name)
                    return f1.Name.CompareTo(f2.Name);
                else if (sortType == FILE_SORT_TYPE.UpdateDateTime_New)
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
        /// 指定フォルダ内の指定ファイル名を含むファイルリストを取得
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        private static string[] GetFileList(string dirName, string fileName, string extention, FILE_SORT_TYPE sortType)
        {
            List<string> result = new List<string>();
            string[] files = GetFileList(dirName, extention, sortType);

            if (files != null)
            {
                // 同種のログファイルを取得する
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].IndexOf(fileName + "_") == 0)
                        result.Add(files[i]);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// 指定フォルダ内の指定ファイル名を含む　指定日より前に作成されたファイルリストを取得
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        private static string[] GetFileList(string dirName, string fileName, string extention, DateTime date)
        {
            List<string> result = new List<string>();
            string[] files = GetFileList(dirName, extention, FILE_SORT_TYPE.UpdateDateTime_Old);

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

    }
}
