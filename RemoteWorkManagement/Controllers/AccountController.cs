using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RemoteWorkManagement.Models;

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

        [HttpPost]
        public JsonResult RecoverPassword(string mail)
        {
            return Json(new { mail = mail }, JsonRequestBehavior.AllowGet);
        }

    }
}