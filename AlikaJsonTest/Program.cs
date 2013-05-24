using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CH.Alika.Json.Shared.Model;
using CH.Alika.Json;
using CH.Alika.Json.Server;
using System.Data.SqlClient;
using System.Data;

namespace TicketServiceTest
{
    class Program
    {
        // sp_sproc_columns @procedure_name = 'name'
        static void Main(string[] args)
        {
            JsonSqlServerEndPoint processor = new JsonSqlServerEndPoint(
                new JsonSqlServerSettings
                {
                    StoredProcedurePrefix = "stproc_wb_",
                    RequestContext = new SimpleRequestContext(new Dictionary<string, object>
                    {
                        { "sessionid", 20 }
                    })
                });

            String response = ""; 
            using (var connection = OpenConnection())
            {
               response = processor.process(connection, JsonConvert.SerializeObject(GetTickets()));
            }
            Console.Out.WriteLine(response);
            Console.Out.WriteLine("done");
        }

        private static SqlConnection OpenConnection()
        {
            var c = new SqlConnection(Properties.Settings.Default.SampleConnectionString);
            c.Open();
            return c;
        }

        private static JsonRpcRequest GetTicketingContext()
        {
            JsonRpcRequest message = new JsonRpcRequest();
            message.Method = "getTicketingContext";
            message.Params = new JObject();
            message.Params.Add("ticketedObject", ToJToken(new EntityRef { id = "0" }));

            return message;
        }

        private static JsonRpcRequest GetTickets()
        {
            JsonRpcRequest message = new JsonRpcRequest();
            message.Method = "getTickets";
            message.Params = new JObject();
            EntityLoadConfig pageLoadConfig = new EntityLoadConfig
            {
                offset = 0,
                limit = 100,
                filters = new List<FilterConfig> {
                    new FilterConfig { value = "1" }
                }
            };

            message.Params.Add("offset", 0);
            message.Params.Add("limit", 100);
            message.Params.Add("filters", 
                ToJToken(new List<FilterConfig> {                    
                    new FilterConfig { value = "1" }
                }));
            return message;
        }

        private static JToken ToJToken(Object o)
        {
            return JToken.FromObject(o);
        }
    }

    class EntityLoadConfig
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public IList<FilterConfig> filters { get; set; }
    }

    class FilterConfig
    {

        public string comparison { get; set; }
        public string field { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    class EntityRef
    {
        public string id { get; set; }
    }

}
