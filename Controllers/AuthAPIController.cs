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
using System.Web.Http.Cors;

namespace whiskyshop.Controllers
{
    public class AuthAPIController : ApiController
    {

        private ApplicationUserManager UserManager
        {
            get
            {
                return System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

        }

        [HttpGet]
        [NotSupportedExceptionFilter]
        public CheckLogin Get()
        {
            CheckLogin outuser = new CheckLogin { UserId = "", Email = "", userName = "", IsConfirmed = false };
            string usarid = User.Identity.GetUserId();
            ApplicationUser usar = UserManager.FindById(usarid);
            if (usar != null)
            {
                if (usar.EmailConfirmed)
                {
                    outuser.IsConfirmed = true;
                }
                outuser.UserId = usarid;
                outuser.userName = usar.UserName;
                outuser.Email = usar.Email;
            }
            return outuser;
        }

        public string Options()
        {
            return null; // HTTP 200 response with empty body
        }


    }
}
