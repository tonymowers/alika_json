using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CH.Alika.Json.Shared.Model;
using System.Data;
using Newtonsoft.Json.Linq;
using CH.Alika.Json.Server.Model;

namespace CH.Alika.Json.Server
{
    public class JsonStoredProcRequest : IStoredProcRequest
    {
        private JsonRpcRequest request;
        private IFieldNameTranslator fieldNameXlator = new DefaultFieldNameTranslator();

        public JsonStoredProcRequest(JsonRpcRequest request)
        {
            this.request = request;
        }

        public string Method
        {
            get
            {
                return request == null ? null : request.Method;
            }
        }

        public string AccessKey
        {
            get
            {
                return request == null ? null : request.AccessKey;
            }
        }

        public IStoredProcParam CreateStoredProcParam(IStoredProcParamInfo stprocParamInfo)
        {
            string name = stprocParamInfo.Name; //  stprocParamInfo["COLUMN_NAME"].ToString();
            string type = stprocParamInfo.Type; // stprocParamInfo["TYPE_NAME"].ToString();
            if (!stprocParamInfo.IsInputParam)
            {
                return null;
            }

            JToken value = LookupStprocParamInRequest(name);
            if (value == null)
            {
                return null;
            }

            return new JsonStoredProcParam(name,type,value);
        }

        private JToken LookupStprocParamInRequest(string name)
        {
            string jsonPath = fieldNameXlator.StProcVariableNameToJsonPath(name);
            JObject p = request == null ? null : request.Params;
            return p == null ? null : p.SelectToken(jsonPath);
        }

        private static bool IsInputParam(IDataRecord stprocParamInfo)
        {
            const short INPUT_VALUE = 1;
            return stprocParamInfo["COLUMN_TYPE"].Equals(INPUT_VALUE);
        }

       
    }
}
