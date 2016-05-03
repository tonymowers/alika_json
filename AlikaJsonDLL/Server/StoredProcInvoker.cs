using System.Data.SqlClient;
using System.IO;
using CH.Alika.Json.Server.Model;
using Newtonsoft.Json;

namespace CH.Alika.Json.Server
{
   
    class StoredProcInvoker
    {
        private readonly ISqlCommandFactory _sqlCmdFactory;
        private readonly IRecordFactory _recordFactory;

        public StoredProcInvoker(ISqlCommandFactory sqlCmdFactory)
        {
            _sqlCmdFactory = sqlCmdFactory;
            _recordFactory = new JRecordFactory();
        }

        public void Invoke(SqlConnection connection, IStoredProcRequest request, TextWriter writer)
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

            SerializeObject(response,writer);
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


        private static void SerializeObject(object value, TextWriter writer)
        {
            JsonSerializer jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings { Formatting = Formatting.Indented });

            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.Formatting = jsonSerializer.Formatting;

                jsonSerializer.Serialize(jsonWriter, value);
            }
        }

    }
}
