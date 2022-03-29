using DatabaseConnector;
using DatabaseConnector.Connectors;
using QueryParser;
using QueryParser.Models;
using QueryParser.QueryParsers;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using Tools.Models;
using Histograms.Models;

namespace Histograms.Managers
{
    public class PostgresHistogramManager : BaseHistogramManager
    {
        public PostgresHistogramManager(ConnectionProperties connectionProperties, int depth) : base(depth)
        {
            DbConnector = new PostgreSqlConnector(connectionProperties);
        }

        public override async Task<List<Task>> AddHistogramsFromDB()
        {
            ClearHistograms();
            List<Task> tasks = new List<Task>();
            IEnumerable<DataRow> allTables = (await GetTablesInSchema()).Rows.Cast<DataRow>();
            foreach (var table in allTables)
            {
                Task t = Task.Run(async () =>
                {
                    string tableName = $"{table["table_name"]}".ToLower();
                    foreach (DataRow row in (await GetAttributenamesForTable(tableName)).Rows)
                        await AddHistogramForAttribute(row, tableName);
                });
                tasks.Add(t);
            }
            return tasks;
        }

        private async Task<DataTable> GetTablesInSchema()
        {
            var returnRows = await DbConnector.CallQuery($"SELECT * FROM information_schema.tables WHERE table_schema = current_schema();");
            if (returnRows.Tables.Count > 0)
                return returnRows.Tables[0];
            return new DataTable();
        }

        private async Task<DataTable> GetAttributenamesForTable(string tableName)
        {
            var returnRows = await DbConnector.CallQuery($"SELECT * FROM information_schema.columns WHERE table_schema = current_schema() AND table_name = '{tableName}';");
            if (returnRows.Tables.Count > 0)
                return returnRows.Tables[0];
            return new DataTable();
        }

        private async Task AddHistogramForAttribute(DataRow row, string tableName)
        {
            string attributeName = $"{row["column_name"]}".ToLower();
            DataSet sortedGroupsDs = await DbConnector.CallQuery(@$"
                SELECT
                    {attributeName} AS val,
                    COUNT({attributeName})
                FROM {tableName} WHERE
                    {attributeName} IS NOT NULL
                GROUP BY {attributeName}
                ORDER BY {attributeName} ASC
            ");

            List<ValueCount> sortedGroups = GetValueCounts(sortedGroupsDs, "val", "COUNT").ToList();

            IHistogram newHistogram = new HistogramEquiDepth(tableName, attributeName, Depth);
            newHistogram.GenerateHistogramFromSortedGroups(sortedGroups);
            Histograms.Add(newHistogram);
        }

        private IEnumerable<ValueCount> GetValueCounts(DataSet dataSet, string valueColumnName, string countColumnName)
        {
            return dataSet.Tables[0].AsEnumerable().Select(r =>
                new ValueCount(
                    (IComparable)r[valueColumnName],
                    (long)r[countColumnName]
                )
            );
        }
        public override string? ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Recorded Histograms:");
            foreach (var histogram in Histograms)
                sb.AppendLine(histogram.ToString());
            return sb.ToString();
        }
    }
}
