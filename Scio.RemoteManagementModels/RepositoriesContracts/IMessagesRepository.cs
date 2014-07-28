using System;
using System.Collections.Generic;
using Scio.RemoteManagementModels.Entities;

namespace Scio.RemoteManagementModels.RepositoriesContracts
{
    public interface IMessagesRepository
    {
        /// <summary>
        /// Gets the messages for user.
        /// </summary>
        /// <param name="userInfoId">The user information identifier.</param>
        /// <returns></returns>
        IEnumerable<Messages> GetMessagesForUser(Guid userInfoId);

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <param name="idMessage">The identifier message.</param>
        /// <returns></returns>
        Messages GetMessage(Guid idMessage);

        /// <summary>
        /// Inserts the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        Guid InsertMessage(Messages message);
    }
}
