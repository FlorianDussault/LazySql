namespace LazySql.Transaction;

public class LazyTransaction : IDisposable
{
    internal SqlConnector SqlConnector { get; }
    internal SqlTransaction SqlTransaction { get; }
    public string TransactionName { get; }

    internal LazyTransaction(SqlConnector sqlConnector, SqlTransaction sqlTransaction, string transactionName)
    {
        SqlConnector = sqlConnector;
        SqlTransaction = sqlTransaction;
        TransactionName = transactionName;
    }

    public void Dispose()
    {
        SqlConnector?.Dispose();
        SqlTransaction?.Dispose();
    }

    public void Commit() => SqlTransaction.Commit();
    public void Rollback() => SqlTransaction.Rollback();

    #region Select
    public ILazyEnumerable<T> Select<T>() => LazyClient.Instance.InternalSelect<T>(null, null, null, null, this);
    public ILazyEnumerable<T> Select<T>(string tableName) => LazyClient.Instance.InternalSelect<T>(null, tableName, null, null, this);
    public ILazyEnumerable<T> Select<T>(string schema, string tableName) => LazyClient.Instance.InternalSelect<T>(schema, tableName, null, null, this);
    public ILazyEnumerable<T> Select<T>(Expression<Func<T, bool>> where) => LazyClient.Instance.InternalSelect<T>(null, null, where, null, this);
    public ILazyEnumerable<T> Select<T>(string tableName, Expression<Func<T, bool>> where) =>
        LazyClient.Instance.InternalSelect<T>(null, tableName, where, null, this);
    public ILazyEnumerable<T> Select<T>(string schema, string tableName, Expression<Func<T, bool>> where) =>
        LazyClient.Instance.InternalSelect<T>(schema, tableName, where, null, this);
    public ILazyEnumerable<T> Select<T>(SqlQuery sqlQuery) => LazyClient.Instance.InternalSelect<T>(null, null, null, sqlQuery, this);
    public ILazyEnumerable<T> Select<T>(string tableName, SqlQuery sqlQuery) => LazyClient.Instance.InternalSelect<T>(null, tableName, null, sqlQuery, this);
    public ILazyEnumerable<T> Select<T>(string schema, string tableName, SqlQuery sqlQuery) => LazyClient.Instance.InternalSelect<T>(schema, tableName, null, sqlQuery, this);

    #endregion

    #region Insert

    public int Insert(object obj) =>
        LazyClient.Instance.InternalInsert(null, null, obj, null, Array.Empty<string>(), this);

    public int Insert(string tableName, object obj) =>
        LazyClient.Instance.InternalInsert(null, tableName, obj, null, Array.Empty<string>(), this);

    public int Insert(string schema, string tableName, object obj) =>
        LazyClient.Instance.InternalInsert(schema, tableName, obj, null, Array.Empty<string>(), this);

    public int Insert(object obj, string autoIncrementColumn, params string[] excludedColumns) =>
        LazyClient.Instance.InternalInsert(null, null, obj, autoIncrementColumn, excludedColumns, this);

    public int Insert(string tableName, object obj, string autoIncrementColumn, params string[] excludedColumns) =>
        LazyClient.Instance.InternalInsert(null, tableName, obj, autoIncrementColumn, excludedColumns, this);

    public int Insert(string schema, string tableName, object obj, string autoIncrementColumn,
        params string[] excludedColumns) =>
        LazyClient.Instance.InternalInsert(schema, tableName, obj, autoIncrementColumn, excludedColumns, this);

    public int Insert(object obj, string autoIncrementColumn) =>
        LazyClient.Instance.InternalInsert(null, null, obj, autoIncrementColumn, Array.Empty<string>(), this);

    public int Insert(string tableName, object obj, string autoIncrementColumn) =>
        LazyClient.Instance.InternalInsert(null, tableName, obj, autoIncrementColumn, Array.Empty<string>(), this);

    public int Insert(string schema, string tableName, object obj, string autoIncrementColumn) =>
        LazyClient.Instance.InternalInsert(schema, tableName, obj, autoIncrementColumn, Array.Empty<string>(), this);

    #endregion

    #region Update
    public int Update<T>(T obj) => LazyClient.Instance.InternalUpdate(obj.GetType(), null, null, obj,  null, null, null, null);
    public int Update<T>(string tableName, T obj) => LazyClient.Instance.InternalUpdate(obj.GetType(), null, tableName, obj, null, null, null, null);

    public int Update<T>(string schema, string tableName, T obj) =>
        LazyClient.Instance.InternalUpdate(obj.GetType(), schema, tableName, obj, null, null, null, null);

