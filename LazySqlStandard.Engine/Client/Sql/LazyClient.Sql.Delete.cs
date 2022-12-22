using LazySql.Transaction;

namespace LazySql;

// ReSharper disable once ClassCannotBeInstantiated
public sealed partial class LazyClient
{
    #region Truncate

    /// <summary>
    /// Truncate table
    /// </summary>
    /// <typeparam name="T">Type of item</typeparam>
    public static void Truncate<T>() => Instance.InternalTruncate(typeof(T), null, null);

    /// <summary>
    /// Truncate Table
    /// </summary>
    /// <param name="tableName">Table Name</param>
    public static void Truncate(string tableName) => Instance.InternalTruncate(null, null, tableName);

    public static void Truncate(string schema, string tableName) => Instance.InternalTruncate(null, schema, tableName);

    /// <summary>
    /// Truncate table
    /// </summary>
    /// <param name="type">Type of object</param>
    /// <param name="tableName">Table Name</param>
    private void InternalTruncate(Type type, string schema, string tableName)
    {
        if (type != null)
        {
            CheckInitialization(type, out ITableDefinition tableDefinition);
            tableName = tableDefinition.GetSchemaAndTableName(tableName);
        }
        using SqlConnector sqlConnector = Open();
        sqlConnector.ExecuteNonQuery($"TRUNCATE TABLE {SqlHelper.TableName(schema, tableName)}");
    }

    #endregion

    #region Delete

    public static int Delete(string schema, string tableName) => Instance.InternalDelete(schema, tableName, null, null);

    public static int Delete(object obj) => Instance.InternalDelete(null, null, obj, null);
    public static int Delete(string tableName, object obj) => Instance.InternalDelete(null, tableName, obj, null);
    public static int Delete(string schema, string tableName, object obj) => Instance.InternalDelete(schema, tableName, obj, null);


    public static int Delete<T>(Expression<Func<T, bool>> where) => Instance.InternalDelete(null, null, typeof(T), where, null, null);

    public static int Delete<T>(string tableName, Expression<Func<T, bool>> where) => Instance.InternalDelete(null, tableName, typeof(T), where, null, null);

    public static int Delete<T>(string schema, string tableName, Expression<Func<T, bool>> where) => Instance.InternalDelete(schema, tableName, typeof(T), where, null, null);

    public static int Delete<T>(SqlQuery where = null) => Instance.InternalDelete(null, null, typeof(T), null, where, null);

    public static int Delete<T>(string tableName, SqlQuery where = null) => Instance.InternalDelete(null, tableName, typeof(T), null, where, null);

    public static int Delete<T>(string schema, string tableName, SqlQuery where = null) => Instance.InternalDelete(schema, tableName, typeof(T), null, where, null);

    /// <summary>
    /// Delete one or more items from the database
    /// </summary>
    /// <param name="type">Type of item</param>
    /// <param name="tableName"></param>
    /// <param name="expression">Filter Expression</param>
    internal int InternalDelete(string schema, string tableName, Type type, Expression expression, SqlQuery sqlQuery, LazyTransaction lazyTransaction)
    {
        CheckInitialization(type, out ITableDefinition tableDefinition);

        DeleteQuery deleteQuery = new(tableDefinition, schema, tableName);

        if (expression != null)
            deleteQuery.SetWhereQuery(new WhereExpressionQuery(expression));
        if (!SqlQuery.IsEmpty(sqlQuery))
            deleteQuery.SetWhereQuery(new WhereSqlQuery(sqlQuery));

        QueryBuilder queryBuilder = deleteQuery.BuildQuery();
        return ExecuteNonQuery(queryBuilder, lazyTransaction);
    }


    /// <summary>
    /// Delete an item from the database
    /// </summary>
    /// <param name="obj">Item</param>
    /// <param name="tableName"></param>
    internal int InternalDelete(string schema, string tableName, object obj, LazyTransaction lazyTransaction)
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

        return InternalDelete(schema, tableName, obj.GetType(), binaryExpression, null, lazyTransaction);
    }

    #endregion
}