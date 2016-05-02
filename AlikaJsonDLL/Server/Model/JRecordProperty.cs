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

        public override void ApplyTo(IDataContainer parent)
        {
            IDataContainer propertyObject = parent.CreateObject();
            base.ApplyTo(propertyObject);
            if (propertyObject.HasValues)
                parent.AddObject(JsonPropertyName(), propertyObject);
        }

        public override bool IsNotPartOfCollection()
        {
            return true;
        }
    }
}
