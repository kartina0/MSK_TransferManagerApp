//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

using DL_Logger;
using SystemConfig;
using ErrorCodeDefine;


namespace BatchModule
{
    /// <summary>
    /// バッチファイルクラス
    /// </summary>
    public class BatchFile
    {
        private const string THIS_NAME = "BatchFile";

        /// <summary>
        /// バッチ情報 (当期)
        /// </summary>
        public BatchInfo BatchInfoCurrent = null;
        /// <summary>
        /// バッチ情報 (次期)
        /// </summary>
        public BatchInfo BatchInfoNext = null;

        /// <summary>
        /// ファイル存在確認
        /// </summary>
        public bool isExistFile 
        { 
            get 
            {
                bool exist = System.IO.File.Exists(IniFile.BatchFileCurrentPath) && System.IO.File.Exists(IniFile.BatchFileNextPath);
                return exist;
            }
        }



        /// <summary>
        /// バッチ情報 読み込み
        /// </summary>
        /// <returns></returns>
        public UInt32 Load()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // ----------------------------------
                // 当期
                // ----------------------------------
                // バッチ情報リスト クリア
                BatchInfoCurrent = null;
                // jsonファイルを読み込みます
                using (StreamReader file = File.OpenText(IniFile.BatchFileCurrentPath))
                {
                    // Json.netのオブジェクトを作成します
                    JsonSerializer serializer = new JsonSerializer();
                    // デシリアライズ関数に読み込んだファイルと、データ用クラスの名称(型)を指定します。
                    // デシリアライズされたデータは、自動的にaccountのメンバ変数に格納されます 
                    BatchInfoCurrent = (BatchInfo)serializer.Deserialize(file, typeof(BatchInfo));
                }


                // ----------------------------------
                // 次期
                // ----------------------------------
                // バッチ情報リスト クリア
                BatchInfoNext = null;
                // jsonファイルを読み込みます
                using (StreamReader file = File.OpenText(IniFile.BatchFileNextPath))
                {
                    // Json.netのオブジェクトを作成します
                    JsonSerializer serializer = new JsonSerializer();
                    // デシリアライズ関数に読み込んだファイルと、データ用クラスの名称(型)を指定します。
                    // デシリアライズされたデータは、自動的にaccountのメンバ変数に格納されます 
                    BatchInfoNext = (BatchInfo)serializer.Deserialize(file, typeof(BatchInfo));
                }

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// バッチ情報 上書き保存
        /// </summary>
        /// <returns></returns>
        public UInt32 Save()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // ----------------------------------
                // 当期
                // ----------------------------------
                if (BatchInfoCurrent != null)
                {
                    // シリアライズ
                    using (var sw = new StreamWriter(IniFile.BatchFileCurrentPath, false, System.Text.Encoding.UTF8))
                    {
                        // JSON データにシリアライズ
                        var jsonData = JsonConvert.SerializeObject(BatchInfoCurrent, Newtonsoft.Json.Formatting.Indented);
                        // JSON データをファイルに書き込み
                        sw.Write(jsonData);
                    }
                }


