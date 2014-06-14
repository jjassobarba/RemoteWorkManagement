using FluentNHibernate.Mapping;
using Scio.RemoteManagementModels.Entities;

namespace Scio.RemoteManagementModels.Mappings
{
    public class RolesMap: ClassMap<Roles>
    {
        public RolesMap()
        {
            Id(x => x.Id);
            Map(x => x.RoleName);
            Map(x => x.ApplicationName);
            HasManyToMany(x => x.UsersInRole)
                .Cascade.All()
                .Inverse()
                .Table("UsersInRoles");
        }
    }
}
