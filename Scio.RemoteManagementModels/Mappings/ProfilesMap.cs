using FluentNHibernate.Mapping;
using Scio.RemoteManagementModels.Entities;

namespace Scio.RemoteManagementModels.Mappings
{
    public class ProfilesMap : ClassMap<Profiles>
    {
        public ProfilesMap()
        {
            Id(x => x.Id);
            Map(x => x.UsersId);
            Map(x => x.ApplicationName);
            Map(x => x.BirthDate);
            Map(x => x.City);
            Map(x => x.Country);
            Map(x => x.FirstName);
            Map(x => x.Gender);
            Map(x => x.IsAnonymous);
            Map(x => x.Language);
            Map(x => x.LastActivityDate);
            Map(x => x.LastName);
            Map(x => x.LastUpdatedDate);
            Map(x => x.Occupation);
            Map(x => x.State);
            Map(x => x.Street);
            Map(x => x.Subscription);
            Map(x => x.Website);
            Map(x => x.Zip);
        }
    }
}
