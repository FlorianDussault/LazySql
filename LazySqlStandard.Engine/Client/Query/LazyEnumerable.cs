// ReSharper disable once CheckNamespace

namespace LazySql.Engine.Client.Query;

/// <summary>
/// LazyEnumerable
/// </summary>
/// <typeparam name="T"></typeparam>
internal sealed class LazyEnumerable<T> : ILazyEnumerable<T>
{
    private readonly TableDefinition _tableDefinition;
    private readonly SelectQuery _selectQuery;

    internal LazyEnumerable(string tableName)
    {
        LazyClient.CheckInitialization(typeof(T), out _tableDefinition);
        _selectQuery = new SelectQuery(_tableDefinition, tableName);
    }

    /// <summary>
    /// Where with expression
    /// </summary>
    /// <param name="whereExpression">Expression</param>
    /// <returns>IEnumerable</returns>
    public ILazyEnumerable<T> Where(Expression<Func<T, bool>> whereExpression)
    {
        _selectQuery.SetWhereQuery(new WhereExpressionQuery(whereExpression));
        return this;
    }

    /// <summary>
    /// Where in SQL
    /// </summary>
    /// <param name="whereSql">SQL</param>
    /// <param name="sqlArguments">Arguments</param>
    /// <returns>IEnumerable</returns>
    public ILazyEnumerable<T> Where(string whereSql, SqlArguments sqlArguments)
    {
        _selectQuery.SetWhereQuery(new WhereSqlQuery(whereSql, sqlArguments));
        return this;
    }

    /// <summary>
    /// OrderBy ASC with expression
    /// </summary>
    /// <param name="orderByExpression">Expression</param>
    /// <returns>IEnumerable</returns>
    public ILazyEnumerable<T> OrderByAsc(Expression<Func<T, object>> orderByExpression)
    {
        _selectQuery.AddOrderBy(new OrderByExpressionQuery(orderByExpression, OrderByDirection.Asc));
        return this;
    }

    /// <summary>
    /// OrderBy ASC with column (as text)
    /// </summary>
    /// <param name="columns">Columns</param>
    /// <returns>IEnumerable</returns>
    public ILazyEnumerable<T> OrderByAsc(params string[] columns)
    {
        foreach (string column in columns)
            _selectQuery.AddOrderBy(new OrderBySqlQuery(column, OrderByDirection.Asc));
        return this;
    }

    /// <summary>
    /// OrderBy DESC with expression
    /// </summary>
    /// <param name="orderByExpression">Expression</param>
    /// <returns>IEnumerable</returns>
    public ILazyEnumerable<T> OrderByDesc(Expression<Func<T, object>> orderByExpression)
    {
        _selectQuery.AddOrderBy(new OrderByExpressionQuery(orderByExpression, OrderByDirection.Desc));
        return this;
    }

    /// <summary>
    /// OrderBy DESC with column (as text)
    /// </summary>
    /// <param name="columns">Columns</param>
    /// <returns>IEnumerable</returns>
    public ILazyEnumerable<T> OrderByDesc(params string[] columns)
    {
        foreach (string column in columns)
            _selectQuery.AddOrderBy(new OrderBySqlQuery(column, OrderByDirection.Desc));
        return this;
    }

    /// <summary>
    /// TOP
    /// </summary>
    /// <param name="top">Number of rows</param>
    /// <returns>IEnumerable</returns>
    public ILazyEnumerable<T> Top(int top)
    {
        _selectQuery.SetTop(top);
        return this;
    }

    /// <summary>
    /// GetEnumerator
    /// </summary>
    /// <returns>Enumerator</returns>
    public IEnumerator<T> GetEnumerator() => Execute().Cast<T>().GetEnumerator();

    /// <summary>
    /// GetEnumerator
    /// </summary>
    /// <returns>IEnumerator</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Execute Query
    /// </summary>
    /// <returns></returns>
    private IEnumerable Execute()
    {
        if (_tableDefinition.Relations.Count == 0)
        {
            foreach (object o in LazyClient.GetWithQuery(typeof(T), _selectQuery))
                yield return o;
            yield break;
        }

        List<object> values = LazyClient.GetWithQuery(typeof(T), _selectQuery).ToList();
        if (values.Count == 0) yield break;

        foreach (RelationInformation relation in _tableDefinition.Relations)
            LazyClient.LoadChildren(typeof(T), relation, values);

        foreach (object value in values)
            yield return value;
    }
}