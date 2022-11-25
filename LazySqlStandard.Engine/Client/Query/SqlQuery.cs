namespace LazySql.Engine.Client.Query
{
    public sealed class SqlQuery
    {
        public static SqlQuery Empty => new();
        internal string Query { get; }
        internal SqlArguments SqlArguments { get; } = new();
        public SqlQuery(string query)
        {
            Query = query;
        }

        internal SqlQuery()
        {

        }

        public SqlQuery Add(string name, SqlType sqlType, object value)
        {
            SqlArguments.Add(name, sqlType, value);
            return this;
        }

        public SqlQuery Add(string name, object value)
        {
            SqlArguments.Add(name, value.GetType().ToSqlType(), value);
            return this;
        }

        public static bool IsEmpty(SqlQuery sqlQuery) => sqlQuery == null || string.IsNullOrEmpty(sqlQuery.Query);
    }
}
