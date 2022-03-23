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
        internal PrintUtil? StatusPrinter { get; private set; }
        internal PrintUtil? ProgressPrinter { get; private set; }
        internal PrintUtil? ReportPrinter { get; private set; }

        internal WindowPrinter() {

        }

        internal WindowPrinter(IConsole statusWindow, IConsole progressWindow, IConsole reportWindow) {
            StatusPrinter = new PrintUtil(statusWindow);
            ProgressPrinter = new PrintUtil(progressWindow);
            ReportPrinter = new PrintUtil(reportWindow);
        }
    }
}
