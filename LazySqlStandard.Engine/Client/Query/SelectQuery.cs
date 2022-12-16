using System.Linq;

namespace LazySql;

/// <summary>
/// Select Query
/// </summary>
internal sealed class SelectQuery : QueryBase
{
    private readonly List<IOrderByQuery> _orderByQueries;
    private int? _top;
    public bool DirectQuery { get; } = false;

    private SqlQuery _preBuildQuery = null;

    public SelectQuery(ITableDefinition tableDefinition, string schema = null, string tableName = null) : base(tableDefinition, schema, tableName)
    {
        _orderByQueries = new List<IOrderByQuery>();
    }

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

        QueryBuilder.Append($"FROM {SqlHelper.TableName(Schema, TableName)} AS {TableAlias} ");
        #endregion

        #region WHERE

        if (WhereQuery != null)
        {
            QueryBuilder.Append("WHERE ");
            WhereQuery.Build(this);
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