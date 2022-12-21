namespace LazySql;

internal sealed class GroupBySqlQuery : IGroupByQuery
{
    private readonly string _columnName;

    public GroupBySqlQuery(string columnName)
    {
        _columnName = columnName;
    }

    public void Build(QueryBase queryBase)
    {
        queryBase.QueryBuilder.Append(_columnName);
    }
}