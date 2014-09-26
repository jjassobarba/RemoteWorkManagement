using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.WebPages;
using RemoteWorkManagement.Models;
using RemoteWorkManagement.Helpers;
using Scio.RemoteManagementModels.RepositoriesContracts;

namespace RemoteWorkManagement.Controllers
{
    public class AccountController : Controller
    {

        private readonly MembershipProvider _membershipProvider;
        private readonly IUserInfoRepository _userInfoRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController" /> class.
        /// </summary>
        /// <param name="membershipProvider">The membership provider.</param>
        /// <param name="userInfoRepository">The user information repository.</param>
        public AccountController(MembershipProvider membershipProvider, IUserInfoRepository userInfoRepository)
        {
            _membershipProvider = membershipProvider;
            _userInfoRepository = userInfoRepository;
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
            return RedirectToAction("Login", "Account");
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
                var user = _membershipProvider.GetUser(model.Email, false);
                if (user != null && user.IsApproved)
                {
                    ModelState.AddModelError("", "Invalid usernarme or password");
                }
                else
                {
                    ModelState.AddModelError("", "Your account is not active please contact the administrator");
                }
            }
            return View(model);
        }

        /// <summary>
        /// Logins the specified return URL.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RecoverPassword(string mail)
        {
            
            string newPassword = _membershipProvider.ResetPassword(mail, string.Empty);
            var usr = _membershipProvider.GetUser(mail, false);
            var userId = Convert.ToInt32(usr.ProviderUserKey);
            var usrInfo = _userInfoRepository.GetUserByMembershipId(userId);
            usrInfo.IsTemporalPassword = true;
            bool result = false;
            if (_userInfoRepository.UpdateUser(usrInfo))
            {
                result = Utilities.MailSender(mail, newPassword, Utilities.EmailType.ForgotPassword);
            }
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Verifies if the user exists
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ValidateUser(string mail)
        {
            string userName = _membershipProvider.GetUserNameByEmail(mail);

            if (!userName.IsEmpty())
                return Json(new { result = "True" }, JsonRequestBehavior.AllowGet);
            return Json(new { result = "False" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
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
            if (success)
            {
                var usr = _membershipProvider.GetUser(user, false);
                var userId = Convert.ToInt32(usr.ProviderUserKey);
                var usrInfo = _userInfoRepository.GetUserByMembershipId(userId);
                usrInfo.IsTemporalPassword = false;
                _userInfoRepository.UpdateUser(usrInfo);
            }
            return Json(new { success = success });
        }

        /// <summary>
        /// Determines whether [is new pass].
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult IsNewPass()
        {
            var success = false;
            var user = User.Identity.Name;
            if (user != null && !string.IsNullOrWhiteSpace(user))
            {
                var userMembership = _membershipProvider.GetUser(user, false);
                var userInfo = _userInfoRepository.GetUserByMembershipId(Convert.ToInt32(userMembership.ProviderUserKey.ToString()));
                success = userInfo.IsTemporalPassword;
            }
            return Json(new { isTemporal = success });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ValidateOldPassword(string password)
        {
            string userName = User.Identity.Name;
            bool isValid = _membershipProvider.ValidateUser(userName, password);
            return Json(new { result = isValid }, JsonRequestBehavior.AllowGet);
        }

    }
}