namespace LazySql.Engine.Client;

/// <summary>
/// LazyClient
/// </summary>
public sealed partial class LazyClient
{
    private IReadOnlyCollection<TableDefinition> _tables;

    #region Singleton
    private static LazyClient Instance { get; } = new LazyClient();

    private LazyClient()
    {
    }

    #endregion
}