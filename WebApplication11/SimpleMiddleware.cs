using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Autofac;
using Autofac.Integration.Owin;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace WebApplication11
{
    public class SimpleMiddleware : OwinMiddleware
    {
        private static readonly PathString Path = new PathString("/test");

        public SimpleMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public async override Task Invoke(IOwinContext context)
        {
            if (!context.Request.Path.Equals(Path))
            {
                await Next.Invoke(context);
                return;
            }

            context.Response.OnSendingHeaders(o =>
            {
                //scope here should be already disposed
                var scopeOnHeaders = context.GetAutofacLifetimeScope();
                var serviceOnHeaders = scopeOnHeaders.Resolve<PluggableComponent>();
                context.Response.Headers.Add("Echo", new[] { serviceOnHeaders.Echo() });
            }, this);

            //scope here should be fine
            var scope = context.GetAutofacLifetimeScope();
            var service = scope.Resolve<PluggableComponent>();

            var responseData = new { Date = DateTime.Now, Echo = service.Echo() };

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(responseData));
        }

    }
}