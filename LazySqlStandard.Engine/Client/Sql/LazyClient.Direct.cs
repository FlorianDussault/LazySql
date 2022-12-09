namespace LazySql;

// ReSharper disable once ClassCannotBeInstantiated
public sealed partial class LazyClient
{
    /// <summary>
    /// Executes a Transact-SQL statement against the connection and returns the number of rows affected. 
    /// </summary>
    /// <param name="query">Query</param>
    public static int ExecuteNonQuery(string query) => Instance.InternalExecuteNonQuery(query, null);

    /// <summary>
    /// Executes a Transact-SQL statement against the connection and returns the number of rows affected. 
    /// </summary>
    /// <param name="sqlQuery">Query</param>
    public static int ExecuteNonQuery(SqlQuery sqlQuery) => Instance.InternalExecuteNonQuery(sqlQuery.Query, sqlQuery.SqlArguments);

    /// <summary>
    /// Executes a Transact-SQL statement against the connection and returns the number of rows affected. 
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="sqlArguments"></param>
    private int InternalExecuteNonQuery(string query, SqlArguments sqlArguments)
    {
        using SqlConnector sqlConnector = Open();
        return sqlConnector.ExecuteNonQuery(query, sqlArguments);
    }

    /// <summary>
    /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored
    /// </summary>
    /// <typeparam name="T">Type of return object</typeparam>
    /// <param name="query">Query</param>
    /// <param name="arguments">Arguments</param>
    /// <returns></returns>
    public static T ExecuteScalar<T>(string query) => Instance.InternalExecuteScalar<T>(query, null);

    /// <summary>
    /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored
    /// </summary>
    /// <typeparam name="T">Type of return object</typeparam>
    /// <param name="query">Query</param>
    /// <param name="arguments">Arguments</param>
    /// <returns></returns>
    public static T ExecuteScalar<T>(SqlQuery query) => Instance.InternalExecuteScalar<T>(query.Query, query.SqlArguments);


    /// <summary>
    /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored
    /// </summary>
    /// <typeparam name="T">Type of return object</typeparam>
    /// <param name="query">Query</param>
    /// <param name="arguments">Arguments</param>
    /// <returns></returns>
    private T InternalExecuteScalar<T>(string query, SqlArguments arguments = null)
    {
        using SqlConnector sqlConnector = Open();
        return (T) sqlConnector.ExecuteScalar(query, arguments);
    }
}