

// ReSharper disable once CheckNamespace
namespace LazySql.Engine.Client;

// ReSharper disable once ClassCannotBeInstantiated
public sealed partial class LazyClient
{
    #region Update

    /// <summary>
    /// Update an item
    /// </summary>
    /// <typeparam name="T">Type of Item</typeparam>
    /// <param name="obj">Item</param>
    /// <param name="where"></param>
    /// <param name="excludedColumns"></param>
    public static int Update<T>(T obj, Expression<Func<T,bool>> where = null, params string[] excludedColumns) => Instance.InternalUpdate(typeof(T), obj, null, where, null, excludedColumns);

    public static int Update<T>(T obj, string tableName = null, Expression<Func<T, bool>> where = null, params string[] excludedColumns) => Instance.InternalUpdate(typeof(T), obj, null, where, null, excludedColumns);


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="where"></param>
    /// <param name="excludedColumns"></param>
    /// <returns></returns>
    public static int Update<T>(T obj, string whereSql = null, params string[] excludedColumns) => Instance.InternalUpdate(typeof(T), obj, null, null, whereSql, excludedColumns);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="tableName"></param>
    /// <param name="where"></param>
    /// <param name="excludedColumns"></param>
    /// <returns></returns>
    public static int Update<T>(T obj, string tableName = null, string whereSql = null, params string[] excludedColumns) => Instance.InternalUpdate(typeof(T), obj, tableName, null, whereSql, excludedColumns);


    private int InternalUpdate(Type type, object obj, string tableName, Expression whereExpression, string whereSql, params string[] excludedColumns)
    {
        CheckInitialization(type, out ITableDefinition tableDefinition);
        return InternalUpdateLazy(tableDefinition, obj, tableName, whereExpression, whereSql, excludedColumns);
    }

    private int InternalUpdateLazy(ITableDefinition tableDefinition,  object obj, string tableName, Expression whereExpression, string whereSql, params string[] excludedColumns)
    {
        tableDefinition.GetColumns(out _, out IReadOnlyList<ColumnDefinition> columns, out _, out IReadOnlyList<ColumnDefinition> primaryKeys);

        UpdateQuery updateQuery = new(obj, tableDefinition, tableName);

        excludedColumns ??= Array.Empty<string>();
        foreach (ColumnDefinition column in columns.Where(c=> excludedColumns.All(ec => !string.Equals(ec,
                     c.Column.ColumnName, StringComparison.InvariantCultureIgnoreCase))))
            updateQuery.AddUpdatedValue(column);
        
        if (whereExpression == null && string.IsNullOrWhiteSpace(whereSql))
        {
            BinaryExpression binaryExpression = null;
            foreach (ColumnDefinition primaryKey in primaryKeys)
            {
                ParameterExpression objExpression = Expression.Parameter(tableDefinition.TableType, "source");
                MemberExpression member = Expression.Property(objExpression, primaryKey.PropertyInfo);
                ConstantExpression value = Expression.Constant(primaryKey.PropertyInfo.GetValue(obj));
                BinaryExpression childExpression = Expression.Equal(member, value);
                if (binaryExpression == null)
                {
                    binaryExpression = childExpression;
                    continue;
                }
                binaryExpression = Expression.AndAlso(binaryExpression, childExpression);
            }
            updateQuery.SetWhereQuery(new WhereExpressionQuery(binaryExpression));
        }
        else if (whereExpression != null)
        {
            updateQuery.SetWhereQuery(new WhereExpressionQuery(whereExpression));
        }
        else if (!string.IsNullOrWhiteSpace(whereSql))
        {
            updateQuery.SetWhereQuery(new WhereSqlQuery(whereSql, null));
        }

        

        QueryBuilder query = updateQuery.BuildQuery();
        using SqlConnector sqlConnector = Open();
        return sqlConnector.ExecuteNonQuery(query.GetQuery(), query.GetArguments());
    }
    #endregion

}