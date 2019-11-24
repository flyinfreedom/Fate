using Fate.Backoffice.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fate.Backoffice.Controllers
{
    public class PopularController : Controller
    {
        // GET: Popular
        public ActionResult Index()
        {
            var db = new FortuneTellingEntities();
            return View(db.PopularFate);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(PopularFate popular, HttpPostedFileBase Photo)
        {
            if (Photo.ContentLength > 0)
            {
                string type = Path.GetExtension(Photo.FileName).Substring(1).ToLower();
                switch (type)
                {
                    case "png":
                        popular.PicType = "image/png";
                        break;
                    case "jpg":
                        popular.PicType = "image/jpeg";
                        break;
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    Photo.InputStream.CopyTo(ms);
                    popular.Pic = ms.GetBuffer();
                }
            }

            using (var db = new FortuneTellingEntities())
            {
                db.PopularFate.Add(popular);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var db = new FortuneTellingEntities();
            return View(db.PopularFate.FirstOrDefault(x => x.PopularId == id));
        }

        [HttpPost]
        public ActionResult Edit(PopularFate instance, HttpPostedFileBase Photo)
        {
            var db = new FortuneTellingEntities();
            var data = db.PopularFate.FirstOrDefault(x => x.PopularId == instance.PopularId);
            if (Photo.ContentLength > 0)
            {
                string type = Path.GetExtension(Photo.FileName).Substring(1).ToLower();
                switch (type)
                {
                    case "png":
                        data.PicType = "image/png";
                        break;
                    case "jpg":
                        data.PicType = "image/jpeg";
                        break;
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    Photo.InputStream.CopyTo(ms);
                    data.Pic = ms.GetBuffer();
                }
            }
            data.Description = instance.Description;
            data.Url = instance.Url;
            data.Sort = instance.Sort;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (var db = new FortuneTellingEntities())
            {
                var data = db.PopularFate.FirstOrDefault(x => x.PopularId == id);
                db.PopularFate.Remove(data);
                db.SaveChanges();
            }
            return Json(new { success = true });
        }
    }
}