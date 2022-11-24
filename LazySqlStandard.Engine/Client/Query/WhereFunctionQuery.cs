namespace LazySql.Engine.Client.Query;

/// <summary>
/// Where with Action
/// </summary>
internal sealed class WhereFunctionQuery : IWhereQuery
{
    private readonly Action<QueryBase> _action;

    public WhereFunctionQuery(Action<QueryBase> action) => _action = action;

    public void Build(QueryBase queryBase)
    {
        _action(queryBase);
    }
}