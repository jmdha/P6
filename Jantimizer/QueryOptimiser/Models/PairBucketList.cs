using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Models
{
    public class PairBucketList
    {
        public List<IHistogramBucket> LeftBuckets { get; set; }
        public List<IHistogramBucket> RightBuckets { get; set; }

        public PairBucketList()
        {
            LeftBuckets = new List<IHistogramBucket>();
            RightBuckets = new List<IHistogramBucket>();
        }

        public PairBucketList(List<IHistogramBucket> leftBuckets, List<IHistogramBucket> rightBuckets)
        {
            LeftBuckets = leftBuckets;
            RightBuckets = rightBuckets;
        }
    }
}
