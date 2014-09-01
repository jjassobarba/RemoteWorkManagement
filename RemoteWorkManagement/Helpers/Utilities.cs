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
                var smtp = new SmtpClient(System.Configuration.ConfigurationManager.AppSettings["smtpServer"],
                    Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["smtpPort"]))
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials =
                        new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["emailAccount"],
                            System.Configuration.ConfigurationManager.AppSettings["emailPassword"])
                };
                var mail = new MailMessage(
                    System.Configuration.ConfigurationManager.AppSettings["emailAccount"],
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