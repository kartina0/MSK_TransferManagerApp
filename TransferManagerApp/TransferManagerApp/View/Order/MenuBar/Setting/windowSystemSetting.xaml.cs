//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using SystemConfig;
using DL_Logger;
using ErrorCodeDefine;
using ServerModule;
using ShareResource;


namespace TransferManagerApp
{
    /// <summary>
    /// windowSystemSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class windowSystemSetting : Window
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "windowSystemSetting";

        /// <summary>
        /// ウィンドウ表示中フラグ
        /// </summary>
        public bool isShowing = false;



        /// <summary>
        /// コンストラクタ
        /// </summary>
        public windowSystemSetting()
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UInt32 rc = 0;
            Button ctrl = (Button)sender;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {ctrl.Name}");
            try
            {
                if (ctrl == btnSave) 
                {
                    rc = Save();
                }
                if (ctrl == btnExit)
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
        /// ツリービュー 選択イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_Selected(object sender, RoutedEventArgs e) 
        {
            UInt32 rc = 0;
            TreeViewItem ctrl = (TreeViewItem)sender;
            Logger.WriteLog(LogType.CONTROL, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() {ctrl.Name}");
            try
            {
                if (ctrl == treeViewItem_Basic) 
                {
                    borderBasic.Visibility = Visibility.Visible;
                    borderEquipment.Visibility = Visibility.Hidden;
                    borderServer.Visibility = Visibility.Hidden;
                }
                else if (ctrl == treeViewItem_Equipment)
                {
                    borderBasic.Visibility = Visibility.Hidden;
                    borderEquipment.Visibility = Visibility.Visible;
                    borderServer.Visibility = Visibility.Hidden;
                }
                else if (ctrl == treeViewItem_Server)
                {
                    borderBasic.Visibility = Visibility.Hidden;
                    borderEquipment.Visibility = Visibility.Hidden;
                    borderServer.Visibility = Visibility.Visible;
                }

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        private UInt32 Save() 
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // 設備
                rc = ucSetting_Equipment.SaveParameter();

                // サーバー
                if (STATUS_SUCCESS(rc))
                    rc = ucSetting_Server.SaveParameter();


            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex);
            }
            finally 
            {
                if (STATUS_SUCCESS(rc)) 
                {
                    IniFile.Save();
                }
                else
                {
                    // エラーなら元に戻す
                    string iniFileName = System.IO.Path.Combine(Const.IniDir, Const.IniFileName);
                    IniFile.Load(iniFileName);
                }
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;


        }



        /// <summary>
        /// Check Error State
        /// </summary>
        private static bool STATUS_SUCCESS(UInt32 err)
        {
            return err == (int)ErrorCodeList.STATUS_SUCCESS;
        }
    }
}
