namespace LazySql;

// ReSharper disable once ClassCannotBeInstantiated
public sealed partial class LazyClient
{
    /// <summary>
    /// Bulk Insert
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="values">Values</param>
    public static void BulkInsert<T>(IEnumerable<T> values) => Instance.InternalBulkInsert(null, null, values);

    /// <summary>
    /// Bulk Insert
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="tableName">Table Name</param>
    /// <param name="values">Values</param>
    public static void BulkInsert<T>(string tableName, IEnumerable<T> values) => Instance.InternalBulkInsert(null, tableName, values);

    public static void BulkInsert<T>(string schema, string tableName, IEnumerable<T> values) => Instance.InternalBulkInsert(schema, tableName, values);

    /// <summary>
    /// Bulk Insert
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="tableName">Table Name</param>
    /// <param name="values">Values</param>
    /// <exception cref="LazySqlException"></exception>
    private void InternalBulkInsert<T>(string schema, string tableName, IEnumerable<T> values)
    {
        CheckInitialization(typeof(T), out ITableDefinition tableDefinition);

        switch (tableDefinition.ObjectType)
        {
            case ObjectType.Dynamic when string.IsNullOrWhiteSpace(tableName):
                throw new LazySqlException($"You cannot call the {nameof(BulkInsert)} method with a Dynamic type without a table name in argument");
            case ObjectType.Dynamic:
                BulkInsertDynamic(schema, tableName, values);
                break;
            case ObjectType.LazyObject:
            case ObjectType.Object:
            default:
                BulkInsertObject(schema, tableName, tableDefinition, values);
                break;
        }
    }

    /// <summary>
    /// Bulk Insert of Lazy and objects
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="tableName">Table Name</param>
    /// <param name="tableDefinition">Table Definition</param>
    /// <param name="values">Values</param>
    private void BulkInsertObject<T>(string schema, string tableName, ITableDefinition tableDefinition, IEnumerable<T> values)
    {
        DataTable dataTable = new();

        tableDefinition.GetColumns(out _, out IReadOnlyList<ColumnDefinition> columnDefinitions, out _, out _);

        #region Create Columns

        foreach (ColumnDefinition column in columnDefinitions)
            dataTable.Columns.Add(column.Column.ColumnName, column.PropertyInfo.PropertyType);

        #endregion

        #region Create Rows
        foreach (object value in values)
        {
            DataRow row = dataTable.NewRow();
            for (int i = 0; i < columnDefinitions.Count; i++) row[i] = columnDefinitions[i].PropertyInfo.GetValue(value);
            dataTable.Rows.Add(row);
        }
        #endregion

        BulkInsert(tableDefinition.GetSchema(schema), tableDefinition.GetTableName(tableName), dataTable);
    }

    /// <summary>
    /// Bulk Insert of dynamic
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="schema"></param>
    /// <param name="tableName">Table name</param>
    /// <param name="values">Values</param>
    private void BulkInsertDynamic<T>(string schema, string tableName, IEnumerable<T> values)
    {
        DataTable dataTable = new();

        using IEnumerator<T> enumerator = values.GetEnumerator();

        if (!enumerator.MoveNext()) return;
        
        #region Create Columns

        List<PropertyInfo> propertyInfos = new();
        foreach (PropertyInfo propertyInfo in enumerator.Current.GetType().GetProperties())
        {
            propertyInfos.Add(propertyInfo);
            dataTable.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType);
        }
        #endregion

        #region Create Rows

        do
        {
            DataRow row = dataTable.NewRow();
            for (int i = 0; i < propertyInfos.Count; i++) row[i] = propertyInfos[i].GetValue(enumerator.Current);
            dataTable.Rows.Add(row);

        } while(enumerator.MoveNext());

        #endregion

        BulkInsert(schema, tableName, dataTable);

    }

    private void BulkInsert(string schema, string tableName, DataTable dataTable)
    {
        using SqlConnector sqlConnector = Open();
        sqlConnector.BulkInsert(schema, tableName, dataTable);
    }
}