using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CH.Alika.Json.Server.Model
{
    class DefaultFieldNameTranslator : IFieldNameTranslator
    {
        public string StProcVariableNameToJsonPath(string path)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirstChar = true;
            foreach (char c in path)
            {
                if ('@'.Equals(c))
                    continue;

                if ('_'.Equals(c))
                {
                    sb.Append('.');
                    isFirstChar = true;
                    continue;
                }

                if (isFirstChar)
                    sb.Append(Char.ToLowerInvariant(c));
                else
                    sb.Append(c);

                isFirstChar = false;
            }

            return sb.ToString();
        }

        public String RecordFieldNameToPropertyName(string path)
        {
            return RecordFieldNameToJsonPath(path).Last();
        }

        public String[] RecordFieldNameToJsonPath(string path)
        {
            String[] split = path.Split('_');

            for (int i = 0; i < split.Length; i++)
            {
                split[i] = CamelCase(split[i]);
            }

            return split;
        }

        public string JsonPropertyName(IDataRecord record)
        {
            return record["_"].ToString().Replace("[]", "");
        }

        public bool IsIgnoredFieldForJson(IDataRecord record, int i)
        {
            if (IsJsonPropertyNameField(record.GetName(i)))
                return true;

            return DBNull.Value.Equals(record.GetValue(i));
        }


        private bool IsJsonPropertyNameField(string fieldName)
        {
            return "_".Equals(fieldName);
        }

        /*
         * Converts property names like the following examples
         *    camelCase -> camelCase
         *    CamelCase -> camelCase
         *    ID -> ID
         *    A -> a
         */ 
        private static string CamelCase(string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
                return str;

            if (str.Length > 1 && Char.IsUpper(str, 1))
                return str;

            return Char.ToLowerInvariant(str[0]).ToString() + str.Substring(1);
        }

    }
}
