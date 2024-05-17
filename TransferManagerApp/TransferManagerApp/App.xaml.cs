//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using SystemConfig;
using ShareResource;


namespace TransferManagerApp
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            UInt32 rc = 0;
            try
            {
                // ------------------------------
                // INIファイル読み込み
                // ------------------------------
                String iniFileName = System.IO.Path.Combine(Const.IniDir, Const.IniFileName);
                rc = IniFile.Load(iniFileName);
                if (rc == 0 && Const.IniFileVersion != IniFile.Version)
                {
                    //Dialogs.ShowInformationMessage(this, string.Format("設定ファイルバージョンが一致していません アプリケーション = {0}, 設定ファイル = {1}", Const.IniFileVersion, IniFile.version), Const.Title, SystemIcons.Error);
                    MessageBox.Show($"設定ファイルバージョンが一致していません アプリケーション = {Const.IniFileVersion}, 設定ファイル = {IniFile.Version}", Const.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    // アプリ終了
                    Application.Current.Shutdown();
                }
                if (rc != 0)
                {
                    //Dialogs.ShowInformationMessage(this, "設定ファイルの読み込みに失敗しました.", Const.Title, SystemIcons.Error);
                    MessageBox.Show("設定ファイルの読み込みに失敗しました", Const.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    // アプリ終了
                    Application.Current.Shutdown();
                }


                // ------------------------------
                // オープニング画面表示
                // ------------------------------
                windowMain main = new windowMain();       // 〇 MainWindowのnewはここですること。 
                windowOpening opening = new windowOpening();
                Nullable<bool> result = opening.ShowDialog();
                if (result == true)
                {// オープニングOK

                    // ------------------------------
                    // メイン画面表示
                    // ------------------------------
                    //windowMain main = new windowMain();
                    main.Show();
                }
                else
                {// オープニングNG
                    
                    // アプリ終了
                    Application.Current.Shutdown();
                }

            }
            catch (Exception ex) 
            {
                // アプリ終了
                Application.Current.Shutdown();
            }

        }


    }
}
