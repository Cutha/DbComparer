using System;
using System.Collections.Generic;
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
        private Table srcTable = new Table();
        private Table destTable = new Table();

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
        /// <param name="sourceTable">The name of the source table without schema.</param>
        /// <param name="destinationTable">The name of the destination table without schema.</param>
        public void Compare(string sourceTable, string destinationTable)
        {
            SqlCommand cmd;
            using (cmd = new SqlCommand(@"
                select t.object_id, s.name schema_name from sys.tables t
                left join sys.schemas s on t.schema_id = s.schema_id
                where t.name = @SRCTABLE", srcConn))
            {
                cmd.Parameters.AddWithValue("SRCTABLE", sourceTable);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        srcTable.ObjectId = reader.GetInt32(reader.GetOrdinal("object_id"));
                        srcTable.Schema = reader.GetString(reader.GetOrdinal("schema_name"));
                    }
                }
            }
            Console.WriteLine("Table \"" + sourceTable + "\" -");
            Console.WriteLine("ObjectId: " + srcTable.ObjectId);
            Console.WriteLine("Schema: " + srcTable.Schema);
        }

        /// <summary>
        /// Closes both the source and destination connections.
        /// </summary>
        public void Dispose()
        {
            if (IsConnected)
            {
                Console.WriteLine("Closing connections...");
                srcConn.Close();
                destConn.Close();
                Console.WriteLine("Connections closed.");
            }
        }
    }
}
