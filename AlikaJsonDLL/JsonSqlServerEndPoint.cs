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
        private String storedProcedurePrefix;
        private IStoredProcRequest requestContext;
        private string sp_sproc_columns;

        public JsonSqlServerEndPoint(JsonSqlServerSettings settings)
        {
            storedProcedurePrefix = settings.StoredProcedurePrefix ?? "";
            requestContext = settings.RequestContext ?? new NullRequestContext();
            sp_sproc_columns = settings.MetaDataStoredProcName ?? "sp_sproc_columns";
        }

        public string process(SqlConnection connection, string payload)
        {
            JsonRpcRequest request = JsonConvert.DeserializeObject<JsonRpcRequest>(payload);
            JObject response = new JObject();


            StoredProcInvoker proc = new StoredProcInvoker(storedProcedurePrefix, sp_sproc_columns);
            response = proc.invoke(connection, CreateRequest(request));

            return JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        private IStoredProcRequest CreateRequest(JsonRpcRequest jsonRequest)
        {
            return new StoredProcRequest(requestContext, new JsonStoredProcRequest(jsonRequest));
        }

        private class NullRequestContext : IStoredProcRequest
        {
            string IStoredProcRequest.Method
            {
                get { return null; }
            }

            IStoredProcParam IStoredProcRequest.CreateStoredProcParam(IStoredProcParamInfo stprocParamInfo)
            {
                return null;
            }
        }

        private class StoredProcRequest : IStoredProcRequest
        {
            private IStoredProcRequest requestContext;
            private IStoredProcRequest request;

            public StoredProcRequest(IStoredProcRequest requestContext, IStoredProcRequest request)
            {
                this.requestContext = requestContext;
                this.request = request;
            }

            string IStoredProcRequest.Method
            {
                get { return requestContext.Method ?? request.Method; }
            }

            IStoredProcParam IStoredProcRequest.CreateStoredProcParam(IStoredProcParamInfo stprocParamInfo)
            {
                return requestContext.CreateStoredProcParam(stprocParamInfo)
                    ?? request.CreateStoredProcParam(stprocParamInfo);
            }
        }
    }
}
