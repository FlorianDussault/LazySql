using System.Collections.Generic;
using System.Linq.Expressions;
using LazySql.Engine.Client.Query;
using LazySql.Engine.Connector;
using LazySql.Engine.Definitions;

namespace LazySql.Engine.Client
{
    // ReSharper disable once ClassCannotBeInstantiated
    public sealed partial class LazyClient
    {
        #region Update

        public static void Update<T>(T obj) where T : LazyBase => Instance.InternalUpdate(obj);

        private void InternalUpdate<T>(T obj) where T : LazyBase
        {
            CheckInitialization<T>(out TableDefinition tableDefinition);

            tableDefinition.GetColumns(out _, out IReadOnlyList<ColumnDefinition> columns, out _, out IReadOnlyList<ColumnDefinition> primaryKeys);

        
            QueryBuilder queryBuilder = new QueryBuilder(tableDefinition);
            queryBuilder.Append($"UPDATE {tableDefinition.Table.TableName} SET ");
            List<string> values = new List<string>();
            for (int i = 0; i < columns.Count;i++)
            {
                string argumentName = queryBuilder.RegisterArgument(columns[i].Column.SqlType, columns[i].PropertyInfo.GetValue(obj));
                values.Add($"{columns[i].Column.SqlColumnName} = {argumentName}");
            }
            queryBuilder.Append(string.Join(", ", values));
       

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

            if (binaryExpression != null)
            {
                queryBuilder.Append(" WHERE ");
                queryBuilder.Append(binaryExpression);
            }

            using (SqlConnector sqlConnector = Open())
                sqlConnector.ExecuteQuery(queryBuilder.GetQuery(), queryBuilder.GetArguments());
        }

        #endregion

    }
}