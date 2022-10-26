using System.Collections.Generic;
using LazySql.Engine.Client;

namespace LazySql.Engine
{
    public static class SqlExtensions
    {
        public static void Insert<T>(this T obj) where T : LazyBase => SqlClient.Insert(obj);

        public static void Insert<T>(this IEnumerable<T> list) where T : LazyBase
        {
            foreach (T obj in list) obj.Insert();
        }

        public static void Update<T>(this T obj) where T : LazyBase => SqlClient.Update(obj);

        public static void Update<T>(this IEnumerable<T> list) where T : LazyBase
        {
            foreach (T obj in list) obj.Update();
        }

        public static void Delete<T>(this T obj) where T : LazyBase => SqlClient.Delete(obj);

        public static void Delete<T>(this IList<T> list) where T : LazyBase
        {
            while (list.Count > 0)
            {
                list[0].Delete();
                list.RemoveAt(0);
            }
        }
    }
}