using System.Data;

namespace CH.Alika.Json.Server.Model
{
    abstract class RecordAbstract : IRecord
    {
        private readonly IDataRecord _record;

        private readonly IFieldNameTranslator _fieldNameXlator = new DefaultFieldNameTranslator();

        protected RecordAbstract(IDataRecord record)
        {
            _record = record;
        }

        public virtual void ApplyTo(IDataContainer parent)
        {
            for (int i = 0; i < _record.FieldCount; i++)
            {
                if (_fieldNameXlator.IsIgnoredFieldForJson(_record,i))
                    continue;

                parent.AddProperty(_record.GetName(i), _record.GetValue(i));
            }
        }

        public abstract bool IsNotPartOfCollection();

        protected string JsonPropertyName()
        {
            return _fieldNameXlator.JsonPropertyName(_record);   
        }
 
    }
}
