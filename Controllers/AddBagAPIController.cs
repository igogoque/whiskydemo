using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Security;
using whiskyshop.Models;

namespace whiskyshop.Controllers
{
    public class AddBagAPIController : ApiController
    {

        public ApplicationContext db = new ApplicationContext();

        [Authorize]
        [HttpPost]
        [NotSupportedExceptionFilter]
        public void Post([FromBody] PurchaseTransfer baggy)
        {
            Product prod = db.Products.Find(baggy.ProductId);
             if (baggy.qnty > prod.qnty - prod.reserved)
            {
                 throw new NotSupportedException("Заказанное количество больше имеющегося на данный момент");
            }
            prod.reserved = prod.reserved + baggy.qnty;
            if (prod.reserved > prod.qnty)
            {
                throw new NotSupportedException("Заказанное количество больше имеющегося на данный момент");
            }
            if (ModelState.IsValid)
            {
                baggy.summa = baggy.qnty * prod.price;
                Purchase tobag = new Purchase
                {
                    buydate = DateTime.Now,
                    summa = baggy.summa,
                    qnty = baggy.qnty,
                    status = 0,
                    Product = prod,
                    UserId = baggy.UserId
                };
                db.Purchases.Add(tobag);
                db.SaveChanges();
            }

        }

        private Exception NotSupportedException(string p)
        {
            throw new NotImplementedException(p);
        }


    }
}
