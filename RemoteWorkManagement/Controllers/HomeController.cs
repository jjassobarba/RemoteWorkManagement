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

        public HomeController()
        {
            
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /// <summary>
        /// Authorizes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Authorize(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }
            MembershipUser membershipUser = _membershipProvider.GetUser(model.Email, false);
            if (membershipUser == null)
            {
                ModelState.AddModelError("Email", "Email con contraseña no validos");
                return View("Index", model);
            }
            if (membershipUser.IsLockedOut)
            {
                ModelState.AddModelError("Email", "El usuario esta bloqueado. Contacte a su administrador");
                return View("Index", model);
            }
            if (!_membershipProvider.ValidateUser(model.Email, model.Password))
            {
                ModelState.AddModelError("Email", "Email o contraseña invalidos");
                return View("Index", model);
            }
            FormsAuthentication.SetAuthCookie(model.Email, true);
            return RedirectToAction("Index", "Dashboard");
        }
    }
}