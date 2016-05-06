using System;
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
            bool optionsSet = false;
            IDataContainer response = new JObjectDataContainer();
            using (var cmd = _sqlCmdFactory.CreateSqlCommand(connection, request))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    do
                    {
                        IOptions options = ApplyResultToJObject(response, reader);
                        if (!optionsSet && options != null)
                        {
                            optionsSet = true;
                            response = new StreamedRootDataContainer(writer,true);
                        }
                    } while (reader.NextResult());
                }
            }

            if (response.IsSerializable)
                SerializeObject(response.ObjectRepresentation,writer);

            response.End();
        }

        private IOptions ApplyResultToJObject(IDataContainer response, SqlDataReader reader)
        {
            while (reader.Read())
            {
                IRecord jsonRecord = _recordFactory.Create(reader);
                if (jsonRecord is IOptions)
                    return jsonRecord as IOptions;

                jsonRecord.ApplyTo(response);
                if (jsonRecord.IsNotPartOfCollection())
                    break;
            }
            return null;
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
