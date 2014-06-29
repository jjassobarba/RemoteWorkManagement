﻿using System;

namespace Scio.RemoteManagementModels.Entities
{
    public class UserInfo
    {
        public virtual Guid IdUserInfo { get; set; }
        public virtual int IdMembership { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Position { get; set; }
        public virtual string ProjectLeader { get; set; }
        public virtual string RemoteDays { get; set; }
        public virtual string FlexTime { get; set; }
        public virtual byte[] Picture { get; set; }
    }
}
