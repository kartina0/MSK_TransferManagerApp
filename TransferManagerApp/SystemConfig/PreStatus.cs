using System;
using System.Linq;

using DL_CommonLibrary;
using ErrorCodeDefine;


namespace SystemConfig
{
    public static class PreStatus
    {

        /// <summary>
        /// 便No
        /// </summary>
        public static int PostIndex = 0;
        /// <summary>
        /// 便開始日時
        /// </summary>
        public static DateTime[] PostStartDt = null;
        /// <summary>
        /// 便終了日時
        /// </summary>
        public static DateTime[] PostEndDt = null;




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
                string section = "";
                bool exist = false;
                string sBuf = "";
                fileName = System.IO.Path.GetFullPath(fileName);

                if (!FileIo.ExistFile(fileName)) rc = (UInt32)ErrorCodeList.FILE_NOT_FOUND;

                if (STATUS_SUCCESS(rc))
                {
                    section = "STATUS";
                    exist = FileIo.ReadIniFile<int>(fileName, section, "POST_INDEX", ref PostIndex);

                    PostStartDt = new DateTime[Const.MaxPostCount];
                    sBuf = "";
                    exist = FileIo.ReadIniFile<string>(fileName, section, "POST_START_DATETIME", ref sBuf);
                    string[] split = sBuf.Split(',');
                    for (int postIndex = 0; postIndex < Const.MaxPostCount; postIndex++) 
                    {
                        if (split.Length < postIndex + 1)
                            break;
                        PostStartDt[postIndex] = DateTime.Parse(split[postIndex]);
                    }

                    PostEndDt = new DateTime[Const.MaxPostCount];
                    sBuf = "";
                    exist = FileIo.ReadIniFile<string>(fileName, section, "POST_END_DATETIME", ref sBuf);
                    split = null;
                    split = sBuf.Split(',');
                    for (int postIndex = 0; postIndex < Const.MaxPostCount; postIndex++)
                    {
                        if (split.Length < postIndex + 1)
                            break;
                        PostEndDt[postIndex] = DateTime.Parse(split[postIndex]);
                    }

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
        /// postIndex
        /// </summary>
        /// <returns></returns>
        public static UInt32 Save(string fileName)
        {
            UInt32 rc = 0;
            try
            {
                string section = "";
                string sBuf = "";
                fileName = System.IO.Path.GetFullPath(fileName);

                if (!FileIo.ExistFile(fileName)) rc = (UInt32)ErrorCodeList.FILE_NOT_FOUND;

                if (STATUS_SUCCESS(rc))
                {
                    section = "STATUS";

                    sBuf = PostIndex.ToString();
                    FileIo.WriteIniValue(fileName, section, "POST_INDEX", sBuf);

                    sBuf = "";
                    for (int postIndex = 0; postIndex < Const.MaxPostCount; postIndex++)
                        sBuf += $"{PostStartDt[postIndex].ToString()},";
                    FileIo.WriteIniValue(fileName, section, "POST_START_DATETIME", sBuf);

                    sBuf = "";
                    for (int postIndex = 0; postIndex < Const.MaxPostCount; postIndex++)
                        sBuf += $"{PostEndDt[postIndex].ToString()},";
                    FileIo.WriteIniValue(fileName, section, "POST_END_DATETIME", sBuf);
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
