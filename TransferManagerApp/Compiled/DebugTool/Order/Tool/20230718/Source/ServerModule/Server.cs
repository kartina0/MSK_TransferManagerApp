using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            }
            catch (Exception ex)
            {

            }
        }
    }
}
