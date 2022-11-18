namespace LazySql.Engine.Connector;

/// <summary>
/// Sql Connector
/// </summary>
internal sealed class SqlConnector : IDisposable
{
    private readonly SqlConnection _sqlConnection;
    private readonly SqlCommand _sqlCommand;

    public SqlConnector()
    {
        _sqlConnection = new SqlConnection(LazyClient.ConnectionString);
        _sqlCommand = new SqlCommand();
        _sqlCommand.Connection = _sqlConnection;
    }

    public void Dispose()
    {
        _sqlConnection.Dispose();
        _sqlCommand.Dispose();

    }

    /// <summary>
    /// Open connection
    /// </summary>
    public void Open()
    {
        _sqlConnection.Open();
    }

    /// <summary>
    /// Add values to SqlCommand
    /// </summary>
    /// <param name="sqlArguments">Arguments</param>
    private void AddValues(SqlArguments sqlArguments)
    {
        if (sqlArguments == null) return;
        foreach (SqlArgument sqlArgument in sqlArguments) AddValue(sqlArgument);
    }

    /// <summary>
    /// Add value to SqlCommand
    /// </summary>
    /// <param name="sqlArgument">Argument</param>
    private void AddValue(SqlArgument sqlArgument)
    {
        SqlParameter parameter = _sqlCommand.Parameters.AddWithValue(sqlArgument.Name, sqlArgument.Value ?? DBNull.Value);
        if (sqlArgument.Type != SqlType.Default)
            parameter.SqlDbType = (SqlDbType) sqlArgument.Type;
        if (sqlArgument.ArgumentType == SqlArgumentType.Out)
            parameter.Direction = ParameterDirection.Output;
        if (sqlArgument.ArgumentType == SqlArgumentType.ReturnValue)
            parameter.Direction = ParameterDirection.ReturnValue;
    }

    /// <summary>
    /// ExecuteQuery
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="arguments">Arguments</param>
    /// <returns>SqlDataReader</returns>
    public SqlDataReader ExecuteQuery(string query, SqlArguments arguments = null)
    {
        _sqlCommand.CommandText = query;

        AddValues(arguments);

        try
        {
            return _sqlCommand.ExecuteReader();
        }
        catch (Exception ex)
        {
            throw LazySqlExecuteException.Generate(ex, query, _sqlCommand.Parameters);
        }
    }

    /// <summary>
    /// ExecuteScalar
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="arguments">Arguments</param>
    /// <returns>Object</returns>
    public object ExecuteScalar(string query, SqlArguments arguments = null)
    {
        _sqlCommand.CommandText = query;
        AddValues(arguments);

        try
        {
            return _sqlCommand.ExecuteScalar();
        }
        catch (Exception ex)
        {
            throw LazySqlExecuteException.Generate(ex, query, _sqlCommand.Parameters);
        }
    }

    /// <summary>
    /// ExecuteNonQuery
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="arguments">Arguments</param>
    public int ExecuteNonQuery(string query, SqlArguments arguments = null)
    {
        _sqlCommand.CommandText = query;
        AddValues(arguments);

        try
        {
            return _sqlCommand.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw LazySqlExecuteException.Generate(ex, query, _sqlCommand.Parameters);
        }
    }

    /// <summary>
    /// ExecuteStoredProcedure
    /// </summary>
    /// <param name="procedureName">Stored Procedure Name</param>
    /// <param name="arguments">Arguments</param>
    /// <returns>Values</returns>
    public (DataSet dataset, SqlArguments arguments, int? returnValue) ExecuteStoredProcedure(string procedureName, SqlArguments arguments)
    {
        _sqlCommand.CommandText = procedureName;
        _sqlCommand.CommandType = CommandType.StoredProcedure;
        AddValues(arguments);
        AddValue(new SqlArgument("@LAZY_RETURN_VALUE",SqlType.Int,null) {ArgumentType = SqlArgumentType.ReturnValue});

        try
        {
            int? returnValue = null;
            using SqlDataAdapter sqlDataAdapter = new();
            sqlDataAdapter.SelectCommand = _sqlCommand;
            sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            DataSet dataSet = new();
            sqlDataAdapter.Fill(dataSet);

            SqlArguments outputArguments = new();
            foreach (SqlParameter sqlParameter in sqlDataAdapter.SelectCommand.Parameters)
            {
                switch (sqlParameter.Direction)
                {
                    case ParameterDirection.Output:
                        outputArguments.Add(new SqlArgument(sqlParameter.ParameterName, (SqlType) sqlParameter.DbType, sqlParameter.Value == DBNull.Value ? null : sqlParameter.Value){ArgumentType = SqlArgumentType.Out});
                        break;
                    case ParameterDirection.ReturnValue:
                        returnValue = (int)sqlParameter.Value;
                        break;
                }
            }

            return (dataSet, outputArguments, returnValue);
        }
        catch (Exception ex)
        {
            throw LazySqlExecuteException.Generate(ex, procedureName, _sqlCommand.Parameters);
        }
    }

    public void BulkInsert(string tableName, DataTable dataTable)
    {
        try
        {
            SqlBulkCopy bulkCopy = new(
                _sqlConnection,
                SqlBulkCopyOptions.TableLock |
                SqlBulkCopyOptions.FireTriggers |
                SqlBulkCopyOptions.UseInternalTransaction,
                null
            );

            List<string> sqlColumns = new();
            using (SqlDataAdapter sqlDataAdapter = new())
            {
                _sqlCommand.CommandText = $"SELECT * FROM {tableName} WHERE 1=0";
                sqlDataAdapter.SelectCommand = _sqlCommand;
                DataSet dataSet = new();
                sqlDataAdapter.Fill(dataSet);
                for (int i = 0; i < dataSet.Tables[0].Columns.Count; i++)
                    sqlColumns.Add(dataSet.Tables[0].Columns[i].ColumnName);
            }


            for (int index = 0; index < dataTable.Columns.Count; index++)
            {
                DataColumn dataColumn = dataTable.Columns[index];
                string columnName = sqlColumns.FirstOrDefault(column =>
                    string.Equals(column, dataColumn.ColumnName, StringComparison.InvariantCultureIgnoreCase));
                if (string.IsNullOrEmpty(columnName))
                {
                    dataTable.Columns.RemoveAt(index--);
                    continue;
                }
                bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(dataColumn.ColumnName, columnName));
            }

            bulkCopy.DestinationTableName = tableName;
            bulkCopy.WriteToServer(dataTable);
        }
        catch (Exception ex)
        {
            throw LazySqlExecuteException.Generate(ex, $"BulkInsert in {tableName}");
        }
    }
}