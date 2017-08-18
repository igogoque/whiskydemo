using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Cors;
using System.Web.Http;
using whiskyshop.Models;

[assembly: OwinStartup(typeof(whiskyshop.Startup))]
namespace whiskyshop
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<ApplicationContext>(ApplicationContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
            });

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            //var policy = new CorsPolicy()
            //{
            //    AllowAnyHeader = true,
            //    AllowAnyMethod = true,
            //    SupportsCredentials = true
            //};

            //policy.Origins.Add("http://localhost:3408"); //angwhisky path

            //app.UseCors(new CorsOptions
            //{
            //    PolicyProvider = new CorsPolicyProvider
            //    {
            //        PolicyResolver = context => Task.FromResult(policy)
            //    }
            //});

            
            var myProvider = new AppAuthorizationServerProvider();
            var OAuthOptions = new OAuthAuthorizationServerOptions
            {
                // устанавливает URL, по которому клиент будет получать токен
                TokenEndpointPath = new PathString("/Token"),
                // указывает на вышеопределенный провайдер авторизации
                Provider = myProvider,
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            };

            app.UseOAuthAuthorizationServer(OAuthOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            //HttpConfiguration config = new HttpConfiguration();
            //WebApiConfig.Register(config);

        }
    }


}