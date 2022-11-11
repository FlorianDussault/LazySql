namespace LazySql.Engine.Definitions;

/// <summary>
/// Column Definition
/// </summary>
internal sealed class ColumnDefinition
{
    /// <summary>
    /// Object Property
    /// </summary>
    public PropertyInfo PropertyInfo { get; }

    /// <summary>
    /// Lazy Column
    /// </summary>
    public LazyColumn Column { get; }

    /// <summary>
    /// Primary Key information
    /// </summary>

    public PrimaryKey PrimaryKey { get; }

    /// <summary>
    /// Index of the column
    /// </summary>
    public int Index { get; }

    public ColumnDefinition(PropertyInfo propertyInfo, LazyColumn column, int index, PrimaryKey primaryKey)
    {
        PropertyInfo = propertyInfo;
        Column = column;
        Index = index;
        PrimaryKey = primaryKey;
    }

    /// <summary>
    /// Get Value of an object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public object GetValue(object obj) => PropertyInfo.GetValue(obj, null );
}