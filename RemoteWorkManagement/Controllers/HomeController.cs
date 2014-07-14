﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using RemoteWorkManagement.DTO;
using Scio.RemoteManagementModels.Entities;
using Scio.RemoteManagementModels.RepositoriesContracts;

namespace RemoteWorkManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly MembershipProvider _membershipProvider;
        private readonly RoleProvider _roleProvider;
        private readonly IUserInfoRepository _userInfoRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController" /> class.
        /// </summary>
        /// <param name="membershipProvider">The membership provider.</param>
        /// <param name="userInfoRepository">The user information repository.</param>
        /// <param name="roleProvider">The role provider.</param>
        public HomeController(MembershipProvider membershipProvider, IUserInfoRepository userInfoRepository, RoleProvider roleProvider)
        {
            _membershipProvider = membershipProvider;
            _userInfoRepository = userInfoRepository;
            _roleProvider = roleProvider;
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
        /// <param name="remoteDays">The remote days.</param>
        /// <param name="flexTime">The flex time.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CreateUser(string username, string firstName, string lastName, string position, string projectLeader, string[] remoteDays, string flexTime)
        {
            MembershipCreateStatus status;
            var password = Membership.GeneratePassword(8, 3);
            var remoteDaysString = remoteDays.Aggregate("", (current, remoteDay) => current + (remoteDay + ","));
            _membershipProvider.CreateUser(username, password, username, string.Empty, string.Empty, true, new Guid(), out status);
            if (status == MembershipCreateStatus.Success)
            {
                var userId = _membershipProvider.GetUser(username, false);
                var user = new Users();
                if (userId != null)
                {
                    user.Id = Convert.ToInt32(userId.ProviderUserKey.ToString());
                }
                var userInfoObject = new UserInfo()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Position = position,
                    ProjectLeader = projectLeader,
                    IdMembership = user,
                    RemoteDays = remoteDaysString,
                    FlexTime = flexTime
                };
                _userInfoRepository.InsertUser(userInfoObject);
            }
            return Json(new { data = status.ToString() });
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
            return Json(new { userInfo = user });
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
    }
}