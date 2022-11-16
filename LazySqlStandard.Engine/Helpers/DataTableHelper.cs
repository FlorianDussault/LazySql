namespace LazySql.Engine.Helpers
{
    internal static class DataTableHelper
    {
        public static IEnumerable<dynamic> ToDynamic(this DataTable dataTable)
        {
            foreach (DataRow dataRow in dataTable.Rows)
            {
                IDictionary<string, object> obj = new ExpandoObject();
                for (int i = 0; i < dataTable.Columns.Count; i++)
                    obj.Add(dataTable.Columns[i].ColumnName,
                        dataRow.IsNull(dataTable.Columns[i].ColumnName) ? null : dataRow[i]);
                yield return obj;
            }
        }

        public static IEnumerable<T> ToLazyObject<T>(this DataTable dataTable)
        {
            LazyClient.CheckInitialization(typeof(T), out TableDefinition tableDefinition);
            tableDefinition.GetColumns(out IReadOnlyList<ColumnDefinition> allColumns, out _, out _, out _);

            List<ColumnDefinition> columnDefinitions = dataTable.Columns.Cast<DataColumn>()
                .Select(dataTableColumn => allColumns.FirstOrDefault(c => string.Equals(c.Column.ColumnName,
                    dataTableColumn.ColumnName, StringComparison.InvariantCultureIgnoreCase)))
                .Where(col => col != null).ToList();


            foreach (DataRow dataRow in dataTable.Rows)
            {
                object instance = Activator.CreateInstance(tableDefinition.TableType);
                foreach (ColumnDefinition columnDefinition in columnDefinitions)
                    columnDefinition.PropertyInfo.SetValue(instance, dataRow.IsNull(dataTable.Columns[columnDefinition.Column.ColumnName].ColumnName) ? null : dataRow[columnDefinition.Column.ColumnName]);
                yield return (T)instance;
            }
        }
    }
}
