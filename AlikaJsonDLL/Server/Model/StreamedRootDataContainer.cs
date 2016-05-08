using System;
using System.IO;
using Newtonsoft.Json;

namespace CH.Alika.Json.Server.Model
{
    internal class StreamedRootDataContainer : IDataContainer
    {
        private readonly IFieldNameTranslator _fieldNameXlator = new DefaultFieldNameTranslator();
        private readonly JsonTextWriter _writer;
        private string _arrayName;
        private readonly bool _isArray;

        public StreamedRootDataContainer(TextWriter textWriter, bool isArray = false)
        {
            _isArray = isArray;
            if (textWriter != null)
            {
                _writer = new JsonTextWriter(textWriter) {Formatting = Formatting.None};
                if (_isArray)
                    _writer.WriteStartArray();
                else
                    _writer.WriteStartObject();
            }
        }

        private StreamedRootDataContainer(JsonTextWriter textWriter)
        {
            if (textWriter != null)
            {
                _writer = textWriter;
                _writer.WriteStartObject();
            }
        }

        public void AddObject(IDataContainer container)
        {
            container.End();
        }

        public void AddProperty(string name, object value)
        {
            if (_isArray)
                throw new NotSupportedException("cannot add properties directly to JSON arrays"); 

            CloseArray();
            if (_writer != null)
            {
                _writer.WritePropertyName(PropertyName(name));
                _writer.WriteValue(value);
            }
        }

        public IDataContainer CreateArrayElement(string name)
        {
            if (_writer != null && !name.Equals(_arrayName) && !_isArray)
            {
                CloseArray();
                _arrayName = name;
                _writer.WritePropertyName(PropertyName(name));
                _writer.WriteStartArray();
            }

            return new StreamedRootDataContainer(_writer);
        }

        public IDataContainer CreateObject(string name)
        {
            if (_isArray)
                throw new NotSupportedException("cannot add nameed objects directly to JSON arrays"); 

            if (_writer != null)
            {
                _writer.WritePropertyName(PropertyName(name));
            }

            return new StreamedRootDataContainer(_writer);
        }

        public object ObjectRepresentation
        {
            get { throw new NotSupportedException(); }
        }

        public bool IsSerializable
        {
            get { return false; }
        }

        public void End()
        {
            if (_writer != null)
            {
                CloseArray();
                if (_isArray)
                    _writer.WriteEndArray();
                else
                    _writer.WriteEndObject();
            }
        }

        private void CloseArray()
        {
            if (_isArray) return;

            if (_arrayName != null && _writer != null)
            {
                _writer.WriteEndArray();
                _arrayName = null;
            }
        }

        private string PropertyName(string name)
        {
            return _fieldNameXlator.RecordFieldNameToPropertyName(name);
        }
    }
}