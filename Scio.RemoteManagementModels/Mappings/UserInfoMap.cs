using FluentNHibernate.Mapping;
using Scio.RemoteManagementModels.Entities;

namespace Scio.RemoteManagementModels.Mappings
{
    public class UserInfoMap : ClassMap<UserInfo>
    {
        public UserInfoMap()
        {
            Id(x => x.IdUserInfo).GeneratedBy.GuidComb();
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.Position);
            Map(x => x.ProjectLeader);
            Map(x => x.RemoteDays);
            Map(x => x.FlexTime);
            Map(x => x.Picture);
            References(x => x.IdMembership).Column("Id").LazyLoad();
        }
    }
}
