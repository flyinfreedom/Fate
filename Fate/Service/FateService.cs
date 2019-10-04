using Fate.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.Service
{
    public class FateService : IFateService
    {
        private string _type;
        private dynamic _param;

        private IResult _result;

        public FateService()
        {
            this._result = new Result();
        }

        public IFateStrategy FateStrategyFactory()
        {
            IFateStrategy strategy = null;

            switch (_type)
            {
                case "ST01":
                    strategy = new ST01(_param);
                    break;
                case "NA01":
                    strategy = new NA01(_param);
                    break;
            }

            return strategy;
        }

        public IResult GetFateResultCode(string type, bool isPayed, dynamic param)
        {
            this._type = type;
            this._param = param;

            IFateStrategy _fateStrategy = FateStrategyFactory();
            this._result = _fateStrategy.Operation(isPayed);

            return this._result;
        }
    }
}