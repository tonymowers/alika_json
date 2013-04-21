using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CH.Alika.Json.Server;
using System.Data;
using System.Data.SqlClient;

namespace CH.Alika.Json
{
    public class SimpleRequestContext : IStoredProcRequest
    {
        Dictionary<string, object> dictionary;

        public SimpleRequestContext(Dictionary<string, object> dictionary)
        {
            this.dictionary = dictionary;
        }

        string IStoredProcRequest.Method
        {
            get { return null; }
        }

        IStoredProcParam IStoredProcRequest.CreateStoredProcParam(IStoredProcParamInfo stprocParamInfo)
        {
            string name = stprocParamInfo.Name.ToLowerInvariant().Substring(1);
            object value;

            if (dictionary.TryGetValue(name,out value))
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
