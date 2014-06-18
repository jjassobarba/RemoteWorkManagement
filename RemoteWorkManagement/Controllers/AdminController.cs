using System.Web.Mvc;

namespace RemoteWorkManagement.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        // GET: Users
        public ActionResult Users()
        {
            return View();
        }
    }
}