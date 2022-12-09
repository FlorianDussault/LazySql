﻿namespace LazySql;

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
    public static int Insert<T>(this T obj) where T : LazyBase => LazyClient.Insert(obj);

    /// <summary>
    /// Insert list of object
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="list"></param>
    public static int Insert<T>(this IEnumerable<T> list) where T : LazyBase => list.Sum(obj => obj.Insert());

    /// <summary>
    /// Update an object
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="obj">Object</param>
    /// <param name="where"></param>
    /// <param name="excludedColumns"></param>
    public static int Update<T>(this T obj, Expression<Func<T, bool>> where, params string[] excludedColumns) => LazyClient.Update(obj, where,excludedColumns);

    public static int Update<T>(this T obj, SqlQuery where, params string[] excludedColumns) => LazyClient.Update(obj, null, where, excludedColumns);

    public static int Update<T>(this T obj) where T : LazyBase => LazyClient.Update(obj);

    public static int Update<T>(this IEnumerable<T> list) where T : LazyBase => list.Sum(obj => obj.Update());

    /// <summary>
    /// Delete an object
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="obj">Object</param>
    public static int Delete<T>(this T obj) where T : LazyBase => LazyClient.Delete(obj);

    public static int Delete<T>(this IEnumerable<T> list) where T : LazyBase => list.Sum(obj => obj.Delete());

    public static SqlQuery Bind(this string query, string paramName, SqlType sqlType, object value) => new SqlQuery(query).Bind(paramName, sqlType, value);

    public static SqlQuery Bind(this string query, string paramName, object value) => new SqlQuery(query).Bind(paramName, value);

    public static StoredQuery BindIn(this string procedureName, string paramName, SqlType  sqlType, object value) => new StoredQuery(procedureName).BindIn(paramName, sqlType, value);
    public static StoredQuery BindIn(this string procedureName, string paramName, object value) => new StoredQuery(procedureName).BindIn(paramName, value);

    public static StoredQuery BindOut(this string procedureName, string paramName, SqlType sqlType) => new StoredQuery(procedureName).BindOut(paramName, sqlType);
}