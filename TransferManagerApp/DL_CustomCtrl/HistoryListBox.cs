// ----------------------------------------------
// Copyright © 2021 DATALINK
// 
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DL_CustomCtrl
{
    /// <summary>
    /// 指定したファイル内容をリストボックスに表示する
    /// </summary>
    public partial class HistoryListBox : ListBox
    {

        #region "Private Variable"

        /// <summary>
        /// ファイル更新でデータを自動更新
        /// </summary>
        private bool _autoUpdateWithFile = false;

        /// <summary>
        /// データフォルダパス
        /// </summary>
        private string _targetDirectory = "";

        /// <summary>
        /// データファイル名
        /// </summary>
        private string _fileFilter = "";

        /// <summary>
        /// ファイル監視
        /// </summary>
        private FileSystemWatcher _fileWatcher = null;

        /// <summary>
        /// ファイル更新時間
        /// </summary>
        private DateTime _fileLastUpdateTime = new DateTime(0);

        /// <summary>
        /// 最大行数
        /// </summary>
        private int _maxLineCount = 100;

        /// <summary>
        /// 読み込みデータ
        /// </summary>
        private List<string> _readData = new List<string>();

        /// <summary>
        /// 前回更新した最新ファイル名
        /// </summary>
        private string _prevLatestFilePath = "";

        /// <summary>
        /// 前回更新した最新ファイルの読み込んだ行数
        /// </summary>
        private int _prevLatestFileLineCount = 0;

        /// <summary>
        /// @@20200902-1
        /// 前回更新した最新ファイルの作成日時
        /// ※ファイルが新規作成された場合を検知する
        /// </summary>
        private DateTime _prevLatestFileCreationTime = new DateTime();

        /// <summary>
        /// 初回確認
        /// </summary>
        private bool _firstLoad = false;

        /// <summary>
        /// 起動時のファイル読み込み
        /// </summary>
        private bool _firstFileLoad = true;

        /// <summary>
        /// @@20190616
        /// 最大行数までデータを追記するモード
        /// FALSE時は毎回リセットして再描画する
        /// </summary>
        private bool _appendMode = true;
        #endregion

        #region "Public Property"

        /// <summary>
        /// データファイルの指定
        /// ワイルドカードを使用することによりフィルタ可能
        /// </summary>
        [Category("Data"),
         Description("データファイルを指定します"),
         DefaultValue("*.*"),
         Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))
        ]
        public string FileFilter
        {
            get { return _fileFilter; }
            set { _fileFilter = value; }
        }

        /// <summary>
        /// データフォルダの指定
        /// </summary>
        [Category("Data"),
         Description("データフォルダを指定します"),
         DefaultValue(""),
         Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))
        ]
        public string DataDirectory
        {
            get { return _targetDirectory; }
            set { _targetDirectory = value; }
        }
        /// <summary>
        /// ファイル更新時にデータを自動更新
        /// デザイン時の設定は不可
        /// </summary>
        [Browsable(false),
         Category("Data"),
         Description("false : Updateメソッドコール時にデータ更新\rtrue : ファイル更新時に自動更新"),
         DefaultValue(false),
        ]
        public bool AutoUpdateWithFileMode
        {
            get { return _autoUpdateWithFile; }
            set
            {
                try
                {
                    if (value)
                    {
                        // 既存クラスを破棄
                        if (_fileWatcher != null)
                            _fileWatcher.Dispose();
                        _fileWatcher = null;

                        // フォルダ作成
                        Directory.CreateDirectory(_targetDirectory);

                        // フォルダが作成できない場合はエラー
                        if (!System.IO.Directory.Exists(_targetDirectory))
                            throw new DirectoryNotFoundException();

                        _fileWatcher = new FileSystemWatcher();
                        //監視するディレクトリを指定
                        _fileWatcher.Path = _targetDirectory;
                        _fileWatcher.Filter = _fileFilter;
                        //サブディレクトリは監視しない
                        _fileWatcher.IncludeSubdirectories = false;

                        // 最終更新日時の変更のみを監視する
                        // ※ファイルが削除されたときの更新ができない
                        _fileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
                        //_fileWatcher.NotifyFilter = NotifyFilters.LastAccess;//| NotifyFilters.Size;
                        // イベント
                        _fileWatcher.Changed += FileWatcher_Changed;
                        _fileWatcher.EnableRaisingEvents = true;

                    }
                    else
                    {
                        _fileWatcher.EnableRaisingEvents = false;
                    }
                    _autoUpdateWithFile = value;

                    FileWatcher_Changed(new object(), new FileSystemEventArgs(WatcherChangeTypes.Changed, "", ""));
                }
                catch { _autoUpdateWithFile = false; }
            }
        }
        #endregion
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HistoryListBox()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }


        /// <summary>
        /// ファイル更新イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {

            // 自動更新無効時は終了
            if (!_autoUpdateWithFile) return;

            // ファイル更新時刻の取得
            DateTime tmpLastWriteTime = File.GetLastWriteTime(e.FullPath);

            // イベントが２回発生するので同一時刻の更新の場合は無視
            if (_fileLastUpdateTime.Ticks == tmpLastWriteTime.Ticks) return;
            _fileLastUpdateTime = tmpLastWriteTime;

            // 対象ファイル名の取得
            string[] targetFiles = new string[1];
            if (_fileFilter == "" || _fileFilter.IndexOf('*') >= 0)
            {
                if (!_firstFileLoad) // @@20180701
                    targetFiles = GetFileListSortUpdateTime(_targetDirectory, _fileFilter);
                else
                    targetFiles = GetFileListSortUpdateTime_Reverse(_targetDirectory, _fileFilter);

            }
            else
            {
                targetFiles[0] = e.FullPath;
            }
            // ファイル読み込み
            ReadFileData(targetFiles);
            _firstFileLoad = false; // @@20180701
            try
            {
                if (this.Visible && !this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        int topIndex = this.TopIndex;

                        for (int i = 0; i < _readData.Count(); i++)
                        {
                            if (this.Items.Count > _maxLineCount)
                            {
                                this.Items.RemoveAt(0);
                            }
                            this.Items.Add(_readData[i]);
                        }
                        if (topIndex < 0) topIndex = 0;
                        this.TopIndex = topIndex;

                    });
                }
            }
            catch(Exception ex) { }
        }

        #region "private method"

        /// <summary>
        /// 指定フォルダ内のファイル名を更新時間の新しい順に取得
        /// ※サブフォルダは含まない
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        private string[] GetFileListSortUpdateTime(string dir, string filter)
        {
            string[] list = System.IO.Directory.GetFiles(dir, filter);
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
                return -f1.LastWriteTime.CompareTo(f2.LastWriteTime);
            }
            );

            for (int i = 0; i < infos.Length; i++)
                result[i] = dir + "\\" + infos[i].Name;

            return result;
        }

        /// <summary>
        /// @@20180701
        /// 指定フォルダ内のファイル名を更新時間の古い順に取得
        /// ※サブフォルダは含まない
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        private string[] GetFileListSortUpdateTime_Reverse(string dir, string filter)
        {
            string[] list = System.IO.Directory.GetFiles(dir, filter);
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
                return f1.LastWriteTime.CompareTo(f2.LastWriteTime);
            }
            );

            for (int i = 0; i < infos.Length; i++)
                result[i] = dir + "\\" + infos[i].Name;

            return result;
        }
        /// <summary>
        /// 指定フォルダ内のファイル名をファイル名順に取得
        /// ※サブフォルダは含まない
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        private string[] GetFileListSortFileName(string dir, string filter)
        {
            string[] list = System.IO.Directory.GetFiles(dir, filter);
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
                return f1.Name.CompareTo(f2.Name);
            }
            );

            for (int i = 0; i < infos.Length; i++)
                result[i] = infos[i].Name;

            return result;
        }

        /// <summary>
        /// ファイルデータを読み込む
        /// </summary>
        /// <param name="files"></param>
        private void ReadFileData(string[] files)
        {
            try
            {

                bool sameFile = false;
                int readEndLine = 0;
                List<string[]> csvData = new List<string[]>();

                // データクリア
                _readData.Clear();
                for (int i = 0; i < files.Length; i++)
                {
                    // 前回読込んだファイルより今回読込むファイルが古い場合は読み込まない
                    if (_prevLatestFilePath != "")
                    {
                        System.IO.FileInfo prevFile = new FileInfo(_prevLatestFilePath);
                        System.IO.FileInfo curFile = new FileInfo(files[i]);
                        if (prevFile.LastWriteTime > curFile.LastWriteTime) break;
                    }

                    // filesはファイル更新順に並んでいるので
                    // 既に前回読みこんだファイルが完了していたら、ファイルは読み込む必要がない
                    if (sameFile) break;

                    // 前回読み込み対象ファイルか確認
                    if (_prevLatestFilePath == files[i]) sameFile = true;

                    // @@20200902-1
                    // 前回読み出したファイルと同一名でファイルが新規作成された場合
                    // 同一ファイルと判断させないように、ファイル作成日時でも比較する
                    if (sameFile)
                    {
                        System.IO.FileInfo finfo = new FileInfo(files[i]);
                        if (_prevLatestFileCreationTime.Ticks != finfo.CreationTime.Ticks) sameFile = false;
                    }

                    string buf = "";
                    using (System.IO.FileStream fs = new FileStream(files[i], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        // 日本語指定で開く
                        using (TextReader tr = new StreamReader(fs, System.Text.Encoding.GetEncoding("shift_jis")))
                        {
                            buf = tr.ReadToEnd();

                        }
                    }
                    buf = buf.Replace("\n", "");
                    buf = buf.Trim();
                    // 1ファイル読み込み
                    string[] lines = buf.Split('\r');

                    if (!_appendMode)
                    {
                        _prevLatestFileLineCount = 0;
                        _readData.Clear();
                    }

                    // 前回読みこんだ続きから読み込み登録する
                    if (sameFile)
                    {
                        readEndLine = _prevLatestFileLineCount;
                    }
                    else
                    {
                        readEndLine = 0;
                    }
                    // 登録する行数を確認
                    int regLine = lines.Length - readEndLine;
                    if (regLine < 0) regLine = 0;

                    // 最終行から指定行まで登録する
                    for (int x = readEndLine; x < lines.Length; x++)
                    {
                        if (lines[x] != "")
                            _readData.Add(lines[x]);

                    }

                    _prevLatestFilePath = files[i];
                    _prevLatestFileLineCount = lines.Length;
                    // @@20200902-1
                    // 前回読み出したファイルと同一名でファイルが新規作成された場合
                    // 同一ファイルと判断させないように、ファイル作成日時でも比較する
                    if (_prevLatestFilePath != "")
                    {
                        System.IO.FileInfo prevFile = new FileInfo(_prevLatestFilePath);
                        _prevLatestFileCreationTime = prevFile.CreationTime;
                    }


                }


                // 最大行数を超える場合は登録前に削除
                if (_readData.Count() > _maxLineCount)
                {
                    int delLine = _readData.Count() - _maxLineCount;
                    _readData.RemoveRange(0, delLine);
                }

            }
            catch { }
        }
        #endregion
    }
}
