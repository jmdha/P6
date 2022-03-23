using Konsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTestSuite.Services
{
    internal interface IWindowManager
    {
        public Dictionary<string, WindowPrinter> WindowPrinters { get; protected set; }

        public IConsole GenerateReportWindow(string name);
    }
}
