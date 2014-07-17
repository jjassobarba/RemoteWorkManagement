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
        /// <param name="user">The user.</param>
        /// <returns></returns>
        IEnumerable<Notifications> GetNotificationsForUser(UserInfo user);

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
