using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Diagnostics;
using System.Linq;
using System.Web.Profile;
using System.Web.Security;
using NHibernate;
using Scio.RemoteManagementModels.Entities;

namespace Scio.RemoteManagementModels.Profile
{
    public class NHProfileProvider : ProfileProvider
    {

        #region private
        private string eventSource = "NHProfileProvider";
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
        private Entities.Profiles GetProfile(string username, bool isAuthenticated)
        {
            Entities.Profiles profile = null;
            //Is authenticated and IsAnonmous are opposites,so flip sign,IsAuthenticated = true -> notAnonymous
            bool isAnonymous = !isAuthenticated;

            using (ISession session = SessionFactory.OpenSession())
            {
                try
                {
                    var usr = session.CreateCriteria(typeof(Entities.Users))
                                                .Add(NHibernate.Criterion.Restrictions.Eq("Username", username))
                                                .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                                .UniqueResult<Entities.Users>();

                    if (usr != null)
                    {
                        profile = session.CreateCriteria(typeof(Entities.Profiles))
                                            .Add(NHibernate.Criterion.Restrictions.Eq("UsersId", usr.Id))
                                            .Add(NHibernate.Criterion.Restrictions.Eq("IsAnonymous", isAnonymous))
                                            .UniqueResult<Entities.Profiles>();
                    }
                }
                catch (Exception e)
                {
                    if (WriteExceptionsToEventLog)
                        WriteToEventLog(e, "GetProfileWithIsAuthenticated");
                    else
                        throw e;
                }

            }
            return profile;
        }

        private Entities.Profiles GetProfile(string username)
        {
            Entities.Profiles profile = null;
            using (var session = SessionFactory.OpenSession())
            {
                try
                {
                    var usr = session.CreateCriteria(typeof(Entities.Users))
                                                .Add(NHibernate.Criterion.Restrictions.Eq("Username", username))
                                                .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                                .UniqueResult<Entities.Users>();

                    if (usr != null)
                    {
                        profile = session.CreateCriteria(typeof(Entities.Profiles))
                                            .Add(NHibernate.Criterion.Restrictions.Eq("UsersId", usr.Id))
                                            .UniqueResult<Entities.Profiles>();
                    }
                }
                catch (Exception e)
                {
                    if (WriteExceptionsToEventLog)
                        WriteToEventLog(e, "GetProfile(username)");
                    else
                        throw e;
                }

            }
            return profile;
        }

        private Entities.Profiles GetProfile(int Id)
        {
            Entities.Profiles profile = null;
            using (var session = SessionFactory.OpenSession())
            {
                try
                {
                    var usr = session.CreateCriteria(typeof(Entities.Users))
                                                .Add(NHibernate.Criterion.Restrictions.Eq("Id", Id))
                                                .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                                .UniqueResult<Entities.Users>();

                    if (usr != null)
                    {
                        profile = session.CreateCriteria(typeof(Entities.Profiles))
                                            .Add(NHibernate.Criterion.Restrictions.Eq("UsersId", usr.Id))
                                            .UniqueResult<Entities.Profiles>();
                    }
                    else
                        throw new ProviderException("Membership User does not exist");

                }
                catch (Exception e)
                {
                    if (WriteExceptionsToEventLog)
                        WriteToEventLog(e, "GetProfile(id)");
                    else
                        throw e;
                }

            }
            return profile;
        }

