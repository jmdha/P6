using PrintUtilities;
using QueryTestSuite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tools.Helpers;

namespace QueryTestSuite.Helpers
{
    internal static class TestOrderSorter
    {
        public static List<DirectoryInfo> GetTestRunOrder(DirectoryInfo testBaseDirPath, FileInfo orderFile)
        {
            List<DirectoryInfo> result = new List<DirectoryInfo>();

            if (!File.Exists(orderFile.FullName))
                throw new IOException($"Order file '{orderFile.FullName}' not found!");

            var parseOrder = JsonSerializer.Deserialize(File.ReadAllText(orderFile.FullName), typeof(TestOrder));
            if (parseOrder is TestOrder order)
            {
                foreach (var testFolder in order.Order)
                {
                    if (testFolder != "*")
                    {
                        DirectoryInfo info = IOHelper.GetDirectory(testBaseDirPath, testFolder);
                        if (Directory.Exists(info.FullName))
                            result.Add(info);
                        else
                            PrintUtil.PrintLine($"Error, Test folder [{testFolder}] was not found!", 0, ConsoleColor.Red);
                    }
                    else
                    {
                        foreach (DirectoryInfo testDir in testBaseDirPath.GetDirectories())
                            if (!order.Order.Contains(testDir.Name))
                                result.Add(testDir);
                    }
                }
            }

            return result;
        }
    }
}
