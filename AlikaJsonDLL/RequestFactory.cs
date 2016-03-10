using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using CH.Alika.Json.Server;
using CH.Alika.Json.Shared.Model;
using Newtonsoft.Json;

namespace CH.Alika.Json
{
    /**
     * Convenience class for creating IStoredProcRequest objects
     * from a variety of data types
     */
    public static class RequestFactory
    {
        /**
         * Create request from json payload
         */
        public static IStoredProcRequest Create(String jsonPayload)
        {
            return Create(JsonConvert.DeserializeObject<JsonRpcRequest>(jsonPayload));
        }

        /**
         * Create request from JstonRpcRequest object
         */
        public static IStoredProcRequest Create(JsonRpcRequest jsonRpcRequest)
        {
            return new JsonStoredProcRequest(jsonRpcRequest);
        }

        /**
         * Layer one request on top of another one.
         *    defaultRequest provides default value for the request
         *    request override values in the defaultRequest
         */
        public static IStoredProcRequest Create(IStoredProcRequest request, IStoredProcRequest defaultRequest)
        {
            return new LayerStoredProcRequest(request,defaultRequest);
        }

        /**
         * Create request from name value pair
         * Useful for parameters from a web get
         */
        public static IStoredProcRequest Create(String method, String accessKey, NameValueCollection nvc)
        {
            IDictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var k in nvc.AllKeys)
            {
                dict.Add(k, nvc[k]);
            }  

            return new DictionaryStoredProcRequest(method, accessKey,dict);
        }

        /**
         * Create request from name value pair
         * Useful when creating a request in code from hard-coded values
         */
        public static IStoredProcRequest Create(String method, String accessKey, IDictionary<string,object> dictionary)
        {
            return new DictionaryStoredProcRequest(method, accessKey, dictionary);
        }
      
        private class LayerStoredProcRequest : IStoredProcRequest
        {
            private readonly IStoredProcRequest _request;
            private readonly IStoredProcRequest _defaultRequest;

            public LayerStoredProcRequest(IStoredProcRequest request, IStoredProcRequest defaultRequest)
            {
                _request = request;
                _defaultRequest = defaultRequest;
            }

            string IStoredProcRequest.Method
            {
                get { return _request.Method ?? _defaultRequest.Method; }
            }

            string IStoredProcRequest.AccessKey
            {
                get { return _request.AccessKey ?? _defaultRequest.AccessKey; }
            }

            IStoredProcParam IStoredProcRequest.CreateStoredProcParam(IStoredProcParamInfo stprocParamInfo)
            {
                return _request.CreateStoredProcParam(stprocParamInfo)
                    ?? _defaultRequest.CreateStoredProcParam(stprocParamInfo);
            }
        }

        public class DictionaryStoredProcRequest : IStoredProcRequest
        {
            private readonly Dictionary<string, object> _dictionary;


            public string Method { get; private set; }
            public string AccessKey { get; private set; }

            public DictionaryStoredProcRequest(string method, string accessKey, IDictionary<string, object> dictionary)
            {
                Method = method;
                AccessKey = accessKey;
                _dictionary = new Dictionary<string, object>(dictionary, StringComparer.OrdinalIgnoreCase);
            }



            IStoredProcParam IStoredProcRequest.CreateStoredProcParam(IStoredProcParamInfo stprocParamInfo)
            {
                string name = stprocParamInfo.Name.ToLowerInvariant().Substring(1);
                object value;

                if (_dictionary.TryGetValue(name, out value))
                {
                    return new StoredProcParam(stprocParamInfo.Name, value);
                }

                return null;
            }

            private class StoredProcParam : IStoredProcParam
            {
                private String name;
                private Object value;

                public StoredProcParam(String name, Object value)
                {
                    this.name = name;
                    this.value = value;
                }

                void IStoredProcParam.AddParam(SqlCommand cmd)
                {
                    cmd.Parameters.Add(name, SqlDbType.VarChar).Value = value;
                }
            }
        }
    }

   
}
