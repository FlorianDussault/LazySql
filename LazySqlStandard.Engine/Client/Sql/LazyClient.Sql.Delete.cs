using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LazySql.Engine.Client.Query;
using LazySql.Engine.Connector;
using LazySql.Engine.Definitions;

// ReSharper disable once CheckNamespace
namespace LazySql.Engine.Client
{
    // ReSharper disable once ClassCannotBeInstantiated
    public sealed partial class LazyClient
    {
        #region Truncate

        /// <summary>
        /// Truncate table
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="reseed">Execute RESEED (DBCC CHECKIDENT) for a table with relations</param>
        public static void Truncate<T>(bool reseed = false) where T : LazyBase => Instance.InternalTruncate(typeof(T), reseed);

        /// <summary>
        /// Truncate table
        /// </summary>
        /// <param name="type">Type of object</param>
        /// <param name="reseed">Execute RESEED (DBCC CHECKIDENT) for a table with relations</param>
        private void InternalTruncate(Type type, bool reseed = false)
        {
            CheckInitialization(type, out TableDefinition tableDefinition);

            using SqlConnector sqlConnector = Open();
            if (reseed)
            {
                InternalDelete(type);
                sqlConnector.ExecuteQuery($"DBCC CHECKIDENT ('{tableDefinition.Table.TableName}', RESEED, 0)");
            }
            else
            {
                sqlConnector.ExecuteQuery($"TRUNCATE TABLE {tableDefinition.Table.TableName}");
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete one or more items from the database
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="expression">Filter expression</param>
        public static void Delete<T>(Expression<Func<T, bool>> expression = null) where T : LazyBase => Instance.InternalDelete(typeof(T), expression);

        /// <summary>
        /// Delete one or more items from the database
        /// </summary>
        /// <param name="type">Type of item</param>
        /// <param name="expression">Filter Expression</param>
        private void InternalDelete(Type type, LambdaExpression expression = null)
        {
            CheckInitialization(type, out TableDefinition tableDefinition);

            QueryBuilder queryBuilder = new(tableDefinition);
            queryBuilder.Append($"DELETE FROM {tableDefinition.Table.TableName}");

            if (expression != null)
                queryBuilder.Append(" WHERE ", expression);

            ExecuteNonQuery(queryBuilder);
        }

        /// <summary>
        /// Delete an item from the database
        /// </summary>
        /// <typeparam name="T">Type of item</typeparam>
        /// <param name="obj">Item</param>
        public static void Delete<T>(T obj) where T : LazyBase => Instance.InternalDelete(obj);

        /// <summary>
        /// Delete an item from the database
        /// </summary>
        /// <param name="obj">Item</param>
        private void InternalDelete(object obj)
        {
            CheckInitialization(obj.GetType(), out TableDefinition tableDefinition);

            tableDefinition.GetColumns(out _, out _, out _, out IReadOnlyList<ColumnDefinition> primaryKeys);

            BinaryExpression binaryExpression = null;
            foreach (ColumnDefinition primaryKey in primaryKeys)
            {
                ParameterExpression objExpression = Expression.Parameter(obj.GetType(), "source");
                MemberExpression member = Expression.Property(objExpression, primaryKey.PropertyInfo);
                ConstantExpression value = Expression.Constant(primaryKey.PropertyInfo.GetValue(obj));
                BinaryExpression childExpression = Expression.Equal(member, value);
                if (binaryExpression == null)
                {
                    binaryExpression = childExpression;
                    continue;
                }
                binaryExpression = Expression.AndAlso(binaryExpression, childExpression);
            }

            QueryBuilder queryBuilder = new(tableDefinition);
            queryBuilder.Append($"DELETE FROM {tableDefinition.Table.TableName} WHERE ");
            queryBuilder.Append(binaryExpression, obj.GetType(), obj);

            using SqlConnector sqlConnector = Open();
            sqlConnector.ExecuteQuery(queryBuilder.GetQuery(), queryBuilder.GetArguments());
        }

        #endregion
    }
}