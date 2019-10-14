using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.Backoffice.Models
{
    public class LoginRequest
    {
        public string LoginId { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
        public string Message { get; set; }
    }
}