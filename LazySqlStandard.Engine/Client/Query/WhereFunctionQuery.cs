namespace LazySql;

/// <summary>
/// Where with Action
/// </summary>
internal sealed class WhereFunctionQuery : IWhereQuery
{
    private readonly Action<QueryBase> _action;
    public bool HasValue => _action != null;

    public WhereFunctionQuery(Action<QueryBase> action) => _action = action;

    public void Build(QueryBase queryBase)
    {
        _action(queryBase);
    }

}