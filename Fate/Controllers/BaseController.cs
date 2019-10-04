using Fate.Helper;
using Fate.Models;
using Fate.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Fate.Controllers
{
    public class BaseController : Controller
    {
        protected FortuneTellingEntities db;

        public BaseController()
        {
            db = new FortuneTellingEntities();
        }

        protected void SetFadeCode(VMFateBase fateCode)
        {
            //HttpCookie fateCodeCookies = new HttpCookie(WebConfigVariable.FateTypeCookieName);
            //fateCodeCookies.Value = fateCode;
            //fateCodeCookies.Expires = DateTime.Now.AddMinutes(30);
            //Response.Cookies.Add(fateCodeCookies);
            fateCode.FateType = this.ControllerContext.RouteData.Values["action"].ToString(); ;
        }

        ~BaseController()
        {
            db.Dispose();
        }

        protected string ViewToString(string viewName, string controllerName, string areaName, object model = null)
        {
            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", controllerName);

            if (areaName != null)
            {
                routeData.Values.Add("Area", areaName);
                routeData.DataTokens["area"] = areaName;
            }

            ControllerContext controllerContext = new ControllerContext(HttpContext, routeData, this);

            return ViewToString(
                controllerContext,
                ViewEngines.Engines.FindView(controllerContext, viewName, null),
                model
            );
        }

        private string ViewToString(ControllerContext controllerContext, ViewEngineResult viewEngineResult, object model)
        {
            using (StringWriter writer = new StringWriter())
            {
                ViewContext viewContext = new ViewContext(
                    ControllerContext,
                    viewEngineResult.View,
                    new ViewDataDictionary(model),
                    new TempDataDictionary(),
                    writer
                );

                viewEngineResult.View.Render(viewContext, writer);

                return writer.ToString();
            }
        }
    }
}