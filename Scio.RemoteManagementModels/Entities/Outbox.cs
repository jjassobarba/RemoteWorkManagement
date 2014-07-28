using System;

namespace Scio.RemoteManagementModels.Entities
{
    public class Outbox
    {
        public virtual Guid IdOutbox { get; set; }
        public virtual Messages IdMessage { get; set; }
        public virtual UserInfo IdUserInfo { get; set; }
        public virtual bool IsForwarded { get; set; }
    }
}
