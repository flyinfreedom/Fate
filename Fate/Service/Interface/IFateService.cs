using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fate.Service
{
    public interface IFateService
    {
        IResult GetFateResultCode(string type, bool isPayed, dynamic param);
        IFateStrategy FateStrategyFactory();
    }

    public interface IFateStrategy
    {
        IResult Operation(bool isPayed);
    }
}
