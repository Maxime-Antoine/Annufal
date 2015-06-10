using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(Annufal.Startup))]

namespace Annufal
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            AuthConfig.Config(app);

            WebApiConfig.Config(config);
            app.UseCors(CorsOptions.AllowAll); //Allow CORS for API

            MvcConfig.Config(); //MVC routes + scripts/styles bundling

            IoCConfig.Config(app, config);

            AutoMapperConfig.Config();

            app.UseWebApi(config);
        }
    }
}