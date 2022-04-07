using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Models
{
    internal class BucketDictionary
    {
        internal Dictionary<string, Dictionary<string, List<IHistogramBucket>>> BDictionary { get; }

        internal BucketDictionary()
        {
            BDictionary = new Dictionary<string, Dictionary<string, List<IHistogramBucket>>>();
        }

        internal void AddBuckets(string tableName, string attributeName, List<IHistogramBucket> histogramBuckets)
        {
            if (!BDictionary.ContainsKey(tableName))
                BDictionary.Add(tableName, new Dictionary<string, List<IHistogramBucket>>());

            if (!BDictionary[tableName].ContainsKey(attributeName))
                BDictionary[tableName].Add(attributeName, histogramBuckets);
            else
                foreach (var bucket in histogramBuckets)
                    if (!BDictionary[tableName][attributeName].Contains(bucket))
                        BDictionary[tableName][attributeName].Add(bucket);
        }
    }
}
