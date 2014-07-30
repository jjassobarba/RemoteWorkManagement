using System;
using System.Collections.Generic;
using Scio.RemoteManagementModels.Entities;


namespace Scio.RemoteManagementModels.RepositoriesContracts
{
    public interface ICheckInOutRepository
    {
        /// <summary>
        /// Gets the ChekInOut for user.
        /// </summary>
        /// <param name="userInfoId">The user information identifier.</param>
        /// <returns></returns>
        IEnumerable<CheckInOut> GetForChekInOutUser(Guid userInfoId);

        /// <summary>
        /// Inserts the notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <returns></returns>
        Guid InsertChekInOut(CheckInOut checkInOut);
    }
}
