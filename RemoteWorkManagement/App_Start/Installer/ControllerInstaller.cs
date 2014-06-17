using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using RemoteWorkManagement.Controllers;

namespace RemoteWorkManagement.App_Start.Installer
{
    public class ControllerInstaller:IWindsorInstaller
    {
        /// <summary>
        /// Performs the installation in the 
        /// <see cref="T:Castle.Windsor.IWindsorContainer" />.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="store">The configuration store.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromAssembly(typeof (HomeController).Assembly)
                    .BasedOn(typeof (Controller))
                    .If(Component.IsInSameNamespaceAs<HomeController>(true))
                    .If(c => c.Name.EndsWith("Controller"))
                    .LifestyleTransient());

        }
    }
}