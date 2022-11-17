namespace LazySql.Engine.Client.Query;

/// <summary>
/// Where with Expression
/// </summary>
internal sealed class WhereExpressionQuery : IWhereQuery
{
    private readonly Expression _expression;
    public WhereExpressionQuery(Expression expression) => _expression = expression;

    public void Build(SelectQuery selectQuery) => selectQuery.QueryBuilder.Append(_expression);
}