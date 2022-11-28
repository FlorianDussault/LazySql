namespace LazySql;

// ReSharper disable once ClassCannotBeInstantiated
public sealed partial class LazyClient
{
    #region Insert

    /// <summary>
    /// Insert an item
    /// </summary>
    /// <typeparam name="T">Type of the item</typeparam>
    /// <param name="obj">Item</param>
    /// <param name="tableName"></param>
    /// <param name="autoIncrementColumn"></param>
    /// <param name="excludedColumns">Excluded columns</param>
    public static int Insert(object obj, string tableName = null, string autoIncrementColumn = null, params string[] excludedColumns) => Instance.InternalInsert(obj, tableName, autoIncrementColumn, excludedColumns);

    /// <summary>
    /// Insert an item
    /// </summary>
    /// <param name="type">Type of the item</param>
    /// <param name="obj">Item</param>
    /// <param name="tableName"></param>
    /// <param name="autoIncrementColumn"></param>
    /// <param name="excludedColumns"></param>
    private int InternalInsert(object obj, string tableName, string autoIncrementColumnName, string[] excludedColumns)
    {
        CheckInitialization(obj.GetType(), out ITableDefinition tableDefinition);

        tableDefinition.GetColumns(out _, out IReadOnlyList<ColumnDefinition> columns, out _,
            out IReadOnlyList<ColumnDefinition> primaryKeys);

        InsertQuery insertQuery = new(obj, tableDefinition, tableName);
        insertQuery.SetAutoIncrementColumn(autoIncrementColumnName);
        insertQuery.SetExcludedColumns(excludedColumns);
        QueryBuilder queryBuilder = insertQuery.BuildQuery(out PropertyInfo autoIncrementProperty);

        if (autoIncrementProperty != null)
        {
            object output = ExecuteScalar(queryBuilder);
            autoIncrementProperty.SetValue(obj, output);
            return 1;
        }

        return ExecuteNonQuery(queryBuilder);
    }
    #endregion
}