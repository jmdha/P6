using PrintUtilities;
using QueryTestSuite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QueryTestSuite.Helpers
{
    internal static class TestOrderSorter
    {
        public static List<DirectoryInfo> GetTestRunOrder(string testBaseDirPath, string orderFile)
        {
            List<DirectoryInfo> result = new List<DirectoryInfo>();

            if (!File.Exists(orderFile))
                throw new IOException($"Error! Order file '{orderFile}' was not found!");

            var parseOrder = JsonSerializer.Deserialize(File.ReadAllText(orderFile), typeof(TestOrder));
            if (parseOrder is TestOrder order)
            {
                foreach (var testFolder in order.Order)
                {
                    if (testFolder != "*")
                    {
                        if (Directory.Exists(Path.Join(testBaseDirPath, testFolder)))
                            result.Add(new DirectoryInfo(Path.Combine(testBaseDirPath, testFolder)));
                        else
                            Console.WriteLine($"Error, Test folder [{testFolder}] was not found!");
                    }
                    else
                    {
                        foreach (DirectoryInfo testDir in new DirectoryInfo(testBaseDirPath).GetDirectories())
                            if (!order.Order.Contains(testDir.Name))
                                result.Add(testDir);
                    }
                }
            }

            return result;
        }
    }
}
