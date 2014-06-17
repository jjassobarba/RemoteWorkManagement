using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RemoteWorkManagement.Startup))]
namespace RemoteWorkManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
        }
    }
}
