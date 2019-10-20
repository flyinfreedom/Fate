using Fate.Backoffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fate.Backoffice.Controllers
{
    public class OrderController : Controller
    {
        public ActionResult Index()
        {
            var db = new FortuneTellingEntities();
            var result = db.Order;
            return View(result);
        }

        public ActionResult Detail(string id)
        {
            using (var db = new FortuneTellingEntities())
            {
                var detail = db.OrderDetail.FirstOrDefault(d => d.OrderId == id);
                ViewBag.Product = db.Product.FirstOrDefault(p => p.ProductId == detail.ProductId).Name;
                return View(detail);
            }
        }
    }
}