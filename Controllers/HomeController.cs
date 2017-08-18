using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using whiskyshop.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using System.Globalization;

namespace whiskyshop.Controllers
{
    public class HomeController : Controller
    {
        ApplicationContext db = new ApplicationContext();

        public int Index1 = 0;
        public int Index2 = 0;
        public int Index3 = 0;

        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }


        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated && Session["BagCount"] == null)
            {
                string uid = User.Identity.GetUserId();
                Session["BagCount"] = db.Purchases.Where(p => p.status == 0).Where(p => p.UserId == uid).Count();
                decimal? su = db.Purchases.Where(p => p.UserId == uid).Where(p => p.status == 0).Sum(p => p.summa);
                Session["BagSum"] = "";
                if (su != null)
                {
                  Session["BagSum"] = string.Format("на сумму {0}", su);
                }
                
            }
            var cats = db.Categories;
            ViewBag.Cats = cats;
            return View();
        }

        public ActionResult Cat(int id, int page = 1, int soart = 0)
        {
            int pagesize = 10;
            ViewBag.IdParam = id;
            ViewBag.PageParam = page;
            ViewBag.Soart = soart;
            IQueryable<Product> prod = db.Products.Include(p => p.Category);
            prod = prod.Where(p => p.Categoryid == id);
            int totalcount = prod.Count();
            switch (soart)
            {
                case 1:
                  prod = prod.OrderByDescending(p => p.name);
                  break;
                case 2:
                  prod = prod.OrderBy(p => p.price);
                  break;
                case 3:
                  prod = prod.OrderByDescending(p => p.price);
                  break;
                default:
                  prod = prod.OrderBy(p => p.name);
                  break;
            }

            prod = prod.Skip((page - 1)*pagesize)
                   .Take(pagesize);
            PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pagesize, TotalItems = totalcount };
            ProductListByCat prdcat = new ProductListByCat { Products = prod.ToList(), PageInfo = pageInfo};
            var caa = db.Categories.Find(id);
            if (caa != null)
            {
                ViewBag.Title = caa.name;
            }
            return View(prdcat);
        }

        public ActionResult Prod(int id)
        {
            Product prod = db.Products.Find(id);
            if (prod != null)
            {
                return View(prod);
            }
            return RedirectToAction("Index");
            
        }

        [HttpPost]
        public ActionResult RandomPresent()
        {
            Random r = new Random();
            int c = db.Products.Count();
            int i1 = 0; int i2 = 0;  int i3 = 0;
            i1 = r.Next(c); i2 = r.Next(c); i3 = r.Next(c);
            Index1 = Index1 == 0 ? i1 : Index1;
            Index2 = Index2 == 0 ? i2 : Index2;
            Index3 = Index3 == 0 ? i3 : Index3;
            while (i1 == i2 || i1 == i3 || i1 == Index1 || i1 == Index2 || i1 == Index3) { i1 = r.Next(c); }
            while (i2 == i1 || i2 == i3 || i2 == Index1 || i2 == Index2 || i2 == Index3) { i2 = r.Next(c); }
            while (i3 == i1 || i3 == i2 || i3 == Index1 || i3 == Index2 || i3 == Index3) { i3 = r.Next(c); }
            List<Product> prod = new List<Product>();
            IEnumerable<Product> a = db.Products;
            prod.Add(a.ElementAt(i1));
            prod.Add(a.ElementAt(i2));
            prod.Add(a.ElementAt(i3));
            Index1 = i1; Index2 = i2; Index3 = i3;
            return PartialView(prod);
        }

        public FilePathResult GetFile()
        {
            string file_path = Server.MapPath("~/Content/demowhiskyprice.xls");
            string file_type = "application/octet-stream";
            string file_name = "demowhiskyprice.xls";
            return File(file_path, file_type, file_name);
        }

    }
}
