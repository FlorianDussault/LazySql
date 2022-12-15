namespace LazySql;

// ReSharper disable once ClassCannotBeInstantiated
public sealed partial class LazyClient
{
    #region Insert

    public static int Insert(object obj) => Instance.InternalInsert(null, null, obj, null, Array.Empty<string>());

    public static int Insert(string tableName, object obj) => Instance.InternalInsert(null, tableName, obj, null, Array.Empty<string>());
    public static int Insert(string schema, string tableName, object obj) => Instance.InternalInsert(schema, tableName, obj, null, Array.Empty<string>());

    public static int Insert(object obj, string autoIncrementColumn, params string[] excludedColumns) => Instance.InternalInsert(null, null, obj, autoIncrementColumn, excludedColumns);

    public static int Insert(string tableName, object obj, string autoIncrementColumn, params string[] excludedColumns) => Instance.InternalInsert(null, tableName, obj, autoIncrementColumn, excludedColumns);

    public static int Insert(string schema, string tableName, object obj, string autoIncrementColumn, params string[] excludedColumns) => Instance.InternalInsert(schema, tableName, obj, autoIncrementColumn, excludedColumns);

    public static int Insert(object obj, string autoIncrementColumn) => Instance.InternalInsert(null, null, obj, autoIncrementColumn, Array.Empty<string>());

    public static int Insert(string tableName, object obj, string autoIncrementColumn) => Instance.InternalInsert(null, tableName, obj, autoIncrementColumn, Array.Empty<string>());

    public static int Insert(string schema, string tableName, object obj, string autoIncrementColumn) => Instance.InternalInsert(schema, tableName, obj, autoIncrementColumn, Array.Empty<string>());


    /// <summary>
    /// Insert an item
    /// </summary>
    /// <param name="type">Type of the item</param>
    /// <param name="obj">Item</param>
    /// <param name="schema"></param>
    /// <param name="tableName"></param>
    /// <param name="autoIncrementColumn"></param>
    /// <param name="excludedColumns"></param>
    private int InternalInsert(string schema, string tableName, object obj, string autoIncrementColumnName, string[] excludedColumns)
    {
        CheckInitialization(obj.GetType(), out ITableDefinition tableDefinition);

        tableDefinition.GetColumns(out _, out IReadOnlyList<ColumnDefinition> columns, out _,
            out IReadOnlyList<ColumnDefinition> primaryKeys);

        InsertQuery insertQuery = new(obj, tableDefinition, schema, tableName);
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