        private Entities.Profiles CreateProfile(string username, bool isAnonymous)
        {
            var p = new Entities.Profiles();
            var profileCreated = false;

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

                        if (usr != null) //membership user exits so create a profile
                        {
                            p.UsersId = usr.Id;
                            p.IsAnonymous = isAnonymous;
                            p.LastUpdatedDate = DateTime.Now;
                            p.LastActivityDate = DateTime.Now;
                            p.ApplicationName = ApplicationName;
                            session.Save(p);
                            transaction.Commit();
                            profileCreated = true;
                        }
                        else
                            throw new ProviderException("Membership User does not exist.Profile cannot be created.");

                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "GetProfile");
                        else
                            throw e;
                    }
                }

            }

            return profileCreated ? p : null;

        }

        private bool IsMembershipUser(string username)
        {

            var hasMembership = false;

            using (var session = SessionFactory.OpenSession())
            {

                try
                {
                    var usr = session.CreateCriteria(typeof(Entities.Users))
                                                .Add(NHibernate.Criterion.Restrictions.Eq("Username", username))
                                                .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                                .UniqueResult<Entities.Users>();

                    if (usr != null) //membership user exits so create a profile
                        hasMembership = true;
                }
                catch (Exception e)
                {
                    if (WriteExceptionsToEventLog)
                        WriteToEventLog(e, "GetProfile");
                    else
                        throw e;
                }

            }

            return hasMembership;

        }

        private bool IsUserInCollection(MembershipUserCollection uc, string username)
        {
            var isInColl = false;
            foreach (MembershipUser u in from MembershipUser u in uc where u.UserName.Equals(username) select u)
            {
                isInColl = true;
            }

            return isInColl;

        }

        // Updates the LastActivityDate and LastUpdatedDate values  when profile properties are accessed by the
        // GetPropertyValues and SetPropertyValues methods. Passing true as the activityOnly parameter will update only the LastActivityDate.

        private void UpdateActivityDates(string username, bool isAuthenticated, bool activityOnly)
        {
            //Is authenticated and IsAnonmous are opposites,so flip sign,IsAuthenticated = true -> notAnonymous
            var isAnonymous = !isAuthenticated;
            var activityDate = DateTime.Now;

            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var pr = GetProfile(username, isAuthenticated);
                    if (pr == null)
                        throw new ProviderException("User Profile not found");
                    try
                    {
                        if (activityOnly)
                        {
                            pr.LastActivityDate = activityDate;
                            pr.IsAnonymous = isAnonymous;
                        }
                        else
                        {
                            pr.LastActivityDate = activityDate;
                            pr.LastUpdatedDate = activityDate;
                            pr.IsAnonymous = isAnonymous;
                        }

                        session.Update(pr);
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(e, "UpdateActivityDates");
                            throw new ProviderException(exceptionMessage);
                        }
                        throw e;
                    }
                }
            }
        }

        private bool DeleteProfile(string username)
        {
            // Check for valid user name.
            if (username == null)
                throw new ArgumentNullException("User name cannot be null.");
            if (username.Contains(","))
                throw new ArgumentException("User name cannot contain a comma (,).");

            var profile = GetProfile(username);
            if (profile == null)
                return false;

            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.Delete(profile);
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(e, "DeleteProfile");
                            throw new ProviderException(exceptionMessage);
                        }
                        throw e;
                    }
                }
            }

            return true;
        }

        private bool DeleteProfile(int id)
        {
            // Check for valid user name.
            var profile = GetProfile(id);
            if (profile == null)
                return false;

            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.Delete(profile);
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(e, "DeleteProfile(id)");
                            throw new ProviderException(exceptionMessage);
                        }
                        throw e;
                    }
                }
            }

            return true;
        }

        private int DeleteProfilesbyId(string[] ids)
        {
            var deleteCount = 0;
            try
            {
                deleteCount += ids.Count(DeleteProfile);
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "DeleteProfiles(Id())");
                    throw new ProviderException(exceptionMessage);
                }
                else
                    throw e;

            }
            return deleteCount;
        }

        private void CheckParameters(int pageIndex, int pageSize)
        {
            if (pageIndex < 0)
                throw new ArgumentException("Page index must 0 or greater.");
            if (pageSize < 1)
                throw new ArgumentException("Page size must be greater than 0.");
        }

        private ProfileInfo GetProfileInfoFromProfile(Entities.Profiles p)
        {

            Entities.Users usr = null;
            using (var session = SessionFactory.OpenSession())
            {
                usr = session.CreateCriteria(typeof(Entities.Users))
                                        .Add(NHibernate.Criterion.Restrictions.Eq("Id", p.UsersId))
                                        .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                        .UniqueResult<Entities.Users>();
            }

            if (usr == null)
                throw new ProviderException("The userid not found in memebership tables.GetProfileInfoFromProfile(p)");



            // ProfileInfo.Size not currently implemented.
            var pi = new ProfileInfo(usr.Username,
                p.IsAnonymous, p.LastActivityDate, p.LastUpdatedDate, 0);

            return pi;
        }
        #endregion

        #region Public Methods
        public override void Initialize(string name, NameValueCollection config)
        {
            // Initialize values from web.config.

            if (config == null)
                throw new ArgumentNullException("config");

            if (name.Length == 0)
                name = "NHProfileProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Sample Fluent Nhibernate Profile provider");
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

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection ppc)
        {
            var username = (string)context["UserName"];
            var isAuthenticated = (bool)context["IsAuthenticated"];
            Entities.Profiles profile = null;

            profile = GetProfile(username, isAuthenticated);
            // The serializeAs attribute is ignored in this provider implementation.
            var svc = new SettingsPropertyValueCollection();

            if (profile == null)
            {
                if (IsMembershipUser(username))
                    profile = CreateProfile(username, false);
                else
                    throw new ProviderException("Profile cannot be created. There is no membership user");
            }


            foreach (SettingsProperty prop in ppc)
            {
                var pv = new SettingsPropertyValue(prop);
                switch (prop.Name)
                {
                    case "IsAnonymous":
                        pv.PropertyValue = profile.IsAnonymous;
                        break;
                    case "LastActivityDate":
                        pv.PropertyValue = profile.LastActivityDate;
                        break;
                    case "LastUpdatedDate":
                        pv.PropertyValue = profile.LastUpdatedDate;
                        break;
                    case "Subscription":
                        pv.PropertyValue = profile.Subscription;
                        break;
                    case "Language":
                        pv.PropertyValue = profile.Language;
                        break;
                    case "FirstName":
                        pv.PropertyValue = profile.FirstName;
                        break;
                    case "LastName":
                        pv.PropertyValue = profile.LastName;
                        break;
                    case "Gender":
                        pv.PropertyValue = profile.Gender;
                        break;
                    case "BirthDate":
                        pv.PropertyValue = profile.BirthDate;
                        break;
                    case "Occupation":
                        pv.PropertyValue = profile.Occupation;
                        break;
                    case "Website":
                        pv.PropertyValue = profile.Website;
                        break;
                    case "Street":
                        pv.PropertyValue = profile.Street;
                        break;
                    case "City":
                        pv.PropertyValue = profile.City;
                        break;
                    case "State":
                        pv.PropertyValue = profile.State;
                        break;
                    case "Zip":
                        pv.PropertyValue = profile.Zip;
                        break;
                    case "Country":
                        pv.PropertyValue = profile.Country;
                        break;

                    default:
                        throw new ProviderException("Unsupported property.");
                }

                svc.Add(pv);
            }

            UpdateActivityDates(username, isAuthenticated, true);
            return svc;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection ppvc)
        {
            // The serializeAs attribute is ignored in this provider implementation.
            var username = (string)context["UserName"];
            var isAuthenticated = (bool)context["IsAuthenticated"];

            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {

                    var profile = GetProfile(username, isAuthenticated) ?? CreateProfile(username, !isAuthenticated);

                    foreach (SettingsPropertyValue pv in ppvc)
                    {
                        switch (pv.Property.Name)
                        {
                            case "IsAnonymous":
                                profile.IsAnonymous = (bool)pv.PropertyValue;
                                break;
                            case "LastActivityDate":
                                profile.LastActivityDate = (DateTime)pv.PropertyValue;
                                break;
                            case "LastUpdatedDate":
                                profile.LastUpdatedDate = (DateTime)pv.PropertyValue;
                                break;
                            case "Subscription":
                                profile.Subscription = pv.PropertyValue.ToString();
                                break;
                            case "Language":
                                profile.Language = pv.PropertyValue.ToString();
                                break;
                            case "FirstName":
                                profile.FirstName = pv.PropertyValue.ToString();
                                break;
                            case "LastName":
                                profile.LastName = pv.PropertyValue.ToString();
                                break;
                            case "Gender":
                                profile.Gender = pv.PropertyValue.ToString();
                                break;
                            case "BirthDate":
                                profile.BirthDate = (DateTime)pv.PropertyValue;
                                break;
                            case "Occupation":
                                profile.Occupation = pv.PropertyValue.ToString();
                                break;
                            case "Website":
                                profile.Website = pv.PropertyValue.ToString();
                                break;
                            case "Street":
                                profile.Street = pv.PropertyValue.ToString();
                                break;
                            case "City":
                                profile.City = pv.PropertyValue.ToString();
                                break;
                            case "State":
                                profile.State = pv.PropertyValue.ToString();
                                break;
                            case "Zip":
                                profile.Zip = pv.PropertyValue.ToString();
                                break;
                            case "Country":
                                profile.Country = pv.PropertyValue.ToString();
                                break;
                            default:
                                throw new ProviderException("Unsupported property.");
                        }
                    }

                    session.SaveOrUpdate(profile);
                    transaction.Commit();
                }
            }

            UpdateActivityDates(username, isAuthenticated, false);
        }

        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            var deleteCount = 0;
            try
            {
                deleteCount += profiles.Cast<ProfileInfo>().Count(p => DeleteProfile(p.UserName));
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "DeleteProfiles(ProfileInfoCollection)");
                    throw new ProviderException(exceptionMessage);
                }
                throw e;
            }
            return deleteCount;
        }

        public override int DeleteProfiles(string[] usernames)
        {
            var deleteCount = 0;
            try
            {
                deleteCount += usernames.Count(DeleteProfile);
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "DeleteProfiles(String())");
                    throw new ProviderException(exceptionMessage);
                }
                throw e;
            }
            return deleteCount;
        }

        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            var userIds = "";
            var anaon = false;
            switch (authenticationOption)
            {
                case ProfileAuthenticationOption.Anonymous:
                    anaon = true;
                    break;
                case ProfileAuthenticationOption.Authenticated:
                    break;
            }

            using (var session = SessionFactory.OpenSession())
            {
                try
                {
                    var profs = session.CreateCriteria(typeof(Entities.Profiles))
                                                    .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                                    .Add(NHibernate.Criterion.Restrictions.Le("LastActivityDate", userInactiveSinceDate))
                                                    .Add(NHibernate.Criterion.Restrictions.Eq("IsAnonymous", anaon))
                                                    .List<Entities.Profiles>();

                    if (profs != null)
                    {
                        userIds = profs.Aggregate(userIds, (current, p) => current + (p.Id.ToString() + ","));
                    }
                }
                catch (Exception e)
                {
                    if (WriteExceptionsToEventLog)
                        WriteToEventLog(e, "DeleteInactiveProfiles");
                    else
                        throw e;
                }

            }

            if (userIds.Length > 0)
                userIds = userIds.Substring(0, userIds.Length - 1);


            return DeleteProfilesbyId(userIds.Split(','));
        }

        public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch,
                                                                       int pageIndex,
                                                                       int pageSize,
                                                                       out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            return GetProfileInfo(authenticationOption, usernameToMatch, null, pageIndex, pageSize, out totalRecords);
        }

        public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch,
                                                                              DateTime userInactiveSinceDate,
                                                                              int pageIndex,
                                                                              int pageSize,
                                                                              out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            return GetProfileInfo(authenticationOption, usernameToMatch, userInactiveSinceDate, pageIndex, pageSize, out totalRecords);
        }

        public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex,
                                                                              int pageSize,
                                                                              out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            return GetProfileInfo(authenticationOption, null, null, pageIndex, pageSize, out totalRecords);
        }

        public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate,
                                                                          int pageIndex,
                                                                          int pageSize,
                                                                          out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            return GetProfileInfo(authenticationOption, null, userInactiveSinceDate, pageIndex, pageSize, out totalRecords);
        }

        public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            var inactiveProfiles = 0;

            var profiles =
              GetProfileInfo(authenticationOption, null, userInactiveSinceDate, 0, 0, out inactiveProfiles);

            return inactiveProfiles;
        }


        // GetProfileInfo
        // Retrieves a count of profiles and creates a 
        // ProfileInfoCollection from the profile data in the 
        // database. Called by GetAllProfiles, GetAllInactiveProfiles,
        // FindProfilesByUserName, FindInactiveProfilesByUserName, 
        // and GetNumberOfInactiveProfiles.
        // Specifying a pageIndex of 0 retrieves a count of the results only.
        //

        private ProfileInfoCollection GetProfileInfo(ProfileAuthenticationOption authenticationOption, string usernameToMatch,
                                                                      object userInactiveSinceDate,
                                                                      int pageIndex,
                                                                      int pageSize,
                                                                      out int totalRecords)
        {

            var isAnaon = false;
            var profilesInfoColl = new ProfileInfoCollection();
            switch (authenticationOption)
            {
                case ProfileAuthenticationOption.Anonymous:
                    isAnaon = true;
                    break;
                case ProfileAuthenticationOption.Authenticated:
                    break;
            }

            using (var session = SessionFactory.OpenSession())
            {
                try
                {
                    var cprofiles = session.CreateCriteria(typeof(Entities.Profiles));
                    cprofiles.Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName));



                    if (userInactiveSinceDate != null)
                        cprofiles.Add(NHibernate.Criterion.Restrictions.Le("LastActivityDate", (DateTime)userInactiveSinceDate));

                    cprofiles.Add(NHibernate.Criterion.Restrictions.Eq("IsAnonymous", isAnaon));


                    var profiles = cprofiles.List<Entities.Profiles>();
                    IList<Entities.Profiles> profiles2 = null;

                    if (profiles == null)
                        totalRecords = 0;
                    else if (profiles.Count < 1)
                        totalRecords = 0;
                    else
                        totalRecords = profiles.Count;



                    //IF USER NAME TO MATCH then fileter out those
                    //Membership.FNHMembershipProvider us = new INCT.FNHProviders.Membership.FNHMembershipProvider();
                    //us.g
                    var uc = System.Web.Security.Membership.FindUsersByName(usernameToMatch);

                    if (usernameToMatch != null)
                    {
                        if (totalRecords > 0)
                        {
                            foreach (Profiles p in profiles.Where(p => IsUserInCollection(uc, usernameToMatch)))
                            {
                                profiles2.Add(p);
                            }

                            if (profiles2 == null)
                                profiles2 = profiles;
                            else if (profiles2.Count < 1)
                                profiles2 = profiles;
                            else
                                totalRecords = profiles2.Count;
                        }
                        else
                            profiles2 = profiles;
                    }
                    else
                        profiles2 = profiles;




                    if (totalRecords <= 0)
                        return profilesInfoColl;

                    if (pageSize == 0)
                        return profilesInfoColl;

                    var counter = 0;
                    var startIndex = pageSize * (pageIndex - 1);
                    int endIndex = startIndex + pageSize - 1;

                    foreach (var p in profiles2)
                    {
                        if (counter >= endIndex)
                            break;
                        if (counter >= startIndex)
                        {
                            var pi = GetProfileInfoFromProfile(p);
                            profilesInfoColl.Add(pi);
                        }
                        counter++;
                    }
                }
                catch (Exception e)
                {
                    if (WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(e, "GetProfileInfo");
                        throw new ProviderException(exceptionMessage);
                    }
                    throw e;
                }
            }
            return profilesInfoColl;
        }



        #endregion
    }
}
