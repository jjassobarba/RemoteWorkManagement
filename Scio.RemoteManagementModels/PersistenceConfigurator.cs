using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;
using Scio.RemoteManagementModels.Membership;

namespace Scio.RemoteManagementModels
{
    public class PersistenceConfigurator
    {
        /// <summary>
        /// Builds the model persistence configuration.
        /// </summary>
        /// <param name="connectionStringKey">The connection string key.</param>
        /// <returns></returns>
        public virtual Configuration BuildModelPersistenceConfiguration(string connectionStringKey)
        {
            var sqlConfiguration = MsSqlConfiguration
                .MsSql2012
                .ConnectionString(x => x.FromConnectionStringWithKey(connectionStringKey))
                .ShowSql();

            var configuration = Fluently.Configure()
                .Database(sqlConfiguration)
                .Mappings(x => x.FluentMappings.AddFromAssembly(typeof (NHMembershipProvider).Assembly))
                .BuildConfiguration();

            return configuration;
        }
    }
}
