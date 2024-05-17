//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using SystemConfig;
using DL_Logger;
using ErrorCodeDefine;
using ShareResource;
using System.Linq;
using ServerModule;

namespace TransferManagerApp
{
    /// <summary>
    /// ucTab_ProgressMonitor.xaml の相互作用ロジック
    /// </summary>
    public partial class ucTab_ProgressMonitor : UserControl, IDisposable
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "ucTab_ProgressMonitor";

        /// <summary>
        /// 仕分進捗情報リスト(全体)
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindAisleProgress> _aisleProgressTotalList = new ObservableCollection<BindAisleProgress>();
        /// <summary>
        /// 仕分進捗情報リスト(アイルごと)
        /// 表にバインディング
        /// </summary>
        private ObservableCollection<BindAisleProgress> _aisleProgressList = new ObservableCollection<BindAisleProgress>();

        /// <summary>
        /// 画面更新タイマー
        /// </summary>
        private DispatcherTimer _tmrUpdateDisplay = null;
        /// <summary>
        /// タイマー重複ロック
        /// </summary>
        private bool _timeLock = false;

        /// <summary>
        /// 便No
        /// </summary>
        public int postIndex = 0;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ucTab_ProgressMonitor()
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

                // ListView初期化
                rc = InitListView();

                // 便Noコンボボックス
                comboPostNo.SelectedIndex = Resource.SystemStatus.CurrentPostIndex;

