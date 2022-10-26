using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LazySql.Engine.Attributes;

namespace LazySql.Engine.Definitions
{
    internal sealed class TableDefinition : List<ColumnDefinition>
    {
        public Type TableType { get; }
        public LazyTable Table { get; }
        public RelationsInformation OneToManyExpressions { get; set; } = null;
        public TableDefinition(Type type, LazyTable table)
        {
            TableType = type;
            Table = table;
        }
        public void Add(PropertyInfo propertyInfo, LazyColumn column, PrimaryKey primaryKey)
        {
            Add(new ColumnDefinition(propertyInfo, column, Count > 0 ? this.Max(c => c.Index) + 1 : 0, primaryKey));
        }

        private IEnumerable<ColumnDefinition> GetColumns()
        {
            return this.OrderBy(c => c.Index);
        }

        public void GetColumns(out IReadOnlyList<ColumnDefinition> allColumns, out IReadOnlyList<ColumnDefinition> columnsWithoutAutoIncrement, out IReadOnlyList<ColumnDefinition> columnsWithoutPrimaryKeys, out IReadOnlyList<ColumnDefinition> primaryKeys)
        {
            List<ColumnDefinition> listAllColumns = new List<ColumnDefinition>();
            List<ColumnDefinition> listColumnsWithoutAutoIncrement = new List<ColumnDefinition>();
            List<ColumnDefinition> listColumnsWithoutPrimaryKeys = new List<ColumnDefinition>();
            List<ColumnDefinition> listPrimaryKeys = new List<ColumnDefinition>();

            foreach (ColumnDefinition columnDefinition in GetColumns())
            {
                listAllColumns.Add(columnDefinition);
                if (columnDefinition.PrimaryKey == null || !columnDefinition.PrimaryKey.AutoIncrement)
                    listColumnsWithoutAutoIncrement.Add(columnDefinition);
                if (columnDefinition.PrimaryKey == null)
                    listColumnsWithoutPrimaryKeys.Add(columnDefinition);
                else
                    listPrimaryKeys.Add(columnDefinition);
            }

            allColumns = listAllColumns;
            columnsWithoutAutoIncrement = listColumnsWithoutAutoIncrement;
            columnsWithoutPrimaryKeys = listColumnsWithoutPrimaryKeys;
            primaryKeys = listPrimaryKeys;
        }

        public ColumnDefinition GetColumn(string propertyName)
        {
            return this.FirstOrDefault(c => string.Equals(c.PropertyInfo.Name, propertyName, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}