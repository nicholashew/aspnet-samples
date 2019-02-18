using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;

namespace ConsoleAppBoilerPlate.Helper
{
    public class SqlHelper
    {
        private static SqlConnection GetConn()
        {
            string connection_string = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connection_string);
            if (conn.State != ConnectionState.Open) conn.Open();
            return conn;
        }

        public static DataSet Fill(string spName, List<SqlParameter> sqlparam)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = GetConn())
            {
                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.CommandText = spName;
                    foreach (SqlParameter parameter in sqlparam)
                    {
                        comm.Parameters.Add(parameter);
                    }
                    using (SqlDataAdapter da = new SqlDataAdapter())
                    {
                        da.SelectCommand = comm;
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }

        public static DataTable Execute(string spName, List<SqlParameter> sqlparam = null)
        {
            DataSet ds = Fill(spName, sqlparam);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        public static string ExecuteXmlRead(string spName, params SqlParam[] parameters)
        {
            string results = "";
            using (SqlConnection conn = GetConn())
            {
                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.CommandText = spName;
                    foreach (SqlParam parameter in parameters)
                    {
                        comm.Parameters.Add(parameter.GetSqlParameter());
                    }

                    System.Xml.XmlReader xmlr = comm.ExecuteXmlReader();

                    xmlr.Read();

                    while (xmlr.ReadState != System.Xml.ReadState.EndOfFile)
                    {
                        results = results + xmlr.ReadOuterXml();
                    }
                }
            }

            return results;
        }

        public static bool RunNonQuery(string spName, params SqlParam[] parameters)
        {
            int ret = 0;
            using (SqlConnection conn = GetConn())
            {
                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.CommandText = spName;
                    foreach (SqlParam parameter in parameters)
                    {
                        comm.Parameters.Add(parameter.GetSqlParameter());
                    }
                    ret = comm.ExecuteNonQuery();
                }
            }
            return ret > 0;
        }

        public static object ExecuteScalar(string spName, List<SqlParameter> sqlparam)
        {
            object ret = null;
            using (SqlConnection conn = GetConn())
            {
                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.CommandText = spName;
                    foreach (SqlParameter parameter in sqlparam)
                    {
                        comm.Parameters.Add(parameter);
                    }
                    ret = comm.ExecuteScalar();
                }
            }
            return ret;
        }

        #region Nested type: SqlParam
        public class SqlParam
        {
            public ParameterDirection Direction;
            public string Name;
            public int Size;
            public object Value;

            public SqlParam(string name, object value)
            {
                Value = value;
                Name = name;
                Direction = ParameterDirection.Input;
            }

            public SqlParam(string name, object value, ParameterDirection direction)
            {
                Value = value;
                Name = name;
                Direction = direction;
            }

            public SqlParam(string name, object value, ParameterDirection direction, int size)
            {
                Value = value;
                Name = name;
                Direction = direction;
                Size = size;
            }

            public SqlDbType GetDBType
            {
                get { return GetSqlDbType(Value); }
            }

            private SqlDbType GetSqlDbType(object obj)
            {
                if (obj is string) return SqlDbType.NVarChar;
                if (obj is DateTime) return SqlDbType.DateTime;
                if (obj is bool) return SqlDbType.Bit;
                if (obj is int) return SqlDbType.Int;
                if (obj is double) return SqlDbType.Money;
                if (obj is long) return SqlDbType.BigInt;
                if (obj is Guid) return SqlDbType.UniqueIdentifier;
                return SqlDbType.NVarChar;
            }

            public SqlParameter GetSqlParameter()
            {
                SqlParameter param = new SqlParameter(Name, GetSqlDbType(Value));
                param.Value = Value;
                param.Direction = Direction;
                if (param.SqlDbType == SqlDbType.NVarChar || Size > 0)
                {
                    param.Size = Size;
                }
                return param;
            }
        }
        #endregion
    }
}
