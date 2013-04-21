using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CH.Alika.Json.Server
{
    public interface IStoredProcRequest
    {
        String Method { get; }
        IStoredProcParam CreateStoredProcParam(IStoredProcParamInfo stprocParamInfo);
    }
}
