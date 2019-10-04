using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.ViewModels
{
    public class VMInvoice
    {
        public string Email { get; set; }
        public string HandleType { get; set; }
        public string ContactPhone { get; set; }
        public string ContactAddress { get; set; }
        public string VATNumber { get; set; }
        public string CompanyName { get; set; }
    }
}