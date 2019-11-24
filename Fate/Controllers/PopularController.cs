using Fate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Fate.Controllers
{
    public class PopularController : ApiController
    {
        public IHttpActionResult GetPopular()
        {
            var db = new FortuneTellingEntities();
            var result = db.PopularFate
                .OrderByDescending(x => x.Sort).ToList();
                
            return Ok(result.Select(s =>
                    new
                    {
                        name = s.Description,
                        link = s.Url,
                        src = $"data:{s.PicType};base64, {Convert.ToBase64String(s.Pic)}"
                    }));
        }
    }
}
