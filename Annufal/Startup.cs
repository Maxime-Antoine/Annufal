using Annufal.Authentication;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Configuration;
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
            //create auth classes in owin context
            app.CreatePerOwinContext(AuthDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            //JWT token generation
            OAuthAuthorizationServerOptions oAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true, //to change in prod
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new CustomOAuthProvider(),
                AccessTokenFormat = new CustomJWTFormatProvider("http://localhost:60481/")
            };
            app.UseOAuthAuthorizationServer(oAuthServerOptions);

            //JWT token comsuption
            var issuer = "http://localhost:60481/";
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

            //Api controllers with [Authorize] filter will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                    }
                });
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