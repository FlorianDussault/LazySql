namespace LazySql;

internal sealed class GroupByExpressionQuery : IGroupByQuery
{
    private readonly Expression _expression;

    public GroupByExpressionQuery(Expression expression)
    {
        _expression = expression;
    }

    public void Build(QueryBase queryBase)
    {
        LambdaParser.Parse(_expression, queryBase.TableDefinition, queryBase.QueryBuilder);
    }
}