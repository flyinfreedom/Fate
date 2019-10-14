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

        public ActionResult Login()
        {
            var model = new LoginRequest();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginRequest request)
        {
            using (var db = new FortuneTellingEntities())
            {
                var user = db.FateAdmin.FirstOrDefault(u => u.LoginId == request.LoginId && u.Password == request.Password);
                var role = "Normal";

                if (request.LoginId.Equals("Admin", StringComparison.InvariantCultureIgnoreCase) && request.Password == "1qaz@WSX")
                {
                    user = new FateAdmin
                    {
                        LoginId = "Admin"
                    };
                    role = "Admin";
                }

                if (user == null)
                {
                    request.Message = "無效的帳號或密碼";
                    return View(request);
                }

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
    }
}