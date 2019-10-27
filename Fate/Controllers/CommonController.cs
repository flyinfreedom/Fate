using Fate.Models;
using Fate.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Fate.Controllers
{
    [RoutePrefix("api")]
    public class CommonController : ApiController
    {
        [Route("condition/cndate/{year}")]
        public List<CNMonthResponse> GetCNDate([FromUri]int year)
        {
            using (var db = new FortuneTellingEntities())
            {

                return db.Taiwanlunisolar
                    .Where(x => x.CNYYYY == year)
                    .GroupBy(x => new { x.CNMM, x.IsLeap })
                    .Select(d => new CNMonthResponse
                    {
                        Month = d.Key.CNMM,
                        IsLeap = d.Key.IsLeap,
                        LastDay = d.Max(l => l.CNDD)
                    })
                    .OrderBy(x => x.Month)
                    .ThenBy(x => x.IsLeap)
                    .ToList();
            }
        }
    }
}
