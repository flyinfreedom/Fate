using Fate.Helper;
using Fate.ViewModels;
using IP2C.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http;

namespace Fate.Controllers
{
    public class PayController : ApiController
    {
        private IPCountryFinder _iPCountryFinder;
        private string ipMappingDataPath = WebConfigVariable.ipTablePath;

        public IHttpActionResult Get()
        {
            VMAPIResult result = new VMAPIResult();
            string path = System.Web.Hosting.HostingEnvironment.MapPath(ipMappingDataPath);
            this._iPCountryFinder = new IPCountryFinder(path);
            if (_iPCountryFinder.GetCountryCode(RequestClientIp(Request)) == "TW")
            {
                result.Success = true;
            }
            return Ok(result);
        }

        private string RequestClientIp(HttpRequestMessage request = null)
        {
            request = request ?? Request;


            
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["HTTP_X_FORWARDED_FOR"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            else if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return null;
            }
        }
    }
}
