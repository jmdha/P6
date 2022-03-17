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
using Tools.Models;
using Tools.Services;

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
            SecretsService<Program> secrets = new SecretsService<Program>();

            string postConnectionString = secrets.GetConnectionString("POSGRESQL");
            var postConnectionProperties = new ConnectionProperties(
                postConnectionString,
                secrets.GetConnectionStringValue(postConnectionString, "Host"),
                int.Parse(secrets.GetConnectionStringValue(postConnectionString, "Port")),
                secrets.GetConnectionStringValue(postConnectionString, "Username"),
                secrets.GetConnectionStringValue(postConnectionString, "Password"),
                secrets.GetConnectionStringValue(postConnectionString, "Database"),
                secrets.GetConnectionStringValue(postConnectionString, "SearchPath"));
            var postConnector = new PostgreSqlConnector(postConnectionProperties);
            var postPlanParser = new PostgreSqlParser();
            var postHistoManager = new PostgresEquiDepthHistogramManager(postConnector.ConnectionProperties, 10);
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

            string mySQLConnectionString = secrets.GetConnectionString("MYSQL");
            var mySQLConnectionProperties = new ConnectionProperties(
                mySQLConnectionString,
                secrets.GetConnectionStringValue(mySQLConnectionString, "Server"),
                int.Parse(secrets.GetConnectionStringValue(mySQLConnectionString, "Port")),
                secrets.GetConnectionStringValue(mySQLConnectionString, "Uid"),
                secrets.GetConnectionStringValue(mySQLConnectionString, "Pwd"),
                secrets.GetConnectionStringValue(mySQLConnectionString, "Database"),
                secrets.GetConnectionStringValue(mySQLConnectionString, "Database"));
            var mySQLConnector = new DatabaseConnector.Connectors.MySqlConnector(mySQLConnectionProperties);
            var mySQLPlanParser = new MySQLParser();
            var mySQLHistoManager = new MySQLEquiDepthHistogramManager(mySQLConnector.ConnectionProperties, 10);
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
