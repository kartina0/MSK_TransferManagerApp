//---------------------------------------------------------
// Copyright © 2023 DATALINK
//---------------------------------------------------------
using System;
using System.Collections.Generic;

using DL_CommonLibrary;
using ErrorCodeDefine;


namespace ShareResource
{
    /// <summary>
    /// ユーザーレベル
    /// </summary>
    public enum UserLevel
    {
        /// <summary>作業者</summary>
        Operator = 0,
        /// <summary>管理者</summary>
        Admin,
        /// <summary>開発者</summary>
        Developer,
    }



    public class UserInformation
    {

        public bool IsExist
        {
            get { return ID != null && PassWord != null && ID != "" && PassWord != ""; }
        }

        public string ID { get; set; } = "";
        public string PassWord { get; set; } = "";
        public UserLevel Level { get; set; } = UserLevel.Operator;
        public string Level_Disp
        {
            get
            {
                if (Level == UserLevel.Operator)
                    return "通常作業者";
                else if (Level == UserLevel.Admin)
                    return "管理者";
                else
                    return "";
            }
        }

        /// <summary>
        /// 文字列取得
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (IsExist)
                return ID + "," + PassWord + Level.ToString();
            return "";
        }

    }

    /// <summary>
    /// ログイン ユーザー情報
    /// </summary>
    public class LoginUserInformation
    {
        /// <summary>
        /// 最大ユーザー数
        /// </summary>
        public const int MaxUserCount = 10;

        /// <summary>
        /// ファイルパス
        /// </summary>
        private string _filePath = "";

        /// <summary>
        /// ユーザー情報
        /// </summary>
        public List<UserInformation> UserInfo = new List<UserInformation>();

        /// <summary>
        /// 現在ユーザーレベル
        /// </summary>
        public UserLevel CurrentLevel = UserLevel.Operator;

        /// <summary>
        /// 現在ID
        /// </summary>
        public string CurrentID { get; set; } = "";

        /// <summary>
        /// ファイル読み込み
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public UInt32 Load(string filePath)
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
                filePath = System.IO.Path.GetFullPath(filePath);

                if (!FileIo.ExistFile(filePath)) rc = (UInt32)ErrorCodeList.FILE_NOT_FOUND;

                if (STATUS_SUCCESS(rc))
                {
                    _filePath = filePath;
                    section = "USER";

                    for (int i = 0; i < MaxUserCount; i++)
                    {
                        string id = "";
                        string pass = "";
                        UserLevel level = UserLevel.Operator;
                        key = string.Format("ID[{0}]", i);
                        exist = FileIo.ReadIniFile(_filePath, section, key, ref sBuf);
                        if (exist) id = sBuf;
                        key = string.Format("PASS[{0}]", i);
                        exist = FileIo.ReadIniFile(_filePath, section, key, ref sBuf);
                        if (exist) pass = sBuf;
                        key = string.Format("LEVEL[{0}]", i);
                        exist = FileIo.ReadIniFile(_filePath, section, key, ref sBuf);
                        if (exist)
                        {
                            if (!Enum.TryParse<UserLevel>(sBuf, out level))
                                level = UserLevel.Operator;
                        }
                        else
                        {
                            level = UserLevel.Operator;
                        }

                        if (id != "" && pass != "")
                        {
                            UserInformation info = new UserInformation();
                            info.ID = id;
                            info.PassWord = pass;
                            info.Level = level;
                            UserInfo.Add(info);
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
        /// ファイル書込み
        /// </summary>
        /// <returns></returns>
        public UInt32 Save()
        {
            return Save(_filePath);
        }

        /// <summary>
        /// ファイル書込み
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public UInt32 Save(string filePath)
        {

            UInt32 rc = 0;
            try
            {

                string section = "";
                string key = "";
                // Get Full Path.
                filePath = System.IO.Path.GetFullPath(filePath);

                if (STATUS_SUCCESS(rc))
                {
                    _filePath = filePath;
                    section = "USER";

                    // 一旦ファイル上のキーを削除
                    FileIo.DeleteFile(filePath);
                    System.Threading.Thread.Sleep(100);
                    for (int i = 0; i < MaxUserCount; i++)
                    {
                        if (UserInfo.Count > i)
                        {
                            key = string.Format("ID[{0}]", i);
                            FileIo.WriteIniValue(_filePath, section, key, UserInfo[i].ID);
                            key = string.Format("PASS[{0}]", i);
                            FileIo.WriteIniValue(_filePath, section, key, UserInfo[i].PassWord);
                            key = string.Format("LEVEL[{0}]", i);
                            FileIo.WriteIniValue(_filePath, section, key, UserInfo[i].Level.ToString());
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
        /// IDからユーザー情報を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserInformation GetUserInfo(string id)
        {
            UserInformation info = null;

            try
            {
                for (int i = 0; i < UserInfo.Count; i++)
                {
                    info = UserInfo[i];
                    if (info.ID == id)
                    {
                        break;
                    }
                }
            }
            catch { }
            return info;
        }

        /// <summary>
        /// Check Error State
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        private bool STATUS_SUCCESS(UInt32 err)
        {
            return err == (int)ErrorCodeList.STATUS_SUCCESS;
        }

        /// <summary>
        /// コピー
        /// </summary>
        /// <param name="dest"></param>
        public void Copy(ref LoginUserInformation dest)
        {
            if (dest == null) dest = new LoginUserInformation();
            dest.UserInfo.Clear();
            for (int i = 0; i < UserInfo.Count; i++)
            {

                UserInformation info = new UserInformation();
                info.ID = UserInfo[i].ID;
                info.PassWord = UserInfo[i].PassWord;
                info.Level = UserInfo[i].Level;

                dest.UserInfo.Add(info);
            }

        }
    }
}
