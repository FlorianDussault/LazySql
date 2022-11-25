namespace LazySql.Engine.Client.Query
{
    internal class DeleteQuery : QueryBase
    {
        public DeleteQuery(ITableDefinition tableDefinition, string tableName) : base(tableDefinition, tableName)
        {
        }


        public override QueryBuilder BuildQuery()
        {
            QueryBuilder.Append($"DELETE FROM {TableName}");

            if (WhereQuery != null)
            {
                QueryBuilder.Append(" WHERE ");
                WhereQuery.Build(this);
            }

            //List<string> values = new();
            //foreach (ColumnDefinition columnToUpdate in _columnsToUpdate)
            //{
            //    string argumentName = QueryBuilder.RegisterArgument(columnToUpdate.Column.SqlType, columnToUpdate.PropertyInfo.GetValue(_obj));
            //    values.Add($"{columnToUpdate.Column.SqlColumnName} = {argumentName}");
            //}

            //QueryBuilder.Append(string.Join(", ", values));

            //if (WhereQuery != null)
            //{
            //    QueryBuilder.Append(" WHERE ");
            //    WhereQuery.Build(this);
            //}

            return QueryBuilder;
        }
    }
}
