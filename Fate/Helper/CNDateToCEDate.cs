using Fate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.Helper
{
    public class CNDateToCEDate
    {
        private FortuneTellingEntities _db;

        public CNDateToCEDate(FortuneTellingEntities db)
        {
            this._db = db;
        }

        public DateTime GetCEDate(int year, int month, int day, bool isLeap)
        {
            DateTime result = new DateTime(1899, 1, 1);
            Taiwanlunisolar model = this._db.Taiwanlunisolar.FirstOrDefault(x => x.YYYY == year && x.CNMM == month && x.CNDD == day && x.IsLeap == isLeap);
            if (model != null)
            {
                result = new DateTime(model.YYYY, model.MM, model.DD);
            }

            return result;
        }
    }
}