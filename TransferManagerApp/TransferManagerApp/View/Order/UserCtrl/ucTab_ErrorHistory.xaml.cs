//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using SystemConfig;
using DL_Logger;
using ErrorCodeDefine;
using ShareResource;


namespace TransferManagerApp
{
    /// <summary>
    /// ucErrorHistory.xaml の相互作用ロジック
    /// </summary>
    public partial class ucTab_ErrorHistory : UserControl, IDisposable
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "ucTab_WorkStatus";

        /// <summary>
        /// ウィンドウ表示中フラグ
        /// </summary>
        public bool isShowing = false;

        ///// <summary>
        ///// エラー履歴フォルダ
        ///// </summary>
        //private string _dir = "";
        /// <summary>
        /// エラー履歴ファイルパス
        /// </summary>
        private string _filePath = "";
        /// <summary>
        /// 前回更新日時
        /// </summary>
        private DateTime _preUpdateDt = DateTime.MinValue;

        /// <summary>
        /// 画面更新タイマー
        /// </summary>
        private DispatcherTimer _tmrUpdateDisplay = null;
        /// <summary>
        /// タイマー重複ロック
        /// </summary>
        private bool _timeLock = false;

        /// <summary>
        /// エラー履歴
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindErrorHistory> _errorHistoryList = new ObservableCollection<BindErrorHistory>();



        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ucTab_ErrorHistory()
        {
            InitializeComponent();
        }
        /// <summary>
        /// ウィンドウロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                    return;


                // 表の各カラムのWidthを調整
                double w = listviewErrorHistory.ActualWidth;
                double rate = 0.1;
                columnErrorHistory_No.Width = Math.Floor(w * rate * 0.5);
                columnErrorHistory_DateTime.Width = Math.Floor(w * rate * 1.8);
                columnErrorHistory_Type.Width = Math.Floor(w * rate * 1.2);
                columnErrorHistory_Content.Width = Math.Floor(w * rate * 6.5);

                //BindErrorHistory b1 = new BindErrorHistory() 
                //{
                //    No = "1",
                //    DateTime = "2023/09/05 16:27:51",
                //    Type = ErrorType.APPLICATION.ToString(),
                //    Content = "Iniファイルが存在しない"
                //};
                //_errorHistoryList.Add(b1);
                //BindErrorHistory b2 = new BindErrorHistory()
                //{
                //    No = "2",
                //    DateTime = "2023/09/05 16:53:29",
                //    Type = ErrorType.SERVER.ToString(),
                //    Content = "データベース接続エラー"
                //};
                //_errorHistoryList.Add(b2);
                listviewErrorHistory.ItemsSource = _errorHistoryList;

                // エラー履歴ファイルパス
                //_dir = Path.GetFullPath(Const.ErrorHistoryDir);
                _filePath = IniFile.ErrorHistoryFilePath;
                // エラー履歴ファイル読み出し
                rc = ReadErrorHistoryFile();


                // ウィンドウ表示中
                isShowing = true;

                // タイマー初期化
                rc = InitTimer();
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
        }
        /// <summary>
        /// ウィンドウクローズ
        /// </summary>
        public void Dispose()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // ウィンドウ表示終了
                isShowing = false;
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, false);
            }
        }


        /// <summary>
        /// 画面更新タイマー初期化
        /// </summary>
        private UInt32 InitTimer()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // タイマのインスタンスを生成
                _tmrUpdateDisplay = new DispatcherTimer();
                // インターバルを設定
                _tmrUpdateDisplay.Interval = TimeSpan.FromMilliseconds(100);
                // タイマメソッドを設定
                _tmrUpdateDisplay.Tick += new EventHandler(tmrUpdateDisplay_tick);
                // タイマを開始
                _tmrUpdateDisplay.Start();
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{GetType().Name}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 画面更新タイマー トリガー
        /// </summary>
        private void tmrUpdateDisplay_tick(object sender, EventArgs e)
        {
            UpdateDisplay();
        }
        /// <summary>
        /// 画面更新
        /// </summary>
        private void UpdateDisplay()
        {
            UInt32 rc = 0;
            try
            {
                // 重複防止
                if (_timeLock)
                    return;
                _timeLock = true;

                //初期化完了確認
                if (!Resource.SystemStatus.initialize_Completed) return;


                // エラー履歴ファイル読み出し
                rc = ReadErrorHistoryFile();



            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                Resource.ErrorHandler(ex);
            }
            finally
            {
                _timeLock = false;
            }
        }


        /// <summary>
        /// エラー履歴ファイル読み出し
        /// </summary>
        /// <returns></returns>
        private UInt32 ReadErrorHistoryFile() 
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{GetType().Name}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // ファイル存在確認
                if (!File.Exists(_filePath))
                    return rc;

                // 更新日時確認
                DateTime updateDt = File.GetLastWriteTime(_filePath);
                if (_preUpdateDt >= updateDt)
                    return rc;


                _preUpdateDt = updateDt;

                // 読み出し
                _errorHistoryList.Clear();
                using (StreamReader reader = new StreamReader(_filePath, Encoding.GetEncoding("shift_jis")))
                {
                    bool header = true;
                    // 末尾まで繰り返す
                    int no = 1;
                    while (!reader.EndOfStream)
                    {
                        // CSVファイルの一行を読み込む
                        string line = reader.ReadLine();
                        // ヘッダーは無視 
                        if (header)
                        {
                            header = false;
                            continue;
                        }

                        // 読み込んだ一行をカンマ毎に分けて配列に格納する
                        string[] values = line.Split(',');
                        BindErrorHistory b = new BindErrorHistory()
                        {
                            No = (no++).ToString(),
                            DateTime = values[0],
                            Type = values[1],
                            Content = values[2]
                        };

                        // リストに格納
                        _errorHistoryList.Add(b);
                    }
                }


            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{GetType().Name}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }

    }


    /// <summary>
    /// エラー履歴 バインド
    /// </summary>
    public class BindErrorHistory : INotifyPropertyChanged
    {

        private string _no;
        /// <summary>
        /// No
        /// </summary>
        public string No
        {
            get { return _no; }
            set
            {
                if (_no != value)
                {
                    _no = value;
                    OnPropertyChanged(nameof(No));
                }
            }
        }

        private string _dateTime;
        /// <summary>
        /// 日時
        /// </summary>
        public string DateTime
        {
            get { return _dateTime; }
            set
            {
                if (_dateTime != value)
                {
                    _dateTime = value;
                    OnPropertyChanged(nameof(DateTime));
                }
            }
        }

        private string _type;
        /// <summary>
        /// 種別
        /// </summary>
        public string Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

        private string _content;
        /// <summary>
        /// エラー内容
        /// </summary>
        public string Content
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged(nameof(Content));
                }
            }
        }


        // ListViewへバインディングしたデータの値が変更されたときに、
        // ListViewに変更を通知して反映させる仕組み。
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }



    /// <summary>
    /// エラー種別
    /// </summary>
    public enum ErrorType 
    {
        /// <summary> アプリケーション </summary>
        APPLICATION = 0,
        /// <summary> アイル </summary>
        PLC = 1,
        /// <summary> サーバー </summary>
        SERVER = 2,
    }

}
