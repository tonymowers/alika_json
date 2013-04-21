using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using CH.Alika.Json.Shared.Model;
using CH.Alika.Json.Server.Model;
using System.Xml;
using Newtonsoft.Json;
using System.Data.SqlTypes;
using System.IO;

/*
 * 
 * 
 * Below is an example of how to read an XML parameter in an Sql stored procedure 

DECLARE @filters xml 
SET @filters= '<root><filters ><comparison>equal</comparison><field>StatusID</field><type>string</type><value>2</value></filters><filters ><comparison /><field /><type /><value>1</value></filters></root>'

select  
   filter.value('comparison[1]','varchar(50)') AS comparision
,  filter.value('field[1]','varchar(50)') AS field
,  filter.value('type[1]','varchar(50)') AS type
,  filter.value('value[1]','varchar(50)') AS value
from @filters.nodes('/root/filters')  AS R(filter)
 
 */

namespace CH.Alika.Json.Server
{
    class JsonStoredProcParam : IStoredProcParam
    {
        private string name;
        private string type;
        private JToken value;

        public JsonStoredProcParam(string name, string type, JToken value)
        {
            this.name = name;
            this.type = type;
            this.value = value;
        }


        public void AddParam(SqlCommand cmd)
        {

            switch (type)
            {
                case "xml":
                    SqlXml sqlXml = JsonParamValueToSqlXml();
                    cmd.Parameters.Add(name, SqlDbType.Xml).Value = sqlXml;
                    break;
                default:
                    cmd.Parameters.Add(name, SqlDbType.VarChar).Value = value.Value<String>();
                    break;
            }

        }

        private SqlXml JsonParamValueToSqlXml()
        {
            JObject jObject = new JObject();
            jObject.Add(name.Substring(1, 1).ToLowerInvariant() + name.Substring(2), value);
            XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(jObject.ToString(), "root", true);
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            doc.WriteTo(xw);
            StringReader transactionXml = new StringReader(sw.ToString());
            XmlTextReader xmlReader = new XmlTextReader(transactionXml);
            SqlXml sqlXml = new SqlXml(xmlReader);
            return sqlXml;
        }

    }
}
