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

            using (var db = new FortuneTellingEntities())
            {
                string email = request.uid.Contains("@") ? request.uid : string.Empty;
                string orderId = CreateOrderId();

                var requestModel = new GetTxIdRequestModel();
                requestModel.amount = db.Product.FirstOrDefault(x => x.ProductId == "Ziwei").Amount;
                requestModel.uid = request.uid.Replace(" ", string.Empty); ;
                requestModel.userIp = GetClientIp(Request);
                requestModel.orderId = orderId;
                requestModel.gameUrl = GetGameUrl("", orderId);
                requestModel.countryPrefix = request.uid.Split(' ')[0];
                requestModel.msisdn = request.uid.Split(' ')[1];

                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(requestModel.GetFullUrl(request.productId));
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
                string paymentData = AESHelper.Encrypt(paymentDataStr, request.productId);


              
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

                return new GetTxIdResponse
                {
                    paymentData = paymentData,
                    paymentUrl = WebConfigVariable.PaymentUrl,
                    txId = responseObj.txId
                };
            }
        }

        [Route("order/{data}")]
        [HttpPost]
        public CreateOrderResponse GetOrder(string data)
        {
            CreateOrderResponse result = new CreateOrderResponse();
            try
            {
                string urlDeconde = HttpUtility.HtmlDecode(data);
                string json = AESHelper.Decrypt(urlDeconde);
                CreateOrderRequest order = JsonConvert.DeserializeObject<CreateOrderRequest>(json);
                string orderId = CreateOrderId();
                using (var db = new FortuneTellingEntities())
                {
                    db.Order.Add(new Order
                    {
                        OrderId = orderId,
                        Datetime = DateTime.Now,
                        Name = order.Name,
                        Email = order.Email,
                        ContactPhone = order.ContactPhone,
                        OrderDetail = new List<OrderDetail>() { new OrderDetail { 
                            ProductId = order.ProductId,
                            OrderId = orderId,
                            DateType = order.DateType,
                            BirthDay = order.BirthDay,
                            BirthHour = order.BirthHour,
                            Gender = order.Gender ?? false,
                            IsLeap = order.IsLeap ?? false
                        }}
                    });

                    db.SaveChanges();
                }
                result.Success = true;
                result.OrderId = orderId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result.Success = false;
                result.Message = e.Message;
            }

            return result;
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
