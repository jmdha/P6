using QueryParser;
using QueryParser.QueryParsers;
using QueryTestSuite.Connectors;
using QueryTestSuite.Models;
using QueryTestSuite.Parsers;
using QueryTestSuite.Services;
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
            IParserManager parserManager = new ParserManager(
                new List<IQueryParser>() { 
                    new JoinQueryParser()
                });

            var returnVal = parserManager.ParseQuery("SELECT * FROM ((D JOIN A ON D.Value1 > A.Value2) JOIN B ON A.Value1 > B.Value2) JOIN C ON C.Value3 < B.Value2");

            SecretsService secrets = new SecretsService();

            var postgresModel = new DatabaseCommunicator(
                "postgre",
                new PostgreSqlConnector(secrets.GetConnectionString("POSGRESQL")),
                new PostgreSqlParser());
            var mySqlModel = new DatabaseCommunicator(
                "mysql",
                new Connectors.MySqlConnector(secrets.GetConnectionString("MYSQL")),
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

