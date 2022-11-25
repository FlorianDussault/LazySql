namespace LazySql;

// ReSharper disable once ClassCannotBeInstantiated
public sealed partial class LazyClient
{
    #region Update

    public static int Update(object obj) => Instance.InternalUpdate(obj.GetType(), obj, null, null, null, null);

    public static int Update<T>(object obj, Expression<Func<T,bool>> where, params string[] excludedColumns) => Instance.InternalUpdate(obj.GetType(), obj, null, where, null, excludedColumns);

    public static int Update(object obj, SqlQuery where, params string[] excludedColumns) => Instance.InternalUpdate(obj.GetType(), obj, null, null, where, excludedColumns);

    public static int Update<T>(object obj, string tableName, Expression<Func<T, bool>> where, params string[] excludedColumns) => Instance.InternalUpdate(obj.GetType(), obj, tableName, where, null, excludedColumns);

    public static int Update(object obj, string tableName, SqlQuery where, params string[] excludedColumns) => Instance.InternalUpdate(obj.GetType(), obj, tableName, null, where, excludedColumns);

    private int InternalUpdate(Type type, object obj, string tableName, Expression whereExpression, SqlQuery whereSql, params string[] excludedColumns)
    {
        CheckInitialization(type, out ITableDefinition tableDefinition);
        return InternalUpdateLazy(tableDefinition, obj, tableName, whereExpression, whereSql, excludedColumns);
    }

    private int InternalUpdateLazy(ITableDefinition tableDefinition,  object obj, string tableName, Expression whereExpression, SqlQuery sqlQuery, params string[] excludedColumns)
    {
        tableDefinition.GetColumns(out _, out IReadOnlyList<ColumnDefinition> columns, out _, out IReadOnlyList<ColumnDefinition> primaryKeys);

        UpdateQuery updateQuery = new(obj, tableDefinition, tableName);

        excludedColumns ??= Array.Empty<string>();
        foreach (ColumnDefinition column in columns.Where(c=> excludedColumns.All(ec => !string.Equals(ec,
                     c.Column.ColumnName, StringComparison.InvariantCultureIgnoreCase))))
            updateQuery.AddUpdatedValue(column);
        
        if (whereExpression == null && SqlQuery.IsEmpty(sqlQuery))
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
        else if (!SqlQuery.IsEmpty(sqlQuery))
        {
            updateQuery.SetWhereQuery(new WhereSqlQuery(sqlQuery));
        }

        QueryBuilder query = updateQuery.BuildQuery();
        using SqlConnector sqlConnector = Open();
        return sqlConnector.ExecuteNonQuery(query.GetQuery(), query.GetArguments());
    }
    #endregion

}