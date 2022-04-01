using Histograms.Models;
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
            var returnRows = await DbConnector.CallQuery($"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = database();");
            if (returnRows.Tables.Count == 0)
                throw new NoNullAllowedException("Unexpected no result from database");

            return returnRows.Tables[0]
                    .AsEnumerable()
                    .Select(r => (string)r["TABLE_NAME"]);
        }

        public override async Task<IEnumerable<string>> GetAttributeNamesForTable(string tableName)
        {
            var returnRows = await DbConnector.CallQuery($"SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = database() AND TABLE_NAME = '{tableName}';");
            if (returnRows.Tables.Count == 0)
                throw new NoNullAllowedException("Unexpected no result from database");

            return returnRows.Tables[0]
                    .AsEnumerable()
                    .Select(r => (string)r["COLUMN_NAME"]);
        }

        public override async Task<List<ValueCount>> GetSortedGroupsFromDb(string tableName, string attributeName)
        {
            DataSet sortedGroupsDs = await DbConnector.CallQuery(@$"
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

    }
}
