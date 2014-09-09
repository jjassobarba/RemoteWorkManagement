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
        /// <param name="checkInOut">The notification.</param>
        /// <returns></returns>
        Guid InsertCheckIn(CheckInOut checkInOut);

        /// <summary>
        /// Insert the notification if is checkOut.
        /// </summary>
        /// <param name="checkInOut">The notification.</param>
        /// <returns></returns>
        bool InserCheckOut(CheckInOut checkInOut);

        /// <summary>
        /// Gets the last checkIni by user.
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        CheckInOut GetLastChekInOutByUser(UserInfo userInfo);

    }
}
