namespace LazySql;

/// <summary>
/// Sql Helper
/// </summary>
internal static class SqlHelper
{
    /// <summary>
    /// Map of types
    /// </summary>
    private static Dictionary<Type, SqlType> _map;

    static SqlHelper()
    {
        _map = new Dictionary<Type, SqlType>
        {
            [typeof(long)] = SqlType.BigInt,
            [typeof(byte[])] = SqlType.Binary,
            [typeof(bool)] = SqlType.Bit,
            [typeof(string)] = SqlType.NVarChar,
            [typeof(char[])] = SqlType.NVarChar,
            [typeof(DateTime)] = SqlType.DateTime,
            [typeof(DateTimeOffset)] = SqlType.DateTimeOffset,
            [typeof(byte)] = SqlType.TinyInt,
            [typeof(short)] = SqlType.SmallInt,
            [typeof(int)] = SqlType.Int,
            [typeof(decimal)] = SqlType.Money,
            [typeof(float)] = SqlType.Real,
            [typeof(double)] = SqlType.Float,
            [typeof(TimeSpan)] = SqlType.Time,
            [typeof(Guid)] = SqlType.UniqueIdentifier
        };

    }

    /// <summary>
    /// Convert Type to SqlType
    /// </summary>
    /// <param name="type">Type of object</param>
    /// <returns>Sql Type</returns>
    public static SqlType ToSqlType(this Type type)
    {
        Type typeToCheck = Nullable.GetUnderlyingType(type) ?? type;
        if (_map.ContainsKey(typeToCheck))
            return _map[typeToCheck];
        return SqlType.Default;
    }

    public static string TableName(string schema, string tableName) => string.IsNullOrWhiteSpace(schema) ? tableName : $"{schema}.{tableName}";
}