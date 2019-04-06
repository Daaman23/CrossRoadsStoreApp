using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CrossRoadsStoreApp.Startup))]
namespace CrossRoadsStoreApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
