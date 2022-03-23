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
    internal class WindowManagerSplit : WindowManager
    {
        public WindowManagerSplit(string title, List<SuiteData> data) : base()
        {
            if (Console.BufferWidth < MinWindowWidth)
                throw new WindowSizeException($"Please increase console buffer width to atleast {MinWindowWidth}");

            IConsole primaryWindow = Window.OpenBox(title, MinSplitWindowWidth, RunWindowHeight + ReportWindowHeight);

            GenerateSuiteWindows(primaryWindow, data);
        }

        private void GenerateSuiteWindows(IConsole parent, List<SuiteData> data)
        {
            int suitesToRun = 0;
            foreach (var dat in data)
                if (dat.ShouldRun)
                    suitesToRun++;

            if (suitesToRun == 1)
            {
                foreach (var suite in data)
                    if (suite.ShouldRun)
                        GenerateSuiteWindow(parent, suite);
            }
            else if (suitesToRun == 2)
            {
                List<SuiteData> matchedSuites = new List<SuiteData>();
                foreach (var suite in data)
                    if (suite.ShouldRun)
                        matchedSuites.Add(suite);
                GenerateSuiteWindow(parent.SplitLeft(), matchedSuites[0]);
                GenerateSuiteWindow(parent.SplitRight(), matchedSuites[1]);

            }
            else
                throw new NotImplementedException();
        }

        private void GenerateSuiteWindow(IConsole parent, SuiteData data)
        {
            var consoles = parent.SplitRows(
                new Split(10, "Status"),
                new Split(3, "Progress"),
                new Split(0, "Report")
            );
            WindowPrinters[data.Name] = new WindowPrinter(this, true);
            WindowPrinters[data.Name].StatusPrinter = new PrintUtil(consoles[0]);
            WindowPrinters[data.Name].ProgressPrinter = new PrintUtil(consoles[1]);
            WindowPrinters[data.Name].ReportPrinter = new PrintUtil(consoles[2]);
        }

        public override IConsole GenerateReportWindow(string name)
        {
            throw new NotImplementedException();
        }
    }
}
