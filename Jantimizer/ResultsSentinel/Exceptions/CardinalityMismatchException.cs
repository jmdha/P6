using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsSentinel.Exceptions
{
    public class CardinalityMismatchException : Exception
    {
        public CardinalityMismatchException(string? message) : base(message)
        {
        }
    }
}
