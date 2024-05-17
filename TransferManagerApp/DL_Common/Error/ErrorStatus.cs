//----------------------------------------------------------
// Copyright © 2018 DATALINK
//----------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DL_CommonLibrary;
using ErrorCodeDefine;
namespace DL_CommonLibrary
{
    /// <summary>
    /// エラー詳細
    /// </summary>
    public class ErrorDetail
    {
        /// <summary>
        /// エラーコード
        /// </summary>
        public UInt32 code = 0;

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string message = "";

        /// <summary>
        /// 発生日時
        /// </summary>
        public DateTime occurTime = new DateTime(0);

        /// <summary>
        /// 解除日時
        /// </summary>
        public DateTime resetTime = new DateTime(0);

        /// <summary>
        /// リセット状態か確認
        /// </summary>
        public bool IsReset
        {
            get
            {
                return resetTime.Ticks > 0;
            }
        }

        /// <summary>
        /// コピー
        /// </summary>
        /// <param name="dest"></param>
        public void Copy(ref ErrorDetail dest)
        {
            dest.code = this.code;
            dest.message = this.message;
            dest.occurTime = this.occurTime;
            dest.resetTime = this.resetTime;
        }
    }

    /// <summary>
    /// エラーステータス
    /// </summary>
    public class ErrorStatus
    {
        private const int maxErrorCount = 100;
        /// <summary>
        /// エラー詳細
        /// </summary>
        public List<ErrorDetail> Item = new List<ErrorDetail>();

        /// <summary>
        /// 最新のエラー情報取得
        /// </summary>
        /// <returns></returns>
        public ErrorDetail GetLatestErrorInfo()
        {
            ErrorDetail info = new ErrorDetail();

            // コピー
            if (Item.Count > 0)
                Item[Item.Count - 1].Copy(ref info);

            return info;
        }

        /// <summary>
        /// エラー設定
        /// </summary>
        /// <param name="errCode"></param>
        public void SetLatestErrorInfo(UInt32 errCode)
        {
            ErrorDetail info = new ErrorDetail();
            info.code = errCode;
            info.occurTime = DateTime.Now;
            info.message = ErrorManager.GetErrorMessage((ErrorCodeList)errCode);
            if (Item.Count >= maxErrorCount)
                Item.RemoveAt(0);
            Item.Add(info);

        }
        /// <summary>
        /// エラー設定
        /// </summary>
        /// <param name="errCode"></param>
        public void SetLatestErrorInfo(UInt32 errCode, string msg)
        {
            ErrorDetail info = new ErrorDetail();
            string codeMsg = ErrorManager.GetErrorMessage((ErrorCodeList)errCode);
            info.code = errCode;
            info.occurTime = DateTime.Now;
            //if (codeMsg == "")
            //    info.message = errCode.ToString() + "," + msg;
            //else
            //    info.message = ErrorManager.GetErrorMessage((ErrorCodeList)errCode) + "," + msg;
            info.message = msg;
            if (Item.Count >= maxErrorCount)
                Item.RemoveAt(0);
            Item.Add(info);
        }

        /// <summary>
        /// 最新のエラーを確認済み状態にする
        /// </summary>
        public void ResetLatestError()
        {
            // コピー
            if (Item.Count > 0)
                Item[Item.Count - 1].resetTime = DateTime.Now;
        }

    }
}
