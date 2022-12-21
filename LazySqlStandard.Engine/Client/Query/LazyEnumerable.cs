using Azure.Core.GeoJson;

namespace LazySql;

/// <summary>
/// LazyEnumerable
/// </summary>
/// <typeparam name="T"></typeparam>
internal sealed class LazyEnumerable<T> : ILazyEnumerable<T>
{
    private readonly ITableDefinition _tableDefinition;
    private readonly SelectQuery _selectQuery;

    internal LazyEnumerable(string schema, string tableName, Expression whereExpression, SqlQuery sqlQuery)
    {
        LazyClient.CheckInitialization(typeof(T), out _tableDefinition);
        _selectQuery = new SelectQuery(_tableDefinition, schema, tableName);
        if (whereExpression != null)
            _selectQuery.SetWhereQuery(new WhereExpressionQuery(whereExpression));
        if (!SqlQuery.IsEmpty(sqlQuery))
            _selectQuery.SetWhereQuery(new WhereSqlQuery(sqlQuery));
    }

    internal LazyEnumerable(SqlQuery sqlQuery)
    {
        LazyClient.CheckInitialization(typeof(T), out _tableDefinition);
        _selectQuery = new SelectQuery(_tableDefinition);
        _selectQuery.SetPreBuild(sqlQuery);
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

    public ILazyEnumerable<T> GroupBy(params Expression<Func<T, object>>[] groupByExpressions)
    {
        foreach (Expression<Func<T, object>> groupByExpression in groupByExpressions)
            _selectQuery.AddGroupBy(new GroupByExpressionQuery(groupByExpression));
        return this;
    }

    public ILazyEnumerable<T> GroupBy(params string[] columns)
    {
        foreach (string column in columns)
            _selectQuery.AddGroupBy(new GroupBySqlQuery(column));
        return this;
    }

    public ILazyEnumerable<T> Columns(params Expression<Func<T, object>>[] columnExpressions)
    {
        foreach (Expression<Func<T, object>> columnExpression in columnExpressions)
            _selectQuery.Columns(new ColumnExpressionQuery(columnExpression));
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

    public int Count
    {
        get
        {
            _selectQuery.CountRows = true;
            QueryBuilder queryBuilder  =_selectQuery.BuildQuery();
            return LazyClient.ExecuteScalar<int>(queryBuilder.GetQuery(), queryBuilder.GetArguments()); ;
        }
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
        if (!_tableDefinition.HasRelations || _selectQuery.DirectQuery)
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