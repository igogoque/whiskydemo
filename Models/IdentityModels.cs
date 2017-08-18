using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Net.Mail;
using System.Web.Mvc;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace whiskyshop.Models
{

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store) : base(store) { }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            ApplicationContext db = context.Get<ApplicationContext>();
            ApplicationUserManager manager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            return manager;
        }
    }



    public class FormattedDbEntityValidationException : Exception
    {
        public FormattedDbEntityValidationException(DbEntityValidationException innerException) :
            base(null, innerException)
        {
        }

        public override string Message
        {
            get
            {
                var innerException = InnerException as DbEntityValidationException;
                if (innerException != null)
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine();
                    sb.AppendLine();
                    foreach (var eve in innerException.EntityValidationErrors)
                    {
                        sb.AppendLine(string.Format("- Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().FullName, eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendLine(string.Format("-- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                                ve.PropertyName,
                                eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                                ve.ErrorMessage));
                        }
                    }
                    sb.AppendLine();

                    return sb.ToString();
                }

                return base.Message;
            }
        }
    }


    public class Category
    {
        public int id { get; set; }
        public string name { get; set; }

        public ICollection<Product> Products { get; set; }

        public Category()
        {
            Products = new List<Product>();
        }
    }


    public class Product
    {
      public int id { get; set; }
      public string name { get; set; }
      public string pic { get; set; }
      public int qnty { get; set; }
      public int reserved { get; set; }
      public string description { get; set; }
      public decimal price { get; set; }
      public int Categoryid { get; set;}
      public Category Category { get; set; }
    }  

    public class ApplicationUser : IdentityUser
    {

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string AuthType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, AuthType);
            return userIdentity;
        }

        public virtual ICollection<Purchase> Purchases { get; set; }

        public ApplicationUser()
        { 
            Purchases = new List<Purchase>();
        }

    }

    public class Purchase
    {
        public int id { get; set; }
        public DateTime buydate { get; set; }
        [Display(Name = "Количество")]
        [Range(1, 2000, ErrorMessage = "Недопустимое количество")]
        public int qnty { get; set; }
        public decimal? summa { get; set; }
        public int status { get; set; }
        public int Productid { get; set;}
        public Product Product { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
    
    
    
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext() : base("WhiskyContext") { }


        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Purchase> Purchases { get; set; }

        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                var newException = new FormattedDbEntityValidationException(e);
                throw newException;
            }
        }


        public void SendMail(string aTo, string aSubj, string aBody)
        {

            // наш email с заголовком письма
            MailAddress from = new MailAddress("demowhisky@yandex.ru", aSubj);
            // кому отправляем
            MailAddress to = new MailAddress(aTo);
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = aSubj;
            // текст письма - включаем в него ссылку
            m.Body = aBody;
            m.IsBodyHtml = true;
            // адрес smtp-сервера, с которого мы и будем отправлять письмо
            SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.yandex.ru", 587);
            // логин и пароль
            smtp.Credentials = new System.Net.NetworkCredential("demowhisky@yandex.ru", "forever95");
            smtp.EnableSsl = true;
            // smtp.UseDefaultCredentials = false;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(m);

        }


    }



}