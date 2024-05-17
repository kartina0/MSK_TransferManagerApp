// ----------------------------------------------
// Copyright © 2017 DATALINK
// ----------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace DL_CustomCtrl
{
    internal static class LicenceCheck
    {
        /// <summary>
        /// ライセンス有無を確認
        /// ソリューションフォルダにクラス名.licファイルがあればOK
        /// </summary>
        /// <param name="ctrlName"></param>
        /// <returns></returns>
        public static bool IsLicence(string ctrlName)
        {
            bool ret = false;
            string fname = System.IO.Path.GetFullPath(ctrlName + ".lic");
            //System.Windows.Forms.MessageBox.Show(fname);
            ret = System.IO.File.Exists(fname);
            return ret;
        }
    }
}
