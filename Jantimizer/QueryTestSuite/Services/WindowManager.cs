using Konsole;
using QueryTestSuite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTestSuite.Services
{
    internal abstract class WindowManager : IWindowManager
    {
        protected const int MinWindowHeight = 0;
        protected const int MinWindowWidth = 120;
        protected const int MinSplitWindowWidth = 215;
        protected const int RunWindowHeight = 15;
        protected const int ReportWindowHeight = 25;

        public Dictionary<string, WindowPrinter> WindowPrinters { get; set; }

        public WindowManager()
        {
            WindowPrinters = new Dictionary<string, WindowPrinter>();
        }

        public static IWindowManager GetWindowManager(string title, List<SuiteData> data)
        {
            if (Console.BufferWidth > MinSplitWindowWidth)
                return new WindowManagerSplit(title, data);
            else
                return new WindowManagerSingle(title, data);
        }

        public abstract IConsole GenerateReportWindow(string name);
    }
}
