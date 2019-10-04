using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fate.ViewModels
{
    public class VMST01 : VMFateBase
    {
        public string FirstLife { get; set; }
        public string FirstLifeSuggest { get; set; }

        public string SecondLife { get; set; }
        public string SecondLifeSuggest { get; set; }

        public string ThirdLife { get; set; }
        public string ThirdSuggest { get; set; }

        public string FourthLife { get; set; }
        public string FourthLifeSuggest { get; set; }
    }
}