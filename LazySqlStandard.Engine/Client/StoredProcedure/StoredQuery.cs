namespace LazySql
{
    public sealed class StoredQuery
    {
        internal string ProcedureName { get; }

        internal SqlArguments SqlArguments { get; } = new();

        public StoredQuery(string procedureName)
        {
            ProcedureName = procedureName;
        }

        public StoredQuery BindIn(string name, SqlType sqlType, object value)
        {
            SqlArguments.Add(new SqlArgument(name, sqlType, value) { ArgumentType = SqlArgumentType.In });
            return this;
        }

        public StoredQuery BindIn(string name, object value)
        {
            SqlArguments.Add(new SqlArgument(name, value.GetType().ToSqlType(), value) { ArgumentType = SqlArgumentType.In });
            return this;
        }

        public StoredQuery BindOut(string name, SqlType sqlType)
        {
            SqlArguments.Add(new SqlArgument(name, sqlType, null) { ArgumentType = SqlArgumentType.Out });
            return this;
        }
    }
}
