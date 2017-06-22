using System.Data.SqlClient;
using System.IO;
using CH.Alika.Json.Server.Model;
using Newtonsoft.Json;

namespace CH.Alika.Json.Server
{
    internal class StoredProcInvoker
    {
        private readonly IRecordFactory _recordFactory;
        private readonly ISqlCommandFactory _sqlCmdFactory;

        public StoredProcInvoker(ISqlCommandFactory sqlCmdFactory)
        {
            _sqlCmdFactory = sqlCmdFactory;
            _recordFactory = new RecordFactory();
        }

        public void Invoke(SqlConnection connection, IStoredProcRequest request, TextWriter writer)
        {
            var optionsSet = false;
            var isArray = false;
            IDataContainer response = new JObjectDataContainer();
            using (var cmd = _sqlCmdFactory.CreateSqlCommand(connection, request))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    do
                    {
                        var options = ApplyResultToJObject(response, reader, isArray);
                        if (optionsSet || options == null) continue;

                        // apply options
                        optionsSet = true;
                        isArray = options.IsArray || options.IsArrayOfArray;
                        response = new StreamedDataContainer(writer, options.IsArray, options.IsArrayOfArray, options.IsFormatted);
                    } while (reader.NextResult());
                }
            }

            if (response.IsSerializable)
                SerializeObject(response.ObjectRepresentation, writer);

            response.End();
        }

        private IOptions ApplyResultToJObject(IDataContainer response, SqlDataReader reader, bool arrayElemntsOnly)
        {
            while (reader.Read())
            {
                var jsonRecord = arrayElemntsOnly
                    ? _recordFactory.CreateArrayElement(reader)
                    : _recordFactory.Create(reader);
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
            var jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings {Formatting = Formatting.Indented});

            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.Formatting = jsonSerializer.Formatting;

                jsonSerializer.Serialize(jsonWriter, value);
            }
        }
    }
}