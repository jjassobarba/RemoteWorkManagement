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
        public virtual int IdProjectLeader { get; set; }
        public virtual int IdSensei { get; set; }
        public virtual string RemoteDays { get; set; }
        public virtual string FlexTime { get; set; }
        public virtual byte[] Picture { get; set; }
        public virtual string OtherFlexTime { get; set; }
        public virtual bool ReceiveNotifications { get; set; }
        public virtual IList<Notifications> Notifications { get; set; }
        public virtual IList<Inbox> Inbox { get; set; }
        public virtual IList<Outbox> Outbox { get; set; }
        public virtual IList<CheckInOut> CheckInOut { get; set; }
        public virtual bool IsTemporalPassword { get; set; }
    }
}
