namespace LazySql;

// ReSharper disable once ClassCannotBeInstantiated
public sealed partial class LazyClient
{
    #region Update

    public static int Update<T>(T obj) => Instance.InternalUpdate(obj.GetType(), null, null, obj,  null, null, null);
    public static int Update<T>(string tableName, T obj) => Instance.InternalUpdate(obj.GetType(), null, tableName, obj, null, null, null);
    public static int Update<T>(string schema, string tableName, T obj) => Instance.InternalUpdate(obj.GetType(), schema, tableName, obj, null, null, null);

    public static int Update<T>(T obj, params string[] excludedColumns) => Instance.InternalUpdate(obj.GetType(), null, null, obj, null, null, excludedColumns);
    public static int Update<T>(string tableName, T obj, params string[] excludedColumns) => Instance.InternalUpdate(obj.GetType(), null, tableName, obj, null, null, excludedColumns);
    public static int Update<T>(string schema, string tableName, T obj, params string[] excludedColumns) => Instance.InternalUpdate(obj.GetType(), schema, tableName, obj, null, null, excludedColumns);

    public static int Update<T>(T obj, Expression<Func<T, bool>> where) => Instance.InternalUpdate(obj.GetType(), null, null, obj, where, null, null);
    public static int Update<T>(string tableName, T obj, Expression<Func<T, bool>> where) => Instance.InternalUpdate(obj.GetType(), null, tableName, obj, where, null, null);
    public static int Update<T>(string schema, string tableName, T obj, Expression<Func<T, bool>> where) => Instance.InternalUpdate(obj.GetType(), schema, tableName, obj, where, null, null);

    public static int Update<T>(T obj, Expression<Func<T, bool>> where, params string[] excludedColumns) => Instance.InternalUpdate(obj.GetType(), null, null, obj, where, null, excludedColumns);
    public static int Update<T>(string tableName, T obj, Expression<Func<T, bool>> where, params string[] excludedColumns) => Instance.InternalUpdate(obj.GetType(), null, tableName, obj, where, null, excludedColumns);
    public static int Update<T>(string schema, string tableName, T obj, Expression<Func<T, bool>> where, params string[] excludedColumns) => Instance.InternalUpdate(obj.GetType(), schema, tableName, obj, where, null, excludedColumns);

    public static int Update<T>(T obj, SqlQuery where) => Instance.InternalUpdate(obj.GetType(), null, null, obj, null, where, null);
    public static int Update<T>(string tableName, T obj, SqlQuery where) => Instance.InternalUpdate(obj.GetType(), null, tableName, obj, null, where, null);
    public static int Update<T>(string schema, string tableName, T obj, SqlQuery where) => Instance.InternalUpdate(obj.GetType(), schema, tableName, obj, null, where, null);

    public static int Update<T>(T obj, SqlQuery where, params string[] excludedColumns) => Instance.InternalUpdate(obj.GetType(), null, null, obj, null, where, excludedColumns);
    public static int Update<T>(string tableName, T obj, SqlQuery where, params string[] excludedColumns) => Instance.InternalUpdate(obj.GetType(), null, tableName, obj, null, where, excludedColumns);
    public static int Update<T>(string schema, string tableName, T obj, SqlQuery where, params string[] excludedColumns) => Instance.InternalUpdate(obj.GetType(), schema, tableName, obj, null, where, excludedColumns);


    private int InternalUpdate(Type type, string schema, string tableName, object obj,  Expression whereExpression, SqlQuery whereSql, params string[] excludedColumns)
    {
        CheckInitialization(type, out ITableDefinition tableDefinition);
        return InternalUpdateLazy(tableDefinition, schema, tableName, obj, whereExpression, whereSql, excludedColumns);
    }

    private int InternalUpdateLazy(ITableDefinition tableDefinition, string schema, string tableName, object obj, Expression whereExpression, SqlQuery sqlQuery, params string[] excludedColumns)
    {
        tableDefinition.GetColumns(out _, out IReadOnlyList<ColumnDefinition> columns, out _, out IReadOnlyList<ColumnDefinition> primaryKeys);

        UpdateQuery updateQuery = new(obj, tableDefinition, schema, tableName);

        excludedColumns ??= Array.Empty<string>();
        foreach (ColumnDefinition column in columns.Where(c=> excludedColumns.All(ec => !string.Equals(ec,
                     c.Column.ColumnName, StringComparison.InvariantCultureIgnoreCase))))
            updateQuery.AddUpdatedValue(column);
        
        if (whereExpression == null && SqlQuery.IsEmpty(sqlQuery) && tableDefinition.ObjectType != ObjectType.Dynamic)
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