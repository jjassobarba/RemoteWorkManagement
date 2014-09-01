using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using RemoteWorkManagement.App_Start;
using RemoteWorkManagement.App_Start.Plumbing;

namespace RemoteWorkManagement
{
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// The _container
        /// </summary>
        private IWindsorContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcApplication"/> class.
        /// </summary>
        public MvcApplication()
        {
            _container = new WindsorContainer();
        }

        /// <summary>
        /// Application_s the start.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            WindsorContainerConfig.Configure(_container);
            
            ControllerBuilder.Current.SetControllerFactory(new CustomControllerFactory(_container));
        }

        /// <summary>
        /// Disposes the <see cref="T:System.Web.HttpApplication" /> instance.
        /// </summary>
        public override void Dispose()
        {
            _container.Dispose();
            base.Dispose();
        }
    }
}
