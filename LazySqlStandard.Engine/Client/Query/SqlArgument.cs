using LazySql.Engine.Enums;

namespace LazySql.Engine.Client.Query
{

internal sealed class SqlArgument
{
    public string Name { get; }
    public SqlType Type { get; }
    public object Value { get; }

    internal SqlArgument(string name, SqlType type, object value)
    {
        Name = name;
        Type = type;
        Value = value;
    }
}
}