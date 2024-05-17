//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using DL_CommonLibrary;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using SystemConfig;
using DL_Logger;
using ErrorCodeDefine;
using ServerModule;
using ShareResource;
using TransferManagerApp_Debug;


namespace TransferManagerApp
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class windowOpening : Window
    {
        private const string THIS_NAME = "windowOpening";

        /// <summary>
        /// 画面更新タイマー
        /// </summary>
        private DispatcherTimer _tmrUpdateDisplay = null;
        /// <summary>
        /// オープニングステップ
        /// </summary>
        private OpeningSteps _step = OpeningSteps.LOADING;
        /// <summary>
        /// オープニング処理 中止フラグ
        /// </summary>
        private bool _cancel = false;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public windowOpening()
        {
            InitializeComponent();
        }
        /// <summary>
        /// ウィンドウロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                lblAppTitle.Content = Const.Title;

                var assembly = Assembly.GetExecutingAssembly().GetName();
                var version = assembly.Version;
                lblVersion.Content = version.ToString();

                // オープニング処理
                Task.Run(OpeningSequence);
                // 画面更新タイマー起動
                rc = InitTimer();

            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Cancel();
            }
        }
        /// <summary>
        /// ウィンドウクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                if (_tmrUpdateDisplay != null)
                    _tmrUpdateDisplay.Stop();
                _tmrUpdateDisplay = null;
            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
        }



        /// <summary>
        /// オープニング処理 中止
        /// </summary>
        private void Cancel()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 画面更新タイマー終了
                if (_tmrUpdateDisplay != null)
                    _tmrUpdateDisplay.Stop();
                _tmrUpdateDisplay = null;

                // Cycleスレッド終了
                Cycle.Close();
                // Serverスレッド終了
                ServerControl.Close();

                // Manager終了
                Manager.Close();
                // Resource 終了
                Resource.Close();
                // Logger 終了
                Logger.Close();
            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                // ウィンドウクローズ
                this.DialogResult = false;
                this.Close();
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
        }


        /// <summary>
        /// オープニング処理
        /// </summary>
        private void OpeningSequence()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            _cancel = false;
            try
            {
                _step = OpeningSteps.LOADING;

                // パス設定
                Resource.AppPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                //Resource.AppPath = Application.StartupPath;
                //Resource.OwnerForm = this;

                // デモ版なのか確認する
                //自分自身のバージョン情報を取得する
                System.Diagnostics.FileVersionInfo ver =
                    System.Diagnostics.FileVersionInfo.GetVersionInfo(
                    System.Reflection.Assembly.GetExecutingAssembly().Location);


                // ------------------------------
                // 二重起動をチェックする
                // ------------------------------
                if (System.Diagnostics.Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName).Length > 1)
                {
                    //Dialogs.ShowInformationMessage(this, "既にプログラムが起動しています.", Const.Title, SystemIcons.Error);
                    MessageBox.Show("既にプログラムが起動しています", Const.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    rc = (UInt32)ErrorCodeList.APP_ALREADY_STARTED;
                }


                //// ------------------------------
                //// Windows設定
                //// ------------------------------
                //// Windows Sleep抑止
                //WindowsControl.WindowsSleep(false);
                //// Windowsシャットダウン抑止メッセージ登録
                //var helper = new System.Windows.Interop.WindowInteropHelper(_window);
                //WindowsControl.DisablePowerSwitch(helper.Handle, "シャットダウンできません");


                // ------------------------------
                // Logger起動
                // ------------------------------
                if (STATUS_SUCCESS(rc))
                {
                    rc = Logger.Open(IniFile.LogDir, Const.Title, Const.LogMaxFileCount, Const.LogMaxLineCount);
                    if (STATUS_SUCCESS(rc))
                        Logger.WriteLog(LogType.INFO, "Logger起動");
                    else
                        Logger.WriteLog(LogType.ERROR, "Logger起動エラー");
                }


                // ------------------------------
                // 前回アプリ終了時のステータスを取得
                // ------------------------------
                if (STATUS_SUCCESS(rc))
                {
                    string preStatusFileName = System.IO.Path.Combine(Const.IniDir, Const.PreStatusFileName);
                    rc = PreStatus.Load(preStatusFileName);
                    if (STATUS_SUCCESS(rc))
                    {
                        Resource.SystemStatus.CurrentPostIndex = PreStatus.PostIndex;
                        Logger.WriteLog(LogType.INFO, "前回アプリ終了時ステータス取得 完了");
                    }
                    else 
                    {
                        Logger.WriteLog(LogType.ERROR, "前回アプリ終了時ステータス取得 エラー");
                    }
                }


                // ------------------------------
                // Resource起動
                // ------------------------------
                if (STATUS_SUCCESS(rc))
                {
                    rc = Resource.Open();
                    if (STATUS_SUCCESS(rc))
                        Logger.WriteLog(LogType.INFO, "Resource起動");
                    else
                        Logger.WriteLog(LogType.ERROR, "Resource起動エラー");
                }


                //// ------------------------------
                //// 前回アプリ終了時の情報を取得
                //// ------------------------------
                ////Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ////config.AppSettings.Settings[key].Value = value;
                ////config.Save(ConfigurationSaveMode.Modified);
                ////ConfigurationManager.RefreshSection("appSettings");
                //if (int.TryParse(ConfigurationManager.AppSettings["PostIndex"], out int postIndex)) 
                //    Resource.SystemStatus.CurrentPostIndex = postIndex;


                // ------------------------------
                // PC情報取得
                // ------------------------------
                if (STATUS_SUCCESS(rc))
                {
                    Resource.SystemStatus.MyPcName = Environment.MachineName;
                    Resource.SystemStatus.MyIpAddress = NetMisc.GetIPAddress();
                    Logger.WriteLog(LogType.SYSTEM, "***********************************");
                    Logger.WriteLog(LogType.SYSTEM, string.Format("起動中 PC名={0} , IP={1}", Resource.SystemStatus.MyPcName, Resource.SystemStatus.MyIpAddress));
                    Logger.WriteLog(LogType.SYSTEM, string.Format("OS Ver={0} ", Environment.OSVersion));

                    Logger.WriteLog(LogType.SYSTEM, string.Format("{0} Ver={1} ", ver.ProductName, ver.FileVersion));
                    Logger.WriteLog(LogType.SYSTEM, "***********************************");
                }


                // ------------------------------
                // 各種ログ出力
                // ------------------------------


                // ------------------------------
                // Manager起動
                // ------------------------------
                if (STATUS_SUCCESS(rc))
                {
                    rc = Manager.Start();
                    if (STATUS_SUCCESS(rc))
                        Logger.WriteLog(LogType.INFO, "Manager起動");
                    else
                        Logger.WriteLog(LogType.ERROR, "Manager起動エラー");
                }


                // ------------------------------
                // BatchFile読み込み
                // ------------------------------
                if (STATUS_SUCCESS(rc))
                {
                    if (Resource.batch.isExistFile)
                    {
                        rc = Resource.batch.Load();
                    }
                    else
                    {
                        Resource.ErrorHandler("バッチファイルが見つかりませんでした");
                    }

                    if (STATUS_SUCCESS(rc))
                        Logger.WriteLog(LogType.INFO, "BatchFile読み込み完了");
                    else
                        Logger.WriteLog(LogType.ERROR, "BatchFile読み込みエラー");
                }


                // ------------------------------
                // マスターファイル読み出し
                // ------------------------------
                if (STATUS_SUCCESS(rc))
                {
                    // 商品マスタ
                    // サーバーからローカルにコピー
                    bool isTodaysFileLocal = false;
                    rc = Resource.Server.MasterFile.CopyMasterWorkFile(out isTodaysFileLocal);
                    if (!STATUS_SUCCESS(rc))
                    {
                        Resource.ErrorHandler(rc);
                        rc = 0;
                    }
                    else if (isTodaysFileLocal)
                    {
                        // 読み出し
                        rc = Resource.Server.MasterFile.ReadMasterWork();
                        if (!STATUS_SUCCESS(rc))
                        {
                            Resource.ErrorHandler(rc);
                            rc = 0;
                        }
                        else
                        {
                            Resource.SystemStatus.IsLoadedTodayMasterWork = true;
                            Logger.WriteLog(LogType.INFO, string.Format("商品マスター読み出し完了"));
                        }
                    }

                    // 店マスタ
                    // サーバーからローカルにコピー
                    isTodaysFileLocal = false;
                    rc = Resource.Server.MasterFile.CopyMasterStoreFile(out isTodaysFileLocal);
                    if (!STATUS_SUCCESS(rc))
                    {
                        Resource.ErrorHandler(rc);
                        rc = 0;
                    }
                    else if (isTodaysFileLocal)
                    {
                        // 読み出し
                        rc = Resource.Server.MasterFile.ReadMasterStore();
                        if (!STATUS_SUCCESS(rc))
                        {
                            Resource.ErrorHandler(rc);
                            rc = 0;
                        }
                        else
                        {
                            Resource.SystemStatus.IsLoadedTodayMasterStore = true;
                            Logger.WriteLog(LogType.INFO, string.Format("店マスター読み出し完了"));
                        }
                    }

                    // 作業者マスタ
                    // サーバーからローカルにコピー
                    isTodaysFileLocal = false;
                    rc = Resource.Server.MasterFile.CopyMasterWorkerFile(out isTodaysFileLocal);
                    if (!STATUS_SUCCESS(rc))
                    {
                        Resource.ErrorHandler(rc);
                        rc = 0;
                    }
                    else if (isTodaysFileLocal)
                    {
                        // 読み出し
                        rc = Resource.Server.MasterFile.ReadMasterWorker();
                        if (!STATUS_SUCCESS(rc))
                        {
                            Resource.ErrorHandler(rc);
                            rc = 0;
                        }
                        else
                        {
                            Resource.SystemStatus.IsLoadedTodayMasterWorker = true;
                            Logger.WriteLog(LogType.INFO, string.Format("作業者マスター読み出し完了"));
                        }
                    }
                }


                // ------------------------------
                // PICKDATA
                // ------------------------------
                if (IniFile.PickDataEnable) 
                {
                    if (STATUS_SUCCESS(rc))
                    {
                        // ------------------------------
                        // PICKDATAフォルダをローカルにZIPバックアップ
                        // ------------------------------
                        rc = Resource.Server.PickData.BackUpPickDataDir();


                        // ------------------------------
                        // PICKDATA読み出し
                        // ------------------------------
                        for (int i = 0; i < Const.MaxPostCount; i++)
                        {
                            // サーバーからローカルにコピー
                            rc = Resource.Server.PickData.CopyPickDataFile(i, out bool isTodaysFileLocal, out bool serverUpdated);
                            if (!STATUS_SUCCESS(rc))
                            {
                                Resource.ErrorHandler(rc);
                                rc = 0;
                            }
                            else if (isTodaysFileLocal)
                            {
                                // PICKDATA 読み出し
                                rc = Resource.Server.PickData.Read(i);
                                if (!STATUS_SUCCESS(rc))
                                {
                                    Resource.ErrorHandler(rc);
                                    string message = $"{i + 1}便のPICKDATAファイル読み出し異常";
                                    Logger.WriteLog(LogType.INFO, message);
                                    rc = 0;
                                }
                                else
                                {
                                    string message = $"{i + 1}便のPICKDATA読み出し完了";
                                    Logger.WriteLog(LogType.INFO, message);
                                }
                            }
                        }
                    }


                    // ------------------------------
                    // DB準備
                    // ------------------------------
                    if (STATUS_SUCCESS(rc)) 
                    {
                        Logger.WriteLog(LogType.INFO, $"DB_IPアドレス : {IniFile.DBIpAddress}");
                        if (IniFile.DBIpAddress == "127.0.0.1") 
                        {// ローカルDBならばPICKDATAを書き込んだりクリアしたりできるようにする
                            Logger.WriteLog(LogType.INFO, string.Format("DBセットアップ"));
                            if (STATUS_SUCCESS(rc))
                            {
                                Debug_DB debugDb = new Debug_DB();

                                // テーブル存在確認
                                if (!debugDb.Debug_CheckTable())
                                {// テーブルなし
                                    Logger.WriteLog(LogType.INFO, string.Format("本日のテーブルなし"));

                                    // DBテーブル作成
                                    rc = debugDb.Debug_CreateTable();
                                    if (STATUS_SUCCESS(rc))
                                        Logger.WriteLog(LogType.INFO, string.Format("本日のテーブル 生成完了"));
                                    else
                                        Logger.WriteLog(LogType.INFO, string.Format("本日のテーブル 生成エラー"));

                                    Thread.Sleep(200);

                                    // DBテーブルにPICKDATAの内容を書き込み
                                    if (STATUS_SUCCESS(rc))
                                    {
                                        for (int i = 0; i < Const.MaxPostCount; i++)
                                            rc = Resource.Server.PickData.WriteDB(i, Resource.Server.MasterFile.MasterStoreList);
                                        if (STATUS_SUCCESS(rc))
                                            Logger.WriteLog(LogType.INFO, string.Format("商品ヘッダテーブル 書き込み完了"));
                                        else
                                            Logger.WriteLog(LogType.INFO, string.Format("商品ヘッダテーブル 書き込みエラー"));
                                    }
                                }
                                else
                                {// テーブルあり
                                    Logger.WriteLog(LogType.INFO, string.Format("既に本日のテーブルあり"));

                                    // 既存のデータをクリアするか確認
                                    MessageBoxResult result = MessageBox.Show("DBをクリアしますか？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                    if (result == MessageBoxResult.Yes)
                                    {
                                        try
                                        {
                                            // 全レコード削除
                                            rc = debugDb.Debug_DeleteRecord();
                                            if (STATUS_SUCCESS(rc))
                                                Logger.WriteLog(LogType.INFO, string.Format("DBレコード削除完了"));
                                            else
                                                Logger.WriteLog(LogType.INFO, string.Format("DBレコード削除エラー"));

                                            // DBテーブルにPICKDATAの内容を書き込み
                                            if (STATUS_SUCCESS(rc))
                                            {
                                                for (int i = 0; i < Const.MaxPostCount; i++)
                                                    rc = Resource.Server.PickData.WriteDB(i, Resource.Server.MasterFile.MasterStoreList);
                                                if (STATUS_SUCCESS(rc))
                                                    Logger.WriteLog(LogType.INFO, string.Format("商品ヘッダテーブル 書き込み完了"));
                                                else
                                                    Logger.WriteLog(LogType.INFO, string.Format("商品ヘッダテーブル 書き込みエラー"));
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }

                }


                // ------------------------------
                // Serverスレッドスタート
                // ------------------------------
                if (STATUS_SUCCESS(rc))
                {
                    ServerControl.Start(Resource.Server);
                    if (STATUS_SUCCESS(rc))
                        Logger.WriteLog(LogType.INFO, "Serverスレッド起動");
                }


                // ------------------------------
                // Serverスレッド初期化完了待ち
                // ------------------------------
                if (STATUS_SUCCESS(rc)) 
                {
                    while (ServerControl.init)
                    {
                        Thread.Sleep(100);
                    }
                    Logger.WriteLog(LogType.INFO, "Serverスレッド初期化完了");
                }


                // ------------------------------
                // Cycleスレッド起動
                // ------------------------------
                if (STATUS_SUCCESS(rc))
                {
                    rc = Cycle.Start();
                    if (STATUS_SUCCESS(rc))
                        Logger.WriteLog(LogType.INFO, "Cycleスレッド起動");
                    else
                        Logger.WriteLog(LogType.ERROR, "Cycleスレッド起動エラー");
                }


                // ------------------------------
                // 起動時に行うファイル削除処理等
                // ------------------------------



            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
            finally 
            {
                if (STATUS_SUCCESS(rc))
                {
                    //progressRingVisibility = false;
                    _step = OpeningSteps.COMP;
                    Resource.SystemStatus.initialize_Completed = true;
                }
                else
                {
                    Resource.ErrorHandler(rc, true);
                    _cancel = !STATUS_SUCCESS(rc);
                }
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
        }

        /// <summary>
        /// 画面更新タイマー初期化
        /// </summary>
        private UInt32 InitTimer()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{GetType().Name} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // タイマのインスタンスを生成
                _tmrUpdateDisplay = new DispatcherTimer();
                // インターバルを設定
                _tmrUpdateDisplay.Interval = TimeSpan.FromMilliseconds(50);
                // タイマメソッドを設定
                _tmrUpdateDisplay.Tick += new EventHandler(tmrUpdateDisplay_tick);
                // タイマを開始
                _tmrUpdateDisplay.Start();
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
                Cancel();
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
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
            try
            {
                // 中止フラグ
                if (_cancel)
                {
                    Cancel();
                    return;
                }

                // オープニング完了
                if (_step == OpeningSteps.COMP)
                {
                    // ローディングCanvas非表示
                    ucWaitingCircle.Visibility = Visibility.Hidden;

                    // ------------------------------
                    // フェードアウト処理
                    // ------------------------------
                    this.Opacity -= 0.1;
                    if (this.Opacity <= 0.1)
                    {
                        // ウィンドウクローズ
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                else 
                {
                    // ローディングCanvas表示
                    ucWaitingCircle.Visibility = Visibility.Visible;
                }

                // ------------------------------
                // メッセージ 表示
                // ------------------------------
                if (_step == OpeningSteps.NOP)
                    lblMessage.Content = "";
                else if (_step == OpeningSteps.LOADING)
                    lblMessage.Content = "起動中";
                else if (_step == OpeningSteps.COMP)
                    lblMessage.Content = "起動完了";

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                _cancel = true;
            }
        }


        private static UInt32 Convert(int postIndex) 
        {
            UInt32 rc = 0;
            try
            {
                foreach (var middle in Resource.Server.PickData.PickDataList[postIndex].middleRecordList)
                {
                    foreach (var minimum in middle.minimumRecordList)
                    {
                        bool ok = false;

                        int stationNo = ((minimum.aisleNo - 1) / 3) + 1;
                        int aisleNo = minimum.aisleNo;
                        int slotNo = minimum.slotNo;
                        MasterStore masterStore = Resource.Server.MasterFile.MasterStoreList.Find(x => x.postInfo[postIndex].station == stationNo &&
                                                                                                        x.postInfo[postIndex].aisle == minimum.aisleNo &&
                                                                                                        x.postInfo[postIndex].slot == minimum.slotNo);
                        string storeCode = masterStore.storeCode;

                        // バッチ情報取得
                        List<string[]> allAislebatchList = new List<string[]>();
                        for (int aisleno = 0; aisleno < Const.MaxAisleCount; aisleno++)
                        {
                            string[] oneAisleSlots = Resource.batch.BatchInfoCurrent.Post[postIndex].Aisle[aisleno].OutputToArray();
                            int index = Array.IndexOf(oneAisleSlots, storeCode);
                            if (index >= 0) 
                            {
                                // バッチ情報に変換
                                minimum.aisleNo = aisleno;
                                minimum.slotNo = index + 1;

                                ok = true;
                                break;
                            }
                        }

                        if (!ok) 
                        {
                            // バッチファイルになかったら消す
                            middle.minimumRecordList.Remove(minimum);
                        }

                    }
                }




            }
            catch (Exception ex)
            {


                throw;
            }

            return rc;
        }






        /// <summary>
        /// Check Error State
        /// </summary>
        private static bool STATUS_SUCCESS(UInt32 err) { return err == (int)ErrorCodeList.STATUS_SUCCESS; }
    }



    /// <summary>
    /// オープニング処理 ステップ
    /// </summary>
    public enum OpeningSteps
    {
        NOP = 0,
        LOADING,
        COMP,
    }
}
