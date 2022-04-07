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

        internal void AddBuckets(BucketDictionary dictionary)
        {
            foreach (var tableLimit in dictionary.BDictionary)
                foreach (var attributeLimit in tableLimit.Value)
                    foreach (IHistogramBucket bucket in attributeLimit.Value)
                    {
                        HandleNonExistance(tableLimit.Key, attributeLimit.Key);
                        BDictionary[tableLimit.Key][attributeLimit.Key].Add(bucket);
                    }
                        
        }

        internal void AddBucket(string tableName, string attributeName, IHistogramBucket histogramBucket)
        {
            HandleNonExistance(tableName, attributeName);
            if (!BDictionary[tableName][attributeName].Contains(histogramBucket))
                BDictionary[tableName][attributeName].Add(histogramBucket);
        }

        internal void AddBuckets(string tableName, string attributeName, List<IHistogramBucket> histogramBuckets)
        {
            HandleNonExistance(tableName, attributeName);
            foreach (var bucket in histogramBuckets)
                if (!BDictionary[tableName][attributeName].Contains(bucket))
                    BDictionary[tableName][attributeName].Add(bucket);
        }

        private void HandleNonExistance(string table, string attribute)
        {
            if (!BDictionary.ContainsKey(table))
                BDictionary.Add(table, new Dictionary<string, List<IHistogramBucket>>());

            if (!BDictionary[table].ContainsKey(attribute))
                BDictionary[table].Add(attribute, new List<IHistogramBucket>());
        }
    }
}
