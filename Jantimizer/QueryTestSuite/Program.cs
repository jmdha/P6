using QueryTestSuite.Connectors;
using QueryTestSuite.Models;
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
            var pgConn = new PostGres("Host=localhost;Username=postgres;Password=Aber1234;Database=JanBase");
            var myConn = new MySql("Host=localhost;Username=Jantimizer;Password=Aber1234");

            var connectorSet = new List<DbConnector>() { pgConn };


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

