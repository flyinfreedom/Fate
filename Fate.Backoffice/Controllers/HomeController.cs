using Fate.Backoffice.Helper;
using Fate.Backoffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Fate.Backoffice.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            var db = new FortuneTellingEntities();
            var user = db.FateAdmin.FirstOrDefault(x => x.LoginId == User.Identity.Name);
            return View(user);
        }

        [HttpPost]
        public ActionResult ChangePassword(FateAdmin instance)
        {
            var db = new FortuneTellingEntities();
            var user = db.FateAdmin.FirstOrDefault(x => x.LoginId == User.Identity.Name);
            user.Password = SHA256Helper.GetHashSha256(instance.Password);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult Login(string ReturnUrl)
        {
            var model = new LoginRequest();
            model.ReturnUrl = ReturnUrl;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginRequest request)
        {
            using (var db = new FortuneTellingEntities())
            {
                var hashPassword = SHA256Helper.GetHashSha256(request.Password);
                var user = db.FateAdmin.FirstOrDefault(u => u.LoginId == request.LoginId && u.Password == hashPassword);             

                if (user == null)
                {
                    request.Message = "無效的帳號或密碼";
                    return View(request);
                }
                var role = (user.LoginId.ToUpper() == "ADMIN" || user.LoginId.ToUpper() == "EDEN") ? "Admin" : "Normal";
                var ticket = new FormsAuthenticationTicket(
                            version: 1,
                            name: user.LoginId, //可以放使用者Id
                            issueDate: DateTime.UtcNow,//現在UTC時間
                            expiration: DateTime.UtcNow.AddMinutes(30),//Cookie有效時間=現在時間往後+30分鐘
                            isPersistent: true,// 是否要記住我 true or false
                            userData: role, //可以放使用者角色名稱
                            cookiePath: FormsAuthentication.FormsCookiePath);

                var encryptedTicket = FormsAuthentication.Encrypt(ticket); //把驗證的表單加密
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(cookie);

                return Redirect(request.ReturnUrl ?? "/");
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);

            //FormsAuthentication.RedirectToLoginPage(); 
            return RedirectToAction("Login", "Home", null);
        }
    }
}