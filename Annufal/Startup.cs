using Annufal.Authentication;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

[assembly: OwinStartup(typeof(Annufal.Startup))]

namespace Annufal
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            _ConfigureOAuth(app);

            _WebApiConfig(config);
            app.UseCors(CorsOptions.AllowAll); //Allow CORS for API

            _MvcConfig();

            _ConfigureIoC(app, config);

            app.UseWebApi(config);
        }

        #region Private methods

        private void _ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new SimpleAuthorizationServerProvider()
            };

            //Token generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }

        private void _WebApiConfig(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        private void _MvcConfig()
        {
            #region Scripts & Styles Bundles

            //js/css bundles
            var bundles = BundleTable.Bundles;

            bundles.Add(new StyleBundle("~/bundles/styles").Include(
                 "~/Content/bootstrap.css",
                 "~/Content/bootstrap-theme.css",
                 "~/Content/app/app.css"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/modernizr-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/angular.js",
                "~/Scripts/angular-route.js",
                "~/Scripts/app/app.js"));

            BundleTable.EnableOptimizations = true; //force optimizations regardless of web.config settings

            #endregion Scripts & Styles Bundles

            #region MVC Routes

            //MVC routes
            var routes = RouteTable.Routes;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            #endregion MVC Routes
        }

        private void _ConfigureIoC(IAppBuilder app, HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            //register controllers
            builder.RegisterControllers(typeof(HttpApplication).Assembly); //MVC
            builder.RegisterApiControllers(typeof(HttpApplication).Assembly); //WebAPI

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

        #endregion Private methods
    }
}