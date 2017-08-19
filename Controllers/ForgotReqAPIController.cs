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
using System.Threading.Tasks;

namespace whiskyshop.Controllers
{
    public class ForgotReqAPIController : ApiController
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
        public void Post(ForgotPassReq masyaga)
        {
            var user = UserManager.FindByEmail(masyaga.forgotemail);
            if (user == null || !(UserManager.IsEmailConfirmed(user.Id)))
            {
               throw new NotSupportedException("Пользователь с данным email не найден среди покупателей");
            }

            if (UserManager.UserTokenProvider == null)
            {
                var provider = new DpapiDataProtectionProvider("Sample");
                UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                  provider.Create("PasswordConfirmation"));
            }

            string code = UserManager.GeneratePasswordResetToken(user.Id);
            var callbackUrl = masyaga.outurl + "forgotconfirm/" + user.Id;

            db.SendMail(masyaga.forgotemail, "Сброс пароля",
               "Для сброса пароля скопируйте данный код:<br><br>" + code + "<br><br> и вставьте его <a href=\""
                             + callbackUrl + "\">по данной ссылке</a>");
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
