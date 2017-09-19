using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using DbComparer;

namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Config();
            string srcConnStr = "";
            string srcTable = "";
            string destConnStr = "";
            string destTable = "";
            if (config.IsLoaded)
            {
                srcConnStr = config.SourceConnectionString;
                srcTable = config.SourceTable;
                destConnStr = config.DestinationConnectionString;
                destTable = config.DestinationTable;
            }
            else
            {
                Console.WriteLine("Config load error");
                return;
            }
            using (TableComparer tc = new TableComparer(srcConnStr, destConnStr))
            {
                tc.Connect();
                TableCompareResult result = tc.Compare(srcTable, destTable);
                Console.WriteLine(result.IsEqual);
            }
            Table table = new Table();
        }
    }

    class Config
    {
        public string SourceConnectionString { get; set; }
        public string SourceTable { get; set; }
        public string DestinationConnectionString { get; set; }
        public string DestinationTable { get; set; }
        public bool IsLoaded { get; private set; }
        private string folderPath = "";

        public Config()
        {
            folderPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            Load();
        }

        public Config(string folderPath)
        {
            this.folderPath = folderPath;
            Load();
        }

        private void Load()
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            string json = "";
            try
            {
                json = File.ReadAllText(Path.Combine(folderPath, "config.json"));
            }
            catch
            {
                return;
            }
            try
            {
                dynamic obj = jsonSerializer.DeserializeObject(json);
                var srcNode = obj["db"]["source"];
                var destNode = obj["db"]["destination"];
                SourceConnectionString = srcNode["connectionString"];
                SourceTable = srcNode["table"];
                DestinationConnectionString = destNode["connectionString"];
                DestinationTable = destNode["table"];
            }
            catch
            {
                return;
            }
            IsLoaded = true;
        }
    }
}
