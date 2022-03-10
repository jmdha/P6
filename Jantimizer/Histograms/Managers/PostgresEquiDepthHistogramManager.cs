using DatabaseConnector;
using DatabaseConnector.Connectors;
using QueryParser;
using QueryParser.Models;
using QueryParser.QueryParsers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.Managers
{
    public class PostgresEquiDepthHistogramManager : IHistogramManager<IHistogram, IDbConnector>
    {
        public IDbConnector DbConnector { get; }
        public List<IHistogram> Histograms { get; }
        public List<string> Tables => Histograms.Select(x => x.TableName).Distinct().ToList();
        public List<string> Attributes {
            get
            {
                var returnList = new List<string>();
                foreach(var histogram in Histograms)
                {
                    returnList.Add($"{histogram.TableName}.{histogram.AttributeName}");
                }
                return returnList;
            }
        }
        public int Depth { get; }

        public PostgresEquiDepthHistogramManager(string connectionString, int depth)
        {
            DbConnector = new PostgreSqlConnector(connectionString);
            Histograms = new List<IHistogram>();
            Depth = depth;
        }

        public async Task AddHistograms(FileInfo setupQueryFile)
        {
            foreach (string line in File.ReadAllLines(setupQueryFile.FullName))
                await AddHistograms(line);
        }

        public void AddHistogram(IHistogram histogram)
        {
            Histograms.Add(histogram);
        }

        public async Task AddHistograms(string setupQuery)
        {
            IParserManager parser = new ParserManager(new List<IQueryParser> { new CreateTableQueryParser() });
            List<INode> nodes = parser.ParseQuery(setupQuery, false);
            if (nodes.Count > 0 && nodes[0] is CreateTableNode node)
            {
                foreach (DataRow row in (await GetAttributenamesForTable(node.TableName)).Rows)
                {
                    await AddHistogramForAttribute(row, node.TableName);
                }
            }
        }

        private async Task<DataTable> GetAttributenamesForTable(string tableName)
        {
            var returnRows = await DbConnector.CallQuery($"SELECT * FROM information_schema.columns WHERE table_schema = 'public' AND table_name = '{tableName}';");
            if (returnRows.Tables.Count > 0)
            {
                return returnRows.Tables[0];
            }
            return new DataTable();
        }

        private async Task AddHistogramForAttribute(DataRow row, string tableName)
        {
            string attributeName = $"{row["column_name"]}";
            var attributeValues = await DbConnector.CallQuery($"SELECT {attributeName} FROM {tableName}");
            if (attributeValues.Tables.Count > 0)
            {
                IHistogram newHistogram = new HistogramEquiDepth(tableName, attributeName, Depth);
                newHistogram.GenerateHistogram(attributeValues.Tables[0], attributeName);
                Histograms.Add(newHistogram);
            }
        }

        public void PrintAllHistograms()
        {
            StringBuilder sb = new StringBuilder();
            Console.WriteLine("Recorded Histograms:");
            foreach(var histogram in Histograms)
            {
                sb.AppendLine(histogram.ToString());
            }
            Console.WriteLine(sb.ToString());
        }
    }
}
