using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BHI_Asset_Portal_WebApp.Startup))]
namespace BHI_Asset_Portal_WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
