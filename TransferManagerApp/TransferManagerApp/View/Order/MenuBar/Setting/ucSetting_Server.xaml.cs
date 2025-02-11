﻿//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using SystemConfig;
using BatchModule;
using DL_Logger;
using ErrorCodeDefine;
using ShareResource;


namespace TransferManagerApp
{
    /// <summary>
    /// ucSetting_Server.xaml の相互作用ロジック
    /// </summary>
    public partial class ucSetting_Server : UserControl, IDisposable
    {
        private const string THIS_NAME = "ucAisle";


        /// <summary>
        /// バインド用オブジェクト
        /// </summary>
        private BindObject _bindObject { get; set; } = new BindObject();



        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ucSetting_Server()
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

                //MyVariable = "aaaaa";
                // IPアドレス
                string[] serverIp = IniFile.DBIpAddress.Split('.');
                _bindObject.IpAddress01 = serverIp[0];
                _bindObject.IpAddress02 = serverIp[1];
                _bindObject.IpAddress03 = serverIp[2];
                _bindObject.IpAddress04 = serverIp[3];
                // PORT
                _bindObject.PortNo = IniFile.DBPortNo.ToString();

                // バインド処理
                DataContext = _bindObject;


                //DataContext = this; // DataContextをコードビハインド自体に設定
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                //Resource.ErrorHandler(ex, true);
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


            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, false);
            }
        }

        /// <summary>
        /// パラメータ保存
        /// </summary>
        /// <returns></returns>
        public UInt32 SaveParameter()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name}()");
            try
            {
                IniFile.DBIpAddress = $"{txtIpAddress01.Text}.{txtIpAddress02.Text}.{txtIpAddress03.Text}.{txtIpAddress04.Text}";
                IniFile.DBPortNo = int.Parse(txtPortNo.Text);
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
        /// Check Error State
        /// </summary>
        private static bool STATUS_SUCCESS(UInt32 err)
        {
            return err == (int)ErrorCodeList.STATUS_SUCCESS;
        }

    }



    /// <summary>
    /// バインド用クラス
    /// </summary>
    public class BindObject : INotifyPropertyChanged
    {

        private string _ipAddress01;
        /// <summary>
        /// IPアドレス 01
        /// </summary>
        public string IpAddress01
        {
            get { return _ipAddress01; }
            set
            {
                bool ok = Validation_IpAddress(ref value);
                if (ok) 
                {
                    if (_ipAddress01 != value)
                    {
                        _ipAddress01 = value;
                        OnPropertyChanged(nameof(IpAddress01));
                    }
                }
            }
        }

        private string _ipAddress02 = "";
        /// <summary>
        /// IPアドレス 02
        /// </summary>
        public string IpAddress02
        {
            get { return _ipAddress02; }
            set
            {
                bool ok = Validation_IpAddress(ref value);
                if (ok)
                {
                    if (_ipAddress02 != value)
                    {
                        _ipAddress02 = value;
                        OnPropertyChanged(nameof(IpAddress02));
                    }
                }
            }
        }

        private string _ipAddress03 = "";
        /// <summary>
        /// IPアドレス 03
        /// </summary>
        public string IpAddress03
        {
            get { return _ipAddress03; }
            set
            {
                bool ok = Validation_IpAddress(ref value);
                if (ok)
                {
                    if (_ipAddress03 != value)
                    {
                        _ipAddress03 = value;
                        OnPropertyChanged(nameof(IpAddress03));
                    }
                }
            }
        }

        private string _ipAddress04 = "";
        /// <summary>
        /// IPアドレス 04
        /// </summary>
        public string IpAddress04
        {
            get { return _ipAddress04; }
            set
            {
                bool ok = Validation_IpAddress(ref value);
                if (ok)
                {
                    if (_ipAddress04 != value)
                    {
                        _ipAddress04 = value;
                        OnPropertyChanged(nameof(IpAddress04));
                    }
                }
            }
        }

        private string _portNo = "";
        /// <summary>
        /// ポート番号
        /// </summary>
        public string PortNo
        {
            get { return _portNo; }
            set
            {
                bool ok = Validation_PortNo(ref value);
                if (ok)
                {
                    if (_portNo != value)
                    {
                        _portNo = value;
                        OnPropertyChanged(nameof(PortNo));
                    }
                }
            }
        }



        /// <summary>
        /// 入力制限 IPアドレス
        /// </summary>
        /// <returns></returns>
        public bool Validation_IpAddress(ref string inputStr)
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
                    if (!(inputStr.Length <= 3)) ok = false;
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"Validation_IpAddress ： {inputStr} , {ex.Message}");
                ok = false;
            }
            return ok;
        }
        /// <summary>
        /// 入力制限 ポート番号
        /// </summary>
        /// <returns></returns>
        public bool Validation_PortNo(ref string inputStr)
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
                    if (!(inputStr.Length <= 5)) ok = false;
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"Validation_PortNo ： {inputStr} , {ex.Message}");
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
