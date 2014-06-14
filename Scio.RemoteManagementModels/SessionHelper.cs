using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace Scio.RemoteManagementModels
{
    public static class SessionHelper
    {
        public static ISessionFactory CreateSessionFactory(string connstr)
        {
            return Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                    .ConnectionString(connstr))
                .Mappings(
                    m =>
                        m.FluentMappings.AddFromAssemblyOf<Membership.NHMembershipProvider>())
                .BuildSessionFactory();
        }
    }
}
