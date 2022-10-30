using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using LazySql.Engine.Client.Query;
using LazySql.Engine.Connector;
using LazySql.Engine.Definitions;

namespace LazySql.Engine.Client
{
    public sealed partial class SqlClient
    {
        #region Truncate

        public static void Truncate<T>(bool reseed = false) where T : LazyBase => Instance.InternalTruncate<T>(reseed);

        private void InternalTruncate<T>(bool reseed = false) where T : LazyBase
        {
            CheckInitialization<T>(out TableDefinition tableDefinition);

            using (SqlConnector sqlConnector = Open())
            {
                if (reseed)
                {
                    InternalDelete<T>();
                    sqlConnector.ExecuteQuery($"DBCC CHECKIDENT ('{tableDefinition.Table.TableName}', RESEED, 0)");
                }
                else
                {
                    sqlConnector.ExecuteQuery($"TRUNCATE TABLE {tableDefinition.Table.TableName}");
                }
            }
        }

        #endregion

        #region Delete

        public static void Delete<T>(Expression<Func<T, bool>> expression = null) where T : LazyBase => Instance.InternalDelete<T>(expression);

        private void InternalDelete<T>(Expression<Func<T,bool>> expression = null) where T : LazyBase
        {
            CheckInitialization<T>(out TableDefinition tableDefinition);

            QueryBuilder queryBuilder = new QueryBuilder(tableDefinition);
            queryBuilder.Append($"DELETE FROM {tableDefinition.Table.TableName}");

            if (expression != null)
            {
                queryBuilder.Append(" WHERE ", expression);
            }

            ExecuteNonQuery(queryBuilder);
        }


        public static void Delete<T>(T obj) where T : LazyBase => Instance.InternalDelete(obj);


        public void InternalDelete<T>(T obj) where T : LazyBase
        {
            CheckInitialization<T>(out TableDefinition tableDefinition);

            tableDefinition.GetColumns(out _, out _, out _, out IReadOnlyList<ColumnDefinition> primaryKeys);

            BinaryExpression binaryExpression = null;
            foreach (ColumnDefinition primaryKey in primaryKeys)
            {
                var objExpression = Expression.Parameter(typeof(T), "source");
                var member = Expression.Property(objExpression, primaryKey.PropertyInfo);
                var value = Expression.Constant(primaryKey.PropertyInfo.GetValue(obj));
                BinaryExpression childExpression = Expression.Equal(member, value);
                if (binaryExpression == null)
                {
                    binaryExpression = childExpression;
                    continue;
                }
                binaryExpression = Expression.AndAlso(binaryExpression, childExpression);
            }

            QueryBuilder queryBuilder = new QueryBuilder(tableDefinition);
            queryBuilder.Append($"DELETE FROM {tableDefinition.Table.TableName} WHERE ");
            queryBuilder.Append(binaryExpression);

            using (SqlConnector sqlConnector = Open())
                sqlConnector.ExecuteQuery(queryBuilder.GetQuery(), queryBuilder.GetArguments());
        }


        #endregion
    }
}