using Fate.Backoffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fate.Backoffice.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            using (var db = new FortuneTellingEntities())
            {
                var products = db.Product.ToList();
                return View(products);
            }
        }

        [HttpPost]
        public ActionResult CreateOrUpdate(string ProductId, FormCollection collection)
        { 
            using(var db = new FortuneTellingEntities())
            {
                var product = db.Product.FirstOrDefault(p => p.ProductId == ProductId);

                if (product == null)
                {
                    product = new Product();
                    TryUpdateModel(product, collection);
                    db.Product.Add(product);
                }
                else
                {
                    TryUpdateModel(product, collection);
                }

                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(string DeleteId)
        {
            using (var db = new FortuneTellingEntities())
            {
                var product = db.Product.FirstOrDefault(p => p.ProductId == DeleteId);
                db.Product.Remove(product);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}