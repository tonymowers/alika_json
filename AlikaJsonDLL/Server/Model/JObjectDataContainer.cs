using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace CH.Alika.Json.Server.Model
{
    class JObjectDataContainer : IDataContainer
    {
        private readonly JObject _jobject;
        private readonly IFieldNameTranslator _fieldNameXlator = new DefaultFieldNameTranslator();

        public JObjectDataContainer()
        {
            _jobject = new JObject();
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
                JArray array = GetArray(name);
                array.Add(container._jobject);
            }
        }

        public void AddObject(string name, IDataContainer data)
        {
            String objectName = _fieldNameXlator.RecordFieldNameToJsonPath(name).Last();
            JObjectDataContainer container = (JObjectDataContainer)data;
            GetContainer(name).Add(objectName, container._jobject);
        }

        public void AddProperty(string name, object value)
        {
            String propertyName = _fieldNameXlator.RecordFieldNameToJsonPath(name).Last();
            AddPropertyTo(GetContainer(name), new JProperty(propertyName, value));
        }

        private JObject GetContainer(string name)
        {
            JObject container = _jobject;
            String[] path = _fieldNameXlator.RecordFieldNameToJsonPath(name);
            int i = 1;
            while (i < path.Length)
            {
                JToken token = _jobject.SelectToken(path[i - 1]);
                if (token == null)
                {
                    JObject newContainer = new JObject();
                    if (container != null) container.Add(path[i - 1], newContainer);
                    container = newContainer;
                }
                else
                {
                    container = token as JObject;
                }
                i++;
            }
            return container;
        }

        // Adds a property to a container
        // replaces any existing property with the new one
        private void AddPropertyTo(JContainer container, JProperty newProperty)
        {
            JProperty oldProperty = null;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (JToken el in container.Children())
            {
                if (!(el is JProperty))
                    continue;

                JProperty p = el as JProperty;

                if (newProperty.Name.Equals(p.Name))
                {
                    oldProperty = p;
                    break;
                }
            }
            if (oldProperty != null)
                oldProperty.Remove();

            container.Add(newProperty);
        }

        private JArray GetArray(String name)
        {
            String arrayName = _fieldNameXlator.RecordFieldNameToJsonPath(name).Last();
            JObject container = GetContainer(name);
            JToken token = container.SelectToken(arrayName);
            if (token == null)
            {
                JArray array = new JArray();
                container.Add(arrayName, array);

                return array;
            }

            if (token is JArray)
            {
                return token as JArray;
            }

            throw new Exception("Attempt to add array element to non-array property [" + name + "]");
        }
    }
}
