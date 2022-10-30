using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        public static IEnumerable<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : LazyBase => Instance.InternalGet(expression);

        private IEnumerable<T> InternalGet<T>(Expression<Func<T, bool>> expression = null) where T : LazyBase
        {
            CheckInitialization<T>(out TableDefinition tableDefinition);
            QueryBuilder queryBuilder = new QueryBuilder(tableDefinition);
            BuildSelect(tableDefinition, queryBuilder);

            if (expression != null)
            {
                queryBuilder.Append(" WHERE ", expression);
            }

            if (tableDefinition.Relations.Count == 0)
            {
                foreach (T value in ExecuteReader<T>(queryBuilder))
                {
                    yield return value;
                }
                yield break;
            }

            List<T> values = ExecuteReader<T>(queryBuilder).ToList();

            foreach (RelationInformation relation in tableDefinition.Relations)
            {
                LoadChildren<T>(relation, values);
            }

            foreach (T value in values)
                yield return value;
            
            yield break;
             

            bool hasRelations = tableDefinition.Relations != null && tableDefinition.Relations.Count > 0;

            foreach (T value in ExecuteReader<T>(queryBuilder))
            {
                if (hasRelations)
                    foreach (RelationInformation relation in tableDefinition.Relations)
                        LoadChildren<T>(relation, value);

                yield return value;
            }
        }
    
        private void BuildSelect(TableDefinition tableDefinition, QueryBuilder queryBuilder)
        {
            tableDefinition.GetColumns(out IReadOnlyList<ColumnDefinition> allColumns, out _, out _, out _);
            queryBuilder.Append($"SELECT {string.Join(", ", allColumns.Where(c => c.Column.SqlType != SqlType.Children).Select(c => c.Column.SqlColumnName))} FROM {tableDefinition.Table.TableName}");

        }

        private void LoadChildren<T>(RelationInformation relationInformation, List<T> values)
        {
            if (values.Count == 0) return;
            CheckInitialization(relationInformation.ChildType, out TableDefinition childTableDefinition);
            QueryBuilder queryBuilder = new QueryBuilder(childTableDefinition);

            BuildSelect(childTableDefinition, queryBuilder);
            queryBuilder.Append(" WHERE ");

            for (var index = 0; index < values.Count; index++)
            {
                queryBuilder.Append("(");
                queryBuilder.Append(relationInformation.Expression, typeof(T), values[index]);
                queryBuilder.Append(")");
                if (index + 1 < values.Count)
                    queryBuilder.Append(" OR ");
            }

            Delegate delegateExpression = relationInformation.Expression.Compile();

            IList childValues = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(relationInformation.ChildType));
            MethodInfo method = typeof(SqlClient).GetMethod(nameof(ExecuteReader), BindingFlags.NonPublic | BindingFlags.Static);
            IEnumerable enumerableChildValues = (IEnumerable)method.MakeGenericMethod(relationInformation.ChildType).Invoke(null, new object[] { queryBuilder });

            foreach (object enumerableValue in enumerableChildValues)
                childValues.Add(enumerableValue);

            if (relationInformation.RelationType == RelationType.OneToMany)
            {
                foreach (T parentValue in values)
                {
                    IList children = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(relationInformation.ChildType));
                    foreach (object childValue in childValues)
                    {
                        if ((bool) delegateExpression.DynamicInvoke(parentValue, childValue))
                        {
                            children.Add(childValue);
                        }
                    }
                    typeof(T).GetProperty(relationInformation.Column)?.SetValue(parentValue, children);
                }
            }
            else if (relationInformation.RelationType == RelationType.OneToOne)
            {
                foreach (T parentValue in values)
                {
                    foreach (object childValue in childValues)
                    {
                        if ((bool) delegateExpression.DynamicInvoke(parentValue, childValue))
                        {
                            typeof(T).GetProperty(relationInformation.Column)?.SetValue(parentValue, childValue);
                            break;
                        }
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            return;
            var deleatge = relationInformation.Expression.Compile();
            
            //var funcComplied = func.Compile();

            foreach (object childValue in childValues)
            {
                
                foreach (T value in values)
                {
                    //if ( deleatge.DynamicInvoke(value, childValue);
                        ; //bool res = funcComplied(value, childValue);
                }
            }

        }

        
        private void LoadChildren<T>(RelationInformation relationInformation, object obj)
        {
            CheckInitialization(relationInformation.ChildType, out TableDefinition childTableDefinition);
            QueryBuilder queryBuilder = new QueryBuilder(childTableDefinition);
            
            BuildSelect(childTableDefinition, queryBuilder);
            queryBuilder.Append(" WHERE ");
            queryBuilder.Append(relationInformation.Expression, typeof(T), obj);

            IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(relationInformation.ChildType));

            MethodInfo method = typeof(SqlClient).GetMethod(nameof(ExecuteReader), BindingFlags.NonPublic | BindingFlags.Static);
            IEnumerable values = (IEnumerable)method.MakeGenericMethod(relationInformation.ChildType).Invoke(null, new object[] { queryBuilder });
           
            if (relationInformation.RelationType == RelationType.OneToOne)
            {
                foreach (object value in values)
                {
                    typeof(T).GetProperty(relationInformation.Column)?.SetValue(obj, value);
                    return;
                }
            }
            else if (relationInformation.RelationType == RelationType.OneToMany)
            {
                foreach (object value in values)
                    list.Add(value);
                typeof(T).GetProperty(relationInformation.Column)?.SetValue(obj, list);
            }
        }
    }
}