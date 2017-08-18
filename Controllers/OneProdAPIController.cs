using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using whiskyshop.Models;

namespace whiskyshop.Controllers
{
    public class OneProdAPIController : ApiController
    {
        ApplicationContext db = new ApplicationContext();

        [AllowAnonymous]
        [HttpGet]
        public IEnumerable<Product> Get(int id)
        {
            return db.Products.Where(p=>p.id == id);
        }

    }
}
