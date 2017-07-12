using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebApplication11.Startup))]
namespace WebApplication11
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseAutofacLifetimeScopeInjector(context =>
            {
                var httpContext = context.Get<HttpContextBase>(typeof(HttpContextBase).FullName);
                var scope = (ILifetimeScope) httpContext.Items[MvcApplication.ScopeKey];
                return scope;
            });

            app.Use<SimpleMiddleware>();
            app.UseAutofacMvc();
        }
    }
}