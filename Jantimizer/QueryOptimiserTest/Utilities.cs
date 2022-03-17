using QueryParser.Models;
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
                nodes.Add(new JoinNode(
                    0,
                    type,
                    leftTableName,
                    "ID",
                    rightTableName,
                    "ID",
                    $"{leftTableName} {ComparisonType.GetOperatorString(type)} {rightTableName}"));
            }

            return nodes;
        }
    }
}
