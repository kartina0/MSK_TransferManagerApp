//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows;

using SystemConfig;
using DL_Logger;
using ErrorCodeDefine;


namespace ServerModule
{
    /// <summary>
    /// 上位サーバーのマスタファイル管理
    /// </summary>
    public class MasterFileManager
    {
        private const string THIS_NAME = "MasterFileManager";

        /// <summary>
        /// IPアドレス
        /// </summary>
        private string _ipAddress = "";

        /// <summary>
        /// 商品マスタリスト
        /// 1~3便
        /// </summary>
        public List<MasterWork>[] MasterWorkList = null;
        /// <summary>
        /// 店マスタリスト
        /// </summary>
        public List<MasterStore> MasterStoreList = new List<MasterStore>();
        /// <summary>
        /// 作業者マスタリスト
        /// </summary>
        public List<MasterWorker> MasterWorkerList = new List<MasterWorker>();

        /// <summary>
        /// 商品マスター ファイルパス (サーバー)
        /// </summary>
        private string _masterWorkFilePathServer = "";
        /// <summary>
        /// 店マスター ファイルパス (サーバー)
        /// </summary>
        private string _masterStoreFilePathServer = "";
        /// <summary>
        /// 作業者マスター ファイルパス (サーバー)
        /// </summary>
        private string _masterWorkerFilePathServer = "";

        /// <summary>
        /// 商品マスター ファイルパス (ローカル)
        /// </summary>
        private string _masterWorkFilePathLocal = "";
        /// <summary>
        /// 店マスター ファイルパス (ローカル)
        /// </summary>
        private string _masterStoreFilePathLocal = "";
        /// <summary>
        /// 作業者マスター ファイルパス (ローカル)
        /// </summary>
        private string _masterWorkerFilePathLocal = "";

        ///// <summary>
        ///// 商品マスター 最終更新日時 (サーバー)
        ///// </summary>
        //private DateTime _latestUpdatedDtServerWork = DateTime.MinValue;
        ///// <summary>
        ///// 店マスター 最終更新日時 (サーバー)
        ///// </summary>
        //private DateTime _latestUpdatedDtServerStore = DateTime.MinValue;
        ///// <summary>
        ///// 作業者マスター 最終更新日時 (サーバー)
        ///// </summary>
        //private DateTime _latestUpdatedDtServerWorker = DateTime.MinValue;

        ///// <summary>
        ///// 商品マスター 最終更新日時 (ローカル)
        ///// </summary>
        //private DateTime _latestUpdatedDtWork = DateTime.MinValue;
        ///// <summary>
        ///// 店マスター 最終更新日時 (ローカル)
        ///// </summary>
        //private DateTime _latestUpdatedDtStore = DateTime.MinValue;
        ///// <summary>
        ///// 作業者マスター 最終更新日時 (ローカル)
        ///// </summary>
        //private DateTime _latestUpdatedDtWorker = DateTime.MinValue;


        #region 商品マスター フォーマット
        /// <summary> 開始位置 商品マスター 納品日 </summary>
        private const int _startAddr_Work_OrderDate = 0;
        /// <summary> 開始位置 商品マスター 取引先コード </summary>
        private const int _startAddr_Work_SupplierCode = 8;
        /// <summary> 開始位置 商品マスター VDRコード </summary>
        private const int _startAddr_Work_VDRCode = 14;
        /// <summary> 開始位置 商品マスター 取引先名(漢字) </summary>
        private const int _startAddr_Work_SupplierName = 20;
        /// <summary> 開始位置 商品マスター 商品コード </summary>
        private const int _startAddr_Work_WorkCode = 70;
        /// <summary> 開始位置 商品マスター 便 </summary>
        private const int _startAddr_Work_PostNo = 76;
        /// <summary> 開始位置 商品マスター JANコード </summary>
        private const int _startAddr_Work_JANCode = 77;
        /// <summary> 開始位置 商品マスター JAN区分 </summary>
        private const int _startAddr_Work_JANClass = 90;
        /// <summary> 開始位置 商品マスター 商品名(漢字) </summary>
        private const int _startAddr_Work_WorkName = 92;
        /// <summary> 開始位置 商品マスター DEPT-CLASS </summary>
        private const int _startAddr_Work_DeptClass = 136;
        /// <summary> 開始位置 商品マスター センター入数 </summary>
        private const int _startAddr_Work_CenterCount = 161;
        /// <summary> 開始位置 商品マスター パック入数 </summary>
        private const int _startAddr_Work_PackCount = 166;
        /// <summary> 開始位置 商品マスター JANコード(下4桁) </summary>
        private const int _startAddr_Work_JANCode4Digits = 268;
        /// <summary> 開始位置 商品マスター 商品名(カナ) </summary>
        private const int _startAddr_Work_WorkNameKana = 272;

        /// <summary> データ長 商品マスター 全体 </summary>
        private const int _dataSize_Work_Total = 294;
        /// <summary> データ長 商品マスター 納品日 </summary>
        private const int _dataSize_Work_OrderDate = 8;
        /// <summary> データ長 商品マスター 取引先コード </summary>
        private const int _dataSize_Work_SupplierCode = 6;
        /// <summary> データ長 商品マスター VDRコード </summary>
        private const int _dataSize_Work_VDRCode = 6;
        /// <summary> データ長 商品マスター 取引先名(漢字) </summary>
        private const int _dataSize_Work_SupplierName = 50;
        /// <summary> データ長 商品マスター 商品コード </summary>
        private const int _dataSize_Work_WorkCode = 6;
        /// <summary> データ長 商品マスター 便 </summary>
        private const int _dataSize_Work_PostNo = 1;
        /// <summary> データ長 商品マスター JANコード </summary>
        private const int _dataSize_Work_JANCode = 13;
        /// <summary> データ長 商品マスター JAN区分 </summary>
        private const int _dataSize_Work_JANClass = 2;
        /// <summary> データ長 商品マスター 商品名(漢字) </summary>
        private const int _dataSize_Work_WorkName = 44;
        /// <summary> データ長 商品マスター DEPT-CLASS </summary>
        private const int _dataSize_Work_DeptClass = 5;
        /// <summary> データ長 商品マスター センター入数 </summary>
        private const int _dataSize_Work_CenterCount = 5;
        /// <summary> データ長 商品マスター パック入数 </summary>
        private const int _dataSize_Work_PackCount = 5;
        /// <summary> データ長 商品マスター JANコード(下4桁) </summary>
        private const int _dataSize_Work_JANCode4Digits = 4;
        /// <summary> データ長 商品マスター 商品名(カナ) </summary>
        private const int _dataSize_Work_WorkNameKana = 22;
        #endregion

        #region 店マスター フォーマット
        /// <summary> 開始位置 店マスター 納品日 </summary>
        private const int _startAddr_Store_OrderDate = 0;
        /// <summary> 開始位置 店マスター 社区分 </summary>
        private const int _startAddr_Store_CompanyType = 8;
        /// <summary> 開始位置 店マスター 店コード </summary>
        private const int _startAddr_Store_StoreCode = 10;
        /// <summary> 開始位置 店マスター 店名(漢字) </summary>
        private const int _startAddr_Store_StoreName = 16;
        /// <summary> 開始位置 店マスター 電話番号 </summary>
        private const int _startAddr_Store_PhoneNumber = 66;

