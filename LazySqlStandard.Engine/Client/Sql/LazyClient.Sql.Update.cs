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
        #region Update

        /// <summary>
        /// Update an item
        /// </summary>
        /// <typeparam name="T">Type of Item</typeparam>
        /// <param name="obj">Item</param>
        public static void Update<T>(T obj) where T : LazyBase => Instance.InternalUpdate(typeof(T), obj);

        private void InternalUpdate(Type type, object obj)
        {
            CheckInitialization(type, out TableDefinition tableDefinition);

            tableDefinition.GetColumns(out _, out IReadOnlyList<ColumnDefinition> columns, out _, out IReadOnlyList<ColumnDefinition> primaryKeys);

            QueryBuilder queryBuilder = new(tableDefinition);
            queryBuilder.Append($"UPDATE {tableDefinition.Table.TableName} SET ");
            List<string> values = new List<string>();
            foreach (ColumnDefinition column in columns)
            {
                string argumentName = queryBuilder.RegisterArgument(column.Column.SqlType, column.PropertyInfo.GetValue(obj));
                values.Add($"{column.Column.SqlColumnName} = {argumentName}");
            }
            queryBuilder.Append(string.Join(", ", values));
       

            BinaryExpression binaryExpression = null;
            foreach (ColumnDefinition primaryKey in primaryKeys)
            {
                ParameterExpression objExpression = Expression.Parameter(type, "source");
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

            if (binaryExpression != null)
            {
                queryBuilder.Append(" WHERE ");
                queryBuilder.Append(binaryExpression);
            }

            using SqlConnector sqlConnector = Open();
            sqlConnector.ExecuteNonQuery(queryBuilder.GetQuery(), queryBuilder.GetArguments());
        }

        #endregion

    }
}