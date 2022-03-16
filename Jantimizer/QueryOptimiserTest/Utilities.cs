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

        static private string GetTableName(int index)
        {
            int startIndex = System.Convert.ToInt32('A');
            int endIndex = System.Convert.ToInt32('Z');

            int indexedIndex = startIndex + index;

            int overflow = (int) Math.Floor((double)indexedIndex / endIndex);
            indexedIndex -= overflow * endIndex;

            string tableName = "";

            for (int i = 0; i < overflow; i++)
                tableName += ((char)endIndex);
            tableName += ((char)indexedIndex);
            return tableName;
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
                    i,
                    $"{leftTableName} {ComparisonType.GetOperatorString(type)} {rightTableName}",
                    type,
                    leftTableName,
                    "ID",
                    rightTableName,
                    "ID"));
            }

            return nodes;
        }
    }
}
