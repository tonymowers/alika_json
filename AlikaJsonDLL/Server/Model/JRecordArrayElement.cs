using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newtonsoft.Json.Linq;

namespace CH.Alika.Json.Server.Model
{
    class JRecordArrayElement : JRecordAbstract
    {
        public JRecordArrayElement(IDataRecord record)
            : base(record)
        {
           
        }

        public override void ApplyTo(JObject parent)
        {
            JObject jObject = new JObject();
            base.ApplyTo(jObject);
            if (jObject.HasValues)
            {
                JArray array = GetArray(parent, JsonPropertyName());
                array.Add(jObject);
            }

        }

        private static JArray GetArray(JObject parent, String objectName)
        {
            JArray array;
            JToken token = parent.SelectToken(objectName);
            if (token == null)
            {
                array = new JArray();
                parent.Add(objectName, array);

                return array;
            }

            if (token is JArray)
            {
                array = (JArray)token;
                return array;
            }

            throw new Exception("Attempt to add array element to none array property [" + objectName + "]");
        }

        public override bool IsNotPartOfCollection()
        {
            return false;
        }
    }
}
