using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using Scio.RemoteManagementModels.Entities;
using Scio.RemoteManagementModels.RepositoriesContracts;

namespace Scio.RemoteManagementModels.RepositoriesImplementations
{
    public class InboxRepository : IInboxRepository
    {
         private ISession _session;

        /// <summary>
         /// Initializes a new instance of the <see cref="InboxRepository"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
         public InboxRepository(ISession session)
        {
            _session = session;
        }

        public IEnumerable<Inbox> GetInboxForUser(Guid idUserInfo)
        {
            var inboxQuery = (_session.Query<Inbox>().Where(x => x.IdUserInfo.IdUserInfo.Equals(idUserInfo))).ToList();
            return inboxQuery;
        }

        public bool DeleteInbox(Guid idInbox)
        {
            throw new NotImplementedException();
        }
    }
}
