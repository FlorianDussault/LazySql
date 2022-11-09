using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using LazySql.Engine.Client.Query;
using LazySql.Engine.Definitions;
using LazySql.Engine.Enums;
using LazySql.Engine.Helpers;

// ReSharper disable once CheckNamespace
namespace LazySql.Engine.Client
{
    // ReSharper disable once ClassCannotBeInstantiated
    public sealed partial class LazyClient
    {
    //    public static IEnumerable<T> GetOld<T>(Expression<Func<T, bool>> expression = null, Expression<Func<T,object>> orderByExpression = null) where T : LazyBase => Instance.InternalGet(typeof(T), expression, orderByExpression).Cast<T>();

    //    private IEnumerable InternalGet(Type type, LambdaExpression expression, LambdaExpression orderByExpression = null)
    //    {
    //        CheckInitialization(type, out TableDefinition tableDefinition);
    //        QueryBuilder queryBuilder = new(tableDefinition);
    //        BuildSelect(tableDefinition, queryBuilder);

    //        if (expression != null)
    //            queryBuilder.Append(" WHERE ", expression);

    //        if(orderByExpression != null)
    //            queryBuilder.Append(" ORDER BY ", orderByExpression);


    //        if (tableDefinition.Relations.Count == 0)
    //        {
    //            foreach (object o in GetWithQuery(type, queryBuilder))
    //                yield return o;
    //            yield break;
    //        }

    //        List<object> values = GetWithQuery(type, queryBuilder).ToList();
    //        if (values.Count == 0) yield break;

    //        foreach (RelationInformation relation in tableDefinition.Relations)
    //            LoadChildren(type, relation, values);

    //        foreach (object value in values)
    //            yield return value;
    //    }

        public static LazyEnumerable<T> Get<T>(Expression<Func<T, bool>> expression = null) where T : LazyBase => Instance.InternalGet2<T>(typeof(T), expression);

        private LazyEnumerable<T> InternalGet2<T>(Type type, LambdaExpression expression) where T :LazyBase
        {
            return new(type, expression);

            //yield return lazyEnumerable.GetEnumerator();

            //CheckInitialization(type, out TableDefinition tableDefinition);
            //QueryBuilder queryBuilder = new(tableDefinition);
            //BuildSelect(tableDefinition, queryBuilder);

            //if (expression != null)
            //    queryBuilder.Append(" WHERE ", expression);

            //if (orderByExpression != null)
            //    queryBuilder.Append(" ORDER BY ", orderByExpression);


            //if (tableDefinition.Relations.Count == 0)
            //{
            //    foreach (object o in GetWithQuery(type, queryBuilder))
            //        yield return o;
            //    yield break;
            //}

            //List<object> values = GetWithQuery(type, queryBuilder).ToList();
            //if (values.Count == 0) yield break;

            //foreach (RelationInformation relation in tableDefinition.Relations)
            //    LoadChildren(type, relation, values);

            //foreach (object value in values)
            //    yield return value;
        }

        internal static IEnumerable<object> GetWithQuery(Type type, QueryBuilder queryBuilder)
        {
            CheckInitialization(type, out TableDefinition tableDefinition);
            if (tableDefinition.Relations.Count == 0)
            {
                foreach (object value in ExecuteReader(queryBuilder))
                    yield return value;
                yield break;
            }

            List<object> values = ExecuteReader(queryBuilder).ToList();
            if (values.Count == 0) yield break;

            foreach (RelationInformation relation in tableDefinition.Relations)
                LoadChildren(type, relation, values);

            foreach (object value in values)
                yield return value;
        }


        internal static void BuildSelect(TableDefinition tableDefinition, QueryBuilder queryBuilder, int? top = null)
        {
            tableDefinition.GetColumns(out IReadOnlyList<ColumnDefinition> allColumns, out _, out _, out _);
            queryBuilder.Append($"SELECT {(top!= null ? $" TOP {top} " : string.Empty)} {string.Join(", ", allColumns.Where(c => c.Column.SqlType != SqlType.Children).Select(c => c.Column.SqlColumnName))} FROM {tableDefinition.Table.TableName}");

        }

        internal static void LoadChildren(Type parentType, RelationInformation relationInformation, List<object> values)
        {
            if (values.Count == 0) return;
            CheckInitialization(relationInformation.ChildType, out TableDefinition childTableDefinition);

            QueryBuilder queryBuilder = new(childTableDefinition);

            BuildSelect(childTableDefinition, queryBuilder);
            queryBuilder.Append(" WHERE ");

            for (var index = 0; index < values.Count; index++)
            {
                queryBuilder.Append("(");
                queryBuilder.Append(relationInformation.Expression, parentType, values[index]);
                queryBuilder.Append(")");
                if (index + 1 < values.Count)
                    queryBuilder.Append(" OR ");
            }

            Delegate delegateExpression = relationInformation.Expression.Compile();
            IEnumerable enumerableChildValues = ReflectionHelper.InvokeStaticMethod<IEnumerable>(typeof(LazyClient),
                nameof(GetWithQuery), new object[] { relationInformation.ChildType, queryBuilder });

            //IEnumerable enumerableChildValues = ReflectionHelper.InvokeStaticMethod<IEnumerable>(typeof(LazyClient), nameof(ExecuteReader),  new object[] { queryBuilder });

            IList childValues = ReflectionHelper.CreateList(relationInformation.ChildType);
            foreach (object enumerableValue in enumerableChildValues)
                childValues.Add(enumerableValue);

            if (relationInformation.RelationType == RelationType.OneToMany)
            {
                foreach (object parentValue in values)
                {
                    IList children = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(relationInformation.ChildType));
                    foreach (object childValue in childValues)
                    {
                        if ((bool)delegateExpression.DynamicInvoke(parentValue, childValue))
                        {
                            children.Add(childValue);
                        }
                    }
                    parentType.GetProperty(relationInformation.Column)?.SetValue(parentValue, children);
                }
            }
            else if (relationInformation.RelationType == RelationType.OneToOne)
            {
                foreach (object parentValue in values)
                {
                    foreach (object childValue in childValues)
                    {
                        if ((bool)delegateExpression.DynamicInvoke(parentValue, childValue))
                        {
                            parentType.GetProperty(relationInformation.Column)?.SetValue(parentValue, childValue);
                            break;
                        }
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}