using FluentNHibernate.Mapping;
using Scio.RemoteManagementModels.Entities;

namespace Scio.RemoteManagementModels.Mappings
{
    public class CheckinoutMap : ClassMap<CheckInOut>
    {
        public CheckinoutMap()
        {
            Id(x => x.IdCheck).GeneratedBy.GuidComb();
            Map(x => x.CheckInDate);
            Map(x => x.CheckOutDate);
            References(x => x.IdUserInfo).Column("IdUserInfo").Cascade.All().LazyLoad();
        }
    }
}
