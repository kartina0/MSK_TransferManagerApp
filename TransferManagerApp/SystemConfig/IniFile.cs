//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.IO;
using System.Linq;

using DL_CommonLibrary;
using ErrorCodeDefine;


namespace SystemConfig
{
    /// <summary>
    /// INIファイル クラス
    /// </summary>
    public static class IniFile
    {
        /// <summary>
        /// this file path
        /// </summary>
        private static string _filePath = "";

        // --------------------------------------
        // SYSTEM
        // --------------------------------------
        /// <summary>
        /// バージョン
        /// </summary>
        public static int Version = 1;
        /// <summary>
        /// デバッグモード
        /// </summary>
        public static bool DebugMode = false;

        // --------------------------------------
        // PATH
        // --------------------------------------
        #region "PATH"
        /// <summary>
        /// システムログフォルダ
        /// </summary>
        public static string LogDir = ".\\Log";
        /// <summary>
        /// メッセージフォルダ
        /// </summary>
        public static string MessageDir = ".\\Message";

        /// <summary>
        /// DBバックアップフォルダ
        /// </summary>
        public static string DBBackup = "..\\Backup";
        /// <summary>
        /// エラー履歴ファイルパス
        /// </summary>
        public static string ErrorHistoryFilePath = "..\\ErrorHistory\\ErrorHistory.csv";
        /// <summary>
        /// 仕分完了報告書 出力フォルダ
        /// </summary>
        public static string OutputOrderReportDir = "..\\Compiled\\Report";
        #endregion

        // --------------------------------------
        // CYCLE
        // --------------------------------------
        /// <summary>
        /// 日付切り替わり時刻
        /// </summary>
        public static DateTime DateChangeTime = DateTime.MinValue;
        /// <summary>
        /// 当日PICKDATA更新時刻
        /// この時刻以降に更新されたPICKDATAファイルは当日ファイル
        /// </summary>
        public static DateTime PickDataUpdatedTime = DateTime.MinValue;

        // --------------------------------------
        // PLC
        // --------------------------------------
        /// <summary>
        /// アイル 有効/無効
        /// </summary>
        public static bool[] AisleEnable = null;
        /// <summary>
        /// ユニット 有効/無効
        /// </summary>
        public static bool[][] UnitEnable = null;
        /// <summary>
        /// PLC 接続文字列
        /// </summary>
        public static string[] PlcConnectionString = null;
        /// <summary>
        /// PLC IPアドレス
        /// </summary>
        public static string[] PlcIpAddress = null;
        ///// <summary>
        ///// PLC IPアドレス
        ///// </summary>
        //public static string PlcIpAddress = "127.0.0.1";

        // --------------------------------------
        // SERVER
        // --------------------------------------
        /// <summary>
        /// サーバー IPアドレス
        /// </summary>
        public static string DBIpAddress = "127.0.0.1";
        /// <summary>
        /// DB ポート番号
        /// </summary>
        public static int DBPortNo = 5432;
        /// <summary>
        /// DB パスワード
        /// </summary>
        public static string DBPassword = "datalink";
        /// <summary>
        /// DB接続文字列
        /// </summary>
        public static string DB_SQL_Connection = "Server={0};Port={1};Database=agf_db;User Id=agf;Password={2};Enlist=true;MinPoolSize=10;MaxPoolSize=100;Connection Idle Lifetime=30;Timeout=5;CommandTimeout=5;";
        /// <summary>
        /// 仕分データ種別 0=DB/1=CSV
        /// </summary>
        public static ORDER_INFO_TYPE OrderInfoType = ORDER_INFO_TYPE.DB;
        /// <summary>
        /// PICKDATA 最新版に更新 1=有効/0=無効
        /// </summary>
        public static bool PickDataUpdate = false;
        /// <summary>
        /// PICKDATA 1=有効/0=無効
        /// </summary>
        public static bool PickDataEnable = false;
        /// <summary>
        /// 起動時DB再作成 1=有効/0=無効
        /// </summary>
        public static bool DBRemake = false;
        /// <summary>
        /// ログインID(DB用)
        /// </summary>
        public static string DBLoginId = "PC";
        /// <summary>
        /// PICKDATA サーバーフォルダ
        /// </summary>
        public static string PickdataServerDir= "Server\\pickdata";
        /// <summary>
        /// PICKDATA ローカルフォルダ 
        /// </summary>
        public static string PickdataLocalDir = "Pickdata\\pickdata";
        /// <summary>
        /// PICKDATA バックアップフォルダ
        /// </summary>
        public static string PickdataBackupDir = "Pickdata\\_Backup";
        /// <summary>
        /// 商品マスター ファイルパス
        /// </summary>
        public static string MasterWorkFilePath = "temp0\\SHOHIN";
        /// <summary>
        /// 店マスター ファイルパス
        /// </summary>
        public static string MasterStoreFilePath = "temp0\\STORE";
        /// <summary>
        /// 作業者マスター ファイルパス
        /// </summary>
        public static string MasterWorkerFilePath = "temp0\\WORKER";

