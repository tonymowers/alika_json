using System.Data;

namespace CH.Alika.Json.Server.Model
{
    internal class RecordFactory : IRecordFactory
    {
        public IRecord Create(IDataRecord record)
        {
            IRecord result;
            switch (GetRecordType(record))
            {
                case RecordType.Options:
                    result = CreateOptions(record);
                    break;
                case RecordType.ArrayElement:
                    result = CreateArrayElement(record);
                    break;
                case RecordType.NewObject:
                    result = CreateNewObjectRecord(record);
                    break;
                default:
                    result = CreateSelfRecord(record);
                    break;
            }

            return result;
        }

        public IRecord CreateArrayElement(IDataRecord record)
        {
            return new RecordArrayElement(record);
        }

        private IRecord CreateOptions(IDataRecord record)
        {
            return new RecordOptions(record);
        }

        private IRecord CreateNewObjectRecord(IDataRecord record)
        {
            return new RecordNewObject(record);
        }

        private IRecord CreateSelfRecord(IDataRecord record)
        {
            return new RecordSelf(record);
        }

        private static RecordType GetRecordType(IDataRecord record)
        {
            if (IsOptionsRecord(record))
            {
                return RecordType.Options;
            }

            if (IsNewObjectRecord(record))
            {
                return RecordType.NewObject;
            }

            if (IsArrayElementRecord(record))
            {
                return RecordType.ArrayElement;
            }

            return RecordType.Self;
        }

        private static bool IsOptionsRecord(IDataRecord record)
        {
            return record.GetName(0).Equals("_") &&
                   record["_"] != null &&
                   record["_"].ToString().Equals("+");
        }

        private static bool IsNewObjectRecord(IDataRecord record)
        {
            if ("_".Equals(record.GetName(0)) && record["_"] != null)
            {
                var objectName = record["_"].ToString();
                return objectName.Length > 0 &&
                       !objectName.EndsWith("[]") &&
                       !".".Equals(objectName);
            }

            return false;
        }

        private static bool IsArrayElementRecord(IDataRecord record)
        {
            return
                record.GetName(0).Equals("_") &&
                record["_"] != null &&
                record["_"].ToString().EndsWith("[]");
        }

        private enum RecordType
        {
            NewObject,
            Self,
            ArrayElement,
            Options
        }
    }
}