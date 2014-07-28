using System;
using System.Collections.Generic;
using Scio.RemoteManagementModels.Entities;

namespace Scio.RemoteManagementModels.RepositoriesContracts
{
    public interface INotificationsRepository
    {
        /// <summary>
        /// Gets the notifications for user.
        /// </summary>
        /// <param name="userInfoId">The user information identifier.</param>
        /// <returns></returns>
        IEnumerable<Notifications> GetNotificationsForUser(Guid userInfoId);

        /// <summary>
        /// Inserts the notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        Guid InsertNotification(Notifications notification);

        /// <summary>
        /// Updates the notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        bool UpdateNotification(Notifications notification);
    }
}
