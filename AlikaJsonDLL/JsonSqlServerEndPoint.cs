using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using System.Collections;
using Newtonsoft.Json.Linq;
using CH.Alika.Json.Shared.Model;
using CH.Alika.Json.Server;

namespace CH.Alika.Json
{
    public class JsonSqlServerEndPoint
    {
        private readonly ISqlCommandFactory _sqlCmdFactory;
        private readonly IStoredProcRequest _requestContext;

        public JsonSqlServerEndPoint(JsonSqlServerSettings settings)
        {
            string storedProcedurePrefix = settings.StoredProcedurePrefix ?? "";
            string spSprocColumns = settings.MetaDataStoredProcName ?? "sp_sproc_columns";
            _requestContext = settings.RequestContext ?? new NullRequestContext();
            _sqlCmdFactory = new SqlCommandFactory(storedProcedurePrefix, spSprocColumns);
        }

        public string process(SqlConnection connection, String payload)
        {
            return Process(connection, RequestFactory.Create(payload));
        }

        public string Process(SqlConnection connection, IStoredProcRequest request)
        {
            StoredProcInvoker proc = new StoredProcInvoker(_sqlCmdFactory);
            JObject response = proc.invoke(connection,RequestFactory.Create(_requestContext, request));

            return JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        private class NullRequestContext : IStoredProcRequest
        {
            string IStoredProcRequest.Method
            {
                get { return null; }
            }

            string IStoredProcRequest.AccessKey
            {
                get { return null; }
            }

            IStoredProcParam IStoredProcRequest.CreateStoredProcParam(IStoredProcParamInfo stprocParamInfo)
            {
                return null;
            }
        }

       
    }
}
