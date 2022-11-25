namespace LazySql;

/// <summary>
/// Query
/// </summary>
internal interface IQuery
{
    void Build(QueryBase queryBase);
}