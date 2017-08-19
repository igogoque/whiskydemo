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
    public class ForgotConfirmAPIController : ApiController
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
        public async void Post(ForgotPassConfirm masyaga)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(masyaga.userid);
            if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
            {
              throw new NotSupportedException("Пользователь не найден среди покупателей или email не подтвержден");
            }

            if (UserManager.UserTokenProvider == null)
            {
                var provider = new DpapiDataProtectionProvider("Sample");
                UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                  provider.Create("PasswordConfirmation"));
            }

            IdentityResult x = await UserManager.ResetPasswordAsync(masyaga.userid, masyaga.usertoken, masyaga.Password);
            if (!x.Succeeded)
            {
                throw new NotSupportedException(x.Errors.ToString());
            }
        }

        public string Options()
        {
            return null; // HTTP 200 response with empty body
        }


    }
}
