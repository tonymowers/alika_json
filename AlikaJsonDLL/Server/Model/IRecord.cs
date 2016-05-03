using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace CH.Alika.Json.Server.Model
{
    interface IRecord
    {
        void ApplyTo(IDataContainer parent);
        bool IsNotPartOfCollection();
    }
}
