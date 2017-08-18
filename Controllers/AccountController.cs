using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using whiskyshop.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.Owin.Security.DataProtection;
using System.Net.Mail;
using System.Data.Entity;
using System.Globalization;

namespace whiskyshop.Controllers
{


    public class AccountController : Controller
    {
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

        }

        private IAuthenticationManager AuthenticationManager
        {
            get 
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ApplicationContext db = new ApplicationContext();
        public int OnceSkipUrlReffer = 0;

        /*public string Donedone()
        {
            ApplicationContext db = new ApplicationContext();
            var rolemanager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var role1 = new IdentityRole { Name = "admin" };
            var role2 = new IdentityRole { Name = "user" };
            rolemanager.Create(role1);
            rolemanager.Create(role2);
            var admin = new ApplicationUser { Email = "ikehurricane@mail.ru", UserName = "ikehurricane@mail.ru", EmailConfirmed = true };
            string password = "Forever95";
            var result = UserManager.Create(admin, password);
            if (result.Succeeded)
            {
                UserManager.AddToRole(admin.Id, role1.Name);
                UserManager.AddToRole(admin.Id, role2.Name);
            }

            db.SaveChanges();
            return "всё ОК бля";
        }*/



        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {

            if (ModelState.IsValid)
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
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code },
                                   protocol: Request.Url.Scheme);

