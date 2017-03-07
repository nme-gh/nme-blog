using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(nme_blog.Startup))]
namespace nme_blog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
