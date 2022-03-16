using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiserTest
{
    static internal class Utilities
    {
        static internal Histograms.HistogramEquiDepth CreateConstHistogram(string tableName, string attibuteName, int depth, int tableSize, int value)
        {
            var tempGram = new Histograms.HistogramEquiDepth(tableName, attibuteName, depth);
            List<int> values = new List<int>();
            for (int i = 0; i < tableSize; i++)
                values.Add(value);
            tempGram.GenerateHistogram(values);
            return tempGram;
        }

        static internal Histograms.HistogramEquiDepth CreateIncreasingHistogram(string tableName, string attibuteName, int depth, int minValue, int maxValue)
        {
            var tempGram = new Histograms.HistogramEquiDepth(tableName, attibuteName, depth);
            List<int> values = new List<int>();
            for (int i = minValue; i < maxValue; i++)
                values.Add(i);
            tempGram.GenerateHistogram(values);
            return tempGram;
        }
    }
}
