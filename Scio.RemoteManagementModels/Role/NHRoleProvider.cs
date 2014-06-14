using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Security;
using NHibernate;
using Scio.RemoteManagementModels.Entities;

namespace Scio.RemoteManagementModels.Role
{
    public class NHRoleProvider : RoleProvider
    {
        #region private
        private string eventSource = "FNHRoleProvider";
        private string eventLog = "RemoteManagementModels";
        private string exceptionMessage = "An exception occurred. Please check the Event Log.";
        private string connectionString;
        private string _applicationName;
        private static ISessionFactory _sessionFactory;

        #endregion

        #region Properties
        /// <summary>Gets the session factory.</summary>
        private static ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
        }
        public override string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        public bool WriteExceptionsToEventLog { get; set; }
        #endregion

        #region Helper Functions
        // A helper function to retrieve config values from the configuration file
        private string GetConfigValue(string configValue, string defaultValue)
        {
            return String.IsNullOrEmpty(configValue) ? defaultValue : configValue;
        }

        private void WriteToEventLog(Exception e, string action)
        {
            var log = new EventLog();
            log.Source = eventSource;
            log.Log = eventLog;

            var message = exceptionMessage + "\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + e;

            log.WriteEntry(message);
        }
        #endregion

        #region Private Methods
        //get a role by name
        private Entities.Roles GetRole(string rolename)
        {
            Entities.Roles role = null;
            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        role = session.CreateCriteria(typeof(Entities.Roles))
                            .Add(NHibernate.Criterion.Restrictions.Eq("RoleName", rolename))
                            .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                            .UniqueResult<Entities.Roles>();

                        //just to lazy init the collection, otherwise get the error 
                        //NHibernate.LazyInitializationException: failed to lazily initialize a collection, no session or session was closed
                        var us = role.UsersInRole;

                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "GetRole");
                        else
                            throw e;
                    }
                }
            }
            return role;
        }

        #endregion

        #region Public Methods
        //initializes the FNH role provider
        public override void Initialize(string name, NameValueCollection config)
        {
            // Initialize values from web.config.

            if (config == null)
                throw new ArgumentNullException("config");

            if (name.Length == 0)
                name = "FNHRoleProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Sample Fluent Nhibernate Role provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            _applicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            WriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));

            // Initialize Connection.
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];
            if (connectionStringSettings == null || connectionStringSettings.ConnectionString.Trim() == "")
                throw new ProviderException("Connection string cannot be blank.");

            connectionString = connectionStringSettings.ConnectionString;
            // create our Fluent NHibernate session factory
            _sessionFactory = SessionHelper.CreateSessionFactory(connectionString);
        }

        //adds a user collection toa roles collection
        public override void AddUsersToRoles(string[] usernames, string[] rolenames)
        {
            Entities.Users usr = null;
            foreach (var rolename in rolenames.Where(rolename => !RoleExists(rolename)))
            {
                throw new ProviderException(String.Format("Role name {0} not found.", rolename));
            }

            foreach (var username in usernames)
            {
                if (username.Contains(","))
                    throw new ArgumentException(String.Format("User names {0} cannot contain commas.", username));
                //is user not exiting //throw exception

                foreach (var rolename in rolenames)
                {
                    if (IsUserInRole(username, rolename))
                        throw new ProviderException(String.Format("User {0} is already in role {1}.", username, rolename));
                }
            }

            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        foreach (var username in usernames)
                        {
                            foreach (var rolename in rolenames)
                            {
                                //get the user
                                usr = session.CreateCriteria(typeof(Entities.Users))
                                            .Add(NHibernate.Criterion.Restrictions.Eq("Username", username))
                                            .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                            .UniqueResult<Entities.Users>();

                                if (usr != null)
                                {
                                    //get the role first from db
                                    var role = session.CreateCriteria(typeof(Entities.Roles))
                                            .Add(NHibernate.Criterion.Restrictions.Eq("RoleName", rolename))
                                            .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                            .UniqueResult<Entities.Roles>();

                                    //Entities.Roles role = GetRole(rolename);
                                    usr.AddRole(role);
                                }
                            }
                            session.SaveOrUpdate(usr);
                        }
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "AddUsersToRoles");
                        else
                            throw e;
                    }

                }
            }
        }

        //create  a new role with a given name
        public override void CreateRole(string rolename)
        {
            if (rolename.Contains(","))
                throw new ArgumentException("Role names cannot contain commas.");

            if (RoleExists(rolename))
                throw new ProviderException("Role name already exists.");

            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var role = new Entities.Roles();
                        role.ApplicationName = this.ApplicationName;
                        role.RoleName = rolename;
                        session.Save(role);
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "CreateRole");
                        else
                            throw e;
                    }
                }
            }
        }

        //delete a role with given name
        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
        {
            var deleted = false;
            if (!RoleExists(rolename))
                throw new ProviderException("Role does not exist.");

            if (throwOnPopulatedRole && GetUsersInRole(rolename).Length > 0)
                throw new ProviderException("Cannot delete a populated role.");

            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var role = GetRole(rolename);
                        session.Delete(role);
                        transaction.Commit();

                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(e, "DeleteRole");
                            return deleted;
                        }
                        throw e;
                    }
                }
            }

            return deleted;
        }

        //get an array of all the roles
        public override string[] GetAllRoles()
        {
            var sb = new StringBuilder();
            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var allroles = session.CreateCriteria(typeof(Entities.Roles))
                                        .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                        .List<Entities.Roles>();

                        foreach (var r in allroles)
                        {
                            sb.Append(r.RoleName + ",");
                        }
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "GetAllRoles");
                        else
                            throw e;
                    }
                }
            }

            if (sb.Length > 0)
            {
                // Remove trailing comma.
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString().Split(',');
            }

            return new string[0];
        }

        //Get roles for a user by username
        public override string[] GetRolesForUser(string username)
        {
            var sb = new StringBuilder();
            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var usr = session.CreateCriteria(typeof(Entities.Users))
                            .Add(NHibernate.Criterion.Restrictions.Eq("Username", username))
                            .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                            .UniqueResult<Entities.Users>();

                        if (usr != null)
                        {
                            var usrroles = usr.Roles;
                            foreach (var r in usrroles)
                            {
                                sb.Append(r.RoleName + ",");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "GetRolesForUser");
                        else
                            throw e;
                    }
                }
            }

            if (sb.Length > 0)
            {
                // Remove trailing comma.
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString().Split(',');
            }

            return new string[0];
        }
        //Get users in a givenrolename
        public override string[] GetUsersInRole(string rolename)
        {
            var sb = new StringBuilder();
            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var role = session.CreateCriteria(typeof(Entities.Roles))
                                        .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                        .Add(NHibernate.Criterion.Restrictions.Eq("RoleName", rolename))
                                        .UniqueResult<Entities.Roles>();

                        var usrs = role.UsersInRole;

                        foreach (var u in usrs)
                        {
                            sb.Append(u.Username + ",");
                        }
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "GetUsersInRole");
                        else
                            throw e;
                    }
                }
            }

            if (sb.Length > 0)
            {
                // Remove trailing comma.
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString().Split(',');
            }

            return new string[0];
        }

        //determine is a user has a given role
        public override bool IsUserInRole(string username, string rolename)
        {
            var userIsInRole = false;
            var sb = new StringBuilder();
            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var usr = session.CreateCriteria(typeof(Entities.Users))
                            .Add(NHibernate.Criterion.Restrictions.Eq("Username", username))
                            .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                            .UniqueResult<Entities.Users>();

                        if (usr != null)
                        {
                            var usrroles = usr.Roles;
                            if (usrroles.Any(r => r.RoleName.Equals(rolename)))
                            {
                                userIsInRole = true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "IsUserInRole");
                        else
                            throw e;
                    }
                }
            }
            return userIsInRole;
        }

        //remeove users from roles
        public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
        {
            foreach (var rolename in rolenames)
            {
                if (!RoleExists(rolename))
                    throw new ProviderException(String.Format("Role name {0} not found.", rolename));
            }

            foreach (var username in usernames)
            {
                foreach (var rolename in rolenames.Where(rolename => !IsUserInRole(username, rolename)))
                {
                    throw new ProviderException(String.Format("User {0} is not in role {1}.", username, rolename));
                }
            }

            //get user , get his roles , the remove the role and save   
            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        foreach (var username in usernames)
                        {
                            var usr = session.CreateCriteria(typeof(Entities.Users))
                                .Add(NHibernate.Criterion.Restrictions.Eq("Username", username))
                                .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                .UniqueResult<Entities.Users>();

                            var rolestodelete = (from rolename in rolenames let roles = usr.Roles from r in roles where r.RoleName.Equals(rolename) select r).ToList();
                            foreach (var rd in rolestodelete)
                                usr.RemoveRole(rd);


                            session.SaveOrUpdate(usr);
                        }
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "RemoveUsersFromRoles");
                        else
                            throw e;
                    }
                }
            }

        }

        //boolen to check if a role exists given a role name
        public override bool RoleExists(string rolename)
        {
            var exists = false;

            var sb = new StringBuilder();
            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var role = session.CreateCriteria(typeof(Entities.Roles))
                                            .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                            .Add(NHibernate.Criterion.Restrictions.Eq("RoleName", rolename))
                                            .UniqueResult<Entities.Roles>();
                        if (role != null)
                            exists = true;

                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "RoleExists");
                        else
                            throw e;
                    }
                }
            }
            return exists;
        }

        //find users that beloeng to a particular role , given a username, Note : does not do a LIke search
        public override string[] FindUsersInRole(string rolename, string usernameToMatch)
        {
            var sb = new StringBuilder();
            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var role = session.CreateCriteria(typeof(Entities.Roles))
                                        .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                        .Add(NHibernate.Criterion.Restrictions.Eq("RoleName", this.ApplicationName))
                                        .UniqueResult<Entities.Roles>();

                        var users = role.UsersInRole;
                        if (users != null)
                        {
                            foreach (var u in users.Where(u => String.Compare(u.Username, usernameToMatch, true) == 0))
                            {
                                sb.Append(u.Username + ",");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "FindUsersInRole");
                        else
                            throw e;
                    }
                }
                if (sb.Length > 0)
                {
                    // Remove trailing comma.
                    sb.Remove(sb.Length - 1, 1);
                    return sb.ToString().Split(',');
                }
                return new string[0];
            }
        }

        #endregion
    }
}
