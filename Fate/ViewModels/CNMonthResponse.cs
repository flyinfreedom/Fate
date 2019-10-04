using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.ViewModels
{
    public class CNMonthResponse
    {
        public int Month { get; set; }
        public bool IsLeap { get; set; }
        public int LastDay { get; set; }
    }
}