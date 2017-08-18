using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using whiskyshop.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace whiskyshop.Controllers
{
    public class ProductAPIController : ApiController
    {
        public ApplicationContext db = new ApplicationContext();

        public IEnumerable<Product> Get(int id = 0)
        {
            if (id != 0)
            {
                return db.Products./*Include(p => p.Category).*/Where(p => p.Categoryid == id).Where(p => p.qnty - p.reserved > 0).ToList();
            }
            else
            {
                return db.Products./*Include(p => p.Category).*/Where(p => p.qnty - p.reserved > 0).ToList();
            }

        }
    }
}
