using System;

namespace Scio.RemoteManagementModels.Entities
{
    public class CheckInOut
    {
        public virtual Guid IdCheck { get; set; }
        public virtual UserInfo IdUserInfo { get; set; }
        public virtual DateTime CheckInDate { get; set; }
        public virtual DateTime CheckOutDate { get; set; }

    }
}
