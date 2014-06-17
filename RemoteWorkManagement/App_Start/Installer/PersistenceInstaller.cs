using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using RemoteWorkManagement.App_Start.Plumbing;

namespace RemoteWorkManagement.App_Start.Installer
{
    public class PersistenceInstaller:IWindsorInstaller
    {
        /// <summary>
        /// Performs the installation in the 
        /// <see cref="T:Castle.Windsor.IWindsorContainer" />.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="store">The configuration store.</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<PersistenceFacility>();
        }
    }
}