using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using Scio.RemoteManagementModels.Entities;
using Scio.RemoteManagementModels.RepositoriesContracts;

namespace Scio.RemoteManagementModels.RepositoriesImplementations
{
    class CheckInOutRepository
    {
        /// <summary>
        /// The _session
        /// </summary>
        private ISession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckInOutRepository"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public CheckInOutRepository(ISession session)
        {
            _session = session;
        }

        /// <summary>
        /// Gets the checkInOut for user.
        /// </summary>
        /// <param name="userInfoId"></param>
        /// <returns></returns>
        public IEnumerable<Notifications> GetForChekInOutUser(Guid userInfoId)
        {
            var notificationsQuery = (_session.Query<Notifications>().Where(x => x.IdUserInfo.IdUserInfo.Equals(userInfoId))).ToList();
            return notificationsQuery;
        }

        /// <summary>
        /// Inserts the checkInOut.
        /// </summary>
        /// <param name="checkInOut">The checkInOut.</param>
        /// <returns></returns>
        public Guid InsertChekInOut(CheckInOut checkInOut)
        {
            Guid id;
            using (var transaction = _session.BeginTransaction())
            {
                id = (Guid)_session.Save(checkInOut);
                transaction.Commit();
            }
            return id;
        }
    }
}
