using Fate.Helper;
using Fate.Models;
using Fate.ViewModels.Payment;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Fate.Service
{
    public class PaymentService
    {
        public QueryTxIdStatusResponse QueryTxIdStatus(string orderId, string txId, string productId)
        {
            string url = WebConfigVariable.QueryTxIdStatusUrl;
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
#if DEBUG
            cid = "PG_99999999";
#endif
            url = $"{url}?cid={cid}&data={HttpUtility.UrlEncode(AESHelper.Encrypt(JsonConvert.SerializeObject(new { orderId, txId }), productId))}";

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            using (WebResponse response = httpRequest.GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string responseStr = sr.ReadToEnd();
                    var responseObj = JsonConvert.DeserializeObject<QueryTxIdStatusResponse>(responseStr);
                    return responseObj;
                }
            }
        }
    }
}