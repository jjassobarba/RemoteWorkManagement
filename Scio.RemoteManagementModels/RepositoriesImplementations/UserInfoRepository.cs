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
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Notifications> GetChildUsers(string userName)
        {
            var today = DateTime.Now.DayOfWeek.ToString();

            // lista de usuarios a cargo del proyect actual 
            var userslist = _session.QueryOver<Notifications>()
                .Where(x => x.ProjectLeaderMail == userName)
                .List();
            
            //lsia de todos los usuarios
            var users = _session.QueryOver<UserInfo>().List();

            //lista de usuarios q deben hacer checkin Hoy
            var listadeldia = (from user in users
                let days = user.RemoteDays.Split(',')
                where days.Any(day => day == today)
                select user).ToList();

            //usuarios que tienen checkin hoy
            var yaTienenCheckin = _session.QueryOver<CheckInOut>().List();
            var yatienenCheckin2 =
                yaTienenCheckin.Where(x => DateTime.Compare(x.CheckInDate.Date, DateTime.Now.Date) == 0).ToList();
            List<UserInfo> checkInUserList = new List<UserInfo>();
            foreach (var user in yatienenCheckin2)
            {
                checkInUserList.Add(user.IdUserInfo);
            }
            var listaDeChecados = checkInUserList;

            //usuarios que no tienen checkin hoy
            var noTienenCheckin = _session.QueryOver<CheckInOut>().List();
            var noTienenCheckin2 =
                noTienenCheckin.Where(x => DateTime.Compare(x.CheckInDate.Date, DateTime.Now.Date) != 0).ToList();

            var noTienenCheckinList = (from user in noTienenCheckin2
                group user by user.IdUserInfo
                into newGroup
                select newGroup).ToList();

            List<Guid> list = new List<Guid>();
            
            foreach (var x in noTienenCheckinList)
            {
                list.Add(x.Key.IdUserInfo); 
            }
            var nombredeusuarios = _session.QueryOver<UserInfo>().Where(x => x.IdUserInfo.IsIn(list)).List();
            



            var gdfg = list;
            var asdsf = nombredeusuarios;
            var otro = listadeldia;
            var sdf = yatienenCheckin2;
            var dfo = noTienenCheckin;
            var idsuariossinchecin = noTienenCheckinList;
            var sdfgdgdf = listaDeChecados;
            return userslist;
        }
    }
}
