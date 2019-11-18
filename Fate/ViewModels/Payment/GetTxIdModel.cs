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
        public string callBackUrl { get { return string.Empty; } }
        public string userIp { get; set; }
        public int snType { get { return 0; } }
        public string gameUrl { get; set; }
        public string countryPrefix { get; set; }
        public string msisdn { get; set; }
        public string channel { get; set; }

        public string GetFullUrl(string productId)
        {
            var obj = new { orderId, uid, amount, callBackUrl, userIp, snType, gameUrl, countryPrefix, msisdn };
            string json = JsonConvert.SerializeObject(obj);
            string encrypt = AESHelper.Encrypt(json, productId);
            string urlEncode = HttpUtility.UrlEncode(encrypt);
            string cid = string.Empty; 
            switch (productId.ToUpper())
            {
                case "ZIWEI":
                    cid = WebConfigVariable.ZiweiCID;
                    break;
                case "ST01":
                    cid = WebConfigVariable.ST01CID;
                    break;
                case "NA01":
                    cid = WebConfigVariable.NA01CID;
                    break;
            }
            return $"{WebConfigVariable.GetTxIdUrl}?cid={cid}&data={urlEncode}";
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