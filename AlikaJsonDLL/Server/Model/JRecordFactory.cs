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
            ARRAY_ELEMENT,
            OPTIONS
        };

        
        public IRecord Create(IDataRecord record)
        {
            IRecord result;
            switch (GetRecordType(record))
            {
                case RecordType.OPTIONS:
                    result = CreateOptions(record);
                    break;
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

        private IRecord CreateOptions(IDataRecord record)
        {
            return new RecordOptions(record);
        }

        protected IRecord CreateArrayElement(IDataRecord record)
        {
            return new RecordProperty(record);
        }

        protected IRecord CreateProperty(IDataRecord record)
        {
            return new RecordArrayElement(record);
        }

        protected IRecord CreateSelfReference(IDataRecord record)
        {
            return new RecordSelf(record);
        }

        private static RecordType GetRecordType(IDataRecord record)
        {
            if (IsOptionsRecord(record))
            {
                return RecordType.OPTIONS;
            }

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

        private static bool IsOptionsRecord(IDataRecord record)
        {
            return record.GetName(0).Equals("_") &&
                record["_"] != null &&
                record["_"].ToString().Equals("+");
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
