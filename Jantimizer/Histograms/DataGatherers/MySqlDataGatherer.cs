using Histograms.Caches;
using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Caches;
using Tools.Models;
using DatabaseConnector;

namespace Histograms.DataGatherers
{
    public class MySqlDataGatherer : BaseDataGatherer, IDataGatherer
    {
        protected ConnectionProperties ConnectionProperties { get; set; }

        public MySqlDataGatherer(ConnectionProperties connectionProperties) : base()
        {
            ConnectionProperties = connectionProperties;
        }

        public override async Task<IEnumerable<string>> GetTableNamesInSchema()
        {
            var returnRows = new DataSet();
            using (var connector = new DatabaseConnector.Connectors.MySqlConnector(ConnectionProperties))
                returnRows = await connector.CallQueryAsync($"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = database();");
            if (returnRows.Tables.Count == 0)
                throw new NoNullAllowedException("Unexpected no result from database");

            return returnRows.Tables[0]
                    .AsEnumerable()
                    .Select(r => (string)r["TABLE_NAME"]);
        }

        public override async Task<IEnumerable<string>> GetAttributeNamesForTable(string tableName)
        {
            var returnRows = new DataSet();
            using (var connector = new DatabaseConnector.Connectors.MySqlConnector(ConnectionProperties))
                returnRows = await connector.CallQueryAsync($"SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = database() AND TABLE_NAME = '{tableName}';");
            if (returnRows.Tables.Count == 0)
                throw new NoNullAllowedException("Unexpected no result from database");

            return returnRows.Tables[0]
                    .AsEnumerable()
                    .Select(r => (string)r["COLUMN_NAME"]);
        }

        public override async Task<List<ValueCount>> GetSortedGroupsFromDb(string tableName, string attributeName)
        {
            var sortedGroupsDs = new DataSet();
            using (var connector = new DatabaseConnector.Connectors.MySqlConnector(ConnectionProperties))
                sortedGroupsDs = await connector.CallQueryAsync(@$"
                    SELECT
                        `{attributeName}` AS val,
                        COUNT(`{attributeName}`) AS c
                    FROM `{tableName}` WHERE
                        `{attributeName}` IS NOT NULL
                    GROUP BY `{attributeName}`
                ");

            return GetValueCounts(sortedGroupsDs, "val", "c").OrderBy(x => x.Value).ToList();
        }

        public override async Task<string> GetTableAttributeColumnHash(string tableName, string attributeName)
        {
            var columnHash = new DataSet();
            using (var connector = new DatabaseConnector.Connectors.MySqlConnector(ConnectionProperties))
                columnHash = await connector.CallQueryAsync($"SET SESSION group_concat_max_len = 100000000000; SELECT md5(group_concat(md5(`{attributeName}`))) as hash FROM `{tableName}`;");
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

        public override async Task<Type> GetAttributeType(string tableName, string attributeName)
        {
            var result = new DataSet();
            using (var connector = new DatabaseConnector.Connectors.MySqlConnector(ConnectionProperties))
                result = await connector.CallQueryAsync($"SELECT `{attributeName}` FROM `{tableName}` LIMIT 1;");
            if (result.Tables.Count == 0)
                throw new ArgumentNullException($"Error! The database did not return a value for the attribute '{tableName}.{attributeName}'");
            if (result.Tables[0].Rows.Count == 0)
                throw new ArgumentNullException($"Error! The database did not return a value for the attribute '{tableName}.{attributeName}'");
            if (result.Tables[0].Columns.Count == 0)
                throw new ArgumentNullException($"Error! The database did not return a value for the attribute '{tableName}.{attributeName}'");
            DataColumn rowResult = result.Tables[0].Columns[0];
            Type typeValue = rowResult.DataType;
            return typeValue;
        }
    }
}
