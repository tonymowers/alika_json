using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Data;

namespace CH.Alika.Json.Server.Model
{
    class RecordSelf : RecordAbstract
    {
       
        public RecordSelf(IDataRecord record) 
            : base(record)
        {
          
        }

        public override bool IsNotPartOfCollection()
        {
            return false;
        }

    }
}
