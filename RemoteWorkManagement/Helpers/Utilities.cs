using System;
using System.Net;
using System.Net.Mail;

namespace RemoteWorkManagement.Helpers
{
    public class Utilities
    {
        /// <summary>
        /// emails sender.
        /// </summary>
        /// <param name="mailto">The mailto.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static bool MailSender(string mailto, string password)
        {
            try
            {
                SmtpClient smtp = new SmtpClient(System.Configuration.ConfigurationSettings.AppSettings["smtpServer"],
                    Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["smtpPort"]));
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(System.Configuration.ConfigurationSettings.AppSettings["emailAccount"], 
                    System.Configuration.ConfigurationSettings.AppSettings["emailPassword"]);
                MailMessage mail = new MailMessage(
                    System.Configuration.ConfigurationSettings.AppSettings["emailAccount"], 
                    mailto, "Please do not reply to this message"
                    , "Your temporary password is: " + password);
                smtp.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}