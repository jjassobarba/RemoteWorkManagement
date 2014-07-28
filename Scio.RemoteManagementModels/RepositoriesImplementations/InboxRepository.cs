using System;
using System.Collections.Generic;
using Scio.RemoteManagementModels.Entities;
using Scio.RemoteManagementModels.RepositoriesContracts;

namespace Scio.RemoteManagementModels.RepositoriesImplementations
{
    public class InboxRepository : IInboxRepository
    {
        public IEnumerable<Inbox> GetInboxForUser(Guid idUserInfo)
        {
            throw new NotImplementedException();
        }

        public bool DeleteInbox(Guid idInbox)
        {
            throw new NotImplementedException();
        }
    }
}
