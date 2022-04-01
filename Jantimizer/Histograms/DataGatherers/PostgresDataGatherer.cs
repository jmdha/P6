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
    public class PostgresDataGatherer : BaseDataGatherer, IDataGatherer
    {
        protected DatabaseConnector.Connectors.PostgreSqlConnector DbConnector { get; set; }

        public PostgresDataGatherer(ConnectionProperties connectionProperties) : base()
        {
            DbConnector = new DatabaseConnector.Connectors.PostgreSqlConnector(connectionProperties);
        }

        public override async Task<IEnumerable<string>> GetTableNamesInSchema()
        {
            var returnRows = await DbConnector.CallQuery($"SELECT * FROM information_schema.tables WHERE table_schema = current_schema();");
            if (returnRows.Tables.Count == 0)
                throw new NoNullAllowedException("Unexpected no result from database");

            return returnRows.Tables[0]
                    .AsEnumerable()
                    .Select(r => (string)r["table_name"]);
        }

        public override async Task<IEnumerable<string>> GetAttributeNamesForTable(string tableName)
        {
            var returnRows = await DbConnector.CallQuery($"SELECT * FROM information_schema.columns WHERE table_schema = current_schema() AND table_name = '{tableName}';");
            if (returnRows.Tables.Count == 0)
                throw new NoNullAllowedException("Unexpected no result from database");

            return returnRows.Tables[0]
                    .AsEnumerable()
                    .Select(r => (string)r["column_name"]);
        }


        public override async Task<List<ValueCount>> GetSortedGroupsFromDb(string tableName, string attributeName)
        {
            tableName = tableName.ToLower();
            attributeName = attributeName.ToLower();

            DataSet sortedGroupsDs = await DbConnector.CallQuery(@$"
                SELECT
                    {attributeName} AS val,
                    COUNT({attributeName})
                FROM {tableName} WHERE
                    {attributeName} IS NOT NULL
                GROUP BY {attributeName}
                ORDER BY {attributeName} ASC
            ");

            return GetValueCounts(sortedGroupsDs, "val", "COUNT").ToList();
        }
    }
}
