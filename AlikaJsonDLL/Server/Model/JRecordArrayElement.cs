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

        public override void ApplyTo(IDataContainer parent)
        {
            IDataContainer container = parent.CreateObject();
            base.ApplyTo(container);
            parent.AddToArray(JsonPropertyName(), container);
        }

        public override bool IsNotPartOfCollection()
        {
            return false;
        }
    }
}
