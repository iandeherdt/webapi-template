using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Practices.Unity;
using Owin;
using Unity.WebApi;

[assembly: OwinStartup(typeof(WebApi.Startup))]

namespace WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            ConfigureWebApi(app, config);
            ConfigureDependencyInjection(config);
        }

        private void ConfigureWebApi(IAppBuilder app, HttpConfiguration config)
        {
            app.UseCors(CorsOptions.AllowAll);
            WebApiConfig.Register(config);
            app.Use(config);
        }

        private void ConfigureDependencyInjection(HttpConfiguration config)
        {
            var container = new UnityContainer();
            var factory = new Factory();
            factory.Configure(container);
            Container.InitializeWith(container);

            config.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}
