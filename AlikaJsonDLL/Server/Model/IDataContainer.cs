using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CH.Alika.Json.Server.Model
{
    interface IDataContainer
    {
        IDataContainer CreateObject(string name);
        IDataContainer CreateArrayElement(string name);
        void AddObject(IDataContainer container);
        void AddProperty(string name, object value);

        object ObjectRepresentation { get; }

        bool IsSerializable { get; }
        void End();
    }
}
