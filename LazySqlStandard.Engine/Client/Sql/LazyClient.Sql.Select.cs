// ReSharper disable once CheckNamespace
using System.Linq.Expressions;

namespace LazySql.Engine.Client;

// ReSharper disable once ClassCannotBeInstantiated
public sealed partial class LazyClient
{
    //private IEnumerable<T> InternalSelect<T>(string query, SqlArguments sqlArguments)
    //{
    //    using SqlConnector sqlConnector = Open();
    //    using SqlDataReader sqlDataReader = sqlConnector.ExecuteQuery(query, sqlArguments);
    //    DataTable dataTable = new();
    //    dataTable.Load(sqlDataReader);

    //    if (typeof(T) == typeof(DynamicObject))
    //    {
    //        return dataTable.ToDynamic().Cast<T>();
    //    }
    //    if (typeof(T) == typeof(LazyBase))
    //    {
    //        return dataTable.ToLazyObject<T>();
    //    }

    //    throw new NotImplementedException();
    //}
    public static ILazyEnumerable<T> Select<T>(string tableName = null) => Instance.InternalSelect<T>(tableName);

    private ILazyEnumerable<T> InternalSelect<T>(string tableName) => new LazyEnumerable2<T>(tableName);

    // Select<T>("table_name").Where(a=> a.Id = 10).Top(10)

}

public interface ILazyEnumerable<T> : IEnumerable<T>
{
    ILazyEnumerable<T> Where(Expression<Func<T, bool>> whereExpression);
    ILazyEnumerable<T> OrderByAsc(Expression<Func<T, object>> orderByExpression);
    ILazyEnumerable<T> OrderByDesc(Expression<Func<T, object>> orderByExpression);
    ILazyEnumerable<T> Top(int top);
}

internal class LazyEnumerable2<T> : ILazyEnumerable<T>
{
    private readonly string _tableName;
    private Expression _whereExpression;
    private readonly List<(bool orderByAsc, Expression expression)> _orderByExpressions;
    private int? _top = null;
    public LazyEnumerable2(string tableName)
    {
        _tableName = tableName;
        _orderByExpressions = new List<(bool orderByAsc, Expression expression)>();
    }

    public ILazyEnumerable<T> Where(Expression<Func<T, bool>> whereExpression)
    {
        _whereExpression = whereExpression;
        return this;
    }

    public ILazyEnumerable<T> OrderByAsc(Expression<Func<T, object>> orderByExpression)
    {
        _orderByExpressions.Add((true, orderByExpression));
        return this;
    }

    public ILazyEnumerable<T> OrderByDesc(Expression<Func<T, object>> orderByExpression)
    {
        _orderByExpressions.Add((false, orderByExpression));
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
        LazyClient.CheckInitialization(typeof(T), out TableDefinition tableDefinition);

        QueryBuilder queryBuilder = new(tableDefinition);
        LazyClient.BuildSelect(tableDefinition, queryBuilder, _top);

        if (_whereExpression != null)
            queryBuilder.Append(" WHERE ", _whereExpression);

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

        if (tableDefinition.Relations.Count == 0)
        {
            foreach (object o in LazyClient.GetWithQuery(typeof(T), queryBuilder))
                yield return o;
            yield break;
        }

        List<object> values = LazyClient.GetWithQuery(typeof(T), queryBuilder).ToList();
        if (values.Count == 0) yield break;

        foreach (RelationInformation relation in tableDefinition.Relations)
            LazyClient.LoadChildren(typeof(T), relation, values);

        foreach (object value in values)
            yield return value;


       
    }
}