using System;

namespace Scio.RemoteManagementModels.Entities
{
    public class Notifications
    {
        public virtual Guid IdNotification { get; set; }
        public virtual UserInfo IdUserInfo { get; set; }
        public virtual string ProjectLeaderMail { get; set; }
        public virtual string SenseiMail { get; set; }
        public virtual string OtherMails { get; set; }
    }
}
