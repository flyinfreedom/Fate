using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.ViewModels.Payment
{
    public class QueryTxIdStatusResponse
    {
        public string resultCode { get; set; }
        public string resultMsg { get; set; }
        public string orderId { get; set; }
        public string txId { get; set; }
        public string media { get; set; }
        public string payTime { get; set; }
        public string uid { get; set; }
        public int amount { get; set; }
        public string serialNumber {get;set;}
        public string memo { get; set; }
        public string paymentCode { get; set; }
    }
}