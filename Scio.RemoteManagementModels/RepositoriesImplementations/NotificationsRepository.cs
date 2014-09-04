using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using Scio.RemoteManagementModels.Entities;
using Scio.RemoteManagementModels.RepositoriesContracts;

namespace Scio.RemoteManagementModels.RepositoriesImplementations
{
    public class NotificationsRepository : INotificationsRepository
    {
        /// <summary>
        /// The _session
        /// </summary>
        private ISession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsRepository"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public NotificationsRepository(ISession session)
        {
            _session = session;
        }

        /// <summary>
        /// Gets the notifications for user.
        /// </summary>
        /// <param name="userInfoId"></param>
        /// <returns></returns>
        public IEnumerable<Notifications> GetNotificationsForUser(Guid userInfoId)
        {
            var notificationsQuery = new List<Notifications>();
            if (userInfoId != Guid.Empty)
            {
                notificationsQuery = (_session.Query<Notifications>().Where(notification => notification.IdUserInfo.IdUserInfo.Equals(userInfoId))).ToList();
            }
            return notificationsQuery;
        }

        public Notifications GetNotificationById(Guid? notificationId)
        {
            var query = _session.QueryOver<Notifications>().Where(p => p.IdNotification == notificationId);
            var notification = query.List<Notifications>().FirstOrDefault();
            return notification;
        }

        /// <summary>
        /// Inserts the notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        public Guid InsertNotification(Notifications notification)
        {
            Guid id;
            using (var transaction = _session.BeginTransaction())
            {
                id = (Guid)_session.Save(notification);
                transaction.Commit();
            }
            return id;
        }

        /// <summary>
        /// Updates the notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        public bool UpdateNotification(Notifications notification)
        {
            try
            {
                using (var transaction = _session.BeginTransaction())
                {
                    _session.Update(notification);
                    transaction.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
