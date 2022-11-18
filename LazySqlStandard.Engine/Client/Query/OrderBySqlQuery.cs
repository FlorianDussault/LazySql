namespace LazySql.Engine.Client.Query;

/// <summary>
/// OrderBy With Sql
/// </summary>
internal sealed class OrderBySqlQuery : IOrderByQuery
{
    private readonly string _columnName;
    private readonly OrderByDirection _orderByDirection;

    public OrderBySqlQuery(string columnName, OrderByDirection orderByDirection)
    {
        _columnName = columnName;
        _orderByDirection = orderByDirection;
    }
    public void Build(SelectQuery selectQuery)
    {
        selectQuery.QueryBuilder.Append(_columnName);
        selectQuery.QueryBuilder.Append(_orderByDirection == OrderByDirection.Asc ? " ASC " : " DESC ");
    }
}