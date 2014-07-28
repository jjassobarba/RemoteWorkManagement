using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Scio.RemoteManagementModels.RepositoriesContracts;
using Scio.RemoteManagementModels.RepositoriesImplementations;

namespace Scio.RemoteManagementModels.Configuration
{
    public class RepositoriesInstaller: IWindsorInstaller
    {
        /// <summary>
        /// Performs the installation in the 
        /// <see cref="T:Castle.Windsor.IWindsorContainer" />.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="store">The configuration store.</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IUserInfoRepository>().ImplementedBy<UserInfoRepository>().LifestylePerWebRequest(),
                Component.For<IMessagesRepository>().ImplementedBy<MessagesRepository>().LifestylePerWebRequest(),
                Component.For<IInboxRepository>().ImplementedBy<InboxRepository>().LifestylePerWebRequest(),
                Component.For<IOutboxRepository>().ImplementedBy<OutboxRepository>().LifestylePerWebRequest(),
                Component.For<INotificationsRepository>().ImplementedBy<NotificationsRepository>().LifestylePerWebRequest());
        }
    }
}
