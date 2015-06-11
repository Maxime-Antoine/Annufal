using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Annufal
{
    public class MvcConfig
    {
        public static void Config()
        {
            //MVC routes
            var routes = RouteTable.Routes;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //js/css bundles
            var bundles = BundleTable.Bundles;

            bundles.Add(new StyleBundle("~/bundles/styles").Include(
                 "~/Content/css/vendor/bootstrap.css",
                 "~/Content/css/vendor/bootstrap-theme.css",
                 "~/Content/css/app.css"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/vendor/modernizr-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/vendor").Include(
                "~/Scripts/vendor/jquery-{version}.js",
                "~/Scripts/vendor/bootstrap.js",
                "~/Scripts/vendor/angular.js",
                "~/Scripts/vendor/angular-route.js"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/app/app.js",
                "~/Scripts/app/app.routes.js",
                "~/Scripts/app/app.auth.js",
                "~/Scripts/app/app.components.login.js",
                "~/Scripts/app/app.conponents.menu.js",
                "~/Scripts/app/app.components.profile.js"));

            BundleTable.EnableOptimizations = true; //force optimizations regardless of web.config settings
        }
    }
}