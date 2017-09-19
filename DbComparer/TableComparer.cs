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
            catch (Exception e)
            {
                IsConnected = false;
            }
        }

        /// <summary>
        /// Compares the source and destination tables.
        /// </summary>
        /// <param name="srcTableName">The name of the source table without schema.</param>
        /// <param name="destTableName">The name of the destination table without schema.</param>
        public TableCompareResult Compare(string srcTableName, string destTableName)
        {
            SqlCommand srcCmd = new SqlCommand(@"
                select s.name schema_name, t.uses_ansi_nulls from sys.tables t
                left join sys.schemas s on t.schema_id = s.schema_id
                where t.name = @TABLE");
            srcCmd.Parameters.Add("@TABLE", SqlDbType.NVarChar);
            srcCmd.Parameters["@TABLE"].Value = srcTableName;
            Task fetchSrcTable = Task.Run(() => GetTableInfo(srcCmd, srcConn, ref srcTable));
            SqlCommand destCmd = new SqlCommand(srcCmd.CommandText);
            destCmd.Parameters.Add("@TABLE", SqlDbType.NVarChar);
            destCmd.Parameters["@TABLE"].Value = destTableName;
            Task fetchDestTable = Task.Run(() => GetTableInfo(destCmd, destConn, ref destTable));
            Task.WaitAll(fetchSrcTable, fetchDestTable);
            TableCompareResult result;
            if (srcTable.Equals(destTable))
            {
                result = new TableCompareResult(true, null);
            }
            else
            {
                result = new TableCompareResult(false, null);
            }
            return result;
        }

        private void GetTableInfo(SqlCommand cmd, SqlConnection conn, ref Table table)
        {
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
            }
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
