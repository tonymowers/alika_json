using System;
using System.IO;
using Newtonsoft.Json;

namespace CH.Alika.Json.Server.Model
{
    internal class StreamedDataContainer : IDataContainer
    {
        private readonly IFieldNameTranslator _fieldNameXlator = new DefaultFieldNameTranslator();
        private readonly bool _isArray;
        private readonly bool _isArrayOfArray;
        private readonly JsonTextWriter _writer;
        private string _arrayName;

        public StreamedDataContainer(TextWriter textWriter, bool isArray = false, bool isArrayOfArray = false, bool isFormatted = false)
        {
            _isArrayOfArray = isArrayOfArray;
            _isArray = isArray || isArrayOfArray;

            _writer = new JsonTextWriter(textWriter)
            {
                Formatting = isFormatted ? Formatting.Indented : Formatting.None
            };
            if (_isArray)
                _writer.WriteStartArray();
            else
                _writer.WriteStartObject();
        }

        private StreamedDataContainer(JsonTextWriter textWriter, bool isArray = false)
        {
            _isArray = isArray;
            _writer = textWriter;
            if (_isArray)
                _writer.WriteStartArray();
            else
                _writer.WriteStartObject();
        }

        public void AddObject(IDataContainer container)
        {
            container.End();
        }

        public void AddProperty(string name, object value)
        {
            if (_isArray)
            {
                _writer.WriteValue(value);
            }
            else
            {
                CloseArray();
                _writer.WritePropertyName(PropertyName(name));
                _writer.WriteValue(value);
            }
        }

        public IDataContainer CreateArrayElement(string name)
        {
            if (!name.Equals(_arrayName) && !_isArray)
            {
                CloseArray();
                _arrayName = name;
                _writer.WritePropertyName(PropertyName(name));
                _writer.WriteStartArray();
            }

            return new StreamedDataContainer(_writer,_isArrayOfArray);
        }

        public IDataContainer CreateObject(string name)
        {
            if (_isArray)
                throw new NotSupportedException("cannot add named objects directly to JSON arrays");


            _writer.WritePropertyName(PropertyName(name));


            return new StreamedDataContainer(_writer);
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
            CloseArray();
            if (_isArray)
                _writer.WriteEndArray();
            else
                _writer.WriteEndObject();
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