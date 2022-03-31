using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistogramsTests.TestHelpers
{
    internal static class DataTableHelper
    {
        internal static DataTable GetDatatable(string columnName, Type columnType)
        {
            return GetDatatable(new string[] { columnName }, new Type[] { columnType });
        }

        internal static DataTable GetDatatable(string[] columnNames, Type[] columnTypes)
        {
            DataTable table = new DataTable();
            for (int i = 0; i < columnNames.Length; i++)
                table.Columns.Add(new DataColumn(columnNames[i], columnTypes[i]));
            return table;
        }

        internal static void AddRow(DataTable dt, int[] data)
        {
            DataRow row = dt.NewRow();
            for (int i = 0; i < data.Length; i++)
                row[i] = data[i];
            dt.Rows.Add(row);
        }
    }
}
