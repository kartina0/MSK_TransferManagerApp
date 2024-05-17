//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using ServerModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Npgsql;
using System.Reflection;

using SystemConfig;
using DL_Logger;
using ErrorCodeDefine;
using ShareResource;


namespace TransferManagerApp_Debug
{
    /// <summary>
    /// 商品ヘッダテーブル、店別小仕分けテーブルに
    /// デバッグ用のデータをセットするためのクラス
    /// </summary>
    public class Debug_DB
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "OrderInfoManager";


        /// <summary>
        /// データベース名
        /// </summary>
        private string _DbName = "transfer_manager_db";
        /// <summary>
        /// テーブル名
        /// </summary>
        private string _TableName_PickHead = "dp01_pick_head_0";
        private string _TableName_PickDetail = "dp02_pick_detail_0";
        /// <summary>
        /// 現在日時
        /// </summary>
        private DateTime _dtNow = DateTime.Now;

        private object _lock = new object();


        public Debug_DB() 
        {
            _TableName_PickHead = _TableName_PickHead + $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}";
            _TableName_PickDetail = _TableName_PickDetail + $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}";
        }

        /// <summary>
        /// デバッグ
        /// 接続
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private UInt32 Debug_Connect(out NpgsqlConnection connection)
        {
            UInt32 rc = 0;
            connection = null;
            try
            {
                // 接続オブジェクト作成
                connection = new NpgsqlConnection(IniFile.DB_SQL_Connection);
                // 接続開始
                connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                rc = 1;
            }
            return rc;
        }
        /// <summary>
        /// デバッグ
        /// 切断
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private UInt32 Debug_Close(NpgsqlConnection connection)
        {
            UInt32 rc = 0;
            try
            {
                // 切断
                connection.Close();
                connection.Dispose();
                connection = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                rc = 1;
            }
            return rc;
        }


        /// <summary>
        /// デバッグ
        /// 当日のテーブル存在確認
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public bool Debug_CheckTable()
        {
            UInt32 rc = 0;
            NpgsqlConnection connection = null;
            bool exist = true;
            try
            {
                // 現在日時
                DateTime dt = DateTime.Now;
                // 日付切り替わり処理
                if (dt < DateTime.Today.AddHours(IniFile.DateChangeTime.Hour).AddMinutes(IniFile.DateChangeTime.Minute))
                {
                    dt = dt.AddDays(-1);
                }

                string todayDate = $"{dt.Year.ToString("D4")}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";
                List<string> tablenames = new List<string>();
                Process pros = null;


                // --------------------------------------------
                // 今日の日付のテーブルが既にあるか確認
                // --------------------------------------------
                exist = true;
                rc = Debug_Connect(out connection);
                // データベース内のすべてのテーブル名を取得するSQLクエリ
                string selectQuery = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';";
                using (var command = new NpgsqlCommand(selectQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tableName = reader["table_name"].ToString();
                            tablenames.Add(tableName);
                        }
                    }
                }
                // テーブル名に今日の日付が入っているか
                if (tablenames.Count <= 0) 
                {
                    exist = false;
                }
                else
                {
                    bool e = false;
                    foreach (string s in tablenames) 
                    {
                        if (s.Contains(todayDate)) 
                        {
                            e = true;
                            break;
                        }
                    }
                    //(tablenames[0].Substring(tablenames[0].Length - 8, 8) != todayDate)
                    
                    exist = e;
                } 

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
                Resource.ErrorHandler(ex, true);
            }
            finally
            {
                Thread.Sleep(500);
                // 切断
                rc = Debug_Close(connection);
            }
            return exist;
        }
        /// <summary>
        /// デバッグ
        /// テーブル作成
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public UInt32 Debug_CreateTable()
        {
            UInt32 rc = 0;
            NpgsqlConnection connection = null;
            try
            {
                // 現在日時
                DateTime dt = DateTime.Now;

                // 日付切り替わり処理
                if (dt < DateTime.Today.AddHours(IniFile.DateChangeTime.Hour).AddMinutes(IniFile.DateChangeTime.Minute))
                {
                    dt = dt.AddDays(-1);
                }

                string todayDate = $"{dt.Year.ToString("D4")}{dt.Month.ToString("D2")}{dt.Day.ToString("D2")}";
                List<string> tablenames = new List<string>();
                Process pros = null;


                // --------------------------------------------
                // 今日の日付のテーブルが既にあるか確認
                // --------------------------------------------
                bool exist = true;
                rc = Debug_Connect(out connection);
                // データベース内のすべてのテーブル名を取得するSQLクエリ
                string selectQuery = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';";
                using (var command = new NpgsqlCommand(selectQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tableName = reader["table_name"].ToString();
                            tablenames.Add(tableName);
                        }
                    }
                }
                // テーブル名に今日の日付が入っているか
                if (tablenames.Count <= 0)
                    exist = false;
                else if (tablenames[0].Substring(tablenames[0].Length - 8, 8) != todayDate)
                    exist = false;


                if (!exist)
                {
                    // --------------------------------------------
                    // 今日のテーブルが無ければ、作成する
                    // --------------------------------------------

                    // バッチファイルパス
                    string dir = System.IO.Path.GetFullPath(".\\DebugTool\\Order\\Tool\\20230718");
                    string fileName = "CreateTable.bat";
                    //string fileName = "test01.bat";
                    // バッチファイル実行用オブジェクトをセットアップ
                    pros = new Process();
                    pros.StartInfo.WorkingDirectory = dir;
                    pros.StartInfo.FileName = System.IO.Path.Combine(dir, fileName); // batch file name to be execute
                    pros.StartInfo.UseShellExecute = false;
                    pros.StartInfo.RedirectStandardInput = true;
                    pros.StartInfo.RedirectStandardOutput = true;
                    pros.StartInfo.CreateNoWindow = false;
                    // バッチファイル実行
                    pros.Start(); // run batch file
                    // 途中で日付の入力を求められるので、現在日付を入力
                    pros.StandardInput.WriteLine(todayDate);
                    pros.StandardInput.Close();

                    //pros.WaitForExit();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                rc = 1;
            }
            finally 
            {
                Thread.Sleep(500);
                // 切断
                rc = Debug_Close(connection);
            }
            return rc;
        }



        /// <summary>
        /// デバッグ
        /// 商品ヘッダテーブル 初期化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public UInt32 Debug_InitDbOrderData()
        {
            UInt32 rc = 0;
            NpgsqlConnection connection = null;
            try
            {
                // 接続
                rc = Debug_Connect(out connection);


                //// テーブル内の全レコード削除
                //string deleteQuery = $"DELETE FROM {_TableName_PickHead};";
                //using (var command = new NpgsqlCommand(deleteQuery, connection))
                //{
                //    command.ExecuteNonQuery();
                //    Console.WriteLine($"テーブル '{_TableName_PickHead}' の全レコードを削除しました。");
                //}


                // データ作成
                DataCreate(out List<ModelPickHead> pickHeads);


                // データ書込み
                string columns = "delivery_date, post_no, delivery_date_order, post_no_order, sku_code, pd_count, sku_name, jan_code, case_volume, pieces_num_total, sku_kana, max_stack_num, " +
                    "sales_price, pick_class, pieces_num_st1, pieces_num_st2, pieces_num_st3, pieces_num_st4, pieces_num_st5, pieces_num_st6, pieces_num_st7, pieces_num_st8, pieces_num_st9, " +
                    "store_num_st1, store_num_st2, store_num_st3, store_num_st4, store_num_st5, store_num_st6, store_num_st7, store_num_st8, store_num_st9, " +
                    "create_date, create_time, create_login_id, renew_date, renew_time, renew_login_id";
                string datas = "@delivery_date, @post_no, @delivery_date_order, @post_no_order, @sku_code, @pd_count, @sku_name, @jan_code, @case_volume, @pieces_num_total, @sku_kana, @max_stack_num, " +
                    "@sales_price, @pick_class, @pieces_num_st1, @pieces_num_st2, @pieces_num_st3, @pieces_num_st4, @pieces_num_st5, @pieces_num_st6, @pieces_num_st7, @pieces_num_st8, @pieces_num_st9, " +
                    "@store_num_st1, @store_num_st2, @store_num_st3, @store_num_st4, @store_num_st5, @store_num_st6, @store_num_st7, @store_num_st8, @store_num_st9, " +
                    "@create_date, @create_time, @create_login_id, @renew_date, @renew_time, @renew_login_id";
                string insertQuery = $"INSERT INTO {_TableName_PickHead} ({columns}) VALUES ({datas})";
                foreach (ModelPickHead data in pickHeads)
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand(insertQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("delivery_date", data.delivery_date);
                        cmd.Parameters.AddWithValue("post_no", data.post_no);
                        cmd.Parameters.AddWithValue("delivery_date_order", data.delivery_date_order);
                        cmd.Parameters.AddWithValue("post_no_order", data.post_no_order);
                        cmd.Parameters.AddWithValue("sku_code", data.sku_code);
                        cmd.Parameters.AddWithValue("pd_count", data.pd_count);
                        cmd.Parameters.AddWithValue("sku_name", data.sku_name);
                        cmd.Parameters.AddWithValue("jan_code", data.jan_code);
                        cmd.Parameters.AddWithValue("case_volume", data.case_volume);
                        cmd.Parameters.AddWithValue("pieces_num_total", data.pieces_num_total);
                        cmd.Parameters.AddWithValue("sku_kana", data.sku_kana);
                        cmd.Parameters.AddWithValue("max_stack_num", data.max_stack_num);
                        cmd.Parameters.AddWithValue("sales_price", data.sales_price);
                        cmd.Parameters.AddWithValue("pick_class", data.pick_class);
                        cmd.Parameters.AddWithValue("pieces_num_st1", data.pieces_num_st1);
                        cmd.Parameters.AddWithValue("pieces_num_st2", data.pieces_num_st2);
                        cmd.Parameters.AddWithValue("pieces_num_st3", data.pieces_num_st3);
                        cmd.Parameters.AddWithValue("pieces_num_st4", data.pieces_num_st4);
                        cmd.Parameters.AddWithValue("pieces_num_st5", data.pieces_num_st5);
                        cmd.Parameters.AddWithValue("pieces_num_st6", data.pieces_num_st6);
                        cmd.Parameters.AddWithValue("pieces_num_st7", data.pieces_num_st7);
                        cmd.Parameters.AddWithValue("pieces_num_st8", data.pieces_num_st8);
                        cmd.Parameters.AddWithValue("pieces_num_st9", data.pieces_num_st9);
                        cmd.Parameters.AddWithValue("store_num_st1", data.store_num_st1);
                        cmd.Parameters.AddWithValue("store_num_st2", data.store_num_st2);
                        cmd.Parameters.AddWithValue("store_num_st3", data.store_num_st3);
                        cmd.Parameters.AddWithValue("store_num_st4", data.store_num_st4);
                        cmd.Parameters.AddWithValue("store_num_st5", data.store_num_st5);
                        cmd.Parameters.AddWithValue("store_num_st6", data.store_num_st6);
                        cmd.Parameters.AddWithValue("store_num_st7", data.store_num_st7);
                        cmd.Parameters.AddWithValue("store_num_st8", data.store_num_st8);
                        cmd.Parameters.AddWithValue("store_num_st9", data.store_num_st9);
                        cmd.Parameters.AddWithValue("create_date", data.create_date);
                        cmd.Parameters.AddWithValue("create_time", data.create_time);
                        cmd.Parameters.AddWithValue("create_login_id", data.create_login_id);
                        cmd.Parameters.AddWithValue("renew_date", data.renew_date);
                        cmd.Parameters.AddWithValue("renew_time", data.renew_time);
                        cmd.Parameters.AddWithValue("renew_login_id", data.renew_login_id);

                        cmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                rc = 1;
            }
            finally
            {
                Thread.Sleep(500);
                // 切断
                rc = Debug_Close(connection);
            }
            return rc;
        }




        /// <summary>
        /// 店別小仕分け 書込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public UInt32 Debug_InitDbStoreInfo()
        {

            UInt32 rc = 0;
            NpgsqlConnection connection = null;
            try
            {
                // 接続
                rc = Debug_Connect(out connection);


                //// テーブル内の全レコード削除
                //string deleteQuery = $"DELETE FROM {_TableName_PickDetail};";
                //using (var command = new NpgsqlCommand(deleteQuery, connection))
                //{
                //    command.ExecuteNonQuery();
                //    Console.WriteLine($"テーブル '{_TableName_PickDetail}' の全レコードを削除しました。");
                //}


                // データ作成
                DataCreate02(out List<ModelPickDetail> pickDetails);


                // データ書込み
                string columns = "delivery_date, post_no, delivery_date_order, post_no_order, sku_code, pd_count, store_code, " +
                    "station_no, aisle_no, slot_no, case_volume, pieces_num, create_date, create_time, create_login_id, renew_date, renew_time, renew_login_id";
                string datas = "@delivery_date, @post_no, @delivery_date_order, @post_no_order, @sku_code, @pd_count, @store_code, " +
                    "@station_no, @aisle_no, @slot_no, @case_volume, @pieces_num, @create_date, @create_time, @create_login_id, @renew_date, @renew_time, @renew_login_id";
                string insertQuery = $"INSERT INTO {_TableName_PickDetail} ({columns}) VALUES ({datas})";

                foreach (ModelPickDetail data in pickDetails)
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand(insertQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("delivery_date", data.delivery_date);
                        cmd.Parameters.AddWithValue("post_no", data.post_no);
                        cmd.Parameters.AddWithValue("delivery_date_order", data.delivery_date_order);
                        cmd.Parameters.AddWithValue("post_no_order", data.post_no_order);
                        cmd.Parameters.AddWithValue("sku_code", data.sku_code);
                        cmd.Parameters.AddWithValue("pd_count", data.pd_count);
                        cmd.Parameters.AddWithValue("store_code", data.store_code);

                        cmd.Parameters.AddWithValue("station_no", data.station_no);
                        cmd.Parameters.AddWithValue("aisle_no", data.aisle_no);
                        cmd.Parameters.AddWithValue("slot_no", data.slot_no);
                        cmd.Parameters.AddWithValue("case_volume", data.case_volume);
                        cmd.Parameters.AddWithValue("pieces_num", data.pieces_num);

                        cmd.Parameters.AddWithValue("create_date", data.create_date);
                        cmd.Parameters.AddWithValue("create_time", data.create_time);
                        cmd.Parameters.AddWithValue("create_login_id", data.create_login_id);
                        cmd.Parameters.AddWithValue("renew_date", data.renew_date);
                        cmd.Parameters.AddWithValue("renew_time", data.renew_time);
                        cmd.Parameters.AddWithValue("renew_login_id", data.renew_login_id);

                        cmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                rc = 1;
            }
            finally
            {
                Thread.Sleep(500);
                // 切断
                rc = Debug_Close(connection);
            }
            return rc;
        }


        /// <summary>
        /// 商品ヘッダ データ作成
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private UInt32 DataCreate(out List<ModelPickHead> pickHeads)
        {
            UInt32 rc = 0;
            pickHeads = new List<ModelPickHead>();
            try
            {
                ModelPickHead pickHead1 = new ModelPickHead()
                {
                    delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                    post_no = "1",
                    delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                    post_no_order = "1",
                    sku_code = "000253",
                    pd_count = 1,

                    //sku_name = "森永　飲めちゃう牛乳プリン　　２０本　　　　",
                    sku_name = "野菜生活スムージーグリーン　　３３０ｍｌ　　",
                    jan_code = "9900000000001",
                    case_volume = 0,
                    pieces_num_total = 1000,
                    //sku_kana = "*20*ﾓﾘﾅｶﾞ ｷﾞｭｳﾆｭｳﾌﾟﾘﾝ ",
                    sku_kana = "ﾔｻｲｾｲｶﾂｽﾑｰｼﾞｰｸﾞﾘｰﾝ    ",
                    max_stack_num = "1",
                    sales_price = 10,
                    pick_class = "0",

                    pieces_num_st1 = 0,
                    pieces_num_st2 = 0,
                    pieces_num_st3 = 0,
                    pieces_num_st4 = 0,
                    pieces_num_st5 = 0,
                    pieces_num_st6 = 0,
                    pieces_num_st7 = 0,
                    pieces_num_st8 = 0,
                    pieces_num_st9 = 0,

                    store_num_st1 = 0,
                    store_num_st2 = 0,
                    store_num_st3 = 0,
                    store_num_st4 = 0,
                    store_num_st5 = 0,
                    store_num_st6 = 0,
                    store_num_st7 = 0,
                    store_num_st8 = 0,
                    store_num_st9 = 0,

                    create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                    create_time = $"101500",
                    create_login_id = "admin",
                    renew_date = "",
                    renew_time = "",
                    renew_login_id = "",
                };
                pickHeads.Add(pickHead1);

                ModelPickHead pickHead2 = new ModelPickHead()
                {
                    delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                    post_no = "1",
                    delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                    post_no_order = "1",
                    sku_code = "000349",
                    pd_count = 1,

                    sku_name = "未◆ＬＷＭブリオッシュ　　　　カスタード　Ｃ",
                    jan_code = "9900000000002",
                    case_volume = 0,
                    pieces_num_total = 1000,
                    sku_kana = "LWMﾌﾞﾘｵｯｼｭ ｶｽﾀｰﾄﾞ     ",
                    max_stack_num = "1",
                    sales_price = 10,
                    pick_class = "0",

                    pieces_num_st1 = 0,
                    pieces_num_st2 = 0,
                    pieces_num_st3 = 0,
                    pieces_num_st4 = 0,
                    pieces_num_st5 = 0,
                    pieces_num_st6 = 0,
                    pieces_num_st7 = 0,
                    pieces_num_st8 = 0,
                    pieces_num_st9 = 0,

                    store_num_st1 = 0,
                    store_num_st2 = 0,
                    store_num_st3 = 0,
                    store_num_st4 = 0,
                    store_num_st5 = 0,
                    store_num_st6 = 0,
                    store_num_st7 = 0,
                    store_num_st8 = 0,
                    store_num_st9 = 0,

                    create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                    create_time = $"101500",
                    create_login_id = "admin",
                    renew_date = "",
                    renew_time = "",
                    renew_login_id = "",
                };
                pickHeads.Add(pickHead2);

                ModelPickHead pickHead3 = new ModelPickHead()
                {
                    delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                    post_no = "1",
                    delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                    post_no_order = "1",
                    sku_code = "009106",
                    pd_count = 1,

                    sku_name = "海鮮恵方巻３本入　　　　　　　　　１日１便　",
                    jan_code = "9900000000003",
                    case_volume = 0,
                    pieces_num_total = 1000,
                    sku_kana = "ｶｲｾﾝｴﾎｳﾏｷ3ﾎﾝｲﾘ   1ﾆﾁ1 ",
                    max_stack_num = "1",
                    sales_price = 10,
                    pick_class = "0",

                    pieces_num_st1 = 0,
                    pieces_num_st2 = 0,
                    pieces_num_st3 = 0,
                    pieces_num_st4 = 0,
                    pieces_num_st5 = 0,
                    pieces_num_st6 = 0,
                    pieces_num_st7 = 0,
                    pieces_num_st8 = 0,
                    pieces_num_st9 = 0,

                    store_num_st1 = 0,
                    store_num_st2 = 0,
                    store_num_st3 = 0,
                    store_num_st4 = 0,
                    store_num_st5 = 0,
                    store_num_st6 = 0,
                    store_num_st7 = 0,
                    store_num_st8 = 0,
                    store_num_st9 = 0,

                    create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                    create_time = $"101500",
                    create_login_id = "admin",
                    renew_date = "",
                    renew_time = "",
                    renew_login_id = "",
                };
                pickHeads.Add(pickHead3);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                rc = 1;
            }
            return rc;
        }

        /// <summary>
        /// 店別小仕分け データ作成
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private UInt32 DataCreate02(out List<ModelPickDetail> pickDetails)
        {
            UInt32 rc = 0;
            pickDetails = new List<ModelPickDetail>();
            try
            {
                for (int i = 0; i < 36; i++)
                {
                    ModelPickDetail pickDetail1 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "000253",
                        pd_count = 1,
                        store_code = $"1001{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "1",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail1);

                    ModelPickDetail pickDetail2 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "000253",
                        pd_count = 1,
                        store_code = $"1002{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "1",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail2);

                    ModelPickDetail pickDetail3 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "000253",
                        pd_count = 1,
                        store_code = $"1003{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "2",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail3);

                    ModelPickDetail pickDetail4 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "000253",
                        pd_count = 1,
                        store_code = $"1004{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "2",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail4);

                    ModelPickDetail pickDetail5 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "000253",
                        pd_count = 1,
                        store_code = $"1005{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "3",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail5);

                    ModelPickDetail pickDetail6 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "000253",
                        pd_count = 1,
                        store_code = $"1006{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "3",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail6);

                    ModelPickDetail pickDetail7 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "000253",
                        pd_count = 1,
                        store_code = $"1007{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "4",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail7);

                    ModelPickDetail pickDetail8 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "000253",
                        pd_count = 1,
                        store_code = $"1008{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "4",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail8);
                }

                for (int i = 0; i < 36; i++)
                {
                    ModelPickDetail pickDetail1 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "000349",
                        pd_count = 1,
                        store_code = $"1001{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "1",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail1);

                    ModelPickDetail pickDetail2 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "000349",
                        pd_count = 1,
                        store_code = $"1002{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "1",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail2);

                    ModelPickDetail pickDetail3 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "000349",
                        pd_count = 1,
                        store_code = $"1003{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "2",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail3);

                    ModelPickDetail pickDetail4 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "000349",
                        pd_count = 1,
                        store_code = $"1004{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "2",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail4);

                    ModelPickDetail pickDetail5 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "000349",
                        pd_count = 1,
                        store_code = $"1005{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "3",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail5);

                    ModelPickDetail pickDetail6 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "000349",
                        pd_count = 1,
                        store_code = $"1006{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "3",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail6);

                    ModelPickDetail pickDetail7 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "000349",
                        pd_count = 1,
                        store_code = $"1007{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "4",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail7);

                    ModelPickDetail pickDetail8 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "000349",
                        pd_count = 1,
                        store_code = $"1008{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "4",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail8);
                }

                for (int i = 0; i < 36; i++)
                {
                    ModelPickDetail pickDetail1 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "009106",
                        pd_count = 1,
                        store_code = $"1001{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "1",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail1);

                    ModelPickDetail pickDetail2 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "009106",
                        pd_count = 1,
                        store_code = $"1002{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "1",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail2);

                    ModelPickDetail pickDetail3 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "009106",
                        pd_count = 1,
                        store_code = $"1003{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "2",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail3);

                    ModelPickDetail pickDetail4 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "009106",
                        pd_count = 1,
                        store_code = $"1004{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "2",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail4);

                    ModelPickDetail pickDetail5 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "009106",
                        pd_count = 1,
                        store_code = $"1005{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "3",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail5);

                    ModelPickDetail pickDetail6 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "009106",
                        pd_count = 1,
                        store_code = $"1006{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "3",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail6);

                    ModelPickDetail pickDetail7 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "009106",
                        pd_count = 1,
                        store_code = $"1007{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "4",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail7);

                    ModelPickDetail pickDetail8 = new ModelPickDetail()
                    {
                        delivery_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no = "1",
                        delivery_date_order = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        post_no_order = "1",
                        sku_code = "009106",
                        pd_count = 1,
                        store_code = $"1008{(i + 1).ToString("D2")}",

                        station_no = "1",
                        aisle_no = "4",
                        slot_no = $"{i + 1}",
                        case_volume = 0,
                        pieces_num = 10,

                        create_date = $"{_dtNow.Year.ToString("D4")}{_dtNow.Month.ToString("D2")}{_dtNow.Day.ToString("D2")}",
                        create_time = $"101500",
                        create_login_id = "admin",
                        renew_date = "",
                        renew_time = "",
                        renew_login_id = "",
                    };
                    pickDetails.Add(pickDetail8);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                rc = 1;
            }
            return rc;
        }


        /// <summary>
        /// デバッグ
        /// 全テーブル削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public UInt32 Debug_DeleteTable()
        {
            UInt32 rc = 0;
            NpgsqlConnection connection = null;
            List<string> tablenames = new List<string>();
            try
            {
                // 接続
                rc = Debug_Connect(out connection);


                // データベース内のすべてのテーブル名を取得するSQLクエリ
                string selectQuery = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';";
                using (var command = new NpgsqlCommand(selectQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tableName = reader["table_name"].ToString();
                            tablenames.Add(tableName);
                        }
                    }
                }


                // 全テーブル削除
                string deleteQuery = "";
                foreach (string tableName in tablenames)
                {
                    deleteQuery = $"DROP TABLE IF EXISTS {tableName};";
                    using (var command = new NpgsqlCommand(deleteQuery, connection))
                    {
                        try
                        {
                            command.ExecuteNonQuery();
                            Console.WriteLine($"テーブル '{tableName}' を削除しました。");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"テーブル '{tableName}' の削除中にエラーが発生しました: {ex.Message}");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                // 切断
                rc = Debug_Close(connection);
            }
            return rc;
        }


        /// <summary>
        /// デバッグ
        /// 全レコード削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public UInt32 Debug_DeleteRecord()
        {
            UInt32 rc = 0;
            NpgsqlConnection connection = null;
            List<string> tablenames = new List<string>();
            try
            {
                // 接続
                rc = Debug_Connect(out connection);


                // データベース内のすべてのテーブル名を取得するSQLクエリ
                string selectQuery = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';";
                using (var command = new NpgsqlCommand(selectQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tableName = reader["table_name"].ToString();
                            tablenames.Add(tableName);
                        }
                    }
                }


                // 全レコード削除
                string deleteQuery = "";
                foreach (string tableName in tablenames)
                {
                    // テーブル内の全レコード削除
                    deleteQuery = $"DELETE FROM {tableName};";
                    using (var command = new NpgsqlCommand(deleteQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                Thread.Sleep(500);
                // 切断
                rc = Debug_Close(connection);
            }
            return rc;
        }


    }



    public class ModelPickHead
    {
        public string delivery_date { get; set; }
        public string post_no { get; set; }
        public string delivery_date_order { get; set; }
        public string post_no_order { get; set; }
        public string sku_code { get; set; }
        public decimal pd_count { get; set; }

        public string sku_name { get; set; }
        public string jan_code { get; set; }
        public decimal case_volume { get; set; }
        public decimal pieces_num_total { get; set; }
        public string sku_kana { get; set; }
        public string max_stack_num { get; set; }
        public decimal sales_price { get; set; }
        public string pick_class { get; set; }

        public decimal pieces_num_st1 { get; set; }
        public decimal pieces_num_st2 { get; set; }
        public decimal pieces_num_st3 { get; set; }
        public decimal pieces_num_st4 { get; set; }
        public decimal pieces_num_st5 { get; set; }
        public decimal pieces_num_st6 { get; set; }
        public decimal pieces_num_st7 { get; set; }
        public decimal pieces_num_st8 { get; set; }
        public decimal pieces_num_st9 { get; set; }

        public decimal store_num_st1 { get; set; }
        public decimal store_num_st2 { get; set; }
        public decimal store_num_st3 { get; set; }
        public decimal store_num_st4 { get; set; }
        public decimal store_num_st5 { get; set; }
        public decimal store_num_st6 { get; set; }
        public decimal store_num_st7 { get; set; }
        public decimal store_num_st8 { get; set; }
        public decimal store_num_st9 { get; set; }

        public string create_date { get; set; }
        public string create_time { get; set; }
        public string create_login_id { get; set; }
        public string renew_date { get; set; }
        public string renew_time { get; set; }
        public string renew_login_id { get; set; }
    }
    public class ModelPickDetail
    {
        public string delivery_date { get; set; }
        public string post_no { get; set; }
        public string delivery_date_order { get; set; }
        public string post_no_order { get; set; }
        public string sku_code { get; set; }
        public decimal pd_count { get; set; }
        public string store_code { get; set; }

        public string station_no { get; set; }
        public string aisle_no { get; set; }
        public string slot_no { get; set; }
        public decimal case_volume { get; set; }
        public decimal pieces_num { get; set; }

        public string create_date { get; set; }
        public string create_time { get; set; }
        public string create_login_id { get; set; }
        public string renew_date { get; set; }
        public string renew_time { get; set; }
        public string renew_login_id { get; set; }
    }


}
