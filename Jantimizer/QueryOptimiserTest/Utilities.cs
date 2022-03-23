using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryParser;
using Histograms.Models;

namespace QueryOptimiserTest
{
    static internal class Utilities
    {
        static internal HistogramEquiDepth CreateConstHistogram(string tableName, string attibuteName, int depth, int tableSize, IComparable value)
        {
            var tempGram = new HistogramEquiDepth(tableName, attibuteName, depth);
            List<IComparable> values = new List<IComparable>();
            for (int i = 0; i < tableSize; i++)
                values.Add(value);
            tempGram.GenerateHistogram(values);
            return tempGram;
        }

        static internal HistogramEquiDepth CreateIncreasingHistogram(string tableName, string attibuteName, int depth, int minValue, int maxValue)
        {
            var tempGram = new HistogramEquiDepth(tableName, attibuteName, depth);
            List<IComparable> values = new List<IComparable>();
            for (int i = minValue; i < maxValue; i++)
                values.Add(i);
            tempGram.GenerateHistogram(values);
            return tempGram;
        }

        static internal List<HistogramEquiDepth> CreateIncreasingHistograms(int count, int[] depths, int[] minValues, int[] maxValues)
        {
            List<HistogramEquiDepth> histograms = new List<HistogramEquiDepth>();
            for (int i = 0; i < count; i++) {
                histograms.Add(CreateIncreasingHistogram(
                    GetTableName(i),
                    "ID",
                    depths[i],
                    minValues[i],
                    maxValues[i]
                ));
            }
            return histograms;
        }

        static public string GetTableName(int index)
        {
            return "T" + index;
        }

        static internal List<INode> GenerateNodes(int nestDepth, ComparisonType.Type type)
        {
            if (nestDepth < 0)
                throw new ArgumentOutOfRangeException("NestDepth must be positive!");

            var nodes = new List<INode>();

            for (int i = 1; i <= nestDepth; i ++)
            {
                string leftTableName = GetTableName(i - 1);
                string rightTableName = GetTableName(i);
                nodes.Add(
                    new JoinNode(
                        i - 1,
                        $"{leftTableName} {QueryParser.Models.ComparisonType.GetOperatorString(type)} {rightTableName}",
                        type,
                        new TableReferenceNode(i - 1, leftTableName, leftTableName),
                        "ID",
                        new TableReferenceNode(i, rightTableName, rightTableName),
                        "ID"
                    ));
                    
            }

            return nodes;
        }
    }
}
