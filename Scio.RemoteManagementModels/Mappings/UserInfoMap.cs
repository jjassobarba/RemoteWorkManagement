using FluentNHibernate.Mapping;
using Scio.RemoteManagementModels.Entities;

namespace Scio.RemoteManagementModels.Mappings
{
    public class UserInfoMap : ClassMap<UserInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoMap"/> class.
        /// </summary>
        public UserInfoMap()
        {
            Id(x => x.IdUserInfo).GeneratedBy.GuidComb();
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.Position);
            Map(x => x.IdProjectLeader);
            Map(x => x.IdSensei);
            Map(x => x.RemoteDays);
            Map(x => x.FlexTime);
            Map(x => x.Picture).Length(100000);
            Map(x => x.OtherFlexTime);
            Map(x => x.ReceiveNotifications);
            Map(x => x.IsTemporalPassword);
            References(x => x.IdMembership).Column("IdMembership").LazyLoad();
            HasMany(x => x.Notifications).Table("Notifications").Cascade.All().LazyLoad();
            HasMany(x => x.Inbox).Table("Inbox").Cascade.All().LazyLoad();
            HasMany(x => x.Outbox).Table("Outbox").Cascade.All().LazyLoad();
            HasMany(x => x.CheckInOut).Table("CheckInOut").Cascade.All().LazyLoad();
        }
    }
}
