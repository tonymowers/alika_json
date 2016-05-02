using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Collections;

namespace CH.Alika.Json.Server.Model
{
    abstract class JRecordAbstract : JRecord
    {
        private IDataRecord record;

        private IFieldNameTranslator fieldNameXlator = new DefaultFieldNameTranslator();

        public JRecordAbstract(IDataRecord record)
        {
            this.record = record;
        }

        public virtual void ApplyTo(IDataContainer parent)
        {
            for (int i = 0; i < record.FieldCount; i++)
            {
                if (fieldNameXlator.IsIgnoredFieldForJson(record,i))
                    continue;

                parent.AddProperty(record.GetName(i), record.GetValue(i));
            }
        }

        public abstract bool IsNotPartOfCollection();

        protected string JsonPropertyName()
        {
            return fieldNameXlator.JsonPropertyName(record);   
        }
 
    }
}
