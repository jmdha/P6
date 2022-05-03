using Histograms.DepthCalculators;
using Histograms.Exceptions;
using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.HistogramSelectors
{
    public class DepthHistogramSelector : IDepthHistogramSelector<IDepthHistogram>
    {
        public DepthHistogramSelector()
        {
        }

        public IDepthHistogram GetHistogramOfTypeOrAlt(IDepthHistogram targetType, Type type, string tableName, string attributeName)
        {
            throw new NotImplementedException("It makes no sense to call this method here! Use the one with depth");
        }

        public IDepthHistogram GetHistogramDepthOfTypeOrAlt(IDepthHistogram targetType, Type type, string tableName, string attributeName, IDepthCalculator depthCalculator)
        {
            if (targetType.AcceptedTypes.Contains(Type.GetTypeCode(type)))
                return targetType;

            IDepthHistogram? histogram = null;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Double:
                    histogram = new HistogramEquiDepthVariance(tableName, attributeName, depthCalculator);
                    break;
                case TypeCode.String:
                case TypeCode.Decimal:
                case TypeCode.Boolean:
                case TypeCode.DateTime:
                    histogram = new HistogramEquiDepth(tableName, attributeName, depthCalculator);
                    break;
                default:
                    throw new HistogramManagerErrorLogException(new ArgumentException($"Could not find a fitting histogram for TypeCode {Type.GetTypeCode(type)}!"));
            }
            return histogram;
        }
    }
}
