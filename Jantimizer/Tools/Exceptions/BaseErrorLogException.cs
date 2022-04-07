using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Exceptions
{
    public abstract class BaseErrorLogException : Exception
    {
        public BaseErrorLogException(string? message) : base(message)
        {
        }

        public abstract string GetErrorLogMessage();
    }
}
