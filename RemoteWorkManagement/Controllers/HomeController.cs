using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Castle.Core.Internal;
using Newtonsoft.Json;
using RemoteWorkManagement.DTO;
using RemoteWorkManagement.Helpers;
using Scio.RemoteManagementModels.Entities;
using Scio.RemoteManagementModels.RepositoriesContracts;

namespace RemoteWorkManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly MembershipProvider _membershipProvider;
        private readonly RoleProvider _roleProvider;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IInboxRepository _inboxRepository;
        private readonly IOutboxRepository _outboxRepository;
        private readonly ICheckInOutRepository _checkInOutRepository;
        private const string DefaultPicture = "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";


        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController" /> class.
        /// </summary>
        /// <param name="membershipProvider">The membership provider.</param>
        /// <param name="userInfoRepository">The user information repository.</param>
        /// <param name="roleProvider">The role provider.</param>
        /// <param name="notificationsRepository"></param>
        /// <param name="inboxRepository"></param>
        /// <param name="outboxRepository"></param>
        /// <param name="checkInOutRepository"></param>
        public HomeController(MembershipProvider membershipProvider, IUserInfoRepository userInfoRepository, RoleProvider roleProvider,
            INotificationsRepository notificationsRepository, IInboxRepository inboxRepository, IOutboxRepository outboxRepository,
            ICheckInOutRepository checkInOutRepository)
        {
            _membershipProvider = membershipProvider;
            _userInfoRepository = userInfoRepository;
            _roleProvider = roleProvider;
            _notificationsRepository = notificationsRepository;
            _inboxRepository = inboxRepository;
            _outboxRepository = outboxRepository;
            _checkInOutRepository = checkInOutRepository;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
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
        /// <param name="rol">The rol.</param>
        /// <param name="projectLeader">The project leader.</param>
        /// <param name="sensei">The sensei.</param>
        /// <param name="remoteDays">The remote days.</param>
        /// <param name="flexTime">The flex time.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CreateUser(string username, string firstName, string lastName, string position, string rol, Guid? projectLeader, Guid? sensei, string remoteDays, string flexTime)
        {
            byte[] byteFile = null;
            var roleList = JsonConvert.DeserializeObject<string[]>(rol);
            for (var x = 1; x < Request.Files.Count + 1; x++)
            {
                var file = Request.Files[x - 1];
                if (file != null && file.ContentLength != 0)
                {
                    int contentLength = file.ContentLength;
                    byteFile = new byte[contentLength];
                    file.InputStream.Read(byteFile, 0, contentLength);
                }
            }
            MembershipCreateStatus status;
            var password = Membership.GeneratePassword(8, 3);

            var remoteList = JsonConvert.DeserializeObject<string[]>(remoteDays);
            var remoteDaysString = remoteList.Aggregate("", (current, remoteDay) => current + (remoteDay + ","));
            _membershipProvider.CreateUser(username, password, username, string.Empty, string.Empty, true, new Guid(), out status);
            if (status == MembershipCreateStatus.Success)
            {
                var userId = _membershipProvider.GetUser(username, false);
                var user = new Users();
                if (userId != null)
                {
                    user.Id = Convert.ToInt32(userId.ProviderUserKey.ToString());
                }
                _roleProvider.AddUsersToRoles(new[] { username }, roleList);
                var userInfoObject = new UserInfo()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Position = position,
                    IdProjectLeader = projectLeader,
                    IdSensei = sensei,
                    IdMembership = user,
                    RemoteDays = remoteDaysString,
                    FlexTime = flexTime,
                    Picture = byteFile
                };
                _userInfoRepository.InsertUser(userInfoObject);
            }
            bool rpt = Utilities.MailSender(username, password);

            return Json(new { data = status.ToString() });
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="idUserInfo">The Id of UserInfo Table</param>
        /// <param name="username">The username.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="position">The position (Team member, leader, etc...)</param>
        /// <param name="rol">The rol.</param>
        /// <param name="projectLeader">The project leader.</param>
        /// <param name="sensei">The sensei.</param>
        /// <param name="remoteDays">The remote days.</param>
        /// <param name="flexTime">The flex time.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateUser(string idUserInfo, string username, string firstName, string lastName, string position, string rol,
            Guid? projectLeader, Guid? sensei, string[] remoteDays, string flexTime)
        {
            var remoteDaysString = remoteDays.Aggregate("", (current, remoteDay) => current + (remoteDay + ","));
            var userId = _membershipProvider.GetUser(username, false);
            var user = new Users();
            if (userId != null)
            {
                user.Id = Convert.ToInt32(userId.ProviderUserKey.ToString());
            }
            var gIdUserInfo = new Guid(idUserInfo);
            var userInfoOldObject = _userInfoRepository.GetUser(gIdUserInfo);
            userInfoOldObject.FirstName = firstName;
            userInfoOldObject.LastName = lastName;
            userInfoOldObject.Position = position;
            userInfoOldObject.IdProjectLeader = projectLeader;
            userInfoOldObject.IdSensei = sensei;
            userInfoOldObject.RemoteDays = remoteDaysString;
            userInfoOldObject.FlexTime = flexTime;
            var status = _userInfoRepository.UpdateUser(userInfoOldObject);
            var roleList = JsonConvert.DeserializeObject<string[]>(rol);
            var userRole = _roleProvider.GetRolesForUser(username);
            if (userRole != null && userRole.Length > 0)
            {
                _roleProvider.RemoveUsersFromRoles(new[] { username }, userRole);
                _roleProvider.AddUsersToRoles(new[] { username }, roleList);
            }
            return Json(new { data = status });
        }

        /// <summary>
        /// Gets all roles.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAllRoles()
        {
            var roles = _roleProvider.GetAllRoles();
            return Json(new { roles = roles }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAllUsers()
        {
            var users = _userInfoRepository.GetUsers();
            var userInfoList = users.Select(userInfo => new UserInfoDTO()
            {
                Id = userInfo.IdUserInfo,
                Name = userInfo.FirstName + " " + userInfo.LastName
            }).ToList();
            return Json(new { users = userInfoList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetUser(Guid userId)
        {
            var user = _userInfoRepository.GetUser(userId);
            var userMapped = new
            {
                IdUserInfo = user.IdUserInfo,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FlexTime = user.FlexTime,
                OtherFlexTime = user.OtherFlexTime,
                Picture = user.Picture.IsNullOrEmpty() ? DefaultPicture : Convert.ToBase64String(user.Picture),
                Position = user.Position,
                ReceiveNotifications = user.ReceiveNotifications,
                RemoteDays = user.RemoteDays,
                IdSensei = user.IdSensei,
                IdProjectLeader = user.IdProjectLeader,
                IdMembership = new
                {
                    IdMembership = user.IdMembership.Id,
                    Email = user.IdMembership.Username
                },
                Rol = new
                {
                    RolName = user.IdMembership.Roles.Select(p => p.RoleName).ToList()
                }
            };
            return Json(new { userInfo = userMapped });
        }

        /// <summary>
        /// Gets all users information.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAllUsersInfo()
        {
            var users = _userInfoRepository.GetUsers();
            var usersInfoList = users.Select(user => new
            {
                IdUserInfo = user.IdUserInfo,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FlexTime = user.FlexTime,
                OtherFlexTime = user.OtherFlexTime,
                Picture = user.Picture.IsNullOrEmpty() ? DefaultPicture : Convert.ToBase64String(user.Picture),
                Position = user.Position,
                //ProjectLeader = user.ProjectLeader,
                ReceiveNotifications = user.ReceiveNotifications,
                RemoteDays = user.RemoteDays,
                IdMembership = new
                {
                    IdMembership = user.IdMembership.Id,
                    Email = user.IdMembership.Username
                },
                Rol = new
                {
                    RolName = user.IdMembership.Roles.Select(p => p.RoleName).FirstOrDefault()
                }
            }).Cast<object>().ToList();

            return Json(new { usersInfo = usersInfoList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadFile()
        {
            for (var x = 1; x < Request.Files.Count + 1; x++)
            {
                var file = Request.Files[x - 1];

                if (file != null && file.ContentLength != 0)
                {

                }
            }
            return Json(new { success = true });
        }

        /// <summary>
        /// Gets the senseis.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetSenseis()
        {
            var senseis = _roleProvider.GetUsersInRole("Sensei");
            var senseisList = (from sensei in senseis
                               select _membershipProvider.GetUser(sensei, false)
                                   into user
                                   select _userInfoRepository.GetUserByMembershipId(Convert.ToInt32(user.ProviderUserKey))
                                       into userInfo
                                       select new
                                       {
                                           id = userInfo.IdUserInfo,
                                           name = userInfo.FirstName + " " + userInfo.LastName
                                       }).Cast<object>().ToList();
            return Json(new { senseis = senseisList });
        }

        /// <summary>
        /// Gets the project leaders.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetProjectLeaders()
        {
            var teamLeaders = _roleProvider.GetUsersInRole("TeamLeader");
            var teamList = (from teamLeader in teamLeaders
                            select _membershipProvider.GetUser(teamLeader, false)
                                into user
                                select _userInfoRepository.GetUserByMembershipId(Convert.ToInt32(user.ProviderUserKey))
                                    into userInfo
                                    select new
                                    {
                                        id = userInfo.IdUserInfo,
                                        name = userInfo.FirstName + " " + userInfo.LastName
                                    }).Cast<object>().ToList();
            return Json(new { teamLeaders = teamList });
        }
    }
}