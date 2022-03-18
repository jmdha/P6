using Npgsql;
using System.Data;
using System.Diagnostics;
using Tools.Models;

namespace DatabaseConnector.Connectors
{
    public class PostgreSqlConnector : BaseDbConnector<NpgsqlConnection, NpgsqlCommand, NpgsqlDataAdapter>
    {
        public PostgreSqlConnector(ConnectionProperties connectionProperties) : base(connectionProperties)
        {

        }

        public override string BuildConnectionString()
        {
            string newStr = ConnectionProperties.ConnectionString;
            if (!newStr.EndsWith(";"))
                newStr += ";";
            return $"{newStr}Database={ConnectionProperties.Database};SearchPath={ConnectionProperties.Schema}";
        }

        public override async Task<DataSet> AnalyseQuery(string query)
        {
            if (!query.ToUpper().Trim().StartsWith("EXPLAIN ANALYZE "))
                return await CallQuery($"EXPLAIN ANALYZE {query}");
            return await CallQuery(query);
        }

        public override async Task<bool> StartServer()
        {
            string posGresPath = GetPostgresPath();
            if (Directory.Exists(@$"{posGresPath}\bin\"))
            {
                Process.Start(@$"{posGresPath}\bin\pg_ctl.exe", $"-D \"{posGresPath}\\data\" start");
                int retryCount = 0;
                while (retryCount < 10)
                {
                    await Task.Delay(1000);
                    if (CheckConnection())
                    {
                        await Task.Delay(10000);
                        return true;
                    }
                    retryCount++;
                }
                return false;
            }
            else
                throw new Exception("Error, PostGres server installation was not found!");
        }

        public override bool StopServer()
        {
            string posGresPath = GetPostgresPath();
            if (Directory.Exists(@$"{posGresPath}\bin\"))
            {
                Process.Start(@$"{posGresPath}\bin\pg_ctl.exe", $"-D \"{posGresPath}\\data\" stop");
                return true;
            }
            else
                throw new Exception("Error, PostGres server installation was not found!");
        }

        private string GetPostgresPath()
        {
            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                if (Directory.Exists(@$"{drive.Name}Program Files\PostgreSQL"))
                    return GetPostgresVersion(@$"{drive.Name}Program Files\PostgreSQL");
            }
            return "Not Found";
        }

        private string GetPostgresVersion(string path)
        {
            for (int i = 0; i < 100; i++)
            {
                if (Directory.Exists($"{path}\\{i}"))
                    return $"{path}\\{i}";
            }
            return "Not Found";
        }
    }
}
