using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EPM.Extension.Web.Startup))]
namespace EPM.Extension.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
        //Test Commit
    }
}
