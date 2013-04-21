using System;
using System.Linq;
using System.Data;

namespace CH.Alika.Json.Server.Model
{
    class JRecordFactory
    {
        private enum RecordType
        {
            SELF,
            PROPERTY,
            ARRAY_ELEMENT
        };

        public string GetJsonPathFromStprocVariableName(string name)
        {
            throw new NotImplementedException();
        }

        public bool IsPropertyNameField(string fieldName)
        {
            return "_".Equals(fieldName);
        }

        public String[] GetJsonPathFromRecordFieldName(string path)
        {
            String[] split = path.Split('_');

            for (int i = 0; i < split.Length; i++)
            {
                split[i] = FirstCharacterToLower(split[i]);
            }

            return split;
        }

        public string GetPropertyName(IDataRecord record)
        {
            return GetJsonPathFromRecordFieldName(record["_"].ToString().Replace("[]", "")).Last();
        }

        private static string FirstCharacterToLower(string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
                return str;

            return Char.ToLowerInvariant(str[0]).ToString() + str.Substring(1);
        }



        public bool IsIgnoredField(IDataRecord record, int i)
        {
            if (IsPropertyNameField(record.GetName(i)))
                return true;

            return DBNull.Value.Equals(record.GetValue(i));
        }
        public static JRecord create(IDataRecord record)
        {
            JRecord result;
            switch (GetRecordType(record))
            {
                case RecordType.ARRAY_ELEMENT:
                    result = new JRecordArrayElement(record);
                    break;
                case RecordType.PROPERTY:
                    result = new JRecordProperty(record);
                    break;
                default:
                    result = new JRecordSelf(record);
                    break;
            }

            return result;
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
