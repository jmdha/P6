using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Globalization;
using System.IO;
using PrintUtilities;
using Konsole;

namespace QueryTestSuite.Services
{
    internal class WindowPrinter
    {
        private IWindowManager ParentWindowManager { get; }
        internal bool SplitWindow { get; private set; }
        internal PrintUtil? StatusPrinter { get; set; }
        internal PrintUtil? ProgressPrinter { get; set; }
        internal PrintUtil? ReportPrinter { private get; set; }

        internal WindowPrinter(IWindowManager parentWindowManager, bool splitWindow) {
            ParentWindowManager = parentWindowManager;
            SplitWindow = splitWindow;
        }

        internal PrintUtil GetReportPrinter(string name)
        {
            if (ReportPrinter == null)
                return new PrintUtil(ParentWindowManager.GenerateReportWindow(name));
            else
                return ReportPrinter;
        }

    }
}
