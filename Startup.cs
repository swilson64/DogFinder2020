using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DogFinder.Startup))]
namespace DogFinder
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
