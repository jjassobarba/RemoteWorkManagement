using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Web.Security;
using NHibernate;
using Scio.RemoteManagementModels.Entities;

namespace Scio.RemoteManagementModels.Membership
{
    public class NHMembershipProvider : MembershipProvider
    {
        #region Private
        // Global connection string, generated password length, generic exception message, event log info.
        private int newPasswordLength = 8;
        private string eventSource = "NHMembershipProvider";
        private string eventLog = "RemoteManagementModels";
        private string exceptionMessage = "An exception occurred. Please check the Event Log.";
        private string connectionString;

        private static ISessionFactory _sessionFactory;
        private string _applicationName;
        private bool _enablePasswordReset;
        private bool _enablePasswordRetrieval;
        private bool _requiresQuestionAndAnswer;
        private bool _requiresUniqueEmail;
        private int _maxInvalidPasswordAttempts;
        private int _passwordAttemptWindow;
        private MembershipPasswordFormat _passwordFormat;
        // Used when determining encryption key values.
        private MachineKeySection _machineKey;
        private int _minRequiredNonAlphanumericCharacters;
        private int _minRequiredPasswordLength;
        private string _passwordStrengthRegularExpression;

        #endregion

        #region Properties

        public override string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        public override bool EnablePasswordReset
        {
            get { return _enablePasswordReset; }
        }


        public override bool EnablePasswordRetrieval
        {
            get { return _enablePasswordRetrieval; }
        }


        public override bool RequiresQuestionAndAnswer
        {
            get { return _requiresQuestionAndAnswer; }
        }


        public override bool RequiresUniqueEmail
        {
            get { return _requiresUniqueEmail; }
        }


        public override int MaxInvalidPasswordAttempts
        {
            get { return _maxInvalidPasswordAttempts; }
        }


        public override int PasswordAttemptWindow
        {
            get { return _passwordAttemptWindow; }
        }


