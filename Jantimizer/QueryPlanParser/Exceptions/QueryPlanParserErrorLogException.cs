using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Exceptions;

namespace QueryPlanParser.Exceptions
{
    public class QueryPlanParserErrorLogException : BaseErrorLogException
    {
        public DataSet PlanData { get; set; }
        public QueryPlanParserErrorLogException(Exception actualException, DataSet planData) : base(actualException)
        {
            PlanData = planData;
        }

        public override string GetErrorLogMessage()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Plan Data tables:");
            int count = 0;
            foreach(DataTable dataTable in PlanData.Tables)
            {
                sb.AppendLine($"\tTable ID: [{count}]");
                sb.Append($"\tColumns:");
                foreach(DataColumn column in dataTable.Columns)
                    sb.Append($" [{column.ColumnName}]");
                sb.AppendLine("");
                sb.AppendLine($"\tRows:");
                int rowCount = 0;
                foreach (DataRow row in dataTable.Rows)
                {
                    sb.Append($"\t\t[{rowCount}] ");
                    foreach (var data in row.ItemArray)
                        sb.Append($" [{data}]");
                    sb.AppendLine();
                    rowCount++;
                }
                count++;
            }
            return sb.ToString();
        }
    }
}
