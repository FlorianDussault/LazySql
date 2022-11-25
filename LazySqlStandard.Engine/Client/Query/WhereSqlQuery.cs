namespace LazySql;

/// <summary>
/// Where if SQL
/// </summary>
internal sealed class WhereSqlQuery : IWhereQuery
{
    private readonly string _whereSql;
    private readonly SqlArguments _sqlArguments;

    public WhereSqlQuery(SqlQuery sqlQuery)
    {
        _whereSql = sqlQuery.Query;
        _sqlArguments = sqlQuery.SqlArguments;
    }

    public void Build(QueryBase queryBase)
    {
        queryBase.QueryBuilder.Append(_whereSql);
        queryBase.QueryBuilder.AddSqlArguments(_sqlArguments);
    }
}