namespace LazySql.Engine.Definitions;

/// <summary>
/// Table Definition
/// </summary>
internal sealed class TableDefinition : List<ColumnDefinition>
{
    /// <summary>
    /// Type of object
    /// </summary>
    public Type TableType { get; }

    /// <summary>
    /// LazyTable attribute
    /// </summary>
    public LazyTable Table { get; }

    /// <summary>
    /// Relations with other objects
    /// </summary>
    public RelationsInformation Relations { get; set; } = null;

    public TableDefinition(Type type, LazyTable table)
    {
        TableType = type;
        Table = table;
    }

    /// <summary>
    /// Add column
    /// </summary>
    /// <param name="propertyInfo">Object property</param>
    /// <param name="column">Column attribute</param>
    /// <param name="primaryKey">Primary Key information</param>
    public void Add(PropertyInfo propertyInfo, LazyColumn column, PrimaryKey primaryKey)
    {
        Add(new ColumnDefinition(propertyInfo, column, Count > 0 ? this.Max(c => c.Index) + 1 : 0, primaryKey));
    }

    /// <summary>
    /// Get Columns
    /// </summary>
    /// <returns></returns>
    private IEnumerable<ColumnDefinition> GetColumns()
    {
        return this.OrderBy(c => c.Index);
    }

    /// <summary>
    /// Get Columns
    /// </summary>
    /// <param name="allColumns">All columns</param>
    /// <param name="columnsWithoutAutoIncrement">Columns without auto increment</param>
    /// <param name="columnsWithoutPrimaryKeys">Columns without primary keys</param>
    /// <param name="primaryKeys">Primary Keys</param>
    public void GetColumns(out IReadOnlyList<ColumnDefinition> allColumns, out IReadOnlyList<ColumnDefinition> columnsWithoutAutoIncrement, out IReadOnlyList<ColumnDefinition> columnsWithoutPrimaryKeys, out IReadOnlyList<ColumnDefinition> primaryKeys)
    {
        List<ColumnDefinition> listAllColumns = new List<ColumnDefinition>();
        List<ColumnDefinition> listColumnsWithoutAutoIncrement = new List<ColumnDefinition>();
        List<ColumnDefinition> listColumnsWithoutPrimaryKeys = new List<ColumnDefinition>();
        List<ColumnDefinition> listPrimaryKeys = new List<ColumnDefinition>();

        foreach (ColumnDefinition columnDefinition in GetColumns())
        {
            listAllColumns.Add(columnDefinition);
            if (columnDefinition.PrimaryKey == null || !columnDefinition.PrimaryKey.AutoIncrement)
                listColumnsWithoutAutoIncrement.Add(columnDefinition);
            if (columnDefinition.PrimaryKey == null)
                listColumnsWithoutPrimaryKeys.Add(columnDefinition);
            else
                listPrimaryKeys.Add(columnDefinition);
        }

        allColumns = listAllColumns;
        columnsWithoutAutoIncrement = listColumnsWithoutAutoIncrement;
        columnsWithoutPrimaryKeys = listColumnsWithoutPrimaryKeys;
        primaryKeys = listPrimaryKeys;
    }

    /// <summary>
    /// Get column definition
    /// </summary>
    /// <param name="propertyName">Property Name</param>
    /// <returns>Column Definition</returns>
    public ColumnDefinition GetColumn(string propertyName)
    {
        return this.FirstOrDefault(c => string.Equals(c.PropertyInfo.Name, propertyName, StringComparison.InvariantCultureIgnoreCase));
    }
}