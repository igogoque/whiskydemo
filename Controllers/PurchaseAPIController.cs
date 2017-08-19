using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Http;
using whiskyshop.Models;
using Microsoft.AspNet.Identity;



namespace whiskyshop.Controllers
{
    public class PurchaseAPIController : ApiController
    {
        public ApplicationContext db = new ApplicationContext();

        [Authorize]
        public IEnumerable<PurchaseTransfer> Get(string id)
        {
            List<PurchaseTransfer> outpur = new List<PurchaseTransfer>();
            IQueryable<Purchase> Purchs = db.Purchases.Include(p => p.Product).Where(p => p.UserId == id).Where(p => p.status == 0);
            foreach (Purchase item in Purchs)
            {
                outpur.Add(new PurchaseTransfer
                {
                    Id = item.id,
                    UserId = item.UserId,
                    ProductId = item.Productid,
                    ProductName = item.Product.name,
                    qnty = item.qnty,
                    summa = item.summa
                });
            }
            return outpur;
        }

        [Authorize]
        [HttpPost]
        public void Post([FromBody] UserInfo uinfo)
        {
            string MailBodyStr = string.Format("Уважаемый {0}!<br>Ваш заказ в нашем демо-магазине принят! Вы заказали: <br><ul>", uinfo.userName);
            IQueryable<Purchase> purch = db.Purchases.Include(p => p.Product).Where(p => p.UserId == uinfo.UserId).Where(p => p.status == 0);
            foreach (Purchase p in purch)
            {
                p.status = 1; //оформлен заказ
                MailBodyStr += string.Format("<li>{0} (количество: {1} сумма: {2})</li>", p.Product.name, p.qnty, p.summa);
                db.Entry(p).State = EntityState.Modified;
            }
            MailBodyStr += "</ul>";
            db.SaveChanges();
            db.SendMail(uinfo.Email, "Ваш заказ принят", MailBodyStr);
            db.SendMail("ikehurricane@mail.ru", "Демовиски заказали", MailBodyStr);

        }

        public string Options()
        {
            return null; // HTTP 200 response with empty body
        }


    }
}
