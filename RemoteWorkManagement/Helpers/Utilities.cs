using System;
using System.Net;
using System.Net.Mail;

namespace RemoteWorkManagement.Helpers
{
    public class Utilities
    {
        private const string NewUserTemplate = "<html><head><title> Welcome to Remote Work Administration System! </title></head><body style='font-family: sans-serif, helvetica, arial;'><img src='http://s9.postimg.org/6suk6q4vj/Banner_Rewoma.png' alt='Remote Logo'><h3 style='color: #5090C1;'> Remote Work Administration System! </h3><p>This e-mail confirms that you are registered on REWOMA system. <h5>YOUR ACCOUNT INFORMATION:</h5><ul><li style='list-style-type: none;'><b>Password:</b> @fieldValue (you can change it the first time you login to the system)</li></ul></p><p>Go to <a href='http://remotework.scio.local/Account/Login'>REWOMA</a> to start reporting your remote work activity.</p><p>If you need assistance for login to the system, please contact HR Department by sending an e-mail to <a href='mailto:hr@sciodev.com'>hr@sciodev.com</a></p><p>Regards.</p><p>HR Team</p><p style='height: 30px; background-color: #e2e2e2;'>&copy Scio Consulting</p></body></html>";
        private const string RecoverPasswordTemplate = "<html><head><title> Recover Password </title></head><body style='font-family: sans-serif, helvetica, arial;'><img src='http://s9.postimg.org/6suk6q4vj/Banner_Rewoma.png' alt='Remote Logo'><h3 style='color: #5090C1;'> Recover Password </h3><p>These are your temporal new credentials:<h5>YOUR ACCOUNT INFORMATION:</h5><ul><li style='list-style-type: none;'><b>Password:</b> @fieldValue</li></ul></p><p>Go to <a href='http://remotework.scio.local/Account/Login'>REWOMA</a> to start reporting your remote work activity.</p><p>If you need assistance for login to the system, please contact HR Department by sending an e-mail to <a href='mailto:hr@sciodev.com'>hr@sciodev.com</a></p><p>Regards.</p><p>HR Team</p><p style='height: 30px; background-color: #e2e2e2;'>&copy Scio Consulting</p></body></html>";
        private const string ChangeRemoteDayTemplate = "<html><head><title> Change Remote Day </title></head><body style='font-family: sans-serif, helvetica, arial;'><img src='http://s9.postimg.org/6suk6q4vj/Banner_Rewoma.png' alt='Remote Logo'><h3 style='color: #5090C1;'> Change Remote Day </h3><p>The user <b>@username</b> has a new request to change his day to work in remote </p><p>Log in <a href='http://remotework.scio.local/Account/Login'>REWOMA</a> to follow the status.</p><p>HR Team</p><p style='height: 30px; background-color: #e2e2e2;'>&copy Scio Consulting</p></body></html>";
        private const string RequestExceptionTemplate = "<html><head><title> Request Exception </title></head><body style='font-family: sans-serif, helvetica, arial;'><img src='http://s9.postimg.org/6suk6q4vj/Banner_Rewoma.png' alt='Remote Logo'><h3 style='color: #5090C1;'> Request Exception </h3><p>The user <b>@username</b> wants an exception to work in remote. </p><p>Log in <a href='http://remotework.scio.local/Account/Login'>REWOMA</a> to follow the status.</p><p>HR Team</p><p style='height: 30px; background-color: #e2e2e2;'>&copy Scio Consulting</p></body></html>";
        private const string CheckInTemplate = "<html><head><title> Check In Template </title></head><body style='font-family: sans-serif, helvetica, arial;'><img src='http://s9.postimg.org/6suk6q4vj/Banner_Rewoma.png' alt='Remote Logo'><h3 style='color: #5090C1;'> Check In </h3><p>Hello Mates, I am able to star working, feel free to contact me.</p><p><b>@fullname</b></p><p style='height: 30px; background-color: #e2e2e2;'>&copy Scio Consulting</p></body></html>";
        private const string CheckOutTemplate = "<html><head><title> Change Remote Dat </title></head><body style='font-family: sans-serif, helvetica, arial;'><img src='http://s9.postimg.org/6suk6q4vj/Banner_Rewoma.png' alt='Remote Logo'><h3 style='color: #5090C1;'> Check Out </h3><p>My shift for today is done. I'll be offline.</p><p><b>@fullname</b></p><p style='height: 30px; background-color: #e2e2e2;'>&copy Scio Consulting</p></body></html>";
        private const string CheckInTemplateUD = "<html><head><title> Check In Template </title></head><body style='font-family: sans-serif, helvetica, arial;'><img src='http://s9.postimg.org/6suk6q4vj/Banner_Rewoma.png' alt='Remote Logo'><h3 style='color: #5090C1;'> Check In </h3><p>Hello Mates, I am able to star working, feel free to contact me.</p><p><b>Additional Comment.</b></p><p> @comment </p><p><b>@fullname</b></p><p style='height: 30px; background-color: #e2e2e2;'>&copy Scio Consulting</p></body></html>";


        /// <summary>
        /// Enum for  the mail types that we are going to send
        /// </summary>
        public enum EmailType
        {
            Welcome,
            ForgotPassword,
            ChangeRemoteDay,
            RequestException,
            CheckIn,
            CheckOut,
            CheckInUD
        }

        /// <summary>
        /// emails sender.
        /// </summary>
        /// <param name="mailto">The mailto.</param>
        /// <param name="fieldValue">The fieldValue.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static bool MailSender(string mailto, string fieldValue, EmailType type)
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
                var bodyMessage = CheckEmailTemplate(type, fieldValue);
                var mail = 
                    new MailMessage(System.Configuration.ConfigurationManager.AppSettings["emailAccount"],mailto, "Welcome to Remote Work Management", bodyMessage);
                mail.IsBodyHtml = true;
                smtp.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks the email template.
        /// </summary>
        /// <param name="emailType">Type of the email.</param>
        /// <param name="fieldValue">The fieldValue.</param>
        private static string CheckEmailTemplate(EmailType emailType, string fieldValue)
        {
            string body = null, bodyaux = null;
            switch (emailType)
            {
                case EmailType.Welcome:
                    body = NewUserTemplate.Replace("@fieldValue", fieldValue);
                    break;
                case EmailType.ForgotPassword:
                    body = RecoverPasswordTemplate.Replace("@fieldValue", fieldValue);
                    break;
                case EmailType.ChangeRemoteDay:
                    body = ChangeRemoteDayTemplate.Replace("@username", fieldValue);
                    break;
                case EmailType.RequestException:
                    body = RequestExceptionTemplate.Replace("@username", fieldValue);
                    break;
                case EmailType.CheckIn:
                    body = CheckInTemplate.Replace("@fullname", fieldValue);
                    break;
                case EmailType.CheckOut:
                    body = CheckOutTemplate.Replace("@fullname", fieldValue);
                    break;
                case EmailType.CheckInUD:
                    string[] msg = fieldValue.Split('|');
                    bodyaux = CheckInTemplateUD.Replace("@fullname", msg[0]);
                    body = bodyaux.Replace("@comment", msg[1]);
                    break;
            }
            return body;
        }
    }
}