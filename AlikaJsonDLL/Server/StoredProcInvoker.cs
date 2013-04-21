using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using CH.Alika.Json.Shared.Model;
using CH.Alika.Json.Server.Model;

namespace CH.Alika.Json.Server
{
   
    class StoredProcInvoker
    {
        private string prefix;
        private string sp_proc_columns;

        public StoredProcInvoker(string prefix, string sp_proc_columns)
        {
            this.prefix = prefix;
            this.sp_proc_columns = sp_proc_columns;
        }

        public JObject invoke(SqlConnection connection, IStoredProcRequest request)
        {
            JObject response = new JObject();
            using (var cmd = StoredProcedureCmd(connection, request))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    do
                    {
                        ApplyResultToJObject(response, reader);
                    } while (reader.NextResult());
                }
            }

            return response;
        }

        private void ApplyResultToJObject(JObject response, SqlDataReader reader)
        {
            while (reader.Read())
            {
                JRecord jsonRecord = JRecordFactory.create(reader);
                jsonRecord.ApplyTo(response);
                if (jsonRecord.IsNotPartOfCollection())
                    break;
            }
        }


        private SqlCommand StoredProcedureCmd(SqlConnection connection, IStoredProcRequest request)
        {
            String procedureName = null;
            IList<IStoredProcParam> procParams = new List<IStoredProcParam>();
            using (var cmd = new SqlCommand
            {
                Connection = connection,
                CommandText = this.sp_proc_columns,
                CommandType = CommandType.StoredProcedure
            })
            {
                cmd.Parameters.Add("procedure_name", SqlDbType.VarChar).Value = StoredProcName(request);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        IStoredProcParamInfo paramInfo = new StoredProcParamInfo(reader);
                        procParams.Add(CreateStoredProcParam(request, paramInfo));
                        procedureName = paramInfo.ProcedureName;  // requested procedure can be mapped to another name
                    }
                    if (procParams.Count == 0)
                        throw new Exception(String.Format("no stored procedure information found for [{0}]", StoredProcName(request)));

                }
            }

            return StoredProc(connection, procedureName, procParams);
        }

        private String StoredProcName(IStoredProcRequest request)
        {
            return this.prefix + request.Method;
        }

        private static IStoredProcParam CreateStoredProcParam(IStoredProcRequest request, IStoredProcParamInfo stprocParamInfo)
        {
            IStoredProcParam storedProcParam = request.CreateStoredProcParam(stprocParamInfo);
            if (storedProcParam == null)
            {
                storedProcParam = new DoNothingParam();
            }

            return storedProcParam;
        }

        private static SqlCommand StoredProc(SqlConnection connection, string commandText, IList<IStoredProcParam> cmdParams)
        {
            SqlCommand cmd = new SqlCommand
            {
                CommandText = commandText,
                CommandType = CommandType.StoredProcedure,
                Connection = connection
            };

            foreach (IStoredProcParam p in cmdParams)
            {
                p.AddParam(cmd);
            }

            return cmd;
        }

        private class StoredProcParamInfo : IStoredProcParamInfo
        {
            private bool isInputParam;
            private string name;
            private string type;
            private string procedureName;

            public StoredProcParamInfo(IDataRecord record) 
            {
                const short INPUT_VALUE = 1;
                isInputParam = record["COLUMN_TYPE"].Equals(INPUT_VALUE);
                procedureName = record["PROCEDURE_NAME"].ToString().Split(';')[0];
                name = record["COLUMN_NAME"].ToString();
                type = record["TYPE_NAME"].ToString();
            }

            public string ProcedureName
            {
                get { return procedureName; }
            }
            public string Name
            {
                get { return name; }
            }

            public string Type
            {
                get { return type; }
            }

            public bool IsInputParam
            {
                get { return isInputParam; }
            }
        }

        private class DoNothingParam : IStoredProcParam
        {
            void IStoredProcParam.AddParam(System.Data.SqlClient.SqlCommand cmd)
            {

            }
        }
      
    }
}
