using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using Scio.RemoteManagementModels.Entities;
using Scio.RemoteManagementModels.RepositoriesContracts;

namespace Scio.RemoteManagementModels.RepositoriesImplementations
{
    public class UserInfoRepository : IUserInfoRepository
    {
        private ISession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoRepository"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public UserInfoRepository(ISession session)
        {
            _session = session;
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserInfo> GetUsers()
        {
            var query = _session.CreateQuery("from UserInfo users");
            var users = query.List<UserInfo>();
            return users;
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="idUser">The identifier user.</param>
        /// <returns></returns>
        public UserInfo GetUser(Guid idUser)
        {
            var user = _session.Get<UserInfo>(idUser);
            return user;
        }

        /// <summary>
        /// Gets the user by membership identifier.
        /// </summary>
        /// <param name="membershipId">The membership identifier.</param>
        /// <returns></returns>
        public UserInfo GetUserByMembershipId(int membershipId)
        {
            var query = _session.QueryOver<UserInfo>().Where(p => p.IdMembership.Id == membershipId);
            var user = query.List<UserInfo>().FirstOrDefault();
            return user;
        }

       /// <summary>
        /// Inserts the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public Guid InsertUser(UserInfo user)
        {
            var id = Guid.Empty;
            using (var transaction = _session.BeginTransaction())
            {
                id = (Guid) _session.Save(user);
                transaction.Commit();
            }
            return id;
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public bool UpdateUser(UserInfo user)
        {
            try
            {
                using (var transaction = _session.BeginTransaction())
                {
                    _session.Update(user);
                    transaction.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="idUser">The identifier user.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool DeleteUser(Guid idUser)
        {
            throw new NotImplementedException();
        }
    }
}
