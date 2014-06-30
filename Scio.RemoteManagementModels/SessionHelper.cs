using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace Scio.RemoteManagementModels
{
    public static class SessionHelper
    {
        /// <summary>
        /// Creates the session factory.
        /// </summary>
        /// <param name="connstr">The connstr.</param>
        /// <returns></returns>
        public static ISessionFactory CreateSessionFactory(string connstr)
        {
            return Fluently.Configure()
                .ExposeConfiguration(cfg => cfg.Properties.Add("use_proxy_validator","false"))
                .Database(MsSqlConfiguration.MsSql2012.ConnectionString(connstr))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Membership.NHMembershipProvider>())
                .BuildSessionFactory();
        }
    }
}
