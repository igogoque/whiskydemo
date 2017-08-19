using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using whiskyshop.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.Owin.Security.DataProtection;


namespace whiskyshop.Controllers
{
    public class RegisterAPIController : ApiController
    {
        private ApplicationContext db = new ApplicationContext();

         private ApplicationUserManager UserManager
        {
            get
            {
                return System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

        }

         [HttpPost]
         [NotSupportedExceptionFilter]
         public void Post([FromBody] OutRegisterModel model) 
         { 
                var alred = UserManager.FindByEmail(model.Email);
                if (alred == null)
                {
                    ApplicationUser user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                    IdentityResult result = UserManager.Create(user, model.Password);
                    if (result.Succeeded)
                    {
                        var provider = new DpapiDataProtectionProvider("Sample");
                        UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                            provider.Create("EmailConfirmation"));
                        // генерируем токен для подтверждения регистрации
                        var code = UserManager.GenerateEmailConfirmationToken(user.Id);
                        // создаем ссылку для подтверждения
                        var callbackUrl = model.OutUrl + "confirmreg/" + user.Id;

                        db.SendMail(user.Email, "Подтверждение email в демо-магазине виски",
                            "Для завершения регистрации скопируйте регистрацонный код:<br><br>" + code + "<br><br> и вставьте его <a href=\""
                             + callbackUrl + "\">по данной ссылке</a>");

                    }
                    else
                    {
                        throw new NotSupportedException(result.Errors.ToString());
                    }
                }
                else throw new NotSupportedException("Пользователь с данным email уже зарегистрирован"); 
            }

         public string Options()
         {
             return null; // HTTP 200 response with empty body
         }

         public string Get()
         {
             return "";
         }

         
    }
}
