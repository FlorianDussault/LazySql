using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using LazySql.Engine.Definitions;

namespace LazySql.Engine.Client.StoredProcedure
{
    public class StoredProcedureOutputTable
    {
        public DataTable DataTable { get; }

        internal StoredProcedureOutputTable()
        {

        }

        internal StoredProcedureOutputTable(DataTable dataTable)
        {
            DataTable = dataTable;
        }

        public IEnumerable<T> Parse<T>() where T : LazyBase
        {
            LazyClient.CheckInitialization(typeof(T), out TableDefinition tableDefinition);
            tableDefinition.GetColumns(out IReadOnlyList<ColumnDefinition> allColumns, out _, out _, out _);
            
            List<ColumnDefinition> columnDefinitions = DataTable.Columns.Cast<DataColumn>()
                .Select(dataTableColumn => allColumns.FirstOrDefault(c => string.Equals(c.Column.ColumnName,
                    dataTableColumn.ColumnName, StringComparison.InvariantCultureIgnoreCase)))
                .Where(col => col != null).ToList();


            foreach (DataRow dataRow in DataTable.Rows)
            {
                object instance = Activator.CreateInstance(tableDefinition.TableType);
                foreach (ColumnDefinition columnDefinition in columnDefinitions)
                    columnDefinition.PropertyInfo.SetValue(instance, dataRow.IsNull(DataTable.Columns[columnDefinition.Column.ColumnName].ColumnName) ? null :  dataRow[columnDefinition.Column.ColumnName]);
                yield return (T)instance;
            }
        }

        public IEnumerable<dynamic> Parse()
        {
            foreach (DataRow dataRow in DataTable.Rows)
            {
                IDictionary<string, object> obj = new ExpandoObject();
                for (int i = 0; i < DataTable.Columns.Count; i++)
                    obj.Add(DataTable.Columns[i].ColumnName,
                        dataRow.IsNull(DataTable.Columns[i].ColumnName) ? null : dataRow[i]);
                yield return obj;
            }
        }
    }
}