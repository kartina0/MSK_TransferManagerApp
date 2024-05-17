// ----------------------------------------------
// Copyright © 2017 DATALINK
// 
// @@20180701       V1.1.0.0
//      起動時ファイルを古いもの順に一度読込む
// @@20180701-2     V1.1.0.0
//      ファイル自動読込の最大行数をプロパティーから指定可能とする
// @@20180821-1
//      ファイル自動読込時のヘッダーを指定できるよう機能追加
//     
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel.Design;
using System.Design;

namespace DL_CustomCtrl
{
    /// <summary>
    /// ファイル自動読み込み/表示するデータグリッド
    /// ヘッダーはデザイン時に作成されていなければ
    /// データの最初の行をヘッダーとして取り扱う
    /// ヘッダーに HeaderSeparateCharで指定した文字がある場合はヘッダーを２段表示する
    /// </summary>
    public partial class MultiHeaderDataGrid : DataGridView
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
        private List<string[]> _readCsvData = new List<string[]>();

        /// <summary>
        /// 読み込みヘッダー
        /// ※最初のファイルのみ
        /// </summary>
        private string[] _readCsvHeater = new string[0];

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
        /// ヘッダ区切り文字
        /// ヘッダを２段表示する場合の区切り文字
        /// </summary>
        private char _headerSeparateChar = '|';

        /// <summary>
        /// ヘッダ高さ
        /// ※デザイン時に列ヘッダーを作成していない場合
        /// </summary>
        private int _headerDefaultHeight = 100;
        /// <summary>
        /// 初回確認
        /// </summary>
        private bool _firstLoad = false;
        /// <summary>
        /// 起動時のファイル読み込み
        /// </summary>
        private bool _firstFileLoad = true;

        /// <summary>
        /// @@20180821-1
        /// ヘッダーをファイルからではなく関数経由で設定
        /// </summary>
        private bool _fixedHeaderStyle = false;
        private string _fixedHeaterString = "";

        /// <summary>
        /// @@20190616
        /// 最大行数までデータを追記するモード
        /// FALSE時は毎回リセットして再描画する
        /// </summary>
        private bool _appendMode = true;
        #endregion


        /// <summary>
        /// ハンドル作成
        /// </summary>
        protected override void CreateHandle()
        {
            base.CreateHandle();
            return;

            // ライセンス確認
            if (this.DesignMode && !LicenceCheck.IsLicence(this.GetType().Name))
            {
                if (!_firstLoad)
                {
                    MessageBox.Show("ライセンスが無い為、使用できません");
                }
                _firstLoad = true;
            }
            else
            {
                base.CreateHandle();

            }
        }


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
        /// ヘッダー区切り文字をの設定
        /// </summary>
        [Category("Data"),
         Description("ヘッダ区切り文字"),
         DefaultValue("|"),
        ]
        public char HeaderSeparateChar
        {
            get { return _headerSeparateChar; }
            set
            {
                _headerSeparateChar = value;
            }
        }

        /// <summary>
        /// ヘッダー区切り文字をの設定
        /// </summary>
        [Category("Data"),
         Description("ヘッダ高さ"),
         DefaultValue("|"),
        ]
        public int HeaderDefaultHeight
        {
            get { return _headerDefaultHeight; }
            set
            {
                _headerDefaultHeight = value;
            }
        }

        /// <summary>
        /// ヘッダー区切り文字をの設定
        /// </summary>
        [Category("Data"),
         Description("ファイル自動更新時の表示最大行数"),
         DefaultValue(100),
        ]
        public int AutoUpdateFileLineCount
        {
            get { return _maxLineCount; }
            set
            {
                _maxLineCount = value;
            }
        }

        /// <summary>
        /// @@20180821-1
        /// ヘッダー設定
        /// </summary>
        [Category("Data"),
         Description("ヘッダー文字列"),
         DefaultValue(""),
        ]
        public string HeaderItems
        {
            get
            {
                return _fixedHeaterString;
            }
            set
            {
                _fixedHeaterString = value;
                _readCsvHeater = value.Split(',');

                if (_readCsvHeater.Length > 0)
                    _fixedHeaderStyle = true;
                else
                    _fixedHeaderStyle = false;
            }
        }

        /// <summary>
        /// @@20181010
        /// </summary>
        [Category("Data"),
         Description("ヘッダー文字列をファイルから読み取る"),
         DefaultValue(""),
        ]
        public bool EnableHeaderTextFromFile
        {
            get { return !_fixedHeaderStyle; }
            set
            {
                _fixedHeaderStyle = !value;
            }
        }


