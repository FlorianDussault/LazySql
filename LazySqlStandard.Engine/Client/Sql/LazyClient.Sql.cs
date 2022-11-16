// ReSharper disable once CheckNamespace
namespace LazySql.Engine.Client;

/// <summary>
/// LazyClient
/// </summary>
// ReSharper disable once ClassCannotBeInstantiated
public sealed partial class LazyClient
{
    internal static string ConnectionString { get; private set; }

    /// <summary>
    /// Open connection to database
    /// </summary>
    /// <returns></returns>
    private SqlConnector Open()
    {
        SqlConnector sqlConnector = new();
        sqlConnector.Open();
        return sqlConnector;
    }

    /// <summary>
    /// ExecuteReader
    /// </summary>
    /// <param name="queryBuilder">Query Builder</param>
    /// <returns></returns>
    internal static IEnumerable<object> ExecuteReader(QueryBuilder queryBuilder) => Instance.InternalExecuteReader(queryBuilder);

    /// <summary>
    /// ExecuteReader
    /// </summary>
    /// <param name="queryBuilder">Query Builder</param>
    /// <returns></returns>
    private IEnumerable<object> InternalExecuteReader(QueryBuilder queryBuilder)
    {
        using SqlConnector sqlConnector = Open();
        using SqlDataReader sqlDataReader =
            sqlConnector.ExecuteQuery(queryBuilder.GetQuery(), queryBuilder.GetArguments());
        TableDefinition tableDefinition = queryBuilder.GetTableDefinition();

        while (sqlDataReader.Read())
        {
            object instance = Activator.CreateInstance(tableDefinition.TableType);

            // TODO: code cleaning
            foreach (ColumnDefinition columnDefinition in tableDefinition.Where(c =>
                         c.Column.SqlType != SqlType.Children))
            {
                object value = null;
                if (!sqlDataReader.IsDBNull(columnDefinition.Index))
                    switch (columnDefinition.Column.SqlType)
                    {
                        //case SqlType.Varchar:
                        //    value = sqlDataReader.GetString(columnDefinition.Index);
                        //    break;
                        //case SqlType.Int:
                        //    value = sqlDataReader.GetInt32(columnDefinition.Index);
                        //    break;
                        //case SqlType.Bit:
                        //    value = sqlDataReader.GetBoolean(columnDefinition.Index);
                        //    break;
                        //case SqlType.BigInt:
                        //    value = sqlDataReader.GetValue(columnDefinition.Index);
                        //    break;
                        default:
                            value = sqlDataReader.GetValue(columnDefinition.Index);
                            break;
                        //throw new ArgumentOutOfRangeException();
                    }

                columnDefinition.PropertyInfo.SetValue(instance, value);
            }

            yield return instance;
        }

        sqlDataReader.Close();
    }

    /// <summary>
    /// ExecuteScalar
    /// </summary>
    /// <param name="queryBuilder">Query Builder</param>
    /// <returns></returns>
    internal object ExecuteScalar(QueryBuilder queryBuilder)
    {
        using SqlConnector sqlConnector = Open();
        return sqlConnector.ExecuteScalar(queryBuilder.GetQuery(), queryBuilder.GetArguments());
    }

    /// <summary>
    /// ExecuteNonQuery
    /// </summary>
    /// <param name="queryBuilder">Query Builder</param>
    internal void ExecuteNonQuery(QueryBuilder queryBuilder)
    {
        using SqlConnector sqlConnector = Open();
        sqlConnector.ExecuteNonQuery(queryBuilder.GetQuery(), queryBuilder.GetArguments());
    }
}