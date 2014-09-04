using System;
using System.Linq;
using System.Web.Mvc;
using Scio.RemoteManagementModels.Entities;
using Scio.RemoteManagementModels.RepositoriesContracts;

namespace RemoteWorkManagement.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IUserInfoRepository _userInfoRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsController" /> class.
        /// </summary>
        /// <param name="notificationsRepository">The notifications repository.</param>
        /// <param name="userInfoRepository">The user information repository.</param>
        public NotificationsController(INotificationsRepository notificationsRepository, IUserInfoRepository userInfoRepository)
        {
            _notificationsRepository = notificationsRepository;
            _userInfoRepository = userInfoRepository;
        }

        /// <summary>
        /// Gets the notification for user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetNotificationForUser(Guid userId)
        {
            var notifications = _notificationsRepository.GetNotificationsForUser(userId).ToList();
            var notifList = notifications.Select(notification => new
            {
                IdNotification = notification.IdNotification,
                ProjectLeader = notification.ProjectLeaderMail,
                Sensei = notification.SenseiMail,
                Others = notification.OtherMails
            }).Cast<object>().ToList();
            return Json(new { notifications = notifList });
        }

        /// <summary>
        /// Inserts the notification.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="projectLeaderMail">The project leader mail.</param>
        /// <param name="senseiMail">The sensei mail.</param>
        /// <param name="otherEmails">The other emails.</param>
        /// <param name="notificationId">The notification identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InsertNotification(Guid userId, string projectLeaderMail, string senseiMail, string otherEmails, Guid? notificationId)
        {
            var success = false;
            var user = _userInfoRepository.GetUser(userId);
            if (notificationId != null && notificationId != Guid.Empty)
            {
                var notificationObj = _notificationsRepository.GetNotificationById(notificationId);
                notificationObj.ProjectLeaderMail = projectLeaderMail;
                notificationObj.SenseiMail = senseiMail;
                notificationObj.OtherMails = otherEmails;
                success = _notificationsRepository.UpdateNotification(notificationObj);
            }
            else
            {
                var notification = new Notifications()
                {
                    IdUserInfo = user,
                    OtherMails = otherEmails,
                    ProjectLeaderMail = projectLeaderMail,
                    SenseiMail = senseiMail
                };
                var idNotification = _notificationsRepository.InsertNotification(notification);
                if (idNotification != Guid.Empty)
                    success = true;
            }
            return Json(new { success = success });
        }

    }
}