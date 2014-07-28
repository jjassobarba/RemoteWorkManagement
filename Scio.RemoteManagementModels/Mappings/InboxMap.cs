using FluentNHibernate.Mapping;
using Scio.RemoteManagementModels.Entities;

namespace Scio.RemoteManagementModels.Mappings
{
    public class InboxMap: ClassMap<Inbox>
    {
        public InboxMap()
        {
            Id(x => x.IdInbox).GeneratedBy.GuidComb();
            Map(x => x.IsForwarded);
            References(x => x.IdMessage).Column("IdMessage").Cascade.All().LazyLoad();
            References(x => x.IdUserInfo).Column("IdUserInfo").Cascade.All().LazyLoad();
        }
    }
}
