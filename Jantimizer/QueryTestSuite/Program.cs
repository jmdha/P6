using DatabaseConnector;
using DatabaseConnector.Connectors;
using Histograms.Managers;
using PrintUtilities;
using QueryGenerator;
using QueryGenerator.QueryGenerators;
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
            var postParserManager = new ParserManager(new List<IQueryParser>() { new PostgresParser(postConnector) });
            var postGenerator = new PostgresGenerator();
            var postgresModel = new SuiteData(
                "postgre",
                postConnector,
                postPlanParser,
                postHistoManager,
                postOptimiser,
                postParserManager,
                postGenerator);

            var mySQLConnector = new DatabaseConnector.Connectors.MySqlConnector(secrets.GetConnectionString("MYSQL"));
            var mySQLPlanParser = new MySQLParser();
            var mySQLHistoManager = new MySQLEquiDepthHistogramManager(mySQLConnector.ConnectionString, 10);
            var mySQLOptimiser = new QueryOptimiserEquiDepth(mySQLHistoManager);
            var mySQLParserManager = new ParserManager(new List<IQueryParser>() { new JoinQueryParser() });
            var mySQLGenerator = new MySQLGenerator();
            var mySQLModel = new SuiteData(
                "mysql",
                mySQLConnector,
                mySQLPlanParser,
                mySQLHistoManager,
                mySQLOptimiser,
                mySQLParserManager,
                mySQLGenerator);

            var connectorSet = new List<SuiteData>() { mySQLModel, postgresModel };

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
