namespace LazySql;

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

    public void Build(QueryBase queryBase)
    {
        queryBase.QueryBuilder.Append(_expression);
        queryBase.QueryBuilder.Append(_orderByDirection == OrderByDirection.Asc ? " ASC " : " DESC ");
    }
}