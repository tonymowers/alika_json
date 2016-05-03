using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CH.Alika.Json.Server.Model
{
    class RecordOptions : IRecord, IOptions
    {
        private IDataRecord _record;

        public RecordOptions(IDataRecord record)
        {
            _record = record;
        }
        public void ApplyTo(IDataContainer parent)
        {
            throw new NotImplementedException();
        }

        public bool IsNotPartOfCollection()
        {
            throw new NotImplementedException();
        }
    }
}
