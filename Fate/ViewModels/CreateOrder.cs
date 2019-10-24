using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.ViewModels
{
    public class CreateOrderRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public string ProductId { get; set; }
        public int? DateType { get; set; }
        public string BirthDay { get; set; }
        public int? BirthHour { get; set; }
        public bool? Gender { get; set; }
        public bool? IsLeap { get; set; }
    }


    public class CreateOrderResponse
    {
        public bool Success { get; set; }
        public string OrderId { get; set; }
        public string Message { get; set; }
    }
}