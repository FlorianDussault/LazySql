namespace LazySql.Engine.Helpers;

/// <summary>
/// List Helper
/// </summary>
internal static class ListHelper
{
    /// <summary>
    /// For foreach, to get flag information of the last item
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="enumerator">Enumerator</param>
    /// <returns>IEnumerable</returns>
    public static IEnumerable<(bool isLast, T value)> ForeachWithLast<T>(this IEnumerator<T> enumerator)
    {
        bool isLast = !enumerator.MoveNext();
        if (isLast) yield break;

        do
        {
            T current = enumerator.Current;
            isLast = !enumerator.MoveNext();
            yield return (isLast, current);
        } while (!isLast);
    }
    /// <summary>
    /// For foreach, to get flag information of the last item
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="enumerable">Enumerable</param>
    /// <returns>IEnumerable</returns>
    public static IEnumerable<(bool isLast, T value)> ForeachWithLast<T>(this IEnumerable<T> enumerable) => enumerable.GetEnumerator().ForeachWithLast();
}