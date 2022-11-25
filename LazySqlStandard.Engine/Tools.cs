namespace LazySql;

/// <summary>
/// Tools
/// </summary>
internal static class Tools
{
    /// <summary>
    /// Convert column name to Sql (with [])
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToSqlColumn(this string str)
    {
        if (string.IsNullOrWhiteSpace(str)) return str;
        str = str.Trim();
        if (str[0] == '[' || str[str.Length - 1] == ']') return str;
        return $"[{str}]";
    }
}