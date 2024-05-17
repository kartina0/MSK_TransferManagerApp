using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

using DL_Logger;
using ErrorCodeDefine;
using ServerModule;
using ShareResource;


namespace TransferManagerApp_Debug
{
    /// <summary>
    /// Debug_windowMasterFile.xaml の相互作用ロジック
    /// </summary>
    public partial class Debug_windowMasterFileManualTool : Window
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "windowMasterFileManualTool";


        /// <summary>
        /// ウィンドウ表示中フラグ
        /// </summary>
        public bool isShowing = false;
        /// <summary>
        /// 画面更新タイマー
        /// </summary>
        private DispatcherTimer _tmrUpdateDisplay = null;
        /// <summary>
        /// タイマー重複ロック
        /// </summary>
        private bool _timeLock = false;

        /// <summary>
        /// 動的コントロール生成用クラス
        /// 商品マスターリスト
        /// </summary>
        private List<ControlMasterWork> _controlMasterWorkList = null;

        /// <summary>
        /// デバッグ
        /// 商品マスター CSV ローカル ファイルパス
        /// </summary>
        public static string MasterWorkDebugFilePath = "..\\Compiled\\Master\\Debug\\SHOHIN";
        /// <summary>
        /// デバッグ
        /// 店マスター CSV ローカル ファイルパス
        /// </summary>
        public static string MasterStoreDebugFilePath = "..\\Compiled\\Master\\Debug\\STORE";
        /// <summary>
        /// デバッグ
        /// 作業者マスター CSV ローカル ファイルパス
        /// </summary>
        public static string MasterWorkerDebugFilePath = "..\\Compiled\\Master\\Debug\\WORKER";



        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Debug_windowMasterFileManualTool()
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
                if (DesignerProperties.GetIsInDesignMode(this))
                    return;

                // ウィンドウ表示中
                isShowing = true;
                // タイトルバーを消しても画面移動可能にする処理
                this.MouseLeftButtonDown += delegate { DragMove(); };


                _controlMasterWorkList = new List<ControlMasterWork>();
                rc = Add();




                // 画面更新タイマー初期化
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                if (_tmrUpdateDisplay != null)
                    _tmrUpdateDisplay.Stop();
                _tmrUpdateDisplay = null;


