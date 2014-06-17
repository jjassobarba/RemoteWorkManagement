using Castle.Windsor;
using RemoteWorkManagement.App_Start.Installer;

namespace RemoteWorkManagement.App_Start
{
    public static class WindsorContainerConfig
    {
        /// <summary>
        /// Configures the specified container.
        /// </summary>
        /// <param name="container">The container.</param>
        public static void Configure(IWindsorContainer container)
        {
            container.AddFacility<MembershipProviderFacility>();
            container.AddFacility<RoleProviderFacility>();

            container.Install(
                new PersistenceInstaller(),
                new ControllerInstaller());
        }
    }
    
}