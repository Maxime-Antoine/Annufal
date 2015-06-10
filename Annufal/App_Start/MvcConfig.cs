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
                "~/Scripts/app/app.js",
                "~/Scripts/app/app.routes.js",
                "~/Scripts/app/app.auth.js",
                "~/Scripts/app/app.components.login.js",
                "~/Scripts/app/app.conponents.menu.js"));

            BundleTable.EnableOptimizations = true; //force optimizations regardless of web.config settings
        }
    }
}