                        db.SendMail(user.Email, "Подтверждение email в демо-магазине виски",
                            "Для завершения регистрации перейдите по ссылке:: <a href=\""
                                                                   + callbackUrl + "\">завершить регистрацию</a>");
                        return View("DisplayEmail");

                    }
                    else
                    {
                        foreach (string error in result.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                    }
                }
                else ModelState.AddModelError("", "Пользователь с данным email уже существует");
            }
            return View();
        }

        public ActionResult Login()
        {
            if (Session["OnceSkipUrlReffer"] == null && Request.UrlReferrer != null)
            {
                Session["UrlForLogin"] = Request.UrlReferrer.AbsoluteUri;
            }
            else
            {
                Session["UrlForLogin"] = null;
                Session["OnceSkipUrlReffer"] = null;
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || UserManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, model.Password) == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError("", "Неверный логин или пароль");
                }
                else
                {
                    if (!user.EmailConfirmed)
                    {
                        ModelState.AddModelError("", "Email не подтвержден, проверьте почту");
                    }
                    else
                    {

                        ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                        AuthenticationManager.SignOut();
                        AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, claim);
                        Session["BagCount"] = db.Purchases.Where(p => p.UserId == user.Id).Where(p => p.status == 0).Count();
                        Session["BagSum"] = string.Format("на сумму {0}", db.Purchases.Where(p => p.UserId == user.Id).Where(p => p.status == 0).Sum(p => p.summa));
                        if (Session["UrlForLogin"] == null)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return Redirect(Session["UrlForLogin"].ToString());
                        }
                    }
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                AuthenticationManager.SignOut();
                Session["Bagcount"] = null; ;
                Session["BagSum"] = null;
            }
            return RedirectToAction("Index","Home");
        }

        public ActionResult Cabinet(int? abay)
        {
            if (!User.Identity.IsAuthenticated)
            {
               return RedirectToAction("Login", "Account");
            }

            if (abay != null)
            {
                ViewBag.AfterBuy = 1;
            }
            ApplicationUser usar = db.Users.Find(User.Identity.GetUserId());
            if (usar == null) 
            {
                return new HttpNotFoundResult();
            }
            return View(usar);
        }

        //удаление юзера
        [HttpPost]
        public async Task<ActionResult> Cabinet(ApplicationUser model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            
            if (ModelState.IsValid)
            {
                var fordel = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                AuthenticationManager.SignOut();

                if (fordel != null)
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        await UserManager.DeleteAsync(fordel);
                        transaction.Commit();
                    }
                }
            }
            return RedirectToAction("Index","Home");
        }

        
        public async Task<ActionResult> ConfirmEmail(string userid, string code)
        {
            ApplicationUser usar = await UserManager.FindByIdAsync(userid);
            if (usar != null) 
            {
                var provider = new DpapiDataProtectionProvider("Sample");
                UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                    provider.Create("EmailConfirmation"));
                UserManager.ConfirmEmail(userid, code);
                return View();
            } else
                return new HttpNotFoundResult();
        }


        public async Task<ActionResult> AddToBag(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.ProdName = "";
            Product prod = await db.Products.FindAsync(id);
            if (prod != null)
            {
                Purchase tobag = new Purchase
                {
                    buydate = DateTime.Now,
                    summa = 0,
                    qnty = 1,
                    status = 0,
                    Product = prod,
                    UserId = User.Identity.GetUserId()
                };
                tobag.summa = prod.price;
                ViewBag.ProdName = prod.name;
                return View(tobag);
            }
        return View();
        }


        [HttpPost]
        public async Task<ActionResult> AddToBag(Purchase model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            model.Product = await db.Products.FindAsync(model.Product.id);
            if (model.qnty >= model.Product.qnty - model.Product.reserved)
            {
                ModelState.AddModelError("qnty", "Заказанное количество больше имеющегося на данный момент");
            }
            model.Product.reserved = model.Product.reserved + model.qnty;
            if (model.Product.reserved > model.Product.qnty)
            {
                ModelState.AddModelError("qnty", "Заказанное количество больше имеющегося на данный момент");
            }
            if (ModelState.IsValid)
            {
                model.summa = model.qnty * model.Product.price;
                db.Purchases.Add(model);
                if (Session["Bagcount"] == null)
                { 
                    Session["Bagcount"] = 0;
                    Session["BagSum"] = "";
                }
                db.SaveChanges();
                Session["BagCount"] = db.Purchases.Where(p => p.UserId == model.UserId).Where(p => p.status == 0).Count();
                Session["BagSum"] =  string.Format("на сумму {0}", db.Purchases.Where(p => p.UserId == model.UserId).Where(p => p.status == 0).Sum(p => p.summa));
                return RedirectToAction("Cat", "Home", new { id = model.Product.Categoryid });
            }
            else 
            {
                return View();
            }
                
        }

        public async Task<ActionResult> Bag(int astatus = 0, int page = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            string uid = User.Identity.GetUserId();
            IQueryable<Purchase> purch = db.Purchases.Include(p => p.Product).Where(p => p.UserId == uid).Where(p => p.status == astatus);
            int pagesize = 10;
            int totalcount = purch.Count();
            Session["BagRoll"] = purch.Sum(p => p.summa);
            purch = purch.OrderBy(p => p.buydate)
                   .Skip((page - 1) * pagesize)
                   .Take(pagesize);
            PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pagesize, TotalItems = totalcount };
            PurchasePageList purchlist = new PurchasePageList { Purchases = purch.ToList(), PageInfo = pageInfo };
            ViewBag.Status = astatus;
            return View(purchlist);
        }


        public async Task<ActionResult> DeleteFromBag(int id, int status = 0)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            string uid = User.Identity.GetUserId();
            Purchase abag = await db.Purchases.Include(p => p.Product).FirstOrDefaultAsync(p => p.id == id);
            if (abag != null)
            {
                abag.Product.reserved = abag.Product.reserved - abag.qnty;
                db.Purchases.Remove(abag);
            }
            db.SaveChanges();
            if (status == 0)
            {
                Session["BagCount"] = db.Purchases.Where(p => p.UserId == uid).Where(p => p.status == status).Count();
                Session["BagSum"] = string.Format("на сумму {0}", db.Purchases.Where(p => p.UserId == uid).Where(p => p.status == status).Sum(p => p.summa));
            }
            Session["BagRoll"] = db.Purchases.Where(p => p.UserId == uid).Where(p => p.status == status).Sum(p => p.summa);
            return RedirectToAction("Bag", "Account", new {astatus = status, page = 1});
        }


        public async Task<ActionResult> Buy()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            string uid = User.Identity.GetUserId();
            ApplicationUser usar = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            string MailBodyStr = string.Format("Уважаемый {0}!<br>Ваш заказ в нашем демо-магазине принят! Вы заказали: <br><ul>", usar.UserName);
            IQueryable<Purchase> purch = db.Purchases.Include(p => p.Product).Where(p => p.UserId == uid).Where(p => p.status == 0);
            foreach (Purchase p in purch)
            {
                p.status = 1; //оформлен заказ
                MailBodyStr += string.Format("<li>{0} (количество: {1} сумма: {2})</li>", p.Product.name, p.qnty, p.summa);
                db.Entry(p).State = EntityState.Modified;
            }
            MailBodyStr += "</ul>";
            db.SaveChanges();
            Session["Bagcount"] = 0;
            Session["BagSum"] = "";
            Session["BagRoll"] = 0;
            db.SendMail(usar.Email, "Ваш заказ принят", MailBodyStr);
            db.SendMail("ikehurricane@mail.ru", "Демовиски заказали", MailBodyStr);
            return RedirectToAction("Cabinet", new { abay = 1});

        }


        public async Task<ActionResult> ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    ModelState.AddModelError("", "Пользователь с данным email не найден среди покупателей");
                    return View("ForgotPassword");
                }

                if (UserManager.UserTokenProvider == null)
                {
                    var provider = new DpapiDataProtectionProvider("Sample");
                    UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                      provider.Create("PasswordConfirmation"));
                }

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account",
                    new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
         
                db.SendMail(model.Email, "Сброс пароля",
                   "Для сброса пароля, перейдите по ссылке <a href=\"" + callbackUrl + "\">сбросить</a>");
                Session["WasPass"] = 1;
                return View("DisplayEmail");
            }
            return View(model);
        }


        public async Task<ActionResult> ResetPassword(string UserId, string code)
        {
            ResetPassModel repass = new ResetPassModel();
            repass.UserId = UserId;
            repass.Token = code;
            return View(repass);
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPassModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindByIdAsync(model.UserId);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    Session["WasPass"] = 3;
                    return View("DisplayEmail");
                }

                if (UserManager.UserTokenProvider == null)
                {
                    var provider = new DpapiDataProtectionProvider("Sample");
                    UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                      provider.Create("PasswordConfirmation"));
                }

                IdentityResult x = await UserManager.ResetPasswordAsync(model.UserId, model.Token, model.NewPass);
                if (x.Succeeded)
                {
                    Session["WasPass"] = 2;
                    Session["OnceSkipUrlReffer"] = 1;
                    return View("DisplayEmail");
                }
                else 
                {
                    foreach (string item in x.Errors)
                    {
                        ModelState.AddModelError("", item);
                    }
                }
            }
            return View(model);
        }

        [Authorize(Roles="admin")]
        public async Task<ActionResult> Admin()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [Authorize(Roles = "admin")]
        public async Task<ActionResult> ClearBase()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            IQueryable<Purchase> lp = db.Purchases.Include(p => p.Product);
            foreach (Purchase item in lp)
            {
                if (item.status == 2)
                {
                    item.Product.qnty = item.Product.qnty + item.qnty;
                }
                else
                {
                    item.Product.reserved = item.Product.reserved - item.qnty < 0 ? 0 : item.Product.reserved - item.qnty;
                }
                db.Entry(item.Product).State = EntityState.Modified;
                db.Purchases.Remove(item);
            }
            db.SaveChanges();
            Session["Bagcount"] = 0;
            Session["AState"] = "База очищена";
            return View("Admin");
        }

        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DelUser()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            IQueryable<ApplicationUser> usars = db.Users;
            usars = usars.Where(p => p.Email != "ikehurricane@mail.ru");
            return View(usars);
        }


        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DelUserImme(string AEmail)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var fordel = await UserManager.FindByEmailAsync(AEmail);
            if (fordel != null)
            {

                IQueryable<Purchase> lp = db.Purchases.Where(p => p.UserId == fordel.Id).Include(p => p.Product);
                foreach (Purchase item in lp)
                {
                    if (item.status == 2)
                    {
                        item.Product.qnty = item.Product.qnty + item.qnty;
                    }
                    else
                    {
                        item.Product.reserved = item.Product.reserved - item.qnty < 0 ? 0 : item.Product.reserved - item.qnty;
                    }
                    db.Entry(item.Product).State = EntityState.Modified;
                    db.Purchases.Remove(item);
                }
                db.SaveChanges();
                
                using (var transaction = db.Database.BeginTransaction())
                {
                    UserManager.Delete(fordel);
                    transaction.Commit();
                }
            }
            Session["AState"] = string.Format("Юзер {0} удален", AEmail);
            return View("Admin");

        }

    }
}
