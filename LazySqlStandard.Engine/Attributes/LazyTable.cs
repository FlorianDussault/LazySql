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
    internal string Schema { get; }

    /// <summary>
    /// Table Name
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="tableName"></param>
    public LazyTable(string schema, string tableName)
    {
        Schema = schema;
        TableName = tableName;
    }

    public LazyTable(string tableName)
    {
        Schema = null;
        TableName = tableName;
    }
}