using LazySql.Transaction;

namespace LazySql;

/// <summary>
/// LazyClient
/// </summary>
// ReSharper disable once ClassCannotBeInstantiated
public sealed partial class LazyClient
{
    private static string _connectionString;
    private static SqlCredential _sqlCredential;

    /// <summary>
    /// Open connection to database
    /// </summary>
    /// <returns></returns>
    private SqlConnector Open()
    {
        SqlConnector sqlConnector = new(_connectionString, _sqlCredential);
        sqlConnector.Open();
        return sqlConnector;
    }

    /// <summary>
    /// ExecuteReader
    /// </summary>
    /// <param name="queryBuilder">Query Builder</param>
    /// <param name="lazyTransaction"></param>
    /// <returns></returns>
    private static IEnumerable<object> ExecuteReader(QueryBuilder queryBuilder, LazyTransaction lazyTransaction) => Instance.InternalExecuteReader(queryBuilder, lazyTransaction);

    /// <summary>
    /// ExecuteReader
    /// </summary>
    /// <param name="queryBuilder">Query Builder</param>
    /// <param name="lazyTransaction"></param>
    /// <returns></returns>
    private IEnumerable<object> InternalExecuteReader(QueryBuilder queryBuilder, LazyTransaction lazyTransaction)
    {
        using SqlConnector sqlConnector = Open();
        using SqlDataReader sqlDataReader = lazyTransaction == null ? sqlConnector.ExecuteQuery(queryBuilder.GetQuery(), queryBuilder.GetArguments()) : lazyTransaction.SqlConnector.ExecuteQuery(queryBuilder.GetQuery(), queryBuilder.GetArguments()) ;
        ITableDefinition tableDefinition = queryBuilder.GetTableDefinition();

        List<(int Index, ColumnDefinition ColumnDefinition)> columns = new();
        for (int i = 0; i < sqlDataReader.FieldCount; i++)
        {
            ColumnDefinition columnDefinition = tableDefinition.FirstOrDefault(c=> string.Equals(c.Column.ColumnName, sqlDataReader.GetName(i), StringComparison.InvariantCultureIgnoreCase));
            if (columnDefinition != null) columns.Add((i, columnDefinition));
        }

        if (tableDefinition.ObjectType == ObjectType.Dynamic)
        {
            while (sqlDataReader.Read())
            {
                IDictionary<string, object> obj = new ExpandoObject();

                for (int i = 0; i < sqlDataReader.FieldCount; i++)
                    obj.Add(sqlDataReader.GetName(i), sqlDataReader.IsDBNull(i) ? null : sqlDataReader.GetValue(i));

                yield return obj;
            }
            yield break;
        }

        while (sqlDataReader.Read())
        {
            object instance = Activator.CreateInstance(tableDefinition.TableType);
            if (tableDefinition.ObjectType == ObjectType.LazyObject) ((LazyBase)instance).OnBeforeLoad();
            foreach ((int Index, ColumnDefinition ColumnDefinition) column in columns)
            {
                object value = null;
                if (!sqlDataReader.IsDBNull(column.Index))
                    value = sqlDataReader.GetValue(column.Index);
                column.ColumnDefinition.PropertyInfo.SetValue(instance, value);
            }
            if (tableDefinition.ObjectType == ObjectType.LazyObject) ((LazyBase)instance).OnLoaded();

            yield return instance;
        }

        sqlDataReader.Close();
    }

    /// <summary>
    /// ExecuteScalar
    /// </summary>
    /// <param name="queryBuilder">Query Builder</param>
    /// <param name="lazyTransaction"></param>
    /// <returns></returns>
    internal object ExecuteScalar(QueryBuilder queryBuilder, LazyTransaction lazyTransaction = null)
    {
        if (lazyTransaction != null)
            return lazyTransaction.SqlConnector.ExecuteScalar(queryBuilder.GetQuery(), queryBuilder.GetArguments());
        
        using SqlConnector sqlConnector = Open();
        return sqlConnector.ExecuteScalar(queryBuilder.GetQuery(), queryBuilder.GetArguments());
    }

    /// <summary>
    /// ExecuteNonQuery
    /// </summary>
    /// <param name="queryBuilder">Query Builder</param>
    /// <param name="lazyTransaction"></param>
    internal int ExecuteNonQuery(QueryBuilder queryBuilder, LazyTransaction lazyTransaction = null)
    {
        if (lazyTransaction != null)
            return lazyTransaction.SqlConnector.ExecuteNonQuery(queryBuilder.GetQuery(), queryBuilder.GetArguments());
        using SqlConnector sqlConnector = Open();
        return sqlConnector.ExecuteNonQuery(queryBuilder.GetQuery(), queryBuilder.GetArguments());
    }
}