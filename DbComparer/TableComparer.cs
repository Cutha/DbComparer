using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbComparer
{
    public class TableComparer : IDisposable
    {
        /// <summary>
        /// The connection string for the source server, which contains the table to change to.
        /// </summary>
        public string SourceConnectionString { get; set; }
        /// <summary>
        /// The connection string for the destination server, which contains the table to be changed.
        /// </summary>
        public string DestinationConnectionString { get; set; }
        /// <summary>
        /// Indicates whether both source and destination servers are connected.
        /// </summary>
        public bool IsConnected { get; private set; }

        private SqlConnection srcConn;
        private SqlConnection destConn;
        private Table srcTable;
        private Table destTable;

        /// <summary>
        /// Initializes a new instance of the DbComparer.TableComparer class.
        /// </summary>
        public TableComparer()
        {
            // default ctor
        }

        /// <summary>
        /// Initializes a new instance of the DbComparer.TableComparer class with the specified source and destination connection strings.
        /// </summary>
        /// <param name="sourceConnectionString">The connection string for the source server, which contains the table to change to.</param>
        /// <param name="destinationConnectionString">The connection string for the destination server, which contains the table to be changed.</param>
        public TableComparer(string sourceConnectionString, string destinationConnectionString)
        {
            SourceConnectionString = sourceConnectionString;
            DestinationConnectionString = destinationConnectionString;
        }

        /// <summary>
        /// Connects to both the source and destination servers. The IsConnected property will be set to indicate whether the connections are successful.
        /// </summary>
        public void Connect()
        {
            try
            {
                srcConn = new SqlConnection(SourceConnectionString);
                destConn = new SqlConnection(DestinationConnectionString);
                srcConn.Open();
                destConn.Open();
                IsConnected = true;
            }
            catch (Exception)
            {
                IsConnected = false;
            }
        }

        /// <summary>
        /// Compares the source and destination tables.
        /// </summary>
        /// <param name="srcTableName">The name of the source table (with schema if not dbo), e.g "admin.Employees".</param>
        /// <param name="destTableName">The name of the destination table (with schema if not dbo), e.g "admin.Employees".</param>
        public string Compare(string srcTableName, string destTableName)
        {
            Task fetchSrcTable = Task.Run(() => GetTableInfo(srcTableName, srcConn, ref srcTable));
            Task fetchDestTable = Task.Run(() => GetTableInfo(destTableName, destConn, ref destTable));
            Task.WaitAll(fetchSrcTable, fetchDestTable);
            string result = "";
            if (srcTable.Equals(destTable))
            {
                result = "Tables are the same.";
            }
            else
            {
                result = srcTable.Diff(destTable);
            }
            return result;
        }

        private void GetTableInfo(string tableName, SqlConnection conn, ref Table table)
        {
            SqlCommand cmd = new SqlCommand(@"
                select s.name schema_name, t.uses_ansi_nulls from sys.tables t
                left join sys.schemas s on t.schema_id = s.schema_id
                where t.object_id = OBJECT_ID(@TABLE)");
            cmd.Parameters.Add("@TABLE", SqlDbType.NVarChar);
            cmd.Parameters["@TABLE"].Value = tableName;
            List<Column> columnList = new List<Column>();
            cmd.Connection = conn;
            using (cmd)
            {
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    table = new Table();
                    if (reader.Read())
                    {
                        table.Schema = reader.GetString(reader.GetOrdinal("schema_name"));
                        table.UsesAnsiNulls = reader.GetBoolean(reader.GetOrdinal("uses_ansi_nulls"));
                    }
                }

                // Get Columns info
                cmd.CommandText = @"select c.name, c.user_type_id, ut.name user_type, c.system_type_id, st.name system_type, c.max_length, c.precision, c.is_nullable, c.is_identity from sys.columns c
                    inner join sys.types ut on c.user_type_id = ut.user_type_id
                    inner join sys.types st on c.system_type_id = st.user_type_id
                    where c.object_id = OBJECT_ID(@TABLE)";
                reader.Close();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Column column = new Column();
                        column.Name = reader.GetString(reader.GetOrdinal("name"));
                        column.Type = reader.GetString(reader.GetOrdinal("user_type"));
                        column.SystemType = reader.GetString(reader.GetOrdinal("system_type"));
                        column.MaxLength = reader.GetInt16(reader.GetOrdinal("max_length"));
                        column.Precision = reader.GetByte(reader.GetOrdinal("precision"));
                        column.IsNullable = reader.GetBoolean(reader.GetOrdinal("is_nullable"));
                        column.IsIdentity = reader.GetBoolean(reader.GetOrdinal("is_identity"));
                        columnList.Add(column);
                    }
                }
            }
            ColumnCollection columnCollection = new ColumnCollection(columnList);
            table.Columns = columnCollection;
        }

        /// <summary>
        /// Closes both the source and destination connections.
        /// </summary>
        public void Dispose()
        {
            if (IsConnected)
            {
                srcConn.Close();
                destConn.Close();
            }
        }
    }
}
