using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace CH.Alika.Json.Server.Model
{
    class JObjectDataContainer : IDataContainer
    {
        private readonly JObject _jobject;
        private readonly IFieldNameTranslator _fieldNameXlator = new DefaultFieldNameTranslator();

        public JObjectDataContainer()
        {
            this._jobject = new JObject();
        }

        public JObject JObject
        {
            get { return _jobject; }
        }

        public bool HasValues 
        {
            get { return _jobject.HasValues; }
        }

        public object ObjectRepresentation
        {
            get { return _jobject; }
        }

        public IDataContainer CreateObject()
        {
            return new JObjectDataContainer();
        }

        public void AddToArray(string name, IDataContainer data)
        {
            JObjectDataContainer container = (JObjectDataContainer)data;
            if (container.HasValues)
            {
                JArray array = GetArray(_jobject, name);
                array.Add(container._jobject);
            }
        }

        public void AddObject(string name, IDataContainer data)
        {
            JObjectDataContainer container = (JObjectDataContainer)data;
            _jobject.Add(name, container._jobject);
        }

        public void AddProperty(string name, object value)
        {
            JContainer container;
            String[] path = _fieldNameXlator.RecordFieldNameToJsonPath(name);
            if (path.Length == 2)
            {
                JToken token = _jobject.SelectToken(path[0]);
                if (token == null)
                {
                    container = new JObject();
                    _jobject.Add(path[0], container);
                }
                else
                {
                    container = (JContainer)token;
                }
            }
            else
            {
                container = _jobject;
            }

            AddPropertyTo(container, new JProperty(path.Last(), value));
        }

        // Adds a property to a container
        // replaces any existing property with the new one
        private void AddPropertyTo(JContainer container, JProperty newProperty)
        {
            JProperty oldProperty = null;

            foreach (JToken el in container.Children())
            {
                if (!(el is JProperty))
                    continue;

                JProperty p = el as JProperty;

                if (p != null && newProperty.Name.Equals(p.Name))
                {
                    oldProperty = p;
                    break;
                }
            }
            if (oldProperty != null)
                oldProperty.Remove();

            container.Add(newProperty);
        }

        private static JArray GetArray(JObject parent, String objectName)
        {
            JArray array;
            JToken token = parent.SelectToken(objectName);
            if (token == null)
            {
                array = new JArray();
                parent.Add(objectName, array);

                return array;
            }

            if (token is JArray)
            {
                array = (JArray)token;
                return array;
            }

            throw new Exception("Attempt to add array element to none array property [" + objectName + "]");
        }
    }
}
