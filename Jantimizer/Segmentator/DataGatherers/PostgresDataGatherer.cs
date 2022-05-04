using DatabaseConnector.Connectors;
using Segmentator.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Caches;
using Tools.Models;
using Tools.Models.JsonModels;

namespace Segmentator.DataGatherers
{
    public class PostgresDataGatherer : BaseDataGatherer, IDataGatherer
    {
        protected ConnectionProperties ConnectionProperties { get; set; }

        public PostgresDataGatherer(ConnectionProperties connectionProperties) : base()
        {
            ConnectionProperties = connectionProperties;
        }

        public override async Task<IEnumerable<string>> GetTableNamesInSchema()
        {
            var returnRows = new DataSet();
            using (var connector = new PostgreSqlConnector(ConnectionProperties))
                returnRows = await connector.CallQueryAsync($"SELECT * FROM information_schema.tables WHERE table_schema = current_schema();");
            if (returnRows.Tables.Count == 0)
                throw new NoNullAllowedException("Unexpected no result from database");

            return returnRows.Tables[0]
                    .AsEnumerable()
                    .Select(r => ((string)r["table_name"]).ToLower());
        }

        public override async Task<IEnumerable<string>> GetAttributeNamesForTable(string tableName)
        {
            var returnRows = new DataSet();
            using (var connector = new PostgreSqlConnector(ConnectionProperties))
                returnRows = await connector.CallQueryAsync($"SELECT * FROM information_schema.columns WHERE table_schema = current_schema() AND table_name = '{tableName}';");
            if (returnRows.Tables.Count == 0)
                throw new NoNullAllowedException("Unexpected no result from database");

            return returnRows.Tables[0]
                    .AsEnumerable()
                    .Select(r => ((string)r["column_name"]).ToLower());
        }


        public override async Task<List<ValueCount>> GetSortedGroupsFromDb(TableAttribute attr)
        {
            var sortedGroupsDs = new DataSet();
            using (var connector = new PostgreSqlConnector(ConnectionProperties))
                sortedGroupsDs = await connector.CallQueryAsync(@$"
                    SELECT
                        ""{attr.Attribute.ToLower()}"" AS val,
                        COUNT(""{attr.Attribute.ToLower()}"")
                    FROM ""{attr.Table.TableName.ToLower()}"" WHERE
                        ""{attr.Attribute.ToLower()}"" IS NOT NULL
                    GROUP BY ""{attr.Attribute.ToLower()}""
                ");

            return GetValueCounts(sortedGroupsDs, "val", "COUNT").OrderBy(x => x.Value).ToList();
        }
    }
}
