using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Data;

namespace CH.Alika.Json.Server.Model
{
    class JRecordProperty : JRecordAbstract
    {

        public JRecordProperty(IDataRecord record)
            : base(record)
        {
        }

        public override void ApplyTo(JObject parent)
        {
            JObject propertyObject = new JObject();
            base.ApplyTo(propertyObject);
            if (propertyObject.HasValues)
                parent.Add(JsonPropertyName(), propertyObject);
        }

        public override bool IsNotPartOfCollection()
        {
            return true;
        }
    }
}
