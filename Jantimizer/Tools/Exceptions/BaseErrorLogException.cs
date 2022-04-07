using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Exceptions
{
    public abstract class BaseErrorLogException : Exception
    {
        public Exception ActualException { get; set; }
        public BaseErrorLogException(Exception actualExceptio)
        {
            ActualException = actualExceptio;
        }

        public abstract string GetErrorLogMessage();
    }
}