        /// <summary> 開始位置 店マスター 1便 コース </summary>
        private const int _startAddr_Store_Course_01 = 82;
        /// <summary> 開始位置 店マスター 1便 順 </summary>
        private const int _startAddr_Store_Process_01 = 85;
        /// <summary> 開始位置 店マスター 1便 ステーション </summary>
        private const int _startAddr_Store_Station_01 = 87;
        /// <summary> 開始位置 店マスター 1便 アイル </summary>
        private const int _startAddr_Store_Aisle_01 = 88;
        /// <summary> 開始位置 店マスター 1便 スロット </summary>
        private const int _startAddr_Store_Slot_01 = 90;
        /// <summary> 開始位置 店マスター 1便 ドッグNo </summary>
        private const int _startAddr_Store_DogNo_01 = 92;
        /// <summary> 開始位置 店マスター 1便 搬入条件 </summary>
        private const int _startAddr_Store_Condition_01 = 95;
        /// <summary> 開始位置 店マスター 1便 到着 </summary>
        private const int _startAddr_Store_TimeArrive_01 = 97;
        /// <summary> 開始位置 店マスター 1便 入場 </summary>
        private const int _startAddr_Store_TimeEntry_01 = 101;
        /// <summary> 開始位置 店マスター 1便 出発 </summary>
        private const int _startAddr_Store_TimeDepart_01 = 105;
        /// <summary> 開始位置 店マスター 1便 終了 </summary>
        private const int _startAddr_Store_TimeFinish_01 = 109;
        /// <summary> 開始位置 店マスター 1便 運送会社 </summary>
        private const int _startAddr_Store_Company_01 = 113;
        /// <summary> 開始位置 店マスター 1便 運送会社名 </summary>
        private const int _startAddr_Store_CompanyName_01 = 117;
        /// <summary> 開始位置 店マスター 1便 車種 </summary>
        private const int _startAddr_Store_CarType_01 = 137;

        /// <summary> 開始位置 店マスター 2便 コース </summary>
        private const int _startAddr_Store_Course_02 = 141;
        /// <summary> 開始位置 店マスター 2便 順 </summary>
        private const int _startAddr_Store_Process_02 = 144;
        /// <summary> 開始位置 店マスター 2便 ステーション </summary>
        private const int _startAddr_Store_Station_02 = 146;
        /// <summary> 開始位置 店マスター 2便 アイル </summary>
        private const int _startAddr_Store_Aisle_02 = 147;
        /// <summary> 開始位置 店マスター 2便 スロット </summary>
        private const int _startAddr_Store_Slot_02 = 149;
        /// <summary> 開始位置 店マスター 2便 ドッグNo </summary>
        private const int _startAddr_Store_DogNo_02 = 151;
        /// <summary> 開始位置 店マスター 2便 搬入条件 </summary>
        private const int _startAddr_Store_Condition_02 = 154;
        /// <summary> 開始位置 店マスター 2便 到着 </summary>
        private const int _startAddr_Store_TimeArrive_02 = 156;
        /// <summary> 開始位置 店マスター 2便 入場 </summary>
        private const int _startAddr_Store_TimeEntry_02 = 160;
        /// <summary> 開始位置 店マスター 2便 出発 </summary>
        private const int _startAddr_Store_TimeDepart_02 = 164;
        /// <summary> 開始位置 店マスター 2便 終了 </summary>
        private const int _startAddr_Store_TimeFinish_02 = 168;
        /// <summary> 開始位置 店マスター 2便 運送会社 </summary>
        private const int _startAddr_Store_Company_02 = 172;
        /// <summary> 開始位置 店マスター 2便 運送会社名 </summary>
        private const int _startAddr_Store_CompanyName_02 = 176;
        /// <summary> 開始位置 店マスター 2便 車種 </summary>
        private const int _startAddr_Store_CarType_02 = 196;

        /// <summary> 開始位置 店マスター 3便 コース </summary>
        private const int _startAddr_Store_Course_03 = 200;
        /// <summary> 開始位置 店マスター 3便 順 </summary>
        private const int _startAddr_Store_Process_03 = 203;
        /// <summary> 開始位置 店マスター 3便 ステーション </summary>
        private const int _startAddr_Store_Station_03 = 205;
        /// <summary> 開始位置 店マスター 3便 アイル </summary>
        private const int _startAddr_Store_Aisle_03 = 206;
        /// <summary> 開始位置 店マスター 3便 スロット </summary>
        private const int _startAddr_Store_Slot_03 = 208;
        /// <summary> 開始位置 店マスター 3便 ドッグNo </summary>
        private const int _startAddr_Store_DogNo_03 = 210;
        /// <summary> 開始位置 店マスター 3便 搬入条件 </summary>
        private const int _startAddr_Store_Condition_03 = 213;
        /// <summary> 開始位置 店マスター 3便 到着 </summary>
        private const int _startAddr_Store_TimeArrive_03 = 215;
        /// <summary> 開始位置 店マスター 3便 入場 </summary>
        private const int _startAddr_Store_TimeEntry_03 = 219;
        /// <summary> 開始位置 店マスター 3便 出発 </summary>
        private const int _startAddr_Store_TimeDepart_03 = 223;
        /// <summary> 開始位置 店マスター 3便 終了 </summary>
        private const int _startAddr_Store_TimeFinish_03 = 227;
        /// <summary> 開始位置 店マスター 3便 運送会社 </summary>
        private const int _startAddr_Store_Company_03 = 231;
        /// <summary> 開始位置 店マスター 3便 運送会社名 </summary>
        private const int _startAddr_Store_CompanyName_03 = 235;
        /// <summary> 開始位置 店マスター 3便 車種 </summary>
        private const int _startAddr_Store_CarType_03 = 255;

        /// <summary> データ長 店マスター 全体 </summary>
        //private const int _dataSize_Store_Total = 259;
        private const int _dataSize_Store_Total = 262;
        /// <summary> データ長 店マスター 納品日 </summary>
        private const int _dataSize_Store_OrderDate = 8;
        /// <summary> データ長 店マスター 社区分 </summary>
        private const int _dataSize_Store_CompanyType = 2;
        /// <summary> データ長 店マスター 店コード </summary>
        private const int _dataSize_Store_StoreCode = 6;
        /// <summary> データ長 店マスター 店名(漢字) </summary>
        private const int _dataSize_Store_StoreName = 50;
        /// <summary> データ長 店マスター 電話番号 </summary>
        private const int _dataSize_Store_PhoneNumber = 16;
        /// <summary> データ長 店マスター コース </summary>
        private const int _dataSize_Store_Course = 3;
        /// <summary> データ長 店マスター 順 </summary>
        private const int _dataSize_Store_Process = 2;
        /// <summary> データ長 店マスター ステーション </summary>
        private const int _dataSize_Store_Station = 1;
        /// <summary> データ長 店マスター アイル </summary>
        private const int _dataSize_Store_Aisle = 2;
        /// <summary> データ長 店マスター スロット </summary>
        private const int _dataSize_Store_Slot = 2;
        /// <summary> データ長 店マスター ドッグNo </summary>
        private const int _dataSize_Store_DogNo = 3;
        /// <summary> データ長 店マスター 搬入条件 </summary>
        private const int _dataSize_Store_Condition = 2;
        /// <summary> データ長 店マスター 到着 </summary>
        private const int _dataSize_Store_TimeArrive = 4;
        /// <summary> データ長 店マスター 入場 </summary>
        private const int _dataSize_Store_TimeEntry = 4;
        /// <summary> データ長 店マスター 出発 </summary>
        private const int _dataSize_Store_TimeDepart = 4;
        /// <summary> データ長 店マスター 終了 </summary>
        private const int _dataSize_Store_TimeFinish = 4;
        /// <summary> データ長 店マスター 運送会社 </summary>
        private const int _dataSize_Store_Company = 4;
        /// <summary> データ長 店マスター 運送会社名 </summary>
        private const int _dataSize_Store_CompanyName = 20;
        /// <summary> データ長 店マスター 車種 </summary>
        private const int _dataSize_Store_CarType = 4;
        #endregion

