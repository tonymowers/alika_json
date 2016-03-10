using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json.Linq;

namespace CH.Alika.Json.Shared.Model
{
    public class JsonRpcRequest
    {
        public String ApiVersion { get; set; }
        public String Method { get; set; }
        public String AccessKey { get; set; }
        public JObject Params { get; set; }
    }
}