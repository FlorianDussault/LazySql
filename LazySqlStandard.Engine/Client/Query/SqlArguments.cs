namespace LazySql;

internal sealed class SqlArguments : List<SqlArgument>
{
    internal string Register(SqlType type,  object obj)
    {
        string argumentName = $"@_LZ_{Count}_LZ_@";
        Add(new SqlArgument(argumentName, type, obj));
        return argumentName;
    }

    public SqlArguments Add(string name, SqlType sqlType, object obj)
    {
        Add(new SqlArgument(name, sqlType, obj));
        return this;
    }

    public SqlArguments AddOut(string name, SqlType sqlType)
    {
        Add(new SqlArgument(name, sqlType, null) {ArgumentType = SqlArgumentType.Out});
        return this;
    }

}