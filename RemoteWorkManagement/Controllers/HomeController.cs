using System;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using RemoteWorkManagement.Models;

namespace RemoteWorkManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly MembershipProvider _membershipProvider;

        public HomeController(MembershipProvider membershipProvider)
        {
            _membershipProvider = membershipProvider;
        }

        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Authorizes the specified username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Authorize(string username, string password)
        {
            var sucess = _membershipProvider.ValidateUser(username, password);
            return Json(new { data = sucess });
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="position">The position.</param>
        /// <param name="projectLeader">The project leader.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CreateUser(string username, string firstName, string lastName, string position, string projectLeader)
        {
            MembershipCreateStatus status;
            var password = Membership.GeneratePassword(8, 3);
            _membershipProvider.CreateUser(username, password, username, string.Empty, string.Empty, true, new Guid(), out status);
            return Json(new { data = status.ToString() });
        }
    }
}