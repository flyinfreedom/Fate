using Fate.Helper;
using System;

namespace Fate.Service
{
    public class ST01 : STBase, IFateStrategy
    {
        private int _cnBirthMonth;
        private int _cnBirthDay;
        private bool _gender;
        private string _zodiac;

        private int _birthTime;
        private DateTime _ceDatetime;
        private CNDate _cn;


        public ST01(dynamic param)
        {
            this._birthTime = param.BirthHour;
            this._gender = param.Gender;

            _ceDatetime = DateTime.Parse(param.BirthDay);
            if (this._birthTime == 0)  //--- 如果被選到的是 23點~24點的子時，將日期加一天
            {
                _ceDatetime = _ceDatetime.AddDays(1);
                this._birthTime = 1;
            }
            else if (this._birthTime == -1)
            {
                //--- 若沒選時辰，則以現在時間判斷
                this._birthTime = Convert.ToInt32(Math.Ceiling((double)((DateTime.Now.Hour + 1) / 2)));
            }

            _cn = new CNDate(_ceDatetime);
            this._cnBirthMonth = Convert.ToInt32(_cn.GetCNDate().Split('/')[1]);
            this._cnBirthDay = Convert.ToInt32(_cn.GetCNDate().Split('/')[2]);

            //--- 若該月份為閏月，且日期大於 15，則將月份加一
            if (_cn.IsLeapMonth && _cnBirthDay > 15) {
                if (_cnBirthMonth == 12)
                {
                    _cnBirthMonth = 1;
                }
                else
                {
                    _cnBirthMonth += 1;
                }
            }

            this._zodiac = _cn.GetCNAnimal();
        }

        public IResult Operation(bool isPayed)
        {
            IResult result = new Result();
            result.FateTypeCode = "ST01";

            STModel zodiac = STList.Find(x => x.Zodiac.Trim() == this._zodiac.Trim());
            int _baseNum = STList.IndexOf(zodiac) + 1;  //--- 用生肖取得第四世的Code

            result.FateResult.Add("ST01_" + _baseNum.ToString().PadLeft(2, '0') + "04");

            if (isPayed) //--- 已付款取出前三世的資料
            {
                int thirdNum = stepOperation(_baseNum, this._cnBirthMonth);  //--- 用農曆月份取得第三世的Code
                result.FateResult.Add("ST01_" + thirdNum.ToString().PadLeft(2, '0') + "03");

                int secondNum = stepOperation(thirdNum, this._cnBirthDay);  //--- 用農曆日取得第二世的Code
                result.FateResult.Add("ST01_" + secondNum.ToString().PadLeft(2, '0') + "02");

                int firstNum = stepOperation(secondNum, this._birthTime);  //--- 用生辰取得第一世的Code
                result.FateResult.Add("ST01_" + firstNum.ToString().PadLeft(2, '0') + "01");
            }

            result.Success = true;
            return result;
        }

        private int stepOperation(int oraginal, int move)
        {
            int result = oraginal;

            if (this._gender)  //--- 男生就正向的算 (用加的)
            {
                result = (oraginal + (move - 1)) % 12;
            }
            else  //--- 男生就逆向的算 (用減的)
            {
                result = ((oraginal + 36) - (move - 1)) % 12;
            }

            result = result == 0 ? 12 : result;
            return result;
        }
    }
}