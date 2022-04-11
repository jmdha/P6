using Histograms.Exceptions;
using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.HistogramSelectors
{
    public class HistogramSelector : IHistogramSelector
    {
        public HistogramSelector()
        {
        }

        public IHistogram GetHistogramDepthOfTypeOrAlt(IHistogram targetType, Type type, string tableName, string attributeName, int depth)
        {
            if (targetType.AcceptedTypes.Contains(Type.GetTypeCode(type)))
                return targetType;

            IHistogram? histogram = null;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.Boolean:
                    histogram = new HistogramEquiDepth(tableName, attributeName, depth);
                    break;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    histogram = new HistogramEquiDepthVariance(tableName, attributeName, depth);
                    break;
                default:
                    throw new HistogramManagerErrorLogException(new ArgumentException($"Could not find a fitting histogram for TypeCode {Type.GetTypeCode(type)}!"));
            }
            return histogram;
        }
    }
}
