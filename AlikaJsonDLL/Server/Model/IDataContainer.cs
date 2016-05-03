using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CH.Alika.Json.Server.Model
{
    interface IDataContainer
    {
        void AddObject(string name, IDataContainer container);
        void AddProperty(string name, object value);
        void AddToArray(string name, IDataContainer value);

        IDataContainer CreateObject();
        bool HasValues
        {
            get;
        }

        object ObjectRepresentation { get; }
    }
}
