using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Archivos.Startup))]
namespace Archivos
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
