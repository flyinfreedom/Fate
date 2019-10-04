using Fate.Models;
using Fate.Service.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.Helper
{
    public class DateTimeHelper
    {
        private FortuneTellingEntities _db;
        public Heavenly Heavenly { get; set; }
        public Branch Branch { get; set; }
        public BirthTimePeriod Birthtime { get; set; }
        public DateTime DateTime { get; set; }
        public int CNMonth { get; set; }
        public int CNDay { get; set; }
        public bool IsLeap { get; set; }

        public DateTimeHelper(FortuneTellingEntities db, DateType type, int year, int month, int day, int birthtime, bool isLeap = false)
        {
            _db = db;
            Taiwanlunisolar taiwanlunisolar = new Taiwanlunisolar();
            if (type == DateType.CNDate)
            {
                taiwanlunisolar = db.Taiwanlunisolar
                    .FirstOrDefault(d =>
                        d.YYYY == year &&
                        d.CNMM == month &&
                        d.CNDD == day &&
                        d.IsLeap == IsLeap);

                if (taiwanlunisolar == null)
                {
                    throw new Exception("this chinese date is not exist");
                }
            }
            else
            {
                taiwanlunisolar = db.Taiwanlunisolar
                    .FirstOrDefault(d =>
                        d.YYYY == year &&
                        d.MM == month &&
                        d.DD == day);

                if (taiwanlunisolar == null)
                {
                    throw new Exception("this date is not exist");
                }
            }

            DateTime = new DateTime(taiwanlunisolar.YYYY, taiwanlunisolar.MM, taiwanlunisolar.DD);

            Heavenly = (Heavenly)taiwanlunisolar.Heavenl;
            Branch = (Branch)taiwanlunisolar.Branch;

            if (birthtime == 0)
            {
                int hour = DateTime.Now.Hour;
                switch (hour)
                {
                    case 23:
                        birthtime = (int)BirthTimePeriod.Zi;
                        break;
                    case 0:
                        birthtime = (int)BirthTimePeriod.Zi;
                        break;
                    case 1:
                        birthtime = (int)BirthTimePeriod.Chou;
                        break;
                    case 2:
                        birthtime = (int)BirthTimePeriod.Chou;
                        break;
                    case 3:
                        birthtime = (int)BirthTimePeriod.Yin;
                        break;
                    case 4:
                        birthtime = (int)BirthTimePeriod.Yin;
                        break;
                    case 5:
                        birthtime = (int)BirthTimePeriod.Mao;
                        break;
                    case 6:
                        birthtime = (int)BirthTimePeriod.Mao;
                        break;
                    case 7:
                        birthtime = (int)BirthTimePeriod.Chen;
                        break;
                    case 8:
                        birthtime = (int)BirthTimePeriod.Chen;
                        break;
                    case 9:
                        birthtime = (int)BirthTimePeriod.Si;
                        break;
                    case 10:
                        birthtime = (int)BirthTimePeriod.Si;
                        break;
                    case 11:
                        birthtime = (int)BirthTimePeriod.Wu;
                        break;
                    case 12:
                        birthtime = (int)BirthTimePeriod.Wu;
                        break;
                    case 13:
                        birthtime = (int)BirthTimePeriod.Wei;
                        break;
                    case 14:
                        birthtime = (int)BirthTimePeriod.Wei;
                        break;
                    case 15:
                        birthtime = (int)BirthTimePeriod.Shen;
                        break;
                    case 16:
                        birthtime = (int)BirthTimePeriod.Shen;
                        break;
                    case 17:
                        birthtime = (int)BirthTimePeriod.You;
                        break;
                    case 18:
                        birthtime = (int)BirthTimePeriod.You;
                        break;
                    case 19:
                        birthtime = (int)BirthTimePeriod.Xu;
                        break;
                    case 20:
                        birthtime = (int)BirthTimePeriod.Xu;
                        break;
                    case 21:
                        birthtime = (int)BirthTimePeriod.Hai;
                        break;
                    case 22:
                        birthtime = (int)BirthTimePeriod.Hai;
                        break;
                }
            }

            if (birthtime == 13)
            {
                DateTime addDay = new DateTime(taiwanlunisolar.YYYY, taiwanlunisolar.MM, taiwanlunisolar.DD);
                addDay = addDay.AddDays(1);
                taiwanlunisolar = db.Taiwanlunisolar
                    .FirstOrDefault(d =>
                        d.YYYY == addDay.Year &&
                        d.MM == addDay.Month &&
                        d.DD == addDay.Day);
            }


            CNMonth = taiwanlunisolar.CNMM;

            if (taiwanlunisolar.IsLeap && taiwanlunisolar.CNDD > 15)
            {
                CNMonth = taiwanlunisolar.CNMM++;
                CNMonth = CNMonth == 13 ? 1 : CNMonth;
            }

            Birthtime = birthtime == 13 ? BirthTimePeriod.Zi : (BirthTimePeriod)birthtime;
            CNDay = taiwanlunisolar.CNDD;
            IsLeap = isLeap;
        }
    }
    public enum DateType
    {
        Normal = 0,
        CNDate = 1,
    }
}