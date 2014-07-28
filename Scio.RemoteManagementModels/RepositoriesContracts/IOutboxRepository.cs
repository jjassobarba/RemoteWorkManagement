using System;
using System.Collections.Generic;
using Scio.RemoteManagementModels.Entities;

namespace Scio.RemoteManagementModels.RepositoriesContracts
{
    public interface IOutboxRepository
    {
        /// <summary>
        /// Gets the out box for user.
        /// </summary>
        /// <param name="idUserInfo">The identifier user information.</param>
        /// <returns></returns>
        IEnumerable<Outbox> GetOutBoxForUser(Guid idUserInfo);

        /// <summary>
        /// Deletes the outbox.
        /// </summary>
        /// <param name="idOutbox">The identifier outbox.</param>
        /// <returns></returns>
        bool DeleteOutbox(Guid idOutbox);
    }
}
