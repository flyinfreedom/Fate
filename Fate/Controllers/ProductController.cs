using Fate.Models;
using System.Linq;
using System.Web.Http;

namespace Fate.Controllers
{
    [RoutePrefix("api/product")]
    public class ProductController : ApiController
    {
        [Route("amount/{productId}")]
        // GET: Product
        public IHttpActionResult GetAmount(string productId)
        {
            using (var db = new FortuneTellingEntities())
            {
                var product = db.Product.FirstOrDefault(x => x.ProductId == productId);
                if (product == null)
                {
                    return BadRequest();
                }
                var result = new AmountResponse {
                    Amount = product.Amount
                };

                return Ok(result);
            }
        }
    }

    public class AmountResponse
    { 
        public int Amount { get; set; }
    }
}