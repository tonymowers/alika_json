using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json.Linq;

/// <summary>
/// Summary description for JsonRpcRequest
/// </summary>
namespace CH.Alika.Json.Shared.Model
{
    public class JsonRpcRequest
    {
        public String ApiVersion { get; set; }
        public String Method { get; set; }
        public JObject Params { get; set; }
    }
}