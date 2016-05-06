using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CH.Alika.Json.Server;
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
            _endpoint = new JsonSqlServerEndPoint(
                new JsonSqlServerSettings
                {
                    MetaDataStoredProcName = "stproc_test_ActionInfoGet",
                    RequestContext = RequestFactory.Create(
                        null,
                        "1234",
                        new Dictionary<string, object>
                        {
                            {"sessionID", 20}
                        })
                });
        }

        [Test]
        public void Echo()
        {
            JsonRpcRequest rpcRequest = new JsonRpcRequest
            {
                ApiVersion = "1.0",
                Method = "stproc_test_Echo",
                Params = new JObject{{"msg","hello"}}
            };
            string response;
            using (var connection = OpenConnection())
            {
                response = _endpoint.Process(connection, RequestFactory.Create(rpcRequest));
            }
            Console.Out.WriteLine(response);
            Console.Out.WriteLine("done");
        }

        [Test]
        public void ObjectGet()
        {
            JsonRpcRequest rpcRequest = new JsonRpcRequest
            {
                ApiVersion = "1.0",
                Method = "stproc_test_ObjectGet",
                Params = new JObject {{"objectID", "10"}}
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
        public void Dataset()
        {
            JsonRpcRequest rpcRequest = new JsonRpcRequest
            {
                ApiVersion = "1.0",
                Method = "stproc_test_Dataset"
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
        public void ObjectUpdate()
        {
            UserData user = new UserData
            {
                UserId = "juser",
                FirstName = "Joe",
                LastName = "User"
            };
            JsonRpcRequest rpcRequest = new JsonRpcRequest
            {
                ApiVersion = "1.0",
                Method = "stproc_test_ObjectUpdate",
                Params = new JObject
                {
                    {"objectID", "10"},
                    {"object", JToken.FromObject(user)}
                }
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
        public void BadAccessKey()
        {
            JsonSqlServerEndPoint alternativeEndpoint = new JsonSqlServerEndPoint(
                new JsonSqlServerSettings
                {
                    MetaDataStoredProcName = "stproc_test_ActionInfoGet",
                    RequestContext = RequestFactory.Create(null, null, new Dictionary<string, object>
                    {
                        {"sessionID", 20}
                    })
                });

            JsonRpcRequest rpcRequest = new JsonRpcRequest
            {
                ApiVersion = "1.0",
                Method = "USERS_GET",
            };
            string response;
            using (var connection = OpenConnection())
            {
                string request = JsonConvert.SerializeObject(rpcRequest);
                response = alternativeEndpoint.process(connection, request);
            }
            Console.Out.WriteLine(response);
            Console.Out.WriteLine("done");
        }
    }

    class UserData
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
