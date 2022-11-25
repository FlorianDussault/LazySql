namespace LazySql;

// ReSharper disable once ClassCannotBeInstantiated
public sealed partial class LazyClient
{
    #region Truncate

    /// <summary>
    /// Truncate table
    /// </summary>
    /// <typeparam name="T">Type of item</typeparam>
    public static void Truncate<T>() => Instance.InternalTruncate(typeof(T));

    /// <summary>
    /// Truncate Table
    /// </summary>
    /// <param name="tableName">Table Name</param>
    public static void Truncate(string tableName) => Instance.InternalTruncate(null, tableName);

    /// <summary>
    /// Truncate table
    /// </summary>
    /// <param name="type">Type of object</param>
    /// <param name="tableName">Table Name</param>
    private void InternalTruncate(Type type, string tableName = null)
    {
        if (type != null)
        {
            CheckInitialization(type, out ITableDefinition tableDefinition);
            tableName = tableDefinition.GetTableName();
        }
        using SqlConnector sqlConnector = Open();
        sqlConnector.ExecuteNonQuery($"TRUNCATE TABLE {tableName}");
    }

    #endregion

    #region Delete

    public static int Delete(object obj) => Instance.InternalDelete(obj, null);

    public static int Delete(object obj, string tableName) => Instance.InternalDelete(obj, tableName);


    public static int Delete<T>(Expression<Func<T, bool>> where) => Instance.InternalDelete(typeof(T), null, where, null);

    public static int Delete<T>(string tableName, Expression<Func<T, bool>> where) => Instance.InternalDelete(typeof(T), tableName, where, null);

    public static int Delete<T>(SqlQuery where) => Instance.InternalDelete(typeof(T), null, null, where);

    public static int Delete<T>(string tableName = null, SqlQuery where = null) => Instance.InternalDelete(typeof(T), tableName, null, where);

    /// <summary>
    /// Delete one or more items from the database
    /// </summary>
    /// <param name="type">Type of item</param>
    /// <param name="tableName"></param>
    /// <param name="expression">Filter Expression</param>
    private int InternalDelete(Type type, string tableName, Expression expression, SqlQuery sqlQuery)
    {
        CheckInitialization(type, out ITableDefinition tableDefinition);

        DeleteQuery deleteQuery = new(tableDefinition, tableName);

        if (expression != null)
            deleteQuery.SetWhereQuery(new WhereExpressionQuery(expression));
        if (!SqlQuery.IsEmpty(sqlQuery))
            deleteQuery.SetWhereQuery(new WhereSqlQuery(sqlQuery));

        QueryBuilder queryBuilder = deleteQuery.BuildQuery();
        return ExecuteNonQuery(queryBuilder.GetQuery(), queryBuilder.GetArguments());
    }


    /// <summary>
    /// Delete an item from the database
    /// </summary>
    /// <param name="obj">Item</param>
    /// <param name="tableName"></param>
    private int InternalDelete(object obj, string tableName)
    {
        CheckInitialization(obj.GetType(), out ITableDefinition tableDefinition);
        tableDefinition.GetColumns(out _, out _, out _, out IReadOnlyList<ColumnDefinition> primaryKeys);

        BinaryExpression binaryExpression = null;
        foreach (ColumnDefinition primaryKey in primaryKeys)
        {
            ParameterExpression objExpression = Expression.Parameter(obj.GetType(), "source");
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

        return InternalDelete(obj.GetType(), tableName, binaryExpression, null);
    }

    #endregion
}