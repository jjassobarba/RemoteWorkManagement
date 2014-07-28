using System;
using System.Collections.Generic;
using Scio.RemoteManagementModels.Entities;
using Scio.RemoteManagementModels.RepositoriesContracts;

namespace Scio.RemoteManagementModels.RepositoriesImplementations
{
    public class MessagesRepository : IMessagesRepository
    {
        public IEnumerable<Messages> GetMessagesForUser(Guid userInfoId)
        {
            throw new NotImplementedException();
        }

        public Messages GetMessage(Guid idMessage)
        {
            throw new NotImplementedException();
        }

        public Guid InsertMessage(Messages message)
        {
            throw new NotImplementedException();
        }
    }
}
