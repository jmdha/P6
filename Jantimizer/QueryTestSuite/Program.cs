using DatabaseConnector;
using DatabaseConnector.Connectors;
using Histograms.Managers;
using PrintUtilities;
using QueryGenerator;
using QueryOptimiser;
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

            var postConnector = new PostgreSqlConnector(secrets.GetConnectionString("POSGRESQL"));
            var postPlanParser = new PostgreSqlParser();
            var postHistoManager = new PostgresEquiDepthHistogramManager(postConnector.ConnectionString, 10);
            var postOptimiser = new QueryOptimiserEquiDepth(postHistoManager);
            var postParserManager = new ParserManager(new List<IQueryParser>() { new JoinQueryParser() });
            var postGenerator = new PostgresGenerator();

            var postgresModel = new TestCase(
                "postgre",
                postConnector,
                postPlanParser,
                postHistoManager,
                postOptimiser,
                postParserManager,
                postGenerator);

            var connectorSet = new List<TestCase>() { postgresModel };

            if (await DatabaseStarter.CheckAndStartServers(connectorSet))
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

                DatabaseStarter.StopAllServers(connectorSet);
            }
        }
    }
}
