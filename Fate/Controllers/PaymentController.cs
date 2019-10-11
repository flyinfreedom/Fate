using Fate.Helper;
using Fate.Models;
using Fate.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Fate.Controllers
{
    [RoutePrefix("api/payment")]
    public class PaymentController : ApiController
    {
        [Route("getTxId")]
        [HttpPost]
        public GetTxIdResponse GetTxId(GetTxIdRequest request)
        {
            string email = request.uid.Contains("@") ? request.uid : string.Empty;
            string orderId = CreateOrderId();

            var requestModel = new GetTxIdRequestModel();
            requestModel.amount = 1;
            requestModel.uid = request.uid;
            requestModel.userIp = GetClientIp(Request);
            requestModel.orderId = orderId;
            requestModel.gameUrl = GetGameUrl("", orderId); 

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(requestModel.GetFullUrl());
            httpRequest.Method = "POST";

            string responseStr = string.Empty;
            var responseObj = new GetTxIdResponseModel();

            using (WebResponse response = httpRequest.GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseStr = sr.ReadToEnd();
                    responseObj = JsonConvert.DeserializeObject<GetTxIdResponseModel>(responseStr);
                }//end using  
            }

            string paymentDataStr = JsonConvert.SerializeObject(new { requestModel.amount, responseObj.orderId });
            string paymentData = AESHelper.Encrypt(paymentDataStr);


            using (var db = new FortuneTellingEntities())
            {
                var condition = JsonConvert.DeserializeObject<ConditionModel>(request.condition);
                var order = new Order
                {
                    OrderId = orderId,
                    Datetime = DateTime.Now,
                    Email = email,
                    ContactPhone = string.IsNullOrEmpty(email) ? request.uid : string.Empty,
                    Name = request.name,
                    Gender = condition.Gender,
                    IPAddress = GetClientIp(Request),
                    TxId = responseObj.txId
                };

                order.OrderDetail.Add(new OrderDetail
                {
                    BirthDay = condition.BirthDay,
                    BirthHour = condition.BirthHour,
                    DateType = (int)condition.DateType,
                    FirstName = condition.FirstName,
                    LastName = condition.LastName,
                    Gender = condition.Gender,
                    IsLeap = condition.IsLeap,
                    ProductId = request.productId
                });

                db.Order.Add(order);
                db.SaveChanges();
            }

            return new GetTxIdResponse { 
                paymentData = paymentData,
                paymentUrl = WebConfigVariable.PaymentUrl,
                txId = responseObj.txId
            };
        }

        private string GetGameUrl(string productId, string orderId)
        {
            string baseUrl = WebConfigVariable.BaseUrl;
            return $"{baseUrl}/index.html#/results?orderid={orderId}";
        }
        private string GetClientIp(HttpRequestMessage request = null)
        {
            request = request ?? Request;

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
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

        private string CreateOrderId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
    }
}
