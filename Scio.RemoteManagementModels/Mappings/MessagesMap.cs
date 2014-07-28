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
            Map(x => x.Subject);
            Map(x => x.Date);
            HasMany(x => x.Inbox).Table("Inbox").Cascade.All().LazyLoad();
            HasMany(x => x.Outbox).Table("Outbox").Cascade.All().LazyLoad();
        }
    }
}
