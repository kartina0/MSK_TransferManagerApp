using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using SystemConfig;
using DL_Logger;
using ErrorCodeDefine;
using ShareResource;


namespace TransferManagerApp
{
    /// <summary>
    /// windowAuthority.xaml の相互作用ロジック
    /// </summary>
    public partial class windowAuthority : Window
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "windowAuthority";

        /// <summary>
        /// ウィンドウ表示中フラグ
        /// </summary>
        public bool isShowing = false;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public windowAuthority()
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

                // タイトルバーを消しても画面移動可能にする処理
                this.MouseLeftButtonDown += delegate { DragMove(); };

                comboAuthority.Items.Add(AUTHORITY.OPERATOR.ToString());
                comboAuthority.Items.Add(AUTHORITY.MANAGER.ToString());
                comboAuthority.Items.Add(AUTHORITY.DEVELOPER.ToString());
                comboAuthority.SelectedIndex = (int)Resource.SystemStatus.Authority;

                // ウィンドウ表示中
                isShowing = true;
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
        private void Window_Closing(object sender, CancelEventArgs e)
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
                Resource.ErrorHandler(ex, true);
            }
        }

        /// <summary>
        /// ボタンクリック イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            UInt32 rc = 0;
            Button ctrl = (Button)sender;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {ctrl.Name}");
            try
            {
                // 選択した権限をセット
                if (comboAuthority.SelectedItem.ToString() == AUTHORITY.OPERATOR.ToString())
                    Resource.SystemStatus.Authority = AUTHORITY.OPERATOR;
                else if (comboAuthority.SelectedItem.ToString() == AUTHORITY.MANAGER.ToString())
                    Resource.SystemStatus.Authority = AUTHORITY.MANAGER;
                else if (comboAuthority.SelectedItem.ToString() == AUTHORITY.DEVELOPER.ToString())
                    Resource.SystemStatus.Authority = AUTHORITY.DEVELOPER;

                // クローズ
                this.Close();
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
        }


    }

}
