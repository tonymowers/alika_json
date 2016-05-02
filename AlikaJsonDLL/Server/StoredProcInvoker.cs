using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using CH.Alika.Json.Shared.Model;
using CH.Alika.Json.Server.Model;

namespace CH.Alika.Json.Server
{
   
    class StoredProcInvoker
    {
        private ISqlCommandFactory _sqlCmdFactory;
        private IRecordFactory _recordFactory;

        public StoredProcInvoker(ISqlCommandFactory sqlCmdFactory)
        {
            this._sqlCmdFactory = sqlCmdFactory;
            this._recordFactory = new JRecordFactory();
        }

        public JObject invoke(SqlConnection connection, IStoredProcRequest request)
        {
            JObjectDataContainer response = new JObjectDataContainer();
            using (var cmd = _sqlCmdFactory.CreateSqlCommand(connection, request))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    do
                    {
                        ApplyResultToJObject(response, reader);
                    } while (reader.NextResult());
                }
            }

            return response.JObject;
        }

        private void ApplyResultToJObject(IDataContainer response, SqlDataReader reader)
        {
            while (reader.Read())
            {
                JRecord jsonRecord = _recordFactory.create(reader);
                jsonRecord.ApplyTo(response);
                if (jsonRecord.IsNotPartOfCollection())
                    break;
            }
        }
      
    }
}
