namespace LazySql;

/// <summary>
/// Where with Expression
/// </summary>
internal sealed class WhereExpressionQuery : IWhereQuery
{
    private readonly Expression _expression;

    public bool HasValue => _expression != null;

    public WhereExpressionQuery(Expression expression) => _expression = expression;

    public void Build(QueryBase queryBase) => queryBase.QueryBuilder.Append(_expression);
    
}