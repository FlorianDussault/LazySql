namespace LazySql;

/// <summary>
/// Attribute to declare a class compatible with LazySql
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class LazyTable : Attribute
{
    /// <summary>
    /// Table Name
    /// </summary>
    internal string TableName { get; }

    /// <summary>
    /// Table Name
    /// </summary>
    /// <param name="tableName"></param>
    public LazyTable(string tableName)
    {
        TableName = tableName;
    }
}