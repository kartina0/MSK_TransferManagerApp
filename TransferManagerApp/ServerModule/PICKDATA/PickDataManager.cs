using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;

using SystemConfig;
using DL_Logger;
using ErrorCodeDefine;


namespace ServerModule
{
    /// <summary>
    /// 上位サーバーのPICKDATAファイル管理
    /// </summary>
    public class PickDataManager
    {
        private const string THIS_NAME = "PickDataManager";


        /// <summary>
        /// PICKDATAリスト
        /// 1~3便
        /// </summary>
        public FirstRecord[] PickDataList = null;

        /// <summary>
        /// PICKDATA ファイルパス (サーバー)
        /// </summary>
        private string[] _pickDataFilePathServer = new string[Const.MaxPostCount];
        /// <summary>
        /// PICKDATA ファイルパス (ローカル)
        /// </summary>
        private string[] _pickDataFilePathLocal = new string[Const.MaxPostCount];


        #region 先頭レコード フォーマット
        /// <summary> 開始位置 先頭レコード ID </summary>
        private const int _startAddr_First_Id = 0;
        /// <summary> 開始位置 先頭レコード 連番 </summary>
        private const int _startAddr_First_Index = 6;
        /// <summary> 開始位置 先頭レコード 処理日付 </summary>
        private const int _startAddr_First_ProcessDate = 8;
        /// <summary> 開始位置 先頭レコード 便 </summary>
        private const int _startAddr_First_Post = 14;
        /// <summary> 開始位置 先頭レコード 納品日 </summary>
        private const int _startAddr_First_OrderDate = 15;
        
        /// <summary> データ長 先頭レコード 全体 </summary>
        private const int _dataSize_First_Total = 256;
        /// <summary> データ長 先頭レコード ID </summary>
        private const int _dataSize_First_Id = 6;
        /// <summary> データ長 先頭レコード 連番 </summary>
        private const int _dataSize_First_Index = 2;
        /// <summary> データ長 先頭レコード 処理日付 </summary>
        private const int _dataSize_First_ProcessDate = 6;
        /// <summary> データ長 先頭レコード 便 </summary>
        private const int _dataSize_First_Post = 1;
        /// <summary> データ長 先頭レコード 納品日 </summary>
        private const int _dataSize_First_OrderDate = 6;
        #endregion

        #region 中仕分レコード フォーマット
        /// <summary> 開始位置 中仕分レコード ID </summary>
        private const int _startAddr_Middle_Id = 0;
        /// <summary> 開始位置 中仕分レコード 便 </summary>
        private const int _startAddr_Middle_Post = 6;
        /// <summary> 開始位置 中仕分レコード 社区分 </summary>
        private const int _startAddr_Middle_CompanyType = 7;
        /// <summary> 開始位置 中仕分レコード JANコード </summary>
        private const int _startAddr_Middle_JanCode = 9;
        /// <summary> 開始位置 中仕分レコード 商品コード </summary>
        private const int _startAddr_Middle_WorkCode = 22;
        /// <summary> 開始位置 中仕分レコード 商品名(漢字) </summary>
        private const int _startAddr_Middle_WorkName = 28;
        /// <summary> 開始位置 中仕分レコード 商品名(ｶﾅ) </summary>
        private const int _startAddr_Middle_WorkNameKana = 88;
        /// <summary> 開始位置 中仕分レコード MAX </summary>
        private const int _startAddr_Middle_Max = 118;
        /// <summary> 開始位置 中仕分レコード センター入数 </summary>
        private const int _startAddr_Middle_CenterCount = 119;
        /// <summary> 開始位置 中仕分レコード 売価 </summary>
        private const int _startAddr_Middle_Price = 122;
        /// <summary> 開始位置 中仕分レコード 数量 </summary>
        private const int _startAddr_Middle_Num = 127;
        /// <summary> 開始位置 中仕分レコード 店舗数 </summary>
        private const int _startAddr_Middle_StoreNum = 132;

        /// <summary> データ長 中仕分レコード 全体 </summary>
        private const int _dataSize_Middle_Total = 256;
        /// <summary> データ長 中仕分レコード ID </summary>
        private const int _dataSize_Middle_Id = 6;
        /// <summary> データ長 中仕分レコード 便 </summary>
        private const int _dataSize_Middle_Post = 1;
        /// <summary> データ長 中仕分レコード 社区分 </summary>
        private const int _dataSize_Middle_CompanyType = 2;
        /// <summary> データ長 中仕分レコード JANコード </summary>
        private const int _dataSize_Middle_JanCode = 13;
        /// <summary> データ長 中仕分レコード 商品コード </summary>
        private const int _dataSize_Middle_WorkCode = 6;
        /// <summary> データ長 中仕分レコード 商品名(漢字) </summary>
        private const int _dataSize_Middle_WorkName = 44;
        //private const int _dataSize_Middle_WorkName = 60;
        /// <summary> データ長 中仕分レコード 商品名(ｶﾅ) </summary>
        private const int _dataSize_Middle_WorkNameKana = 30;
        /// <summary> データ長 中仕分レコード MAX </summary>
        private const int _dataSize_Middle_Max = 1;
        /// <summary> データ長 中仕分レコード センター入数 </summary>
        private const int _dataSize_Middle_CenterCount = 3;
        /// <summary> データ長 中仕分レコード 売価 </summary>
        private const int _dataSize_Middle_Price = 5;
        /// <summary> データ長 中仕分レコード 数量 </summary>
        private const int _dataSize_Middle_Num = 5;
        /// <summary> データ長 中仕分レコード 店舗数 </summary>
        private const int _dataSize_Middle_StoreNum = 3;
        #endregion