        /// <summary>
        /// @@20190614
        /// </summary>
        [Category("Data"),
         Description("追記モード"),
         DefaultValue(""),
        ]
        public bool EnableAppendMode
        {
            get { return _appendMode; }
            set
            {
                _appendMode = value;
            }
        }

        /// <summary>
        /// ファイル更新時にデータを自動更新
        /// デザイン時の設定は不可
        /// </summary>
        #region "Public Property"
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
                if(!_firstFileLoad) // @@20180701
                    targetFiles = GetFileListSortUpdateTime(_targetDirectory, _fileFilter);
                else
                    targetFiles = GetFileListSortUpdateTime_Reverse(_targetDirectory, _fileFilter);
                
            }
            else
            {
                targetFiles[0] = e.FullPath;
            }
            // ファイル読み込み
            ReadCsvFileData(targetFiles);
            _firstFileLoad = false; // @@20180701

            try
            {
                if (this.Visible && !this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        UpdateDataGrid();
                    });
                }
            }
            catch { }
        }

        /// <summary>
        /// CSVファイルデータを読み込む
        /// </summary>
        /// <param name="files"></param>
        private void ReadCsvFileData(string[] files)
        {
            try
            {

                bool sameFile = false;
                int     readEndLine = 0;
                List<string[]> csvData = new List<string[]>();

                // データクリア
                //_readCsvData.Clear();
                if(!_fixedHeaderStyle)
                    _readCsvHeater = new string[0];
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

                    string buf ="";
                    using (System.IO.FileStream fs = new FileStream(files[i], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        // 日本語指定で開く
                        using(TextReader tr = new StreamReader(fs, System.Text.Encoding.GetEncoding("shift_jis")))
                        {
                            buf = tr.ReadToEnd();

                        }
                    }
                    buf = buf.Replace("\n","");
                    buf = buf.Trim();
                    // 1ファイル読み込み
                    string[] lines = buf.Split('\r');

                    if(!_appendMode)
                    {
                        _prevLatestFileLineCount = 0;
                        _readCsvData.Clear();
                    }

                    // 前回読みこんだ続きから読み込み登録する
                    if (sameFile)
                    {
                        readEndLine = _prevLatestFileLineCount;
                    }
                    else
                    {
                        if (_fixedHeaderStyle)
                            readEndLine = 0;
                        else
                            readEndLine = 1;    // ヘッダー行は読み込まないようにするため 1 にしておく
                    }
                    // 登録する行数を確認
                    int regLine = lines.Length - readEndLine;
                    if (regLine < 0) regLine = 0;
                    
                    // 最終行から指定行まで登録する
                    for (int x = readEndLine; x < lines.Length; x++)
                    {
                        string[] temp = lines[x].Split(',');

                        if (lines[x] != "")
                            _readCsvData.Add(temp);

                        //if (_readCsvData.Count > _maxLineCount) break;
                    }


                    // 最大行数を超える場合は登録前に削除
                    if (_readCsvData.Count() > _maxLineCount)
                    {
                        int delLine = _readCsvData.Count() - _maxLineCount;
                        _readCsvData.RemoveRange(0, delLine);
                    }

                    /// ヘッダ登録
                    if (_firstFileLoad)
                    {
                        if (!_fixedHeaderStyle && i == 0)
                        {
                            _readCsvHeater = lines[0].Split(',');
                        }
                        _prevLatestFilePath = files[i];
                        _prevLatestFileLineCount = lines.Length;
                        
                    }
                    else
                    {
                        if (!_fixedHeaderStyle && i == 0)
                        {
                            _readCsvHeater = lines[0].Split(',');
                        }
                        _prevLatestFilePath = files[i];
                        _prevLatestFileLineCount = lines.Length;
                    }

                    // @@20200902-1
                    // 前回読み出したファイルと同一名でファイルが新規作成された場合
                    // 同一ファイルと判断させないように、ファイル作成日時でも比較する
                    if (_prevLatestFilePath != "")
                    {
                        System.IO.FileInfo prevFile = new FileInfo(_prevLatestFilePath);
                        _prevLatestFileCreationTime = prevFile.CreationTime;
                    }


                }
            }
            catch { }
        }

        /// <summary>
        /// データグリッ更新
        /// </summary>
        private void UpdateDataGrid()
        {
            // 
            // 列が登録されていな場合はHeaderデータを設定
            if (this.Columns.Count <= 0)
            {
                for (int i = 0; i < _readCsvHeater.Length; i++)
                {
                    DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                    col.HeaderText = _readCsvHeater[i];
                    
                    this.Columns.Add(col);
                }
                this.ColumnHeadersHeight = HeaderDefaultHeight;
            }

            // 全行のデータを書き込む
            int index = this.FirstDisplayedScrollingRowIndex;
            this.Rows.Clear();
            for (int i = 0; i < _readCsvData.Count(); i++)
                this.Rows.Add(_readCsvData[i]);
            if(this.Rows.Count<index) index = this.Rows.Count-1;
            if (index > 0) this.FirstDisplayedScrollingRowIndex = index;

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MultiHeaderDataGrid()
        {
            InitializeComponent();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="container"></param>
        public MultiHeaderDataGrid(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
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

            Array.Sort(infos, delegate(System.IO.FileInfo f1, System.IO.FileInfo f2)
            {
                return -f1.LastWriteTime.CompareTo(f2.LastWriteTime);
            }
            );

            for (int i = 0; i < infos.Length; i++)
                result[i] = dir　+"\\" + infos[i].Name;

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

            Array.Sort(infos, delegate(System.IO.FileInfo f1, System.IO.FileInfo f2)
            {
                return f1.Name.CompareTo(f2.Name);
            }
            );

            for (int i = 0; i < infos.Length; i++)
                result[i] = infos[i].Name;

            return result;
        }

        #endregion


        #region "override"

        /// <summary>
        /// ヘッダー描画
        /// ヘッダを　HeaderSeparateChar で指定した文字列で区切ると２行表示する
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            SolidBrush backColor = new SolidBrush(e.CellStyle.BackColor);
            // SolidBrush backColor = new SolidBrush(Color.Red);

            SolidBrush foreColor = new SolidBrush(e.CellStyle.ForeColor);

            SolidBrush gridForeColor = new SolidBrush(this.ColumnHeadersDefaultCellStyle.ForeColor);
            Pen gridLine = new Pen(this.GridColor);

            //　列ヘッダーかどうか調べる
            if (e.ColumnIndex < 0 && e.RowIndex >= 0)
            {
                //セルを描画する
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All);

                //行番号を描画する範囲を決定する
                //e.AdvancedBorderStyleやe.CellStyle.Paddingは無視しています
                Rectangle indexRect = e.CellBounds;
                indexRect.Inflate(-2, -2);
                //行番号を描画する
                TextRenderer.DrawText(e.Graphics,
                    (e.RowIndex + 1).ToString(),
                    e.CellStyle.Font,
                    indexRect,
                    e.CellStyle.ForeColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
                //描画が完了したことを知らせる
                e.Handled = true;
            }

            // カラムヘッダー
            if (e.RowIndex == -1 && e.Value != null)
            {
                // カンマ区切りで AAA,BBB とヘッダが設定されていたら AAAをヘッダー上部 BBBをヘッダ下部に描画する
                // 2行までしか対応していない
                string[] header = e.Value.ToString().Split(HeaderSeparateChar);


                if (header.Length == 1)
                {   // ヘッダー１行の場合

                    // ヘッダーを削除
                    e.Graphics.FillRectangle(backColor, e.CellBounds);


                    StringFormat format = new StringFormat();

                    format.Alignment = StringAlignment.Center;
                    format.FormatFlags = StringFormatFlags.NoWrap;
                    format.Trimming = StringTrimming.EllipsisCharacter;

                    int x = e.CellBounds.X;
                    int y = e.CellBounds.Y;
                    int w = e.CellBounds.Width;
                    int h = e.CellBounds.Height;

                    float fontSize = DefaultCellStyle.Font.Size;


                    // ヘッダ文字
                    //e.Graphics.DrawString(header[0], e.CellStyle.Font, foreColor, x + w / 2, y, format);
                    //Font font0 = new Font(e.CellStyle.Font.Name, e.CellStyle.Font.Size, FontStyle.Bold);
                    Font font0 = new Font(e.CellStyle.Font.Name, fontSize, FontStyle.Bold);

                    e.Graphics.DrawString(header[0], font0, foreColor, x + w / 2, y + (h / 2) - fontSize, format);

                    // 外枠線 描画
                    e.Graphics.DrawRectangle(gridLine, x - 1, y - 1, e.CellBounds.Right, e.CellBounds.Bottom);

                    // 下線（上の外枠線では下線が合わないのでここで下線を描画）
                    e.Graphics.DrawLine(gridLine, x, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);

                    // 最終列の場合、セルのない個所に外枠線が描画されるのでここで消してしまう
                    if (e.ColumnIndex >= this.Columns.Count - 1)
                    {
                        // ヘッダーを削除
                        backColor = new SolidBrush(this.BackgroundColor);
                        e.Graphics.FillRectangle(backColor, x + w + 4, y + 1, w, h);
                    }

                    e.Handled = true;

                }
                else if (header.Length >= 2)
                {   // ヘッダー２行

                    int center = e.CellBounds.Bottom / 2;

                    StringFormat format = new StringFormat();

                    format.Alignment = StringAlignment.Center;
                    format.FormatFlags = StringFormatFlags.NoWrap;
                    format.Trimming = StringTrimming.EllipsisCharacter;

                    int x = e.CellBounds.X;
                    int y = e.CellBounds.Y;
                    int w = e.CellBounds.Width;
                    int h = e.CellBounds.Height;
                    float fontSize = DefaultCellStyle.Font.Size;

                    // セルの削除
                    e.Graphics.FillRectangle(backColor, x, y + h / 2, w, h / 2);

                    // １行目ヘッダー描画
                    string[] leftHeader = new string[0];
                    leftHeader = this.Columns[e.ColumnIndex - 1].HeaderText.Split(HeaderSeparateChar);  // 左列のヘッダー取得
                                                                                                        // 左セルのヘッダー１行目と同じ文字列なら描画しない
                    if (leftHeader.Length >= 1 && leftHeader[0] != header[0])
                    {
                        // グループセル幅を得る
                        int ww = w;
                        string[] rightHeader = new string[0];
                        for (int idx = e.ColumnIndex + 1; idx < this.Columns.Count; idx++)
                        {
                            // 右セルのヘッダー１行目と異なった文字列ならグループセルを終える
                            rightHeader = this.Columns[idx].HeaderText.Split(HeaderSeparateChar);       // 右列のヘッダー取得
                            if (rightHeader.Length >= 1 && rightHeader[0] != header[0])
                                break;
                            ww += w;    // セル幅の加算
                        }

                        // グループセルの削除
                        e.Graphics.FillRectangle(backColor, x, y, ww, h / 2);

                        // ヘッダ1行目描画 
                        //e.Graphics.DrawString(header[0], e.CellStyle.Font, gridForeColor, x + ww / 2, y, format);
                        //Font font1 = new Font("Arial", 9, FontStyle.Bold);
                        Font font1 = new Font(e.CellStyle.Font.Name, fontSize, FontStyle.Bold);

                        // 文字を分割したセルの中心に描画
                        e.Graphics.DrawString(header[0], font1, gridForeColor, x + ww / 2, y + (h / 4) - fontSize, format);

                        // １行目ヘッダーを分ける左縦線
                        e.Graphics.DrawLine(gridLine, x - 1, y + 1, x - 1, center);

                    }

                    // ヘッダ２行目描画
                    //e.Graphics.DrawString(header[1], e.CellStyle.Font, foreColor, x + w / 2, y + center, format);
                    Font font2 = new Font(e.CellStyle.Font.Name, fontSize, FontStyle.Bold);
                    e.Graphics.DrawString(header[1], font2, foreColor, x + w / 2, y + center + (h / 4) - fontSize, format);

                    // 2行目 外枠線 描画
                    e.Graphics.DrawRectangle(gridLine, x - 1, y + center - 1, e.CellBounds.Right, e.CellBounds.Bottom);

                    // 下線（上の外枠線では下線が合わないのでここで下線を描画）
                    e.Graphics.DrawLine(gridLine, x, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);


                    // 最終列の場合、セルのない個所に外枠線が描画されるのでここで消してしまう
                    if (e.ColumnIndex >= this.Columns.Count - 1)
                    {
                        // ヘッダーを削除
                        backColor = new SolidBrush(this.BackgroundColor);
                        e.Graphics.FillRectangle(backColor, x + w + 4, y + 1, w, h);
                    }
                    e.Handled = true;
                }

            }

            base.OnCellPainting(e);

        }

        #endregion

    }
}
