using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using RemoteWorkManagement.Models;
using Scio.RemoteManagementModels.RepositoriesContracts;
using Scio.RemoteManagementModels.RepositoriesImplementations;

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
        /// Determines whether [is new pass].
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult IsNewPass()
        {
            var user = User.Identity.Name;
            var userMembership = _membershipProvider.GetUser(user, false);
            var userInfo = _userInfoRepository.GetUserByMembershipId(Convert.ToInt32(userMembership.ProviderUserKey.ToString()));
            return Json(new { isTemporal = userInfo.IsTemporalPassword });
        }

    }
}