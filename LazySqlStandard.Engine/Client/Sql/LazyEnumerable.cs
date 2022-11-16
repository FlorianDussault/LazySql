

// ReSharper disable once CheckNamespace
namespace LazySql.Engine.Client;

internal class LazyEnumerable<T> : ILazyEnumerable<T>
{
    private readonly string _tableName;
    private readonly TableDefinition _tableDefinition;
    private Expression _whereExpression;
    private (string whereSql, SqlArguments arguments) _whereSql;
    private readonly List<(bool orderByAsc, Expression expression)> _orderByExpressions;
    private int? _top = null;

    public LazyEnumerable(string tableName)
    {
        LazyClient.CheckInitialization(typeof(T), out _tableDefinition);
        _tableName = tableName;
        _orderByExpressions = new List<(bool orderByAsc, Expression expression)>();
    }

    public ILazyEnumerable<T> Where(Expression<Func<T, bool>> whereExpression)
    {
        _whereExpression = whereExpression;
        return this;
    }

    public ILazyEnumerable<T> Where(string whereSql, SqlArguments sqlArguments)
    {
        _whereSql = (whereSql, sqlArguments);
        return this;
    }

    public ILazyEnumerable<T> OrderByAsc(Expression<Func<T, object>> orderByExpression)
    {
        _orderByExpressions.Add((true, orderByExpression));
        return this;
    }

    public ILazyEnumerable<T> OrderByAsc(params string[] columns)
    {
        foreach (string column in columns)
            _orderByExpressions.Add((true, Expression.Constant(column)));
        return this;
    }

    public ILazyEnumerable<T> OrderByDesc(Expression<Func<T, object>> orderByExpression)
    {
        _orderByExpressions.Add((false, orderByExpression));
        return this;
    }

    public ILazyEnumerable<T> OrderByDesc(params string[] columns)
    {
        foreach (string column in columns)
            _orderByExpressions.Add((false, Expression.Constant(column)));
        return this;
    }

    public ILazyEnumerable<T> Top(int top)
    {
        _top = top;
        return this;
    }

    public IEnumerator<T> GetEnumerator() => Execute().Cast<T>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private IEnumerable Execute()
    {
        QueryBuilder queryBuilder = new(_tableDefinition);
        LazyClient.BuildSelect(_tableDefinition, _tableName, queryBuilder, _top);

        if (_whereExpression != null)
            queryBuilder.Append(" WHERE ", _whereExpression);
        if (_whereSql.whereSql != null)
        {
            queryBuilder.Append(" WHERE ");
            queryBuilder.Append(_whereSql.whereSql);
            if (_whereSql.arguments != null)
                queryBuilder.AddSqlArguments(_whereSql.arguments);
        }


        if (_orderByExpressions.Count > 0)
        {
            queryBuilder.Append(" ORDER BY ");
            foreach ((bool isLast, (bool orderByAsc, Expression expression) value) valueTuple in _orderByExpressions
                         .ForeachWithLast())
            {
                queryBuilder.Append(valueTuple.value.expression);
                queryBuilder.Append(valueTuple.value.orderByAsc ? " ASC " : " DESC ");
                if (!valueTuple.isLast)
                    queryBuilder.Append(", ");
            }
        }

        if (_tableDefinition.Relations.Count == 0)
        {
            foreach (object o in LazyClient.GetWithQuery(typeof(T), queryBuilder))
                yield return o;
            yield break;
        }

        List<object> values = LazyClient.GetWithQuery(typeof(T), queryBuilder).ToList();
        if (values.Count == 0) yield break;

        foreach (RelationInformation relation in _tableDefinition.Relations)
            LazyClient.LoadChildren(typeof(T), relation, values);

        foreach (object value in values)
            yield return value;



    }
}

//public class LazyEnumerable<T> : IEnumerable<T>
//{
//    private readonly Type _type;
//    private readonly LambdaExpression _expression;
//    private readonly List<(bool orderByAsc, Expression expression)> _orderByExpressions;
//    private int? _top;

//    internal LazyEnumerable(Type type, LambdaExpression expression)
//    {
//        _type = type;
//        _expression = expression;
//        _orderByExpressions = new();
//        _top = null;
//    }

//    public LazyEnumerable<T> OrderByAsc(Expression<Func<T, object>> orderByExpression)
//    {
//        _orderByExpressions.Add((true, orderByExpression));
//        return this;
//    }

//    public LazyEnumerable<T> OrderByDesc(Expression<Func<T, object>> orderByExpression)
//    {
//        _orderByExpressions.Add((false, orderByExpression));
//        return this;
//    }

//    public LazyEnumerable<T> Top(int top)
//    {
//        _top = top;
//        return this;
//    }

//    public IEnumerator<T> GetEnumerator()
//    {
//        return Execute().Cast<T>().GetEnumerator();
//    }

//    IEnumerator IEnumerable.GetEnumerator()
//    {
//        return GetEnumerator();
//    }

//    private IEnumerable Execute()
//    {
//        LazyClient.CheckInitialization(_type, out TableDefinition tableDefinition);
//        QueryBuilder queryBuilder = new(tableDefinition);
//        LazyClient.BuildSelect(tableDefinition, null, queryBuilder, _top);

//        if (_expression != null)
//            queryBuilder.Append(" WHERE ", _expression);

//        if (_orderByExpressions.Count > 0)
//        {
//            queryBuilder.Append(" ORDER BY ");
//            foreach ((bool isLast, (bool orderByAsc, Expression expression) value) valueTuple in _orderByExpressions.ForeachWithLast())
//            {
//                queryBuilder.Append(valueTuple.value.expression);
//                queryBuilder.Append(valueTuple.value.orderByAsc ? " ASC " : " DESC ");
//                if (!valueTuple.isLast)
//                    queryBuilder.Append(", ");
//            }
//        }

//        if (tableDefinition.Relations.Count == 0)
//        {
//            foreach (object o in LazyClient.GetWithQuery(_type, queryBuilder))
//                yield return o;
//            yield break;
//        }

//        List<object> values = LazyClient.GetWithQuery(_type, queryBuilder).ToList();
//        if (values.Count == 0) yield break;

//        foreach (RelationInformation relation in tableDefinition.Relations)
//            LazyClient.LoadChildren(_type, relation, values);

//        foreach (object value in values)
//            yield return value;
//    }

//}