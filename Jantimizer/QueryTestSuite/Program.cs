using PrintUtilities;
using QueryTestSuite.Helpers;
using QueryTestSuite.Models;
using QueryTestSuite.Services;
using QueryTestSuite.SuiteDatas;
using QueryTestSuite.TestRunners;
using System.Text.Json;
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

            var pgData = PostgreEquiDepthData.GetData(secrets);
            var myData = MySQLEquiDepthData.GetData(secrets);

            myData.QueryParserManager.QueryParsers.Add(pgData.QueryParserManager.QueryParsers[0]);

            var connectorSet = new List<SuiteData>() {pgData, myData };

            string testBaseDirPath = Path.GetFullPath("../../../Tests");

            foreach(DirectoryInfo dirInfo in TestOrderSorter.GetTestRunOrder(testBaseDirPath, Path.Join(testBaseDirPath, "testorder.json")))
                await RunTestSuite(connectorSet, dirInfo);
        }

        private static async Task RunTestSuite(List<SuiteData> connectorSet, DirectoryInfo info) 
        {
            TestSuite suite = new TestSuite(connectorSet, DateTime.Now);

            PrintUtil.PrintLine($"Running test collection [{info.Name}]", 0, ConsoleColor.Magenta);
            await suite.RunTests(info);
            PrintUtil.PrintLine($"Test collection [{info.Name}] finished!", 0, ConsoleColor.Magenta);
        }
    }
}
