using System;
using System.Collections.Generic;
using System.Data;
using LazySql.Engine.Client;
using LazySql.Engine.Client.Query;
using LazySql.Engine.Enums;
using LazySql.Engine.Exceptions;

#if NETCORE
using Microsoft.Data.SqlClient;
#elif NETSTANDARD
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

namespace LazySql.Engine.Connector
{
    internal sealed class SqlConnector : IDisposable
    {
        private readonly SqlConnection _sqlConnection;
        private readonly SqlCommand _sqlCommand;

        public SqlConnector()
        {
            _sqlConnection = new SqlConnection(SqlClient.ConnectionString);
            _sqlCommand = new SqlCommand();
            _sqlCommand.Connection = _sqlConnection;
        }

        public void Dispose()
        {
            _sqlConnection.Dispose();
            _sqlCommand.Dispose();

        }

        public void Open()
        {
            _sqlConnection.Open();
        }

        private void AddValues(SqlArguments sqlArguments)
        {
            if (sqlArguments == null) return;
            foreach (SqlArgument sqlArgument in sqlArguments)
            {
                var parameter = _sqlCommand.Parameters.AddWithValue(sqlArgument.Name, sqlArgument.Value ?? DBNull.Value);
                if (sqlArgument.Type != SqlType.Default)
                    parameter.DbType = (DbType) sqlArgument.Type;
            }
        }

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

        public void ExecuteNonQuery(string query, SqlArguments arguments = null)
        {
            _sqlCommand.CommandText = query;
            AddValues(arguments);

            try
            {
                _sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw LazySqlExecuteException.Generate(ex, query, _sqlCommand.Parameters);
            }
        }
    }
}