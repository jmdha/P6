using DatabaseConnector;
using DatabaseConnector.Connectors;
using PrintUtilities;
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

            PrintUtil.PrintLine($"Checking if databases are online...", 0, ConsoleColor.DarkGray);
            bool allGood = true;
            foreach(var connector in connectorSet)
            {
                if (connector.Connector.CheckConnection())
                {
                    PrintUtil.PrintLine($"Connector [{connector.Name}] is online!", 0, ConsoleColor.Green);
                }
                else
                {
                    PrintUtil.PrintLine($"Error! Connector [{connector.Name}] is not online!", 0, ConsoleColor.Red);
                    PrintUtil.PrintLine($"Attempting to start...", 0, ConsoleColor.Yellow);

                    if (!await connector.Connector.StartServer())
                    {
                        PrintUtil.PrintLine($"Error! Connector [{connector.Name}] could not start!", 0, ConsoleColor.Red);
                        allGood = false;
                        break;
                    }
                }
            }

            if (allGood)
            {
                string testBaseDirPath = Path.GetFullPath("../../../Tests");

                DateTime timeStamp = DateTime.Now;

                foreach (DirectoryInfo testDir in new DirectoryInfo(testBaseDirPath).GetDirectories())
                {
                    TestSuite suite = new TestSuite(connectorSet, timeStamp);

                    PrintUtil.PrintLine($"Running test collection [{testDir.Name}]", 0, ConsoleColor.Magenta);
                    await suite.RunTests(testDir);
                    PrintUtil.PrintLine($"Test collection [{testDir.Name}] finished!", 0, ConsoleColor.Magenta);
                }

                foreach (var connector in connectorSet)
                    connector.Connector.StopServer();
            }
        }
    }
}