                // ----------------------------------
                // 次期
                // ----------------------------------
                if (BatchInfoNext != null)
                {
                    // シリアライズ
                    using (var sw = new StreamWriter(IniFile.BatchFileNextPath, false, System.Text.Encoding.UTF8))
                    {
                        // JSON データにシリアライズ
                        var jsonData = JsonConvert.SerializeObject(BatchInfoNext, Newtonsoft.Json.Formatting.Indented);
                        // JSON データをファイルに書き込み
                        sw.Write(jsonData);
                    }
                }

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME}.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }



        /// <summary>
        /// Check Error State
        /// </summary>
        private bool STATUS_SUCCESS(UInt32 err) { return err == (int)ErrorCodeList.STATUS_SUCCESS; }
    }




    /// <summary>
    /// バッチ情報
    /// </summary>
    public class BatchInfo
    {
        /// <summary>
        /// 便リスト 1~3便
        /// </summary>
        public List<Post> Post = new List<Post>();
    }
    /// <summary>
    /// 便
    /// </summary>
    public class Post
    {
        /// <summary>
        /// アイルリスト
        /// </summary>
        public List<Aisle> Aisle = new List<Aisle>();
    }
    /// <summary>
    /// アイル
    /// </summary>
    public class Aisle
    {
        /// <summary>
        /// バッチリスト
        /// </summary>
        public List<Batch> Batch = new List<Batch>();

        /// <summary>
        /// 当アイルの全バッチの店コードを配列で出力
        /// </summary>
        /// <returns></returns>
        public string[] OutputToArray(bool[] enableUnit = null)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"Aisle {MethodBase.GetCurrentMethod().Name}()");
            string[] storeCodeArray = null;
            try
            {
                List<string> storeCodeList = new List<string>();
                foreach (Batch b in Batch)
                {
                    List<string> stores = b.OutputToArray(enableUnit).ToList();
                    storeCodeList.AddRange(stores);
                }
                storeCodeArray = storeCodeList.ToArray();
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"Aisle.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return storeCodeArray;
        }
    }
    /// <summary>
    /// 1バッチ分の情報
    /// </summary>
    public class Batch
    {
        #region 各スロットの店コード
        /// <summary> スロット1の店コード </summary>
        public string Slot01 { get; set; }
        /// <summary> スロット2の店コード </summary>
        public string Slot02 { get; set; }
        /// <summary> スロット3の店コード </summary>
        public string Slot03 { get; set; }
        /// <summary> スロット4の店コード </summary>
        public string Slot04 { get; set; }
        /// <summary> スロット5の店コード </summary>
        public string Slot05 { get; set; }
        /// <summary> スロット6の店コード </summary>
        public string Slot06 { get; set; }
        /// <summary> スロット7の店コード </summary>
        public string Slot07 { get; set; }
        /// <summary> スロット8の店コード </summary>
        public string Slot08 { get; set; }
        /// <summary> スロット9の店コード </summary>
        public string Slot09 { get; set; }
        /// <summary> スロット10の店コード </summary>
        public string Slot10 { get; set; }
        /// <summary> スロット11の店コード </summary>
        public string Slot11 { get; set; }
        /// <summary> スロット12の店コード </summary>
        public string Slot12 { get; set; }
        /// <summary> スロット13の店コード </summary>
        public string Slot13 { get; set; }
        /// <summary> スロット14の店コード </summary>
        public string Slot14 { get; set; }
        /// <summary> スロット15の店コード </summary>
        public string Slot15 { get; set; }
        /// <summary> スロット16の店コード </summary>
        public string Slot16 { get; set; }
        /// <summary> スロット17の店コード </summary>
        public string Slot17 { get; set; }
        /// <summary> スロット18の店コード </summary>
        public string Slot18 { get; set; }
        /// <summary> スロット19の店コード </summary>
        public string Slot19 { get; set; }
        /// <summary> スロット20の店コード </summary>
        public string Slot20 { get; set; }
        /// <summary> スロット21の店コード </summary>
        public string Slot21 { get; set; }
        /// <summary> スロット22の店コード </summary>
        public string Slot22 { get; set; }
        /// <summary> スロット23の店コード </summary>
        public string Slot23 { get; set; }
        /// <summary> スロット24の店コード </summary>
        public string Slot24 { get; set; }
        /// <summary> スロット25の店コード </summary>
        public string Slot25 { get; set; }
        /// <summary> スロット26の店コード </summary>
        public string Slot26 { get; set; }
        /// <summary> スロット27の店コード </summary>
        public string Slot27 { get; set; }
        /// <summary> スロット28の店コード </summary>
        public string Slot28 { get; set; }
        /// <summary> スロット29の店コード </summary>
        public string Slot29 { get; set; }
        /// <summary> スロット30の店コード </summary>
        public string Slot30 { get; set; }
        /// <summary> スロット31の店コード </summary>
        public string Slot31 { get; set; }
        /// <summary> スロット32の店コード </summary>
        public string Slot32 { get; set; }
        /// <summary> スロット33の店コード </summary>
        public string Slot33 { get; set; }
        /// <summary> スロット34の店コード </summary>
        public string Slot34 { get; set; }
        /// <summary> スロット35の店コード </summary>
        public string Slot35 { get; set; }
        /// <summary> スロット36の店コード </summary>
        public string Slot36 { get; set; }
        #endregion


        /// <summary>
        /// 当バッチの店コードを配列で出力
        /// </summary>
        /// <param name="enableUnit">ユニット有効無効(引数なしなら全てtrueとして扱う)</param>
        /// <returns></returns>
        public string[] OutputToArray(bool[] enableUnit = null)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"Batch {MethodBase.GetCurrentMethod().Name}()");
            string[] array = new string[Const.MaxUnitCount * Const.MaxSlotCount];
            try
            {
                if (enableUnit == null)
                    enableUnit = new bool[Const.MaxUnitCount] { true, true, true };

                // trueなら店コードを代入
                // falseなら""のまま
                if (enableUnit[0]) 
                {
                    array[0] = Slot01;
                    array[1] = Slot02;
                    array[2] = Slot03;
                    array[3] = Slot04;
                    array[4] = Slot05;
                    array[5] = Slot06;
                    array[6] = Slot07;
                    array[7] = Slot08;
                    array[8] = Slot09;
                    array[9] = Slot10;
                    array[10] = Slot11;
                    array[11] = Slot12;
                }

                if (enableUnit[1]) 
                {
                    array[12] = Slot13;
                    array[13] = Slot14;
                    array[14] = Slot15;
                    array[15] = Slot16;
                    array[16] = Slot17;
                    array[17] = Slot18;
                    array[18] = Slot19;
                    array[19] = Slot20;
                    array[20] = Slot21;
                    array[21] = Slot22;
                    array[22] = Slot23;
                    array[23] = Slot24;
                }

                if (enableUnit[2]) 
                {
                    array[24] = Slot25;
                    array[25] = Slot26;
                    array[26] = Slot27;
                    array[27] = Slot28;
                    array[28] = Slot29;
                    array[29] = Slot30;
                    array[30] = Slot31;
                    array[31] = Slot32;
                    array[32] = Slot33;
                    array[33] = Slot34;
                    array[34] = Slot35;
                    array[35] = Slot36;
                }

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"Batch.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return array;
        }

        /// <summary>
        /// 店コードの配列をバッチに入力
        /// </summary>
        /// <returns></returns>
        public string[] InputFromArray(string[] array)
        {
            UInt32 rc = 0;
            //Logger.WriteLog(LogType.METHOD_IN, $"Batch {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                Slot01 = array[0];
                Slot02 = array[1];
                Slot03 = array[2];
                Slot04 = array[3];
                Slot05 = array[4];
                Slot06 = array[5];
                Slot07 = array[6];
                Slot08 = array[7];
                Slot09 = array[8];
                Slot10 = array[9];
                Slot11 = array[10];
                Slot12 = array[11];
                Slot13 = array[12];
                Slot14 = array[13];
                Slot15 = array[14];
                Slot16 = array[15];
                Slot17 = array[16];
                Slot18 = array[17];
                Slot19 = array[18];
                Slot20 = array[19];
                Slot21 = array[20];
                Slot22 = array[21];
                Slot23 = array[22];
                Slot24 = array[23];
                Slot25 = array[24];
                Slot26 = array[25];
                Slot27 = array[26];
                Slot28 = array[27];
                Slot29 = array[28];
                Slot30 = array[29];
                Slot31 = array[30];
                Slot32 = array[31];
                Slot33 = array[32];
                Slot34 = array[33];
                Slot35 = array[34];
                Slot36 = array[35];

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            //Logger.WriteLog(LogType.METHOD_OUT, $"Batch.{MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return array;
        }
    }
}
