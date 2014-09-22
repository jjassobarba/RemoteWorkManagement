using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using FluentNHibernate.Conventions;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Linq;
using NHibernate.Mapping;
using Scio.RemoteManagementModels.Entities;
using Scio.RemoteManagementModels.RepositoriesContracts;
using System.Collections;

namespace Scio.RemoteManagementModels.RepositoriesImplementations
{
    public class UserInfoRepository : IUserInfoRepository
    {
        private ISession _session;
        private const string DefaultPicture = "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";


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
        
        /// <summary>
        /// Gets the remaining users
        /// </summary>
        /// <returns>list</returns>
        public IEnumerable<UserInfo> GetRemainingUsers(string userName)
        {
            var today = DateTime.Now.DayOfWeek.ToString();
            var checkInOutList = _session.QueryOver<CheckInOut>().List();
            var users = _session.QueryOver<UserInfo>().List();

            var userslist = _session.QueryOver<Notifications>()
                .Where(x => x.ProjectLeaderMail == userName || x.SenseiMail == userName)
                .List();
            List<UserInfo> userListToday = userslist.Select(x => x.IdUserInfo).ToList();

            var usersRemainingToday = (from user in userListToday
                let days = user.RemoteDays.Split(',')
                where days.Any(day => day == today)
                select user).ToList();

            List<UserInfo> realUserRemainingToday = usersRemainingToday.ToList();

            var usersCheckIn = checkInOutList
                .Where(x => DateTime.Compare(x.CheckInDate.Date, DateTime.Now.Date) == 0).ToList();
            
            foreach (var userDone in usersCheckIn)
            {
                foreach (var userRemaining in usersRemainingToday.Where(userRemaining => userDone.IdUserInfo.IdUserInfo == userRemaining.IdUserInfo))
                {
                    realUserRemainingToday.Remove(userRemaining);
                }
            }
            return realUserRemainingToday;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public IEnumerable<object> GetReadyUsers(string userName)
        {
            var today = DateTime.Now.DayOfWeek.ToString();
            var checkInOutList = _session.QueryOver<CheckInOut>().List();
            List<object> readyUsers = new List<object>();

            var userslist = _session.QueryOver<Notifications>()
                .Where(x => x.ProjectLeaderMail == userName || x.SenseiMail == userName)
                .List();

            // List of users assigned to this project leader/sensei
            List<UserInfo> userListToday = userslist.Select(x => x.IdUserInfo).ToList();

            // List of users allowed to work remotely assigned to this pl/s
            var usersRemainingToday = (from user in userListToday
                                       let days = user.RemoteDays.Split(',')
                                       where days.Any(day => day == today)
                                       select user).ToList();

            // List of all the Checkin registered today
            var usersCheckIn = checkInOutList
                .Where(x => DateTime.Compare(x.CheckInDate.Date, DateTime.Now.Date) == 0).ToList();

            foreach (var userDone in usersCheckIn)
            {
                foreach (var userRemaining in usersRemainingToday
                    .Where(userRemaining => userDone.IdUserInfo.IdUserInfo == userRemaining.IdUserInfo))
                {
                    var usersInfoList = new
                    {
                        IdUserInfo = userRemaining.IdUserInfo,
                        FirstName = userRemaining.FirstName,
                        LastName = userRemaining.LastName,
                        FlexTime = GetTimeAfterCheckIn(userDone),
                        OtherFlexTime = userRemaining.OtherFlexTime,
                        Picture = userRemaining.Picture == null
                                ? DefaultPicture
                                : Convert.ToBase64String(userRemaining.Picture),
                        Position = userRemaining.Position,
                        ReceiveNotifications = userRemaining.ReceiveNotifications,
                        RemoteDays = userRemaining.RemoteDays,
                        IsManualCheckIn = userDone.IsManualCheckIn,
                        CommentsCheckIn = userDone.Comments,
                        IdMembership = new
                        {
                            IdMembership = userRemaining.IdMembership.Id,
                            Email = userRemaining.IdMembership.Username
                        },
                        Rol = new
                        {
                            RolName = userRemaining.IdMembership.Roles.Select(p => p.RoleName).FirstOrDefault()
                        }
                    };
                    readyUsers.Add(usersInfoList);
                }
            }
            return readyUsers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public IEnumerable<object> GetNotAllowedCheckInUsers(string userName)
        {
            List<object> users = new List<object>();
            var today = DateTime.Now.DayOfWeek.ToString();
            var checkInOutList = _session.QueryOver<CheckInOut>().List();

            var unauthorizedCheckInOutList = checkInOutList
                .Where(x => !x.IsAuthorized && DateTime.Compare(x.CheckInDate.Date,
                                                                           DateTime.Now.Date) == 0).ToList();
            
            var assignedNotificationsList = _session.QueryOver<Notifications>()
                .Where(x => x.ProjectLeaderMail == userName || x.SenseiMail == userName)
                .List();

            List<UserInfo> assignedUsersList = assignedNotificationsList.Select(x => x.IdUserInfo).ToList();
           
            foreach (var unauthorizedUser in unauthorizedCheckInOutList)
            {
                foreach (var userInList in assignedUsersList)
                {
                    if (unauthorizedUser.IdUserInfo.IdUserInfo == userInList.IdUserInfo)
                    {
                        var usersInfoList = new
                        {
                            IdUserInfo = userInList.IdUserInfo,
                            FirstName = userInList.FirstName,
                            LastName = userInList.LastName,
                            FlexTime = GetTimeAfterCheckIn(unauthorizedUser),
                            OtherFlexTime = userInList.OtherFlexTime,
                            Picture = userInList.Picture == null
                                    ? DefaultPicture
                                    : Convert.ToBase64String(userInList.Picture),
                            Position = userInList.Position,
                            ReceiveNotifications = userInList.ReceiveNotifications,
                            RemoteDays = userInList.RemoteDays,
                            IsManualCheckIn = unauthorizedUser.IsManualCheckIn,
                            CommentsCheckIn = unauthorizedUser.Comments,
                            IdMembership = new
                            {
                                IdMembership = userInList.IdMembership.Id,
                                Email = userInList.IdMembership.Username
                            },
                            Rol = new
                            {
                                RolName = userInList.IdMembership.Roles.Select(p => p.RoleName).FirstOrDefault()
                            }
                        };
                        users.Add(usersInfoList);
                    }
                }
            }
            return users;
        }


        /// <summary>
        /// Gets the time that the user has been online
        /// </summary>
        /// <param name="checkInOut"></param>
        /// <returns></returns>
        public string GetTimeAfterCheckIn(CheckInOut checkInOut)
        {
            string message = "";
            TimeSpan failTime = new TimeSpan(checkInOut.CheckInDate.Ticks - DateTime.Now.Ticks);
            if (failTime < TimeSpan.Zero)
            {
                message = failTime.ToString(@"hh\:mm\:ss") + " online";
            }
            else
            {
                message = failTime.ToString(@"hh\:mm\:ss") + " online";
            }
            return message;
        }

    }
}
