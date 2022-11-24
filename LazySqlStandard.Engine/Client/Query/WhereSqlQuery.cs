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

    public void Build(QueryBase queryBase)
    {
        queryBase.QueryBuilder.Append(_whereSql);
        queryBase.QueryBuilder.AddSqlArguments(_sqlArguments);
    }
}