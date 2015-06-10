using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Annufal
{
    public class WebApiConfig
    {
        public static void Config(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            //convention based routing
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}