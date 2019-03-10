using System;
using System.Collections.Generic;
using System.Linq;
using Google.DataTable.Net.Wrapper;
using Microsoft.AspNetCore.Identity.UI.Pages.Internal.Account;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NetflixStatizier.Helper
{
    public static class GoogleDataTableBuilder
    {
        public static string GetDataTableJsonFromDictionnary<TKey, TValue>(IDictionary<TKey, TValue> data)
        {
            var dataTable = new DataTable();
            dataTable.AddColumn(new Column(GetColumnTypeFromType(data.Keys.First().GetType())));

            return dataTable.GetJson();
        }

        private static ColumnType GetColumnTypeFromType(Type type)
        {
            throw new NotImplementedException();
            //switch (type.)
            //{
            //    case Int32.:
            //    case double:
            //    case float:
            //        return ColumnType.Number;
            //}
        }
    }
}
