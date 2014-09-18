using System;
using System.Collections.Generic;
using Scio.RemoteManagementModels.Entities;

namespace Scio.RemoteManagementModels.RepositoriesContracts
{
    public interface IUserInfoRepository
    {
        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <returns></returns>
        IEnumerable<UserInfo> GetUsers();

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="idUser">The identifier user.</param>
        /// <returns></returns>
        UserInfo GetUser(Guid idUser);

        /// <summary>
        /// Gets the user by membership identifier.
        /// </summary>
        /// <param name="membershipId">The membership identifier.</param>
        /// <returns></returns>
        UserInfo GetUserByMembershipId(int membershipId);

        /// <summary>
        /// Gets the remaining users - for CheckIn.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        IEnumerable<UserInfo> GetRemainingUsers(string userName);

        /// <summary>
        /// Gets ready users - for CheckIn.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        IEnumerable<UserInfo> GetReadyUsers(string userName);

        /// <summary>
        /// Inserts the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        Guid InsertUser(UserInfo user);

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        bool UpdateUser(UserInfo user);

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="idUser">The identifier user.</param>
        /// <returns></returns>
        bool DeleteUser(Guid idUser);
    }
}
