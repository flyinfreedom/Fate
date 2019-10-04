using System;
using System.Collections.Generic;

namespace Fate.Service
{
    public interface IResult
    {
        Guid ID { get; }

        bool Success { get; set; }

        string Message { get; set; }

        Exception Exception { get; set; }

        List<IResult> InnerResults { get; }

        string FateTypeCode { get; set; }

        List<string> FateResult { get; set; }
    }
}
