using System;

namespace Scio.RemoteManagementModels.Entities
{
    public class CheckInOut
    {
        public virtual Guid IdCheck { get; set; }
        public virtual UserInfo IdUserInfo { get; set; }
        public virtual DateTime CheckInDate { get; set; }
        public virtual DateTime CheckOutDate { get; set; }
        public virtual bool IsManualCheckIn { get; set; }
        public virtual bool IsManualCheckOut { get; set; }
        public virtual bool IsAuthorized { get; set; }
        public virtual string Comments { get; set; }
    }
}
