﻿using Fate.Helper;
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

                string uid = string.Empty;
                string msisdn = string.Empty;
                if (!string.IsNullOrEmpty(request.uid))
                {
                    msisdn = request.uid.Split(' ')[1];
                    if (request.uid.Contains("+886") && request.uid.Split(' ')[1][0] != '0')
                    {
                        uid = request.uid.Split(' ')[0] + "0" + request.uid.Split(' ')[1];
                        msisdn = "0" + msisdn;
                    }
                    else
                    {
                        uid = request.uid.Replace(" ", "");
                    }
                }

                var requestModel = new GetTxIdRequestModel();
                requestModel.amount = db.Product.FirstOrDefault(x => x.ProductId == "Ziwei").Amount;
                requestModel.uid = uid;
                requestModel.userIp = GetClientIp(Request);
                requestModel.orderId = orderId;
                requestModel.backUrl = GetGameUrl("", orderId);
                requestModel.countryPrefix = request.uid.Split(' ')[0];
                requestModel.msisdn = msisdn;
                requestModel.buyUsage = 1;
                requestModel.useUsage = 1;
                requestModel.channel = request.channel;
                requestModel.commodityId = GetCommodityId(request.productId);

#if DEBUG
                requestModel.amount = 1;
#endif

                string GetCommodityId(string productId)
                {
                    switch (productId.ToUpper())
                    {
                        case "ZIWEI":
                            return "20190001";
                        case "ST01":
                            return "20190002";
                        case "NA01":
                            return "20190003";
                    }
                    return string.Empty;
                }

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
                var amuont = db.Product.FirstOrDefault(x => x.ProductId == request.productId).Amount;
              
                    var condition = JsonConvert.DeserializeObject<ConditionModel>(request.condition);
                    var order = new Order
                    {
                        OrderId = orderId,
                        Datetime = DateTime.Now,
                        Email = email,
                        ContactPhone = string.IsNullOrEmpty(email) ? request.uid : string.Empty,
                        Uid = uid,
                        Name = request.name,
                        Amount = amuont,
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

        [Route("order")]
        [HttpPost]
        public CreateOrderResponse GetOrder(string data)
        {
            CreateOrderResponse result = new CreateOrderResponse();
            try
            {
                string urlDeconde = HttpUtility.HtmlDecode(data);
                string json = AESHelper.Decrypt(urlDeconde, string.Empty);
                CreateOrderRequest order = JsonConvert.DeserializeObject<CreateOrderRequest>(json);
                string orderId = CreateOrderId();
                using (var db = new FortuneTellingEntities())
                {
                    int? amount = db.Product.FirstOrDefault(x => x.ProductId == order.ProductId)?.Amount;
                    db.Order.Add(new Order
                    {
                        OrderId = orderId,
                        Datetime = DateTime.Now,
                        Name = order.Name,
                        Uid = order.ContactPhone,
                        Email = order.Email ?? string.Empty,
                        ContactPhone = order.ContactPhone,
                        IsPayed = true,
                        Amount = amount,
                        OrderDetail = new List<OrderDetail>() { new OrderDetail { 
                            ProductId = order.ProductId,
                            OrderId = orderId,
                            DateType = order.DateType,
                            BirthDay = order.BirthDay,
                            BirthHour = order.BirthHour,
                            Gender = order.Gender == 1 ? true : false,
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
            return $"{baseUrl}/Fate/index.html#/results?orderid={orderId}";
        }

        private string GetClientIp(HttpRequestMessage request = null)
        {
#if DEBUG
            return "127.0.0.1";
#endif
            request = request ?? Request;

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                var HTTP_X_FORWARDED_FOR = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.ServerVariables.Get("HTTP_X_FORWARDED_FOR");
                if (!string.IsNullOrEmpty(HTTP_X_FORWARDED_FOR)) {
                    return HTTP_X_FORWARDED_FOR;
                }
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

        [HttpPost]
        [Route("callback")]
        public IHttpActionResult Callback(string cid, string Data)
        {
            var productId = string.Empty;
            switch (cid.ToUpper())
            {
                case "CC_20190001":
                    productId = "Ziwei";
                    break;
                case "CC_20190002":
                    productId = "ST01";
                    break;
                case "CC_20190003":
                    productId = "NA01";
                    break;
            }

            string dataString = AESHelper.Decrypt(Data, productId);
            var callbackResult = JsonConvert.DeserializeObject<CallbackModel>(dataString);

            using (var db = new FortuneTellingEntities())
            {
                var order = db.Order.FirstOrDefault(x => x.OrderId == callbackResult.orderId);
                if (order == null) {
                    return BadRequest($"order is null, message is |||{dataString}|||");
                }
                order.IsPayed = true;
                db.SaveChanges();
            }

            return Ok(new { resultCode = "0000", resultMsg = "更新成功" });
        }

        private class CallbackModel
        {
            public string orderId { get; set; }
            public string uid { get; set; }
            public string commodityId { get; set; }
            public int amount { get; set; }
            public int useUsage { get; set; }
            public string memo { get; set; }
            public string txId { get; set; }
        }
    }
}
