//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using DL_Logger;
using ErrorCodeDefine;
using ShareResource;


namespace TransferManagerApp
{
    /// <summary>
    /// UserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class ucTab_OperationMonitor : UserControl, IDisposable
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "ucTab_OperationMonitor";





        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ucTab_OperationMonitor()
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

                // ユーザーコントロールに入力
                ucAisle01.AisleIndex = 0;
                ucAisle02.AisleIndex = 1;
                ucAisle03.AisleIndex = 2;
                ucAisle04.AisleIndex = 3;
                //ucAisle05.AisleIndex = 4;

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, false);
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
                //if (_tmrUpdateDisplay != null)
                //    _tmrUpdateDisplay.Stop();
                //_tmrUpdateDisplay = null;

                ucAisle01.Dispose();
                ucAisle01 = null;
                ucAisle02.Dispose();
                ucAisle02 = null;
                ucAisle03.Dispose();
                ucAisle03 = null;
                ucAisle04.Dispose();
                ucAisle04 = null;

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, false);
            }
        }


        ///// <summary>
        ///// (デバッグ)
        ///// ワーク情報をアイルにセット
        ///// </summary>
        ///// <returns></returns>
        //public UInt32 SetWork(BindRegistWork workInfo) 
        //{
        //    UInt32 rc = 0;
        //    Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
        //    try
        //    {
        //        ucAisle01.SetWork(workInfo);


        //    }
        //    catch (Exception ex)
        //    {
        //        rc = (Int32)ErrorCodeList.EXCEPTION;
        //        Resource.ErrorHandler(ex);
        //    }
        //    Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
        //    return rc;

        //}


    }
}
