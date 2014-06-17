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
        public ActionResult Authorize(string username, string password)
        {
            var sucess =_membershipProvider.ValidateUser(username, password);
            return null;
        }
    }
}