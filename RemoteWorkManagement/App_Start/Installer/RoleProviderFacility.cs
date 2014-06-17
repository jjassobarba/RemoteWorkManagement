using System.Web.Security;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;

namespace RemoteWorkManagement.App_Start.Installer
{
    public class RoleProviderFacility:AbstractFacility
    {
        /// <summary>
        /// The custom initialization for the Facility.
        /// </summary>
        /// <remarks>
        /// It must be overridden.
        /// </remarks>
        protected override void Init()
        {
            Kernel.Register(Component.For<RoleProvider>().Instance(Roles.Provider).LifestylePerWebRequest());
        }
    }
}