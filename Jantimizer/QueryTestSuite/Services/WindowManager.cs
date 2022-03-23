using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Globalization;
using System.IO;
using Konsole;
using QueryTestSuite.Models;
using QueryTestSuite.Exceptions;

namespace QueryTestSuite.Services
{
    internal class WindowManager
    {
        internal IConsole ParentWindow { get; private set; }
        internal Dictionary<string, WindowPrinter> windowPrinters = new Dictionary<string, WindowPrinter>();

        internal void GenerateConsoleLayout(string title, List<SuiteData> data) {
            if (Console.BufferWidth < 100)
                throw new WindowSizeException("Need bigger window");

            int suitesToRun = 0;
            foreach (var dat in data) {
                if (dat.ShouldRun)
                    suitesToRun++;
            }
            ParentWindow = Window.OpenBox(title, 100, 15 + (25 * suitesToRun));
            var consoles = ParentWindow.SplitRows(
                new Split(15, "Statuses"),
                new Split(0, "Reports")
            );
            IConsole statusWindow = consoles[0];
            IConsole reportsWindow = consoles[1];

            GenerateStatusWindow(statusWindow, data, suitesToRun);
            GenerateReportsWindow(reportsWindow, data, 25);
            
            Consoles.Add("Reports", reportsWindow);
            Consoles.Add("Statuses", statusWindow);
        }
    }
}
