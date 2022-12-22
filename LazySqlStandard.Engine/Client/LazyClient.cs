using LazySql.Transaction;

namespace LazySql;

/// <summary>
/// LazyClient
/// </summary>
public sealed partial class LazyClient
{
    private List<ITableDefinition> _tables;

    #region Singleton
    internal static LazyClient Instance { get; } = new();

    private LazyClient()
    {
    }

    #endregion
    
    
    #region Transactions

    public static LazyTransaction BeginTransaction(string transactionName) => Instance.InternalBeginTransaction(transactionName);
    private LazyTransaction InternalBeginTransaction(string transactionName) => Open().BeginTransaction(transactionName);
    public static LazyTransaction BeginTransaction(IsoLevel isolationLevel, string transactionName) => Instance.InternalBeginTransaction(isolationLevel, transactionName);

    private LazyTransaction InternalBeginTransaction(IsoLevel isolationLevel, string transactionName) => Open().BeginTransaction(isolationLevel, transactionName);

    #endregion
}