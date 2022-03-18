using PrintUtilities;
using QueryTestSuite.Models;
using QueryTestSuite.Services;
using QueryTestSuite.SuiteDatas;
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

            var connectorSet = new List<SuiteData>() { 
                MySQLEquiDepthData.GetData(secrets),
                PostgreEquiDepthData.GetData(secrets)};

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
