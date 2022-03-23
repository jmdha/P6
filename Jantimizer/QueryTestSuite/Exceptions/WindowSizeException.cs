using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTestSuite.Exceptions
{
    public class WindowSizeException : Exception
    {
        public WindowSizeException(string? message) : base(message)
        {
        }
    }
}
