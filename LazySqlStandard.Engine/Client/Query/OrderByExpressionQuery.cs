namespace LazySql.Engine.Client.Query;

/// <summary>
/// OrderBy With Expression
/// </summary>
internal sealed class OrderByExpressionQuery : IOrderByQuery
{
    private readonly Expression _expression;
    private readonly OrderByDirection _orderByDirection;

    public OrderByExpressionQuery(Expression expression, OrderByDirection orderByDirection)
    {
        _expression = expression;
        _orderByDirection = orderByDirection;
    }

    public void Build(SelectQuery selectQuery)
    {
        selectQuery.QueryBuilder.Append(_expression);
        selectQuery.QueryBuilder.Append(_orderByDirection == OrderByDirection.Asc ? " ASC " : " DESC ");
    }
}