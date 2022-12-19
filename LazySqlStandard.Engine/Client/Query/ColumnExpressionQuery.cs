namespace LazySql;

internal sealed class ColumnExpressionQuery : IColumnQuery
{
    private readonly Expression _expression;

    public ColumnExpressionQuery(Expression expression)
    {
        _expression = expression;
    }

    public void Build(QueryBase queryBase)
    {
        LambdaParser.Parse(_expression, queryBase.TableDefinition, queryBase.QueryBuilder);
    }
}