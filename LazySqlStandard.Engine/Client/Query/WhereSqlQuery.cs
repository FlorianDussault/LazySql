namespace LazySql;

/// <summary>
/// Where if SQL
/// </summary>
internal sealed class WhereSqlQuery : IWhereQuery
{
    private readonly string _whereSql;
    private readonly SqlArguments _sqlArguments;

    public bool HasValue => !string.IsNullOrWhiteSpace(_whereSql) || _sqlArguments.Count > 0;


    public WhereSqlQuery(SqlQuery sqlQuery)
    {
        _whereSql = sqlQuery.Query;
        _sqlArguments = sqlQuery.SqlArguments ?? new SqlArguments();
    }

    public void Build(QueryBase queryBase)
    {
        queryBase.QueryBuilder.Append(_whereSql);
        queryBase.QueryBuilder.AddSqlArguments(_sqlArguments);
    }

}