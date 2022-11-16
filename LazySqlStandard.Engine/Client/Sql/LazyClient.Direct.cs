// ReSharper disable once CheckNamespace
using LazySql.Engine.Client.Query;

namespace LazySql.Engine.Client;

// ReSharper disable once ClassCannotBeInstantiated
public sealed partial class LazyClient
{
    /// <summary>
    /// Executes a Transact-SQL statement against the connection and returns the number of rows affected. 
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="arguments">Arguments</param>
    public static int ExecuteNonQuery(string query, SqlArguments arguments = null) => Instance.InternalExecuteNonQuery(query, arguments);

    /// <summary>
    /// Executes a Transact-SQL statement against the connection and returns the number of rows affected. 
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="arguments">Arguments</param>
    private int InternalExecuteNonQuery(string query, SqlArguments arguments) => Open().ExecuteNonQuery(query, arguments ?? new SqlArguments());

    /// <summary>
    /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored
    /// </summary>
    /// <typeparam name="T">Type of return object</typeparam>
    /// <param name="query">Query</param>
    /// <param name="arguments">Arguments</param>
    /// <returns></returns>
    public static T ExecuteScalar<T>(string query, SqlArguments arguments = null) => Instance.InternalExecuteScalar<T>(query, arguments);

    /// <summary>
    /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored
    /// </summary>
    /// <typeparam name="T">Type of return object</typeparam>
    /// <param name="query">Query</param>
    /// <param name="arguments">Arguments</param>
    /// <returns></returns>
    private T InternalExecuteScalar<T>(string query, SqlArguments arguments = null) => (T)Open().ExecuteScalar(query, arguments);
}