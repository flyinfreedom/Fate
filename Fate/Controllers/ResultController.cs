using Fate.Helper;
using Fate.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Fate.Controllers
{
    public class ResultController : ApiController
    {
        private IFateService _fateService;

        public ResultController()
        {
            this._fateService = new FateService();
        }

        public IHttpActionResult Post()
        {
            string token = "temp";
            string fateType = "ST01";
            object param = new { };

            bool isPayed = CheckIsPayed(token);

            IResult result = this._fateService.GetFateResultCode(fateType, isPayed, param);
            return Ok(result);
        }


        private bool CheckIsPayed(string token)
        {
            throw new NotImplementedException();
        }
    }
}
