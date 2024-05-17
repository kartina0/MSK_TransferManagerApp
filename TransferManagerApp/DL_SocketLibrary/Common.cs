//----------------------------------------------------------
// Copyright © 2017 DATALINK
//----------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace DL_Socket
{
    class Common
    {

        public byte[] StringToByteArray(String pString)
        {
            byte[] retVal = new Byte[0];
            try
            {
                retVal = new byte[pString.Length];

                for (int iCtr = 1; iCtr <= pString.Length; iCtr++)
                {
                    retVal[iCtr - 1] = Convert.ToByte(Convert.ToChar(pString.Substring(iCtr - 1, 1)));

                }
            }
            catch { }
            return retVal;
        }


        public string ByteArrayToString(byte[] pByteArray)
        {
            string strData = "";
            foreach (byte b in pByteArray)
            {
                strData += Convert.ToChar(b).ToString();
            }

            return strData;
        }

        public Byte[] CombineByteArray(Byte[] bySource1, Byte[] bySource2)
        {
            Byte[] byReturnValue = new Byte[bySource1.Length + bySource2.Length];
            Array.Copy(bySource1, 0, byReturnValue, 0, bySource1.Length);
            Array.Copy(bySource2, 0, byReturnValue, bySource1.Length, bySource2.Length);

            return byReturnValue;
        }

        public String[] CombineArray(String[] pSource1, String[] pSource2)
        {
            String[] strReturnValue = new String[pSource1.Length + pSource2.Length];
            Array.Copy(pSource1, 0, strReturnValue, 0, pSource1.Length);
            Array.Copy(pSource2, 0, strReturnValue, pSource1.Length, pSource2.Length);

            return strReturnValue;
        }
    }
}
