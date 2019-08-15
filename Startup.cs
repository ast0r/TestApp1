using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestApp1.Startup))]
namespace TestApp1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
