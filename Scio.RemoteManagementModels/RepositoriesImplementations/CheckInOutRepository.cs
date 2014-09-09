using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using Scio.RemoteManagementModels.Utils;
using Scio.RemoteManagementModels.Entities;
using Scio.RemoteManagementModels.RepositoriesContracts;

namespace Scio.RemoteManagementModels.RepositoriesImplementations
{
    public class CheckInOutRepository : ICheckInOutRepository
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
        public IEnumerable<CheckInOut> GetForChekInOutUser(Guid userInfoId)
        {
            var chekInOutQuery = (_session.Query<CheckInOut>().Where(x => x.IdUserInfo.IdUserInfo.Equals(userInfoId))).ToList();
            return chekInOutQuery;
        }

        /// <summary>
        /// Inserts checkIn.
        /// </summary>
        /// <param name="checkIn">The checkInOut.</param>
        /// <returns></returns>
        public Guid InsertCheckIn(CheckInOut checkIn)
        {
            checkIn.CheckOutDate = Utils.Utils.MinDate();
            Guid id;
            using (var transaction = _session.BeginTransaction())
            {
                id = (Guid)_session.Save(checkIn);
                transaction.Commit();
            }
            return id;
        }

        /// <summary>
        /// Inserts CheckOut
        /// </summary>
        /// <param name="checkOut"></param>
        /// <returns></returns>
        public bool InserCheckOut(CheckInOut checkOut)
        {
            bool success = false;
            using (var transaction = _session.BeginTransaction())
            {
                _session.Update(checkOut);
                transaction.Commit();
                success = true;
            }
            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public CheckInOut GetLastChekInOutByUser(UserInfo userInfo)
        { 
            Guid? midUserInfo = userInfo.IdUserInfo;
            Guid idUserInfo = midUserInfo ?? Guid.Empty;
            var query = _session.QueryOver<CheckInOut>().Where(x => x.IdUserInfo.IdUserInfo == idUserInfo);
            var checkInOut = query.List<CheckInOut>().LastOrDefault();
            return checkInOut;
        }
    }
}
