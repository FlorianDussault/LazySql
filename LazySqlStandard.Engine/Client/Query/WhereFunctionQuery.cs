namespace LazySql.Engine.Client.Query;

/// <summary>
/// Where with Action
/// </summary>
internal sealed class WhereFunctionQuery : IWhereQuery
{
    private readonly Action<SelectQuery> _action;

    public WhereFunctionQuery(Action<SelectQuery> action) => _action = action;

    public void Build(SelectQuery selectQuery)
    {
        _action(selectQuery);
    }
}