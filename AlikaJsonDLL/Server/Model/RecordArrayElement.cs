using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newtonsoft.Json.Linq;

namespace CH.Alika.Json.Server.Model
{
    class RecordArrayElement : RecordAbstract
    {
        public RecordArrayElement(IDataRecord record)
            : base(record)
        {
           
        }

        public override void ApplyTo(IDataContainer parent)
        {
            IDataContainer container = parent.CreateArrayElement(JsonPropertyName());
            base.ApplyTo(container);
            parent.AddObject(container);
        }

        public override bool IsNotPartOfCollection()
        {
            return false;
        }
    }
}
