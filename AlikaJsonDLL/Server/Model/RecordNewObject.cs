using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Data;

namespace CH.Alika.Json.Server.Model
{
    class RecordNewObject : RecordAbstract
    {

        public RecordNewObject(IDataRecord record)
            : base(record)
        {
        }

        public override void ApplyTo(IDataContainer parent)
        {
            IDataContainer newObject = parent.CreateObject(JsonPropertyName());
            base.ApplyTo(newObject);
            parent.AddObject(newObject);
        }

        public override bool IsNotPartOfCollection()
        {
            return true;
        }
    }
}
