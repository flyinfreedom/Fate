using Fate.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.ViewModels
{
    public class ConditionModel
    {
        public DateType DateType { get; set; }
        public string BirthDay { get; set; }
        public int BirthHour { get; set; }
        public bool Gender { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public bool IsLeap { get; set; }
    }
}