using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CH.Alika.Json.Server.Model
{
    interface IFieldNameTranslator
    {
        // stored procecure variable names are converted to a Json path
        // and so the path can be used to search for the data in the
        // inbound Json Request message
        string StProcVariableNameToJsonPath(string name);

        // data base column names are converted to Json paths
        // and those paths are used to put record data into Json
        // outbound json messages
        string[] RecordFieldNameToJsonPath(string fieldName);

        // Json property name into which the record
        // data should be applied to in the Json outbound message
        string JsonPropertyName(IDataRecord record);

        // returns true if the data record field should not
        // go into the outbound Json message
        bool IsIgnoredFieldForJson(IDataRecord record, int i);
    }
}
