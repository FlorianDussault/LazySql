namespace LazySql;

public interface ILazyEnumerable<T> : IEnumerable<T>
{
    ILazyEnumerable<T> OrderByAsc(Expression<Func<T, object>> orderByExpression);
    ILazyEnumerable<T> OrderByAsc(params string[] columns);
    ILazyEnumerable<T> OrderByDesc(Expression<Func<T, object>> orderByExpression);
    ILazyEnumerable<T> OrderByDesc(params string[] columns);

    ILazyEnumerable<T> GroupBy(params Expression<Func<T, object>>[] groupByExpressions);

    ILazyEnumerable<T> Top(int top);

    ILazyEnumerable<T> Columns(params Expression<Func<T, object>>[] columnsExpressions);

    int Count { get; }
}