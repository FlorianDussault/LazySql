namespace LazySql;

internal class DeleteQuery : QueryBase
{
    public DeleteQuery(ITableDefinition tableDefinition,string schema, string tableName) : base(tableDefinition, schema, tableName)
    {
    }


    public virtual QueryBuilder BuildQuery()
    {
        QueryBuilder.Append($"DELETE FROM {SqlHelper.TableName(Schema, TableName)}");

        if (WhereQuery != null)
        {
            QueryBuilder.Append(" WHERE ");
            WhereQuery.Build(this);
        }
        
        return QueryBuilder;
    }
}