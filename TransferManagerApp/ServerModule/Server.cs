//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;


namespace ServerModule
{
    /// <summary>
    /// 上位サーバークラス
    /// </summary>
    public class Server
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "Server";

        /// <summary>
        /// 仕分データ管理クラス
        /// </summary>
        public OrderInfoManager OrderInfo = null;
        /// <summary>
        /// マスターファイル管理クラス
        /// </summary>
        public MasterFileManager MasterFile = null;
        /// <summary>
        /// PICKDATA管理クラス
        /// </summary>
        public PickDataManager PickData = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Server()
        {
            try
            {
                // 仕分データ
                OrderInfo = new OrderInfoManager();
                // マスターファイル
                MasterFile = new MasterFileManager();
                // PICKDATA
                PickData = new PickDataManager();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
