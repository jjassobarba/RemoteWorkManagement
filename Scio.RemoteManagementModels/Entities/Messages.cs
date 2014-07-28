using System;
using System.Collections.Generic;

namespace Scio.RemoteManagementModels.Entities
{
    public class Messages
    {
        public virtual Guid IdMessage { get; set; }
        public virtual UserInfo IdUserInfo { get; set; }
        public virtual Guid IdTo { get; set; }
        public virtual string Message { get; set; }
        public virtual bool IsRead { get; set; }
        public virtual string Subject { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual List<Inbox> Inbox { get; set; }
        public virtual List<Outbox> Outbox { get; set; }


    }
}
