using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using Scio.RemoteManagementModels.Entities;
using Scio.RemoteManagementModels.RepositoriesContracts;

namespace Scio.RemoteManagementModels.RepositoriesImplementations
{
    public class OutboxRepository: IOutboxRepository
    {
        /// <summary>
        /// The _session
        /// </summary>
        private ISession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutboxRepository"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public OutboxRepository(ISession session)
        {
            _session = session;
        }

        public IEnumerable<Outbox> GetOutBoxForUser(Guid idUserInfo)
        {
            var outBoxQuery = (_session.Query<Outbox>().Where(x => x.IdUserInfo.IdUserInfo.Equals(idUserInfo))).ToList();
            return outBoxQuery;
        }

        public bool DeleteOutbox(Guid idOutbox)
        {
            throw new NotImplementedException();
        }
    }
}
