using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Globalization;
using System.IO;
using Konsole;
using QueryTestSuite.Models;
using QueryTestSuite.Exceptions;
using PrintUtilities;

namespace QueryTestSuite.Services
{
    internal class WindowManagerSingle : WindowManager
    {
        public WindowManagerSingle(string title, List<SuiteData> data) : base() {
            if (Console.BufferWidth < MinWindowWidth)
                throw new WindowSizeException($"Please increase console buffer width to atleast {MinWindowWidth}");

            int suitesToRun = 0;
            foreach (var dat in data) {
                if (dat.ShouldRun)
                    suitesToRun++;
            }
           
            GenerateRunWindow(title, data, suitesToRun);
        }

        private void GenerateRunWindow(string title, List<SuiteData> suites, int suitesToRun)
        {
            IConsole runWindow = Window.OpenBox(title, MinWindowWidth, RunWindowHeight);
            if (suitesToRun == 1)
            {
                GenerateRunWindow(suites[0].Name, runWindow.OpenBox(suites[0].Name));
            }
            else if (suitesToRun >= 2)
            {
                GenerateRunWindow(suites[0].Name, runWindow.SplitLeft(suites[0].Name));
                GenerateRunWindow(suites[1].Name, runWindow.SplitRight(suites[1].Name));
            }
        }

        private void GenerateRunWindow(string name, IConsole window)
        {
            var consoles = window.SplitRows(
                new Split(0, "Status"),
                new Split(3, "Progress")
            );
            WindowPrinters.Add(name, new WindowPrinter(this, false));
            WindowPrinters[name].StatusPrinter = new PrintUtil(consoles[0]);
            WindowPrinters[name].ProgressPrinter = new PrintUtil(consoles[1]);
        }

        public override IConsole GenerateReportWindow(string name)
        {
            IConsole reportWindow = Window.OpenBox(name, MinWindowWidth, ReportWindowHeight);
            WindowPrinters[name].ReportPrinter = new PrintUtil(reportWindow);
            return reportWindow;
        }
    }
}
