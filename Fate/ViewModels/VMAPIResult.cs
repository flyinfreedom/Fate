using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.ViewModels
{
    public class VMAPIResult
    {
        public VMAPIResult() : this(false)
        {

        }

        public VMAPIResult(bool success)
        {
            Success = success;
        }

        public bool Success { get; set; }
        public string Mesaage { get; set; }
        public dynamic Data { get; set; }
    }
}