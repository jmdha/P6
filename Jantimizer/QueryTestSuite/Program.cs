using PrintUtilities;
using QueryTestSuite.Helpers;
using QueryTestSuite.Models;
using QueryTestSuite.Services;
using QueryTestSuite.SuiteDatas;
using QueryTestSuite.TestRunners;
using System.Text.Json;
using Tools.Helpers;
using Tools.Models;
using Tools.Services;

namespace QueryTestSuite
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Setup suite datas
            var pgData = SuiteDataSets.GetPostgresSD();
            var myData = SuiteDataSets.GetMySQLSD(pgData.QueryParserManager.QueryParsers[0]);
            var connectorSet = new List<SuiteData>() { pgData, myData };

            // Get the order file and run each test case
            DateTime runTime = DateTime.UtcNow;
            var testBaseDirPath = IOHelper.GetDirectory("../../../Tests");
            var orderFile = IOHelper.GetFile(testBaseDirPath, "testorder.json");
            List<DirectoryInfo> runOrder = TestOrderSorter.GetTestRunOrder(testBaseDirPath, orderFile);
            foreach (DirectoryInfo dirInfo in runOrder)
                await RunTestSuite(connectorSet, dirInfo, runTime);
        }

        private static async Task RunTestSuite(List<SuiteData> connectorSet, DirectoryInfo info, DateTime time) 
        {
            TestSuite suite = new TestSuite(connectorSet, time);

            PrintUtil.PrintLine($"Running test collection [{info.Name}]", 0, ConsoleColor.Magenta);
            await suite.RunTests(info);
            PrintUtil.PrintLine($"Test collection [{info.Name}] finished!", 0, ConsoleColor.Magenta);
        }
    }
}
