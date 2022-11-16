namespace LazySql.Engine.Client;

public interface ILazyEnumerable<T> : IEnumerable<T>
{
    ILazyEnumerable<T> Where(Expression<Func<T, bool>> whereExpression);
    ILazyEnumerable<T> Where(string sql, SqlArguments sqlArguments = null);
    ILazyEnumerable<T> OrderByAsc(Expression<Func<T, object>> orderByExpression);
    ILazyEnumerable<T> OrderByAsc(params string[] columns);
    ILazyEnumerable<T> OrderByDesc(Expression<Func<T, object>> orderByExpression);
    ILazyEnumerable<T> OrderByDesc(params string[] columns);
    ILazyEnumerable<T> Top(int top);
}