using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;

namespace RemoteWorkManagement.App_Start.Plumbing
{
    public class CustomControllerFactory : DefaultControllerFactory
    {
        /// <summary>
        /// The container
        /// </summary>
        private readonly IWindsorContainer Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomControllerFactory"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public CustomControllerFactory(IWindsorContainer container)
        {
            Container = container;
        }

        /// <summary>
        /// Creates the specified controller by using the specified request context.
        /// </summary>
        /// <param name="requestContext">The context of the HTTP request, which includes the HTTP context and route data.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <returns>
        /// The controller.
        /// </returns>
        /// <exception cref="System.Web.HttpException">404</exception>
        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            var controllerType = GetControllerType(requestContext, controllerName);
            if (controllerType == null)
            {
                throw new HttpException(
                    404,
                    string.Format("The controller for path '{0}' could not be found.",
                                  requestContext.HttpContext.Request.Path));
            }
            return Container.Resolve(controllerType) as IController;
        }

        /// <summary>
        /// Releases the specified controller.
        /// </summary>
        /// <param name="controller">The controller to release.</param>
        public override void ReleaseController(IController controller)
        {
            Container.Release(controller);
        }
    }
}