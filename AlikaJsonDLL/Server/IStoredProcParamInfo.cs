using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CH.Alika.Json.Server
{
    public interface IStoredProcParamInfo
    {
        string ProcedureName { get; }

        string Name { get; }

        string Type { get; }

        bool IsInputParam { get; }
    }
}
