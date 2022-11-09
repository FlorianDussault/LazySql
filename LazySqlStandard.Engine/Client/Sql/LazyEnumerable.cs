using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LazySql.Engine.Client.Query;
using LazySql.Engine.Definitions;
using LazySql.Engine.Helpers;

// ReSharper disable once CheckNamespace
namespace LazySql.Engine.Client
{
    public class LazyEnumerable<T> : IEnumerable<T>
    {
        private readonly Type _type;
        private readonly LambdaExpression _expression;
        private readonly List<(bool orderByAsc, Expression expression)> _orderByExpressions;
        private int? _top;

        internal LazyEnumerable(Type type, LambdaExpression expression)
        {
            _type = type;
            _expression = expression;
            _orderByExpressions = new();
            _top = null;
        }

        public LazyEnumerable<T> OrderByAsc(Expression<Func<T, object>> orderByExpression)
        {
            _orderByExpressions.Add((true, orderByExpression));
            return this;
        }

        public LazyEnumerable<T> OrderByDesc(Expression<Func<T, object>> orderByExpression)
        {
            _orderByExpressions.Add((false, orderByExpression));
            return this;
        }

        public LazyEnumerable<T> Top(int top)
        {
            _top = top;
            return this;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Execute().Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable Execute()
        {
            LazyClient.CheckInitialization(_type, out TableDefinition tableDefinition);
            QueryBuilder queryBuilder = new(tableDefinition);
            LazyClient.BuildSelect(tableDefinition, queryBuilder, _top);

            if (_expression != null)
                queryBuilder.Append(" WHERE ", _expression);

            if (_orderByExpressions.Count > 0)
            {
                queryBuilder.Append(" ORDER BY ");
                foreach ((bool isLast, (bool orderByAsc, Expression expression) value) valueTuple in _orderByExpressions.ForeachWithLast())
                {
                    queryBuilder.Append(valueTuple.value.expression);
                    queryBuilder.Append(valueTuple.value.orderByAsc ? " ASC " : " DESC ");
                    if (!valueTuple.isLast)
                        queryBuilder.Append(", ");
                }
            }

            if (tableDefinition.Relations.Count == 0)
            {
                foreach (object o in LazyClient.GetWithQuery(_type, queryBuilder))
                    yield return o;
                yield break;
            }

            List<object> values = LazyClient.GetWithQuery(_type, queryBuilder).ToList();
            if (values.Count == 0) yield break;

            foreach (RelationInformation relation in tableDefinition.Relations)
                LazyClient.LoadChildren(_type, relation, values);

            foreach (object value in values)
                yield return value;
        }
        
    }
}