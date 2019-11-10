using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcPaging;

namespace Fate.Backoffice.Models
{
    public class OrderRequest
    {
        public int page { get; set; }
        public string OrderId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public bool? IsPay { get; set; }
        public int Amount { get; set; }
        public DateTime? SDatetime { get; set; }
        public DateTime? EDatetime { get; set; }
        public IPagedList<Order> Orders { get; set; }

        public OrderRequest()
        {
            page = 0;
            Name = string.Empty;
            Email = string.Empty;
            ContactPhone = string.Empty;
        }
    }
}