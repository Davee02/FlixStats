using System;
using System.Collections.Generic;
using Google.DataTable.Net.Wrapper;
using NetflixStatizier.Utilities;

namespace NetflixStatizier.Helper
{
    public static class GoogleDataTableBuilder
    {
        public static string GetDataTableJsonFromDictionnary<TKey, TValue>(IDictionary<TKey, TValue> data, string columnName1, string columnName2)
        {
            var dataTable = new DataTable();
            dataTable.AddColumn(new Column(GetColumnTypeFromType(typeof(TKey)), columnName1, columnName1));
            dataTable.AddColumn(new Column(GetColumnTypeFromType(typeof(TValue)), columnName2, columnName2));

            foreach (var (key, value) in data)
            {
                var row = dataTable.NewRow();
                row.AddCellRange(new[]
                    {
                        new Cell(key),
                        new Cell(value)
                    });
                dataTable.AddRow(row);
            }

            return dataTable.GetJson();
        }

        private static ColumnType GetColumnTypeFromType(Type type)
        {
            if (TypeUtilities.IsNumericType(type))
                return ColumnType.Number;

            if (type == typeof(bool) || type == typeof(bool?))
                return ColumnType.Boolean;

            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return ColumnType.Datetime;

            return ColumnType.String;
        }
    }
}
