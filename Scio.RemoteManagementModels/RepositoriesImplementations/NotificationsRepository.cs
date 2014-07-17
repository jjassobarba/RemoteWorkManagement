using System;
using System.Collections.Generic;
using Scio.RemoteManagementModels.Entities;
using Scio.RemoteManagementModels.RepositoriesContracts;

namespace Scio.RemoteManagementModels.RepositoriesImplementations
{
    public class NotificationsRepository : INotificationsRepository
    {
        public IEnumerable<Notifications> GetNotificationsForUser(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Guid InsertNotification(Notifications notification)
        {
            throw new NotImplementedException();
        }

        public bool UpdateNotification(Notifications notification)
        {
            throw new NotImplementedException();
        }
    }
}