        #region 作業者マスター フォーマット
        /// <summary> 開始位置 作業者マスター 作業者No(代表) </summary>
        private const int _startAddr_Worker_WorkerChiefNo = 0;
        /// <summary> 開始位置 作業者マスター 作業者名(代表) </summary>
        private const int _startAddr_Worker_WorkerChiefName = 3;
        /// <summary> 開始位置 作業者マスター 1便 作業者No </summary>
        private const int _startAddr_Worker_WorkerNo_01 = 53;
        /// <summary> 開始位置 作業者マスター 2便 作業者No </summary>
        private const int _startAddr_Worker_WorkerNo_02 = 56;
        /// <summary> 開始位置 作業者マスター 3便 作業者No </summary>
        private const int _startAddr_Worker_WorkerNo_03 = 59;
        /// <summary> 開始位置 作業者マスター 当日3便 作業者No </summary>
        private const int _startAddr_Worker_WorkerNoActual_03 = 62;

        /// <summary> データ長 作業者マスター 全体 </summary>
        private const int _dataSize_Worker_Total = 70;
        /// <summary> データ長 作業者マスター 作業者No(代表) </summary>
        private const int _dataSize_Worker_WorkerChiefNo = 3;
        /// <summary> データ長 作業者マスター 作業者名(代表) </summary>
        private const int _dataSize_Worker_WorkerChiefName = 50;
        /// <summary> データ長 作業者マスター 作業者No </summary>
        private const int _dataSize_Worker_WorkerNo = 3;
        #endregion



        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MasterFileManager()
        {
            try
            {
                _ipAddress = IniFile.DBIpAddress;

                MasterWorkList = new List<MasterWork>[Const.MaxPostCount];
                for (int i = 0; i < Const.MaxPostCount; i++)
                    MasterWorkList[i] = new List<MasterWork>();

                _masterWorkFilePathServer = Path.Combine(IniFile.PickdataServerDir, IniFile.MasterWorkFilePath);
                _masterStoreFilePathServer = Path.Combine(IniFile.PickdataServerDir, IniFile.MasterStoreFilePath);
                _masterWorkerFilePathServer = Path.Combine(IniFile.PickdataServerDir, IniFile.MasterWorkerFilePath);

                _masterWorkFilePathLocal = Path.Combine(IniFile.PickdataLocalDir, IniFile.MasterWorkFilePath);
                _masterStoreFilePathLocal = Path.Combine(IniFile.PickdataLocalDir, IniFile.MasterStoreFilePath);
                _masterWorkerFilePathLocal = Path.Combine(IniFile.PickdataLocalDir, IniFile.MasterWorkerFilePath);

            }
            catch (Exception ex)
            {
            }
        }


