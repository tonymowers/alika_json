using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace CH.Alika.Json.Server
{
    public interface IStoredProcParam
    {
        void AddParam(SqlCommand cmd);
    }
}
