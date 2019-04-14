using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Atos.Startup))]
namespace Atos
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