        #region 商品マスター処理
        /// <summary>
        /// 商品マスタファイルをローカルにコピー
        /// </summary>
        /// <returns></returns>
        public UInt32 CopyMasterWorkFile(out bool isTodaysFileLocal)
        {
            UInt32 rc = 0;
            isTodaysFileLocal = false;
            try
            {
                // -----------------------------------------------
                // サーバーとローカルのファイル情報を確認
                // -----------------------------------------------
                // サーバーのファイルの存在チェック
                if (!File.Exists(_masterWorkFilePathServer))
                {
                    rc = (UInt32)ErrorCodeList.PICKDATA_FILE_IS_NOT_EXIST_SERVER;
                    return rc;
                }

                // サーバーのファイルが当日のものかチェック
                bool isTodaysFileServer = false;
                DateTime currentDt = DateTime.Now;
                DateTime fileDt = File.GetLastWriteTime(_masterWorkFilePathServer);
                if (currentDt < DateTime.Today.AddHours(IniFile.DateChangeTime.Hour).AddMinutes(IniFile.DateChangeTime.Minute))
                {// 日付切り替わり時刻前 => ファイルの最終更新日が前日ならばOK
                    if (fileDt.Date == DateTime.Today.AddDays(-1).Date)
                    {// 最終更新日が前日
                        isTodaysFileServer = true;
                    }
                    else
                    {// 最終更新日が前日ではない
                        string message = $"サーバーの商品マスタファイルが本日のものではありません。";
                        Logger.WriteLog(LogType.ERROR, message);
                    }
                }
                else
                {// 日付切り替わり時刻後 = 最終更新日が当日のファイルならばOK
                    if (fileDt.Date == DateTime.Today.Date)
                    {// 最終更新日が当日
                        isTodaysFileServer = true;
                    }
                    else
                    {// 最終更新日が当日ではない
                        string message = $"サーバーの商品マスタファイルが本日のものではありません。";
                        Logger.WriteLog(LogType.ERROR, message);
                    }
                }

                // ローカルのファイルの存在チェック
                bool localExist = false;
                if (File.Exists(_masterWorkFilePathLocal))
                    localExist = true;


                // -----------------------------------------------
                // サーバーとローカルを比較してコピー
                // -----------------------------------------------
                bool serverUpdated = false;
                if (isTodaysFileServer)
                {// サーバーに本日のファイルがある

                    // サーバーとローカルのファイルの更新日時を比較
                    if (localExist)
                    {
                        DateTime latestUpdatedDtServer = File.GetLastWriteTime(_masterWorkFilePathServer);
                        DateTime latestUpdatedDtLocal = File.GetLastWriteTime(_masterWorkFilePathLocal);
                        if (latestUpdatedDtServer != latestUpdatedDtLocal)
                            serverUpdated = true;
                    }

                    // サーバーからローカルにファイルをコピー
                    if (!localExist || serverUpdated)
                    {
                        // temp0フォルダがなければ作成
                        string dir = Path.GetDirectoryName(_masterWorkFilePathLocal);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        // ファイルをコピー
                        File.Copy(_masterWorkFilePathServer, _masterWorkFilePathLocal, true);
                    }
                }


                // -----------------------------------------------
                // 最終的にローカルのファイルが当日のものかチェック
                // -----------------------------------------------
                localExist = false;
                if (File.Exists(_masterWorkFilePathLocal))
                    localExist = true;
                if (localExist)
                {
                    currentDt = DateTime.Now;
                    fileDt = File.GetLastWriteTime(_masterWorkFilePathLocal);
                    if (currentDt < DateTime.Today.AddHours(IniFile.DateChangeTime.Hour).AddMinutes(IniFile.DateChangeTime.Minute))
                    {// 日付切り替わり時刻前 => ファイルの最終更新日が前日ならばOK
                        if (fileDt.Date == DateTime.Today.AddDays(-1).Date)
                        {// 最終更新日が前日
                            isTodaysFileLocal = true;
                        }
                        else
                        {// 最終更新日が前日ではない
                            string message = $"ローカルの商品マスタファイルが本日のものではありません。";
                            Logger.WriteLog(LogType.ERROR, message);
                        }
                    }
                    else
                    {// 日付切り替わり時刻後 = 最終更新日が当日のファイルならばOK
                        if (fileDt.Date == DateTime.Today.Date)
                        {// 最終更新日が当日
                            isTodaysFileLocal = true;
                        }
                        else
                        {// 最終更新日が当日ではない
                            string message = $"ローカルの商品マスタファイルが本日のものではありません。";
                            Logger.WriteLog(LogType.ERROR, message);
                        }
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
        /// 商品マスタファイル 読み出し
        /// </summary>
        /// <returns>エラーコード</returns>
        public UInt32 ReadMasterWork()
        {
            UInt32 rc = 0;
            try
            {

                // -----------------------------------------------
                // 商品マスター読み出し
                // -----------------------------------------------
                int index = 0;
                byte[] byteData = null;
                int errorCount = 0;
                // リストをクリア
                for (int i = 0; i < Const.MaxPostCount; i++)
                    MasterWorkList[i].Clear();
                //ファイルを開く
                using (System.IO.FileStream fs = new System.IO.FileStream(_masterWorkFilePathLocal, System.IO.FileMode.Open, System.IO.FileAccess.Read)) 
                {
                    // 商品数取得
                    int count = (int)(fs.Length / _dataSize_Work_Total);
                    // 商品数分ループ
                    for (int i = 0; i < count; i++) 
                    {
                        byteData = null;
                        byteData = new byte[_dataSize_Work_Total];
                        // 読み出し開始位置を設定
                        fs.Seek(i * _dataSize_Work_Total, SeekOrigin.Begin);
                        // 1商品分のデータを読み出し
                        fs.Read(byteData, 0, _dataSize_Work_Total);


                        // データ変換
                        MasterWork work = new MasterWork();
                        string text = "";
                        byte[] b = null;
                        // 納品日
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Work_OrderDate];
                            Array.Copy(byteData, _startAddr_Work_OrderDate, b, 0, b.Length);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            work.orderDate = DateTime.ParseExact(text + "000000", "yyyyMMddHHmmss", null);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [納品日] エラー");
                            errorCount++;
                            continue;
                        }
                        // 取引先コード
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Work_SupplierCode];
                            Array.Copy(byteData, _startAddr_Work_SupplierCode, b, 0, b.Length);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            work.supplierCode = text;
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [取引先コード] エラー");
                            errorCount++;
                            continue;
                        }
                        // VDRコード
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Work_VDRCode];
                            Array.Copy(byteData, _startAddr_Work_VDRCode, b, 0, b.Length);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            work.VDRCode = text;
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [VDRコード] エラー");
                            errorCount++;
                            continue;
                        }
                        // 取引先名(漢字)
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Work_SupplierName];
                            Array.Copy(byteData, _startAddr_Work_SupplierName, b, 0, b.Length);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            work.supplierName = text;
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [取引先名(漢字)] エラー");
                            errorCount++;
                            continue;
                        }
                        // 商品コード
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Work_WorkCode];
                            Array.Copy(byteData, _startAddr_Work_WorkCode, b, 0, b.Length);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            work.workCode = text;
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [商品コード] エラー");
                            errorCount++;
                            continue;
                        }
                        // 便
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Work_PostNo];
                            Array.Copy(byteData, _startAddr_Work_PostNo, b, 0, b.Length);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            work.postNo = int.Parse(text);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [便] エラー");
                            errorCount++;
                            continue;
                        }
                        // JANコード
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Work_JANCode];
                            Array.Copy(byteData, _startAddr_Work_JANCode, b, 0, b.Length);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            work.JANCode = text;
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [JANコード] エラー");
                            errorCount++;
                            continue;
                        }
                        // JAN区分
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Work_JANClass];
                            Array.Copy(byteData, _startAddr_Work_JANClass, b, 0, b.Length);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            work.JANClass = int.Parse(text);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [JAN区分] エラー");
                            errorCount++;
                            continue;
                        }
                        // 商品名(漢字)
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Work_WorkName];
                            Array.Copy(byteData, _startAddr_Work_WorkName, b, 0, b.Length);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            work.workName = text;
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [商品名(漢字)] エラー");
                            errorCount++;
                            continue;
                        }
                        // DEPT-CLASS
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Work_DeptClass];
                            Array.Copy(byteData, _startAddr_Work_DeptClass, b, 0, b.Length);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            work.deptClass = text;
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [DEPT-CLASS] エラー");
                            errorCount++;
                            continue;
                        }
                        // センター入数
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Work_CenterCount];
                            Array.Copy(byteData, _startAddr_Work_CenterCount, b, 0, b.Length);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            work.centerCount = double.Parse(text);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [センター入数] エラー");
                            errorCount++;
                            continue;
                        }
                        // パック数
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Work_PackCount];
                            Array.Copy(byteData, _startAddr_Work_PackCount, b, 0, b.Length);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            work.packCount = double.Parse(text);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [パック入数] エラー");
                            errorCount++;
                            continue;
                        }
                        // JAN下4桁
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Work_JANCode4Digits];
                            Array.Copy(byteData, _startAddr_Work_JANCode4Digits, b, 0, b.Length);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            work.JANCode4digits = text;
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [JAN下4桁] エラー");
                            errorCount++;
                            continue;
                        }
                        // 商品名(カナ)
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Work_WorkNameKana];
                            Array.Copy(byteData, _startAddr_Work_WorkNameKana, b, 0, b.Length);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            work.workNameKana = text;
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [商品名(ｶﾅ)] エラー");
                            errorCount++;
                            continue;
                        }

                        // リストに追加
                        if(1 <= work.postNo && work.postNo <= 3)
                            MasterWorkList[work.postNo - 1].Add(work);
                    }
                }

                // エラー発生行数を表示
                if (errorCount > 0) 
                {
                    string message = $"商品マスターファイル読み込みエラー エラー発生行数合計: {errorCount}行";
                    Logger.WriteLog(LogType.ERROR, message);
                    MessageBox.Show(message, "確認", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.MASTER_WORK_FILE_READ_ERROR;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        #endregion


        #region 店マスター処理
        /// <summary>
        /// 店マスタファイルをローカルにコピー
        /// </summary>
        /// <returns></returns>
        public UInt32 CopyMasterStoreFile(out bool isTodaysFileLocal)
        {
            UInt32 rc = 0;
            isTodaysFileLocal = false;
            try
            {
                // -----------------------------------------------
                // サーバーとローカルのファイル情報を確認
                // -----------------------------------------------
                // サーバーのファイルの存在チェック
                if (!File.Exists(_masterStoreFilePathServer))
                {
                    rc = (UInt32)ErrorCodeList.PICKDATA_FILE_IS_NOT_EXIST_SERVER;
                    return rc;
                }

                // サーバーのファイルが当日のものかチェック
                bool isTodaysFileServer = false;
                DateTime currentDt = DateTime.Now;
                DateTime fileDt = File.GetLastWriteTime(_masterStoreFilePathServer);
                if (currentDt < DateTime.Today.AddHours(IniFile.DateChangeTime.Hour).AddMinutes(IniFile.DateChangeTime.Minute))
                {// 日付切り替わり時刻前 => ファイルの最終更新日が前日ならばOK
                    if (fileDt.Date == DateTime.Today.AddDays(-1).Date)
                    {// 最終更新日が前日
                        isTodaysFileServer = true;
                    }
                    else
                    {// 最終更新日が前日ではない
                        string message = $"サーバーの店マスタファイルが本日のものではありません。";
                        Logger.WriteLog(LogType.ERROR, message);
                    }
                }
                else
                {// 日付切り替わり時刻後 = 最終更新日が当日のファイルならばOK
                    if (fileDt.Date == DateTime.Today.Date)
                    {// 最終更新日が当日
                        isTodaysFileServer = true;
                    }
                    else
                    {// 最終更新日が当日ではない
                        string message = $"サーバーの店マスタファイルが本日のものではありません。";
                        Logger.WriteLog(LogType.ERROR, message);
                    }
                }

                // ローカルのファイルの存在チェック
                bool localExist = false;
                if (File.Exists(_masterStoreFilePathLocal))
                    localExist = true;


                // -----------------------------------------------
                // サーバーとローカルを比較してコピー
                // -----------------------------------------------
                bool serverUpdated = false;
                if (isTodaysFileServer)
                {// サーバーに本日のファイルがある

                    // サーバーとローカルのファイルの更新日時を比較
                    if (localExist)
                    {
                        DateTime latestUpdatedDtServer = File.GetLastWriteTime(_masterStoreFilePathServer);
                        DateTime latestUpdatedDtLocal = File.GetLastWriteTime(_masterStoreFilePathLocal);
                        if (latestUpdatedDtServer != latestUpdatedDtLocal)
                            serverUpdated = true;
                    }

                    // サーバーからローカルにファイルをコピー
                    if (!localExist || serverUpdated)
                    {
                        // temp0フォルダがなければ作成
                        string dir = Path.GetDirectoryName(_masterStoreFilePathLocal);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        // ファイルをコピー
                        File.Copy(_masterStoreFilePathServer, _masterStoreFilePathLocal, true);
                    }
                }


                // -----------------------------------------------
                // 最終的にローカルのファイルが当日のものかチェック
                // -----------------------------------------------
                localExist = false;
                if (File.Exists(_masterStoreFilePathLocal))
                    localExist = true;
                if (localExist)
                {
                    currentDt = DateTime.Now;
                    fileDt = File.GetLastWriteTime(_masterStoreFilePathLocal);
                    if (currentDt < DateTime.Today.AddHours(IniFile.DateChangeTime.Hour).AddMinutes(IniFile.DateChangeTime.Minute))
                    {// 日付切り替わり時刻前 => ファイルの最終更新日が前日ならばOK
                        if (fileDt.Date == DateTime.Today.AddDays(-1).Date)
                        {// 最終更新日が前日
                            isTodaysFileLocal = true;
                        }
                        else
                        {// 最終更新日が前日ではない
                            string message = $"ローカルの店マスタファイルが本日のものではありません。";
                            Logger.WriteLog(LogType.ERROR, message);
                        }
                    }
                    else
                    {// 日付切り替わり時刻後 = 最終更新日が当日のファイルならばOK
                        if (fileDt.Date == DateTime.Today.Date)
                        {// 最終更新日が当日
                            isTodaysFileLocal = true;
                        }
                        else
                        {// 最終更新日が当日ではない
                            string message = $"ローカルの店マスタファイルが本日のものではありません。";
                            Logger.WriteLog(LogType.ERROR, message);
                        }
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
        /// 店マスタファイル 読み出し
        /// </summary>
        /// <returns>エラーコード</returns>
        public UInt32 ReadMasterStore()
        {
            UInt32 rc = 0;
            try
            {

                // -----------------------------------------------
                // 店マスター読み出し
                // -----------------------------------------------
                int index = 0;
                byte[] byteData = null;
                int errorCount = 0;
                // リストをクリア
                MasterStoreList.Clear();
                //ファイルを開く
                using (System.IO.FileStream fs = new System.IO.FileStream(_masterStoreFilePathLocal, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    // 商品数取得
                    int count = (int)(fs.Length / _dataSize_Store_Total);
                    // 商品数分ループ
                    for (int i = 0; i < count; i++)
                    {
                        byteData = null;
                        byteData = new byte[_dataSize_Store_Total];
                        // 読み出し開始位置を設定
                        fs.Seek(i * _dataSize_Store_Total, SeekOrigin.Begin);
                        // 1商品分のデータを読み出し
                        fs.Read(byteData, 0, _dataSize_Store_Total);


                        // データ変換
                        MasterStore store = new MasterStore();
                        string text = "";
                        byte[] b = null;
                        // 納品日
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_OrderDate];
                            Array.Copy(byteData, _startAddr_Store_OrderDate, b, 0, b.Length);
                            text = Encoding.GetEncoding("shift_jis").GetString(b);
                            store.orderDate = DateTime.ParseExact(text + "000000", "yyyyMMddHHmmss", null);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [納品日] エラー");
                            errorCount++;
                            continue;
                        }
                        // 社区分コード
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_CompanyType];
                            Array.Copy(byteData, _startAddr_Store_CompanyType, b, 0, b.Length);
                            store.companyType = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [社区分コード] エラー");
                            errorCount++;
                            continue;
                        }
                        // 店コード
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_StoreCode];
                            Array.Copy(byteData, _startAddr_Store_StoreCode, b, 0, b.Length);
                            store.storeCode = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [店コード] エラー");
                            errorCount++;
                            continue;
                        }
                        // 店名(漢字)
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_StoreName];
                            Array.Copy(byteData, _startAddr_Store_StoreName, b, 0, b.Length);
                            store.storeName = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [店名(漢字)] エラー");
                            errorCount++;
                            continue;
                        }
                        // 電話番号
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_PhoneNumber];
                            Array.Copy(byteData, _startAddr_Store_PhoneNumber, b, 0, b.Length);
                            store.phoneNumber = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [電話番号] エラー");
                            errorCount++;
                            continue;
                        }

                        // 1便
                        // コース
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Course];
                            Array.Copy(byteData, _startAddr_Store_Course_01, b, 0, b.Length);
                            store.postInfo[0].course = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [1便 コース] エラー");
                            errorCount++;
                            continue;
                        }
                        // 順
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Process];
                            Array.Copy(byteData, _startAddr_Store_Process_01, b, 0, b.Length);
                            store.postInfo[0].process = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [1便 順] エラー");
                            errorCount++;
                            continue;
                        }
                        // ステーション
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Station];
                            Array.Copy(byteData, _startAddr_Store_Station_01, b, 0, b.Length);
                            store.postInfo[0].station = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [1便 ステーション] エラー");
                            errorCount++;
                            continue;
                        }
                        // アイル
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Aisle];
                            Array.Copy(byteData, _startAddr_Store_Aisle_01, b, 0, b.Length);
                            store.postInfo[0].aisle = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [1便 アイル] エラー");
                            errorCount++;
                            continue;
                        }
                        // スロット
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Slot];
                            Array.Copy(byteData, _startAddr_Store_Slot_01, b, 0, b.Length);
                            store.postInfo[0].slot = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [1便 スロット] エラー");
                            errorCount++;
                            continue;
                        }
                        // ドッグNo
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_DogNo];
                            Array.Copy(byteData, _startAddr_Store_DogNo_01, b, 0, b.Length);
                            store.postInfo[0].dogNo = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [1便 ドッグNo] エラー");
                            errorCount++;
                            continue;
                        }
                        // 搬入条件
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Condition];
                            Array.Copy(byteData, _startAddr_Store_Condition_01, b, 0, b.Length);
                            store.postInfo[0].condition = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [1便 搬入条件] エラー");
                            errorCount++;
                            continue;
                        }
                        // 到着
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_TimeArrive];
                            Array.Copy(byteData, _startAddr_Store_TimeArrive_01, b, 0, b.Length);
                            store.postInfo[0].timeArrive = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [1便 到着] エラー");
                            errorCount++;
                            continue;
                        }
                        // 入場
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_TimeEntry];
                            Array.Copy(byteData, _startAddr_Store_TimeEntry_01, b, 0, b.Length);
                            store.postInfo[0].timeEntry = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [1便 入場] エラー");
                            errorCount++;
                            continue;
                        }
                        // 出発
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_TimeDepart];
                            Array.Copy(byteData, _startAddr_Store_TimeDepart_01, b, 0, b.Length);
                            store.postInfo[0].timeDepart = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [1便 出発] エラー");
                            errorCount++;
                            continue;
                        }
                        // 終了
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_TimeFinish];
                            Array.Copy(byteData, _startAddr_Store_TimeFinish_01, b, 0, b.Length);
                            store.postInfo[0].timeFinish = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [1便 終了] エラー");
                            errorCount++;
                            continue;
                        }
                        // 運送会社
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Company];
                            Array.Copy(byteData, _startAddr_Store_Company_01, b, 0, b.Length);
                            store.postInfo[0].company = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [1便 運送会社] エラー");
                            errorCount++;
                            continue;
                        }
                        // 運送会社名
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_CompanyName];
                            Array.Copy(byteData, _startAddr_Store_CompanyName_01, b, 0, b.Length);
                            store.postInfo[0].companyName = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [1便 運送会社名] エラー");
                            errorCount++;
                            continue;
                        }
                        // 車種
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_CarType];
                            Array.Copy(byteData, _startAddr_Store_CarType_01, b, 0, b.Length);
                            store.postInfo[0].carType = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [1便 車種] エラー");
                            errorCount++;
                            continue;
                        }

                        // 2便
                        // コース
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Course];
                            Array.Copy(byteData, _startAddr_Store_Course_02, b, 0, b.Length);
                            store.postInfo[1].course = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [2便 コース] エラー");
                            errorCount++;
                            continue;
                        }
                        // 順
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Process];
                            Array.Copy(byteData, _startAddr_Store_Process_02, b, 0, b.Length);
                            store.postInfo[1].process = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [2便 順] エラー");
                            errorCount++;
                            continue;
                        }
                        // ステーション
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Station];
                            Array.Copy(byteData, _startAddr_Store_Station_02, b, 0, b.Length);
                            store.postInfo[1].station = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [2便 ステーション] エラー");
                            errorCount++;
                            continue;
                        }
                        // アイル
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Aisle];
                            Array.Copy(byteData, _startAddr_Store_Aisle_02, b, 0, b.Length);
                            store.postInfo[1].aisle = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [2便 アイル] エラー");
                            errorCount++;
                            continue;
                        }
                        // スロット
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Slot];
                            Array.Copy(byteData, _startAddr_Store_Slot_02, b, 0, b.Length);
                            store.postInfo[1].slot = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [2便 スロット] エラー");
                            errorCount++;
                            continue;
                        }
                        // ドッグNo
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_DogNo];
                            Array.Copy(byteData, _startAddr_Store_DogNo_02, b, 0, b.Length);
                            store.postInfo[1].dogNo = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [2便 ドッグNo] エラー");
                            errorCount++;
                            continue;
                        }
                        // 搬入条件
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Condition];
                            Array.Copy(byteData, _startAddr_Store_Condition_02, b, 0, b.Length);
                            store.postInfo[1].condition = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [2便 搬入条件] エラー");
                            errorCount++;
                            continue;
                        }
                        // 到着
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_TimeArrive];
                            Array.Copy(byteData, _startAddr_Store_TimeArrive_02, b, 0, b.Length);
                            store.postInfo[1].timeArrive = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [2便 到着] エラー");
                            errorCount++;
                            continue;
                        }
                        // 入場
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_TimeEntry];
                            Array.Copy(byteData, _startAddr_Store_TimeEntry_02, b, 0, b.Length);
                            store.postInfo[1].timeEntry = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [2便 入場] エラー");
                            errorCount++;
                            continue;
                        }
                        // 出発
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_TimeDepart];
                            Array.Copy(byteData, _startAddr_Store_TimeDepart_02, b, 0, b.Length);
                            store.postInfo[1].timeDepart = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [2便 出発] エラー");
                            errorCount++;
                            continue;
                        }
                        // 終了
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_TimeFinish];
                            Array.Copy(byteData, _startAddr_Store_TimeFinish_02, b, 0, b.Length);
                            store.postInfo[1].timeFinish = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [2便 終了] エラー");
                            errorCount++;
                            continue;
                        }
                        // 運送会社
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Company];
                            Array.Copy(byteData, _startAddr_Store_Company_02, b, 0, b.Length);
                            store.postInfo[1].company = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [2便 運送会社] エラー");
                            errorCount++;
                            continue;
                        }
                        // 運送会社名
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_CompanyName];
                            Array.Copy(byteData, _startAddr_Store_CompanyName_02, b, 0, b.Length);
                            store.postInfo[1].companyName = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [2便 運送会社名] エラー");
                            errorCount++;
                            continue;
                        }
                        // 車種
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_CarType];
                            Array.Copy(byteData, _startAddr_Store_CarType_02, b, 0, b.Length);
                            store.postInfo[1].carType = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [2便 車種] エラー");
                            errorCount++;
                            continue;
                        }

                        // 3便
                        // コース
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Course];
                            Array.Copy(byteData, _startAddr_Store_Course_03, b, 0, b.Length);
                            store.postInfo[2].course = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [3便 コース] エラー");
                            errorCount++;
                            continue;
                        }
                        // 順
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Process];
                            Array.Copy(byteData, _startAddr_Store_Process_03, b, 0, b.Length);
                            store.postInfo[2].process = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [3便 順] エラー");
                            errorCount++;
                            continue;
                        }
                        // ステーション
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Station];
                            Array.Copy(byteData, _startAddr_Store_Station_03, b, 0, b.Length);
                            store.postInfo[2].station = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [3便 ステーション] エラー");
                            errorCount++;
                            continue;
                        }
                        // アイル
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Aisle];
                            Array.Copy(byteData, _startAddr_Store_Aisle_03, b, 0, b.Length);
                            store.postInfo[2].aisle = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [3便 アイル] エラー");
                            errorCount++;
                            continue;
                        }
                        // スロット
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Slot];
                            Array.Copy(byteData, _startAddr_Store_Slot_03, b, 0, b.Length);
                            store.postInfo[2].slot = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [3便 スロット] エラー");
                            errorCount++;
                            continue;
                        }
                        // ドッグNo
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_DogNo];
                            Array.Copy(byteData, _startAddr_Store_DogNo_03, b, 0, b.Length);
                            store.postInfo[2].dogNo = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [3便 ドッグNo] エラー");
                            errorCount++;
                            continue;
                        }
                        // 搬入条件
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Condition];
                            Array.Copy(byteData, _startAddr_Store_Condition_03, b, 0, b.Length);
                            store.postInfo[2].condition = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [3便 搬入条件] エラー");
                            errorCount++;
                            continue;
                        }
                        // 到着
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_TimeArrive];
                            Array.Copy(byteData, _startAddr_Store_TimeArrive_03, b, 0, b.Length);
                            store.postInfo[2].timeArrive = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [3便 到着] エラー");
                            errorCount++;
                            continue;
                        }
                        // 入場
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_TimeEntry];
                            Array.Copy(byteData, _startAddr_Store_TimeEntry_03, b, 0, b.Length);
                            store.postInfo[2].timeEntry = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [3便 入場] エラー");
                            errorCount++;
                            continue;
                        }
                        // 出発
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_TimeDepart];
                            Array.Copy(byteData, _startAddr_Store_TimeDepart_03, b, 0, b.Length);
                            store.postInfo[2].timeDepart = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [3便 出発] エラー");
                            errorCount++;
                            continue;
                        }
                        // 終了
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_TimeFinish];
                            Array.Copy(byteData, _startAddr_Store_TimeFinish_03, b, 0, b.Length);
                            store.postInfo[2].timeFinish = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [3便 終了] エラー");
                            errorCount++;
                            continue;
                        }
                        // 運送会社
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_Company];
                            Array.Copy(byteData, _startAddr_Store_Company_03, b, 0, b.Length);
                            store.postInfo[2].company = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [3便 運送会社] エラー");
                            errorCount++;
                            continue;
                        }
                        // 運送会社名
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_CompanyName];
                            Array.Copy(byteData, _startAddr_Store_CompanyName_03, b, 0, b.Length);
                            store.postInfo[2].companyName = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [3便 運送会社名] エラー");
                            errorCount++;
                            continue;
                        }
                        // 車種
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Store_CarType];
                            Array.Copy(byteData, _startAddr_Store_CarType_03, b, 0, b.Length);
                            store.postInfo[2].carType = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [3便 車種] エラー");
                            errorCount++;
                            continue;
                        }

                        // リストに追加
                        MasterStoreList.Add(store);
                    }

                    //// 更新日時を保持
                    //_latestUpdatedDtStore = File.GetLastWriteTime(IniFile.MasterStoreLocalFilePath);
                }


                // エラー発生行数を表示
                if (errorCount > 0)
                {
                    string message = $"店マスターファイル読み込みエラー エラー発生行数合計: {errorCount}行";
                    Logger.WriteLog(LogType.ERROR, message);
                    MessageBox.Show(message, "確認", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.MASTER_STORE_FILE_READ_ERROR;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        #endregion


        #region 作業者マスター処理
        /// <summary>
        /// 作業者マスタファイルをローカルにコピー
        /// </summary>
        /// <returns></returns>
        public UInt32 CopyMasterWorkerFile(out bool isTodaysFileLocal)
        {
            UInt32 rc = 0;
            isTodaysFileLocal = false;
            try
            {
                // -----------------------------------------------
                // サーバーとローカルのファイル情報を確認
                // -----------------------------------------------
                // サーバーのファイルの存在チェック
                if (!File.Exists(_masterWorkerFilePathServer))
                {
                    rc = (UInt32)ErrorCodeList.PICKDATA_FILE_IS_NOT_EXIST_SERVER;
                    return rc;
                }

                // サーバーのファイルが当日のものかチェック
                bool isTodaysFileServer = false;
                DateTime currentDt = DateTime.Now;
                DateTime fileDt = File.GetLastWriteTime(_masterWorkerFilePathServer);
                if (currentDt < DateTime.Today.AddHours(IniFile.DateChangeTime.Hour).AddMinutes(IniFile.DateChangeTime.Minute))
                {// 日付切り替わり時刻前 => ファイルの最終更新日が前日ならばOK
                    if (fileDt.Date == DateTime.Today.AddDays(-1).Date)
                    {// 最終更新日が前日
                        isTodaysFileServer = true;
                    }
                    else
                    {// 最終更新日が前日ではない
                        string message = $"サーバーの作業者マスタファイルが本日のものではありません。";
                        Logger.WriteLog(LogType.ERROR, message);
                    }
                }
                else
                {// 日付切り替わり時刻後 = 最終更新日が当日のファイルならばOK
                    if (fileDt.Date == DateTime.Today.Date)
                    {// 最終更新日が当日
                        isTodaysFileServer = true;
                    }
                    else
                    {// 最終更新日が当日ではない
                        string message = $"サーバーの作業者マスタファイルが本日のものではありません。";
                        Logger.WriteLog(LogType.ERROR, message);
                    }
                }

                // ローカルのファイルの存在チェック
                bool localExist = false;
                if (File.Exists(_masterWorkerFilePathLocal))
                    localExist = true;


                // -----------------------------------------------
                // サーバーとローカルを比較してコピー
                // -----------------------------------------------
                bool serverUpdated = false;
                if (isTodaysFileServer)
                {// サーバーに本日のファイルがある

                    // サーバーとローカルのファイルの更新日時を比較
                    if (localExist)
                    {
                        DateTime latestUpdatedDtServer = File.GetLastWriteTime(_masterWorkerFilePathServer);
                        DateTime latestUpdatedDtLocal = File.GetLastWriteTime(_masterWorkerFilePathLocal);
                        if (latestUpdatedDtServer != latestUpdatedDtLocal)
                            serverUpdated = true;
                    }

                    // サーバーからローカルにファイルをコピー
                    if (!localExist || serverUpdated)
                    {
                        // temp0フォルダがなければ作成
                        string dir = Path.GetDirectoryName(_masterWorkerFilePathLocal);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        // ファイルをコピー
                        File.Copy(_masterWorkerFilePathServer, _masterWorkerFilePathLocal, true);
                    }
                }


                // -----------------------------------------------
                // 最終的にローカルのファイルが当日のものかチェック
                // -----------------------------------------------
                localExist = false;
                if (File.Exists(_masterWorkerFilePathLocal))
                    localExist = true;
                if (localExist)
                {
                    currentDt = DateTime.Now;
                    fileDt = File.GetLastWriteTime(_masterWorkerFilePathLocal);
                    if (currentDt < DateTime.Today.AddHours(IniFile.DateChangeTime.Hour).AddMinutes(IniFile.DateChangeTime.Minute))
                    {// 日付切り替わり時刻前 => ファイルの最終更新日が前日ならばOK
                        if (fileDt.Date == DateTime.Today.AddDays(-1).Date)
                        {// 最終更新日が前日
                            isTodaysFileLocal = true;
                        }
                        else
                        {// 最終更新日が前日ではない
                            string message = $"ローカルの作業者マスタファイルが本日のものではありません。";
                            Logger.WriteLog(LogType.ERROR, message);
                        }
                    }
                    else
                    {// 日付切り替わり時刻後 = 最終更新日が当日のファイルならばOK
                        if (fileDt.Date == DateTime.Today.Date)
                        {// 最終更新日が当日
                            isTodaysFileLocal = true;
                        }
                        else
                        {// 最終更新日が当日ではない
                            string message = $"ローカルの作業者マスタファイルが本日のものではありません。";
                            Logger.WriteLog(LogType.ERROR, message);
                        }
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
        /// 作業者マスタファイル 読み出し
        /// </summary>
        /// <returns>エラーコード</returns>
        public UInt32 ReadMasterWorker()
        {
            UInt32 rc = 0;
            try
            {

                // -----------------------------------------------
                // 作業者マスター読み出し
                // -----------------------------------------------
                int index = 0;
                byte[] byteData = null;
                int errorCount = 0;
                // リストをクリア
                MasterWorkerList.Clear();
                //ファイルを開く
                using (System.IO.FileStream fs = new System.IO.FileStream(_masterWorkerFilePathLocal, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    // 商品数取得
                    int count = (int)(fs.Length / _dataSize_Worker_Total);
                    // 商品数分ループ
                    for (int i = 0; i < count; i++)
                    {
                        byteData = null;
                        byteData = new byte[_dataSize_Worker_Total];
                        // 読み出し開始位置を設定
                        fs.Seek(i * _dataSize_Worker_Total, SeekOrigin.Begin);
                        // 1商品分のデータを読み出し
                        fs.Read(byteData, 0, _dataSize_Worker_Total);


                        // データ変換
                        MasterWorker worker = new MasterWorker();
                        string text = "";
                        byte[] b = null;

                        // 作業者NO(代表)
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Worker_WorkerChiefNo];
                            Array.Copy(byteData, _startAddr_Worker_WorkerChiefNo, b, 0, b.Length);
                            worker.workerChiefNo = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [作業者NO(代表)] エラー");
                            errorCount++;
                            continue;
                        }
                        // 作業者名(代表)
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Worker_WorkerChiefName];
                            Array.Copy(byteData, _startAddr_Worker_WorkerChiefName, b, 0, b.Length);
                            worker.workerChiefName = Encoding.GetEncoding("shift_jis").GetString(b);
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [作業者名(代表)] エラー");
                            errorCount++;
                            continue;
                        }
                        // 作業者NO 1便
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Worker_WorkerNo];
                            Array.Copy(byteData, _startAddr_Worker_WorkerNo_01, b, 0, b.Length);
                            worker.workerNo[0] = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [作業者NO 1便] エラー");
                            errorCount++;
                            continue;
                        }
                        // 作業者NO 2便
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Worker_WorkerNo];
                            Array.Copy(byteData, _startAddr_Worker_WorkerNo_02, b, 0, b.Length);
                            worker.workerNo[1] = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [作業者NO 2便] エラー");
                            errorCount++;
                            continue;
                        }
                        // 作業者NO 3便
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Worker_WorkerNo];
                            Array.Copy(byteData, _startAddr_Worker_WorkerNo_03, b, 0, b.Length);
                            worker.workerNo[2] = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [作業者NO 3便] エラー");
                            errorCount++;
                            continue;
                        }
                        // 作業者NO 当日3便
                        try
                        {
                            b = null;
                            b = new byte[_dataSize_Worker_WorkerNo];
                            Array.Copy(byteData, _startAddr_Worker_WorkerNoActual_03, b, 0, b.Length);
                            worker.workerNo[3] = int.Parse(Encoding.GetEncoding("shift_jis").GetString(b));
                        }
                        catch (Exception)
                        {
                            Logger.WriteLog(LogType.ERROR, $"{i + 1}行目 [作業者NO 当日3便] エラー");
                            errorCount++;
                            continue;
                        }

                        // リストに追加
                        MasterWorkerList.Add(worker);
                    }

                    //// 更新日時を保持
                    //_latestUpdatedDtWorker = File.GetLastWriteTime(IniFile.MasterWorkerLocalFilePath);
                }

                // エラー発生行数を表示
                if (errorCount > 0)
                {
                    string message = $"作業者マスターファイル読み込みエラー エラー発生行数合計: {errorCount}行";
                    Logger.WriteLog(LogType.ERROR, message);
                    MessageBox.Show(message, "確認", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.MASTER_WORKER_FILE_READ_ERROR;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        #endregion





        /// <summary>
        /// 全ての商品名を取得
        /// </summary>
        /// <returns>エラーコード</returns>
        public UInt32 GetAllWorkName(out string[] workNameList)
        {
            UInt32 rc = 0;
            workNameList = null;
            try
            {
                // 1~3便まで全て取得
                List<string> allWorkName = new List<string>();
                for (int i = 0; i < Const.MaxPostCount; i++)
                {
                    var list = MasterWorkList[i].Select(x => x.workName).ToArray();
                    allWorkName.AddRange(list);
                }
                // 重複を除外
                workNameList = allWorkName.Distinct().ToArray();
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
        /// 全ての取引先名を取得
        /// </summary>
        /// <returns>エラーコード</returns>
        public UInt32 GetAllSuplierName(out string[] supplierNameList)
        {
            UInt32 rc = 0;
            supplierNameList = null;
            try
            {
                // 1~3便まで全て取得
                List<string> allSuppName = new List<string>();
                for (int i = 0; i < Const.MaxPostCount; i++)
                {
                    var list = MasterWorkList[i].Select(x => x.supplierName).ToArray();
                    allSuppName.AddRange(list);
                }

                // 重複を除外
                supplierNameList = allSuppName.Distinct().ToArray();
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
        /// 商品名からJANコードを取得
        /// </summary>
        /// <returns>エラーコード</returns>
        public UInt32 GetJanCodeFromWorkName(int postIndex, string workName, out string janCode)
        {
            UInt32 rc = 0;
            janCode = "";
            try
            {
                janCode = MasterWorkList[postIndex].Where(x => x.workName == workName).FirstOrDefault().JANCode;
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
        /// JANコードから商品名を取得
        /// </summary>
        /// <returns>エラーコード</returns>
        public UInt32 GetWorkNameFromJanCode(int postIndex, string janCode, out string workName)
        {
            UInt32 rc = 0;
            workName = "";
            try
            {
                workName = MasterWorkList[postIndex].Where(x => x.JANCode == janCode).FirstOrDefault().workName;
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
        /// JANコードから商品コードを取得
        /// </summary>
        /// <returns>エラーコード</returns>
        public UInt32 GetWorkCodeFromJanCode(int postIndex, string janCode, out string workCode)
        {
            UInt32 rc = 0;
            workCode = "";
            try
            {
                workCode = MasterWorkList[postIndex].Where(x => x.JANCode == janCode).FirstOrDefault().workCode;
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
        /// JANコードから取引先名、取引先コード取得
        /// </summary>
        /// <returns>エラーコード</returns>
        public UInt32 GetSuplierFromJanCode(int postIndex, string janCode, out string supplierCode, out string supplierName)
        {
            UInt32 rc = 0;
            supplierCode = "";
            supplierName = "";
            try
            {
                var a = MasterWorkList[postIndex].Where(x => x.JANCode == janCode).FirstOrDefault();
                supplierCode = a.supplierCode;
                supplierName = a.supplierName;
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
        /// 店コードから店名を取得
        /// </summary>
        /// <returns>エラーコード</returns>
        public UInt32 GetStoreNameFromStoreCode(string storeCode, out string storeName)
        {
            UInt32 rc = 0;
            storeName = "";
            try
            {
                MasterStore a = MasterStoreList.Where(x => x.storeCode == storeCode).FirstOrDefault();
                storeName = a.storeName;
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
        /// Check Error State
        /// </summary>
        private static bool STATUS_SUCCESS(UInt32 err) { return err == (int)ErrorCodeList.STATUS_SUCCESS; }





















        ///// <summary>
        ///// デバッグ
        ///// 店マスタファイル 読み出し
        ///// </summary>
        ///// <returns>エラーコード</returns>
        //public UInt32 DebugCsvToMasterFile()
        //{
        //    UInt32 rc = 0;
        //    try
        //    {
        //        // 配列からリストに格納する
        //        List<string> lists = new List<string>();

        //        //ファイルを開く
        //        StreamReader sr = new StreamReader(IniFile.MasterStoreCsvFilePath, System.Text.Encoding.GetEncoding("shift_jis"));
        //        {
        //            // 末尾まで繰り返す
        //            while (!sr.EndOfStream)
        //            {
        //                // CSVファイルの一行を読み込む
        //                string line = sr.ReadLine();
        //                // 読み込んだ一行をカンマ毎に分けて配列に格納する
        //                string[] values = line.Split(',');


        //                lists.AddRange(values);
        //            }
        //        }

        //        string s = string.Join("", lists);
        //        StreamWriter streamWriter = new StreamWriter(IniFile.MasterStoreServerFilePath + "___");
        //        streamWriter.Write(s);
        //        streamWriter.Close();

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteLog(LogType.ERROR, ex.Message);
        //        rc = (Int32)ErrorCodeList.EXCEPTION;
        //    }
        //    //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
        //    return rc;
        //}

        ///// <summary>
        ///// デバッグ
        ///// 商品マスターファイル出力
        ///// </summary>
        ///// <param name="list"></param>
        ///// <returns></returns>
        //public UInt32 Debug_WriteMasterWork(List<string[]> workList, string filePath) 
        //{
        //    UInt32 rc = 0;
        //    try
        //    {

        //        if (workList == null || workList.Count <= 0)
        //            return rc;


        //        // CSV書込み
        //        using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding("shift-jis")))
        //        {

        //            foreach (string[] line in workList) 
        //            {
        //                // 納品日
        //                sw.Write(line[0]);
        //                // 取引先コード
        //                sw.Write(line[1]);

        //                // VDRコード
        //                sw.Write("000000");

        //                // 取引先名(漢字)
        //                sw.Write(line[2]);
        //                // 商品コード
        //                sw.Write(line[3]);
        //                // 便
        //                sw.Write(line[4]);
        //                // JANコード
        //                sw.Write(line[5]);

        //                // JAN区分
        //                sw.Write("00");

        //                // 商品名(漢字)
        //                sw.Write(line[6]);

        //                // DEPT-CLASS
        //                sw.Write("00000");
        //                // 販売許容時間
        //                sw.Write("00000");
        //                // 入荷許容時間
        //                sw.Write("00000");
        //                // センター別入荷許容時間
        //                sw.Write("00000");
        //                // 物流センター区分
        //                sw.Write("0");
        //                // MAX積み付け段階
        //                sw.Write("0");
        //                // ピッキング区分
        //                sw.Write("0");
        //                // ピッキング形態
        //                sw.Write("0");
        //                // 値付
        //                sw.Write("0");
        //                // センター入数
        //                sw.Write("00000");

        //                // パック入数
        //                sw.Write(line[7]);

        //                // 物流コスト率
        //                sw.Write("000000");
        //                // センター入り原価
        //                sw.Write("0000000000");
        //                // 店着原価
        //                sw.Write("0000000000");
        //                // 売価1
        //                sw.Write("000000");
        //                // 値引率
        //                sw.Write("00000");
        //                // 売価2
        //                sw.Write("000000");
        //                // 最小発注数量
        //                sw.Write("0000");
        //                // 追加発注数量
        //                sw.Write("0000");
        //                // 代表CDCコード
        //                sw.Write("000000");
        //                // CDC名
        //                sw.Write("ああああああああああああああああああああ");
        //                // JANコード下4桁
        //                sw.Write("0000");

        //                // 商品名(ｶﾅ)
        //                sw.Write(line[8]);

        //            }
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteLog(LogType.ERROR, ex.Message);
        //        rc = (Int32)ErrorCodeList.EXCEPTION;
        //    }
        //    //Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
        //    return rc;
        //}




    }
}
