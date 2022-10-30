namespace LazySql.Engine
{
    internal static class Tools
    {
        public static string ToSqlColumn(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;
            str = str.Trim();
            if (str[0] == '[' || str[str.Length - 1] == ']') return str;
            return $"[{str}]";
        }
    }
}