using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using Scio.RemoteManagementModels.Entities;
using Scio.RemoteManagementModels.RepositoriesContracts;

namespace Scio.RemoteManagementModels.RepositoriesImplementations
{
    public class MessagesRepository : IMessagesRepository
    {
        /// <summary>
        /// The _session
        /// </summary>
        private ISession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagesRepository"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public MessagesRepository(ISession session)
        {
            _session = session;
        }

        public IEnumerable<Messages> GetMessagesForUser(Guid userInfoId)
        {
            var messageQuery = (_session.Query<Messages>().Where(x => x.IdUserInfo.IdUserInfo.Equals(userInfoId))).ToList();
            return messageQuery;
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