    public int Update<T>(T obj, params string[] excludedColumns) => LazyClient.Instance.InternalUpdate(obj.GetType(), null, null, obj, null, null, excludedColumns, null);
    public int Update<T>(string tableName, T obj, params string[] excludedColumns) => LazyClient.Instance.InternalUpdate(obj.GetType(), null, tableName, obj, null, null, excludedColumns, null);
    public int Update<T>(string schema, string tableName, T obj, params string[] excludedColumns) => LazyClient.Instance.InternalUpdate(obj.GetType(), schema, tableName, obj, null, null, excludedColumns, null);

    public int Update<T>(T obj, Expression<Func<T, bool>> where) => LazyClient.Instance.InternalUpdate(obj.GetType(), null, null, obj, where, null, null, null);
    public int Update<T>(string tableName, T obj, Expression<Func<T, bool>> where) => LazyClient.Instance.InternalUpdate(obj.GetType(), null, tableName, obj, where, null, null, null);
    public int Update<T>(string schema, string tableName, T obj, Expression<Func<T, bool>> where) => LazyClient.Instance.InternalUpdate(obj.GetType(), schema, tableName, obj, where, null, null, null);

    public int Update<T>(T obj, Expression<Func<T, bool>> where, params string[] excludedColumns) => LazyClient.Instance.InternalUpdate(obj.GetType(), null, null, obj, where, null, excludedColumns, null);
    public int Update<T>(string tableName, T obj, Expression<Func<T, bool>> where, params string[] excludedColumns) => LazyClient.Instance.InternalUpdate(obj.GetType(), null, tableName, obj, where, null, excludedColumns, null);
    public int Update<T>(string schema, string tableName, T obj, Expression<Func<T, bool>> where, params string[] excludedColumns) => LazyClient.Instance.InternalUpdate(obj.GetType(), schema, tableName, obj, where, null, excludedColumns, null);

    public int Update<T>(T obj, SqlQuery where) => LazyClient.Instance.InternalUpdate(obj.GetType(), null, null, obj, null, where, null, null);
    public int Update<T>(string tableName, T obj, SqlQuery where) => LazyClient.Instance.InternalUpdate(obj.GetType(), null, tableName, obj, null, where, null, null);
    public int Update<T>(string schema, string tableName, T obj, SqlQuery where) => LazyClient.Instance.InternalUpdate(obj.GetType(), schema, tableName, obj, null, where, null, null);

    public int Update<T>(T obj, SqlQuery where, params string[] excludedColumns) => LazyClient.Instance.InternalUpdate(obj.GetType(), null, null, obj, null, where, excludedColumns, null);
    public int Update<T>(string tableName, T obj, SqlQuery where, params string[] excludedColumns) => LazyClient.Instance.InternalUpdate(obj.GetType(), null, tableName, obj, null, where, excludedColumns, null);
    public int Update<T>(string schema, string tableName, T obj, SqlQuery where, params string[] excludedColumns) => LazyClient.Instance.InternalUpdate(obj.GetType(), schema, tableName, obj, null, where, excludedColumns, null);

    #endregion

    #region Delete
    public int Delete(string schema, string tableName) => LazyClient.Instance.InternalDelete(schema, tableName, null, this);

    public int Delete(object obj) => LazyClient.Instance.InternalDelete(null, null, obj, this);
    public int Delete(string tableName, object obj) => LazyClient.Instance.InternalDelete(null, tableName, obj, this);
    public int Delete(string schema, string tableName, object obj) => LazyClient.Instance.InternalDelete(schema, tableName, obj, this);


    public int Delete<T>(Expression<Func<T, bool>> where) => LazyClient.Instance.InternalDelete(null, null, typeof(T), where, null, this);

    public int Delete<T>(string tableName, Expression<Func<T, bool>> where) => LazyClient.Instance.InternalDelete(null, tableName, typeof(T), where, null, this);

    public int Delete<T>(string schema, string tableName, Expression<Func<T, bool>> where) => LazyClient.Instance.InternalDelete(schema, tableName, typeof(T), where, null, this);

    public int Delete<T>(SqlQuery where = null) => LazyClient.Instance.InternalDelete(null, null, typeof(T), null, where, this);

    public int Delete<T>(string tableName, SqlQuery where = null) => LazyClient.Instance.InternalDelete(null, tableName, typeof(T), null, where, this);

    public int Delete<T>(string schema, string tableName, SqlQuery where = null) => LazyClient.Instance.InternalDelete(schema, tableName, typeof(T), null, where, this);

    #endregion
}