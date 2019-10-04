using Fate.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.ViewModels
{
    public class GetTxIdRequestModel
    {
        public string orderId { get; set; }
        public string uid { get; set; }
        public int amount { get; set; }
        public string callBackUrl { get { return WebConfigVariable.PaymentCallBackUrl; } }
        public string userIp { get; set; }
        public int snType { get { return 0; } }
        public string gameUrl { get; set; }

        public string GetFullUrl()
        {
            var obj = new { orderId, uid, amount, callBackUrl, userIp, snType, gameUrl };
            string json = JsonConvert.SerializeObject(obj);
            string encrypt = AESHelper.Encrypt(json);
            string urlEncode = HttpUtility.UrlEncode(encrypt);
            return $"{WebConfigVariable.GetTxIdUrl}?cid={WebConfigVariable.CID}&data={urlEncode}";
        }
    }

    public class GetTxIdResponseModel
    { 
        public string resultCode { get; set; }
        public string resultMsg { get; set; }
        public string orderId { get; set; }
        public string txId { get; set; }
        public int amount { get; set; }
    }
}