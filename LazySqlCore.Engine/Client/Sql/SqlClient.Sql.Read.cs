using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LazySql.Engine.Client.Query;
using LazySql.Engine.Definitions;
using LazySql.Engine.Enums;

namespace LazySql.Engine.Client
{
    // ReSharper disable once ClassCannotBeInstantiated
    public sealed partial class SqlClient
    {
        public static IEnumerable<T> Get<T>(Expression<Func<T, object>> expression = null) where T : LazyBase => Instance.InternalGet(expression);

        private IEnumerable<T> InternalGet<T>(Expression<Func<T, object>> expression = null) where T : LazyBase
        {
            CheckInitialization<T>(out TableDefinition tableDefinition);
            QueryBuilder queryBuilder = new QueryBuilder(tableDefinition);
            BuildSelect(tableDefinition, queryBuilder);

            if (expression != null)
            {
                queryBuilder.Append(" WHERE ", expression);
            }

            bool haRelations = tableDefinition.OneToManyExpressions != null && tableDefinition.OneToManyExpressions.Count > 0;

            foreach (T value in ExecuteReader<T>(queryBuilder))
            {
                if (haRelations)
                {

                    foreach (RelationInformation relation in tableDefinition.OneToManyExpressions)
                        LoadChildren<T>(relation, value);
                }
                yield return value;
            }
        }
    
        private void BuildSelect(TableDefinition tableDefinition, QueryBuilder queryBuilder)
        {
            tableDefinition.GetColumns(out IReadOnlyList<ColumnDefinition> allColumns, out _, out _, out _);
            queryBuilder.Append($"SELECT {string.Join(", ", allColumns.Where(c => c.Column.SqlType != SqlType.Children).Select(c => c.Column.SqlColumnName))} FROM {tableDefinition.Table.TableName}");

        }

        private void LoadChildren<T>(RelationInformation relationInformation, object obj)
        {
            SqlClient.CheckInitialization(relationInformation.ChildType, out TableDefinition childTableDefinition);
            QueryBuilder queryBuilder = new QueryBuilder(childTableDefinition);
            
            BuildSelect(childTableDefinition, queryBuilder);
            queryBuilder.Append(" WHERE ");
            queryBuilder.Append(relationInformation.Expression, typeof(T), obj);

            IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(relationInformation.ChildType));

            MethodInfo method = typeof(SqlClient).GetMethod(nameof(ExecuteReader), BindingFlags.NonPublic | BindingFlags.Static);
            IEnumerable values = (IEnumerable)method.MakeGenericMethod(relationInformation.ChildType).Invoke(null, new object[] { queryBuilder });
            foreach (object value in values)
                list.Add(value);
        
            typeof(T).GetProperty(relationInformation.Column)?.SetValue(obj, list);
        }
    }
}