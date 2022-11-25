namespace LazySql;

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
        
        return QueryBuilder;
    }
}