using FluentNHibernate.Mapping;
using Scio.RemoteManagementModels.Entities;

namespace Scio.RemoteManagementModels.Mappings
{
    public class MessagesMap : ClassMap<Messages>
    {
        public MessagesMap()
        {
            Id(x => x.IdMessage).GeneratedBy.GuidComb();
            Map(x => x.IdTo);
            Map(x => x.Message);
            Map(x => x.IsRead);
            References(x => x.IdUserInfo).Column("IdUserInfo").Cascade.All().LazyLoad();
        }
    }
}
