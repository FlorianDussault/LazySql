namespace LazySql;

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

            foreach ((int Index, ColumnDefinition ColumnDefinition) column in columns)
            {
                object value = null;
                if (!sqlDataReader.IsDBNull(column.Index))
                    value = sqlDataReader.GetValue(column.Index);
                column.ColumnDefinition.PropertyInfo.SetValue(instance, value);
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
    internal int ExecuteNonQuery(QueryBuilder queryBuilder)
    {
        using SqlConnector sqlConnector = Open();
        return sqlConnector.ExecuteNonQuery(queryBuilder.GetQuery(), queryBuilder.GetArguments());
    }
}