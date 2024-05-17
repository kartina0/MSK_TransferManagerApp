using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using ErrorCodeDefine;

namespace DL_CommonLibrary
{


    /// <summary>
    /// E-Mailクラス
    /// </summary>
    public static class eMail
    {
        /// <summary>
        /// メール送信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="recipient"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static UInt32 SendMail(string smtp, int port , string userName, string passWord, string sendName, string sendAddress , string recvtName,string recvAddress, string subject, string body)
        {
            UInt32 rc = 0;
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(sendAddress, sendName);
                msg.To.Add(new MailAddress(recvAddress, recvtName));

                msg.Subject = subject;
                msg.Body = body;

                System.Net.Mail.SmtpClient sc = new System.Net.Mail.SmtpClient();
                //SMTPサーバーなどを設定する
                sc.Host = smtp;
                sc.Port = port;
                sc.Credentials = new NetworkCredential(userName, passWord);

                sc.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                //メッセージを送信する
                sc.Send(msg);

                //後始末
                msg.Dispose();
                //後始末（.NET Framework 4.0以降）
                sc.Dispose();

            }
            catch (Exception ex)
            {
                rc = (UInt32)ErrorCodeList.EXCEPTION;
                ErrorManager.ErrorHandler(ex);
            }
            return rc;
        }

        /// <summary>
        /// メール送信（認証無し）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="recipient"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static UInt32 SendMail(string smtp, int port, string sendName, string sendAddress, string recvtName, string recvAddress, string subject, string body)
        {
            UInt32 rc = 0;
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(sendAddress, sendName);
                msg.To.Add(new MailAddress(recvAddress, recvtName));

                msg.Subject = subject;
                msg.Body = body;

                System.Net.Mail.SmtpClient sc = new System.Net.Mail.SmtpClient();
                //SMTPサーバーなどを設定する
                sc.Host = smtp;
                sc.Port = port;

                sc.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                //メッセージを送信する
                sc.Send(msg);

                //後始末
                msg.Dispose();
                //後始末（.NET Framework 4.0以降）
                sc.Dispose();

            }
            catch (Exception ex)
            {
                rc = (UInt32)ErrorCodeList.EXCEPTION;
                ErrorManager.ErrorHandler(ex);
            }
            return rc;
        }


    }
}
