//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;


namespace ServerModule
{
    /// <summary>
    /// 商品マスタファイル データ
    /// </summary>
    public class MasterWork
    {
        /// <summary>
        /// 納品日
        /// </summary>
        public DateTime orderDate = DateTime.MinValue;
        /// <summary>
        /// 取引先コード
        /// </summary>
        public string supplierCode = "";
        /// <summary>
        /// VDRコード
        /// </summary>
        public string VDRCode = "";
        /// <summary>
        /// 取引先名(漢字)
        /// </summary>
        public string supplierName = "";
        /// <summary>
        /// 商品コード
        /// </summary>
        public string workCode = "";
        /// <summary>
        /// 便No
        /// </summary>
        public int postNo = 0;
        /// <summary>
        /// JANコード
        /// </summary>
        public string JANCode = "";
        /// <summary>
        /// JAN区分
        /// </summary>
        public int JANClass = 0;
        /// <summary>
        /// 商品名(漢字)
        /// </summary>
        public string workName = "";
        /// <summary>
        /// DEPT-CLASS
        /// </summary>
        public string deptClass = "";
        /// <summary>
        /// センター入数
        /// </summary>
        public double centerCount = 0;
        /// <summary>
        /// パック入数
        /// </summary>
        public double packCount = 0;
        /// <summary>
        /// JANコード(下4桁)
        /// </summary>
        public string JANCode4digits = "";
        /// <summary>
        /// 商品名(カナ)
        /// </summary>
        public string workNameKana = "";

    }
}
