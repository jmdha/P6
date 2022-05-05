using DatabaseConnector.Connectors;
using Milestoner.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Caches;
using Tools.Models;
using Tools.Models.JsonModels;

namespace Milestoner.DataGatherers
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

        public override async Task<TypeCode> GetTypeCodeFromDb(TableAttribute attribute)
        {
            var result = new DataSet();
            using (var connector = new PostgreSqlConnector(ConnectionProperties))
                result = await connector.CallQueryAsync($"SELECT \"{attribute.Attribute}\" FROM \"{attribute.Table.TableName}\" LIMIT 1;");
            if (result.Tables.Count == 0)
                throw new ArgumentNullException($"Error! The database did not return a value for the attribute '{attribute}'");
            if (result.Tables[0].Rows.Count == 0)
                throw new ArgumentNullException($"Error! The database did not return a value for the attribute '{attribute}'");
            if (result.Tables[0].Columns.Count == 0)
                throw new ArgumentNullException($"Error! The database did not return a value for the attribute '{attribute}'");
            DataColumn rowResult = result.Tables[0].Columns[0];
            Type typeValue = rowResult.DataType;
            return Type.GetTypeCode(typeValue);
        }
    }
}
