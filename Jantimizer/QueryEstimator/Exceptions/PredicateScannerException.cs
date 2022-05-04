using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEstimator.Exceptions
{
    public enum PredicateScannerErrorType { None, Unscannable, NoTableAttributePrediacte, IlligalFilter }
    public class PredicateScannerException : Exception
    {
        public PredicateScannerErrorType Type { get; set; }
        public PredicateScannerException(string? message, PredicateScannerErrorType type) : base(message)
        {
            Type = type;
        }

        public override string ToString()
        {
            return $"Predicate error occured! Type: {Type}";
        }
    }
}
