﻿using DatabaseConnector;
using DatabaseConnector.Connectors;
using QueryParser;
using QueryParser.Models;
using QueryParser.QueryParsers;
using System.Data;
using System.Text;
using Tools.Models;
using Histograms.Models;

namespace Histograms.Managers
{
    public class MySQLEquiDepthHistogramManager : BaseEquiDepthHistogramManager
    {
        public MySQLEquiDepthHistogramManager(ConnectionProperties connectionProperties, int depth) : base(connectionProperties, depth)
        {
            DbConnector = new DatabaseConnector.Connectors.MySqlConnector(connectionProperties);
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
                    string tableName = $"{table["TABLE_NAME"]}".ToLower();
                    foreach (DataRow row in (await GetAttributenamesForTable(tableName)).Rows)
                        await AddHistogramForAttribute(row, tableName);
                });
                tasks.Add(t);
            }
            return tasks;
        }

        private async Task<DataTable> GetTablesInSchema()
        {
            var returnRows = await DbConnector.CallQuery($"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = database();");
            if (returnRows.Tables.Count > 0)
                return returnRows.Tables[0];
            return new DataTable();
        }

        private async Task<DataTable> GetAttributenamesForTable(string tableName)
        {
            var returnRows = await DbConnector.CallQuery($"SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = database() AND TABLE_NAME = '{tableName}';");
            if (returnRows.Tables.Count > 0)
                return returnRows.Tables[0];
            return new DataTable();
        }

        private async Task AddHistogramForAttribute(DataRow row, string tableName)
        {
            string attributeName = $"{row["COLUMN_NAME"]}".ToLower();
            var attributeValues = await DbConnector.CallQuery($"SELECT {attributeName} FROM {tableName}");
            if (attributeValues.Tables.Count > 0)
            {
                IHistogram newHistogram = new HistogramEquiDepth(tableName, attributeName, Depth);
                newHistogram.GenerateHistogram(attributeValues.Tables[0], attributeName);
                Histograms.Add(newHistogram);
            }
        }
    }
}