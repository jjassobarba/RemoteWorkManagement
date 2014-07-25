using System;

namespace Scio.RemoteManagementModels.Entities
{
    public class Messages
    {
        public virtual Guid IdMessage { get; set; }
        public virtual UserInfo IdUserInfo { get; set; }
        public virtual Guid IdTo { get; set; }
        public virtual string Message { get; set; }
        public virtual bool IsRead { get; set; }
    }
}
