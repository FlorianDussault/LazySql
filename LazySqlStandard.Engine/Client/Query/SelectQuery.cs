using System.Linq;

namespace LazySql;

/// <summary>
/// Select Query
/// </summary>
internal sealed class SelectQuery : QueryBase
{
    private readonly List<IOrderByQuery> _orderByQueries;
    private readonly List<IGroupByQuery> _groupByQueries;
    private readonly List<IColumnQuery> _columnQueries;
    private int? _top;
    public bool DirectQuery { get; } = false;
    public bool CountRows { get; set; } = false;

    private SqlQuery _preBuildQuery = null;

    public SelectQuery(ITableDefinition tableDefinition, string schema = null, string tableName = null) : base(tableDefinition, schema, tableName)
    {
        _orderByQueries = new List<IOrderByQuery>();
        _groupByQueries = new List<IGroupByQuery>();
        _columnQueries = new List<IColumnQuery>();
    }

    /// <summary>
    /// Set ORDER BY
    /// </summary>
    /// <param name="orderByQuery"></param>
    public void AddOrderBy(IOrderByQuery orderByQuery) => _orderByQueries.Add(orderByQuery);

    public void AddGroupBy(IGroupByQuery groupByQuery) => _groupByQueries.Add(groupByQuery);

    public void Columns(IColumnQuery columnQuery) => _columnQueries.Add(columnQuery);

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
        QueryBuilder.Reset();

        if (!SqlQuery.IsEmpty(_preBuildQuery))
        {
            QueryBuilder.Append(_preBuildQuery.Query);
            QueryBuilder.AddSqlArguments(_preBuildQuery.SqlArguments);
            return QueryBuilder;
        }


        QueryBuilder.Append("SELECT ");

        #region TOP
        if (_top != null)
            QueryBuilder.Append($"TOP {_top} ");
        #endregion

        #region Columns

        if (CountRows)
        {
            QueryBuilder.Append(" COUNT(1) ");
        }
        else if (_columnQueries.Count > 0)
        {
            foreach ((bool isLast, IColumnQuery value) valueTuple in _columnQueries.ForeachWithLast())
            {
                valueTuple.value.Build(this);
                if (!valueTuple.isLast)
                    QueryBuilder.Append(", ");
            }
        }
        else if (TableDefinition.ObjectType != ObjectType.LazyObject)
        {
            QueryBuilder.Append(" * ");
        }
        else
        {
            TableDefinition.GetColumns(out IReadOnlyList<ColumnDefinition> allColumns, out _, out _, out _);
            string columnsList = string.Join(", ", allColumns.Where(c => c.Column.SqlType != SqlType.Children).Select(c => $"{TableAlias}.{c.Column.SqlColumnName}"));
            QueryBuilder.Append($"{columnsList} ");
        }
        #endregion

        #region FROM
        QueryBuilder.Append($" FROM {SqlHelper.TableName(Schema, TableName)} AS {TableAlias} ");
        #endregion

        #region WHERE

        if (WhereQuery != null)
        {
            QueryBuilder.Append("WHERE ");
            WhereQuery.Build(this);
        }

        #endregion

        #region GROUP BY

        if (_groupByQueries.Count > 0)
        {
            QueryBuilder.Append(" GROUP BY ");
            foreach ((bool isLast, IGroupByQuery groupBy) valueTuple in _groupByQueries.ForeachWithLast())
            {
                valueTuple.groupBy.Build(this);
                if (!valueTuple.isLast)
                    QueryBuilder.Append(", ");
            }
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

    public void SetPreBuild(SqlQuery sqlQuery)
    {
        _preBuildQuery = sqlQuery;
    }
}