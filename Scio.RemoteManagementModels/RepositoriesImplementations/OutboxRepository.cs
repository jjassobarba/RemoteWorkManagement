using System;
using System.Collections.Generic;
using Scio.RemoteManagementModels.Entities;
using Scio.RemoteManagementModels.RepositoriesContracts;

namespace Scio.RemoteManagementModels.RepositoriesImplementations
{
    public class OutboxRepository: IOutboxRepository
    {
        public IEnumerable<Outbox> GetOutBoxForUser(Guid idUserInfo)
        {
            throw new NotImplementedException();
        }

        public bool DeleteOutbox(Guid idOutbox)
        {
            throw new NotImplementedException();
        }
    }
}
