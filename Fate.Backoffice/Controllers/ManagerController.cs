using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Fate.Backoffice.Helper;
using Fate.Backoffice.Models;

namespace Fate.Backoffice.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class ManagerController : Controller
    {
        // GET: Manager
        public ActionResult Index()
        {
            var db = new FortuneTellingEntities();
            return View(db.FateAdmin.Where(x => x.LoginId != "Admin"));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FateAdmin instance)
        {
            if (instance.LoginId.ToUpper() == "ADMIN") {
                return View(instance);
            }

            instance.Password = SHA256Helper.Encoding("0000");
            var db = new FortuneTellingEntities();
            db.FateAdmin.Add(instance);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {
            var db = new FortuneTellingEntities();
            return View(db.FateAdmin.FirstOrDefault(x => x.LoginId == id));
        }

        [HttpPost]
        public ActionResult Delete(FateAdmin fateAdmin)
        {
            var db = new FortuneTellingEntities();
            var item = db.FateAdmin.FirstOrDefault(x => x.LoginId == fateAdmin.LoginId);
            db.FateAdmin.Remove(item);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}