                // 画面更新タイマー初期化
                rc = InitTimer();
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Resource.ErrorHandler(rc, true);
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
                if (_tmrUpdateDisplay != null)
                    _tmrUpdateDisplay.Stop();
                _tmrUpdateDisplay = null;
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, false);
            }
        }
        /// <summary>
        /// 便Noコンボボックス 選択イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboPostNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                postIndex = comboPostNo.SelectedIndex;
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
        }

        /// <summary>
        /// ListView初期化
        /// </summary>
        /// <returns></returns>
        private UInt32 InitListView()
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 表へのデータバインド処理
                _aisleProgressTotalList.Add(new BindAisleProgress
                {
                    AisleNo = "全体",
                    OrderCount = 0,
                    OrderCompCount = 0,
                    OrderRemainCount = 0,
                    OrderProgress = 0,
                    StartTime = "00:00:00",
                    ElapsedTime = "00:00:00",
                    Status = "",
                });
                dataGridOrderProgressTotal.ItemsSource = _aisleProgressTotalList;

                // 表の各カラムのWidthを調整
                double w = dataGridOrderProgressTotal.ActualWidth;
                columnAisleNoTotal.Width = Math.Floor(w * 0.08);
                columnOrderCountTotal.Width = Math.Floor(w * 0.08);
                columnOrderCompCountTotal.Width = Math.Floor(w * 0.08);
                columnOrderRemainCountTotal.Width = Math.Floor(w * 0.08);
                columnOrderProgressTotal.Width = Math.Floor(w * 0.3);
                columnStartTimeTotal.Width = Math.Floor(w * 0.13);
                columnElapsedTimeTotal.Width = Math.Floor(w * 0.13);
                columnStatusTotal.Width = Math.Floor(w * 0.09);




                //listviewOrderProgressTotal.ItemsSource = _aisleProgressTotalList;

                //// 表の各カラムのWidthを調整
                //double w = listviewOrderProgressTotal.ActualWidth;
                //columnAisleNoTotal.Width = Math.Floor(w * 0.04);
                //columnOrderCountTotal.Width = Math.Floor(w * 0.1);
                //columnOrderCompCountTotal.Width = Math.Floor(w * 0.1);
                //columnOrderRemainCountTotal.Width = Math.Floor(w * 0.1);
                //columnOrderProgressTotal.Width = Math.Floor(w * 0.3);
                //columnStartTimeTotal.Width = Math.Floor(w * 0.13);
                //columnElapsedTimeTotal.Width = Math.Floor(w * 0.13);
                //columnStatusTotal.Width = Math.Floor(w * 0.09);



                // 表へのデータバインド処理
                for (int i = 0; i < Const.MaxAisleCount; i++)
                {
                    for (int j = 0; j < Const.MaxBatchCount; j++) 
                    {
                        _aisleProgressList.Add(new BindAisleProgress
                        {
                            AisleNo = $"{i + 1}",
                            BatchNo = $"{j + 1}",
                            OrderCount = 0,
                            OrderCompCount = 0,
                            OrderRemainCount = 0,
                            OrderProgress = 0,
                            StartTime = "00:00:00",
                            ElapsedTime = "00:00:00",
                            Status = "",
                        });
                    }
                }
                dataGridOrderProgress.ItemsSource = _aisleProgressList;

                // 表の各カラムのWidthを調整
                w = dataGridOrderProgress.ActualWidth;
                columnAisleNo.Width = Math.Floor(w * 0.04);
                columnBatchNo.Width = Math.Floor(w * 0.04);
                columnOrderCount.Width = Math.Floor(w * 0.08);
                columnOrderCompCount.Width = Math.Floor(w * 0.08);
                columnOrderRemainCount.Width = Math.Floor(w * 0.08);
                columnOrderProgress.Width = Math.Floor(w * 0.3);
                columnStartTime.Width = Math.Floor(w * 0.13);
                columnElapsedTime.Width = Math.Floor(w * 0.13);
                columnStatus.Width = Math.Floor(w * 0.09);


                //for (int i = 0; i < Const.MaxAisleCount; i++) 
                //{
                //    // 表へのデータバインド処理
                //    _aisleProgressList.Add(new BindAisleProgress
                //    {
                //        AisleNo = $"{i + 1}",

                //        OrderCount = 0,
                //        OrderCompCount = 0,
                //        OrderRemainCount = 0,
                //        OrderProgress = 0,
                //        StartTime = "00:00:00",
                //        ElapsedTime = "00:00:00",
                //        Status = "",
                //    });
                //}
                //listviewOrderProgress.ItemsSource = _aisleProgressList;

                //// 表の各カラムのWidthを調整
                //w = listviewOrderProgress.ActualWidth;
                //columnAisleNo.Width = Math.Floor(w * 0.04);
                //columnOrderCount.Width = Math.Floor(w * 0.1);
                //columnOrderCompCount.Width = Math.Floor(w * 0.1);
                //columnOrderRemainCount.Width = Math.Floor(w * 0.1);
                //columnOrderProgress.Width = Math.Floor(w * 0.3);
                //columnStartTime.Width = Math.Floor(w * 0.13);
                //columnElapsedTime.Width = Math.Floor(w * 0.13);
                //columnStatus.Width = Math.Floor(w * 0.09);


            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }

        /// <summary>
        /// 画面更新タイマー初期化
        /// </summary>
        private UInt32 InitTimer()
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
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
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
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


                // ---------------------------------------
                // 全体
                int orderCountTotal = (int)Resource.Server.OrderInfo.OrderDataList[postIndex].Sum(x => x.orderCountTotal);
                int orderCompCountTotal = (int)Resource.Server.OrderInfo.ExecuteDataList[postIndex].Sum(x => x.orderCompCountTotal);
                int orderRemainCountTotal = orderCountTotal - orderCompCountTotal;
                int progressPerTotal = 0;
                if (orderCountTotal == 0) progressPerTotal = 0;
                else progressPerTotal = (int)((double)(orderCompCountTotal * 100) / (double)orderCountTotal);
                _aisleProgressTotalList[0].OrderCount = orderCountTotal;
                _aisleProgressTotalList[0].OrderCompCount = orderCompCountTotal;
                _aisleProgressTotalList[0].OrderRemainCount = orderRemainCountTotal;
                _aisleProgressTotalList[0].OrderProgress = progressPerTotal;

                DataGridRow row = (DataGridRow)dataGridOrderProgressTotal.ItemContainerGenerator.ContainerFromIndex(0);
                if (row != null)
                    row.Background = Brushes.AliceBlue;


                // ---------------------------------------
                // アイルごと
                for (int i = 0; i < Const.MaxAisleCount; i++)
                {
                    if (IniFile.AisleEnable[i])
                    {// 有効アイル

                        // ※バッチ情報によるマテハン向けのアイル変換を忘れずに

                        // ---------------------------------------
                        // バッチごと
                        for (int j = 0; j < Const.MaxBatchCount; j++) 
                        {
                            int orderCount = 0;
                            int orderCompCount = 0;
                            string[] currentbatchArray = Resource.batch.BatchInfoCurrent.Post[postIndex].Aisle[i].Batch[j].OutputToArray(IniFile.UnitEnable[i]);

                            // 仕分数
                            foreach (OrderData data in Resource.Server.OrderInfo.OrderDataList[postIndex]) 
                            {
                                // 店別小仕分けデータから、バッチの店コードに該当する箇所だけカウントしていく
                                foreach (string storeCode in currentbatchArray) 
                                {
                                    if (storeCode == null)
                                        continue;
                                    orderCount += (int)(data.storeDataList.Where(x => x.storeCode == storeCode).Sum(x => x.orderCount));
                                }
                            }
                            // 仕分完了数
                            foreach (ExecuteData data in Resource.Server.OrderInfo.ExecuteDataList[postIndex]) 
                            {
                                // 店別小仕分けデータから、バッチの店コードに該当する箇所だけカウントしていく
                                foreach (string storeCode in currentbatchArray)
                                {
                                    if (storeCode == null)
                                        continue;
                                    orderCompCount += (int)(data.storeDataList.Where(x => x.storeCode == storeCode).Sum(x => x.orderCompCount));
                                }
                            }

                            int orderRemainCount = orderCount - orderCompCount;
                            int progressPer = 0;
                            if (orderCount == 0) progressPer = 0;
                            else progressPer = (int)((double)(orderCompCount * 100) / (double)orderCount);


                            _aisleProgressList[i*Const.MaxBatchCount + j].OrderCount = orderCount;
                            _aisleProgressList[i*Const.MaxBatchCount + j].OrderCompCount = orderCompCount;
                            _aisleProgressList[i*Const.MaxBatchCount + j].OrderRemainCount = orderRemainCount;
                            _aisleProgressList[i*Const.MaxBatchCount + j].OrderProgress = progressPer;

                            _aisleProgressList[i].IsVisible = true;
                        }

                        ChangeRowColor(dataGridOrderProgress, i, System.Windows.Media.Brushes.AliceBlue);
                    }
                    else 
                    {// 無効アイル
                        ChangeRowColor(dataGridOrderProgress, i, System.Windows.Media.Brushes.Gray);
                        _aisleProgressList[i].IsVisible = false;
                    }
                }


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
        /// 表の指定行の色を変更
        /// </summary>
        private UInt32 ChangeRowColor(DataGrid dataGrid, int aisleIndex, SolidColorBrush color)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                if (dataGrid.Items.Count > 0)
                {
                    //row = (ListViewItem)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
                    //if (row != null)
                    //{
                    //    row.Background = color;
                    //}


                    // バッチ数分の行数を色変更
                    for (int batchIndex = 0; batchIndex < Const.MaxBatchCount; batchIndex++) 
                    {
                        DataGridRow row = (DataGridRow)dataGridOrderProgress.ItemContainerGenerator.ContainerFromIndex(aisleIndex * Const.MaxBatchCount + batchIndex);
                        if (row != null)
                        {
                            row.Background = color;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        ///// <summary>
        ///// 表の指定行の色を変更
        ///// </summary>
        //private UInt32 ChangeRowColor(ListView listView, int rowIndex, SolidColorBrush color)
        //{
        //    UInt32 rc = 0;
        //    //Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
        //    try
        //    {
        //        if (listView.Items.Count > 0)
        //        {
        //            ListViewItem row = (ListViewItem)listView.ItemContainerGenerator.ContainerFromIndex(rowIndex);
        //            if (row != null)
        //            {
        //                row.Background = color;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        rc = (Int32)ErrorCodeList.EXCEPTION;
        //        Resource.ErrorHandler(ex);
        //    }
        //    //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
        //    return rc;
        //}


        /// <summary>
        /// Check Error State
        /// </summary>
        private static bool STATUS_SUCCESS(UInt32 err)
        {
            return err == (int)ErrorCodeList.STATUS_SUCCESS;
        }


    }



    /// <summary>
    /// エントリーワーク情報
    /// 画面の表にデータバインドするようのクラス
    /// </summary>
    public class BindAisleProgress : INotifyPropertyChanged
    {

        private bool _isVisible;
        /// <summary>
        /// データ行の表示非表示
        /// ※今回はListViewを5行固定で表示させるため、空のデータ行は非表示にしたい。
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged(nameof(IsVisible));
                }
            }
        }

        private string _aisleNo;
        /// <summary>
        /// アイルNo
        /// </summary>
        public string AisleNo
        {
            get { return _aisleNo; }
            set
            {
                if (_aisleNo != value)
                {
                    _aisleNo = value;
                    OnPropertyChanged(nameof(AisleNo));
                }
            }
        }

        private string _batchNo;
        /// <summary>
        /// バッチNo
        /// </summary>
        public string BatchNo
        {
            get { return _batchNo; }
            set
            {
                if (_batchNo != value)
                {
                    _batchNo = value;
                    OnPropertyChanged(nameof(BatchNo));
                }
            }
        }

        private double _orderCount;
        /// <summary>
        /// 仕分予定数
        /// </summary>
        public double OrderCount
        {
            get { return _orderCount; }
            set
            {
                if (_orderCount != value)
                {
                    _orderCount = value;
                    OnPropertyChanged(nameof(OrderCount));
                }
            }
        }

        private double _orderCompCount;
        /// <summary>
        /// 仕分完了数
        /// </summary>
        public double OrderCompCount
        {
            get { return _orderCompCount; }
            set
            {
                if (_orderCompCount != value)
                {
                    _orderCompCount = value;
                    OnPropertyChanged(nameof(OrderCompCount));
                }
            }
        }

        private double _orderRemainCount;
        /// <summary>
        /// 残り数
        /// </summary>
        public double OrderRemainCount
        {
            get { return _orderRemainCount; }
            set
            {
                if (_orderRemainCount != value)
                {
                    _orderRemainCount = value;
                    OnPropertyChanged(nameof(OrderRemainCount));
                }
            }
        }

        private double _orderProgress;
        /// <summary>
        /// 進捗 (％)
        /// </summary>
        public double OrderProgress
        {
            get { return _orderProgress; }
            set
            {
                if (_orderProgress != value)
                {
                    _orderProgress = value;
                    OnPropertyChanged(nameof(OrderProgress));
                }
            }
        }

        private string _startTime;
        /// <summary>
        /// 開始時刻
        /// </summary>
        public string StartTime
        {
            get { return _startTime; }
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    OnPropertyChanged(nameof(StartTime));
                }
            }
        }

        private string _elapsedTime;
        /// <summary>
        /// 経過時間
        /// </summary>
        public string ElapsedTime
        {
            get { return _elapsedTime; }
            set
            {
                if (_elapsedTime != value)
                {
                    _elapsedTime = value;
                    OnPropertyChanged(nameof(ElapsedTime));
                }
            }
        }

        private string _status;
        /// <summary>
        /// ステータス
        /// </summary>
        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
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

}
