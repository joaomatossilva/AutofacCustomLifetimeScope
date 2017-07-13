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

        private static IContainer Container { get; set; }

        protected void Application_Start()
        {
            Container = BuildContainer();
            var lifeTimeScopeProvider = new LifeTimeScopeProvider(Container);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container, lifeTimeScopeProvider));

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
            builder.RegisterType<PluggableComponent>().AsSelf().InstancePerRequest();

            //setup the middleware
            builder.RegisterType<SimpleMiddleware>().AsSelf();

            //Register the Controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            var container = builder.Build();
            return container;
        }

        private void ConfigureAutofac(HttpContext context)
        {
            //configure the container and start a lifetimescope (with the same tag as the AspNet request)
            var scope = Container.BeginLifetimeScope(Autofac.Core.Lifetime.MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            scope.CurrentScopeEnding += (sender, args) =>
            {
                int a = 0;
            };

            //Save the scope on the context and register it to be disposed on the end of the context
            context.Items[ScopeKey] = scope;
            context.DisposeOnPipelineCompleted(scope);
        }
    }

    public class LifeTimeScopeProvider : ILifetimeScopeProvider
    {
        public LifeTimeScopeProvider(ILifetimeScope container)
        {
            ApplicationContainer = container;
        }

        public ILifetimeScope GetLifetimeScope(Action<ContainerBuilder> configurationAction)
        {
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("Http Context Not Available");
            }
            return (ILifetimeScope)HttpContext.Current.Items[MvcApplication.ScopeKey];
        }

        public void EndLifetimeScope()
        {
        }

        public ILifetimeScope ApplicationContainer { get; }
    }
}
