using System.Collections.Generic;

namespace LazySql.Engine.Helpers
{
    internal static class ListHelper
    {
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

            //while (!isLast)
            //{
            //    yield return (false, enumerator.Current);
            //    isLast = !enumerator.MoveNext();
            //    //if (isLast)
            //    //    yield return (true, enumerator.Current);
            //}
        }
        public static IEnumerable<(bool isLast, T value)> ForeachWithLast<T>(this IEnumerable<T> enumerable) => enumerable.GetEnumerator().ForeachWithLast();
    }
}
