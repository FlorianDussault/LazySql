namespace LazySql;

public interface ILazyEnumerable<T> : IEnumerable<T>
{
    ILazyEnumerable<T> OrderByAsc(Expression<Func<T, object>> orderByExpression);
    ILazyEnumerable<T> OrderByAsc(params string[] columns);
    ILazyEnumerable<T> OrderByDesc(Expression<Func<T, object>> orderByExpression);
    ILazyEnumerable<T> OrderByDesc(params string[] columns);
    ILazyEnumerable<T> Top(int top);
}