                // ウィンドウ表示終了
                isShowing = false;
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
        }
        /// <summary>
        /// ボタン クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UInt32 rc = 0;
            Button ctrl = (Button)sender;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {ctrl.Name}");
            try
            {
                if (ctrl == btnWrite)
                {
                    rc = OutputToFile();
                }
                else if (ctrl == btnAdd)
                {
                    rc = Add();
                }
                else if (ctrl == btnDelete)
                {
                    rc = Delete();
                }
                else if (ctrl == btnClose)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
        }
        /// <summary>
        /// ファイル出力
        /// </summary>
        /// <returns></returns>
        private UInt32 OutputToFile()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                List<string> workList = new List<string>();





                //rc = Resource.Server.MasterFile.Debug_WriteMasterWork();

                // 商品テーブル作成
                ControlMasterWork work = new ControlMasterWork();
                _controlMasterWorkList.Add(work);
                panelTable.Children.Add(_controlMasterWorkList[_controlMasterWorkList.Count - 1].panel);

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
        /// 追加
        /// </summary>
        /// <returns></returns>
        private UInt32 Add()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 商品テーブル作成
                ControlMasterWork work = new ControlMasterWork();
                _controlMasterWorkList.Add(work);
                panelTable.Children.Add(_controlMasterWorkList[_controlMasterWorkList.Count - 1].panel);

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
        /// 削除
        /// </summary>
        /// <returns></returns>
        private UInt32 Delete()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 商品テーブル削除
                panelTable.Children.Remove(_controlMasterWorkList[_controlMasterWorkList.Count - 1].panel);
                _controlMasterWorkList.Remove(_controlMasterWorkList[_controlMasterWorkList.Count - 1]);

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
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
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



            }
            catch (Exception ex)
            {
                Resource.ErrorHandler(ex);
            }
            finally
            {
                _timeLock = false;
            }
        }




        /// <summary>
        /// Check Error State
        /// </summary>
        private static bool STATUS_SUCCESS(UInt32 err)
        {
            return err == (int)ErrorCodeList.STATUS_SUCCESS;
        }

    }




    /// <summary>
    /// コントロール動的作成用クラス
    /// テーブル1行分のコントロール
    /// </summary>
    public class ControlMasterWork 
    {
        /// <summary>
        /// バインド用オブジェクト
        /// 各TextBoxにバインド
        /// </summary>
        private BindObject _bindObject { get; set; } = new BindObject();

        /// <summary>
        /// [コントロール] StackPanel
        /// </summary>
        public StackPanel panel = new StackPanel();
        /// <summary>
        /// [コントロール] 商品リスト
        /// </summary>
        public TextBox[] controlTextWorks = null;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ControlMasterWork() 
        {
            // ----------------------------------
            // StackPanel
            // ----------------------------------
            panel.Orientation = Orientation.Horizontal;



            // ----------------------------------
            // TextBox配列
            // ----------------------------------
            controlTextWorks = new TextBox[10];
            for (int i = 0; i < controlTextWorks.Length; i++) 
            {
                controlTextWorks[i] = new TextBox();

                controlTextWorks[i].Height = 30;
                controlTextWorks[i].FontSize = 16;
                controlTextWorks[i].BorderBrush = System.Windows.Media.Brushes.Black;
                controlTextWorks[i].BorderThickness = new Thickness(0.2);
                controlTextWorks[i].VerticalAlignment = VerticalAlignment.Center;
                controlTextWorks[i].VerticalContentAlignment = VerticalAlignment.Center;
                controlTextWorks[i].HorizontalContentAlignment = HorizontalAlignment.Center;
                panel.Children.Add(controlTextWorks[i]);
            }

            // 納品日
            controlTextWorks[0].Width = 120;
            // TextBoxにバインド
            Binding bindOrderDate = new Binding("OrderDate");
            bindOrderDate.Source = _bindObject;
            bindOrderDate.Mode = BindingMode.TwoWay;
            bindOrderDate.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            controlTextWorks[0].SetBinding(TextBox.TextProperty, bindOrderDate);

            // 取引先コード
            controlTextWorks[1].Width = 120;
            // TextBoxにバインド
            Binding bindSupplierCode = new Binding("SupplierCode");
            bindSupplierCode.Source = _bindObject;
            bindSupplierCode.Mode = BindingMode.TwoWay;
            bindSupplierCode.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            controlTextWorks[1].SetBinding(TextBox.TextProperty, bindSupplierCode);

            // 取引先名(漢字)
            controlTextWorks[2].Width = 700;
            // TextBoxにバインド
            Binding bindSupplierName = new Binding("SupplierName");
            bindSupplierName.Source = _bindObject;
            bindSupplierName.Mode = BindingMode.TwoWay;
            bindSupplierName.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            controlTextWorks[2].SetBinding(TextBox.TextProperty, bindSupplierName);

            // 商品コード
            controlTextWorks[3].Width = 120;
            // TextBoxにバインド
            Binding bindWorkCode = new Binding("WorkCode");
            bindWorkCode.Source = _bindObject;
            bindWorkCode.Mode = BindingMode.TwoWay;
            bindWorkCode.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            controlTextWorks[3].SetBinding(TextBox.TextProperty, bindWorkCode);

            // 便
            controlTextWorks[4].Width = 50;
            // TextBoxにバインド
            Binding bindPost = new Binding("Post");
            bindPost.Source = _bindObject;
            bindPost.Mode = BindingMode.TwoWay;
            bindPost.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            controlTextWorks[4].SetBinding(TextBox.TextProperty, bindPost);

            // JANコード
            controlTextWorks[5].Width = 120;
            // TextBoxにバインド
            Binding bindJANCode = new Binding("JANCode");
            bindJANCode.Source = _bindObject;
            bindJANCode.Mode = BindingMode.TwoWay;
            bindJANCode.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            controlTextWorks[5].SetBinding(TextBox.TextProperty, bindJANCode);

            // 商品名(漢字)
            controlTextWorks[6].Width = 700;
            // TextBoxにバインド
            Binding bindWorkName = new Binding("WorkName");
            bindWorkName.Source = _bindObject;
            bindWorkName.Mode = BindingMode.TwoWay;
            bindWorkName.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            controlTextWorks[6].SetBinding(TextBox.TextProperty, bindWorkName);

            // パック入数
            controlTextWorks[7].Width = 100;
            // TextBoxにバインド
            Binding bindPackCount = new Binding("PackCount");
            bindPackCount.Source = _bindObject;
            bindPackCount.Mode = BindingMode.TwoWay;
            bindPackCount.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            controlTextWorks[7].SetBinding(TextBox.TextProperty, bindPackCount);

            // 商品名(ｶﾅ)
            controlTextWorks[8].Width = 700;
            // TextBoxにバインド
            Binding bindWorkNameKana = new Binding("WorkNameKana");
            bindWorkNameKana.Source = _bindObject;
            bindWorkNameKana.Mode = BindingMode.TwoWay;
            bindWorkNameKana.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            controlTextWorks[8].SetBinding(TextBox.TextProperty, bindWorkNameKana);

        }

    }


    /// <summary>
    /// バインド用クラス
    /// ※テキストボックスへの入力制限処理をするためにバインド
    /// </summary>
    public class BindObject : INotifyPropertyChanged
    {
        private string _orderDate;
        /// <summary>
        /// 納品日
        /// </summary>
        public string OrderDate
        {
            get { return _orderDate; }
            set
            {
                bool ok = Validation_Int(ref value, 8);
                if (ok)
                {
                    if (_orderDate != value)
                    {
                        _orderDate = value;
                        OnPropertyChanged(nameof(OrderDate));
                    }
                }
            }
        }

        private string _supplierCode;
        /// <summary>
        /// 取引先コード
        /// </summary>
        public string SupplierCode
        {
            get { return _supplierCode; }
            set
            {
                bool ok = Validation_Int(ref value, 6);
                if (ok)
                {
                    if (_supplierCode != value)
                    {
                        _supplierCode = value;
                        OnPropertyChanged(nameof(SupplierCode));
                    }
                }
            }
        }

        private string _supplierName;
        /// <summary>
        /// 取引先名(漢字)
        /// </summary>
        public string SupplierName
        {
            get { return _supplierName; }
            set
            {
                bool ok = Validation_String(ref value, 50);
                if (ok)
                {
                    if (_supplierName != value)
                    {
                        _supplierName = value;
                        OnPropertyChanged(nameof(SupplierName));
                    }
                }
            }
        }

        private string _workCode;
        /// <summary>
        /// 商品コード
        /// </summary>
        public string WorkCode
        {
            get { return _workCode; }
            set
            {
                bool ok = Validation_Int(ref value, 6);
                if (ok)
                {
                    if (_workCode != value)
                    {
                        _workCode = value;
                        OnPropertyChanged(nameof(WorkCode));
                    }
                }
            }
        }

        private string _post;
        /// <summary>
        /// 便
        /// </summary>
        public string Post
        {
            get { return _post; }
            set
            {
                bool ok = Validation_Int(ref value, 1);
                if (ok)
                {
                    if (_post != value)
                    {
                        _post = value;
                        OnPropertyChanged(nameof(Post));
                    }
                }
            }
        }

        private string _janCode;
        /// <summary>
        /// JANコード
        /// </summary>
        public string JANCode
        {
            get { return _janCode; }
            set
            {
                bool ok = Validation_Int(ref value, 13);
                if (ok)
                {
                    if (_janCode != value)
                    {
                        _janCode = value;
                        OnPropertyChanged(nameof(JANCode));
                    }
                }
            }
        }

        private string _workName;
        /// <summary>
        /// 商品名(漢字)
        /// </summary>
        public string WorkName
        {
            get { return _workName; }
            set
            {
                bool ok = Validation_String(ref value, 44);
                if (ok)
                {
                    if (_workName != value)
                    {
                        _workName = value;
                        OnPropertyChanged(nameof(WorkName));
                    }
                }
            }
        }

        private string _packCount;
        /// <summary>
        /// パック入数
        /// </summary>
        public string PackCount
        {
            get { return _packCount; }
            set
            {
                bool ok = Validation_Int(ref value, 10);
                if (ok)
                {
                    if (_packCount != value)
                    {
                        _packCount = value;
                        OnPropertyChanged(nameof(PackCount));
                    }
                }
            }
        }

        private string _workNameKana;
        /// <summary>
        /// 商品名(ｶﾅ)
        /// </summary>
        public string WorkNameKana
        {
            get { return _workNameKana; }
            set
            {
                bool ok = Validation_Int(ref value, 44);
                if (ok)
                {
                    if (_workNameKana != value)
                    {
                        _workNameKana = value;
                        OnPropertyChanged(nameof(WorkNameKana));
                    }
                }
            }
        }



        /// <summary>
        /// 入力制限
        /// 数値チェック && 文字数チェック
        /// </summary>
        /// <returns></returns>
        public bool Validation_Int(ref string inputStr, int count)
        {
            bool ok = true;
            try
            {
                // 空だったら0としておく
                if (inputStr.Length <= 0)
                {
                    inputStr = "0";
                }
                else
                {
                    // 数字チェック
                    if (!int.TryParse(inputStr, out int val)) ok = false;
                    // 最大文字数チェック
                    if (!(inputStr.Length <= count)) ok = false;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"Validation_Int ： {inputStr},{count} , {ex.Message}");
                ok = false;
            }
            return ok;
        }
        /// <summary>
        /// 入力制限
        /// stringのバイト数チェック
        /// </summary>
        /// <returns></returns>
        public bool Validation_String(ref string inputStr, int count)
        {
            bool ok = true;
            try
            {
                int byteCount = Encoding.GetEncoding("shift_jis").GetByteCount(inputStr);

                // 最大文字数チェック
                if (!(byteCount <= count)) ok = false;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"Validation_String ： {inputStr},{count} , {ex.Message}");
                ok = false;
            }
            return ok;
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
