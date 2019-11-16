using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.ViewModels
{
    public class VMFateBase
    {
        public string FateType { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string ContactAddress { get; set; }
        public string ContactPhone { get; set; }
        public string VATNumber { get; set; }
        public string VATTitle { get; set; }
        public string InvoiceHandle { get; set; }
        public string Recipient { get; set; }
        public string RecipientCode { get; set; }

        public string CaringCode { get; set; }

        public string DateType { get; set; }
        public string BirthDay { get; set; }
        public int BirthHour { get; set; }
        public bool Gender { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string channel { get; set; }
        public string OrderId { get; set; }
    }
}