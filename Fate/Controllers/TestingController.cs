using Fate.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Fate.Controllers
{
    [RoutePrefix("api/testing")]
    public class TestingController : ApiController
    {
        [HttpPost]
        [Route("encrypt")]
        public string EncryptTesting(ASEString data)
        {            
            return AESHelper.Encrypt(data.testString, data.productId);
        }

        [HttpPost]
        [Route("decrypt")]
        public string DecryptTesting(ASEString data)
        {
            return AESHelper.Decrypt(data.testString, data.productId);
        }
    }

    public class ASEString
    { 
        public string testString { get; set; }
        public string productId { get; set; }
    }
}
