using FluentNHibernate.Mapping;
using Scio.RemoteManagementModels.Entities;

namespace Scio.RemoteManagementModels.Mappings
{
    public class NotificationsMap:ClassMap<Notifications>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsMap"/> class.
        /// </summary>
        public NotificationsMap()
        {
            Id(x => x.IdNotification).GeneratedBy.GuidComb();
            Map(x => x.OtherMails);
            Map(x => x.ProjectLeaderMail);
            Map(x => x.TeamMail);
            References(x => x.IdUserInfo).Column("IdUserInfo").LazyLoad();
        }
    }
}
