using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Owin;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Annufal
{
    public class IoCConfig
    {
        public static void Config(IAppBuilder app, HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            //register controllers
            builder.RegisterControllers(typeof(HttpApplication).Assembly); //MVC
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()); //WebAPI

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()) //Register ***Services
                   .Where(t => t.Name.EndsWith("Service"))
                   .AsImplementedInterfaces();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container); //WebAPI
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container)); //MVC

            app.UseAutofacMiddleware(container); //Register before MVC / WebApi
            app.UseAutofacMvc();
            app.UseAutofacWebApi(config);
        }
    }
}