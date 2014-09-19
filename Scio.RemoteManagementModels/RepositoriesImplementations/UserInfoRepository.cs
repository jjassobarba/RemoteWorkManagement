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

            //lista de usuarios q deben hacer checkin Hoy lista con userinfo objects
            var listadeldia = (from user in users 
                let days = user.RemoteDays.Split(',')
                where days.Any(day => day == today)
                select user).ToList();

            //usuarios que tienen checkin hoy
            var yaTienenCheckin = _session.QueryOver<CheckInOut>().List();

            var yatienenCheckin2 =  yaTienenCheckin
                .Where(x => DateTime.Compare(x.CheckInDate.Date, DateTime.Now.Date) == 0).ToList();
            List<UserInfo> checkInUserList = yatienenCheckin2.Select(user => user.IdUserInfo).ToList();
            var listaDeChecados = checkInUserList;
            
            //usuarios que no tienen checkin hoy
            var noTienenCheckin = _session.QueryOver<CheckInOut>().List();
            var noTienenCheckin2 =
                noTienenCheckin.Where(x => DateTime.Compare(x.CheckInDate.Date, DateTime.Now.Date) != 0
                    &&  DateTime.Compare(x.CheckInDate.Date, DateTime.Now.Date) != 0).ToList();

            var noTienenCheckinList = (from user in noTienenCheckin2
                group user by user.IdUserInfo
                into newGroup
                select newGroup).ToList();
            List<Guid> list = noTienenCheckinList.Select(x => x.Key.IdUserInfo).ToList();

            var nombredeusuarios = _session.QueryOver<UserInfo>()
                .Where(x => x.IdUserInfo.IsIn(list))
                .List();
            var dsf = userListToday;
            var gdfg = list;
            var asdsf = nombredeusuarios;
            var otro = listadeldia;
            var sdf = yatienenCheckin2;
            var dfo = noTienenCheckin;
            var idsuariossinchecin = noTienenCheckinList;
            var sdfgdgdf = listaDeChecados;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public IEnumerable<UserInfo> GetReadyUsers(string userName)
        {
            var today = DateTime.Now.DayOfWeek.ToString();
            var checkInOutList = _session.QueryOver<CheckInOut>().List();
            List<UserInfo> readyUsers = new List<UserInfo>();

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
                    readyUsers.Add(userRemaining);
                }
            }
            return readyUsers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public IEnumerable<UserInfo> GetNotAllowedCheckInUsers(string userName)
        {
            List<UserInfo> users = new List<UserInfo>();
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
                        users.Add(userInList);
                    }
                }
            }
            return users;
        }

    }
}
