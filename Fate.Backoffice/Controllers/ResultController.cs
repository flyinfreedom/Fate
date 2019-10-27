using Fate.Backoffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fate.Backoffice.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ResultController : Controller
    {
        // GET: Result
        public ActionResult Index(string id)
        {
            using (var db = new FortuneTellingEntities())
            {
                var result = db.Result.ToList();
                if (!string.IsNullOrEmpty(id))
                {
                    result = db.Result.Where(x => x.Product == id).ToList();
                }
                ViewBag.Product = db.Product.ToList();
                return View(result);
            }
        }

        public ActionResult CreateOrUpdate(string Code, FormCollection collection)
        {
            using (var db = new FortuneTellingEntities())
            {
                var result = db.Result.FirstOrDefault(p => p.Code == Code);

                if (result == null)
                {
                    result = new Result();
                    TryUpdateModel(result, collection);
                    db.Result.Add(result);
                }
                else
                {
                    TryUpdateModel(result, collection);
                }

                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string DeleteId)
        {
            using (var db = new FortuneTellingEntities())
            {
                var result = db.Result.FirstOrDefault(p => p.Code == DeleteId);
                db.Result.Remove(result);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}