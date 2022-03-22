using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Exceptions
{
    public class MissingKeyException : Exception
    {
        public MissingKeyException(string message) : base(message) { }
    }
}
