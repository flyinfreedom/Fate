using Fate.Helper;
using Fate.Service.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.ViewModels
{
    public class ZiweiRequest
    {
        public int BirthTime { get; set; }
        public DateType DateType { get; set; }
        public bool IsLeap { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public string OrderId { get; set; }
    }
}