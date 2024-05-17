using ErrorCodeDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SystemConfig;

namespace ServerModule
{
    /// <summary>
    /// 上位サーバーのマスタファイル管理
    /// </summary>
    public class MasterFileManager
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "FTP";

        /// <summary>
        /// IPアドレス
        /// </summary>
        private string _ipAddress = "";

        /// <summary>
        /// 商品マスタリスト
        /// </summary>
        public List<MasterWork> WorkMasterList = new List<MasterWork>();
        /// <summary>
        /// 店マスタリスト
        /// </summary>
        public List<MasterStore> StoreMasterList = new List<MasterStore>();
        /// <summary>
        /// 作業者マスタリスト
        /// </summary>
        public List<MasterWorker> WorkerMasterList = new List<MasterWorker>();



        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MasterFileManager()
        {
            try
            {
                _ipAddress = IniFile.ServerIpAddress;
            }
            catch (Exception ex)
            {
            }
        }


        /// <summary>
        /// 接続確認
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            bool updated = false;
            try
            {
            }
            catch (Exception ex)
            {
            }
            return updated;
        }
        /// <summary>
        /// サーバーのファイルの更新確認
        /// </summary>
        /// <returns></returns>
        public bool IsUpdatedFile(string filePath)
        {
            bool updated = false;
            try
            {
            }
            catch (Exception ex)
            {
            }
            return updated;
        }



        /// <summary>
        /// 商品マスタファイル 読み出し
        /// </summary>
        /// <returns>エラーコード</returns>
        public uint ReadWorkMasterFile(string filePath)
        {
            UInt32 rc = 0;
            try
            {

            }
            catch (Exception ex)
            {
                rc = (uint)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// 店マスタファイル 読み出し
        /// </summary>
        /// <returns>エラーコード</returns>
        public uint ReadStoreMasterFile(string filePath)
        {
            UInt32 rc = 0;
            try
            {

            }
            catch (Exception ex)
            {
                rc = (uint)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// 作業者マスタファイル 読み出し
        /// </summary>
        /// <returns>エラーコード</returns>
        public uint ReadWorkerMasterFile(string filePath)
        {
            UInt32 rc = 0;
            try
            {

            }
            catch (Exception ex)
            {
                rc = (uint)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }


        /// <summary>
        /// マスタファイルをローカルにコピー
        /// </summary>
        /// <returns>エラーコード</returns>
        public uint CopyMasterFile(string srcPath, string destPath)
        {
            UInt32 rc = 0;
            try
            {

            }
            catch (Exception ex)
            {
                rc = (uint)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        

    }
}
