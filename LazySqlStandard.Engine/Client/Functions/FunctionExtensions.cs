namespace LazySql;

internal static class FunctionExtensions
{
    [Obsolete]
    public static int Count(this int value)
    {
        return 0;
    }

    public static int Max(this int value)
    {
        return 0;
    }

    public static T As<T>(this T obj, string columnName)
    {
        return obj;
    }
}