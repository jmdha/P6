using Histograms;
using QueryTestSuite.Connectors;
using QueryTestSuite.Models;
using QueryTestSuite.Parsers;
using QueryTestSuite.Services;
using System.Data;
using System.Linq;

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

            var postgresModel = new DatabaseCommunicator(
                "postgre",
                new PostgreSqlConnector(secrets.GetConnectionString("POSGRESQL")),
                new PostgreSqlParser());
            var mySqlModel = new DatabaseCommunicator(
                "mysql",
                new Connectors.MySqlConnector(secrets.GetConnectionString("MYSQL")),
                new MySQLParser());

            var connectorSet = new List<DatabaseCommunicator>() { postgresModel };


            await postgresModel.Connector.CallQuery(new FileInfo("../../../Tests/RandomNumberBlocks/cleanup.postgre.sql"));
            await postgresModel.Connector.CallQuery(new FileInfo("../../../Tests/RandomNumberBlocks/setup.postgre.sql"));
            var valuesToIndexQueryResult = await postgresModel.Connector.CallQuery("SELECT int4 FROM c");
            var valuesToIndexRows = valuesToIndexQueryResult.Tables[0].Rows;
            var valuesToIndex = new List<int>();
            for(int i=0; i< valuesToIndexRows.Count; i++)
                valuesToIndex.Add((int)valuesToIndexRows[i]["int4"]);

            int[] histogramValues = valuesToIndex.ToArray();
            var histogram = new HistogramEquiWidth(histogramValues, 20);


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

