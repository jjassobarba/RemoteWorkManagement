using System;
using System.Collections.Generic;

namespace Scio.RemoteManagementModels.Entities
{
    public class UserInfo
    {
        public virtual Guid IdUserInfo { get; set; }
        public virtual Users IdMembership { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Position { get; set; }
        public virtual string ProjectLeader { get; set; }
        public virtual string RemoteDays { get; set; }
        public virtual string FlexTime { get; set; }
        public virtual byte[] Picture { get; set; }
        public virtual string OtherFlexTime { get; set; }
        public virtual bool ReceiveNotifications { get; set; }
        public virtual IList<Notifications> Notifications { get; set; }
        public virtual IList<Messages> Messages { get; set; }
        public virtual IList<CheckInOut> CheckInOut { get; set; }
    }
}
