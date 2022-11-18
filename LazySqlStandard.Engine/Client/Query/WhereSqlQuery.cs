namespace LazySql.Engine.Client.Query;

/// <summary>
/// Where if SQL
/// </summary>
internal sealed class WhereSqlQuery : IWhereQuery
{
    private readonly string _whereSql;
    private readonly SqlArguments _sqlArguments;

    public WhereSqlQuery(string whereSql, SqlArguments sqlArguments)
    {
        _whereSql = whereSql;
        _sqlArguments = sqlArguments;
    }

    public void Build(SelectQuery selectQuery)
    {
        selectQuery.QueryBuilder.Append(_whereSql);
        selectQuery.QueryBuilder.AddSqlArguments(_sqlArguments);
    }
}