        public override MembershipPasswordFormat PasswordFormat
        {
            get { return _passwordFormat; }
        }


        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _minRequiredNonAlphanumericCharacters; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return _minRequiredPasswordLength; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return _passwordStrengthRegularExpression; }
        }

        // If false, exceptions are thrown to the caller. If true,
        // exceptions are written to the event log.
        public bool WriteExceptionsToEventLog { get; set; }

        /// <summary>Gets the session factory.</summary>
        private static ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
        }
        #endregion

        #region Implemented

        // Initilaize the provider 
        public override void Initialize(string name, NameValueCollection config)
        {
            // Initialize values from web.config.
            if (config == null)
                throw new ArgumentNullException("config");

            if (name.Length == 0)
                name = "NHMemebershipProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Fluent Nhibernate Membership provider");
            }
            // Initialize the abstract base class.
            base.Initialize(name, config);

            _applicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            _maxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            _passwordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            _minRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
            _minRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            _passwordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            _enablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            _enablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            _requiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            _requiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
            WriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));


            var tempFormat = config["passwordFormat"] ?? "Hashed";

            switch (tempFormat)
            {
                case "Hashed":
                    _passwordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    _passwordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    _passwordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }


            //
            // Initialize Connection.
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];
            if (connectionStringSettings == null || connectionStringSettings.ConnectionString.Trim() == "")
                throw new ProviderException("Connection string cannot be blank.");

            connectionString = connectionStringSettings.ConnectionString;
            // Get encryption and decryption key information from the configuration.

            //Encryption skipped
            var cfg =
                WebConfigurationManager.OpenWebConfiguration(
                    System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            _machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

            if (_machineKey.ValidationKey.Contains("AutoGenerate"))
                if (PasswordFormat != MembershipPasswordFormat.Clear)
                    throw new ProviderException("Hashed or Encrypted passwords are not supported with auto-generated keys.");


            // create our Fluent NHibernate session factory
            _sessionFactory = SessionHelper.CreateSessionFactory(connectionString);
            //private static ISessionFactory _sessionFactory2 =_sessionFactory;

        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer,
            bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            var args = new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);
            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            var u = GetUser(username, false);

            if (u == null)
            {
                var createDate = DateTime.Now;

                //provider user key in our case is auto int

                using (var session = SessionFactory.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var user = new Entities.Users
                        {
                            Username = username,
                            Password = EncodePassword(password),
                            Email = email,
                            PasswordQuestion = passwordQuestion,
                            PasswordAnswer = EncodePassword(passwordAnswer),
                            IsApproved = isApproved,
                            Comment = "",
                            CreationDate = createDate,
                            LastPasswordChangedDate = createDate,
                            LastActivityDate = createDate,
                            ApplicationName = _applicationName,
                            IsLockedOut = false,
                            LastLockedOutDate = createDate,
                            FailedPasswordAttemptCount = 0,
                            FailedPasswordAttemptWindowStart = createDate,
                            FailedPasswordAnswerAttemptCount = 0,
                            FailedPasswordAnswerAttemptWindowStart = createDate
                        };

                        try
                        {
                            var retId = (int)session.Save(user);

                            transaction.Commit();
                            status = (retId < 1) ? MembershipCreateStatus.UserRejected : MembershipCreateStatus.Success;
                        }
                        catch (Exception e)
                        {
                            status = MembershipCreateStatus.ProviderError;
                            if (WriteExceptionsToEventLog)
                                WriteToEventLog(e, "CreateUser");
                        }
                    }
                }

                //retrive and return user by user name
                return GetUser(username, false);
            }
            status = MembershipCreateStatus.DuplicateUserName;
            return null;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion,
            string newPasswordAnswer)
        {
            var rowsAffected = 0;
            if (!ValidateUser(username, password))
                return false;

            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var usr = GetUserByUsername(username);
                        if (usr != null)
                        {
                            usr.PasswordQuestion = newPasswordQuestion;
                            usr.PasswordAnswer = newPasswordAnswer;
                            session.Update(usr);
                            transaction.Commit();
                            rowsAffected = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "ChangePasswordQuestionAndAnswer");
                        throw new ProviderException(exceptionMessage);
                    }
                }
            }

            return rowsAffected > 0;
        }

        public override string GetPassword(string username, string answer)
        {
            string password;
            string passwordAnswer;

            if (!EnablePasswordRetrieval)
                throw new ProviderException("Password Retrieval Not Enabled.");

            if (PasswordFormat == MembershipPasswordFormat.Hashed)
                throw new ProviderException("Cannot retrieve Hashed passwords.");

            try
            {
                var usr = GetUserByUsername(username);

                if (usr == null)
                    throw new MembershipPasswordException("The supplied user name is not found.");
                if (usr.IsLockedOut)
                    throw new MembershipPasswordException("The supplied user is locked out.");

                password = usr.Password;
                passwordAnswer = usr.PasswordAnswer;
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetPassword");
                throw new ProviderException(exceptionMessage);
            }

            if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
            {
                UpdateFailureCount(username, "passwordAnswer");
                throw new MembershipPasswordException("Incorrect password answer.");
            }

            if (PasswordFormat == MembershipPasswordFormat.Encrypted)
                password = UnEncodePassword(password);

            return password;
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var rowsAffected = 0;
            if (!ValidateUser(username, oldPassword))
                return false;

            var args = new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");

            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {

                    try
                    {
                        var usr = GetUserByUsername(username);

                        if (usr != null)
                        {
                            usr.Password = EncodePassword(newPassword);
                            usr.LastPasswordChangedDate = DateTime.Now;
                            session.Update(usr);
                            transaction.Commit();
                            rowsAffected = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "ChangePassword");
                        throw new ProviderException(exceptionMessage);
                    }
                }
            }

            return rowsAffected > 0;
        }

        public override string ResetPassword(string username, string answer)
        {
            var rowsAffected = 0;

            if (!EnablePasswordReset)
                throw new NotSupportedException("Password reset is not enabled.");

            if (answer == null && RequiresQuestionAndAnswer)
            {
                UpdateFailureCount(username, "passwordAnswer");
                throw new ProviderException("Password answer required for password reset.");
            }

            string newPassword =
                            System.Web.Security.Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);


            var args = new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");

            var passwordAnswer = "";

            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var usr = GetUserByUsername(username);
                        if (usr == null)
                            throw new MembershipPasswordException("The supplied user name is not found.");

                        if (usr.IsLockedOut)
                            throw new MembershipPasswordException("The supplied user is locked out.");

                        if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
                        {
                            UpdateFailureCount(username, "passwordAnswer");
                            throw new MembershipPasswordException("Incorrect password answer.");
                        }

                        usr.Password = EncodePassword(newPassword);
                        usr.LastPasswordChangedDate = System.DateTime.Now;
                        usr.Username = username;
                        usr.ApplicationName = this.ApplicationName;
                        session.Update(usr);
                        transaction.Commit();
                        rowsAffected = 1;
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "ResetPassword");
                        throw new ProviderException(exceptionMessage);
                    }
                }
            }

            if (rowsAffected > 0)
                return newPassword;
            throw new MembershipPasswordException("User not found, or user is locked out. Password not Reset.");
        }

        public override void UpdateUser(MembershipUser user)
        {
            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var usr = GetUserByUsername(user.UserName);
                        if (usr != null)
                        {
                            usr.Email = user.Email;
                            usr.Comment = user.Comment;
                            usr.IsApproved = user.IsApproved;
                            session.Update(usr);
                            transaction.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(e, "UpdateUser");
                            throw new ProviderException(exceptionMessage);
                        }
                    }
                }
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            var isValid = false;

            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var usr = GetUserByUsername(username);
                        if (usr == null)
                            return false;
                        if (usr.IsLockedOut)
                            return false;

                        if (CheckPassword(password, usr.Password))
                        {
                            if (usr.IsApproved)
                            {
                                isValid = true;
                                usr.LastLoginDate = DateTime.Now;
                                session.Update(usr);
                                transaction.Commit();
                            }
                        }
                        else
                            UpdateFailureCount(username, "password");
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(e, "ValidateUser");
                            throw new ProviderException(exceptionMessage);
                        }
                        throw e;
                    }
                }
            }
            return isValid;
        }

        public override bool UnlockUser(string userName)
        {
            var unlocked = false;


            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {

                    try
                    {
                        var usr = GetUserByUsername(userName);

                        if (usr != null)
                        {
                            usr.LastLockedOutDate = DateTime.Now;
                            usr.IsLockedOut = false;
                            session.Update(usr);
                            transaction.Commit();
                            unlocked = true;
                        }
                    }
                    catch (Exception e)
                    {
                        WriteToEventLog(e, "UnlockUser");
                        throw new ProviderException(exceptionMessage);
                    }
                }
            }

            return unlocked;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            return GetMembershipUserByKeyOrUser(true, string.Empty, providerUserKey, userIsOnline);
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            return GetMembershipUserByKeyOrUser(false, username, 0, userIsOnline);
        }

        public override string GetUserNameByEmail(string email)
        {
            Entities.Users usr = null;
            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {

                    try
                    {
                        usr = session.CreateCriteria(typeof(Entities.Users))
                                            .Add(NHibernate.Criterion.Restrictions.Eq("Email", email))
                                            .UniqueResult<Entities.Users>();
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "GetUserNameByEmail");
                        throw new ProviderException(exceptionMessage);
                    }
                }
            }
            return usr == null ? string.Empty : usr.Username; ;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            var rowsAffected = 0;

            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var usr = GetUserByUsername(username);
                        if (usr != null)
                        {
                            session.Delete(usr);
                            transaction.Commit();
                            rowsAffected = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "DeleteUser");
                        throw new ProviderException(exceptionMessage);
                    }
                }
            }

            return rowsAffected > 0;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;
            var counter = 0;
            var startIndex = pageSize * pageIndex;
            var endIndex = startIndex + pageSize - 1;

            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        totalRecords = (Int32)session.CreateCriteria(typeof(Entities.Users))
                                    .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                    .SetProjection(NHibernate.Criterion.Projections.Count("Id")).UniqueResult();

                        if (totalRecords <= 0) { return users; }

                        var allusers = GetUsers();
                        foreach (Users u in allusers.TakeWhile(u => counter < endIndex))
                        {
                            if (counter >= startIndex)
                            {
                                MembershipUser mu = GetMembershipUserFromUser(u);
                                users.Add(mu);
                            }
                            counter++;
                        }
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "GetAllUsers");
                        throw new ProviderException(exceptionMessage);
                    }
                }
            }
            return users;
        }

        public override int GetNumberOfUsersOnline()
        {
            var onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
            var compareTime = DateTime.Now.Subtract(onlineSpan);
            var numOnline = 0;

            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        numOnline = (Int32)session.CreateCriteria(typeof(Users))
                                           .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                           .Add(NHibernate.Criterion.Restrictions.Gt("LastActivityDate", compareTime))
                                           .SetProjection(NHibernate.Criterion.Projections.Count("Id")).UniqueResult();
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "GetNumberOfUsersOnline");
                        throw new ProviderException(exceptionMessage);
                    }
                }
            }

            return numOnline;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            var counter = 0;
            var startIndex = pageSize * pageIndex;
            var endIndex = startIndex + pageSize - 1;
            totalRecords = 0;

            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var allusers = GetUsersLikeUsername(usernameToMatch);
                        if (allusers == null)
                            return users;
                        if (allusers.Count > 0)
                            totalRecords = allusers.Count;
                        else
                            return users;

                        foreach (var u in allusers)
                        {
                            if (counter >= endIndex)
                                break;
                            if (counter >= startIndex)
                            {
                                var mu = GetMembershipUserFromUser(u);
                                users.Add(mu);
                            }
                            counter++;
                        }
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(e, "FindUsersByName");
                            throw new ProviderException(exceptionMessage);
                        }
                        throw e;
                    }
                }
            }

            return users;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            var counter = 0;
            var startIndex = pageSize * pageIndex;
            var endIndex = startIndex + pageSize - 1;
            totalRecords = 0;

            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var allusers = GetUsersLikeEmail(emailToMatch);
                        if (allusers == null)
                            return users;
                        if (allusers.Count > 0)
                            totalRecords = allusers.Count;
                        else
                            return users;

                        foreach (var u in allusers)
                        {
                            if (counter >= endIndex)
                                break;
                            if (counter >= startIndex)
                            {
                                var mu = GetMembershipUserFromUser(u);
                                users.Add(mu);
                            }
                            counter++;
                        }
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(e, "FindUsersByEmail");
                            throw new ProviderException(exceptionMessage);
                        }
                        throw e;
                    }
                }
            }
            return users;
        }
        #endregion

        #region Helpers
        // A Function to retrieve config values from the configuration file
        private string GetConfigValue(string configValue, string defaultValue)
        {
            return String.IsNullOrEmpty(configValue) ? defaultValue : configValue;
        }

        //Fn to create a Membership user from a Entities.Users class
        private MembershipUser GetMembershipUserFromUser(Entities.Users usr)
        {
            var u = new MembershipUser(Name,
                                                  usr.Username,
                                                  usr.Id,
                                                  usr.Email,
                                                  usr.PasswordQuestion,
                                                  usr.Comment,
                                                  usr.IsApproved,
                                                  usr.IsLockedOut,
                                                  usr.CreationDate,
                                                  usr.LastLoginDate,
                                                  usr.LastActivityDate,
                                                  usr.LastPasswordChangedDate,
                                                  usr.LastLockedOutDate);

            return u;
        }

        //Fn that performs the checks and updates associated with password failure tracking
        private void UpdateFailureCount(string username, string failureType)
        {
            var windowStart = new DateTime();
            var failureCount = 0;

            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var usr = GetUserByUsername(username);

                        if (usr != null)
                        {
                            if (failureType == "password")
                            {
                                failureCount = usr.FailedPasswordAttemptCount;
                                windowStart = usr.FailedPasswordAttemptWindowStart;
                            }

                            if (failureType == "passwordAnswer")
                            {
                                failureCount = usr.FailedPasswordAnswerAttemptCount;
                                windowStart = usr.FailedPasswordAnswerAttemptWindowStart;
                            }
                        }

                        var windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);

                        if (failureCount == 0 || DateTime.Now > windowEnd)
                        {
                            // First password failure or outside of PasswordAttemptWindow. 
                            // Start a new password failure count from 1 and a new window starting now.

                            if (failureType == "password")
                            {
                                usr.FailedPasswordAttemptCount = 1;
                                usr.FailedPasswordAttemptWindowStart = DateTime.Now; ;
                            }

                            if (failureType == "passwordAnswer")
                            {
                                usr.FailedPasswordAnswerAttemptCount = 1;
                                usr.FailedPasswordAnswerAttemptWindowStart = DateTime.Now; ;
                            }
                            session.Update(usr);
                            transaction.Commit();
                        }
                        else
                        {
                            if (failureCount++ >= MaxInvalidPasswordAttempts)
                            {
                                // Password attempts have exceeded the failure threshold. Lock out
                                // the user.
                                usr.IsLockedOut = true;
                                usr.LastLockedOutDate = DateTime.Now;
                                session.Update(usr);
                                transaction.Commit();
                            }
                            else
                            {
                                // Password attempts have not exceeded the failure threshold. Update
                                // the failure counts. Leave the window the same.

                                if (failureType == "password")
                                    usr.FailedPasswordAttemptCount = failureCount;

                                if (failureType == "passwordAnswer")
                                    usr.FailedPasswordAnswerAttemptCount = failureCount;

                                session.Update(usr);
                                transaction.Commit();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(e, "UpdateFailureCount");
                            throw new ProviderException("Unable to update failure count and window start." + exceptionMessage);
                        }
                        throw e;
                    }
                }
            }

        }

        //CheckPassword: Compares password values based on the MembershipPasswordFormat.
        private bool CheckPassword(string password, string dbpassword)
        {
            var pass1 = password;
            var pass2 = dbpassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = EncodePassword(password);
                    break;
            }

            return pass1 == pass2;
        }

        //EncodePassword:Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
        private string EncodePassword(string password)
        {
            var encodedPassword = password;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword =
                      Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    var hash = new HMACSHA1();
                    hash.Key = HexToByte(_machineKey.ValidationKey);
                    encodedPassword =
                      Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }
            return encodedPassword;
        }

        // UnEncodePassword :Decrypts or leaves the password clear based on the PasswordFormat.
        private string UnEncodePassword(string encodedPassword)
        {
            var password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password =
                      Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot unencode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return password;
        }

        //   Converts a hexadecimal string to a byte array. Used to convert encryption key values from the configuration.    
        private byte[] HexToByte(string hexString)
        {
            var returnBytes = new byte[hexString.Length / 2];
            for (var i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        // WriteToEventLog
        // A helper function that writes exception detail to the event log. Exceptions
        // are written to the event log as a security measure to avoid private database
        // details from being returned to the browser. If a method does not return a status
        // or boolean indicating the action succeeded or failed, a generic exception is also 
        // thrown by the caller.

        private void WriteToEventLog(Exception e, string action)
        {
            var log = new EventLog();
            log.Source = eventSource;
            log.Log = eventLog;

            var message = "An exception occurred communicating with the data source.\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + e.ToString();

            log.WriteEntry(message);
        }
        #endregion

        #region Private
        //single fn to get a membership user by key or username
        private MembershipUser GetMembershipUserByKeyOrUser(bool isKeySupplied, string username, object providerUserKey, bool userIsOnline)
        {
            MembershipUser u = null;

            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {

                    try
                    {
                        Users usr = null;
                        if (isKeySupplied)
                            usr = session.CreateCriteria(typeof(Users))
                                            .Add(NHibernate.Criterion.Restrictions.Eq("Id", (int)providerUserKey))
                                            .UniqueResult<Users>();

                        else
                            usr = session.CreateCriteria(typeof(Users))
                                            .Add(NHibernate.Criterion.Restrictions.Eq("Username", username))
                                            .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                            .UniqueResult<Users>();

                        if (usr != null)
                        {
                            u = GetMembershipUserFromUser(usr);

                            if (userIsOnline)
                            {
                                usr.LastActivityDate = DateTime.Now;
                                session.Update(usr);
                                transaction.Commit();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "GetUser(Object, Boolean)");
                        throw new ProviderException(exceptionMessage);
                    }
                }
            }
            return u;
        }

        private Users GetUserByUsername(string username)
        {
            Users usr = null;
            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {

                    try
                    {
                        usr = session.CreateCriteria(typeof(Users))
                                        .Add(NHibernate.Criterion.Restrictions.Eq("Username", username))
                                        .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                        .UniqueResult<Users>();


                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "UnlockUser");
                        throw new ProviderException(exceptionMessage);
                    }
                }
            }
            return usr;

        }

        private IList<Users> GetUsers()
        {
            IList<Users> usrs = null;
            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {

                    try
                    {
                        usrs = session.CreateCriteria(typeof(Users))
                                        .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                        .List<Users>();

                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "GetUsers");
                        throw new ProviderException(exceptionMessage);
                    }
                }
            }
            return usrs;

        }

        private IList<Users> GetUsersLikeUsername(string usernameToMatch)
        {
            IList<Users> usrs = null;
            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {

                    try
                    {
                        usrs = session.CreateCriteria(typeof(Users))
                                        .Add(NHibernate.Criterion.Restrictions.Like("Username", usernameToMatch))
                                        .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                        .List<Users>();

                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "GetUsersMatchByUsername");
                        throw new ProviderException(exceptionMessage);
                    }
                }
            }
            return usrs;

        }

        private IList<Users> GetUsersLikeEmail(string emailToMatch)
        {
            IList<Users> usrs = null;
            using (var session = SessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {

                    try
                    {
                        usrs = session.CreateCriteria(typeof(Users))
                                        .Add(NHibernate.Criterion.Restrictions.Like("Email", emailToMatch))
                                        .Add(NHibernate.Criterion.Restrictions.Eq("ApplicationName", this.ApplicationName))
                                        .List<Users>();

                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                            WriteToEventLog(e, "GetUsersMatchByEmail");
                        throw new ProviderException(exceptionMessage);
                    }
                }
            }
            return usrs;

        }
        #endregion
    }
}