        // --------------------------------------
        // BATCHファイル
        // --------------------------------------
        /// <summary>
        /// バッチファイル(当期) ファイルパス 
        /// </summary>
        public static string BatchFileCurrentPath = @"..\Compiled\Batch\BatchFile_Current.json";
        /// <summary>
        /// バッチファイル(次期) ファイルパス 
        /// </summary>
        public static string BatchFileNextPath = @"..\Compiled\Batch\BatchFile_Next.json";



        /// <summary>
        /// Load File
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static UInt32 Load(string fileName)
        {
            UInt32 rc = 0;
            try
            {

                // Load From File
                string section = "";
                bool exist = false;
                string sBuf = "";
                int iTemp = 0;
                string key = "";
                // Get Full Path.
                fileName = System.IO.Path.GetFullPath(fileName);

                if (!FileIo.ExistFile(fileName)) rc = (UInt32)ErrorCodeList.FILE_NOT_FOUND;

                if (STATUS_SUCCESS(rc))
                {
                    _filePath = fileName;
                    // --------------------------
                    // [SYSTEM]
                    // --------------------------
                    section = "SYSTEM";
                    exist = FileIo.ReadIniFile<int>(_filePath, section, "VERSION", ref Version);
                    exist = FileIo.ReadIniFile<bool>(_filePath, section, "DEBUG_MODE", ref DebugMode);


                    // --------------------------
                    // [PATH]
                    // --------------------------
                    section = "PATH";
                    sBuf = "";

                    exist = FileIo.ReadIniFile<string>(_filePath, section, "LOG_DIR", ref sBuf);
                    if (sBuf != "") LogDir = sBuf; // System.IO.Path.GetFullPath(sBuf);
                                                   //if (!exist) FileIo.WriteIniValue(_filePath, section, "SYSTEM_LOG_DIR", systemLogDir);

                    //exist = FileIo.ReadIniFile<string>(_filePath, section, "SYSTEM_LOG_DIR", ref sBuf);
                    //if (sBuf != "") systemLogDir = sBuf; // System.IO.Path.GetFullPath(sBuf);
                    ////if (!exist) FileIo.WriteIniValue(_filePath, section, "SYSTEM_LOG_DIR", systemLogDir);

                    //sBuf = "";
                    //exist = FileIo.ReadIniFile<string>(_filePath, section, "ALARM_LOG_DIR", ref sBuf);
                    //if (sBuf != "") alarmLogDir = sBuf; //System.IO.Path.GetFullPath(sBuf);
                    ////if (!exist) FileIo.WriteIniValue(_filePath, section, "ALARM_LOG_DIR", alarmLogDir);

                    //sBuf = "";
                    //exist = FileIo.ReadIniFile<string>(_filePath, section, "HISTORY_LOG_DIR", ref sBuf);
                    //if (sBuf != "") historyLogDir = sBuf; //System.IO.Path.GetFullPath(sBuf);
                    ////if (!exist) FileIo.WriteIniValue(_filePath, section, "HISTORY_LOG_DIR", historyLogDir);

                    //sBuf = "";
                    //exist = FileIo.ReadIniFile<string>(_filePath, section, "OPERATION_LOG_DIR", ref sBuf);
                    //if (sBuf != "") operationLogDir = sBuf; //System.IO.Path.GetFullPath(sBuf);
                    ////if (!exist) FileIo.WriteIniValue(_filePath, section, "OPERATION_LOG_DIR", operationLogDir);
                    //sBuf = "";

                    exist = FileIo.ReadIniFile<string>(_filePath, section, "DB_BACKUP_DIR", ref DBBackup);
                    exist = FileIo.ReadIniFile<string>(_filePath, section, "ERROR_HISTORY_FILE_PATH", ref ErrorHistoryFilePath);
                    exist = FileIo.ReadIniFile<string>(_filePath, section, "OUTPUT_ORDER_REPORT_FILE_PATH", ref OutputOrderReportDir);


                    // フォルダ作成
                    if (LogDir != "") System.IO.Directory.CreateDirectory(LogDir);
                    //if (systemLogDir != "") System.IO.Directory.CreateDirectory(systemLogDir);
                    //if (alarmLogDir != "") System.IO.Directory.CreateDirectory(alarmLogDir);
                    //if (historyLogDir != "") System.IO.Directory.CreateDirectory(historyLogDir);
                    //if (operationLogDir != "") System.IO.Directory.CreateDirectory(operationLogDir);
                    if (DBBackup != "") System.IO.Directory.CreateDirectory(DBBackup);
                    if (ErrorHistoryFilePath != "") 
                    {
                        string dir = Path.GetDirectoryName(ErrorHistoryFilePath);
                        System.IO.Directory.CreateDirectory(dir);
                    } 
                    if (OutputOrderReportDir != "") System.IO.Directory.CreateDirectory(OutputOrderReportDir);


                    // --------------------------
                    // [CYCLE]
                    // --------------------------
                    section = "CYCLE";
                    sBuf = "";
                    exist = FileIo.ReadIniFile<string>(_filePath, section, "DATE_CHANGE_TIME", ref sBuf);
                    string[] split = sBuf.Split(':');
                    if (split.Length > 0) 
                    {
                        DateChangeTime = DateChangeTime.AddHours(int.Parse(split[0]));
                        if (split.Length > 1) 
                        {
                            DateChangeTime = DateChangeTime.AddMinutes(int.Parse(split[1]));
                        }
                    }

                    sBuf = "";
                    exist = FileIo.ReadIniFile<string>(_filePath, section, "PICKDATA_UPDATED_TIME", ref sBuf);
                    split = null;
                    split = sBuf.Split(':');
                    if (split.Length > 0)
                    {
                        PickDataUpdatedTime = PickDataUpdatedTime.AddHours(int.Parse(split[0]));
                        if (split.Length > 1)
                        {
                            PickDataUpdatedTime = PickDataUpdatedTime.AddMinutes(int.Parse(split[1]));
                        }
                    }


                    // --------------------------
                    // [PLC]
                    // --------------------------
                    section = "PLC";
                    sBuf = "";
                    AisleEnable = new bool[Const.MaxAisleCount];
                    exist = FileIo.ReadIniFile<string>(_filePath, section, "AISLE_ENABLE", ref sBuf);
                    string[] array = sBuf.Split(',');
                    for (int i = 0; i < Const.MaxAisleCount; i++)
                    {
                        AisleEnable[i] = (array[i] == "1");
                    }

                    UnitEnable = new bool[Const.MaxAisleCount][];
                    for (int i = 0; i < Const.MaxAisleCount; i++)
                    {
                        sBuf = "";
                        exist = FileIo.ReadIniFile<string>(_filePath, section, string.Format("UNIT_ENABLE[{0}]", i), ref sBuf);
                        UnitEnable[i] = new bool[Const.MaxUnitCount];
                        array = sBuf.Split(',');
                        for (int j = 0; j < Const.MaxUnitCount; j++)
                        {
                            UnitEnable[i][j] = (array[j] == "1");
                        }
                    }
                    PlcConnectionString = Misc.CreateArray<string>("", Const.MaxAisleCount);
                    for (int i = 0; i < Const.MaxAisleCount; i++)
                        exist = FileIo.ReadIniFile<string>(_filePath, section, string.Format("PLC_CONNECTION[{0}]", i), new string[1] { "//" }, ref PlcConnectionString[i]);
                    //exist = FileIo.ReadIniFile<string>(_filePath, section, string.Format("PLC_CONNECTION[{0}]", i), ref PlcConnectionString[i]);
                    PlcIpAddress = Misc.CreateArray<string>("", Const.MaxAisleCount);
                    for (int i = 0; i < Const.MaxAisleCount; i++)
                        exist = FileIo.ReadIniFile<string>(_filePath, section, string.Format("IP_ADDRESS[{0}]", i), ref PlcIpAddress[i]);


                    // --------------------------
                    // [SERVER]
                    // --------------------------
                    section = "SERVER";
                    exist = FileIo.ReadIniFile<string>(_filePath, section, "DB_IP_ADDRESS", ref DBIpAddress);
                    exist = FileIo.ReadIniFile<int>(_filePath, section, "DB_PORT_NO", ref DBPortNo);
                    exist = FileIo.ReadIniFile<string>(_filePath, section, "DB_PASSWORD", ref DBPassword);
                    exist = FileIo.ReadIniFile<string>(_filePath, section, "DB_SQL_CONNECTION", new string[1] { "//" }, ref DB_SQL_Connection);
                    DB_SQL_Connection = string.Format(DB_SQL_Connection, DBIpAddress, DBPortNo, DBPassword);

                    int val = 0;
                    exist = FileIo.ReadIniFile<int>(_filePath, section, "ORDER_INFO_TYPE", ref val);
                    OrderInfoType = (ORDER_INFO_TYPE)val;

                    bool b = false;
                    exist = FileIo.ReadIniFile<bool>(_filePath, section, "PICKDATA_UPDATE", ref b);
                    PickDataUpdate = b;

                    b = false;
                    exist = FileIo.ReadIniFile<bool>(_filePath, section, "PICKDATA_ENABLE", ref b);
                    PickDataEnable = b;

                    b = false;
                    exist = FileIo.ReadIniFile<bool>(_filePath, section, "DB_REMAKE", ref b);
                    DBRemake = b;

                    exist = FileIo.ReadIniFile<string>(_filePath, section, "DB_LOGIN_ID", ref DBLoginId);


                    exist = FileIo.ReadIniFile<string>(_filePath, section, "PICKDATA_SERVER_DIR", ref PickdataServerDir);
                    exist = FileIo.ReadIniFile<string>(_filePath, section, "PICKDATA_LOCAL_DIR", ref PickdataLocalDir);
                    exist = FileIo.ReadIniFile<string>(_filePath, section, "PICKDATA_BACKUP_DIR", ref PickdataBackupDir);
                    exist = FileIo.ReadIniFile<string>(_filePath, section, "MASTER_WORK_FILE_PATH", ref MasterWorkFilePath);
                    exist = FileIo.ReadIniFile<string>(_filePath, section, "MASTER_STORE_FILE_PATH", ref MasterStoreFilePath);
                    exist = FileIo.ReadIniFile<string>(_filePath, section, "MASTER_WORKER_FILE_PATH", ref MasterWorkerFilePath);


                    // --------------------------------------
                    // BATCHファイル
                    // --------------------------------------
                    section = "BATCH";
                    exist = FileIo.ReadIniFile<string>(_filePath, section, "BATCH_CURRENT_FILE_PATH", ref BatchFileCurrentPath);
                    exist = FileIo.ReadIniFile<string>(_filePath, section, "BATCH_NEXT_FILE_PATH", ref BatchFileNextPath);


                }
            }
            catch (Exception ex)
            {
                //Resource.ErrorHandler(ex, true);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        /// <summary>
        /// Save File
        /// </summary>
        /// <returns></returns>
        public static UInt32 Save()
        {
            UInt32 rc = 0;
            try
            {

                // Load From File
                string section = "";
                string key = "";
                string sBuf = "";
                if (STATUS_SUCCESS(rc))
                {
                    // --------------------------
                    // [PLC]
                    // --------------------------
                    section = "PLC";
                    for (int i = 0; i < Const.MaxAisleCount; i++) 
                    {
                        if(IniFile.AisleEnable[i])
                            sBuf += "1,";
                        else
                            sBuf += "0,";
                    }
                    sBuf = sBuf.Remove(7, 1);
                    FileIo.WriteIniValue(_filePath, section, "AISLE_ENABLE", sBuf);

                    for (int i = 0; i < Const.MaxAisleCount; i++)
                    {
                        sBuf = "";
                        for (int j = 0; j < Const.MaxUnitCount; j++)
                        {
                            if (IniFile.UnitEnable[i][j]) 
                                sBuf += "1,";
                            else
                                sBuf += "0,";
                        }
                        sBuf = sBuf.Remove(5, 1);
                        FileIo.WriteIniValue(_filePath, section, string.Format("UNIT_ENABLE[{0}]", i), sBuf);
                    }


                    // --------------------------
                    // [SERVER]
                    // --------------------------
                    section = "SERVER";
                    FileIo.WriteIniValue(_filePath, section, "IP_ADDRESS", DBIpAddress);
                    FileIo.WriteIniValue(_filePath, section, "DB_PORT_NO", DBPortNo.ToString());

                }
            }
            catch
            {
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }


        /// <summary>
        /// Save Setting
        /// </summary>
        /// <returns></returns>
        public static UInt32 SaveSetting()
        {
            UInt32 rc = 0;
            try
            {

                // Load From File
                string section = "";
                string key = "";
                if (STATUS_SUCCESS(rc))
                {
                    section = "SETTING";
                    //FileIo.WriteIniValue(_filePath, section, "LAST_WORK_DIR", LastWorkDir);
                }
            }
            catch
            {
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        /// <summary>
        /// リスト読み込み
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dest"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public static UInt32 LoadList<T>(string fileName, string section, string baseKey, int maxCount, ref T[] dest)
        {
            UInt32 rc = 0;
            try
            {
                // Load From File
                string key = "";
                string temp = "";
                bool exist = false;
                // Get Full Path.
                fileName = System.IO.Path.GetFullPath(fileName);

                if (!FileIo.ExistFile(fileName)) rc = (UInt32)ErrorCodeList.FILE_NOT_FOUND;

                dest = new T[0];

                if (STATUS_SUCCESS(rc))
                {
                    _filePath = fileName;


                    for (int i = 0; i < maxCount; i++)
                    {
                        key = string.Format("{0}[{1}]", baseKey, i);
                        exist = FileIo.ReadIniFile<string>(_filePath, section, key, ref temp);
                        if (exist)
                        {
                            int cnt = dest.Length;
                            Array.Resize<T>(ref dest, cnt + 1);
                            dest[cnt] = FileIo.ConvType<T>(temp);
                        }
                    }


                }
            }
            catch
            {
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        /// <summary>
        /// リスト読み込み
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dest"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public static UInt32 LoadList<T>(string fileName, string section, string baseKey, int minCount, int maxCount, ref T[] dest)
        {
            UInt32 rc = 0;
            try
            {

                // Load From File
                string key = "";
                string temp = "";
                bool exist = false;
                // Get Full Path.
                fileName = System.IO.Path.GetFullPath(fileName);

                if (!FileIo.ExistFile(fileName)) rc = (UInt32)ErrorCodeList.FILE_NOT_FOUND;

                dest = new T[0];

                if (STATUS_SUCCESS(rc))
                {
                    _filePath = fileName;


                    for (int i = 0; i < maxCount; i++)
                    {
                        key = string.Format("{0}[{1}]", baseKey, i);
                        exist = FileIo.ReadIniFile<string>(_filePath, section, key, ref temp);
                        if (exist)
                        {
                            int cnt = dest.Length;
                            Array.Resize<T>(ref dest, cnt + 1);
                            dest[cnt] = FileIo.ConvType<T>(temp);
                        }
                    }
                }

                // 不足分の配列を作成
                for (int i = dest.Count(); i < minCount; i++)
                {
                    int cnt = dest.Length;
                    Array.Resize<T>(ref dest, cnt + 1);
                }


            }
            catch
            {
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// リスト保存
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dest"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public static UInt32 SaveList<T>(string fileName, string section, string baseKey, int maxCount, T[] dataList)
        {
            UInt32 rc = 0;
            try
            {

                // Load From File
                string key = "";
                string temp = "";

                if (STATUS_SUCCESS(rc))
                {
                    _filePath = fileName;


                    for (int i = 0; i < maxCount; i++)
                    {

                        key = string.Format("{0}[{1}]", baseKey, i);
                        temp = "";
                        if (dataList.Count() > i)
                        {
                            temp = dataList[i].ToString();
                        }
                        FileIo.WriteIniValue(_filePath, section, key, temp);
                    }
                }
            }
            catch
            {
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }



        /// <summary>
        /// Check Error State
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        private static bool STATUS_SUCCESS(UInt32 err)
        {
            return err == (int)ErrorCodeList.STATUS_SUCCESS;
        }
    }
}
