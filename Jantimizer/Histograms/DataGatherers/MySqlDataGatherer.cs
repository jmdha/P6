using Histograms.Models;
using Histograms.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models;

namespace Histograms.DataGatherers
{
    public class MySqlDataGatherer : BaseDataGatherer, IDataGatherer
    {
        protected DatabaseConnector.Connectors.MySqlConnector DbConnector { get; set; }

        public MySqlDataGatherer(ConnectionProperties connectionProperties) : base()
        {
            DbConnector = new DatabaseConnector.Connectors.MySqlConnector(connectionProperties);
        }


        public override async Task<IEnumerable<string>> GetTableNamesInSchema()
        {
            var returnRows = await DbConnector.CallQueryAsync($"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = database();");
            if (returnRows.Tables.Count == 0)
                throw new NoNullAllowedException("Unexpected no result from database");

            return returnRows.Tables[0]
                    .AsEnumerable()
                    .Select(r => (string)r["TABLE_NAME"]);
        }

        public override async Task<IEnumerable<string>> GetAttributeNamesForTable(string tableName)
        {
            var returnRows = await DbConnector.CallQueryAsync($"SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = database() AND TABLE_NAME = '{tableName}';");
            if (returnRows.Tables.Count == 0)
                throw new NoNullAllowedException("Unexpected no result from database");

            return returnRows.Tables[0]
                    .AsEnumerable()
                    .Select(r => (string)r["COLUMN_NAME"]);
        }

        public override async Task<List<ValueCount>> GetSortedGroupsFromDb(string tableName, string attributeName)
        {
            DataSet sortedGroupsDs = await DbConnector.CallQueryAsync(@$"
                SELECT
                    {attributeName} AS val,
                    COUNT({attributeName}) AS c
                FROM {tableName} WHERE
                    {attributeName} IS NOT NULL
                GROUP BY {attributeName}
                ORDER BY {attributeName} ASC
            ");

            return GetValueCounts(sortedGroupsDs, "val", "c").ToList();
        }

        public override async Task<IHistogram?> GetHistogramFromCacheOrNull(string tableName, string attributeName)
        {
            var retVal = HistogramCache.GetHistogramOrNull(tableName, attributeName, await GetTableAttributeColumnHash(tableName, attributeName));
            return retVal;
        }

        public override async Task<string> GetTableAttributeColumnHash(string tableName, string attributeName)
        {
            DataSet columnHash = await DbConnector.CallQueryAsync($"SELECT md5(group_concat(md5({attributeName}))) as hash FROM {tableName};");
            if (columnHash.Tables.Count == 0)
                throw new ArgumentNullException($"Error! The database did not return a hash value for the column '{tableName}.{attributeName}'");
            if (columnHash.Tables[0].Rows.Count == 0)
                throw new ArgumentNullException($"Error! The database did not return a hash value for the column '{tableName}.{attributeName}'");
            DataRow hashRow = columnHash.Tables[0].Rows[0];
            if (!hashRow.Table.Columns.Contains("hash"))
                throw new ArgumentNullException($"Error! The database did not return a hash value for the column '{tableName}.{attributeName}'");
            string hashValue = (string)hashRow["hash"];
            return hashValue;
        }
    }
}
