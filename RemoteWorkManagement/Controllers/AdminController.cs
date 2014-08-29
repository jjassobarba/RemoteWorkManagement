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

        //Get Notifications
        public ActionResult Notifications()
        {
            return View();
        }

        //Get Reports
        public ActionResult Reports()
        {
            ViewBag.Title = "Reports";
            return View();
        }
    }
}