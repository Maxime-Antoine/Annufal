using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Annufal
{
    public class IocConfig
    {
        public static void Config()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(HttpApplication).Assembly); //Register MVC controllers
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()); //Register WepAPI controllers

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()) //Register ***Services
                   .Where(t => t.Name.EndsWith("Service"))
                   .AsImplementedInterfaces();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container)); //set MVC dependency resolver
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container); //set WebAPI dependency resolver
        }
    }
}