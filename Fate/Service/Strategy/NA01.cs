using Fate.Helper;
using Fate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.Service
{
    public class NA01 : IFateStrategy
    {
        private int _firstNum = 0;
        private int _secondNum = 0;
        //private string[] _fiveProperty = new string[] { "木", "火", "土", "金", "水" };
        private string[] _fiveProperty = new string[] { "水", "木", "木", "火", "火", "土", "土", "金", "金", "水" };
        private string[] _riseMapping = new string[] { "木火", "火土", "土金", "金水", "水木" };
        private string[] _declineMapping = new string[] { "火木", "土火", "金土", "水金", "木水" };
        private string[] _resultMapping = new string[] { "金木木金", "木土土木", "土水水土", "水火火水", "火金金火" };

        private readonly FortuneTellingEntities _db;  //--- 可以考慮用Cache，或是和Controller 共用 _db連線

        public NA01(dynamic param)
        {
            this._db = new FortuneTellingEntities();
            InitNameNum(_db, param);
            _db.Dispose();
        }

        private void InitNameNum(FortuneTellingEntities db, dynamic param)
        {
            string firstName = CharSetConverter.ToTraditional(param.FirstName);
            string lastName = CharSetConverter.ToTraditional(param.LastName);
            if (firstName.Length == 1 && lastName.Length == 1)
            {
                int lastNameNum = db.WordLibrary.FirstOrDefault(x => x.Word == lastName).Stroke;
                int firstNameNum = db.WordLibrary.FirstOrDefault(x => x.Word == firstName).Stroke;

                this._firstNum = (firstNameNum + lastNameNum) % 9;
                this._secondNum = (firstNameNum + 1) % 9;
                return;
            }

            if (lastName.Length == 2 && firstName.Length == 1)
            {
                int lastNameNum = db.WordLibrary.FirstOrDefault(x => x.Word == lastName.Remove(1)).Stroke;
                int lastNameNum2 = db.WordLibrary.FirstOrDefault(x => x.Word == lastName.Remove(0, 1)).Stroke;
                int firstNameNum = db.WordLibrary.FirstOrDefault(x => x.Word == firstName).Stroke;

                this._firstNum = (lastNameNum + lastNameNum2 + firstNameNum) % 9;
                this._secondNum = (firstNameNum + 1) % 9; 
                return;
            }

            if (lastName.Length == 1 && firstName.Length == 2)
            {
                int lastNameNum = db.WordLibrary.FirstOrDefault(x => x.Word == lastName).Stroke;
                int firstNameNum = db.WordLibrary.FirstOrDefault(x => x.Word == firstName.Remove(1)).Stroke;
                int firstNameNum2 = db.WordLibrary.FirstOrDefault(x => x.Word == firstName.Remove(0, 1)).Stroke;

                this._firstNum = (lastNameNum + firstNameNum) % 9;
                this._secondNum = (firstNameNum + firstNameNum2) % 9;
                return;
            }

            if (lastName.Length == 2 && firstName.Length == 2)
            {
                int lastNameNum = db.WordLibrary.FirstOrDefault(x => x.Word == lastName.Remove(1)).Stroke;
                int lastNameNum2 = db.WordLibrary.FirstOrDefault(x => x.Word == lastName.Remove(0, 1)).Stroke;
                int firstNameNum = db.WordLibrary.FirstOrDefault(x => x.Word == firstName.Remove(1)).Stroke;
                int firstNameNum2 = db.WordLibrary.FirstOrDefault(x => x.Word == firstName.Remove(0, 1)).Stroke;

                this._firstNum = (lastNameNum + lastNameNum2 + firstNameNum) % 9;
                this._secondNum = (firstNameNum + firstNameNum2) % 9;
                return;
            }
        }

        public IResult Operation(bool isPayed)
        {
            IResult result = new Result();

            if (_firstNum == _secondNum)
            {
                result.FateResult.Add("NA01-Equal");
                return result;
            }

            if ((_firstNum != _secondNum) && (GetFivePropertyCode(this._firstNum) == GetFivePropertyCode(this._secondNum)))
            {
                result.FateResult.Add("NA01-Equal2");
                return result;
            }

            string property = GetFivePropertyCode(this._firstNum) + GetFivePropertyCode(this._secondNum);

            if (_riseMapping.ToList().Any(x => x == property))
            {
                result.FateResult.Add("NA01-Rise");
                return result;
            }

            if (_declineMapping.ToList().Any(x => x == property))
            {
                result.FateResult.Add("NA01-Decline");
                return result;
            }

            for (int i = 0; i < 5; i++)
            {
                if (_resultMapping[i].Contains(property))
                {
                    result.FateResult.Add("NA01-" + (i + 1).ToString());
                    return result;
                }
            }

            return result;
        }

        private string GetFivePropertyCode(int number)
        {
            return this._fiveProperty[number];
        }
    }
}