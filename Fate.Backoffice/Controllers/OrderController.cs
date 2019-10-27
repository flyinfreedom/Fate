using Fate.Backoffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPaging;

namespace Fate.Backoffice.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private int _pageSize = 20;
        public ActionResult Index()
        {
            var db = new FortuneTellingEntities();
            var result = new OrderRequest();
            result.Orders = db.Order.OrderByDescending(x => x.Datetime).ToPagedList(0, _pageSize);
            return View(result);
        }

        [HttpPost]
        public ActionResult Index(OrderRequest instance)
        {
            var db = new FortuneTellingEntities();
            var query = db.Order.AsQueryable();
            if (!string.IsNullOrEmpty(instance.OrderId))
            {
                query = query.Where(x => x.OrderId == instance.OrderId);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(instance.Name))
                {
                    query = query.Where(x => x.Name.Contains(instance.Name));
                }
                if (!string.IsNullOrWhiteSpace(instance.Email))
                {
                    query = query.Where(x => x.Email.Contains(instance.Email));
                }
                if (!string.IsNullOrWhiteSpace(instance.ContactPhone))
                {
                    query = query.Where(x => x.ContactPhone.Contains(instance.ContactPhone));
                }
            }

            instance.Orders = query.OrderByDescending(x => x.Datetime).ToPagedList(instance.page > 0 ? instance.page - 1 : 0, _pageSize);
            return View(instance);
        }

        public ActionResult Detail(string id)
        {
            using (var db = new FortuneTellingEntities())
            {
                var detail = db.OrderDetail.FirstOrDefault(d => d.OrderId == id);
                if (detail != null)
                {
                    ViewBag.Product = db.Product.FirstOrDefault(p => p.ProductId == detail.ProductId).Name;
                }
                return View(detail);
            }
        }
    }
}