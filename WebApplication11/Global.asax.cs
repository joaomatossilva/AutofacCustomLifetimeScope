using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;

namespace WebApplication11
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public const string ScopeKey = "CustomRequestScope";


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest()
        {
            var context = HttpContext.Current;
            ConfigureAutofac(context);
        }

        private IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            //setup the registry of the PluggableComponent
            builder.RegisterType<PluggableComponent>().AsSelf().InstancePerLifetimeScope();

            //setup the middleware
            builder.RegisterType<SimpleMiddleware>().AsSelf();

            //Register the Controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            var container = builder.Build();
            return container;
        }

        private void ConfigureAutofac(HttpContext context)
        {
            //configure the container and start a lifetimescope
            var container = BuildContainer();
            var scope = container.BeginLifetimeScope(ScopeKey);

            //Save the scope on the context and register it to be disposed on the end of the context
            context.Items[ScopeKey] = scope;
            context.DisposeOnPipelineCompleted(scope);

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
