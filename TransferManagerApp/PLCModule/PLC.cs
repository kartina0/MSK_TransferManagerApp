//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;

using DL_PlcInterfce;


namespace PLCModule
{
    /// <summary>
    /// PLCクラス
    /// </summary>
    public class PLC
    {
        /// <summary>
        /// PLC 操作
        /// </summary>
        public PLCAccess Access = null;
        /// <summary>
        /// PLC ステータス
        /// </summary>
        public PLCStatus Status = null;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PLC(string connectionString) 
        {
            Access = new PLCAccess(connectionString);
            Status = new PLCStatus();
        }

        /// <summary>
        /// 接続
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public UInt32 Open(string connectionString) 
        {
            UInt32 rc = 0;
            try
            {
                rc = Access.Open(connectionString);
            }
            catch (Exception ex)
            {

            }
            return rc;
        }
        /// <summary>
        /// 切断
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public UInt32 Close()
        {
            UInt32 rc = 0;
            try
            {
                Access.Close();
            }
            catch (Exception ex)
            {

            }
            return rc;
        }
    }


}
