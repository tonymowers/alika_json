using System;
using System.Text;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
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

        // ReSharper disable once InconsistentNaming
        public string process(SqlConnection connection, String payload)
        {
            return Process(connection, RequestFactory.Create(payload));
        }

        public string Process(SqlConnection connection, IStoredProcRequest request)
        {
            StringBuilder sb = new StringBuilder(1024);
            StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture);
            Process(connection, request, sw);
            return sw.ToString();
        }

        public void Process(SqlConnection connection, IStoredProcRequest request, TextWriter writer)
        {
            StoredProcInvoker proc = new StoredProcInvoker(_sqlCmdFactory);
            proc.Invoke(connection, RequestFactory.Create(_requestContext, request),writer);
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
