using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fate.ViewModels
{
    public class VMNA01 : VMFateBase
    {
        [Required]
        [StringLength(2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(2)]
        public string LastName { get; set; }

        public string Result { get; set; }
    }
}