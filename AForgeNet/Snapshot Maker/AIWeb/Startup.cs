using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AIWeb.Startup))]
namespace AIWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
