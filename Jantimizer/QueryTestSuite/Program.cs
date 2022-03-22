using PrintUtilities;
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
            DateTime timeStamp = DateTime.Now;

            if (!File.Exists(Path.Join(testBaseDirPath, "testorder.json")))
                throw new IOException("Error! Order file 'testorder.json' was not found!");

            var parseOrder = JsonSerializer.Deserialize(File.ReadAllText(Path.Join(testBaseDirPath, "testorder.json")), typeof(TestOrder));
            if (parseOrder is TestOrder order)
            {
                foreach (var testFolder in order.Order)
                {
                    if (testFolder != "*")
                    {
                        if (Directory.Exists(Path.Join(testBaseDirPath, testFolder)))
                            await RunTestSuite(connectorSet, new DirectoryInfo(Path.Combine(testBaseDirPath, testFolder)));
                        else
                            PrintUtil.PrintLine($"Error, Test folder [{testFolder}] was not found!", 0, ConsoleColor.Red);
                    }
                    else
                    {
                        foreach (DirectoryInfo testDir in new DirectoryInfo(testBaseDirPath).GetDirectories())
                            if (!order.Order.Contains(testDir.Name))
                                await RunTestSuite(connectorSet, testDir);
                    }
                }
            }
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
