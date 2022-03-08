using QueryEditorTool.QueryModifiers;
using QueryEditorTool.QueryParsers;
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
            IQueryParser parser = new QueryParser();
            var returnVal = parser.ParseQuery("SELECT * FROM (A JOIN B ON A.Value1 > B.Value2) JOIN C ON C.Value3 < B.Value2");
            string parsed = returnVal.ToString();

            IQueryModifier modifier = new QueryModifier();
            modifier.ReorderMovableNodes(returnVal, 0, 1);

            parsed = returnVal.ToString();

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

