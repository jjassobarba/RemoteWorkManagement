using System;
using System.Web.Mvc;
using System.Web.Security;
using Scio.RemoteManagementModels.Entities;
using Scio.RemoteManagementModels.RepositoriesContracts;

namespace RemoteWorkManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly MembershipProvider _membershipProvider;
        private readonly IUserInfoRepository _userInfoRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController" /> class.
        /// </summary>
        /// <param name="membershipProvider">The membership provider.</param>
        /// <param name="userInfoRepository">The user information repository.</param>
        public HomeController(MembershipProvider membershipProvider, IUserInfoRepository userInfoRepository)
        {
            _membershipProvider = membershipProvider;
            _userInfoRepository = userInfoRepository;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
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
            if(status == MembershipCreateStatus.Success)
            {
                var userInfoObject = new UserInfo()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Position = position,
                    ProjectLeader = projectLeader
                };
                _userInfoRepository.InsertUser(userInfoObject);
            }
            return Json(new { data = status.ToString() });
        }
    }
}