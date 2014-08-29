using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using RemoteWorkManagement.Models;
using System.Net.Mail;
using System.Net;
using System;

namespace RemoteWorkManagement.Controllers
{
    public class AccountController : Controller
    {

        private readonly MembershipProvider _membershipProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="membershipProvider">The membership provider.</param>
        public AccountController(MembershipProvider membershipProvider)
        {
            _membershipProvider = membershipProvider;
        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Logs the off.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Logins the specified username.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (_membershipProvider.ValidateUser(model.Email, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.Email, true);
                    return RedirectToAction("Index", "Admin");
                }
                ModelState.AddModelError("", "Invalid usernarme or password");
            }
            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RecoverPassword(string mail)
        {
            string newPassword = _membershipProvider.ResetPassword(mail,string.Empty);
            bool result=MailSender(mail, newPassword);            
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }

        /// Changes the password.
        /// </summary>
        /// <param name="newPassword">The new password.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ChangePassword(string newPassword, string oldPassword)
        {
            var user = User.Identity.Name;
            var success = _membershipProvider.ChangePassword(user, oldPassword, newPassword);
            return Json(new { success = success });
        }

        /// <summary>
        /// Mails the sender.
        /// </summary>
        /// <param name="mailto">The mailto.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public bool MailSender(string mailto, string password)
        {
            try
            {
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("sciorewoma@gmail.com", "*@dm1n2o14*");
                MailMessage mail = new MailMessage("sciorewoma@gmail.com", mailto, "Please do not reply to this message", "Your temporary password is:: " + password);
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