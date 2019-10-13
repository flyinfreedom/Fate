using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.ViewModels
{
    public class GetTxIdRequest
    {
        public string condition { get; set; }
        public string name { get; set; }
        public string productId { get; set; }
        public string uid { get; set; }
        public string email { get; set; }
    }
}