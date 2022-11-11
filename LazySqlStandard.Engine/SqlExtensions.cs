namespace LazySql.Engine;

/// <summary>
/// Sql Extensions
/// </summary>
public static class SqlExtensions
{
    /// <summary>
    /// Insert an object
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="obj">Object</param>
    public static void Insert<T>(this T obj) where T : LazyBase => LazyClient.Insert(obj);

    /// <summary>
    /// Insert list of object
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="list"></param>
    public static void Insert<T>(this IEnumerable<T> list) where T : LazyBase
    {
        foreach (T obj in list) obj.Insert();
    }

    /// <summary>
    /// Update an object
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="obj">Object</param>
    public static void Update<T>(this T obj) where T : LazyBase => LazyClient.Update(obj);

    /// <summary>
    /// Update list of object
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="list"></param>
    public static void Update<T>(this IEnumerable<T> list) where T : LazyBase
    {
        foreach (T obj in list) obj.Update();
    }

    /// <summary>
    /// Delete an object
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="obj">Object</param>
    public static void Delete<T>(this T obj) where T : LazyBase => LazyClient.Delete(obj);

    /// <summary>
    /// Delete list of object
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="list"></param>
    public static void Delete<T>(this IList<T> list) where T : LazyBase
    {
        while (list.Count > 0)
        {
            list[0].Delete();
            list.RemoveAt(0);
        }
    }
}