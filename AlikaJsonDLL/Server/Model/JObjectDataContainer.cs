using System;
using Newtonsoft.Json.Linq;

namespace CH.Alika.Json.Server.Model
{
    internal class JObjectDataContainer : IDataContainer
    {
        private readonly IFieldNameTranslator _fieldNameXlator = new DefaultFieldNameTranslator();
        private readonly bool _isArray;
        private readonly JObject _jobject;
        private readonly string _name;

        public JObjectDataContainer()
        {
            _name = null;
            _isArray = false;
            _jobject = new JObject();
        }

        private JObjectDataContainer(string name, bool isArray)
        {
            _isArray = isArray;
            _name = name;
            _jobject = new JObject();
        }

        private bool HasValues
        {
            get { return _jobject.HasValues; }
        }

        public object ObjectRepresentation
        {
            get { return _jobject; }
        }

        public bool IsSerializable
        {
            get { return true; }
        }

        public void End()
        {
            // nothing to do
        }

        public IDataContainer CreateArrayElement(string name)
        {
            return new JObjectDataContainer(name, true);
        }

        public IDataContainer CreateObject(string name)
        {
            return new JObjectDataContainer(name, false);
        }

        public void AddObject(IDataContainer data)
        {
            var container = (JObjectDataContainer) data;
            if (!container.HasValues)
                return;

            if (container._isArray)
            {
                var array = GetArray(container._name);
                array.Add(container._jobject);
            }
            else
            {
                var objectName = _fieldNameXlator.RecordFieldNameToPropertyName(container._name);
                GetContainer(container._name).Add(objectName, container._jobject);
            }
        }

        public void AddProperty(string name, object value)
        {
            var propertyName = _fieldNameXlator.RecordFieldNameToPropertyName(name);
            AddPropertyTo(GetContainer(name), new JProperty(propertyName, value));
        }

        private JObject GetContainer(string name)
        {
            var container = _jobject;
            var path = _fieldNameXlator.RecordFieldNameToJsonPath(name);
            var i = 1;
            while (i < path.Length)
            {
                var token = _jobject.SelectToken(path[i - 1]);
                if (token == null)
                {
                    var newContainer = new JObject();
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
            foreach (var el in container.Children())
            {
                if (!(el is JProperty))
                    continue;

                var p = el as JProperty;

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

        private JArray GetArray(string name)
        {
            var arrayName = _fieldNameXlator.RecordFieldNameToPropertyName(name);
            var container = GetContainer(name);
            var token = container.SelectToken(arrayName);
            if (token == null)
            {
                var array = new JArray();
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