using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.ViewModels
{
    public class GetTxIdResponse
    {
        public string paymentUrl { get; set; }
        public string txId { get; set; }
        public string paymentData { get; set; }
    }
}