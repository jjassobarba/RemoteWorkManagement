using System;
using System.Collections.Generic;
using Scio.RemoteManagementModels.Entities;

namespace Scio.RemoteManagementModels.RepositoriesContracts
{
    public interface IInboxRepository
    {
        /// <summary>
        /// Gets the inbox for user.
        /// </summary>
        /// <param name="idUserInfo">The identifier user information.</param>
        /// <returns></returns>
        IEnumerable<Inbox> GetInboxForUser(Guid idUserInfo);

        /// <summary>
        /// Deletes the inbox.
        /// </summary>
        /// <param name="idInbox">The identifier inbox.</param>
        /// <returns></returns>
        bool DeleteInbox(Guid idInbox);
    }
}
