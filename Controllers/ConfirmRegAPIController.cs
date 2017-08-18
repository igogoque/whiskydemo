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
    public class ConfirmRegAPIController : ApiController
    {
        private ApplicationUserManager UserManager
        {
            get
            {
                return System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

        }

        [HttpPost]
        [NotSupportedExceptionFilter]
        public void Post([FromBody] OutRegConfirm model) 
        {
            ApplicationUser usar = UserManager.FindById(model.userid);
            if (usar != null) 
            {
                var provider = new DpapiDataProtectionProvider("Sample");
                UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                    provider.Create("EmailConfirmation"));
                UserManager.ConfirmEmail(model.userid, model.usertoken);
            } else
                throw new NotSupportedException("Пользователь не существует"); 
        }
    }
}
