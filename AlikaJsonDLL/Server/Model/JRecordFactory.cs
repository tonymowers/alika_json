using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace CH.Alika.Json.Server.Model
{
    class JRecordFactory : IRecordFactory
    {
        private enum RecordType
        {
            SELF,
            PROPERTY,
            ARRAY_ELEMENT
        };

        
        public JRecord create(IDataRecord record)
        {
            JRecord result;
            switch (GetRecordType(record))
            {
                case RecordType.ARRAY_ELEMENT:
                    result = CreateArrayElement(record);
                    break;
                case RecordType.PROPERTY:
                    result = CreateProperty(record);
                    break;
                default:
                    result = CreateSelfReference(record);
                    break;
            }

            return result;
        }

        protected JRecord CreateArrayElement(IDataRecord record)
        {
            return new JRecordProperty(record);
        }

        protected JRecord CreateProperty(IDataRecord record)
        {
            return new JRecordArrayElement(record);
        }

        protected JRecord CreateSelfReference(IDataRecord record)
        {
            return new JRecordSelf(record);
        }

        private static RecordType GetRecordType(IDataRecord record)
        {
            if (!IsNewObjectRecord(record))
            {
                return RecordType.SELF;
            }

            if (IsArrayElementRecord(record))
            {
                return RecordType.ARRAY_ELEMENT;
            }

            return RecordType.PROPERTY;
        }

        private static bool IsNewObjectRecord(IDataRecord record)
        {
            return record.GetName(0).Equals("_");
        }

        private static bool IsArrayElementRecord(IDataRecord record)
        {
            return
                record.GetName(0).Equals("_") &&
                record["_"] != null &&
                record["_"].ToString().EndsWith("[]");

        }


    }
}
