using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CH.Alika.Json.Server;

namespace CH.Alika.Json
{
    public class SimpleRequestContext : IStoredProcRequest
    {
        readonly Dictionary<string, object> _dictionary;

        public SimpleRequestContext(Dictionary<string, object> dictionary)
        {
            this._dictionary = new Dictionary<string, object>(dictionary,StringComparer.OrdinalIgnoreCase);
        }

        string IStoredProcRequest.Method
        {
            get { return null; }
        }

        IStoredProcParam IStoredProcRequest.CreateStoredProcParam(IStoredProcParamInfo stprocParamInfo)
        {
            string name = stprocParamInfo.Name.ToLowerInvariant().Substring(1);
            object value;

            if (_dictionary.TryGetValue(name,out value))
            {
                return new StoredProcParam(stprocParamInfo.Name, value);
            }

            return null;
        }

        class StoredProcParam : IStoredProcParam
        {
            private String name;
            private Object value;

            public StoredProcParam(String name, Object value)
            {
                this.name = name;
                this.value = value;
            }

            void IStoredProcParam.AddParam(SqlCommand cmd)
            {
                cmd.Parameters.Add(name, SqlDbType.VarChar).Value = value;
            }
        }
    }
}
