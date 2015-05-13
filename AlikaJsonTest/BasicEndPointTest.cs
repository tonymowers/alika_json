using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CH.Alika.Json.Shared.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CH.Alika.Json.Test
{
    [TestFixture]
    public class BasicEndPointTest
    {
        private JsonSqlServerEndPoint _endpoint;

        private static SqlConnection OpenConnection()
        {
            var c = new SqlConnection(Properties.Settings.Default.AlikaJsonTestConnectionString);
            c.Open();
            return c;
        }

        [SetUp]
        public void SetUp()
        {
            _endpoint =  new JsonSqlServerEndPoint(
                new JsonSqlServerSettings
                {
                    StoredProcedurePrefix = "stproc_alika_",
                    RequestContext = new SimpleRequestContext(new Dictionary<string, object>
                    {
                        { "sessionID", 20 }
                    })
                });    
        }

        [Test]
        public void GetUsers()
        {
            JsonRpcRequest rpcRequest = new JsonRpcRequest
            {
                ApiVersion = "1.0",
                Method = "UsersGet"
            };
            string response;
            using (var connection = OpenConnection())
            {
                string request = JsonConvert.SerializeObject(rpcRequest);
                response = _endpoint.process(connection,request);
            }
            Console.Out.WriteLine(response);
            Console.Out.WriteLine("done");  
        }

        [Test]
        public void GetUser()
        {
            JsonRpcRequest rpcRequest = new JsonRpcRequest
            {
                ApiVersion = "1.0",
                Method = "UserGet",
                Params = new JObject {{"userID", "juser"}}
            };
            string response;
            using (var connection = OpenConnection())
            {
                string request = JsonConvert.SerializeObject(rpcRequest);
                response = _endpoint.process(connection, request);
            }
            Console.Out.WriteLine(response);
            Console.Out.WriteLine("done");
        }

        [Test]
        public void UpdateUser()
        {
            UserData user = new UserData
            {
                userID = "juser",
                firstName = "Joe",
                lastName = "User"
            };
            JsonRpcRequest rpcRequest = new JsonRpcRequest
            {
                ApiVersion = "1.0",
                Method = "UserUpdate",
                Params = new JObject { { "user", JToken.FromObject(user) } }
            };
            string response;
            using (var connection = OpenConnection())
            {
                string request = JsonConvert.SerializeObject(rpcRequest);
                response = _endpoint.process(connection, request);
            }
            Console.Out.WriteLine(response);
            Console.Out.WriteLine("done");
        }

        [Test]
        public void GetSessionDetails()
        {
            JsonRpcRequest rpcRequest = new JsonRpcRequest
            {
                ApiVersion = "1.0",
                Method = "SessionDetailsGet",
            };
            string response;
            using (var connection = OpenConnection())
            {
                string request = JsonConvert.SerializeObject(rpcRequest);
                response = _endpoint.process(connection, request);
            }
            Console.Out.WriteLine(response);
            Console.Out.WriteLine("done");
        }
    }

    class UserData
    {
        public string userID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
}