        #region 小仕分レコード フォーマット
        /// <summary> 開始位置 小仕分レコード ID </summary>
        private const int _startAddr_Minimum_Id = 0;
        /// <summary> 開始位置 小仕分レコード 連番 </summary>
        private const int _startAddr_Minimum_Index = 6;
        /// <summary> 開始位置 小仕分レコード アイル </summary>
        private const int _startAddr_Minimum_AisleNo = 8;
        /// <summary> 開始位置 小仕分レコード スロット </summary>
        private const int _startAddr_Minimum_SlotNo = 10;
        /// <summary> 開始位置 小仕分レコード 数量 </summary>
        private const int _startAddr_Minimum_Num = 12;

        /// <summary> データ長 小仕分レコード 全体 </summary>
        private const int _dataSize_Minimum_Total = 256;
        /// <summary> データ長 小仕分レコード ID </summary>
        private const int _dataSize_Minimum_Id = 6;
        /// <summary> データ長 小仕分レコード 連番 </summary>
        private const int _dataSize_Minimum_Index = 2;
        /// <summary> データ長 小仕分レコード アイル </summary>
        private const int _dataSize_Minimum_AisleNo = 2;
        /// <summary> データ長 小仕分レコード スロット </summary>
        private const int _dataSize_Minimum_SlotNo = 2;
        /// <summary> データ長 小仕分レコード 数量 </summary>
        private const int _dataSize_Minimum_Num = 3;
        #endregion

        #region 最終レコード フォーマット
        /// <summary> データ長 小仕分レコード 全体 </summary>
        private const int _dataSize_Final_Total = 256;
        #endregion


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PickDataManager()
        {
            PickDataList = new FirstRecord[Const.MaxPostCount];
            for (int i = 0; i < Const.MaxPostCount; i++)
                PickDataList[i] = new FirstRecord();

            for (int postIndex = 0; postIndex < Const.MaxPostCount; postIndex++)
            {
                _pickDataFilePathServer[postIndex] = Path.Combine(IniFile.PickdataServerDir, $"{postIndex + 1}\\PICKDATA0");
                _pickDataFilePathLocal[postIndex] = Path.Combine(IniFile.PickdataLocalDir, $"{postIndex + 1}\\PICKDATA0");
            }
        }


