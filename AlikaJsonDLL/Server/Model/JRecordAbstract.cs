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

        public virtual void ApplyTo(JObject parent)
        {
            for (int i = 0; i < record.FieldCount; i++)
            {
                JContainer container;
                if (fieldNameXlator.IsIgnoredFieldForJson(record,i))
                    continue;

                String[] path = fieldNameXlator.RecordFieldNameToJsonPath(record.GetName(i));
                if (path.Length == 2)
                {
                    JToken token = parent.SelectToken(path[0]);
                    if (token == null)
                    {
                        container = new JObject();
                        parent.Add(path[0], container);
                    }
                    else
                    {
                        container = (JContainer)token;
                    }
                }
                else
                {
                    container = parent;
                }

                AddPropertyTo(container,  new JProperty(path.Last(), record.GetValue(i)));
            }
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
        public abstract bool IsNotPartOfCollection();

        protected string JsonPropertyName()
        {
            return fieldNameXlator.JsonPropertyName(record);   
        }


       
    }
}
