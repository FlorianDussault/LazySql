﻿namespace LazySql
{
    internal class UpdateQuery : QueryBase
    {
        private readonly object _obj;
        private List<ColumnDefinition> _columnsToUpdate;

        public UpdateQuery(object obj, ITableDefinition tableDefinition, string schema, string tableName) : base(tableDefinition, schema, tableName)
        {
            _obj = obj;
            _columnsToUpdate = new List<ColumnDefinition>();
        }

        public void AddUpdatedValue(ColumnDefinition columnDefinition)
        {
            _columnsToUpdate.Add(columnDefinition);
        }

        public virtual QueryBuilder BuildQuery()
        {
            QueryBuilder.Append($"UPDATE {SqlHelper.TableName(Schema, TableName)} SET ");

            List<string> values = new();
            foreach (ColumnDefinition columnToUpdate in _columnsToUpdate)
            {
                string argumentName = QueryBuilder.RegisterArgument(columnToUpdate.Column.SqlType, columnToUpdate.PropertyInfo.GetValue(_obj));
                values.Add($"{columnToUpdate.Column.SqlColumnName} = {argumentName}");
            }

            QueryBuilder.Append(string.Join(", ", values));

            if (WhereQuery is {HasValue: true})
            {
                QueryBuilder.Append(" WHERE ");
                WhereQuery.Build(this);
            }

            return QueryBuilder;
        }
    }
}
