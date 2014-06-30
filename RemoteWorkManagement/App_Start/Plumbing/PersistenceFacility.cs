using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using NHibernate;
using Scio.RemoteManagementModels.Configuration;

namespace RemoteWorkManagement.App_Start.Plumbing
{
    public class PersistenceFacility : AbstractFacility
    {
        /// <summary>
        /// The custom initialization for the Facility.
        /// </summary>
        /// <remarks>
        /// It must be overridden.
        /// </remarks>
        protected override void Init()
        {
            var persistenceConfigurator = new PersistenceConfigurator();
            var persistenceConfiguration = persistenceConfigurator.BuildModelPersistenceConfiguration("NhConnecString");
            Kernel.Register(
                Component.For<ISessionFactory>().UsingFactoryMethod(persistenceConfiguration.BuildSessionFactory),
                Component.For<ISession>().UsingFactoryMethod(k => k.Resolve<ISessionFactory>().OpenSession()).LifestylePerWebRequest());
        }
    }
}