using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CIMS_V2.Startup))]
namespace CIMS_V2
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
