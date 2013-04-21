using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CH.Alika.Json.Server;

namespace CH.Alika.Json
{
    public class JsonSqlServerSettings
    {
        public string StoredProcedurePrefix { get; set; }
        public IStoredProcRequest RequestContext { get; set; }
        public string MetaDataStoredProcName { get; set; } // defaults to "sp_sproc_columns"
    }
}
