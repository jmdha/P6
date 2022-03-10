using DatabaseConnector;
using DatabaseConnector.Connectors;
using QueryParser;
using QueryParser.QueryParsers;
using QueryPlanParser.Parsers;
using QueryTestSuite.Models;
using QueryTestSuite.Services;
using QueryTestSuite.TestRunners;

namespace QueryTestSuite
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Task.WaitAll(AsyncMain(args));
        }

        async static Task AsyncMain(string[] args)
        {
            SecretsService secrets = new SecretsService();

            var postgresModel = new DBConnectorParser(
                "postgre",
                new PostgreSqlConnector(secrets.GetConnectionString("POSGRESQL")),
                new PostgreSqlParser());
            var mySqlModel = new DBConnectorParser(
                "mysql",
                new DatabaseConnector.Connectors.MySqlConnector(secrets.GetConnectionString("MYSQL")),
                new MySQLParser());

            var connectorSet = new List<DBConnectorParser>() { postgresModel };


            string testBaseDirPath = Path.GetFullPath("../../../Tests");

            DateTime timeStamp = DateTime.Now;

            foreach (DirectoryInfo testDir in new DirectoryInfo(testBaseDirPath).GetDirectories())
            {
                TestSuite suite = new TestSuite(connectorSet, timeStamp);

                Console.WriteLine($"Running Collection: {testDir}");
                await suite.RunTests(testDir);
            }
        }
    }
}
