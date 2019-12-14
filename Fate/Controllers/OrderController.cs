using Fate.Helper;
using Fate.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Fate.Controllers
{
    [RoutePrefix("api/order")]
    public class OrderController : ApiController
    {
        public IHttpActionResult GetOrder(string uid)
        {
            string urlDeconde = HttpUtility.HtmlDecode(uid);
            string uidDecrypt = AESHelper.Decrypt(urlDeconde, string.Empty);

            using (var db = new FortuneTellingEntities())
            {
                var orders = db.Order
                    .Where(x => x.Uid == uidDecrypt || x.ContactPhone == uidDecrypt)
                    .OrderBy(x => x.Datetime)
                    .ToList();
                var orderDetailJson = orders.SelectMany(x => x.OrderDetail)
                    .Select(x => new { 
                        x.OrderId,
                        x.BirthDay,
                        x.BirthHour,
                        x.DateType,
                        x.Gender
                    })
                    .ToDictionary(x => x.OrderId, x => JsonConvert.SerializeObject(x));
                var orderDetail = orders.SelectMany(x => x.OrderDetail).ToDictionary(x => x.OrderId, x => x);
                var products = db.Product.ToDictionary(x => x.ProductId, x => x);

                return Ok(orders.Select(x => new OrderResponse {
                    OrderId = x.OrderId,
                    ProductId = orderDetail[x.OrderId].ProductId,
                    ProductName = products[orderDetail[x.OrderId].ProductId].Name,
                    Name = x.Name,
                    Condition = orderDetailJson[x.OrderId]
                }).ToList());
            }
        }

        [Route("ziwei")]
        [HttpGet]
        public IHttpActionResult RegetZiweiResult(string condition)
        {
            string conditionDeconde = HttpUtility.HtmlDecode(condition);
            string conditionDecrypt = AESHelper.Decrypt(conditionDeconde, string.Empty);
            ZiweiCondition data = JsonConvert.DeserializeObject<ZiweiCondition>(conditionDecrypt);

            using (var db = new FortuneTellingEntities())
            {
                string id = db.OrderDetail
                    .Where(x => (x.Order.Uid.Contains(data.uid) || x.Order.ContactPhone.Contains(data.uid))
                    && x.DateType == data.datetype
                    && x.BirthDay == data.birthday
                    && x.ProductId.Contains("Ziwei")
                    && x.Order.IsPayed).FirstOrDefault()?.OrderId;

                if (string.IsNullOrEmpty(id))
                {
                    return Ok(new { success = false, orderId = string.Empty, message = "no data found" });
                }
                return Ok(new { success = true, orderId = id, message = "success" });
            }
        }
    }

    public class ZiweiCondition
    { 
        public string uid { get; set; }
        public string birthday { get; set; }
        public int? datetype { get; set; }
    }

    public class OrderResponse
    { 
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string Name { get; set; }
        public string Condition { get; set; }
    }
}
