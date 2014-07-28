using FluentNHibernate.Mapping;
using Scio.RemoteManagementModels.Entities;

namespace Scio.RemoteManagementModels.Mappings
{
    public class OutboxMap: ClassMap<Outbox>
    {
        public OutboxMap()
        {
            Id(x => x.IdOutbox).GeneratedBy.GuidComb();
            Map(x => x.IsForwarded);
            References(x => x.IdMessage).Column("IdMessage").Cascade.All().LazyLoad();
            References(x => x.IdUserInfo).Column("IdUserInfo").Cascade.All().LazyLoad();
        }
    }
}
