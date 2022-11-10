using LazySql.Engine.Enums;

namespace LazySql.Engine.Client.Query
{
    public sealed class SqlArgument
    {
        public string Name { get; }
        public SqlType Type { get; }
        public object Value { get; }

        internal SqlArgumentType ArgumentType { get; set; } = SqlArgumentType.In;

        internal SqlArgument(string name, SqlType type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }

    internal enum SqlArgumentType
    {
        In = 0,
        Out = 1,
        ReturnValue = 2
    }
}