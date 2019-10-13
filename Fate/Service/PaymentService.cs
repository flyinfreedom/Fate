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
        public QueryTxIdStatusResponse QueryTxIdStatus(string orderId, string txId)
        {
            string url = WebConfigVariable.QueryTxIdStatusUrl;
            url = $"{url}?cid={WebConfigVariable.CID}&data={HttpUtility.UrlEncode(AESHelper.Encrypt(JsonConvert.SerializeObject(new { orderId, txId })))}";

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