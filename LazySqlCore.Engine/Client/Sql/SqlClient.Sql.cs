using System;
using System.Collections.Generic;
using System.Linq;
using LazySql.Engine.Client.Query;
using LazySql.Engine.Connector;
using LazySql.Engine.Definitions;
using LazySql.Engine.Enums;


#if NETCORE
using Microsoft.Data.SqlClient;
#elif NETSTANDARD
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

namespace LazySql.Engine.Client 
{
    public sealed partial class SqlClient
    {
        internal static string ConnectionString { get; private set; }

        private SqlConnector Open()
        {
            SqlConnector sqlConnector = new SqlConnector();
            sqlConnector.Open();
            return sqlConnector;
        }

        internal static IEnumerable<T> ExecuteReader<T>(QueryBuilder queryBuilder) where T : LazyBase => Instance.InternalExecuteReader<T>(queryBuilder);
        private IEnumerable<T> InternalExecuteReader<T>(QueryBuilder queryBuilder) where T : LazyBase
        {
            using (SqlConnector sqlConnector = Open())
            {
                SqlDataReader sqlDataReader =
                    sqlConnector.ExecuteQuery(queryBuilder.GetQuery(), queryBuilder.GetArguments());
                TableDefinition tableDefinition = queryBuilder.GetTableDefinition();

                while (sqlDataReader.Read())
                {
                    T instance = (T) Activator.CreateInstance(tableDefinition.TableType);

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
        }

        internal object ExecuteScalar<T>(QueryBuilder queryBuilder) where T : LazyBase
        {
            using (SqlConnector sqlConnector = Open())
                return sqlConnector.ExecuteScalar(queryBuilder.GetQuery(), queryBuilder.GetArguments());
        }

        internal void ExecuteNonQuery(QueryBuilder queryBuilder)
        {
            using (SqlConnector sqlConnector = Open())
                sqlConnector.ExecuteNonQuery(queryBuilder.GetQuery(), queryBuilder.GetArguments());
        }
    }
}