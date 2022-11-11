namespace LazySql.Engine.Client.Query;

/// <summary>
/// Sql Argument
/// </summary>
public sealed class SqlArgument
{
    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Type
    /// </summary>
    public SqlType Type { get; }

    /// <summary>
    /// Value
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// Argument Type
    /// </summary>
    internal SqlArgumentType ArgumentType { get; set; } = SqlArgumentType.In;

    internal SqlArgument(string name, SqlType type, object value)
    {
        Name = name;
        Type = type;
        Value = value;
    }
}