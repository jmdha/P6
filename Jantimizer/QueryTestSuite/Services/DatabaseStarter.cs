using PrintUtilities;
using QueryTestSuite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTestSuite.Services
{
    internal static class DatabaseStarter
    {
        public static async Task<bool> CheckAndStartServers(List<TestCase> connectorParsers)
        {
            PrintUtil.PrintLine($"Checking if databases are online...", 0, ConsoleColor.DarkGray);
            foreach (var connector in connectorParsers)
            {
                if (connector.Connector.CheckConnection())
                {
                    PrintUtil.PrintLine($"Connector [{connector.Name}] is online!", 0, ConsoleColor.Green);
                }
                else
                {
                    PrintUtil.PrintLine($"Warning! Connector [{connector.Name}] is not online!", 0, ConsoleColor.Yellow);
                    PrintUtil.PrintLine($"Attempting to start...", 0, ConsoleColor.Yellow);

                    if (!await connector.Connector.StartServer())
                    {
                        PrintUtil.PrintLine($"Error! Connector [{connector.Name}] could not start!", 0, ConsoleColor.Red);
                        return false;
                    }
                    else
                        PrintUtil.PrintLine($"Connector [{connector.Name}] is online!", 0, ConsoleColor.Green);
                }
            }
            return true;
        }

        public static void StopAllServers(List<TestCase> connectorParsers)
        {
            PrintUtil.PrintLine($"Stopping active servers", 0, ConsoleColor.DarkGray);
            foreach (var connector in connectorParsers)
                connector.Connector.StopServer();
        }
    }
}
