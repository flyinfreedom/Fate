using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Fate.Helper
{
    public class CNDate
    {
        private TaiwanLunisolarCalendar _tlc;
        private DateTime _datetime;
        static string CAnimal = "鼠牛虎兔龍蛇馬羊猴雞狗豬";
        private bool _isLeapMonth = false;

        public CNDate(DateTime datetime) {
            _tlc = new TaiwanLunisolarCalendar();
            this._datetime = datetime;
        }

        public bool IsLeapMonth { get { return _isLeapMonth; } }

        public string GetCNDate()
        {
            int lun60Year = _tlc.GetSexagenaryYear(_datetime);
            int TeanGeanYear = _tlc.GetCelestialStem(lun60Year) - 1;
            int DeGeYear = _tlc.GetTerrestrialBranch(lun60Year) - 1;

            int lunMonth = _tlc.GetMonth(_datetime);
            int leapMonth = _tlc.GetLeapMonth(_tlc.GetYear(_datetime.AddYears(0)));
            if (leapMonth > 0 && lunMonth >= leapMonth)
            {
                lunMonth -= 1;
                _isLeapMonth = true;
            }
            int lunDay = _tlc.GetDayOfMonth(_datetime);

            return String.Format("{0}/{1}/{2}", this._datetime.Year.ToString(), lunMonth, lunDay);
        }

        public string GetCNAnimal()
        {
            int lun60Year = _tlc.GetSexagenaryYear(_datetime.AddYears(0));
            int DeGeYear = _tlc.GetTerrestrialBranch(lun60Year) - 1;

            return CAnimal[DeGeYear].ToString();
        }
    }
}