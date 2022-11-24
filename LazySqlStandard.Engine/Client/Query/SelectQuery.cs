namespace LazySql.Engine.Client.Query;

/// <summary>
/// Select Query
/// </summary>
internal sealed class SelectQuery
{
    /// <summary>
    /// Query Builder
    /// </summary>
    internal QueryBuilder QueryBuilder { get; }
    
    /// <summary>
    /// Table Definition
    /// </summary>
    private ITableDefinition TableDefinition { get; }

    /// <summary>
    /// Alias Name
    /// </summary>
    public string TableAlias { get; }

    private readonly string _tableName;
    private IWhereQuery _whereQuery;
    private readonly List<IOrderByQuery> _orderByQueries;
    private int? _top;

    public SelectQuery(ITableDefinition tableDefinition, string tableName = null)
    {
        TableDefinition = tableDefinition;
        QueryBuilder = new QueryBuilder(tableDefinition);
        _orderByQueries = new List<IOrderByQuery>();
        _tableName = tableDefinition.GetTableName(tableName);
        TableAlias = $"{_tableName}_{Guid.NewGuid().ToString().Substring(0, 4)}";
    }

    /// <summary>
    /// Set WHERE
    /// </summary>
    /// <param name="whereQuery"></param>
    public void SetWhereQuery(IWhereQuery whereQuery) => _whereQuery = whereQuery;

    /// <summary>
    /// Set ORDER BY
    /// </summary>
    /// <param name="orderByQuery"></param>
    public void AddOrderBy(IOrderByQuery orderByQuery) => _orderByQueries.Add(orderByQuery);

    /// <summary>
    /// SET TOP
    /// </summary>
    /// <param name="top"></param>
    public void SetTop(int? top) => _top = top;

    /// <summary>
    /// Build Query
    /// </summary>
    /// <returns></returns>
    public QueryBuilder BuildQuery()
    {
        QueryBuilder.Append("SELECT ");

        #region TOP
        if (_top != null)
            QueryBuilder.Append($"TOP {_top} ");
        #endregion

        #region Columns

        if (TableDefinition.ObjectType != ObjectType.LazyObject)
        {
            QueryBuilder.Append("* ");
        }
        else
        {
            TableDefinition.GetColumns(out IReadOnlyList<ColumnDefinition> allColumns, out _, out _, out _);
            string columnsList = string.Join(", ", allColumns.Where(c => c.Column.SqlType != SqlType.Children).Select(c => $"{TableAlias}.{c.Column.SqlColumnName}"));
            QueryBuilder.Append($"{columnsList} ");
        }
        #endregion

        #region FROM
        QueryBuilder.Append($"FROM {_tableName} AS {TableAlias} ");
        #endregion

        #region WHERE

        if (_whereQuery != null)
        {
            QueryBuilder.Append("WHERE ");
            _whereQuery.Build(this);
        }

        #endregion

        #region ORDER BY

        if (_orderByQueries.Count > 0)
        {
            QueryBuilder.Append(" ORDER BY ");
            foreach ((bool isLast, IOrderByQuery value) tuple in _orderByQueries.ForeachWithLast())
            {
                tuple.value.Build(this);
                if (!tuple.isLast)
                    QueryBuilder.Append(", ");
            }
        }

        #endregion

        return QueryBuilder;
    }
}