using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Exceptions;

namespace Histograms.Exceptions
{
    public class HistogramManagerErrorLogException : BaseErrorLogException
    {
        public HistogramManagerErrorLogException(Exception actualExceptio) : base(actualExceptio)
        {
        }

        public override string GetErrorLogMessage()
        {
            var sb = new StringBuilder();

            return sb.ToString();
        }
    }
}