        /// <summary>
        /// サーバーのPICKDATAフォルダをローカルにzip保存
        /// </summary>
        /// <returns></returns>
        public UInt32 BackUpPickDataDir()
        {
            UInt32 rc = 0;
            try
            {
                // バックアップフォルダがなければ作成
                string dir = Path.GetDirectoryName(IniFile.PickdataBackupDir);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                // サーバーのPICKDATAファイルの日時取得
                DateTime latestUpdatedDtServer = File.GetLastWriteTime(_pickDataFilePathServer[1]);

                // ローカルのバックアップフォルダにZIP化して保存しておく
                string zipPath = Path.Combine(IniFile.PickdataBackupDir, $"pickdata_{latestUpdatedDtServer.Year.ToString("D4")}{latestUpdatedDtServer.Month.ToString("D2")}{latestUpdatedDtServer.Day.ToString("D2")}.zip");
                if (!File.Exists(zipPath))
                    ZipFile.CreateFromDirectory(IniFile.PickdataServerDir, zipPath);

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }

        /// <summary>
        /// サーバーのPICKDATAフォルダをローカルにコピー
        /// </summary>
        /// <returns></returns>
        public UInt32 CopyPickDataFile(int postIndex, out bool isTodaysFileLocal, out bool isServerUpdated) 
        {
            UInt32 rc = 0;
            isTodaysFileLocal = false;
            isServerUpdated = false;
            try
            {
                // -----------------------------------------------
                // サーバーとローカルのファイル情報を確認
                // -----------------------------------------------
                // サーバーのPICKDATA0の存在チェック
                if (!File.Exists(_pickDataFilePathServer[postIndex]))
                {
                    rc = (UInt32)ErrorCodeList.PICKDATA_FILE_IS_NOT_EXIST_SERVER;
                    return rc;
                }

                // サーバーのファイルが当日のものかチェック
                // ローカルのPICKDATA0が当日のファイルかチェック
                // 1.日付切り替わり時刻より前なら、前日のPICKDATA更新時刻(11:00)以降のファイルが有ればOK
                // 2.日付切り替わり時刻より後なら、今日のPICKDATA更新時刻(11:00)以降のファイルがあればOK
                bool isTodaysFileServer = false;
                DateTime currentDt = DateTime.Now;
                DateTime fileDt = File.GetLastWriteTime(_pickDataFilePathServer[postIndex]);
                if (currentDt < DateTime.Today.AddHours(IniFile.DateChangeTime.Hour).AddMinutes(IniFile.DateChangeTime.Minute))
                {// 日付切り替わり時刻前 = 前日11:00以降のPICKDATAがあればよい
                    if (fileDt >= DateTime.Today.AddDays(-1).AddHours(IniFile.PickDataUpdatedTime.Hour).AddMinutes(IniFile.PickDataUpdatedTime.Minute))
                    {// PICKDATAの更新日時が前日11:00以降
                        isTodaysFileServer = true;
                    }
                    else
                    {// PICKDATAの更新日時が前日11:00より前
                        string message = $"{postIndex + 1}便のPICKDATA0ファイルが本日のものではありません。";
                        Logger.WriteLog(LogType.ERROR, message);
                    }
                }
                else
                {// 日付切り替わり時刻後 = 当日11:00以降のPICKDATAがあればよい
                    if (fileDt >= DateTime.Today.AddHours(IniFile.PickDataUpdatedTime.Hour).AddMinutes(IniFile.PickDataUpdatedTime.Minute))
                    {// PICKDATAの更新日時が当日11:00以降
                        isTodaysFileServer = true;
                    }
                    else
                    {// PICKDATAの更新日時が当日11:00より前
                        string message = $"{postIndex + 1}便のPICKDATA0ファイルが本日のものではありません。";
                        Logger.WriteLog(LogType.ERROR, message);
                    }
                }

                // ローカルのPICKDATA0の存在チェック
                bool localExist = false;
                if (File.Exists(_pickDataFilePathLocal[postIndex]))
                    localExist = true;


                // -----------------------------------------------
                // サーバーとローカルを比較してコピー
                // -----------------------------------------------
                if (isTodaysFileServer)
                {// サーバーに本日のファイルがある

                    // サーバーとローカルのファイルの更新日時を比較
                    if (localExist)
                    {
                        DateTime latestUpdatedDtServer = File.GetLastWriteTime(_pickDataFilePathServer[postIndex]);
                        DateTime latestUpdatedDtLocal = File.GetLastWriteTime(_pickDataFilePathLocal[postIndex]);
                        if (latestUpdatedDtServer != latestUpdatedDtLocal)
                            isServerUpdated = true;
                    }
                    else 
                    {
                        isServerUpdated = true;
                    }

                    // サーバーからローカルにファイルをコピー
                    if (!localExist || isServerUpdated)
                    {
                        // temp0フォルダがなければ作成
                        string dir = Path.GetDirectoryName(_pickDataFilePathLocal[postIndex]);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        // ファイルをコピー
                        File.Copy(_pickDataFilePathServer[postIndex], _pickDataFilePathLocal[postIndex], true);
                    }
                }


                // -----------------------------------------------
                // 最終的にローカルのファイルが当日のものかチェック
                // -----------------------------------------------
                localExist = false;
                if (File.Exists(_pickDataFilePathLocal[postIndex]))
                    localExist = true;
                if (localExist)
                {
                    currentDt = DateTime.Now;
                    fileDt = File.GetLastWriteTime(_pickDataFilePathLocal[postIndex]);
                    if (currentDt < DateTime.Today.AddHours(IniFile.DateChangeTime.Hour).AddMinutes(IniFile.DateChangeTime.Minute))
                    {// 日付切り替わり時刻前 = 前日11:00以降のPICKDATAがあればよい
                        if (fileDt >= DateTime.Today.AddDays(-1).AddHours(IniFile.PickDataUpdatedTime.Hour).AddMinutes(IniFile.PickDataUpdatedTime.Minute))
                        {// PICKDATAの更新日時が前日11:00以降
                            isTodaysFileLocal = true;
                        }
                        else
                        {// PICKDATAの更新日時が前日11:00より前
                            string message = $"{postIndex + 1}便のPICKDATA0ファイルが本日のものではありません。";
                            Logger.WriteLog(LogType.ERROR, message);
                        }
                    }
                    else
                    {// 日付切り替わり時刻後 = 当日11:00以降のPICKDATAがあればよい
                        if (fileDt >= DateTime.Today.AddHours(IniFile.PickDataUpdatedTime.Hour).AddMinutes(IniFile.PickDataUpdatedTime.Minute))
                        {// PICKDATAの更新日時が当日11:00以降
                            isTodaysFileLocal = true;
                        }
                        else
                        {// PICKDATAの更新日時が当日11:00より前
                            string message = $"{postIndex + 1}便のPICKDATA0ファイルが本日のものではありません。";
                            Logger.WriteLog(LogType.ERROR, message);
                        }
                    }
                }




                //// サーバーとローカルのPICKDATAの更新日時を比較
                //bool serverUpdated = false;
                //DateTime latestUpdatedDtServer = File.GetLastWriteTime(_pickDataFilePathServer[postIndex]);
                //DateTime latestUpdatedDtLocal = File.GetLastWriteTime(_pickDataFilePathLocal[postIndex]);
                //if (latestUpdatedDtServer != latestUpdatedDtLocal) 
                //{
                //    serverUpdated = true;
                //}

                //// サーバーからローカルにPICKDATAをコピー
                //if (localNotExist || serverUpdated)
                //{
                //    // temp0フォルダがなければ作成
                //    string dir = Path.GetDirectoryName(_pickDataFilePathLocal[postIndex]);
                //    if (!Directory.Exists(dir))
                //        Directory.CreateDirectory(dir);

                //    // ファイルをコピー
                //    File.Copy(_pickDataFilePathServer[postIndex], _pickDataFilePathLocal[postIndex], true);


                //    //// Pickdataフォルダがなければ作成
                //    //if (!Directory.Exists(IniFile.PickdataBackupDir))
                //    //    Directory.CreateDirectory(IniFile.PickdataBackupDir);

                //    //// 既存のpickdataフォルダを削除
                //    //if (File.Exists(IniFile.PickdataLocalDir))
                //    //    Directory.Delete(IniFile.PickdataLocalDir);

                //    //// 今日のpickdataフォルダをサーバーからローカルにコピー
                //    //rc = DirectoryCopy(IniFile.PickdataServerDir, IniFile.PickdataLocalDir);

                //    //// ローカルのバックアップフォルダにZIP化して保存しておく
                //    //DateTime today = DateTime.Now;
                //    //string zipPath = Path.Combine(IniFile.PickdataBackupDir, $"pickdata_{today.Year.ToString("D4")}{today.Month.ToString("D2")}{today.Day.ToString("D2")}.zip");
                //    //if (!File.Exists(zipPath))
                //    //    ZipFile.CreateFromDirectory(IniFile.PickdataLocalDir, zipPath);


                //    isCopied = true;
                //}

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }

        ///// <summary>
        ///// ローカルPICKDATAが当日のものか確認
        ///// </summary>
        ///// <returns></returns>
        //public UInt32 CheckPickDataToday(int postIndex, out bool isTodayPickData)
        //{
        //    UInt32 rc = 0;
        //    isTodayPickData = false;
        //    try
        //    {
        //        // ローカルのPICKDATA0が当日のファイルかチェック
        //        // 1.日付切り替わり時刻より前なら、前日のPICKDATA更新時刻(11:00)以降のファイルが有ればOK
        //        // 2.日付切り替わり時刻より後なら、今日のPICKDATA更新時刻(11:00)以降のファイルがあればOK
        //        DateTime currentDt = DateTime.Now;
        //        DateTime dtPickLocal = File.GetLastWriteTime(_pickDataFilePathLocal[postIndex]);
        //        if (currentDt < DateTime.Today.AddHours(IniFile.DateChangeTime.Hour).AddMinutes(IniFile.DateChangeTime.Minute))
        //        {// 日付切り替わり時刻前 = 前日11:00以降のPICKDATAがあればよい
        //            if (dtPickLocal >= DateTime.Today.AddDays(-1).AddHours(IniFile.PickDataUpdatedTime.Hour).AddMinutes(IniFile.PickDataUpdatedTime.Minute))
        //            {// PICKDATAの更新日時が前日11:00以降
        //                isTodayPickData = true;
        //            }
        //            else
        //            {// PICKDATAの更新日時が前日11:00より前
        //                string message = $"{postIndex + 1}便のPICKDATA0ファイルが本日のものではありません。";
        //                Logger.WriteLog(LogType.ERROR, message);
        //            }
        //        }
        //        else
        //        {// 日付切り替わり時刻後 = 当日11:00以降のPICKDATAがあればよい
        //            if (dtPickLocal >= DateTime.Today.AddHours(IniFile.PickDataUpdatedTime.Hour).AddMinutes(IniFile.PickDataUpdatedTime.Minute))
        //            {// PICKDATAの更新日時が当日11:00以降
        //                isTodayPickData = true;
        //            }
        //            else
        //            {// PICKDATAの更新日時が当日11:00より前
        //                string message = $"{postIndex + 1}便のPICKDATA0ファイルが本日のものではありません。";
        //                Logger.WriteLog(LogType.ERROR, message);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
        //        rc = (Int32)ErrorCodeList.EXCEPTION;
        //    }
        //    //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
        //    return rc;
        //}


        ///// <summary>
        ///// サーバーのPICKDATAファイルの更新確認
        ///// </summary>
        ///// <returns></returns>
        //public bool IsUpdatedPickData()
        //{
        //    bool serverUpdated = false;
        //    //Logger.WriteLog(LogType.METHOD_IN, $"{GetType().Name} {MethodBase.GetCurrentMethod().Name}()");
        //    try
        //    {
        //        if (System.IO.File.Exists(IniFile.PickDataServerFilePath))
        //        {
        //            // サーバーのマスタファイルの更新日時を取得、保持
        //            _latestUpdatedDtServerPickData = System.IO.File.GetLastWriteTime(IniFile.PickDataServerFilePath);
        //            // サーバーとローカルで更新日時を比較
        //            if (_latestUpdatedDtLocalPickData != _latestUpdatedDtServerPickData)
        //            {// サーバーのマスタファイルが更新されている！
        //                serverUpdated = true;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
        //    }
        //    //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {serverUpdated}");
        //    return serverUpdated;
        //}
        ///// <summary>
        ///// サーバーのPICKDATAファイルをローカルコピー
        ///// </summary>
        ///// <returns></returns>
        //public UInt32 CopyPickData()
        //{
        //    UInt32 rc = 0;
        //    //Logger.WriteLog(LogType.METHOD_IN, $"{GetType().Name} {MethodBase.GetCurrentMethod().Name}()");
        //    try
        //    {
        //        if (System.IO.File.Exists(IniFile.PickDataServerFilePath))
        //        {
        //            if (System.IO.File.Exists(IniFile.PickDataLocalFilePath))
        //            {
        //                // すでに最新版をコピー済なら終了
        //                if (_latestUpdatedDtServerPickData == System.IO.File.GetLastWriteTime(IniFile.MasterWorkLocalFilePath))
        //                    return rc;
        //            }

        //            // サーバーからローカルにコピー(既にあれば上書きする)
        //            File.Copy(IniFile.PickDataServerFilePath, IniFile.PickDataLocalFilePath, true);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
        //        rc = (Int32)ErrorCodeList.EXCEPTION;
        //    }
        //    //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
        //    return rc;
        //}
        /// <summary>
        /// 1便 読み出し
        /// </summary>
        /// <returns>エラーコード</returns>
        public UInt32 Read(int postIndex)
        {
            UInt32 rc = 0;
            int index = 0;
            byte[] byteData = null;
            byte[] buf = null;
            byte[] record = null;
            try
            {
                string localFilePath = Path.Combine(IniFile.PickdataLocalDir, $"{postIndex + 1}\\PICKDATA0");

                // ファイル存在確認
                if (!File.Exists(localFilePath)) 
                {
                    rc = (UInt32)ErrorCodeList.PICKDATA_FILE_IS_NOT_EXIST_LOCAL;
                    return rc;
                }


                // リストをクリア
                PickDataList[postIndex] = new FirstRecord();
                using (System.IO.FileStream fs = new System.IO.FileStream(localFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    // ファイルサイズ分のバイト配列を作成
                    byteData = new byte[fs.Length];
                    // ファイルからバイトデータを読み込む
                    fs.Read(byteData, 0, (int)fs.Length);

                    // 先頭レコード～最終レコードまで読み出し
                    byte[] b = null;
                    string text = "";
                    while (true)
                    {
                        // 残りバイト数をチェック
                        if (byteData.Length <= 6) 
                        {
                            // ループ終了
                            break;
                        }


                        // ------------------------------
                        // IDを判定
                        // ------------------------------
                        if (byteData[4] == 48 && byteData[5] == 48)
                        {// 先頭レコード 00

                            // 再度バイト数をチェック
                            if (byteData.Length < _dataSize_First_Total) 
                            {
                                Logger.WriteLog(LogType.ERROR, "[PICKDATA読み出しエラー] 先頭レコードIDを取得したが、1レコード分のバイトデータがない");
                                break;
                            }

                            // 1レコード分のバイト配列にコピー
                            record = new byte[_dataSize_First_Total];
                            Array.Copy(byteData, 0, record, 0, _dataSize_First_Total);

                            // ------------------------------
                            // 読み込み
                            // ------------------------------
                            // 便
                            b = new byte[_dataSize_First_Post];
                            Array.Copy(record, _startAddr_First_Post, b, 0, _dataSize_First_Post);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            postIndex = int.Parse(text) - 1;
                            if (postIndex < 0 || postIndex > 2) 
                            {
                                Logger.WriteLog(LogType.ERROR, $"PICKDATAの便Noが範囲外です");
                                rc = (Int32)ErrorCodeList.EXCEPTION;
                                break;
                            }

                            PickDataList[postIndex].postNo = int.Parse(text);

                            // 連番
                            b = new byte[_dataSize_First_Index];
                            Array.Copy(record, _startAddr_First_Index, b, 0, _dataSize_First_Index);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            PickDataList[postIndex].index = int.Parse(text);
                            // 処理日付
                            b = new byte[_dataSize_First_ProcessDate];
                            Array.Copy(byteData, _startAddr_First_ProcessDate, b, 0, _dataSize_First_ProcessDate);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            PickDataList[postIndex].processDate = DateTime.ParseExact("20" + text + "000000", "yyyyMMddHHmmss", null);
                            // 納品日
                            b = new byte[_dataSize_First_OrderDate];
                            Array.Copy(byteData, _startAddr_First_OrderDate, b, 0, _dataSize_First_OrderDate);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            PickDataList[postIndex].orderDate = DateTime.ParseExact("20" + text + "000000", "yyyyMMddHHmmss", null);


                            // バイトデータから今回のレコードを削除
                            int after_length = byteData.Length - _dataSize_First_Total;
                            buf = new byte[after_length];
                            Array.Copy(byteData, _dataSize_First_Total, buf, 0, after_length);
                            byteData = new byte[after_length];
                            Array.Copy(buf, 0, byteData, 0, after_length);
                        }
                        else if (byteData[4] == 48 && byteData[5] == 49)
                        {// 中仕分レコード 01

                            // 再度バイト数をチェック
                            if (byteData.Length < _dataSize_Middle_Total)
                            {
                                Logger.WriteLog(LogType.ERROR, "[PICKDATA読み出しエラー] 中仕分レコードIDを取得したが、1レコード分のバイトデータがない");
                                break;
                            }

                            // 1レコード分のバイト配列にコピー
                            record = new byte[_dataSize_Middle_Total];
                            Array.Copy(byteData, 0, record, 0, _dataSize_Middle_Total);

                            // ------------------------------
                            // 読み込み
                            // ------------------------------
                            MiddleRecord middle = new MiddleRecord();
                            // 便
                            b = new byte[_dataSize_Middle_Post];
                            Array.Copy(record, _startAddr_Middle_Post, b, 0, _dataSize_Middle_Post);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            middle.postNo = int.Parse(text);
                            // 社区分
                            b = new byte[_dataSize_Middle_CompanyType];
                            Array.Copy(record, _startAddr_Middle_CompanyType, b, 0, _dataSize_Middle_CompanyType);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            middle.companyType = int.Parse(text);
                            // JANコード
                            b = new byte[_dataSize_Middle_JanCode];
                            Array.Copy(record, _startAddr_Middle_JanCode, b, 0, _dataSize_Middle_JanCode);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            middle.JANCode = text;
                            // 商品コード
                            b = new byte[_dataSize_Middle_WorkCode];
                            Array.Copy(record, _startAddr_Middle_WorkCode, b, 0, _dataSize_Middle_WorkCode);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            middle.workCode = text;
                            // 商品名(漢字)
                            b = new byte[_dataSize_Middle_WorkName];
                            Array.Copy(record, _startAddr_Middle_WorkName, b, 0, _dataSize_Middle_WorkName);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            middle.workName = text;
                            // 商品名(ｶﾅ)
                            b = new byte[_dataSize_Middle_WorkNameKana];
                            Array.Copy(record, _startAddr_Middle_WorkNameKana, b, 0, _dataSize_Middle_WorkNameKana);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            middle.workNameKana = text;
                            // MAX
                            b = new byte[_dataSize_Middle_Max];
                            Array.Copy(record, _startAddr_Middle_Max, b, 0, _dataSize_Middle_Max);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            middle.max = int.Parse(text);
                            // センター入数
                            b = new byte[_dataSize_Middle_CenterCount];
                            Array.Copy(record, _startAddr_Middle_CenterCount, b, 0, _dataSize_Middle_CenterCount);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            middle.centerCount = int.Parse(text);
                            // 売価
                            b = new byte[_dataSize_Middle_Price];
                            Array.Copy(record, _startAddr_Middle_Price, b, 0, _dataSize_Middle_Price);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            middle.price = int.Parse(text);

                            // 数量、店舗数
                            int len = _dataSize_Middle_Num + _dataSize_Middle_StoreNum;
                            for (int i = 0; i < Const.MaxStationCount; i++)
                            {
                                // 数量
                                b = new byte[_dataSize_Middle_Num];
                                Array.Copy(record, _startAddr_Middle_Num + (i * len), b, 0, _dataSize_Middle_Num);
                                text = Encoding.GetEncoding("shift_jis").GetString(b);
                                middle.countList[i].count = int.Parse(text);
                                // 店舗数
                                b = new byte[_dataSize_Middle_StoreNum];
                                Array.Copy(record, _startAddr_Middle_StoreNum + (i * len), b, 0, _dataSize_Middle_StoreNum);
                                text = Encoding.GetEncoding("shift_jis").GetString(b);
                                middle.countList[i].storeCount = int.Parse(text);
                            }

                            // リストに追加
                            PickDataList[postIndex].middleRecordList.Add(middle);


                            // バイトデータから今回のレコードを削除
                            int after_length = byteData.Length - _dataSize_Middle_Total;
                            buf = new byte[after_length];
                            Array.Copy(byteData, _dataSize_Middle_Total, buf, 0, after_length);
                            byteData = new byte[after_length];
                            Array.Copy(buf, 0, byteData, 0, after_length);
                        }
                        else if (byteData[4] == 48 && byteData[5] == 50)
                        {// 小仕分レコード 02

                            // 再度バイト数をチェック
                            if (byteData.Length < _dataSize_Minimum_Total)
                            {
                                Logger.WriteLog(LogType.ERROR, "[PICKDATA読み出しエラー] 小仕分レコードIDを取得したが、1レコード分のバイトデータがない");
                                break;
                            }

                            // 1レコード分のバイト配列にコピー
                            record = new byte[_dataSize_Minimum_Total];
                            Array.Copy(byteData, 0, record, 0, _dataSize_Minimum_Total);

                            // ------------------------------
                            // 読み込み
                            // ------------------------------
                            int len = _dataSize_Minimum_AisleNo + _dataSize_Minimum_SlotNo + _dataSize_Minimum_Num;
                            for (int i = 0; i < 35; i++)
                            {
                                MinimumRecord minimum = new MinimumRecord();

                                // アイル
                                b = new byte[_dataSize_Minimum_AisleNo];
                                Array.Copy(record, _startAddr_Minimum_AisleNo + (i * len), b, 0, _dataSize_Minimum_AisleNo);
                                text = Encoding.GetEncoding("shift_jis").GetString(b);
                                if (int.Parse(text) <= 0) 
                                {   // 0だったら終了
                                    break;
                                }
                                minimum.aisleNo = int.Parse(text);
                                // スロット
                                b = new byte[_dataSize_Minimum_SlotNo];
                                Array.Copy(record, _startAddr_Minimum_SlotNo + (i * len), b, 0, _dataSize_Minimum_SlotNo);
                                text = Encoding.GetEncoding("shift_jis").GetString(b);
                                minimum.slotNo = int.Parse(text);
                                // 数量
                                b = new byte[_dataSize_Minimum_Num];
                                Array.Copy(record, _startAddr_Minimum_Num + (i * len), b, 0, _dataSize_Minimum_Num);
                                text = Encoding.GetEncoding("shift_jis").GetString(b);
                                minimum.count = int.Parse(text);

                                // リストに追加
                                PickDataList[postIndex].middleRecordList[PickDataList[postIndex].middleRecordList.Count() - 1].minimumRecordList.Add(minimum);
                            }


                            // バイトデータから今回のレコードを削除
                            int after_length = byteData.Length - _dataSize_Minimum_Total;
                            buf = new byte[after_length];
                            Array.Copy(byteData, _dataSize_Minimum_Total, buf, 0, after_length);
                            byteData = new byte[after_length];
                            Array.Copy(buf, 0, byteData, 0, after_length);
                        }
                        else if (byteData[4] == 57 && byteData[5] == 57)
                        {// 最終レコード 99
                         
                            // 再度バイト数をチェック
                            if (byteData.Length < _dataSize_Final_Total)
                            {
                                Logger.WriteLog(LogType.ERROR, "[PICKDATA読み出しエラー] 最終レコードIDを取得したが、1レコード分のバイトデータがない");
                                break;
                            }

                            // バイトデータから今回のレコードを削除
                            int after_length = byteData.Length - _dataSize_Minimum_Total;
                            buf = new byte[after_length];
                            Array.Copy(byteData, _dataSize_Minimum_Total, buf, 0, after_length);
                            byteData = new byte[after_length];
                            Array.Copy(buf, 0, byteData, 0, after_length);

                            // ループ終了
                            break;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.PICKDATA_FILE_READ_ERROR;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }


        /// <summary>
        /// 1ファイル DB書き込み
        /// </summary>
        /// <returns>エラーコード</returns>
        public UInt32 WriteDB(int postIndex, List<MasterStore> masterStoreList)
        {
            UInt32 rc = 0;
            try
            {
                if (PickDataList[postIndex].middleRecordList.Count <= 0) 
                    return rc;

                // 現在日時
                DateTime dt = DateTime.Now;
                // 日付切り替わり処理
                if (dt < DateTime.Today.AddHours(IniFile.DateChangeTime.Hour).AddMinutes(IniFile.DateChangeTime.Minute))
                {
                    dt = dt.AddDays(-1);
                }

                string workHeader_TableName = $"dp01_pick_head_0{dt.Year.ToString("D4")}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";
                string storeOrder_TableName = $"dp02_pick_detail_0{dt.Year.ToString("D4")}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";


                OrderInfo_IF_DB dbIF = new OrderInfo_IF_DB();
                OrderData orderData = null;
                foreach (var middle in PickDataList[postIndex].middleRecordList)
                {
                    // ------------------------------
                    // 商品ヘッダテーブル
                    // ------------------------------
                    orderData = new OrderData();
                    orderData.orderDate = PickDataList[postIndex].processDate;
                    orderData.postNo = PickDataList[postIndex].postNo;
                    orderData.orderDateRequest = PickDataList[postIndex].orderDate;
                    orderData.postNoRequest = PickDataList[postIndex].postNo;
                    orderData.workCode = middle.workCode;
                    orderData.index = PickDataList[postIndex].index;
                    orderData.workName = middle.workName;
                    orderData.JANCode = middle.JANCode;
                    orderData.caseVolume = 0;
                    int orderTotalCount = middle.minimumRecordList.Select(x => x.count).Sum();
                    orderData.orderCountTotal = orderTotalCount;
                    orderData.workNameKana = middle.workNameKana;
                    orderData.maxStackNum = middle.max;
                    orderData.salesPrice = middle.price;
                    orderData.process = ORDER_PROCESS.UNLOAD;
                    for (int i = 0; i < Const.MaxStationCount; i++) 
                    {
                        orderData.orderCount[i] = middle.countList[i].count;
                        orderData.storeCount[i] = middle.countList[i].storeCount;
                    }
                    orderData.createDateTime = dt;
                    orderData.createLoginId = "admin";
                    //orderData.updateLoginId = IniFile.DBLoginId;



                    //// 総仕分済数量   ※ここでtrue=>falseと書き換えるのはNG。データベースに書き込まれなくなる
                    //if (!middle.isChanged)
                    //    middle.isChanged = (middle.orderCompCountTotal != middle.storeDataList.Select(x => x.orderCompCount).Sum());
                    //middle.orderCompCountTotal = middle.storeDataList.Select(x => x.orderCompCount).Sum();


                    // 商品ヘッダテーブルへ1行書き込み
                    rc = dbIF.SetOrderItemFromPickData(workHeader_TableName, orderData);


                    // ------------------------------
                    // 店別小仕分けテーブル
                    // ------------------------------
                    foreach (var minimum in middle.minimumRecordList) 
                    {
                        OrderStoreData storeData = new OrderStoreData();

                        storeData.orderDate = PickDataList[postIndex].orderDate.AddDays(-1);
                        storeData.postNo = PickDataList[postIndex].postNo;
                        storeData.orderDateRequest = PickDataList[postIndex].orderDate;
                        storeData.postNoRequest = PickDataList[postIndex].postNo;
                        storeData.workCode = middle.workCode;
                        storeData.index = PickDataList[postIndex].index;


                        int stationNo = ((minimum.aisleNo - 1) / 3) + 1;
                        int aisleNo = minimum.aisleNo;
                        int slotNo = minimum.slotNo;


                        // 店マスター連携処理
                        MasterStore masterStore = masterStoreList.Find(x => x.postInfo[postIndex].station == stationNo &&
                                                                            x.postInfo[postIndex].aisle == minimum.aisleNo &&
                                                                            x.postInfo[postIndex].slot == minimum.slotNo);
                        string storeCode = masterStore.storeCode;



                        storeData.stationNo = stationNo;
                        storeData.aisleNo = minimum.aisleNo;
                        storeData.slotNo = minimum.slotNo;

                        storeData.storeCode = masterStore.storeCode;
                        storeData.caseVolume = 0;
                        storeData.orderCount = minimum.count;
                        storeData.createDateTime = dt;
                        storeData.createLoginId = "admin";

                        // 店別小仕分けテーブルへ1行書き込み
                        rc = dbIF.SetOrderStoreItemsFromPickData(storeOrder_TableName, storeData);
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }












        /// <summary>
        /// ディレクトリコピーメソッド
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        public UInt32 DirectoryCopy(string sourcePath, string destinationPath)
        {
            UInt32 rc = 0;
            try
            {
                DirectoryInfo sourceDirectory = new DirectoryInfo(sourcePath);
                DirectoryInfo destinationDirectory = new DirectoryInfo(destinationPath);

                //コピー先のディレクトリがなければ作成する
                if (destinationDirectory.Exists == false)
                {
                    destinationDirectory.Create();
                    destinationDirectory.Attributes = sourceDirectory.Attributes;
                }

                //ファイルのコピー
                foreach (FileInfo fileInfo in sourceDirectory.GetFiles())
                {
                    //同じファイルが存在していたら、常に上書きする
                    fileInfo.CopyTo(destinationDirectory.FullName + @"\" + fileInfo.Name, true);
                }

                //ディレクトリのコピー（再帰を使用）
                foreach (System.IO.DirectoryInfo directoryInfo in sourceDirectory.GetDirectories())
                {
                    DirectoryCopy(directoryInfo.FullName, destinationDirectory.FullName + @"\" + directoryInfo.Name);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

    }
}
