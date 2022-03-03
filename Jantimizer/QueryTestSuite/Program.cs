using QueryTestSuite.Connectors;
using QueryTestSuite.Models;
using QueryTestSuite.Parsers;
using System.Data;

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
            var postgresModel = new DatabaseCommunicator(
                "Postgres",
                new PostgreSqlConnector("Host=localhost;Username=postgres;Password=password;Database=postgres"),
                new PostgreSqlParser());
            var mySqlModel = new DatabaseCommunicator(
                "MySQL",
                new Connectors.MySqlConnector("Host=localhost;Username=Jantimizer;Password=Aber1234"),
                new MySQLParser());

            var connectorSet = new List<DatabaseCommunicator>() { postgresModel };


            string testBaseDirPath = Path.GetFullPath("../../../Tests");

            foreach (DirectoryInfo testDir in new DirectoryInfo(testBaseDirPath).GetDirectories())
            {
                TestSuite suite = new TestSuite(connectorSet);

                Console.WriteLine($"Running Collection: {testDir}");
                await suite.RunTests(testDir);
            }

        }
    }
}

