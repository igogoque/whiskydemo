using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using whiskyshop.Models;

namespace whiskyshop.Controllers
{
    public class DelBagAPIController : ApiController
    {
        public ApplicationContext db = new ApplicationContext();

        [Authorize]
        [HttpPost]
        public void PostDeleteFromBag([FromBody] PurchaseTransfer baggy)
        {
            Purchase abag = db.Purchases.Include(p => p.Product).Where(p => p.UserId == baggy.UserId).FirstOrDefault(p => p.id == baggy.Id);
            if (abag != null)
            {
                abag.Product.reserved = abag.Product.reserved - abag.qnty;
                db.Purchases.Remove(abag);
            }
            db.SaveChanges();
        }

        public string Options()
        {
            return null; // HTTP 200 response with empty body
        }


    }
}
